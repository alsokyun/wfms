using GTI.WFMS.Models.Common;
using System.Collections;
using System.Data;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// ChscResSubListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChscResSubListView : UserControl
    {



        public ChscResSubListView(string FTR_CDE, int FTR_IDN)
        {
            InitializeComponent();

            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "selectChscResSubList");
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }
    }
}
