﻿using GTIFramework.Common.Utils.ViewEffect;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Dash.View
{
    /// <summary>
    /// UcChart04.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcChart04 : UserControl
    {
        public UcChart04()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

        }

        private void XYDiagram2D_BeforeZoom(object sender, DevExpress.Xpf.Charts.XYDiagram2DBeforeZoomEventArgs e)
        {

        }
    }
}
