using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// MapViewModel 에서 레이어관련 항목 및 처리
    /// </summary>
    public class LayerModel
    {

        /* 
         * 레이어객체리스트
        public FeatureLayer BML_GADM_AS = new FeatureLayer();//울산광역행정구역
        public FeatureLayer WTL_FLOW_PS = new FeatureLayer();
        public FeatureLayer WTL_FIRE_PS = new FeatureLayer();
         */

        public Dictionary<string, FeatureLayer> layers = new Dictionary<string, FeatureLayer>()
        {
            {"WTL_FLOW_PS",  new FeatureLayer()},
            {"WTL_FIRE_PS",  new FeatureLayer()},
            {"WTL_GAIN_PS",  new FeatureLayer()},
            {"WTL_HEAD_PS",  new FeatureLayer()},
            {"WTL_LEAK_PS",  new FeatureLayer()},
            {"WTL_MANH_PS",  new FeatureLayer()},
            {"WTL_META_PS",  new FeatureLayer()},
            {"WTL_PRES_PS",  new FeatureLayer()},
            {"WTL_PRGA_PS",  new FeatureLayer()},
            {"WTL_RSRV_PS",  new FeatureLayer()},
            {"WTL_SERV_PS",  new FeatureLayer()},
            {"WTL_STPI_PS",  new FeatureLayer()},
            {"WTL_VALV_PS",  new FeatureLayer()},

            {"BML_GADM_AS",  new FeatureLayer()},
            {"WTL_PURI_AS",  new FeatureLayer()},
            
            {"WTL_PIPE_LM",  new FeatureLayer()},
            {"WTL_SPLY_LS",  new FeatureLayer()},
        };


        // 레이어 심볼 Renderer
        public UniqueValueRenderer uniqueValueRenderer = new UniqueValueRenderer();


















        /// <summary>
        /// Shape 레이어 보이기/끄기
        /// </summary>
        /// <param name="_map"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public async void ShowShapeLayer(Map _map, string layerNm, bool chk)
        {
            try
            {
                // 0.해당레이어 가져오기
                //Type memberType = this.GetType();
                FeatureLayer layer = layers[layerNm];


                //레이어 ON
                if (chk)
                {
                    if (_map.OperationalLayers.Contains(layer))
                    {
                        //on상태 아무것도 안함
                    }
                    else
                    {
                        if (layer != null && layer.LoadStatus == LoadStatus.Loaded) //레이어객체 있으면 단순추가
                        {
                            _map.OperationalLayers.Add(layer);
                        }
                        else //레이어객체 없으면 Shape 로딩
                        {
                            string shapefilePath = Path.Combine("D:\\DEVGTI\\layer", layerNm + ".shp");
                            try
                            {
                                ShapefileFeatureTable layerTable = await ShapefileFeatureTable.OpenAsync(shapefilePath);

                                layer = new FeatureLayer(layerTable); /////// 신규레이어 생성 /////// 
                                layers[layerNm] = layer; /////// 딕셔너리에 자동으로 저장되지는 않을것임 /////// 

                                _map.OperationalLayers.Add(layer);
                                layer.Renderer = uniqueValueRenderer.Clone(); //렌더러는 레이어 각각 할당해야하므로 렌더러복사하여 할당

                            }
                            catch (Exception e)
                            {
                                //throw e;
                            }
                        }
                    }
                }
                //레이어 OFF
                else
                {
                    if (_map.OperationalLayers.Contains(layer))
                    {
                        _map.OperationalLayers.Remove(layer);
                    }
                    else
                    {
                        //off상태 아무것도 안함
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }



        // 레이어 심볼 Renderer 구성 초기화
        public void InitUniqueValueRenderer()
        {
            // 1.Point 마커 스타일링 - 속성값에따른 이미지 선별매핑
            //상수맨홀
            var SA100Uri = new Uri("file:///D:/DEVGTI/style_img/SA100.gif");
            PictureMarkerSymbol SA100Symbol = new PictureMarkerSymbol(SA100Uri);
            UniqueValue SA100Value = new UniqueValue("SA100", "SA100", SA100Symbol, "SA100"); //string description, string label, Symbol symbol, object value
            //유량계
            var SA117Uri = new Uri("file:///D:/DEVGTI/style_img/SA117.gif");
            PictureMarkerSymbol SA117Symbol = new PictureMarkerSymbol(SA117Uri);
            UniqueValue SA117Value = new UniqueValue("SA117", "SA117", SA117Symbol, "SA117"); //string description, string label, Symbol symbol, object value
            //급수탑
            var SA118Uri = new Uri("file:///D:/DEVGTI/style_img/SA118.gif");
            PictureMarkerSymbol SA118Symbol = new PictureMarkerSymbol(SA118Uri);
            UniqueValue SA118Value = new UniqueValue("SA118", "SA118", SA118Symbol, "SA118"); //string description, string label, Symbol symbol, object value
            //소화전
            var SA119Uri = new Uri("file:///D:/DEVGTI/style_img/SA119.gif");
            PictureMarkerSymbol SA119Symbol = new PictureMarkerSymbol(SA119Uri);
            UniqueValue SA119Value = new UniqueValue("SA119", "SA119", SA119Symbol, "SA119");
            //수압계
            var SA121Uri = new Uri("file:///D:/DEVGTI/style_img/SA121.gif");
            PictureMarkerSymbol SA121Symbol = new PictureMarkerSymbol(SA121Uri);
            UniqueValue SA121Value = new UniqueValue("SA121", "SA121", SA121Symbol, "SA121"); //string description, string label, Symbol symbol, object value
            //급수전계량기
            var SA122Uri = new Uri("file:///D:/DEVGTI/style_img/SA122.gif");
            PictureMarkerSymbol SA122Symbol = new PictureMarkerSymbol(SA122Uri);
            UniqueValue SA122Value = new UniqueValue("SA122", "SA122", SA122Symbol, "SA122"); //string description, string label, Symbol symbol, object value
            //제수변
            var SA200Uri = new Uri("file:///D:/DEVGTI/style_img/SA200.gif");
            PictureMarkerSymbol SA200Symbol = new PictureMarkerSymbol(SA200Uri);
            UniqueValue SA200Value = new UniqueValue("SA200", "SA200", SA200Symbol, "SA200"); //string description, string label, Symbol symbol, object value
            //누수지점
            var SA300Uri = new Uri("file:///D:/DEVGTI/style_img/SA300.gif");
            PictureMarkerSymbol SA300Symbol = new PictureMarkerSymbol(SA300Uri);
            UniqueValue SA300Value = new UniqueValue("SA300", "SA300", SA300Symbol, "SA300"); //string description, string label, Symbol symbol, object value
            
            uniqueValueRenderer.FieldNames.Add("FTR_CDE");
            uniqueValueRenderer.UniqueValues.Add(SA100Value);
            uniqueValueRenderer.UniqueValues.Add(SA117Value);
            uniqueValueRenderer.UniqueValues.Add(SA118Value);
            uniqueValueRenderer.UniqueValues.Add(SA119Value);
            uniqueValueRenderer.UniqueValues.Add(SA121Value);
            uniqueValueRenderer.UniqueValues.Add(SA122Value);
            uniqueValueRenderer.UniqueValues.Add(SA200Value);
            uniqueValueRenderer.UniqueValues.Add(SA300Value);


            /*
             */
            // 2.Line 스타일링 - 속성값에따른 라인컬러 선별매핑
            //상수관로
            SimpleLineSymbol SA001Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 1);
            UniqueValue SA001Value = new UniqueValue("SA001", "SA001", SA001Symbol, "SA001"); 

            uniqueValueRenderer.UniqueValues.Add(SA001Value);






            // 3.Polygon 스타일링 - 속성값에따른 라인&내부컬러 선별매핑
            //정수장
            SimpleFillSymbol SA113Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue SA113Value = new UniqueValue("SA113", "SA113", SA113Symbol, "SA113");


            //울산행정구역
            SimpleLineSymbol EA035Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 1);
            UniqueValue EA035Value = new UniqueValue("EA035", "EA035", EA035Symbol, "EA035");


            uniqueValueRenderer.UniqueValues.Add(SA113Value);
            uniqueValueRenderer.UniqueValues.Add(EA035Value);


        }


    }

}
