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
    /// GIS 전역함수변수 - Runtime 버전
    /// </summary>
    public class CmmRun
    {
        #region ========= Runtime, Object 공통 ========= 

      

        #endregion

        // 레이어 심볼 Renderer
        public static UniqueValueRenderer uniqueValueRenderer;

        // 레이어 심볼 Renderer 구성 초기화 - shape버전 레이어구성시에만 사용함
        public static void InitUniqueValueRenderer()
        {
            CmmRun.uniqueValueRenderer = new UniqueValueRenderer();

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

            CmmRun.uniqueValueRenderer.FieldNames.Add("FTR_CDE");

            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA003Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA100Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA110Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA112Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA114Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA117Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA118Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA119Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA120Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA121Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA122Value);

            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA200Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA201Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA202Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA203Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA204Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA205Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA206Value);

            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA300Value);


            /*
             */
            // 2.Line 스타일링 - 속성값에따른 라인컬러 선별매핑
            //상수관로
            SimpleLineSymbol SA001Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2);
            UniqueValue SA001Value = new UniqueValue("SA001", "SA001", SA001Symbol, "SA001");
            //급수관로
            SimpleLineSymbol SA002Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
            UniqueValue SA002Value = new UniqueValue("SA002", "SA002", SA002Symbol, "SA002");

            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA001Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA002Value);






            // 3.Polygon 스타일링 - 속성값에따른 라인&내부컬러 선별매핑
            //정수장
            SimpleFillSymbol SA113Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue SA113Value = new UniqueValue("SA113", "SA113", SA113Symbol, "SA113");

            //대블록
            SimpleFillSymbol BZ001Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue BZ001Value = new UniqueValue("BZ001", "BZ001", BZ001Symbol, "BZ001");
            //중블록
            SimpleFillSymbol BZ002Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue BZ002Value = new UniqueValue("BZ002", "BZ002", BZ002Symbol, "BZ002");
            //소블록
            SimpleFillSymbol BZ003Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue BZ003Value = new UniqueValue("BZ003", "BZ003", BZ003Symbol, "BZ003");


            //행정구역
            SimpleLineSymbol EA035Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.Red, 2);
            UniqueValue EA035Value = new UniqueValue("EA035", "EA035", EA035Symbol, "EA035");


            CmmRun.uniqueValueRenderer.UniqueValues.Add(SA113Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(EA035Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(BZ001Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(BZ002Value);
            CmmRun.uniqueValueRenderer.UniqueValues.Add(BZ003Value);


        }
    }
}
