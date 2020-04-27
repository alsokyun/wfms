using GTI.WFMS.Models.Common;
using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// CnstMngAddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstMngAddView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        //public event BackCmd backEvent;


        public CnstMngAddView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            //강제이벤트 발생
            //BackCmd backCmd = new BackCmd(_backCmd);
            //backEvent += new BackCmd(_backCmd);
            //backEvent(null, null);


            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;
           
            
        }

   

        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            //공통팝업창 사이즈 초기화
            FmsUtil.popWinView.Height = 631;
            NavigationService.Navigate(new CnstMngListView());
        }
    }
}
