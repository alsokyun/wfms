using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTI.WFMS.Modules.Pipe.View
{
    /// <summary>
    /// WtsMnhoAddView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WtsMnhoAddView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        public event BackCmd backEvent;

        public WtsMnhoAddView()
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
            NavigationService.Navigate(new WtsMnhoListView());
        }

    }
}
