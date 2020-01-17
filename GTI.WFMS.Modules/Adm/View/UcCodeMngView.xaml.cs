using GTIFramework.Common.Utils.ViewEffect;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Adm
{
    /// <summary>
    /// UcCodeMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcCodeMngView : UserControl
    {
        public UcCodeMngView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }
    }
}
