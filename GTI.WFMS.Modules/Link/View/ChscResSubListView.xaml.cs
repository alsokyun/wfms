using GTI.WFMS.Models.Common;
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
