using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using GTI.WFMS.Models.Common;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// GIS 전역변수 - ArcObject 버전
    /// </summary>
    public class GisCm
    {



        // 레이어 심볼 Renderer - ArcObject 버전
        public static ESRI.ArcGIS.Carto.UniqueValueRenderer uniqueValueRenderer;

        // 레이어 심볼 Renderer 구성 초기화 - ArcObject
        public static void InitUniqueValueRenderer()
        {
            uniqueValueRenderer = new UniqueValueRenderer();
            uniqueValueRenderer.FieldCount = 1;
            uniqueValueRenderer.set_Field(0, "FTR_CDE");


            /* PictureMarkerSymbol 정의 */
            IRgbColor rgbColorCls = new RgbColor(); //배경칼라
            rgbColorCls.Red = 255;
            rgbColorCls.Green = 255;
            rgbColorCls.Blue = 255;


            // Create the gif marker and assign properties.
            //스탠파이프
            IPictureMarkerSymbol pictureSymbolSA003 = new PictureMarkerSymbol();
            pictureSymbolSA003.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA003"));
            pictureSymbolSA003.Angle = 0;
            pictureSymbolSA003.BitmapTransparencyColor = rgbColorCls;
            //pictureMarkerSymbol.Size = 16;
            pictureSymbolSA003.XOffset = 0;
            pictureSymbolSA003.YOffset = 0;
            //상수맨홀
            IPictureMarkerSymbol pictureSymbolSA100 = new PictureMarkerSymbol();
            pictureSymbolSA100.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA100"));
            pictureSymbolSA100.Angle = 0;
            pictureSymbolSA100.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA100.XOffset = 0;
            pictureSymbolSA100.YOffset = 0;
            //수원지
            IPictureMarkerSymbol pictureSymbolSA110 = new PictureMarkerSymbol();
            pictureSymbolSA110.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA110"));
            pictureSymbolSA110.Angle = 0;
            pictureSymbolSA110.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA110.XOffset = 0;
            pictureSymbolSA110.YOffset = 0;
            //취수장
            IPictureMarkerSymbol pictureSymbolSA112 = new PictureMarkerSymbol();
            pictureSymbolSA112.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA112"));
            pictureSymbolSA112.Angle = 0;
            pictureSymbolSA112.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA112.XOffset = 0;
            pictureSymbolSA112.YOffset = 0;
            //배수지
            IPictureMarkerSymbol pictureSymbolSA114 = new PictureMarkerSymbol();
            pictureSymbolSA114.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA114"));
            pictureSymbolSA114.Angle = 0;
            pictureSymbolSA114.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA114.XOffset = 0;
            pictureSymbolSA114.YOffset = 0;
            //유량계
            IPictureMarkerSymbol pictureSymbolSA117 = new PictureMarkerSymbol();
            pictureSymbolSA117.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA117"));
            pictureSymbolSA117.Angle = 0;
            pictureSymbolSA117.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA117.XOffset = 0;
            pictureSymbolSA117.YOffset = 0;
            //급수탑
            IPictureMarkerSymbol pictureSymbolSA118 = new PictureMarkerSymbol();
            pictureSymbolSA118.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA118"));
            pictureSymbolSA118.Angle = 0;
            pictureSymbolSA118.BitmapTransparencyColor = rgbColorCls;
            //pictureMarkerSymbol.Size = 16;
            pictureSymbolSA118.XOffset = 0;
            pictureSymbolSA118.YOffset = 0;
            //소화전
            IPictureMarkerSymbol pictureSymbolSA119 = new PictureMarkerSymbol();
            pictureSymbolSA119.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA119"));
            pictureSymbolSA119.Angle = 0;
            pictureSymbolSA119.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA119.XOffset = 0;
            pictureSymbolSA119.YOffset = 0;
            //저수조
            IPictureMarkerSymbol pictureSymbolSA120 = new PictureMarkerSymbol();
            pictureSymbolSA120.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA120"));
            pictureSymbolSA120.Angle = 0;
            pictureSymbolSA120.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA120.XOffset = 0;
            pictureSymbolSA120.YOffset = 0;
            //수압계
            IPictureMarkerSymbol pictureSymbolSA121 = new PictureMarkerSymbol();
            pictureSymbolSA121.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA121"));
            pictureSymbolSA121.Angle = 0;
            pictureSymbolSA121.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA121.XOffset = 0;
            pictureSymbolSA121.YOffset = 0;
            //급수전계량기
            IPictureMarkerSymbol pictureSymbolSA122 = new PictureMarkerSymbol();
            pictureSymbolSA122.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA122"));
            pictureSymbolSA122.Angle = 0;
            pictureSymbolSA122.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA122.XOffset = 0;
            pictureSymbolSA122.YOffset = 0;
            //제수변
            IPictureMarkerSymbol pictureSymbolSA200 = new PictureMarkerSymbol();
            pictureSymbolSA200.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA200"));
            pictureSymbolSA200.Angle = 0;
            pictureSymbolSA200.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA200.XOffset = 0;
            pictureSymbolSA200.YOffset = 0;
            //역지변
            IPictureMarkerSymbol pictureSymbolSA201 = new PictureMarkerSymbol();
            pictureSymbolSA201.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA201"));
            pictureSymbolSA201.Angle = 0;
            pictureSymbolSA201.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA201.XOffset = 0;
            pictureSymbolSA201.YOffset = 0;
            //이토변
            IPictureMarkerSymbol pictureSymbolSA202 = new PictureMarkerSymbol();
            pictureSymbolSA202.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA202"));
            pictureSymbolSA202.Angle = 0;
            pictureSymbolSA202.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA202.XOffset = 0;
            pictureSymbolSA202.YOffset = 0;
            //배기변
            IPictureMarkerSymbol pictureSymbolSA203 = new PictureMarkerSymbol();
            pictureSymbolSA203.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA203"));
            pictureSymbolSA203.Angle = 0;
            pictureSymbolSA203.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA203.XOffset = 0;
            pictureSymbolSA203.YOffset = 0;
            //감압변
            IPictureMarkerSymbol pictureSymbolSA204 = new PictureMarkerSymbol();
            pictureSymbolSA204.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA204"));
            pictureSymbolSA204.Angle = 0;
            pictureSymbolSA204.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA204.XOffset = 0;
            pictureSymbolSA204.YOffset = 0;
            //안전변
            IPictureMarkerSymbol pictureSymbolSA205 = new PictureMarkerSymbol();
            pictureSymbolSA205.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureGIF, BizUtil.GetDataFolder("style_img", "SA205"));
            pictureSymbolSA205.Angle = 0;
            pictureSymbolSA205.BitmapTransparencyColor = rgbColorCls;
            pictureSymbolSA205.XOffset = 0;
            pictureSymbolSA205.YOffset = 0;



            /* 라인심볼정의 */
            //상수관로
            ISimpleLineSymbol lineSymbolSA001 = new SimpleLineSymbol();
            lineSymbolSA001.Color = new RgbColor() { Red = 0, Green = 102, Blue = 255 };
            //급수관로
            ISimpleLineSymbol lineSymbolSA002 = new SimpleLineSymbol();
            lineSymbolSA002.Color = new RgbColor() { Red = 0, Green = 204, Blue = 153 };
            //울산행정구역
            //ISimpleLineSymbol lineSymbolEA305 = new SimpleLineSymbol();
            //lineSymbolEA305.Color = new RgbColor() { Red = 255, Green = 0, Blue = 0 };


            /* Fill심볼정의 */
            //라인심볼
            ISimpleLineSymbol pSLS = new SimpleLineSymbol();
            pSLS.Color = new RgbColor() { Red = 0, Green = 51, Blue = 204 };
            //정수장
            ISimpleFillSymbol fillSymbolSA113 = new SimpleFillSymbol();
            fillSymbolSA113.Color = new RgbColor() { Red = 51, Green = 153, Blue = 255 };
            fillSymbolSA113.Outline = pSLS;  //외각선은 라인심볼로 지정

            //라인심볼
            pSLS = new SimpleLineSymbol();
            pSLS.Color = new RgbColor() { Red = 255, Green = 0, Blue = 0 };
            //울산행정구역
            ISimpleFillSymbol fillSymbolEA305 = new SimpleFillSymbol();
            fillSymbolEA305.Color = new RgbColor() { Red = 51, Green = 153, Blue = 255 };
            fillSymbolEA305.Outline = pSLS;  //외각선은 라인심볼로 지정



            /* uniqueValue 에따른 심볼적용 */
            uniqueValueRenderer.AddValue("SA003", "Name", pictureSymbolSA003 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA003", pictureSymbolSA003 as ISymbol);
            uniqueValueRenderer.AddValue("SA100", "Name", pictureSymbolSA100 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA100", pictureSymbolSA100 as ISymbol);
            uniqueValueRenderer.AddValue("SA110", "Name", pictureSymbolSA110 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA110", pictureSymbolSA110 as ISymbol);
            uniqueValueRenderer.AddValue("SA112", "Name", pictureSymbolSA112 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA112", pictureSymbolSA112 as ISymbol);
            uniqueValueRenderer.AddValue("SA114", "Name", pictureSymbolSA114 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA114", pictureSymbolSA114 as ISymbol);
            uniqueValueRenderer.AddValue("SA117", "Name", pictureSymbolSA117 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA117", pictureSymbolSA117 as ISymbol);
            uniqueValueRenderer.AddValue("SA118", "Name", pictureSymbolSA118 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA118", pictureSymbolSA118 as ISymbol);
            uniqueValueRenderer.AddValue("SA119", "Name", pictureSymbolSA119 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA119", pictureSymbolSA119 as ISymbol);
            uniqueValueRenderer.AddValue("SA120", "Name", pictureSymbolSA120 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA120", pictureSymbolSA120 as ISymbol);
            uniqueValueRenderer.AddValue("SA121", "Name", pictureSymbolSA121 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA121", pictureSymbolSA121 as ISymbol);
            uniqueValueRenderer.AddValue("SA122", "Name", pictureSymbolSA122 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA122", pictureSymbolSA122 as ISymbol);
            uniqueValueRenderer.AddValue("SA200", "Name", pictureSymbolSA200 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA200", pictureSymbolSA200 as ISymbol);
            uniqueValueRenderer.AddValue("SA201", "Name", pictureSymbolSA201 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA201", pictureSymbolSA201 as ISymbol);
            uniqueValueRenderer.AddValue("SA202", "Name", pictureSymbolSA202 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA202", pictureSymbolSA202 as ISymbol);
            uniqueValueRenderer.AddValue("SA203", "Name", pictureSymbolSA203 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA203", pictureSymbolSA203 as ISymbol);
            uniqueValueRenderer.AddValue("SA204", "Name", pictureSymbolSA204 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA204", pictureSymbolSA204 as ISymbol);
            uniqueValueRenderer.AddValue("SA205", "Name", pictureSymbolSA205 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA205", pictureSymbolSA205 as ISymbol);

            uniqueValueRenderer.AddValue("SA001", "Name", lineSymbolSA001 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA001", lineSymbolSA001 as ISymbol);
            uniqueValueRenderer.AddValue("SA002", "Name", lineSymbolSA002 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA002", lineSymbolSA002 as ISymbol);

            uniqueValueRenderer.AddValue("EA305", "Name", fillSymbolEA305 as ISymbol);
            uniqueValueRenderer.set_Symbol("EA305", fillSymbolEA305 as ISymbol);

            uniqueValueRenderer.AddValue("SA113", "Name", fillSymbolSA113 as ISymbol);
            uniqueValueRenderer.set_Symbol("SA113", fillSymbolSA113 as ISymbol);

        }






        // 레이어 한글명
        public static string getLayerKorNm(string layerNm)
        {
            string korNm = "";
            switch (layerNm)
            {
                case "WTL_STPI_PS":
                    korNm = "스탠파이프";
                    break;
                case "WTL_SERV_PS":
                    korNm = "배수지";
                    break;
                case "WTL_RSRV_PS":
                    korNm = "저수조";
                    break;
                case "WTL_PRES_PS":
                    korNm = "가압장";
                    break;
                case "WTL_META_PS":
                    korNm = "급수전계량기";
                    break;
                case "WTL_MANH_PS":
                    korNm = "상수맨홀";
                    break;
                case "WTL_HEAD_PS":
                    korNm = "수원지";
                    break;
                case "WTL_GAIN_PS":
                    korNm = "취수장";
                    break;
                case "WTL_PRGA_PS":
                    korNm = "수압계";
                    break;
                case "WTL_FLOW_PS":
                    korNm = "유량계";
                    break;
                case "WTL_SPLY_LS":
                    korNm = "급수관로";
                    break;
                case "WTL_PIPE_LM":
                    korNm = "상수관로";
                    break;
                case "WTL_PURI_AS":
                    korNm = "정수장";
                    break;
                case "WTL_FIRE_PS^SA118":
                    korNm = "급수탑";
                    break;
                case "WTL_FIRE_PS^SA119":
                    korNm = "소화전";
                    break;
                case "WTL_VALV_PS^SA200":
                    korNm = "제수변";
                    break;
                case "WTL_VALV_PS^SA201":
                    korNm = "역지변";
                    break;
                case "WTL_VALV_PS^SA202":
                    korNm = "이토변";
                    break;
                case "WTL_VALV_PS^SA203":
                    korNm = "배기변";
                    break;
                case "WTL_VALV_PS^SA204":
                    korNm = "감압변";
                    break;
                case "WTL_VALV_PS^SA205":
                    korNm = "안전변";
                    break;

                case "BML_GADM_AS":
                    korNm = "울산행정구역";
                    break;
                default:
                    break;
            }
            return korNm;
        }





    }
}
