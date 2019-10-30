using GTI.WFMS.Modules.Adm.View;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

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


            /* 이벤트등록 */
            //닫기버튼
            btnXSignClose.Click += BtnClose_Click;

            //윈도우창이동
            bdTitle.PreviewMouseDown += BdTitle_PreviewMouseDown;


        }

        //윈도우창이동
        private void BdTitle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    this.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    this.WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }

        //닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
