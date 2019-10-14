using RDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// ====================================
// 제      목  : HoltWinters 수요량, 공급량 예측분석
// 작  성  자  : 이기연
// 작  성  일  : 2017-09-07
// 모 듈 설 명 : 단기예측 분석 - HoltWinters 분석 방법 적용 (생산관리, 관망관리)
#region // 개발이력
/*
 *  구분 : 신규개발
 *  일자 : 2017-09-07
 *  작성자 : 이기연
 *  내용 : 최초작성
 *  이슈 : 1. 2014년 동국대에서 진행하였던 상수도 수량예측 프로그램 로직 발췌
 *         2. 발췌한 로직을 관망관리(Java) 단기수요예측을 R스크립트를 이용하여 분석
 *         3. 생산관리(C#)에서 단기예측(일별예측)을 RDotNet으로 수정
 *         
 *         참조버전 = NuGet - R.NET : 1.7.0
 *                          - R.NET.Community : 1.7.0
 *                    R.Package - Rcpp : 0.12.10
 *                              - zoo : 1.8-0
 *                              - timeDate : 3012.100
 *                              - forecast : 8.0
 */

/*
*  구분 : R버전 변경
*  일자 : 2019-04-16
*  작성자 : 이기연
*  내용 : 참조버전 = NuGet - R.NET : 1.7.0
*                          - R.NET.Community : 1.7.0
*                    R설치 파일 - R-3.4.4-win.exe (32,64 전부)
*                    R.Package - Rcpp : 1.0.1
*                              - zoo : 1.8-5
*                              - timeDate : 3043.102
*                              - forecast : 8.5
*/
#endregion
// ====================================

namespace GTIFramework.Analysis.WaterPrediction
{
    public class HoltWinters
    {



