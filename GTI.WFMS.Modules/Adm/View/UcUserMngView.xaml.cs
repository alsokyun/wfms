using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Adm
{
    /// <summary>
    /// UcUserMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcUserMngView : Page
    {
        public UcUserMngView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }
    }
}
