using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using GTIFramework.Common.MessageBox;

namespace GTIFramework.Analysis.WaterAnalysis
{
    public class NMFAnalysis
    {
        public NMFAnalysis()
        {

        }

        DataRow drNMFResult;

        public DataRow FLSMCAnalysis(DataTable dtRawData)
        {
            try
            {
                drNMFResult = null;

                //값이 작은것 중에 시간이 작은거
                dtRawData.DefaultView.Sort = "CAL_MODI_VAL ASC, PRS ASC";

                if(dtRawData.Rows.Count == 0)
                    drNMFResult = null;
                else
                    drNMFResult = dtRawData.DefaultView[0].Row;

                return drNMFResult;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
            }
        }

        public DataRow FLAnalysis(DataTable dtRawData)
        {
            try
            {
                drNMFResult = null;

                //값이 작은것 중에 시간이 작은거
                dtRawData.DefaultView.Sort = "AVG1HR ASC, PRS ASC";

                if (dtRawData.Rows.Count == 0)
                    drNMFResult = null;
                else
                    drNMFResult = dtRawData.DefaultView[0].Row;

                return drNMFResult;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
            }
        }
    }
}
