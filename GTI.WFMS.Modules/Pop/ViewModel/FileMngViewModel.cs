using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
        Thread upload_thread;
        string target_file_path;
        string source_file_path;
        int ret_fil_seq;   //업로드후 생성된 첨부파일ID

        //화면컴포넌트
        TableView gv;
        FileMngView fileMngView;
        TextBlock txtFIL_SEQ;
        Button btnClose;


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

                upload_thread = new Thread(new ThreadStart(UploadFileListFX));
                upload_thread.Start();

            });

            //파일삭제버튼 이벤트
            RemoveCmd = new RelayCommand<object>(delegate (object obj)
            {
                string seq = obj as string;
                string file_name = "";
                string del_file_path = "";



                //0.첨부파일정보
                Hashtable param = new Hashtable();
                param.Add("FIL_SEQ", this.FIL_SEQ);
                param.Add("SEQ", Convert.ToInt16(seq));
                param.Add("sqlId", "SelectFileDtl");
                DataTable dt = BizUtil.SelectList(param);
                try
                {
                    file_name = dt.Rows[0]["DWN_NAM"].ToString();
                }
                catch (Exception){}


                //1.첨부파일상세테이블 삭제
                param["sqlId"] = "DeleteFileSeq";
                BizUtil.Update(param);


                //2.물리적파일 삭제
                del_file_path = System.IO.Path.Combine(dir_name, file_name);
                try
                {
                    FileInfo fi = new FileInfo(del_file_path);
                    fi.Delete();
                }
                catch (Exception ex)
                {
                    Messages.ShowInfoMsgBox(ex.ToString());
                }

                //삭제성공
                Messages.ShowOkMsgBox();
                InitModel();
            });




            //기존파일 다운로드버튼 이벤트
            DownloadCmd = new RelayCommand<object>(delegate (object obj)
            {
                string file_name = obj as string;
                source_file_path = System.IO.Path.Combine(dir_name, file_name);


                //파일다운로드
                saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "저장경로를 지정하세요.";
                //초기 파일명 지정
                saveFileDialog.FileName = file_name;
                saveFileDialog.OverwritePrompt = true;
                //saveFileDialog.Filter = "Excel|*.xlsx";
                //saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Filter = "All files (*.*)|*.*";

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
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //파일인포
                    //lbFiles.Items.Add(Path.GetFileName(filename)); //파일명
                    foreach (FileInfo fi in files)
                    {
                        Items.Add(fi);
                    }
                }
            });


            //윈도우 마우스드래그
            WindowMoveCommand = new DelegateCommand<object>(delegate (object obj)
            {
                try
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        if (fileMngView.WindowState == WindowState.Maximized)
                        {
                            fileMngView.Top = Mouse.GetPosition(fileMngView).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                            fileMngView.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(fileMngView).X + 20;

                            fileMngView.WindowState = WindowState.Normal;
                        }
                        fileMngView.DragMove();
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBoxLog(ex);
                }
            });



        }

        //초기로딩 객체가져오기
        private void OnLoaded(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;

            //그리드뷰인스턴스
            fileMngView = values[0] as FileMngView;
            gv = fileMngView.gv;
            
            txtFIL_SEQ = fileMngView.txtFIL_SEQ;
            btnClose = fileMngView.btnClose;


            // 초기조회
            InitModel();

        }

        //초기모델조회
        private void InitModel()
        {
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileDtl");
            param.Add("FIL_SEQ", txtFIL_SEQ.Text);

            dt = BizUtil.SelectList(param);

            Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string file_name2 = row["DWN_NAM"].ToString();
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);

                FileInfo fi = new FileInfo(file_path2);
                fi.Attributes = new FileAttributes();
                if (fi.Exists)
                {
                    Items.Add(fi);
                }
            }
        }





        #region =========  프로퍼티 ===========
        public virtual ObservableCollection<FileInfo> Items { get; set; }
        public DelegateCommand<object> LoadedCommand { get; set; }
        public RelayCommand<object> SaveCmd { get; set; } //뷰에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> RemoveCmd { get; set; } 
        public RelayCommand<object> FindFileCmd { get; set; }
        public RelayCommand<object> DownloadCmd { get; set; }

        public DelegateCommand<object> WindowMoveCommand { get; set; }

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








        // 업로드 스레드핸들러
        private void UploadFileListFX()
        {
            try
            {
                //로딩바..
                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));

                
                //업로드시작...
                UploadFileList();

                // 생성된 첨부파일아이디 반환 -> 뷰쪽으로 바인딩 -> 부모창에서 접근가능
                this.FIL_SEQ = ret_fil_seq.ToString();


                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                        //팝업닫기
                        btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                   })));


            }
            catch (Exception ex)
            {
                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }




        /// <summary>
        /// 물리적파일을 업로드한후, 파일테이블에 등록하고, _FIL_SEQ를 생성한다
        /// </summary>
        /// <returns></returns>
        private void UploadFileList()
        {
            
            int _file_seq = -1;
            List<Hashtable> FILE_DTL_LIST = new List<Hashtable>();//첨부파일상세 리스트



            // 0.첨부파일 수정모드인경우, 해당파일 모두삭제
            if (this.FIL_SEQ != null)
            {
                try
                {
                    _file_seq = Convert.ToInt16(this.FIL_SEQ);
                }
                catch (Exception) { }
            }


            Hashtable param = new Hashtable();
            if (_file_seq > 0)
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
                string file_path = System.IO.Path.Combine(dir_name, file_name);
                string file_name2 = DateTime.Now.ToString("yyyyMMddHHmmssff") + (seq++).ToString() +  fi.Extension; //저장되는파일이름
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);


                try
                {
                    fi.CopyTo(file_path, true);
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.ToString());
                }


                //파일상세정보 추가
                Hashtable FILE_DTL = new Hashtable();//첨부파일상세
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
            _file_seq = SaveFileList(_file_seq, FILE_DTL_LIST);



            ret_fil_seq = _file_seq;
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
                //로딩바..
                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));

                //다운로드시작...
                System.IO.File.Copy(source_file_path, target_file_path, true);

                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                   })));


            }
            catch (Exception ex)
            {
                fileMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (fileMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }



    }



}
