using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// AttFacDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AttFacDtlView: Window
    {

        // 생성자
        public AttFacDtlView( string _FTR_CDE, int _FTR_IDN, int _ATTA_SEQ)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            //뷰모델로 키값전달
            txtFTR_CDE.Text = _FTR_CDE;
            txtFTR_IDN.Text = _FTR_IDN.ToString();
            txtATTA_SEQ.Text = _ATTA_SEQ.ToString();
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
