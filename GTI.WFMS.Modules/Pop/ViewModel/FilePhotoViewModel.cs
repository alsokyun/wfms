using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Model;
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class FilePhotoViewModel : FileMapDtl
    {

 

        // 파일관련 전역변수 - 환경설정으로 관리해야함
        string dir_name = @"" + FmsUtil.fileDir;

        //파일다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread download_thread;
        Thread upload_thread;
        string target_file_path;
        string source_file_path;
        int ret_fil_seq;   //업로드후 생성된 첨부파일ID

        //화면컴포넌트
        TableView gv;
        FilePhotoView filePhotoView;
        TextBlock txtFIL_SEQ;
        TextBlock txtBIZ_ID;
        Button btnClose;
        Image imgView;
        Grid grdImg;
        

        #region =========  프로퍼티 ===========

        /*
         * Items - FileItems 
         */
        public virtual ObservableCollection<FileInfo> ItemsFile { get; set; } //파일객체
        public virtual ObservableCollection<FileDtl> ItemsSelect { get; set; } //파일DB객체
        public virtual ObservableCollection<FileDtl> ItemsAdd { get; set; } //파일DB객체

        /*
         바인딩 뷰모델
        private FileMapDtl mapDtl;
        public FileMapDtl MapDtl
        {
            get { return mapDtl; }
            set
            {
                this.mapDtl = value;
                RaisePropertyChanged("MapDtl");
            }
        }
         */

        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> DelCmd { get; set; }
        public RelayCommand<object> SaveFileCmd { get; set; } //뷰에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> SaveCmd { get; set; } 
        public RelayCommand<object> RemoveCmd { get; set; }
        public RelayCommand<object> FindFileCmd { get; set; }
        public RelayCommand<object> DownloadCmd { get; set; }
        public RelayCommand<object> PreviewCmd { get; set; }
        
        public DelegateCommand<object> WindowMoveCommand { get; set; }

      

        #endregion





        /// <summary>
        /// 생성자
        /// </summary>        
        public FilePhotoViewModel()
        {
            // 초기로딩처리
            LoadedCommand = new DelegateCommand<object>(OnLoaded);


            // 파일인포리스트
            ItemsFile = new ObservableCollection<FileInfo>();
            ItemsSelect = new ObservableCollection<FileDtl>();
            ItemsAdd = new ObservableCollection<FileDtl>();



            //파일저장버튼 이벤트
            SaveFileCmd = new RelayCommand<object>(delegate (object obj)
            {
                upload_thread = new Thread(new ThreadStart(UploadFileListFX));
                upload_thread.Start();

            });

            //첨부내용 저장버튼 이벤트
            SaveCmd = new RelayCommand<object>(OnSave);


            

            //파일삭제버튼 이벤트
            DelCmd = new DelegateCommand<object>(delegate (object obj)
            {
                string seq = "";
                try
                {
                    seq = obj.ToString();
                }
                catch (Exception) { }

                if (FmsUtil.IsNull(seq) || "0".Equals(seq))
                {
                    Messages.ShowErrMsgBox("삭제할 대상이 없습니다.");
                    return;
                }


                string file_name = "";
                string del_file_path = "";



                //0.첨부파일정보가져오기
                Hashtable param = new Hashtable();
                try
                {
                    param.Add("FIL_SEQ", this.FIL_SEQ);
                    param.Add("SEQ", seq);
                    param.Add("sqlId", "SelectFileDtl");
                    DataTable dt = BizUtil.SelectList(param);
                    //물리파일명 가져오기
                    file_name = dt.Rows[0]["UPF_NAM"].ToString();
                }
                catch (Exception ex)
                {
                    Messages.ShowInfoMsgBox(ex.ToString());
                    InitModel();
                    return;
                }


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
                catch (Exception) { }

                //삭제성공
                Messages.ShowOkMsgBox();
                InitModel();
            });




            //기존파일 다운로드버튼 이벤트
            DownloadCmd = new RelayCommand<object>(delegate (object obj)
            {
                FileDtl dtl = obj as FileDtl;
                string file_name = dtl.DWN_NAM;
                string file_name2 = dtl.UPF_NAM;

                try
                {
                    source_file_path = System.IO.Path.Combine(dir_name, file_name2);
                }
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("다운로드할 수 없습니다.");
                    return;
                }


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

            //미리보기 
            PreviewCmd = new RelayCommand<object>(delegate (object obj)
            {
                string file_name = "";
                try
                {
                    file_name = obj as string;
                }
                catch (Exception){}

                if (FmsUtil.IsNull(file_name))
                {
                    Messages.ShowInfoMsgBox("이미지파일이 없습니다.");
                    return;
                }

                string file_path = System.IO.Path.Combine(dir_name, file_name);

                try
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(file_path);
                    bi.EndInit();
                    

                    imgView.Source = bi;
                    grdImg.Visibility = Visibility.Visible;
                }
                catch (Exception ){
                    Messages.ShowInfoMsgBox("이미지 정보가 없습니다.");
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

                    foreach (FileInfo fi in files)
                    {
                        try
                        {
                            //파일객체
                            ItemsFile.Add(fi);

                            //파일db객체
                            FileDtl dtl = new FileDtl();
                            dtl.DWN_NAM = fi.Name;
                            dtl.FIL_TYP = fi.Extension.Replace(".", "");
                            dtl.FIL_SIZ = fi.Length.ToString();
                            ItemsSelect.Add(dtl);
                        }
                        catch (Exception){}
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
                        if (filePhotoView.WindowState == WindowState.Maximized)
                        {
                            filePhotoView.Top = Mouse.GetPosition(filePhotoView).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                            filePhotoView.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(filePhotoView).X + 20;

                            filePhotoView.WindowState = WindowState.Normal;
                        }
                        filePhotoView.DragMove();
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

            //그리드뷰인스턴스
            filePhotoView = obj as FilePhotoView;
            gv = filePhotoView.gv;

            txtFIL_SEQ = filePhotoView.txtFIL_SEQ;
            txtBIZ_ID = filePhotoView.txtBIZ_ID;
            
            btnClose = filePhotoView.btnClose;
            imgView = filePhotoView.imgView;
            grdImg = filePhotoView.grdImg;
            

            // 초기조회
            InitModel();
        }



        //초기모델조회
        private void InitModel()
        {
            grdImg.Visibility = Visibility.Hidden;

            Hashtable param = new Hashtable();

            // 0.파일첨부내역
            param.Add("sqlId", "SelectFileMap");
            param.Add("BIZ_ID", txtBIZ_ID.Text);
            param.Add("FIL_SEQ", txtFIL_SEQ.Text);

            FileMapDtl result = new FileMapDtl();
            result = BizUtil.SelectObject(param) as FileMapDtl;


            // 내역없으면 신규첨부내용
            if (!FmsUtil.IsNull(result))
            {
                //결과를 뷰모델멤버로 매칭
                Type dbmodel = result.GetType();
                Type model = this.GetType();

                //모델프로퍼티 순회
                foreach (PropertyInfo prop in model.GetProperties())
                {
                    string propName = prop.Name;
                    //db프로퍼티 순회
                    foreach (PropertyInfo dbprop in dbmodel.GetProperties())
                    {
                        string colName = dbprop.Name;
                        var colValue = dbprop.GetValue(result, null);
                        if (colName.Equals(propName))
                        {
                            prop.SetValue(this, Convert.ChangeType(colValue, prop.PropertyType));
                        }
                    }
                    Console.WriteLine(propName + " - " + prop.GetValue(this, null));
                }



                // 1.첨부파일
                param.Clear();
                param.Add("sqlId", "SelectFileDtl2");
                param.Add("FIL_SEQ", txtFIL_SEQ.Text);

                ItemsSelect = new ObservableCollection<FileDtl>(BizUtil.SelectListObj<FileDtl>(param));
            }

        }


        /// <summary>
        /// 첨부DB 저장처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {
            // 파일첨부확인 - 필수첨부이다
            if (FmsUtil.IsNull(this.FIL_SEQ) || this.FIL_SEQ == 0)
            {
                Messages.ShowInfoMsgBox("첨부파일을 먼저 등록(저장)하세요.");
                return;
            }

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(filePhotoView)) return;

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;


            try
            {
                this.GRP_TYP = "111"; //사진파일
                BizUtil.Update2(this, "SaveFileMap2");
            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }




            //저장처리 성공
            Messages.ShowOkMsgBox();
            InitModel();

        }








        #region ========== 드래그앤드롭 처리함수 ==============

        public void DragRecordOver(bool dragEnter)
        {
            //Method must exist.
            Console.WriteLine("DragRecordOver");
        }


        public void DropRecord(List<FileInfo> filesData)
        {
            foreach (FileInfo fi in filesData)
                if (!ItemsFile.Any(x => x.FullName == fi.FullName))
                {
                    try
                    {
                        //파일객체
                        ItemsFile.Add(fi);

                        //파일db객체
                        FileDtl dtl = new FileDtl();
                        dtl.DWN_NAM = fi.Name;
                        dtl.FIL_TYP = fi.Extension.Replace(".", "");
                        dtl.FIL_SIZ = fi.Length.ToString();
                        ItemsSelect.Add(dtl);

                    }
                    catch (Exception){}
                }
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
                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));


                //업로드시작...
                UploadFileList();

                // 생성된 첨부파일아이디 반환 -> 뷰쪽으로 바인딩 -> 부모창에서 접근가능
                this.FIL_SEQ = ret_fil_seq;


                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                       //팝업닫기
                       //btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                   })));


            }
            catch (Exception ex)
            {
                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
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


            // 1.물리적파일 저장
            if (!System.IO.Directory.Exists(dir_name))
            {
                System.IO.Directory.CreateDirectory(dir_name);
            }

            /// Items는 추가된 파일객체만이다
            foreach (FileInfo fi in ItemsFile)
            {
                //string file_name = fi.Name;
                //string file_path = System.IO.Path.Combine(dir_name, file_name);
                string file_name2 = DateTime.Now.ToString("yyyyMMddHHmmssff") + fi.Extension; //저장되는파일이름
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);

                try
                {
                    // 1.파일드라이브에 저장
                    fi.CopyTo(file_path2, true);


                    // 2.저장되는 물리적파일명 db에 기록
                    //파일db객체
                    FileDtl dtl = new FileDtl();
                    dtl.UPF_NAM = file_name2;
                    dtl.DWN_NAM = fi.Name;
                    dtl.FIL_TYP = fi.Extension.Replace(".", "");
                    dtl.FIL_SIZ = fi.Length.ToString();
                    ItemsAdd.Add(dtl);
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.Message);
                }


            }


            // 2.첨부파일 등록
            int _file_seq = 0; //신규마스터
            try
            {
                _file_seq = Convert.ToInt16(this.FIL_SEQ);
            }
            catch (Exception) { }

            // 첨부파일db 저장
            ret_fil_seq = SaveFileList(_file_seq);
            this.FIL_SEQ = ret_fil_seq;

        }





        /// <summary>
        /// 첨부파일 DB등록
        /// </summary>
        /// <param name="file_seq"></param>
        /// <param name="fILE_DTL_LIST"></param>
        private int SaveFileList(int file_seq)
        {
            Hashtable param = new Hashtable();

            //1.마스터신규인 경우 
            if (file_seq < 1)
            {
                //마스터등록(파일마스터 채번)
                param.Clear();
                param.Add("sqlId", "InsertFileMst");
                file_seq = BizUtil.InsertR(param);
            }



            //2.디테일등록
            foreach (FileDtl dt in ItemsAdd)
            {
                //파일마스터키
                dt.FIL_SEQ = file_seq;

                BizUtil.Insert2(dt, "InsertFileDtl2");
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
                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));

                //다운로드시작...
                try
                {
                    System.IO.File.Copy(source_file_path, target_file_path, true);
                }
                catch (Exception)
                {
                    (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                    Messages.ShowErrMsgBox("파일을 다운로드할 수 없습니다.");
                    return;
                }

                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                   })));


            }
            catch (Exception ex)
            {
                filePhotoView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (filePhotoView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }



    }



}