        /// <summary>
        /// HoltWinters(단기예측) 분석 실행
        /// </summary>
        /// <returns>분석 실패시 return null, 과거기간으로 2일치(24시간) 예측</returns>
        public DataTable Run(DataTable dtrawdata)
        {
            string Rcode = string.Empty;
            DataTable dtresult = new DataTable();

            try
            {
                string trans = "none";
                REngine.SetEnvironmentVariables();
                REngine rEngine = REngine.GetInstance();
                rEngine.Initialize();

                #region ##중요!!! install.Packages 처음 실행시 Libary Download CRAN미러!!!!!!버전확인!!!!!!
                //Rcode = "install.packages(\"Rcpp\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"zoo\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"timeDate\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"forecast\")";
                //rEngine.Evaluate(Rcode);
                #endregion

                #region repuire 참조 (성공 true, 실패 false) 실패시 Debug 출력 참조
                Rcode = "brepuire1 <- require(zoo)";
                rEngine.Evaluate(Rcode);

                Rcode = "brepuire2 <- require(timeDate)";
                rEngine.Evaluate(Rcode);

                Rcode = "brepuire3 <- require(forecast)";
                rEngine.Evaluate(Rcode);

                //보여줄 필요 없기에 주석처리
                //Rcode = "list(brepuire1, brepuire2, brepuire3)";
                //rEngine.Evaluate(Rcode);

                #endregion

                #region 분석 Parameter
                Rcode = "h <- 48";
                rEngine.Evaluate(Rcode);

                Rcode = "freq = 24";
                rEngine.Evaluate(Rcode);

                Rcode = "jump <- 60*60";
                rEngine.Evaluate(Rcode);

                Rcode = "ll <- 13";
                rEngine.Evaluate(Rcode);

                Rcode = "ylab <- \"유량\"";
                rEngine.Evaluate(Rcode);

                Rcode = "xlab <- \"Time\"";
                rEngine.Evaluate(Rcode);

                Rcode = "trans=\"" + trans + "\"";
                rEngine.Evaluate(Rcode);
                #endregion

                #region 분석 Data Select
                //csv 선택
                //Rcode = "rawdata <- read.table(file.choose(), header=TRUE, sep = ',')";
                //rEngine.Evaluate(Rcode);

                //DataTable -> dataFrame 변환
                convertTOFrame(rEngine, "rawdata", dtrawdata);

                Rcode = "list(rawdata)";
                rEngine.Evaluate(Rcode);
                #endregion

                #region 분석
                Rcode = "x <- rawdata[, 1]";
                rEngine.Evaluate(Rcode);

                Rcode = "y <- as.numeric(rawdata[, 2])";
                rEngine.Evaluate(Rcode);

                Rcode = "x <- strptime(x, format=\"%Y-%m-%d %H\")";
                rEngine.Evaluate(Rcode);

                Rcode = "time.pred <- substring(as.character(strptime(x[length(x)]+1:h*jump, format=\"%Y-%m-%d %H\")), 1, ll)";
                rEngine.Evaluate(Rcode);

                if (trans.Equals("none"))
                {
                    Rcode = "fun <- \"\"";
                    rEngine.Evaluate(Rcode);

                    Rcode = "ytr <- y";
                    rEngine.Evaluate(Rcode);
                }
                else
                {
                    Rcode = "fun <- trans";
                    rEngine.Evaluate(Rcode);

                    Rcode = "ytr <- eval(call(fun, y))";
                    rEngine.Evaluate(Rcode);

                    Rcode = "ylab <- paste(fun, \"(\", ylab,\")\", sep=\"\")";
                    rEngine.Evaluate(Rcode);
                }

                Rcode = "fit <- HoltWinters(ts(ytr, freq=freq), start.periods=2, seasonal=\"additive\")";
                rEngine.Evaluate(Rcode);

                //Rcode = "pred <- forecast.HoltWinters(fit, h=h, level=80)";
                //rEngine.Evaluate(Rcode);

                Rcode = "pred <- forecast(fit, h=h, level=80)";
                rEngine.Evaluate(Rcode);

                Rcode = "ylim1 <- min(c(min(ytr), min(pred$lower[,1])))";
                rEngine.Evaluate(Rcode);

                Rcode = "ylim2 <- max(c(max(ytr), max(pred$upper[,1])))";
                rEngine.Evaluate(Rcode);

                Rcode = "ylim  <- c(ylim1, ylim2)";
                rEngine.Evaluate(Rcode);

                Rcode = "p.x <- 1:h + length(ytr);";
                rEngine.Evaluate(Rcode);

                Rcode = "xh <- x[length(x)]+1:h*jump";
                rEngine.Evaluate(Rcode);

                Rcode = "xt <- format(c(x, xh), \"%b-%d %H\")";
                NumericVector xt = rEngine.Evaluate(Rcode).AsNumeric();

                if (xt.Length > 20)
                {
                    Rcode = "ix <- seq(1, length(xt), length=20)";
                    rEngine.Evaluate(Rcode);
                }
                else
                {
                    Rcode = "ix <- seq(1, length(xt))";
                    rEngine.Evaluate(Rcode);
                }

                Rcode = "a <- as.data.frame(pred)";
                rEngine.Evaluate(Rcode);

                Rcode = "pred <- data.frame(round(a,2))";
                rEngine.Evaluate(Rcode);

                Rcode = "colname <- c(\"Time\",\"Forecast\",\"-20% lower\",\"+20% upper\")";
                rEngine.Evaluate(Rcode);

                Rcode = "list(x=time.pred, init=round(c(fit$alpha, fit$beta, fit$gamma),8), coeff=round(fit$coefficients,4), pred=pred, colname=colname, fit.method=\"Holt Winters\")";
                rEngine.Evaluate(Rcode);
                #endregion

                #region 분석 결과 가공 DataTable Type
                DataFrame dfpred = rEngine.Evaluate("data.frame(pred)").AsDataFrame();
                DataTable dtpred = new DataTable();
                dtpred = convertTOtable(dfpred);

                DataFrame dfx = rEngine.Evaluate("data.frame(x = time.pred)").AsDataFrame();
                DataTable dtx = new DataTable();
                dtx = convertTOtable(dfx);

                dtpred.Columns.Add("DT");
                dtpred.Columns["DT"].SetOrdinal(0);

                if (dtpred.Rows.Count == dtx.Rows.Count)
                {
                    dtresult.Columns.Add("DT");
                    dtresult.Columns.Add("Forecast");
                    dtresult.Columns.Add("Lo80");
                    dtresult.Columns.Add("Hi80");

                    foreach (DataRow r in dtpred.Rows)
                    {
                        int idxr = dtpred.Rows.IndexOf(r);

                        DataRow addr = dtresult.NewRow();
                        addr["DT"] = dtx.Rows[idxr]["x"];
                        addr["Forecast"] = dtpred.Rows[idxr]["Point.Forecast"];
                        addr["Lo80"] = dtpred.Rows[idxr]["Lo.80"];
                        addr["Hi80"] = dtpred.Rows[idxr]["Hi.80"];
                        dtresult.Rows.Add(addr);
                    }
                }
                #endregion

                #region 결과리턴
                if (dtresult.Rows.Count == 0) return null;
                else return dtresult;
                #endregion
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        /// <summary>
        /// DataFrame -> DataTable 변환
        /// </summary>
        /// <param name="dfdataset"></param>
        /// <returns></returns>
        private DataTable convertTOtable(DataFrame dfdataset)
        {
            DataTable dttemp = new DataTable();

            try
            {
                for (int i = 0; i < dfdataset.ColumnCount; i++)
                {
                    dttemp.Columns.Add(dfdataset.ColumnNames[i]);
                }

                for (int i = 0; i < dfdataset.RowCount; i++)
                {
                    DataRow r = dttemp.NewRow();

                    for (int j = 0; j < dfdataset.ColumnCount; j++)
                    {
                        r[j] = dfdataset[i, j];
                    }

                    dttemp.Rows.Add(r);
                }

                return dttemp;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        /// <summary>
        /// DataTable -> DataFrame 변환
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dtdata"></param>
        /// <returns></returns>
        private DataFrame convertTOFrame(REngine rEngine, string name, DataTable dtdata)
        {
            try
            {
                DataFrame dataFrame = null;
                IEnumerable[] columns = new IEnumerable[dtdata.Columns.Count];
                string[] columnNames = dtdata.Columns.Cast<DataColumn>()
                                       .Select(x => x.ColumnName)
                                       .ToArray();
                for (int i = 0; i < dtdata.Columns.Count; i++)
                {
                    switch (Type.GetTypeCode(dtdata.Columns[i].DataType))
                    {
                        case TypeCode.String:
                            columns[i] = dtdata.Rows.Cast<DataRow>().Select(row => row.Field<string>(i)).ToArray();
                            break;
                        case TypeCode.Double:
                            columns[i] = dtdata.Rows.Cast<DataRow>().Select(row => row.Field<double>(i)).ToArray();
                            break;
                        case TypeCode.Int32:
                            columns[i] = dtdata.Rows.Cast<DataRow>().Select(row => row.Field<int>(i)).ToArray();
                            break;
                        case TypeCode.Decimal:
                            columns[i] = dtdata.Rows.Cast<DataRow>().Select(row => Convert.ToDouble(row.Field<decimal>(i))).ToArray();
                            break;
                        default:
                            columns[i] = dtdata.Rows.Cast<DataRow>().Select(row => row[i]).ToArray();
                            break;
                    }
                }

                dataFrame = rEngine.CreateDataFrame(columns: columns, columnNames: columnNames, stringsAsFactors: false);
                rEngine.SetSymbol(name, dataFrame);

                return dataFrame;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        #region csv 읽어오기 test
        static DataTable GetDataTableFromCsv()
        {
            OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            DialogResult result = openFileDialog.ShowDialog();

            DataTable data = new DataTable();

            if (result == DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                {
                    var reader = ReadAsLines(openFileDialog.FileName);

                    //this assume the first record is filled with the column names
                    var headers = reader.First().Split(',');
                    foreach (var header in headers)
                    {
                        data.Columns.Add(header);
                    }

                    var records = reader.Skip(1);
                    foreach (var record in records)
                    {
                        data.Rows.Add(record.Split(','));
                    }
                }
            }

            return data;
        }

        static IEnumerable<string> ReadAsLines(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
                while (!reader.EndOfStream)
                    yield return reader.ReadLine();
        }
        #endregion
    }
}
