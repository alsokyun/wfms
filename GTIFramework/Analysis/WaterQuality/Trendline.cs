using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTIFramework.Analysis.WaterQuality
{
    public class Trendline
    {
        //원점 (0,0)를 지나는 추세선 (기존 LID)
        public static Hashtable PassZeroPointTrend(string keyFieldName, string xFieldName, string yFieldName, DataTable sourceData)
        {
            Hashtable result = new Hashtable();          // 분석에 필요한 DataTable
            DataTable trendData = new DataTable();       // 분석 결과를 담을 HashTable
            trendData.Columns.Add(keyFieldName);
            trendData.Columns.Add(xFieldName);
            trendData.Columns.Add(yFieldName);
            trendData.Columns.Add("TREND");

            //분석에 필요한 인자들 선언 및 계산식 주석
            int cnt = 0;                           // 평균을 계산을 위한 카운트  
            double xySum = 0;                      // Sum(XValue * Yvalue)
            double xSum = 0;                       // Sum(XValue)
            double ySum = 0;                       // Sum(YValue)
            double yAvg = 0;                       // Sum(YValue)/cnt
            double a = 0;                          // 기울기 a = xySum / xPow2Sum^2
            double R = 0;                          // 결정계수  R^2 = 1 - ( Sum((Yi-Yr)^2) / Sum((Yi-Yavg)^2) ) 
            double YiYavgSum = 0;                  // Sum(YiYavg) = Sum( (YValue-yAvg)^2 )
            double YiYrSum = 0;                    // Sum(YiYr) = Sum( (YValue-X(Value)*a)^2 )
            double xPow2Sum = 0;                   // Sum(x^2)

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //트렌드 데이서 생성 및 y,x,xy 의 총합
            foreach (DataRow row in sourceData.Rows)
            {
                if ("".Equals(row[keyFieldName].ToString()) || "".Equals(row[xFieldName].ToString()) || "".Equals(row[yFieldName].ToString())) continue;

                DataRow newRow = trendData.NewRow();
                newRow[keyFieldName] = row[keyFieldName];
                newRow[xFieldName] = row[xFieldName];
                newRow[yFieldName] = row[yFieldName];

                double xValue = double.Parse(row[xFieldName].ToString());
                double yValue = double.Parse(row[yFieldName].ToString());

                cnt = cnt + 1;

                xSum = xSum + xValue;
                ySum = ySum + yValue;
                xySum = xySum + (xValue * yValue);
                xPow2Sum = xPow2Sum + Math.Pow(xValue, 2);

                trendData.Rows.Add(newRow);
            }
            yAvg = ySum / cnt;               // y값 평균
            a = xySum / xPow2Sum;            // 추세선 기울기 산정

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //YiYavgSum, YiYrSum 산정
            foreach (DataRow row in sourceData.Rows)
            {
                if ("".Equals(row[keyFieldName].ToString()) || "".Equals(row[xFieldName].ToString()) || "".Equals(row[yFieldName].ToString())) continue;

                DataRow newRow = trendData.NewRow();
                newRow[keyFieldName] = row[keyFieldName];
                newRow[xFieldName] = row[xFieldName];
                newRow[yFieldName] = row[yFieldName];

                double xValue = double.Parse(row[xFieldName].ToString());
                double yValue = double.Parse(row[yFieldName].ToString());

                YiYavgSum = YiYavgSum + Math.Pow((yValue - yAvg), 2);
                YiYrSum = YiYrSum + Math.Pow(yValue - (xValue * a), 2);
            }

            R = 1 - (YiYrSum / YiYavgSum);

            //추세선데이터 
            foreach (DataRow row in trendData.Rows)
            {
                row["TREND"] = a * double.Parse(row[xFieldName].ToString());
            }

            result.Add("slope", a);                // 추세선 기울기
            result.Add("Coefficient", R);          // 결정계수 R^2
            result.Add("trendData", trendData);    // 추세선 데이터


            return result;
        }

    }
}
