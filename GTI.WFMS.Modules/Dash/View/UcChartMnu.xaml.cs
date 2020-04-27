using GTIFramework.Common.Utils.ViewEffect;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Dash.View
{
    /// <summary>
    /// UcChartMnu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcChartMnu : UserControl
    {

        // 뷰생성자
        public UcChartMnu()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

        }

    }
}
