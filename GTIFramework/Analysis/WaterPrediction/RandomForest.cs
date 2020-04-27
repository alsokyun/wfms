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
// 제      목 : RandomForest 수요량, 공급량 예측분석
// 작  성  자 : 이기연
// 작  성  일 : 2017-09-08
// 모 듈 설 명: 장기예측 분석 - RandomForest 분석 방법 적용 (생산관리, 관망관리)
#region // 개발이력
/*
 *  구분 : 신규개발
 *  일자 : 2017-09-08
 *  작성자 : 이기연
 *  내용 : 최초작성
 *  이슈 : 
 *  구분 : 신규개발
 *  일자 : 2017-09-08
 *  작성자 : 이기연
 *  내용 : 최초작성
 *  이슈 : 1. 2014년 동국대에서 진행하였던 상수도 수량예측 프로그램 로직 발췌
 *         2. 발췌한 로직을 관망관리(Java) 장기수요예측을 R스크립트를 이용하여 분석
 *         3. 생산관리(C#)에서 장기예측(월별예측)을 RDotNet으로 수정
 *         
 *         참조버전 = NuGet - R.NET : 1.7.0
 *                          - R.NET.Community : 1.7.0
 *                    R.Package - Rcpp : 0.12.10
 *                              - zoo : 1.8-0
 *                              - timeDate : 3012.100
 *                              - randomForest : 4.6-12
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
*                              - randomForest : 4.6-14
*/
#endregion
// ====================================

