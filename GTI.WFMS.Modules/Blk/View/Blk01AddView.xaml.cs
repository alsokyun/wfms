using GTI.WFMS.Models.Common;
using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Blk.View
{
    /// <summary>
    /// Blk01AddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Blk01AddView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);


        public Blk01AddView()
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
            //공통팝업창 사이즈 원복
            FmsUtil.popWinView.Height = 631;
            NavigationService.Navigate(new Blk01ListView());
        }

    }
}
