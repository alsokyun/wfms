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
    /// PdjtUseListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtUseListView : UserControl
    {
        public PdjtUseListView()
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
                string PDH_NUM = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "PDH_NUM").ToString();

                Window pop = new PdjtUseDtlView(PDH_NUM);
                if (pop.ShowDialog() is bool)
                {
                    //재조회
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        

    }
}
