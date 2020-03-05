using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Fclt.View
{
    /// <summary>
    /// PrsPmpAddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PrsPmpAddView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);

        public PrsPmpAddView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;

        }

        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PrsPmpListView());
        }

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
