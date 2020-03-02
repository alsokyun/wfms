using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        //그리드행추가시 이벤트처리
        private void AddingNewRow(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

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
            foreach (WttSubcDt row in ((List<WttSubcDt>)grid.ItemsSource))
            {
                row.CHK = "True";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (WttSubcDt row in ((List<WttSubcDt>)grid.ItemsSource))
            {
                row.CHK = null;
            }
        }


        //행추가
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            gv.AddNewRow();
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
