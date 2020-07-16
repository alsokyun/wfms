using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Stat.View
{
    /// <summary>
    /// FcltStatListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FcltStatListView : Page
    {
        public FcltStatListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }

        
    }
}
