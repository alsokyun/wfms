using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// FaqDocView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FaqDocView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        private string _SEQ;

        public FaqDocView(string SEQ)
        {
            InitializeComponent();

            _SEQ = SEQ;
            this.txtSEQ.EditValue = SEQ;

            // 테마일괄적용...
            ThemeApply.Themeapply(this);



            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;
            btnBack.Click += delegate (object sender, RoutedEventArgs e)
            {
                NavigationService.Navigate(new FaqListView());
            };


            

        }






        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FaqListView());
        }

        // 수정페이지로
        private void BtnModi_Click(object sender, RoutedEventArgs e)
        {
            ///=> 뷰모델과바인딩된 객체값을 변경해서 뷰모델로 최종적으로 파라미터 전달
            NavigationService.Navigate(new FaqDtlView(_SEQ));
        }
    }
}
