using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTIFramework.Analysis.WaterQuality
{
    public class WQForeCL
    {
        public DataTable WQFore(DataRowView rPoint, DataTable dtPipe)
        {
            DataTable dtresult = new DataTable();

            try
            {
                dtresult = dtPipe.Clone();
                dtresult.Columns.Add("TIME");
                dtresult.Columns.Add("KB");
                dtresult.Columns.Add("KW");
                dtresult.Columns.Add("CL");


                if (rPoint["CL"].ToString().Equals("")
                    || rPoint["TMP"].ToString().Equals("")
                    || rPoint["Q"].ToString().Equals("")
                    || rPoint["KW"].ToString().Equals(""))
                {
                    return dtresult;
                }


                double initCL = Convert.ToDouble(rPoint["CL"].ToString());
                double initTMP = Convert.ToDouble(rPoint["TMP"].ToString());
                double initQ = Convert.ToDouble(rPoint["Q"].ToString());
                double initKw = Convert.ToDouble(rPoint["KW"].ToString());
                double initpi = Math.PI;
                double ttTime = 0;

                foreach (DataRow dr in dtPipe.Rows)
                {
                    DataRow addr = dtresult.NewRow();
                    addr["ORD"] = dr["ORD"];
                    addr["PIPE_SEQ"] = dr["PIPE_SEQ"];
                    addr["POINT_SEQ"] = dr["POINT_SEQ"];
                    addr["PIPE_D"] = dr["PIPE_D"];
                    addr["PIPE_L"] = dr["PIPE_L"];


                    //1.PIPE 관경, 길이 분석적용값 셋팅
                    double PipeD = Convert.ToDouble(dr["PIPE_D"].ToString());
                    double PipeL = Convert.ToDouble(dr["PIPE_L"].ToString());

                    //2.입력자료별 분석 필요자료 산정
                    double dQ = Math.Round(initQ / 3600, 4, MidpointRounding.AwayFromZero);                       //유량(m^3/s)
                    double dD = PipeD / 1000;                                                                     //관경(m)
                    double dA = Math.Round(initpi * (Math.Pow(dD, 2)) / 4, 4, MidpointRounding.AwayFromZero);     //단면적(m^2)
                    double dV = Math.Round(dQ / dA, 4, MidpointRounding.AwayFromZero);                            //유속(m/s)
                    double dL = PipeL;                                                                            //거리(m)
                    double dT = initTMP;                                                                          //수온(°C)
                    double dKb = Math.Round(-0.0398 * Math.Exp(0.0742 * dT), 4, MidpointRounding.AwayFromZero);   //수체감소계수
                    double bKw = initKw;                                                                          //벽체감소계수
                    double dTime = Math.Round(dL / dV / 86400, 4, MidpointRounding.AwayFromZero);                 //도달시간
                    double cl = initCL;

                    //3.농도감소예측

                    double dbulk;   //수체감소량
                    double dwall;   //벽체감소량
                    double totaldc; //염소농도 감소량

                    if (initCL > 0)
                    {
                        dbulk = Math.Round(dKb * cl * dTime, 4, MidpointRounding.AwayFromZero);                //수체감소량
                        dwall = Math.Round(bKw * 4 / dD * dTime, 4, MidpointRounding.AwayFromZero);            //벽체감소량
                        totaldc = Math.Round(dbulk + dwall, 4, MidpointRounding.AwayFromZero);                 //염소농도 감소량
                    }
                    else
                    {
                        dbulk = 0;            //수체감소량
                        dwall = 0;            //벽체감소량
                        totaldc = 0;          //염소농도 감소량
                    }

                    //4.잔류염소 농도
                    cl = cl + totaldc;                                                                            //잔류염소농도
                    ttTime = ttTime + dTime;

                    addr["KB"] = dbulk;
                    addr["KW"] = dwall;
                    if (cl < 0)
                    {
                        cl = 0;
                    }
                    addr["CL"] = Math.Round(cl, 4, MidpointRounding.AwayFromZero);

                    addr["TIME"] = ttTime;
                    dtresult.Rows.Add(addr);

                    initCL = cl;
                }

                //TestAnal(rPoint);

                return dtresult;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        private void TestAnal(DataRowView rPoint)
        {
            double initCL = 1;
            double initTMP = 20;
            double initQ = 800;
            double initKw = -0.002;
            double initpi = Math.PI;

            //1.PIPE 관경, 길이 분석적용값 셋팅
            double PipeD = 500;
            double PipeL = 3000;

            //2.입력자료별 분석 필요자료 산정
            double dQ = Math.Round(initQ / 3600, 4, MidpointRounding.AwayFromZero);                       //유량(m^3/s)
            double dD = PipeD / 1000;                                                                     //관경(m)
            double dA = Math.Round(initpi * (Math.Pow(dD, 2)) / 4, 4, MidpointRounding.AwayFromZero);     //단면적(m^2)
            double dV = Math.Round(dQ / dA, 4, MidpointRounding.AwayFromZero);                            //유속(m/s)
            double dL = PipeL;                                                                            //거리(m)
            double dT = initTMP;                                                                          //수온(°C)
            double dKb = Math.Round(-0.0398 * Math.Exp(0.0742 * dT), 4, MidpointRounding.AwayFromZero);   //수체감소계수
            double bKw = initKw;                                                                          //벽체감소계수
            double dTime = Math.Round(dL / dV / 86400, 4, MidpointRounding.AwayFromZero);                 //도달시간
            double cl = initCL;

            //3.농도감소예측
            double dbulk = Math.Round(dKb * cl * dTime, 4, MidpointRounding.AwayFromZero);                //수체감소량
            double dwall = Math.Round(bKw * 4 / dD * dTime, 4, MidpointRounding.AwayFromZero);            //벽체감소량
            double totaldc = Math.Round(dbulk + dwall, 4, MidpointRounding.AwayFromZero);                 //염소농도 감소량

            //4.잔류염소 농도
            cl = cl + totaldc;                                                                            //잔류염소농도

        }
    }
}
