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
    /// FaqDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FaqDtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        private string _SEQ;

        public FaqDtlView(string SEQ)
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
    }
}
