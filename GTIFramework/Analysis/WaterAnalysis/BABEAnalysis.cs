using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTIFramework.Analysis.WaterAnalysis
{
    public class BABEAnalysis
    {
        public BABEAnalysis()
        {
            //1.배경누수량 산정
            //1)AZNP 조회
            //2)PFC 계산
            //3)ICF 값 선정
            //4)배경누수량 산정

            //2.파열누수량 산정
            //1)야간최소유량 조회
            //2)야간사용량 산정
            //3)무수수량 산정
            //4)파열누수량 산정

            //3.BABE 분석
        }

        public BABEAnalysis(string asd)
        {
            //1.배경누수량 산정
            //1)AZNP 조회
            //2)PFC 계산
            //3)ICF 값 선정
            //4)배경누수량 산정

            //2.파열누수량 산정
            //1)야간최소유량 조회
            //2)야간사용량 산정
            //3)무수수량 산정
            //4)파열누수량 산정

            //3.BABE 분석
        }

        //Param
        double dICF;
        double dAZNP;
        double dNMF;
        double dNIGHT_PRS;
        //double dAVG_PRS;
        DataTable dtTENMINPRS;
        double dDRAINPIPE_LEN;
        double dPIPE_CNT;
        double dINHOUSEPIPE_LEN;
        double dDRAINPIPE_BG_LEAK_QTY_COEF;
        double dINPIPE_BG_LEAK_QTY_COEF;
        double dINHOUSEPIPE_BG_LEAK_QTY_COEF;
        double dMETER_CNT;

        double dHOUSE_NIGHT_USE_QTY;

        double? dPCF;
        double? dNightUse;
        double? dBackLaek;
        double? dBurstLaek;
        double? dBABEResult;
        
        DataTable dtresult;
        DataRow dradd;

        bool bMonthResult = false;

        public DataTable BabeAnalysis(Hashtable htBABEParam)
        {
            try
            {
                Hashtable htResult = new Hashtable();

                dtresult = null;
                dtresult = new DataTable();

                dtTENMINPRS = null;
                dtTENMINPRS = new DataTable();

                dPCF = null;
                dNightUse = null;
                dBackLaek = null;
                dBurstLaek = null;
                dBABEResult = null;
                bMonthResult = false;

                //Param 정리
                dICF = Convert.ToDouble(htBABEParam["ICF"].ToString());
                dAZNP = Convert.ToDouble(htBABEParam["AZNP"].ToString());
                dNMF = Convert.ToDouble(htBABEParam["NMF"].ToString());
                dNIGHT_PRS = Convert.ToDouble(htBABEParam["NIGHT_PRS"].ToString());
                dtTENMINPRS = htBABEParam["TENMINPRS"] as DataTable;
                dHOUSE_NIGHT_USE_QTY = Convert.ToDouble(htBABEParam["HOUSE_NIGHT_USE_QTY"].ToString());
                dPIPE_CNT = Convert.ToDouble(htBABEParam["PIPE_CNT"].ToString());
                dDRAINPIPE_LEN = Convert.ToDouble(htBABEParam["DRAINPIPE_LEN"].ToString());

                //1.배경누수량 산정
                //[(AZNP*0.5) + (AZNP^2*0.0042)] / 35.5
                dPCF = ((dAZNP * 0.5) + (Math.Pow(dAZNP, 2) * 0.0042)) / 35.5;

                //dPCF 존재시 분석
                if (dPCF != null)
                {
                    if(double.TryParse(htBABEParam["INHOUSEPIPE_LEN"].ToString(), out dINHOUSEPIPE_LEN))
                    {
                        //(ICF  *  PCF * (( 배수관망연장(km)/1000 * 20) + ( 급수관수 * 1.25 ) + ( 옥내급수관연장(km)/1000 * 33 )))
                        //dBackLaek = (dICF * dPCF * ((dDRAINPIPE_LEN / 1000 * 20) + (dPIPE_CNT * 1.25) + (dINHOUSEPIPE_LEN / 1000 * 33)));

                        //(ICF * 배수관망연장 * 20 + (급수관수 * 1.25) + (옥내급수관연장 * 0.033)) / 1000 * PCF
                        dBackLaek = (dICF * dDRAINPIPE_LEN * 20 + (dPIPE_CNT * 1.25) + (dINHOUSEPIPE_LEN * 0.033)) / 1000 * dPCF;
                    }
                    else if(double.TryParse(htBABEParam["METER_CNT"].ToString(), out dMETER_CNT)
                        && double.TryParse(htBABEParam["DRAINPIPE_BG_LEAK_QTY_COEF"].ToString(), out dDRAINPIPE_BG_LEAK_QTY_COEF)
                        && double.TryParse(htBABEParam["INPIPE_BG_LEAK_QTY_COEF"].ToString(), out dINPIPE_BG_LEAK_QTY_COEF)
                        && double.TryParse(htBABEParam["INHOUSEPIPE_BG_LEAK_QTY_COEF"].ToString(), out dINHOUSEPIPE_BG_LEAK_QTY_COEF))
                    {
                        //(ICF * PCF * (( 배수관망연장(km)/1000 * 배수관배경누수량계수 ) + ( 인입급수관배경누수량계수 + 옥내배경누수량계수 ) * 급수전수 ))
                        dBackLaek = (dICF * dPCF * ((dDRAINPIPE_LEN / 1000 * dDRAINPIPE_BG_LEAK_QTY_COEF) + (dINPIPE_BG_LEAK_QTY_COEF + dINHOUSEPIPE_BG_LEAK_QTY_COEF) * dMETER_CNT));
                    }
                    //분석에 필요한 Param이 없을경우 null 리턴
                    else
                    {
                        return null;
                    }
                }
                //없을경우 null 리턴
                else
                {
                    return null;
                }

                //2.파열누수량 산정 (분기만 해놓고 안함)
                //월분석 결과가 있을경우
                if (bMonthResult)
                {

                }
                //월분석 결과가 없을경우
                else if (!bMonthResult)
                {
                    //야간사용량
                    //변경시 데이터관리, 야간최소유량 수정 필요
                    dNightUse = dPIPE_CNT * dHOUSE_NIGHT_USE_QTY / 1000;

                    //파열 누수량 
                    dBurstLaek = dNMF - dNightUse - dBackLaek;
                }

                //3.BABE 분석 진행
                dtresult.Columns.Add("ANALY_DT");
                dtresult.Columns.Add("PRS");
                dtresult.Columns.Add("FLSMC");
                dtresult.Columns.Add("dNightUse");
                dtresult.Columns.Add("dBackLaek");
                dtresult.Columns.Add("dBurstLaek");
                dtresult.Columns.Add("dBABEResult");

                foreach (DataRow dr in dtTENMINPRS.Rows)
                {
                    dBABEResult = (dBackLaek + dBurstLaek) / 24 * Math.Pow((Convert.ToDouble(dr["PRS"].ToString()) / dNIGHT_PRS), 1.5);

                    dradd = dtresult.NewRow();
                    dradd["ANALY_DT"] = dr["MESR_TM"].ToString();
                    dradd["PRS"] = Convert.ToDouble(dr["PRS"].ToString());
                    dradd["FLSMC"] = Convert.ToDouble(dr["FLSMC"].ToString());
                    dradd["dNightUse"] = dNightUse;
                    dradd["dBackLaek"] = dBackLaek;
                    dradd["dBurstLaek"] = dBurstLaek;
                    dradd["dBABEResult"] = dBABEResult;
                    dtresult.Rows.Add(dradd.ItemArray);
                }

                //htResult.Add("dNightUse", dNightUse);
                //htResult.Add("dBackLaek", dBackLaek);
                //htResult.Add("dBurstLaek", dBurstLaek);
                //htResult.Add("dBABEResult", dBABEResult);

                return dtresult;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
            }
        }
    }
}
