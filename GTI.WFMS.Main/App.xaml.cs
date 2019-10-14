using System;
using System.Windows;

using System.Threading;

using GTIFramework.Common.Log;

using GTIFramework.Common.MessageBox;
using GTI.WFMS.Main.View;

namespace GTI.WFMS.Main
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex _mutex = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            string strtitle = "InfoFacility";

            bool bcreateNew = false;

            try
            {
                _mutex = new Mutex(true, strtitle, out bcreateNew);

                if (bcreateNew)
                {
                    Logs.DBdefault();

                    Login login = new Login();
                    Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    bool? res = login.ShowDialog();

                    if (!res ?? true)
                    {
                        Shutdown(1);
                    }
                    else
                    {
                        base.OnStartup(e);

                        // 프리즘을 사용하기위해 Bootstrapper를 시작한다...
                        Bootstrapper bs = new Bootstrapper();
                        bs.Run();
                    }
                }
                else
                {
                    MessageBox.Show("InfoFacility 실행중입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Messages.ShowInfoMsgBox(e.Exception.ToString());
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }
    }
}
