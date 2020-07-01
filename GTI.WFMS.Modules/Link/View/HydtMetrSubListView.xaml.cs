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

            //뷰모델로 키전달
            txtFTR_CDE.Text = FTR_CDE;
            txtFTR_IDN.Text = FTR_IDN.ToString();

        }
    }
}
