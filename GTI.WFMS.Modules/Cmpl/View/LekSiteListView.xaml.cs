using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// LekSiteListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LekSiteListView : Page
    {
        public LekSiteListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }



        // 등록 팝업
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            LekSiteAddView view = new LekSiteAddView();
            if (view.ShowDialog() is bool)
            {
                //재조회
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;
            try
            {
                string FTR_CDE = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_CDE").ToString();
                string FTR_IDN = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_IDN").ToString();

                LekSiteDtlView view = new LekSiteDtlView(FTR_CDE, FTR_IDN);
                if (view.ShowDialog() is bool)
                {
                    //재조회
                    btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
