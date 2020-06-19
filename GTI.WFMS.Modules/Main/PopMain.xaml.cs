using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GTI.WFMS.Modules.Main
{
    /// <summary>
    /// PopMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopMain : Popup
    {
        private string v; //화면컨트롤명

        public PopMain()
        {
            InitializeComponent();
            


            // 2.테마일괄적용...
            ThemeApply.Themeapply(this);



            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };
            gridContent.Children.Add(thumb);

            MouseDown += (sender, e) =>
            {
                thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };
        }


        public PopMain(string v) : this()
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
            Uri uri =  new Uri(path, UriKind.Relative);
            this.srcFrm.Source = uri;
            //this.srcFrm.Source = new Uri("pack://application:,,,/GTI.WFMS.Modules;component/Adm/View/UcPageView.xaml", UriKind.Absolute);
            


            /* 이벤트등록 */
            //닫기버튼
            btnXSignClose.Click += BtnClose_Click;

        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

    }
}
