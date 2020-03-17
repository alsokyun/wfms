using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using GTI.WFMS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



        // 레이어 심볼 Renderer
        public static UniqueValueRenderer uniqueValueRenderer;






        // 레이어 심볼 Renderer 구성 초기화 - shape버전 레이어구성시에만 사용함
        public static void InitUniqueValueRenderer()
        {
            GisCmm.uniqueValueRenderer = new UniqueValueRenderer();

            // 1.Point 마커 스타일링 - 속성값에따른 이미지 선별매핑
            //스탠드파이프
            var SA003Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA003"), UriKind.Relative);
            PictureMarkerSymbol SA003Symbol = new PictureMarkerSymbol(SA003Uri);
            UniqueValue SA003Value = new UniqueValue("SA003", "SA003", SA003Symbol, "SA003");
            //상수맨홀
            var SA100Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA100"), UriKind.Relative);
            PictureMarkerSymbol SA100Symbol = new PictureMarkerSymbol(SA100Uri);
            UniqueValue SA100Value = new UniqueValue("SA100", "SA100", SA100Symbol, "SA100"); //string description, string label, Symbol symbol, object value
            //취수장
            var SA112Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA112"), UriKind.Relative);
            PictureMarkerSymbol SA112Symbol = new PictureMarkerSymbol(SA112Uri);
            UniqueValue SA112Value = new UniqueValue("SA112", "SA112", SA112Symbol, "SA112"); //string description, string label, Symbol symbol, object value
            //배수지
            var SA114Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA114"), UriKind.Relative);
            PictureMarkerSymbol SA114Symbol = new PictureMarkerSymbol(SA114Uri);
            UniqueValue SA114Value = new UniqueValue("SA114", "SA114", SA114Symbol, "SA114"); //string description, string label, Symbol symbol, object value
            //수원지
            var SA110Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA110"), UriKind.Relative);
            PictureMarkerSymbol SA110Symbol = new PictureMarkerSymbol(SA110Uri);
            UniqueValue SA110Value = new UniqueValue("SA110", "SA110", SA110Symbol, "SA110"); //string description, string label, Symbol symbol, object value

            //유량계
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.UriSource = new Uri(BizUtil.GetDataFolder("style_img", "SA117"), UriKind.Relative);
            //bi.EndInit();
            //byte[] data;
            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bi));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    encoder.Save(ms);
            //    data = ms.ToArray();
            //}
            //FileStream fs = new FileStream(BizUtil.GetDataFolder("style_img", "SA117"), FileMode.Open);
            //var SA117Uri = await RuntimeImage.FromStreamAsync(fs);


            //RuntimeImage SA117Uri;
            //using (FileStream fs = new FileStream(BizUtil.GetDataFolder("style_img", "SA117"), FileMode.Open, FileAccess.Read))
            //{
            //    SA117Uri = await RuntimeImage.FromStreamAsync(fs);
            //    //fs.Flush();
            //    //fs.Dispose();
            //    //fs.Close();
            //}
            var SA117Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA117"), UriKind.Relative);
            PictureMarkerSymbol SA117Symbol = new PictureMarkerSymbol(SA117Uri); ;
            UniqueValue SA117Value = new UniqueValue("SA117", "SA117", SA117Symbol, "SA117"); //string description, string label, Symbol symbol, object value



            //급수탑
            var SA118Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA118"), UriKind.Relative);
            PictureMarkerSymbol SA118Symbol = new PictureMarkerSymbol(SA118Uri);
            UniqueValue SA118Value = new UniqueValue("SA118", "SA118", SA118Symbol, "SA118"); //string description, string label, Symbol symbol, object value
            //소화전
            var SA119Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA119"), UriKind.Relative);
            PictureMarkerSymbol SA119Symbol = new PictureMarkerSymbol(SA119Uri);
            UniqueValue SA119Value = new UniqueValue("SA119", "SA119", SA119Symbol, "SA119");
            //저수조
            var SA120Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA120"), UriKind.Relative);
            PictureMarkerSymbol SA120Symbol = new PictureMarkerSymbol(SA120Uri);
            UniqueValue SA120Value = new UniqueValue("SA120", "SA120", SA120Symbol, "SA120");
            //수압계
            var SA121Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA121"), UriKind.Relative);
            PictureMarkerSymbol SA121Symbol = new PictureMarkerSymbol(SA121Uri);
            UniqueValue SA121Value = new UniqueValue("SA121", "SA121", SA121Symbol, "SA121"); //string description, string label, Symbol symbol, object value
            //급수전계량기
            var SA122Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA122"), UriKind.Relative);
            PictureMarkerSymbol SA122Symbol = new PictureMarkerSymbol(SA122Uri);
            UniqueValue SA122Value = new UniqueValue("SA122", "SA122", SA122Symbol, "SA122"); //string description, string label, Symbol symbol, object value

            /* 변류시설 그룹 */
            //제수변
            var SA200Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA200"), UriKind.Relative);
            PictureMarkerSymbol SA200Symbol = new PictureMarkerSymbol(SA200Uri);
            UniqueValue SA200Value = new UniqueValue("SA200", "SA200", SA200Symbol, "SA200"); //string description, string label, Symbol symbol, object value
            //역지변
            var SA201Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA201"), UriKind.Relative);
            PictureMarkerSymbol SA201Symbol = new PictureMarkerSymbol(SA201Uri);
            UniqueValue SA201Value = new UniqueValue("SA201", "SA201", SA201Symbol, "SA201"); //string description, string label, Symbol symbol, object value
            //이토변
            var SA202Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA202"), UriKind.Relative);
            PictureMarkerSymbol SA202Symbol = new PictureMarkerSymbol(SA202Uri);
            UniqueValue SA202Value = new UniqueValue("SA202", "SA202", SA202Symbol, "SA202"); //string description, string label, Symbol symbol, object value
            //배기변
            var SA203Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA203"), UriKind.Relative);
            PictureMarkerSymbol SA203Symbol = new PictureMarkerSymbol(SA203Uri);
            UniqueValue SA203Value = new UniqueValue("SA203", "SA203", SA203Symbol, "SA203"); //string description, string label, Symbol symbol, object value
            //감압변
            var SA204Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA204"), UriKind.Relative);
            PictureMarkerSymbol SA204Symbol = new PictureMarkerSymbol(SA204Uri);
            UniqueValue SA204Value = new UniqueValue("SA204", "SA204", SA204Symbol, "SA204"); //string description, string label, Symbol symbol, object value
            //안전변
            var SA205Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA205"), UriKind.Relative);
            PictureMarkerSymbol SA205Symbol = new PictureMarkerSymbol(SA205Uri);
            UniqueValue SA205Value = new UniqueValue("SA205", "SA205", SA205Symbol, "SA205"); //string description, string label, Symbol symbol, object value
            //소화전제수변
            var SA206Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA206"), UriKind.Relative);
            PictureMarkerSymbol SA206Symbol = new PictureMarkerSymbol(SA206Uri);
            UniqueValue SA206Value = new UniqueValue("SA206", "SA206", SA206Symbol, "SA206"); //string description, string label, Symbol symbol, object value

            //누수지점
            var SA300Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA300"), UriKind.Relative);
            PictureMarkerSymbol SA300Symbol = new PictureMarkerSymbol(SA300Uri);
            UniqueValue SA300Value = new UniqueValue("SA300", "SA300", SA300Symbol, "SA300"); //string description, string label, Symbol symbol, object value

            GisCmm.uniqueValueRenderer.FieldNames.Add("FTR_CDE");

            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA003Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA100Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA110Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA112Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA114Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA117Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA118Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA119Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA120Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA121Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA122Value);

            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA200Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA201Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA202Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA203Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA204Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA205Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA206Value);

            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA300Value);


            /*
             */
            // 2.Line 스타일링 - 속성값에따른 라인컬러 선별매핑
            //상수관로
            SimpleLineSymbol SA001Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2);
            UniqueValue SA001Value = new UniqueValue("SA001", "SA001", SA001Symbol, "SA001");
            //급수관로
            SimpleLineSymbol SA002Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
            UniqueValue SA002Value = new UniqueValue("SA002", "SA002", SA002Symbol, "SA002");

            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA001Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA002Value);






            // 3.Polygon 스타일링 - 속성값에따른 라인&내부컬러 선별매핑
            //정수장
            SimpleFillSymbol SA113Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue SA113Value = new UniqueValue("SA113", "SA113", SA113Symbol, "SA113");


            //울산행정구역
            SimpleLineSymbol EA035Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.Red, 2);
            UniqueValue EA035Value = new UniqueValue("EA035", "EA035", EA035Symbol, "EA035");


            GisCmm.uniqueValueRenderer.UniqueValues.Add(SA113Value);
            GisCmm.uniqueValueRenderer.UniqueValues.Add(EA035Value);


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

                default:
                    break;
            }

            return layerNm;
        }


    }
}