namespace GTIFramework.Analysis.WaterPrediction
{
    public class RandomForest
    {
        /// <summary>
        /// RandomForest(장기예측) 분석 실행
        /// </summary>
        /// <param name="conditions"> h : 30 (예측기간 1달 -> 1 * 30), ntrees : 100, 200, 300 (표본트리), fdate 과거기간시작날짜, edate 과거기간종료날짜 </param>
        /// <returns>분석 실패시 return null</returns>
        public DataTable Run(Hashtable conditions)
        {
            string Rcode = string.Empty;
            DataTable temp = new DataTable();

            try
            {
                REngine.SetEnvironmentVariables();
                REngine rEngine = REngine.GetInstance();

                #region ##중요!!! install.Packages 처음 실행시 Libary Download CRAN미러!!!!!!버전확인!!!!!!
                //Rcode = "install.packages(\"Rcpp\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"zoo\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"timeDate\")";
                //rEngine.Evaluate(Rcode);
                //Rcode = "install.packages(\"randomForest\")";
                //rEngine.Evaluate(Rcode);
                #endregion

                #region repuire 참조 (성공 true, 실패 false) 실패시 Debug 출력 참조
                Rcode = "brepuire1 <- require(zoo)";
                rEngine.Evaluate(Rcode);

                Rcode = "brepuire2 <- require(timeDate)";
                rEngine.Evaluate(Rcode);

                Rcode = "brepuire3 <- require(randomForest)";
                rEngine.Evaluate(Rcode);

                //보여줄 필요 없기에 주석처리
                //Rcode = "list(brepuire1, brepuire2, brepuire3)";
                //rEngine.Evaluate(Rcode);
                #endregion

                #region 분석 Parameter
                //날짜 포멧 설정
                Rcode = "format <- \"%Y-%m-%d\"";
                rEngine.Evaluate(Rcode);

                //예측 기간 설정 (일수 h = 30 이면 30일 예측)
                Rcode = "h <- " + conditions["h"].ToString();
                rEngine.Evaluate(Rcode);

                //표본트리 설정
                Rcode = "ntrees <- " + conditions["ntrees"].ToString();
                rEngine.Evaluate(Rcode);

                //시작날짜 설정
                Rcode = "fdate <- as.Date(\"" + conditions["fdate"].ToString() + "\", format=format)";
                rEngine.Evaluate(Rcode);

                //종료날짜 설정
                Rcode = "edate <- as.Date(\"" + conditions["edate"].ToString() + "\", format=format)";
                rEngine.Evaluate(Rcode);

                Rcode = "n.train <- as.integer(edate-fdate)+1";
                rEngine.Evaluate(Rcode);

                Rcode = "edate <- edate+h";
                rEngine.Evaluate(Rcode);
                #endregion

                #region 분석 데이터 유량데이터, 기상데이터 조회 완료후 Merge
                //DataTable -> dataFrame 변환
                DataTable dailyData = new DataTable();
                dailyData = conditions["dailyData"] as DataTable;
                convertTOFrame(rEngine, "dailyData", dailyData);
                DataFrame test = rEngine.Evaluate("dailyData").AsDataFrame();

                //DataTable -> dataFrame 변환
                DataTable ClimateData = new DataTable();
                ClimateData = conditions["ClimateData"] as DataTable;
                convertTOFrame(rEngine, "ClimateData", ClimateData);
                DataFrame test1 = rEngine.Evaluate("ClimateData").AsDataFrame();

                //d1 데이터 프레임 Column Name 정의
                Rcode = "d1 <- data.frame(TIME=dailyData[,1], X=as.numeric(dailyData[,2]), stringsAsFactors=FALSE)";
                rEngine.Evaluate(Rcode);

                //d2 데이터 프레임 Column Name 정의
                Rcode = "d2 <- data.frame(TIME=ClimateData[,1], MINT=as.numeric(ClimateData[,2]), MAXT=as.numeric(ClimateData[,3]), AVGT=as.numeric(ClimateData[,4]), CLOUD=as.numeric(ClimateData[,5]), RAIN=as.numeric(ClimateData[,6]), stringsAsFactors=FALSE)";
                rEngine.Evaluate(Rcode);

                //d1, d2 TIME으로 Merge
                Rcode = "dbData <- merge(d1, d2, by.Y=\"TIME\", all=TRUE, incomparables = NA)";
                rEngine.Evaluate(Rcode);
                #endregion

                #region 분석
                Rcode = "dbData[,1] <- as.Date(strptime(dbData[,1], format=format))";
                rEngine.Evaluate(Rcode);

                Rcode = "names(dbData)[2]<-\"y\"";
                rEngine.Evaluate(Rcode);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                Rcode = "weekd <- paste(c(\"일\", \"월\", \"화\", \"수\", \"목\", \"금\", \"토\"), \"요일\", sep=\"\")";
                rEngine.Evaluate(Rcode);

                Rcode = "ddd <- data.frame(dbData, WEEK = factor(weekdays(dbData[,1]), levels=weekd))";
                rEngine.Evaluate(Rcode);

                Rcode = "n <- nrow(ddd)";
                rEngine.Evaluate(Rcode);

                Rcode = "trainData <- ddd[1:n.train,]";
                rEngine.Evaluate(Rcode);

                Rcode = "testData <- ddd[(n.train + 1):n,]";
                rEngine.Evaluate(Rcode);
                
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                Rcode = "set.seed(1234567)";
                rEngine.Evaluate(Rcode);

                Rcode = "rf <- randomForest(y ~ .,data=trainData, ntree=ntrees, proximity=TRUE, na.action = na.omit)";
                rEngine.Evaluate(Rcode);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                Rcode = "pred <- predict(rf, newdata=testData)";
                rEngine.Evaluate(Rcode);

                Rcode = "test.err <- mean((pred - testData$y) ^ 2, na.rm = T)";
                rEngine.Evaluate(Rcode);

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                Rcode = "imp <- importance(rf)";
                rEngine.Evaluate(Rcode);

                Rcode = "o <- order(imp, decreasing =T)";
                rEngine.Evaluate(Rcode);

                Rcode = "imp <-imp[o,]";
                rEngine.Evaluate(Rcode);

                Rcode = "impvar <- as.character(names(imp))";
                rEngine.Evaluate(Rcode);

                Rcode = "trainData[,1] <- as.character(trainData[,1])";
                rEngine.Evaluate(Rcode);
                #endregion

                DataFrame dfpred = rEngine.Evaluate("data.frame(pred = round(pred, 2))").AsDataFrame();
                DataTable dtpred = new DataTable();
                dtpred = convertTOtable(dfpred);

                #region 결과리턴
                if (dtpred.Rows.Count == 0) return null;
                else return dtpred;
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
