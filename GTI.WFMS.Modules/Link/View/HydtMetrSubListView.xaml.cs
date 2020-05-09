using GTI.WFMS.Models.Common;
using System.Collections;
using System.Data;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// HydtMetrSubListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HydtMetrSubListView : UserControl
    {



        public HydtMetrSubListView(string FTR_CDE, int FTR_IDN)
        {
            InitializeComponent();

            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "selectHydtMetrSubList");
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }
    }
}
