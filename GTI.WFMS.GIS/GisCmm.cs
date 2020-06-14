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





        /// FTR_CDE에서 레이어명(테이블명) 가져오기
        public static string GetLayerNm(string FTR_CDE)
        {
            string layerNm = "";


            switch (FTR_CDE)
            {
                case "SA001": layerNm = "WTL_PIPE_LM"; break;
                case "SA002": layerNm = "WTL_SPLY_LS"; break;
                case "SA003": layerNm = "WTL_STPI_PS"; break;
                case "SA100": layerNm = "WTL_MANH_PS"; break;
                case "SA110": layerNm = "WTL_HEAD_PS"; break;
                case "SA112": layerNm = "WTL_GAIN_PS"; break;
                case "SA113": layerNm = "WTL_PURI_AS"; break;
                case "SA114": layerNm = "WTL_SERV_PS"; break;
                case "SA117": layerNm = "WTL_FLOW_PS"; break;
                case "SA118": layerNm = "WTL_FIRE_PS^SA118"; break;
                case "SA119": layerNm = "WTL_FIRE_PS^SA119"; break;
                case "SA120": layerNm = "WTL_RSRV_PS"; break;
                case "SA121": layerNm = "WTL_PRGA_PS"; break;
                case "SA122": layerNm = "WTL_META_PS"; break;
                case "SA200": layerNm = "WTL_VALV_PS^SA200"; break;
                case "SA201": layerNm = "WTL_VALV_PS^SA201"; break;
                case "SA202": layerNm = "WTL_VALV_PS^SA202"; break;
                case "SA203": layerNm = "WTL_VALV_PS^SA203"; break;
                case "SA204": layerNm = "WTL_VALV_PS^SA204"; break;
                case "SA205": layerNm = "WTL_VALV_PS^SA205"; break;
                case "SA206": layerNm = "WTL_PRES_PS"; break;
                case "SA300": layerNm = "WTL_LEAK_PS"; break;

                case "SA901": layerNm = "WTL_PIPE_LX"; break;
                case "SA902": layerNm = "WTL_SPLY_LX"; break;
                case "SA903": layerNm = "WTL_PIPE_LY"; break;

                case "BZ001": layerNm = "WTL_BZ001"; break;
                case "BZ002": layerNm = "WTL_BZ002"; break;
                case "BZ003": layerNm = "WTL_BZ003"; break;


                default:
                    break;
            }

            return layerNm;
        }


    }
}
