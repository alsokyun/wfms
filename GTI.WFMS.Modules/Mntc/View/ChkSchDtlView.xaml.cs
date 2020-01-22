using DevExpress.Xpf.Scheduling;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// ChkSchDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChkSchDtlView: Window
    {

        //사업소 DB연결이 없으면
        public ChkSchDtlView( )
        {
            InitializeComponent();
            //ThemeApply.Themeapply(this);

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
        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

    }
}
