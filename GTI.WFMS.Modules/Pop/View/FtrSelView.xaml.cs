using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Main;
using GTI.WFMS.Modules.Pop.ViewModel;
using GTIFramework.Common.Utils.ViewEffect;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// FtrSelView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FtrSelView : Window
    {

        /// <summary>
        /// 생성자
        /// </summary>
        public FtrSelView(string FTR_IDN)
        {

            InitializeComponent();


            // 2.테마일괄적용...
            //ThemeApply.Themeapply(this);


            //파일키저장
            txtFTR_IDN.Text = FTR_IDN;
        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();
        }


    }
}
