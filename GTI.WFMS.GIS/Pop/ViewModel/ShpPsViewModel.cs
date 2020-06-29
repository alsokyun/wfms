using DevExpress.Xpf.Core;
using GTI.WFMS.GIS.Pop.View;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    public class ShpPsViewModel : INotifyPropertyChanged
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
        ShpPsView shpPsView;
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
        public ShpPsViewModel()
        {
            ItemsSelect = new ObservableCollection<FileDtl>();
            ItemsFile = new ObservableCollection<FileInfo>();
            


            // 초기로딩처리
            LoadedCommand = new RelayCommand<object>(delegate(object obj) {
                if (obj == null) return;
                //그리드뷰인스턴스
                shpPsView = obj as ShpPsView;


                // 초기조회
                InitModel();
            });


            //shp파일 임포트
            ImportCmd = new RelayCommand<object>(delegate (object obj) {
                //필수체크
                if (!BizUtil.ValidReq(shpPsView)) return;

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Shape files |*.shp;*.dbf";
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
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
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

            //저장된 shp파일 목록
            DirectoryInfo di = new DirectoryInfo(BizUtil.GetDataFolder("shape"));

            foreach (FileInfo fi in di.GetFiles("*.shp"))
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
                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));


                //업로드시작...
                UploadFileList();



                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       //db 원격파워셀스크립트 수행 - 티베로 gisLoader, tbloader 
                       ExPs_GisTbloader();


                       (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                       InitModel();
                       //팝업닫기
                       //btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                   })));


            }
            catch (Exception ex)
            {
                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
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
                string db_shp_file_path = Path.Combine(SHP, fi.Name);

                try
                {
                    // 1.shp파일 프로그램 위치에 저장
                    fi.CopyTo(shp_file_path, true);

                    // 2.shp파일 db서버 위치에 원격복사
                    ExPs_CopyShape(shp_file_path, db_shp_file_path);




                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.Message);
                }


            }

        }







        /// <summary>
        /// 원격파일 복사
        /// </summary>
        private void ExPs_CopyShape(string path, string destination)
        {
            // Username and Password for the remote machine.
            var userName = User;
            string pw = PW;

            // Creates a secure string for the password
            SecureString securePassword = new SecureString();
            foreach (char c in pw)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            // Creates a PSCredential object
            PSCredential creds = new PSCredential(userName, securePassword);




            // Creates the runspace for PowerShell
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // Create the PSSession connection to the remote machine.
            //ComputerName
            string computerName = IP;

            PowerShell powershell = PowerShell.Create();
            PSCommand command = new PSCommand();
            command.AddCommand("New-PSSession");
            command.AddParameter("ComputerName", computerName);
            command.AddParameter("Credential", creds);
            powershell.Commands = command;
            runspace.Open();
            powershell.Runspace = runspace;
            Collection<PSObject> result = powershell.Invoke();



            // Takes the PSSession object and places it into a PowerShell variable
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", "session");
            command.AddParameter("Value", result[0]);
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();



            // Calls the Copy-Item cmdlet as a script and passes the PSSession, Path and destination parameters to it
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript("Copy-Item -Path " + path + " -Destination " + destination + " -ToSession $session");
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();

            // 세션종료
            powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            powershell.AddScript("Remove-PSSession $session");
            powershell.Invoke();
        }



        /// <summary>
        /// 원격서버스크립트수행
        /// </summary>
        private void ExPs_GisTbloader()
        {
            // Username and Password for the remote machine.
            var userName = User;
            string pw = PW;

            // Creates a secure string for the password
            SecureString securePassword = new SecureString();
            foreach (char c in pw)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            // Creates a PSCredential object
            PSCredential creds = new PSCredential(userName, securePassword);




            // Creates the runspace for PowerShell
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // Create the PSSession connection to the remote machine.
            //ComputerName
            string computerName = IP;

            PowerShell powershell = PowerShell.Create();
            PSCommand command = new PSCommand();
            command.AddCommand("New-PSSession");
            command.AddParameter("ComputerName", computerName);
            command.AddParameter("Credential", creds);
            powershell.Commands = command;
            runspace.Open();
            powershell.Runspace = runspace;
            Collection<PSObject> result = powershell.Invoke();



            // Takes the PSSession object and places it into a PowerShell variable
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", "session");
            command.AddParameter("Value", result[0]);
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();



            // Calls the Copy-Item cmdlet as a script and passes the PSSession, Path and destination parameters to it

            // 1.gisLoader 실행
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript(@"Invoke-Command -Session $session  -ScriptBlock {Invoke-Expression -Command:'cmd /c gisLoader d:\shape\WTL_SPLY_LS.shp infofms.WTL_SPLY_LS' }");
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();

            // 2.tbloader 실행
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript(@"Invoke-Command  -Session $session -ScriptBlock {Invoke-Expression -Command:'cmd /c tbloader userid=infouser/infouser control=d:\shape\WTL_SPLY_LS.ctl' } ");
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();

            // 세션종료
            powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            powershell.AddScript("Remove-PSSession $session");
            powershell.Invoke();
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
                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));

                //다운로드시작...
                try
                {
                    System.IO.File.Copy(source_file_path, target_file_path, true);
                }
                catch (Exception)
                {
                    (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                    Messages.ShowErrMsgBox("파일을 다운로드할 수 없습니다.");
                    return;
                }

                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                   })));


            }
            catch (Exception ex)
            {
                shpPsView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (shpPsView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }


        #endregion



    }
}
