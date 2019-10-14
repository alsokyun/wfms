using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.Handle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace GTIFramework.Analysis.WaterDataTransfer
{
    public class HMIDataTransfer
    {
        public HMIDataTransfer()
        {

        }

        DataTable dtresult;

        int intFLSMCnt;
        double douFLSMMin;
        double douFLSMMax;
        double douFillingFLSMCSum;
        DataRow dradd;

        /// <summary>
        /// 누락 적산차 필링
        /// </summary>
        /// <param name="dtinputData"></param>
        /// <returns></returns>
        public DataTable FLSMCFilling(DataTable dtinputData)
       {
            dtresult = new DataTable();
            dtresult.Columns.Add("DT");
            dtresult.Columns.Add("FLSM");
            dtresult.Columns.Add("FLSMC");

            try
            {
                intFLSMCnt = Convert.ToInt32(dtinputData.Rows.Count-1);
                douFLSMMin = 0;
                douFLSMMax = 0;

                if (double.TryParse(dtinputData.Rows[0]["FLSM_MESR_VAL"].ToString(), out douFLSMMin))
                {
                    if (double.TryParse(dtinputData.Rows[dtinputData.Rows.Count - 1]["FLSM_MESR_VAL"].ToString(), out douFLSMMax))
                    {
                        douFillingFLSMCSum = 0;

                        dtinputData.Rows.RemoveAt(dtinputData.Rows.Count - 1);

                        foreach (DataRow dr in dtinputData.Rows)
                        {
                            try
                            {
                                dradd = dtresult.NewRow();

                                dradd["DT"] = dr["MESR_TM"];

                                if (!dr["WEEK1"].ToString().Equals(""))
                                    dradd["FLSMC"] = dr["WEEK1"];
                                else if (!dr["WEEK2"].ToString().Equals(""))
                                    dradd["FLSMC"] = dr["WEEK2"];
                                else if (!dr["WEEK3"].ToString().Equals(""))
                                    dradd["FLSMC"] = dr["WEEK3"];
                                else if (!dr["WEEK4"].ToString().Equals(""))
                                    dradd["FLSMC"] = dr["WEEK4"];
                                else if (!dr["MONTHSAVG"].ToString().Equals(""))
                                    dradd["FLSMC"] = dr["MONTHSAVG"];

                                dradd["FLSMC"] = Convert.ToDouble(dradd["FLSMC"].ToString());
                                douFillingFLSMCSum = douFillingFLSMCSum + Convert.ToDouble(dradd["FLSMC"].ToString());

                                dradd["FLSM"] = dr["FLSM_MESR_VAL"];

                                dtresult.Rows.Add(dradd.ItemArray);
                            }
                            catch (Exception ex) { }
                        }
                    }
                }

                foreach (DataRow dr in dtresult.Rows)
                {
                    dr["FLSMC"] = Math.Truncate((Convert.ToDouble(dr["FLSMC"].ToString()) + ((douFLSMMax - douFLSMMin - douFillingFLSMCSum) / intFLSMCnt)) * 100) / 100;
                }

                return dtresult;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return dtresult;
            }
        }
    }
}
