using GTIFramework.Common.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTIFramework.Analysis.WaterPrediction
{
    public class WeightAnal
    {
        public DataTable Anal(DataTable rawdata, double yearAvg)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DataTable dtresult = new DataTable();
            dtresult.Columns.Add("YM");
            dtresult.Columns.Add("VAL");

            double preMVal;      //전달 유량
            double preYVal;      //전년도(전달과 같은달) 유량
            double preMWeight;   //가중치 preMVal / preYVal;

            try
            {
                if(yearAvg==0)
                {
                    return null;
                }

                preMVal = Convert.ToDouble(rawdata.Rows[12]["MVAL"].ToString());
                preYVal = Convert.ToDouble(rawdata.Rows[0]["MVAL"].ToString());

                if(preYVal!=0)
                {
                    preMWeight = preMVal / preYVal;
                }
                else
                {
                    preMWeight = 1;
                }                

                for (int i = 0; i < 3; i++)
                {
                    DateTime Date = (DateTime.ParseExact(rawdata.Rows[12]["YM"].ToString(), "yyyyMM", provider)).AddMonths(i+1);
                    double Mval = Convert.ToDouble(rawdata.Rows[i + 1]["MVAL"]); //전년도(예측월과 같은달 유량)
                    double val = Math.Round(yearAvg * Mval / yearAvg * preMWeight, 4, MidpointRounding.AwayFromZero);
                    DataRow dr = dtresult.NewRow();
                    dr[0] = Date.ToString("yyyyMM");

                    if (double.IsInfinity(val)) dr[1] = 0;
                    else dr[1] = val;

                    dtresult.Rows.Add(dr);
                }

                return dtresult;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                throw null;
            }


        }
    }
}
