using GTI.WFMS.Modules.Adm.View;
using System;
using System.Reflection;
using System.Windows;

namespace GTI.WFMS.Modules.Main
{
    /// <summary>
    /// PopWin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopWin : Window
    {
        private string v; //화면컨트롤명

        public PopWin()
        {
            InitializeComponent();
        }

        public PopWin(string v) : this()
        {

            this.v = v;

            /* 동적 클래스 생성으로 페이지 Navigate
            string className = "GTI.WFMS.Modules.Adm.View." + "UcPageView";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type classType = assembly.GetType(className);
            object pageInstance = Activator.CreateInstance(classType);
            this.srcFrm.Navigate(pageInstance);
             */


            string path = "../" + v;
            this.srcFrm.Source = new Uri(path, UriKind.Relative);
            //this.srcFrm.Source = new Uri("pack://application:,,,/GTI.WFMS.Modules;component/Adm/View/UcPageView.xaml", UriKind.Absolute);

        }
    }
}
