using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using GTI.WFMS.Models.Common;
using System;
using System.Collections;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// GIS 전역변수
    /// </summary>
    public class GisCmm
    {

        // Coordinates for Ulsan
        public static MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        //private double _ulsanScale = 150000;
        public static double _ulsanScale = 500000;
        public static double _ulsanScale2 = 150000;




        //선택된 피처의 공간위치정보
        public static string WKT_POINT = "";//포인트 WKT
        public static string WKT_LINE = "";//라인 WKT
        public static string WKT_POLYGON = "";//폴리곤 WKT


        //포인트 위치 DB저장
        public static void SavePoint(string FTR_CDE, string FTR_IDN, string TABLE_NM)
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId","updatePoint");
            param.Add("TABLE_NM", TABLE_NM);
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);
            param.Add("WKT_POINT", WKT_POINT);
            BizUtil.Update(param);
        }
        //포인트 라인 DB저장
        public static void SavePolyline(string FTR_CDE, string FTR_IDN, string TABLE_NM)
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "updatePolyline");
            param.Add("TABLE_NM", TABLE_NM);
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);
            param.Add("WKT_LINE ", WKT_LINE);
            BizUtil.Update(param);
        }
        //포인트 폴리곤 DB저장
        public static void SavePolygon(string FTR_CDE, string FTR_IDN, string TABLE_NM)
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "updatePolygon");
            param.Add("TABLE_NM", TABLE_NM);
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);
            param.Add("WKT_POLYGON", WKT_POLYGON);
            BizUtil.Update(param);
        }








    }
}
