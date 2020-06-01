using GTI.WFMS.Modules.Cnst.Model;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// WttChngDt.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WttSubcDtView : UserControl
    {

        public DataTable dt; //그리드데이터 소스

        private string CNT_NUM;
        public WttSubcDtView(string _CNT_NUM)
        {
            InitializeComponent();

            this.CNT_NUM = _CNT_NUM;
            this.txtCNT_NUM.EditValue = _CNT_NUM; //키를 뷰모델로 넘기기위해 뷰바인딩 활용
        }



        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            //CheckEdit ce = sender as CheckEdit;
            //bool chk = ce.IsChecked is bool;
            foreach (WttSubcDt row in (ObservableCollection<WttSubcDt>)grid.ItemsSource)
            {
                row.CHK = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (WttSubcDt row in (ObservableCollection<WttSubcDt>)grid.ItemsSource)
            {
                row.CHK = "N";
            }
        }


        private void grid_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Total")
            {
                e.Value = Convert.ToInt32(grid.GetCellValueByListIndex(e.ListSourceRowIndex, "UnitPrice")) *
                    Convert.ToDouble(grid.GetCellValueByListIndex(e.ListSourceRowIndex, "UnitsOnOrder"));
            }
        }

    }
}
