using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// CnstCmplAddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstCmplAddView : Window
    {

        /// <summary>
        /// 생성자
        /// </summary>
        public CnstCmplAddView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void CnstCmplAddView_KeyDown(object sender, KeyEventArgs e)
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


        //상수공사 선택팝업
        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {
            String inCNT_NUM = this.txtCNT_NUM.Text; ;
            String outCNT_NUM = "";

            if (inCNT_NUM != null && inCNT_NUM != "")
            {
                if (Messages.ShowYesNoMsgBox("공사번호를 변경하시겠습니까?") != MessageBoxResult.Yes) return;
            }

            try
            {
                // 상수공사대장 윈도우
                CnstMngPopView cnstMngPopView = new CnstMngPopView("");
                cnstMngPopView.Owner = Window.GetWindow(this);

                //공사번호 리턴
                if (cnstMngPopView.ShowDialog() is bool)
                {
                    outCNT_NUM = cnstMngPopView.txtRET_CNT_NAM.Text;
                    if (outCNT_NUM != null && outCNT_NUM != "" && inCNT_NUM != outCNT_NUM)
                    {
                        this.txtCNT_NUM.Text = outCNT_NUM;
                    }

                    this.txtCNT_NUM.SelectAll();
                    this.txtCNT_NUM.Focus();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }


    }
}
