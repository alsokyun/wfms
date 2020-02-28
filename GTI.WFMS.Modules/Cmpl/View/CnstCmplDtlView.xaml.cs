using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// CnstCmplDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstCmplDtlView : Window
    {
        private string _RCV_NUM;


        /// <summary>
        /// 생성자
        /// </summary>
        public CnstCmplDtlView(string RCV_NUM)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            //뷰모델로 키전달하기기위 뷰에서 UpdateTrigger 발생시킴
            _RCV_NUM = RCV_NUM;
            this.txtRCV_NUM.EditValue = RCV_NUM;
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void CnstCmplDtlView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

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



    }
}
