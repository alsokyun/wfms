using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduling;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Main.View
{
    /// <summary>
    /// PopWinView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopWinView: Window
    {
        private string v; //화면컨트롤명

        // 생성자
        public PopWinView(string v)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);



            //팝업화면 기본초기화
            this.v = v;


            string path = "../../" + v;
            Uri uri = new Uri(path, UriKind.Relative);
            this.srcFrm.Source = uri;
            //this.srcFrm.Source = new Uri("pack://application:,,,/GTI.WFMS.Modules;component/Adm/View/UcPageView.xaml", UriKind.Absolute);




        }




















        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

        }

    }
}
