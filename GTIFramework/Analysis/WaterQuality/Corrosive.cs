using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ====================================
// 제      목 : 부식성 알고리즘 Class
// 작  성  자 : 이기연
// 작  성  일 : 2017-03-14
// 모 듈 설 명: 수질관리 솔루션에 들어가는 부식성지수 분석 Class
#region // 개발이력
/*
 *  구분 : 신규개발
 *  일자 : 2017-03-14
 *  작성자 : 이기연
 *  내용 : 최초작성
 *  이슈 : 
 */
#endregion
// ====================================

namespace GTIFramework.Analysis.WaterQuality
{
    public class Corrosive
    {
        /// <summary>
        /// 제목 : 부식성 알고리즘 Main
        /// 내용 : 부식성 지수를 분석하기 위한 필수 인자값을 DataTable로 받아
        ///        부식성 지수를 분석후 DataTable을 리턴
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static DataTable AnalCorrosive(DataTable temp)
        {
            try
            {
                //샘플데이터 (적용시에는 사용자 입력데이터 테이블 구조 맞춰야 됨)
                //temp.Columns.Add("SPOT");  //지점
                //temp.Columns.Add("Ca2+");  //Ca2+
                //temp.Columns.Add("ALK");   //알카리
                //temp.Columns.Add("CON");   //전기전도도
                //temp.Columns.Add("WTEMP"); //수온
                //temp.Columns.Add("pH");    //PH
                //temp = SampleData();

                #region 부식성지수 계산
                temp.Columns.Add("LI");             //부식성지수
                temp.Columns.Add("pHs");            //pHs
                temp.Columns.Add("MCa2+");          //Ca2+ (g-mole/L)
                temp.Columns.Add("MALK");           //alkalinit (g-mole/L)
                temp.Columns.Add("pCa2+");          //pCa2+
                temp.Columns.Add("Palkalinity");    //Palkalinity
                temp.Columns.Add("T");              //절대온도
                temp.Columns.Add("E");              //유전상수
                temp.Columns.Add("A");              //A
                temp.Columns.Add("I");              //이온강도
                temp.Columns.Add("pfm");            //pfm
                temp.Columns.Add("logT");           //log10(T)
                temp.Columns.Add("Pk2");            //Pk2
                temp.Columns.Add("pKw");            //pKw
                temp.Columns.Add("pKsc");           //pKsc
                temp.Columns.Add("HCO3-");          //HCO3-
                temp.Columns.Add("PHCO3-");         //PHCO3-
                temp.Columns.Add("K2");             //K2
                temp.Columns.Add("Ks");             //Ks

                foreach (DataRow r in temp.Rows)
                {
                    //temp.Columns.Add("Ca2+");  //Ca2+
                    //temp.Columns.Add("ALK");   //알카리
                    //temp.Columns.Add("CON");   //전기전도도
                    //temp.Columns.Add("WTEMP"); //수온
                    //temp.Columns.Add("pH");    //PH

                    if ( r["Ca2+"]!=DBNull.Value 
                        && r["ALK"] != DBNull.Value
                        && r["CON"] != DBNull.Value
                        && r["WTEMP"] != DBNull.Value
                        && r["pH"] != DBNull.Value )
                    {
                        //순서(1) : MCa2+, MALK, T, I
                        r["MCa2+"] = Convert.ToDouble(r["Ca2+"]) / 40000;                                                                                           // MCa2 = Ca2+ / 40000
                        r["MALK"] = Convert.ToDouble(r["ALK"]) / 50000;                                                                                             // MALK = ALK / 50000
                        r["T"] = Convert.ToDouble(r["WTEMP"]) + 273.15;                                                                                             // T = WTEMP + 273.15
                        r["I"] = Convert.ToDouble(r["CON"]) / 40000;                                                                                                // I = CON / 40000

                        //순서(2) : pCa2+, Palkalinity, E, logT, pKw
                        r["pCa2+"] = -Math.Log10(Convert.ToDouble(r["MCa2+"]));                                                                                      // pCa2+ = LOG10(MCa2+)
                        r["Palkalinity"] = -Math.Log10(Convert.ToDouble(r["MALK"]));                                                                                 // Palkalinity = LOG10(MALK)
                        r["E"] = (60954 / (Convert.ToDouble(r["T"]) + 116)) - 68.937;                                                                               // E = (60954 / (T + 116)) - 68.937
                        r["logT"] = Math.Log10(Convert.ToDouble(r["T"]));                                                                                           // logT = LOG10(T)
                        r["pKw"] = (4471 / Convert.ToDouble(r["T"])) + (0.01706 * Convert.ToDouble(r["T"])) - 6.0875;                                               // pKw = (4471 / T) + (0.01706 * T) - 6.0875

                        //순서(3) : A, Pk2, pKsc
                        r["A"] = 1820000 * Math.Pow((Convert.ToDouble(r["E"]) * Convert.ToDouble(r["T"])), -1.5);                                                   // A = 1820000 * ( E * T ) ^ -1.5
                        r["Pk2"] = 107.8871
                            + (0.03252849 * Convert.ToDouble(r["T"]))
                            - (5151.79 / Convert.ToDouble(r["T"]))
                            - (38.92562 * Convert.ToDouble(r["logT"]))
                            + (563713.9 / Math.Pow(Convert.ToDouble(r["T"]), 2));                                                                                    // Pk2 = 107.8871 + (0.03252849*T) - (5151.79/T) - (38.92562*LogT) + (563713.9/T^2)
                        r["pKsc"] = 171.9065
                            + (0.077993 * Convert.ToDouble(r["T"]))
                            - (2839.319 / Convert.ToDouble(r["T"]))
                            - (71.595 * Convert.ToDouble(r["logT"]));                                                                                               // pKsc = 171.9065 + (0.077993*T) - (2839.319/T) - (71.595*LogT)

                        //순서(4) : pfm, K2, Ks
                        r["pfm"] = Convert.ToDouble(r["A"])
                            * ((Math.Pow(Convert.ToDouble(r["I"]), 0.5) / (1 + Math.Pow(Convert.ToDouble(r["I"]), 0.5))) - (0.3 * Convert.ToDouble(r["I"])));      // pfm = A * ( (I^0.5/(1+I^0.5)) - (0.3*I) )
                        r["K2"] = Math.Pow(10, -(Convert.ToDouble(r["Pk2"])));                                                                                      // K2 = 10^-(Pk2)
                        r["Ks"] = Math.Pow(10, -(Convert.ToDouble(r["pKsc"])));                                                                                     // K2 = 10^-(pKsc)

                        //순서(5) : HCO3-, pHs
                        r["HCO3-"] = (Convert.ToDouble(r["MALK"])
                            + Math.Pow(10, (Convert.ToDouble(r["pfm"]) - Convert.ToDouble(r["pH"])))
                            - Math.Pow(10, (Convert.ToDouble(r["pH"]) + Convert.ToDouble(r["pfm"])
                            - Convert.ToDouble(r["pKw"])))) / (1 + (0.5 * Math.Pow(10, (Convert.ToDouble(r["pH"]) - Convert.ToDouble(r["Pk2"])))));                 // HCO3- = (MALK + 10 ^ (pfm - pH) - 10 ^ (pH + pfm - pKw)) / (1 + (0.5 * 10 ^ (pH - Pk2)))
                        r["pHs"] = Convert.ToDouble(r["Pk2"])
                            - Convert.ToDouble(r["pKsc"])
                            + Convert.ToDouble(r["pCa2+"])
                            + Convert.ToDouble(r["Palkalinity"])
                            + (5 * Convert.ToDouble(r["pfm"]));                                                                                                     // pHs = Pk2 - pKsc + pCa2+ + Palkalinity + (5 * pfm)

                        //순서(6) : PHCO3-
                        r["PHCO3-"] = -Math.Log10(Convert.ToDouble(r["HCO3-"]));                                                                                    // PHCO3- = -LOG10(HCO3-)
                        r["LI"] = Convert.ToDouble(r["pH"]) - Convert.ToDouble(r["pHs"]);                                                                           // LI = pH - pHs

                        //소수점 정리
                        r["MCa2+"] = Math.Round(Convert.ToDouble(r["MCa2+"]), 6, MidpointRounding.AwayFromZero);
                        r["MALK"] = Math.Round(Convert.ToDouble(r["MALK"]), 5, MidpointRounding.AwayFromZero);
                        r["T"] = Math.Round(Convert.ToDouble(r["T"]), 2, MidpointRounding.AwayFromZero);
                        r["I"] = Math.Round(Convert.ToDouble(r["I"]), 4, MidpointRounding.AwayFromZero);
                        r["pCa2+"] = Math.Round(Convert.ToDouble(r["pCa2+"]), 2, MidpointRounding.AwayFromZero);
                        r["Palkalinity"] = Math.Round(Convert.ToDouble(r["Palkalinity"]), 2, MidpointRounding.AwayFromZero);
                        r["E"] = Math.Round(Convert.ToDouble(r["E"]), 4, MidpointRounding.AwayFromZero);
                        r["logT"] = Math.Round(Convert.ToDouble(r["logT"]), 2, MidpointRounding.AwayFromZero);
                        r["pKw"] = Math.Round(Convert.ToDouble(r["pKw"]), 2, MidpointRounding.AwayFromZero);
                        r["A"] = Math.Round(Convert.ToDouble(r["A"]), 4, MidpointRounding.AwayFromZero);
                        r["Pk2"] = Math.Round(Convert.ToDouble(r["Pk2"]), 2, MidpointRounding.AwayFromZero);
                        r["pKsc"] = Math.Round(Convert.ToDouble(r["pKsc"]), 2, MidpointRounding.AwayFromZero);
                        r["pfm"] = Math.Round(Convert.ToDouble(r["pfm"]), 4, MidpointRounding.AwayFromZero);
                        r["K2"] = string.Format("{0:E2}", Convert.ToDouble(r["K2"]));
                        r["Ks"] = string.Format("{0:E2}", Convert.ToDouble(r["Ks"]));
                        r["HCO3-"] = Math.Round(Convert.ToDouble(r["HCO3-"]), 4, MidpointRounding.AwayFromZero);
                        r["pHs"] = Math.Round(Convert.ToDouble(r["pHs"]), 3, MidpointRounding.AwayFromZero);
                        r["PHCO3-"] = Math.Round(Convert.ToDouble(r["PHCO3-"]), 4, MidpointRounding.AwayFromZero);
                        r["LI"] = Math.Round(Convert.ToDouble(r["LI"]), 2, MidpointRounding.AwayFromZero);
                    }
                }
                #endregion

                return temp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sample Data생성 (개발중에만 사용)
        /// </summary>
        private static DataTable SampleData()
        {
            #region 샘플 데이터
            DataTable temp = new DataTable();

            //컬럼생성
            temp.Columns.Add("SPOT");  //지점
            temp.Columns.Add("Ca2+");  //Ca2+
            temp.Columns.Add("ALK");   //알카리
            temp.Columns.Add("CON");   //전기전도도
            temp.Columns.Add("WTEMP"); //수온
            temp.Columns.Add("pH");    //PH

            //데이터 생성
            DataRow r = temp.NewRow();
            r["SPOT"] = "회야정수장";
            r["Ca2+"] = 20.7;
            r["ALK"] = 46;
            r["CON"] = 274;
            r["WTEMP"] = 26.7;
            r["pH"] = 6.9;
            temp.Rows.Add(r);

            DataRow r1 = temp.NewRow();
            r1["SPOT"] = "천상1정수";
            r1["Ca2+"] = 11.9;
            r1["ALK"] = 45;
            r1["CON"] = 137;
            r1["WTEMP"] = 23.7;
            r1["pH"] = 6.8;
            temp.Rows.Add(r1);

            DataRow r2 = temp.NewRow();
            r2["SPOT"] = "천상2정수";
            r2["Ca2+"] = 12.6;
            r2["ALK"] = 41;
            r2["CON"] = 136;
            r2["WTEMP"] = 24.3;
            r2["pH"] = 6.94;
            temp.Rows.Add(r2);

            DataRow r3 = temp.NewRow();
            r3["SPOT"] = "회야표층";
            r3["Ca2+"] = 18.9;
            r3["ALK"] = 51;
            r3["CON"] = 266;
            r3["WTEMP"] = 26.4;
            r3["pH"] = 8.0;
            temp.Rows.Add(r3);

            DataRow r4 = temp.NewRow();
            r4["SPOT"] = "회야26m";
            r4["Ca2+"] = 18.8;
            r4["ALK"] = 52;
            r4["CON"] = 268;
            r4["WTEMP"] = 27.6;
            r4["pH"] = 7.8;
            temp.Rows.Add(r4);

            DataRow r5 = temp.NewRow();
            r5["SPOT"] = "회야20m";
            r5["Ca2+"] = 18.9;
            r5["ALK"] = 49;
            r5["CON"] = 268;
            r5["WTEMP"] = 27.5;
            r5["pH"] = 7.7;
            temp.Rows.Add(r5);

            DataRow r6 = temp.NewRow();
            r6["SPOT"] = "회야원수";
            r6["Ca2+"] = 19;
            r6["ALK"] = 46;
            r6["CON"] = 266;
            r6["WTEMP"] = 26.6;
            r6["pH"] = 7.4;
            temp.Rows.Add(r6);

            DataRow r7 = temp.NewRow();
            r7["SPOT"] = "천상일반원수";
            r7["Ca2+"] = 12;
            r7["ALK"] = 43;
            r7["CON"] = 126;
            r7["WTEMP"] = 23.8;
            r7["pH"] = 7.1;
            temp.Rows.Add(r7);

            DataRow r8 = temp.NewRow();
            r8["SPOT"] = "천상고도원수";
            r8["Ca2+"] = 12.5;
            r8["ALK"] = 45;
            r8["CON"] = 126;
            r8["WTEMP"] = 23.8;
            r8["pH"] = 7.1;
            temp.Rows.Add(r8);

            DataRow r9 = temp.NewRow();
            r9["SPOT"] = "사연표층";
            r9["Ca2+"] = 12.3;
            r9["ALK"] = 55;
            r9["CON"] = 124;
            r9["WTEMP"] = 24.5;
            r9["pH"] = 8.6;
            temp.Rows.Add(r9);

            return temp;
            #endregion
        }
    }
}
