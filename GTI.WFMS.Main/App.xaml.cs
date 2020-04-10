using System;
using System.Windows;

using System.Threading;

using GTIFramework.Common.Log;

using GTIFramework.Common.MessageBox;
using GTI.WFMS.Main.View;
using ESRI.ArcGIS.esriSystem;

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

                        // ESRI ArcObject Engine 라이센스체크
                        ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine);
                        InitializeEngineLicense();


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











        #region ========= ArcObject Engine 라이센스관련 체크코드 ========= 


        //private LicenseInitializer m_AOLicenseInitializer = new GTI.WFMS.Main.LicenseInitializer();

        void Application_Startup(object sender, StartupEventArgs e)
        {
            //ESRI License Initializer generated code.
            //m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine },
            //new esriLicenseExtensionCode[] { });
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            //m_AOLicenseInitializer.ShutdownApplication();
        }


        private void InitializeEngineLicense()
        {
            AoInitialize aoi = new AoInitializeClass();

            //more license choices could be included here
            esriLicenseProductCode productCode = esriLicenseProductCode.esriLicenseProductCodeEngine;
            if (aoi.IsProductCodeAvailable(productCode) == esriLicenseStatus.esriLicenseAvailable)
            {
                aoi.Initialize(productCode);
            }
        }


        #endregion
    }
}
