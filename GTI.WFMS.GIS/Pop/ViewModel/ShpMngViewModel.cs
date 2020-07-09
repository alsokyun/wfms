using DevExpress.Xpf.Core;
using GTI.WFMS.GIS.Pop.View;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    public class ShpMngViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 인터페이스 구현부분
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /// 멤버변수
        ShpMngView shpMngView;
        string _IP;
        string _SHP;
        string _User;
        string _PW;

        // 파일관련 전역변수 - 환경설정으로 관리해야함
        Thread upload_thread;

        //파일다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread download_thread;
        string target_file_path;
        string source_file_path;
        string dir_name = @"" + FmsUtil.fileDir;




        /// <summary>
        ///  프로퍼티
        /// </summary>
        public RelayCommand<object> LoadedCommand { get; set; }
        public RelayCommand<object> DownloadCmd { get; set; }
        public RelayCommand<object> ImportCmd { get; set; }
        
        public virtual ObservableCollection<FileInfo> ItemsFile { get; set; } //업로드파일객체
        public virtual ObservableCollection<FileDtl> ItemsSelect { get; set; } //조회된파일객체

        public string IP
        {
            get { return _IP; }
            set
            {
                this._IP = value;
                OnPropertyChanged("IP");
            }
        }
        public string SHP
        {
            get { return _SHP; }
            set
            {
                this._SHP = value;
                OnPropertyChanged("SHP");
            }
        }
        public string User
        {
            get { return _User; }
            set
            {
                this._User = value;
                OnPropertyChanged("User");
            }
        }
        public string PW
        {
            get { return _PW; }
            set
            {
                this._PW = value;
                OnPropertyChanged("PW");
            }
        }






        /// 생성자
        public ShpMngViewModel()
        {
            ItemsSelect = new ObservableCollection<FileDtl>();
            ItemsFile = new ObservableCollection<FileInfo>();
            


            // 초기로딩처리
            LoadedCommand = new RelayCommand<object>(delegate(object obj) {
                if (obj == null) return;
                //그리드뷰인스턴스
                shpMngView = obj as ShpMngView;


                // 초기조회
                InitModel();
            });


            //shp파일 임포트
            ImportCmd = new RelayCommand<object>(delegate (object obj) {
                //필수체크
                if (!BizUtil.ValidReq(shpMngView)) return;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Shape files |*.shp;*.shx;*.dbf;*.prj";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //파일인포

                    int cnt = 0;//전체파일수
                    int chk = 0;//shp,dat파일수
                    foreach (FileInfo fi in files)
                    {
                        try
                        {
                            //파일객체
                            ItemsFile.Add(fi);
                            if (fi.Extension.Contains("shp") || fi.Extension.Contains("shx") || fi.Extension.Contains("dbf") || fi.Extension.Contains("prj"))
                            {
                                chk++;
                            }
                            cnt++;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    if(chk < 4)
                    {
                        MessageBox.Show("shp, shx, dbf, prj 파일 4개를 선택해야합니다.");
                        return;
                    }
                    if (cnt > 4)
                    {
                        MessageBox.Show("한번에 한종류의 shp파일만 업로드할수 있습니다.");
                        return;
                    }



                    //파일업로드시작
                    upload_thread = new Thread(new ThreadStart(UploadFileListFX));
                    upload_thread.Start();

                }

            });




            //기존파일 다운로드버튼 이벤트
            DownloadCmd = new RelayCommand<object>(delegate (object obj)
            {
                FileDtl dtl = obj as FileDtl;
                string file_name = dtl.DWN_NAM;

                try
                {
                    source_file_path = BizUtil.GetDataFolder("shape", file_name);
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
                saveFileDialog.Filter = "All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    target_file_path = saveFileDialog.FileName;
                    download_thread = new Thread(new ThreadStart(DownloadFX));
                    download_thread.Start();
                }
            });



            

        }









        #region 내부함수


        /// 조회
        private void InitModel()
        {
            ItemsSelect.Clear();
            ItemsFile.Clear();

            //저장된 shp파일 목록
            DirectoryInfo di = new DirectoryInfo(BizUtil.GetDataFolder("shape"));
            //foreach (FileInfo fi in di.GetFiles().Where(f=> f.Extension.Contains("shp") || f.Extension.Contains("dbf") || f.Extension.Contains("prj") || f.Extension.Contains("shx")))
            foreach (FileInfo fi in di.GetFiles().Where(f=> f.Extension.Contains("shp") ))
            {
                try
                {
                    FileDtl dtl = new FileDtl();
                    dtl.DWN_NAM = fi.Name;
                    dtl.FIL_TYP = fi.Extension.Replace(".", "");
                    dtl.FIL_SIZ = fi.Length.ToString();

                    //파일객체
                    ItemsSelect.Add(dtl);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }














        // 업로드 스레드핸들러
        private void UploadFileListFX()
        {
            try
            {
                //로딩바..
                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;

                    })));


                //1.업로드시작...
                UploadFileList();


                //2.db 원격파워셀스크립트 수행 - 티베로 gisLoader, tbloader 
                ExCmd_GisTbloader();


                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {

                       (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       //Messages.ShowOkMsgBox();
                       InitModel();
                       //팝업닫기
                       //btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                   })));


            }
            catch (Exception ex)
            {
                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
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
            // 1.shp 물리적파일 저장

            /// Items는 추가된 파일객체만이다
            foreach (FileInfo fi in ItemsFile)
            {
                string shp_file_path = Path.Combine(BizUtil.GetDataFolder("shape"), fi.Name);

                try
                {
                    // 1.shp파일 프로그램 위치에 저장
                    fi.CopyTo(shp_file_path, true);

                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.Message);
                }


            }

        }








        /// <summary>
        /// cmd.exe 실행 및 command 수행
        /// </summary>
        private void ExCmd_GisTbloader()
        {
            //0. Cmd.exe 호출
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"cmd";
            psi.WindowStyle = ProcessWindowStyle.Hidden;             // cmd창이 숨겨지도록 하기
            psi.CreateNoWindow = true;                               // cmd창을 띄우지 안도록 하기

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;        // cmd창에서 데이터를 가져오기
            psi.RedirectStandardInput = true;          // cmd창으로 데이터 보내기
            psi.RedirectStandardError = true;          // cmd창에서 오류 내용 가져오기


            Process process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo = psi;
            process.Start();

            //shp 저장소 경로이동
            string cdcmd = "c: ";
            process.StandardInput.Write(cdcmd + Environment.NewLine); 
            cdcmd = "cd " + Path.Combine(BizUtil.GetDataFolder("shape"));
            process.StandardInput.Write(cdcmd + Environment.NewLine); 

            foreach (FileInfo fi in ItemsFile)
            {
                if (!fi.Extension.Contains("shp")) continue; //shp파일에 대해서만 수행

                //1.gisLoader 수행해서 clt 파일생성
                string gisLoadercmd = "gisLoader " + fi.Name + " infofms." + fi.Name.Replace(".shp","");

                process.StandardInput.Write(gisLoadercmd + Environment.NewLine); // 명령어를 보낼때는 꼭 마무리를 해줘야 한다
                process.StandardInput.Close(); //StandardOutput 읽기위해서는 input을 닫아줘야함

                string result = process.StandardOutput.ReadToEnd();
                StringBuilder sb = new StringBuilder();
                sb.Append("[Control파일 생성] \r\n");
                sb.Append(result);
                sb.Append("\r\n");

                if (!result.Contains("complete"))
                {
                    MessageBox.Show("gisLoader Control파일 생성에 실패하였습니다.");
                    return;
                }

                process.StandardInput.Close();

                process.WaitForExit();
                process.Close();
                break; //한파일에대해서만 수행함
            }




            //2.tbloader 수행
            process.Start();

            //shp 저장소 경로이동
            cdcmd = "c: ";
            process.StandardInput.Write(cdcmd + Environment.NewLine);
            cdcmd = "cd " + Path.Combine(BizUtil.GetDataFolder("shape"));
            process.StandardInput.Write(cdcmd + Environment.NewLine);


            foreach (FileInfo fi in ItemsFile)
            {
                if (!fi.Extension.Contains("shp")) continue; //shp파일에 대해서만 수행


                string ctl = fi.Name.Replace(".shp", "") + ".ctl";
                FileInfo ctl_fi = new FileInfo(Path.Combine(BizUtil.GetDataFolder("shape"), ctl));
                if (!ctl_fi.Exists)
                {
                    MessageBox.Show("Control파일이 없습니다.");
                    return;
                }

                // ctl파일에 인코딩 utf8 추가
                string new_file = ""; //추가수정파일내용
                try
                {
                    using (StreamReader r = File.OpenText(Path.Combine(BizUtil.GetDataFolder("shape"), ctl)))
                    {
                        string line = "";
                        while ((line = r.ReadLine()) != null)
                        {
                            new_file += line + "\n";
                            if (line.Contains("LOAD DATA"))
                            {
                                new_file += "CHARACTERSET UTF8" + "\n";
                            }
                        }
                    }

                    //using (StreamWriter w = File.AppendText(Path.Combine(BizUtil.GetDataFolder("shape"), ctl)))
                    //{
                    //    w.WriteLine("CHARACTERSET UTF8");
                    //}

                    //ctl파일 재생성
                    File.WriteAllText(Path.Combine(BizUtil.GetDataFolder("shape"), ctl), new_file);

                }
                catch (Exception){}


                string tbloadercmd = "tbloader userid=infofms/infofms@tibero control=" + ctl;
                process.StandardInput.Write(tbloadercmd + Environment.NewLine); // 명령어를 보낼때는 꼭 마무리를 해줘야 한다
                process.StandardInput.Close(); //StandardOutput 읽기위해서는 input을 닫아줘야함

                string result2 = process.StandardOutput.ReadToEnd();
                StringBuilder sb2 = new StringBuilder();
                sb2.Append("[Import 수행] \r\n");
                sb2.Append(result2);
                sb2.Append("\r\n");

                if (!result2.Contains("success"))
                {
                    MessageBox.Show("tbloader 임포트에 실패하였습니다.");
                    return;
                }

                MessageBox.Show("정상적으로 처리되었습니다.");

                process.WaitForExit();
                process.Close();
                break; //한파일에대해서만 수행함
            }


        }





        /// <summary>
        /// 파워셀 - db 원격스크립트 수행 - 티베로 gisLoader, tbloader 
        /// </summary>
        /// <param name="scriptfile"></param>
        private void ExPsScript(string scriptfile)
        {
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            RunspaceInvoke scriptInvoker = new RunspaceInvoke();
            scriptInvoker.Invoke("Set-ExecutionPolicy RemoteSigned");

            Pipeline pipeline = runspace.CreatePipeline();

            //Here's how you add a new script with arguments
            Command myCommand = new Command(scriptfile);
            //CommandParameter testParam = new CommandParameter("key", "value");
            //myCommand.Parameters.Add(testParam);

            pipeline.Commands.Add(myCommand);

            // Execute PowerShell script
            pipeline.Invoke();
        }





        /// <summary>
        /// 파일다운로드 쓰레드 핸들러
        /// </summary>
        private void DownloadFX()
        {
            try
            {
                //로딩바..
                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));

                //다운로드시작...
                try
                {
                    //shp파일
                    System.IO.File.Copy(source_file_path, target_file_path, true);
                    try
                    {
                        //dbf파일
                        System.IO.File.Copy(source_file_path.Replace(".shp", ".dbf"), target_file_path.Replace(".shp", ".dbf"), true);
                    }
                    catch (Exception){}
                    try
                    {
                        //prj파일
                        System.IO.File.Copy(source_file_path.Replace(".shp", ".prj"), target_file_path.Replace(".shp", ".prj"), true);
                    }
                    catch (Exception){}
                    try
                    {
                        //shx파일
                        System.IO.File.Copy(source_file_path.Replace(".shp", ".shx"), target_file_path.Replace(".shp", ".shx"), true);
                    }
                    catch (Exception){}
                }
                catch (Exception)
                {
                    (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                    Messages.ShowErrMsgBox("파일을 다운로드할 수 없습니다.");
                    return;
                }

                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                   })));


            }
            catch (Exception ex)
            {
                shpMngView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpMngView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }


        #endregion



    }
}
