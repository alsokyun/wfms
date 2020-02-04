using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace GTI.WFMS.Modules.Pipe.Report
{
    public partial class FireFacReport : DevExpress.XtraReports.UI.XtraReport
    {
        public FireFacReport()
        {
            InitializeComponent();
        }

        private void FireFacReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}
