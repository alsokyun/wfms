using GTIFramework.Common.MessageBox;
using System;
using System.Data;

namespace GTIFramework.Analysis.WaterAnalysis
{
    public class AZPNPAnalysis
    {
        public AZPNPAnalysis()
        {

        }

        double dGamma = 1;
        double dGA = 9.81;

        //AZP
        double dAZP = 0;
        double dAZP_INPIPE_ALT;
        double dAZP_METER_AVG_ALT;
        double dAZP_DIAM;
        double dAZP_cDIAM;
        double dAZP_cAVGPRS;
        double dAZP_cAVGFLSMC;
        double dAZP_AREA;
        double dAZP_Vel;

        //AZNP
        double dAZNP = 0;
        double dAZNP_INPIPE_ALT;
        double dAZNP_METER_AVG_ALT;
        double dAZNP_DIAM;
        double dAZNP_cDIAM;
        double dAZNP_cMINPRS;
        double dAZNP_cMINFLSMC;
        double dAZNP_AREA;
        double dAZNP_Vel;

        public double? AZPAnalysis(double dAVGFLSMC, double dAVGPRS, DataRow drpoint)
        {
            try
            {
                //관직경
                dAZP_DIAM = Convert.ToDouble(drpoint["DIAM"].ToString());
                //유입관저고
                dAZP_INPIPE_ALT = Convert.ToDouble(drpoint["INPIPE_ALT"].ToString());
                //급수전평균고도
                dAZP_METER_AVG_ALT = Convert.ToDouble(drpoint["METER_AVG_ALT"].ToString());
                //c관직경
                dAZP_cDIAM = dAZP_DIAM / 1000;
                //c_평균압력
                dAZP_cAVGPRS = dAVGPRS * 10;
                //c_평균유량
                dAZP_cAVGFLSMC = dAVGFLSMC / 3600;
                //AREA
                dAZP_AREA = (3.14 * (Math.Pow(dAZP_cDIAM, 2))) / 4;
                //Vel
                dAZP_Vel = dAZP_cAVGFLSMC / dAZP_AREA;

                //AZP = ( 유입관저고 + ( c_평균압력 / Gamma  ) + ( Vel^2 / GA * 2 ) ) - 급수전평균고도
                dAZP = (dAZP_INPIPE_ALT + (dAZP_cAVGPRS / dGamma) + (Math.Pow(dAZP_Vel, 2) / dGA * 2)) - dAZP_METER_AVG_ALT;

                return dAZP;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
            }
        }

        public double? AZNPAnalysis(double dMINFLSMC, double dMINPRS, DataRow drpoint)
        {
            try
            {
                //관직경
                dAZNP_DIAM = Convert.ToDouble(drpoint["DIAM"].ToString());
                //유입관저고
                dAZNP_INPIPE_ALT = Convert.ToDouble(drpoint["INPIPE_ALT"].ToString());
                //급수전평균고도
                dAZNP_METER_AVG_ALT = Convert.ToDouble(drpoint["METER_AVG_ALT"].ToString());
                //c관직경
                dAZNP_cDIAM = dAZNP_DIAM / 1000;
                //c_일 최소유량 시점의 압력
                dAZNP_cMINPRS = dMINPRS * 10;
                //c_일 최소유량
                dAZNP_cMINFLSMC = dMINFLSMC / 3600;
                //AREA
                dAZNP_AREA = (3.14 * (Math.Pow(dAZNP_cDIAM, 2))) / 4;
                //Vel
                dAZNP_Vel = dAZNP_cMINFLSMC / dAZNP_AREA;

                //AZNP = ( 유입관저고 + (c_최소유량시점압력/ Gamma) + (Vel^2 / GA * 2)) - 급수전평균고도
                dAZNP = (dAZNP_INPIPE_ALT + (dAZNP_cMINPRS/dGamma) + (Math.Pow(dAZNP_Vel, 2) / dGA * 2)) - dAZNP_METER_AVG_ALT;

                return dAZNP;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
            }
        }
    }
}
