using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Pipe.View
{
    /// <summary>
    /// FlowMtAddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowMtAddView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);

        public FlowMtAddView()
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
            NavigationService.Navigate(new FlowMtListView());
        }

    }
}
