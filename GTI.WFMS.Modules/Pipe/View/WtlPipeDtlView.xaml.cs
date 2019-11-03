using System;
using System.Collections.Generic;
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

namespace GTI.WFMS.Modules.Pipe.View
{
    /// <summary>
    /// WtlPipeDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WtlPipeDtlView : Page
    {
        public WtlPipeDtlView(string FTR_CDE, int FTR_IDN)
        {
            InitializeComponent();

            this.txtFTR_CDE.EditValue = FTR_CDE;
            this.txtFTR_IDN.EditValue = FTR_IDN;
        }
    }
}
