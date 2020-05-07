using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// OptMtListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OptMtListView : UserControl
    {
        public OptMtListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }



        //선택된 항목으로 내역팝업
        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;
            try
            {
                string G2_ID = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "G2_ID").ToString();

                Window pop = new OptMtDtlView(G2_ID);
                if (pop.ShowDialog() is bool)
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
