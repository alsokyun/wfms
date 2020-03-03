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


        //선택된 항목으로 페이지이동
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string FTR_CDE = "";
            string FTR_IDN = "";

            GridControl gc = sender as GridControl;

            try
            {
                FTR_CDE = ((DataRowView)gc.CurrentItem).Row["FTR_CDE"].ToString();
                FTR_IDN = ((DataRowView)gc.CurrentItem).Row["FTR_IDN"].ToString();
                

                LekSiteDtlView view = new LekSiteDtlView(FTR_CDE, FTR_IDN);
                if (view.ShowDialog() is bool)
                {
                    //재조회
                    btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return; //throw;
            }
            
        }

        // 등록 팝업
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            CnstCmplAddView view = new CnstCmplAddView();
            if (view.ShowDialog() is bool)
            {
                //재조회
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
