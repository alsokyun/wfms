using DevExpress.Mvvm;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class FileMngViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // 파일관련 전역변수 - 환경설정으로 관리해야함
        string dir_name = @"D:\GTI\FILE";

        //파일다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread download_thread;
        string target_file_path;
        string source_file_path;

        //그리드
        TableView gv;




        // 생성자
        public FileMngViewModel()
        {
            // 초기로딩처리
            LoadedCommand = new DelegateCommand<object>(OnLoaded);


            // 파일인포리스트
            Items = new ObservableCollection<FileInfo>();

            //파일저장버튼 이벤트
            SaveCmd = new RelayCommand<object>(delegate (object obj)
            {
                int ret = UploadFileList(this.FIL_SEQ);
                this.FIL_SEQ = ret.ToString();// 생성된 첨부파일아이디 반환 -> 뷰쪽으로 바인딩 -> 부모창에서 접근가능
            });

            //파일삭제버튼 이벤트
            RemoveCmd = new RelayCommand<object>(delegate (object obj)
            {
                //gv.DeleteRow(gv.FocusedRowHandle);
            });

            //기존파일 다운로드버튼 이벤트
            DownloadCmd = new RelayCommand<object>(delegate (object obj)
            {
                string file_name = obj as string;
                string dir_name = "";
                source_file_path = System.IO.Path.Combine(dir_name, file_name);


                //파일다운로드
                saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "저장경로를 지정하세요.";
                //초기 파일명 지정
                saveFileDialog.FileName = file_name;
                saveFileDialog.OverwritePrompt = true;
                //saveFileDialog.Filter = "Excel|*.xlsx";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    target_file_path = saveFileDialog.FileName;
                    download_thread = new Thread(new ThreadStart(DownloadFX));
                    download_thread.Start();
                }



            });

            
            //파일찾기버튼 이벤트
            FindFileCmd = new RelayCommand<object>(delegate (object obj)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        //lbFiles.Items.Add(Path.GetFileName(filename)); //파일명
                        FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //파일인포
                        foreach (FileInfo fi in files)
                        {
                            Items.Add(fi);
                        }
                    }

                }
            });


            // 초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileDtl");
            param.Add("FIL_SEQ", this.FIL_SEQ);

            dt = BizUtil.SelectList(param);

            Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string file_name2 = row["UPF_NAM"].ToString();
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);

                FileInfo fi = new FileInfo(file_path2);
                Items.Add(fi);
            }


        }

        //초기로딩 객체가져오기
        private void OnLoaded(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;

            //그리드뷰인스턴스
            gv = values[0] as TableView;
        }





        #region =========  프로퍼티 ===========
        public virtual ObservableCollection<FileInfo> Items { get; set; }
        public DelegateCommand<object> LoadedCommand { get; set; }
        public RelayCommand<object> SaveCmd { get; set; } //뷰에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> RemoveCmd { get; set; } 
        public RelayCommand<object> FindFileCmd { get; set; }
        public RelayCommand<object> DownloadCmd { get; set; }
        
        private string __FIL_SEQ;


        public string FIL_SEQ
        {
            get { return __FIL_SEQ; }
            set
            {
                this.__FIL_SEQ = value;
                RaisePropertyChanged("FIL_SEQ");
            }
        }

        #endregion





        #region ========== 드래그앤드롭 처리함수 ==============

        public void DragRecordOver(bool dragEnter)
        {
            //Method must exist.
            Console.WriteLine("DragRecordOver");
        }


        public void DropRecord(List<FileInfo> filesData)
        {
            foreach (FileInfo fi in filesData)
                if (!Items.Any(x => x.FullName == fi.FullName))
                    Items.Add(fi);
        }

        public void StartRecordDrag(bool allowDrag)
        {
            //Method is optional
            Console.WriteLine("StartRecordDrag");
        }
        public void GiveRecordDragFeedback(bool dragMove)
        {
            //Method must exist.
            Console.WriteLine("GiveRecordDragFeedback");
        }

        public void CompleteRecordDragDrop(string filePath)
        {
            Console.WriteLine("CompleteRecordDragDrop");
            //File.WriteAllBytes(filePath, fileByte);
        }

        #endregion














        /// <summary>
        /// 물리적파일을 업로드한후, 파일테이블에 등록하고, _FIL_SEQ를 생성한다
        /// </summary>
        /// <returns></returns>
        private int UploadFileList(string _FIL_SEQ)
        {
            int file_seq = -1;
            List<Hashtable> FILE_DTL_LIST = new List<Hashtable>();//첨부파일상세 리스트
            Hashtable FILE_DTL = new Hashtable();//첨부파일상세



            // 0.첨부파일 수정모드인경우, 해당파일 모두삭제
            if (__FIL_SEQ != null)
            {
                try
                {
                    file_seq = Convert.ToInt16(_FIL_SEQ);
                }
                catch (Exception) { }
            }


            Hashtable param = new Hashtable();
            if (file_seq > 0)
            {
                //파일마스터 관련파일 모두 일단 삭제
                param.Add("sqlId", "DeleteFileSeq");
                BizUtil.Update(param);

                //실제파일삭제 구현(어려울시 파일남겨둘수도...)
            }



            // 1.물리적파일 저장
            if (!System.IO.Directory.Exists(dir_name))
            {
                System.IO.Directory.CreateDirectory(dir_name);
            }
            int seq = 0;
            foreach (FileInfo fi in Items)
            {
                string file_name = fi.Name;
                string file_name2 = DateTime.Now.ToString("yyyyMMddHHmmssff") + (seq++).ToString() +  fi.Extension; //저장되는파일이름
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);


                try
                {
                    fi.CopyTo(file_path2, true);
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.ToString());
                }


                //파일상세정보 추가
                FILE_DTL.Clear();
                FILE_DTL.Add("SEQ", seq++);
                FILE_DTL.Add("FIL_RST", "");//사진파일해상도
                FILE_DTL.Add("FIL_SIZ", fi.Length / 1000);
                FILE_DTL.Add("FIL_TYP", fi.Extension.ToString().Replace(".",""));
                FILE_DTL.Add("UPF_NAM", file_path2);
                FILE_DTL.Add("DWN_NAM", file_name);
                FILE_DTL.Add("CUR_TFS", "1");//?
                
                FILE_DTL_LIST.Add(FILE_DTL);


            }


            // 2.첨부파일 등록
            file_seq = SaveFileList(file_seq, FILE_DTL_LIST);





            return file_seq;
        }





        /// <summary>
        /// 첨부파일 등록
        /// </summary>
        /// <param name="file_seq"></param>
        /// <param name="fILE_DTL_LIST"></param>
        private int SaveFileList(int file_seq, List<Hashtable> FILE_DTL_LIST)
        {
            Hashtable param = new Hashtable();

            //1.마스터신규인 경우 
            if (file_seq < 0)
            {
                //마스터등록(파일마스터 채번)
                param.Clear();
                param.Add("sqlId", "InsertFileMst");
                param.Add("FIL_SEQ", file_seq); 
                file_seq = BizUtil.InsertR(param);
            }



            //2.디테일등록
            foreach (Hashtable dtl in FILE_DTL_LIST)
            {
                try
                {
                    dtl.Add("sqlId", "InsertFileDtl");
                }
                catch (Exception){}
                dtl["FIL_SEQ"] = file_seq;

                BizUtil.InsertR(dtl);
            }


            return file_seq;
        }



        /// <summary>
        /// 파일다운로드 쓰레드 핸들러
        /// </summary>
        private void DownloadFX()
        {
            try
            {
                System.IO.File.Copy(source_file_path, target_file_path);
                Messages.ShowOkMsgBox();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }



    }
}
