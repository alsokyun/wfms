using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using GTI.WFMS.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// MapViewModel 에서 레이어관련 항목 및 처리
    /// </summary>
    public class LayerModel
    {
        //생성자
        public LayerModel()
        {
            // ArcGIS LocalServer start..
            //Initialize_LocalServer();
        }


        public Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };

        public MapView mapView = new MapView(); //뷰의 MapView


        /* 
         * 레이어객체리스트
        public FeatureLayer BML_GADM_AS = new FeatureLayer();//울산광역행정구역
        public FeatureLayer WTL_FLOW_PS = new FeatureLayer();
        public FeatureLayer WTL_FIRE_PS = new FeatureLayer();
         */

        public Dictionary<string, FeatureLayer> layers = new Dictionary<string, FeatureLayer>()
        {
            {"WTL_FLOW_PS",  new FeatureLayer()},
            {"WTL_FIRE_PS,FTR_CDE='SA118'",  new FeatureLayer()},
            {"WTL_FIRE_PS,FTR_CDE='SA119'",  new FeatureLayer()},
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
            {"WTL_VALV_PS,FTR_CDE='SA200'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA201'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA202'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA203'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA204'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA205'",  new FeatureLayer()},
            {"WTL_VALV_PS,FTR_CDE='SA206'",  new FeatureLayer()},

            {"BML_GADM_AS",  new FeatureLayer()},
            {"WTL_PURI_AS",  new FeatureLayer()},

            {"WTL_PIPE_LM",  new FeatureLayer()},
            {"WTL_SPLY_LS",  new FeatureLayer()},
        };


        // 레이어 심볼 Renderer
        public UniqueValueRenderer uniqueValueRenderer = new UniqueValueRenderer();









        #region =========== shape 레이어 구성부분 ==============

        /// <summary>
        /// Shape 레이어 보이기/끄기 - Shape버전
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public async void ShowShapeLayer(MapView _mapView, string _layerNm, bool chk)
        {
            try
            {
                // 0.해당레이어 가져오기
                string filterExp = "";
                string layerNm = "";

                try
                {
                    string[] ary = _layerNm.Split(',');
                    layerNm = ary[0]; //레이어테이블명
                    filterExp = ary[1]; //필터표현식
                }
                catch (Exception e) { }

                FeatureLayer layer = layers[_layerNm];
                //Type memberType = this.GetType();



                // 1.레이어 ON
                if (chk)
                {
                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        //on상태 아무것도 안함
                    }
                    else
                    {
                        if (layer != null && layer.LoadStatus == LoadStatus.Loaded) //레이어객체 있으면 단순추가
                        {
                            _mapView.Map.OperationalLayers.Add(layer);
                        }
                        else //레이어객체 없으면 Shape 로딩
                        {
                            string shapefilePath = Path.Combine(BizUtil.GetDataFolder("shape", layerNm + ".shp"));
                            try
                            {
                                ShapefileFeatureTable layerTable = await ShapefileFeatureTable.OpenAsync(shapefilePath);

                                layer = new FeatureLayer(layerTable); /////// 신규레이어 생성 /////// 
                                layers[_layerNm] = layer; /////// 딕셔너리에 자동으로 저장되지는 않을것임 /////// 

                                _mapView.Map.OperationalLayers.Add(layer);
                                layer.Renderer = uniqueValueRenderer.Clone(); //렌더러는 레이어 각각 할당해야하므로 렌더러복사하여 할당


                            }
                            catch (Exception e)
                            {
                                //throw e;
                            }
                        }
                    }


                    // 필터링 인수있으면 하위시설물으로 필터
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        layer.DefinitionExpression = filterExp;
                    }
                }
                // 2.레이어 OFF
                else
                {
                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        _mapView.Map.OperationalLayers.Remove(layer);
                    }
                    else
                    {
                        //off상태 아무것도 안함
                    }

                    // 필터링 인수있으면 하위시설물으로 필터 리셋
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        layer.DefinitionExpression = "";
                    }
                }




            }
            catch (Exception e)
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }






        // 레이어 심볼 Renderer 구성 초기화 - shape버전 레이어구성시에만 사용함
        public void InitUniqueValueRenderer()
        {
            // 1.Point 마커 스타일링 - 속성값에따른 이미지 선별매핑
            //스탠드파이프
            SimpleLineSymbol SA003Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 1);
            UniqueValue SA003Value = new UniqueValue("SA003", "SA003", SA003Symbol, "SA003");
            //상수맨홀
            var SA100Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA100.gif"), UriKind.Relative);
            PictureMarkerSymbol SA100Symbol = new PictureMarkerSymbol(SA100Uri);
            UniqueValue SA100Value = new UniqueValue("SA100", "SA100", SA100Symbol, "SA100"); //string description, string label, Symbol symbol, object value
            //취수장
            var SA112Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA112.gif"), UriKind.Relative);
            PictureMarkerSymbol SA112Symbol = new PictureMarkerSymbol(SA112Uri);
            UniqueValue SA112Value = new UniqueValue("SA112", "SA112", SA112Symbol, "SA112"); //string description, string label, Symbol symbol, object value
            //배수지
            var SA114Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA114.gif"), UriKind.Relative);
            PictureMarkerSymbol SA114Symbol = new PictureMarkerSymbol(SA114Uri);
            UniqueValue SA114Value = new UniqueValue("SA114", "SA114", SA114Symbol, "SA114"); //string description, string label, Symbol symbol, object value
            //수원지
            var SA110Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA110.gif"), UriKind.Relative);
            PictureMarkerSymbol SA110Symbol = new PictureMarkerSymbol(SA110Uri);
            UniqueValue SA110Value = new UniqueValue("SA110", "SA110", SA110Symbol, "SA110"); //string description, string label, Symbol symbol, object value
            //유량계
            var SA117Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA117.gif"), UriKind.Relative);
            PictureMarkerSymbol SA117Symbol = new PictureMarkerSymbol(SA117Uri);
            UniqueValue SA117Value = new UniqueValue("SA117", "SA117", SA117Symbol, "SA117"); //string description, string label, Symbol symbol, object value
            //급수탑
            var SA118Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA118.gif"), UriKind.Relative);
            PictureMarkerSymbol SA118Symbol = new PictureMarkerSymbol(SA118Uri);
            UniqueValue SA118Value = new UniqueValue("SA118", "SA118", SA118Symbol, "SA118"); //string description, string label, Symbol symbol, object value
            //소화전
            var SA119Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA119.gif"), UriKind.Relative);
            PictureMarkerSymbol SA119Symbol = new PictureMarkerSymbol(SA119Uri);
            UniqueValue SA119Value = new UniqueValue("SA119", "SA119", SA119Symbol, "SA119");
            //저수조
            var SA120Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA120.gif"), UriKind.Relative);
            PictureMarkerSymbol SA120Symbol = new PictureMarkerSymbol(SA120Uri);
            UniqueValue SA120Value = new UniqueValue("SA120", "SA120", SA120Symbol, "SA120");
            //수압계
            var SA121Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA121.gif"), UriKind.Relative);
            PictureMarkerSymbol SA121Symbol = new PictureMarkerSymbol(SA121Uri);
            UniqueValue SA121Value = new UniqueValue("SA121", "SA121", SA121Symbol, "SA121"); //string description, string label, Symbol symbol, object value
            //급수전계량기
            var SA122Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA122.gif"), UriKind.Relative);
            PictureMarkerSymbol SA122Symbol = new PictureMarkerSymbol(SA122Uri);
            UniqueValue SA122Value = new UniqueValue("SA122", "SA122", SA122Symbol, "SA122"); //string description, string label, Symbol symbol, object value

            /* 변류시설 그룹 */
            //제수변
            var SA200Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA200.gif"), UriKind.Relative);
            PictureMarkerSymbol SA200Symbol = new PictureMarkerSymbol(SA200Uri);
            UniqueValue SA200Value = new UniqueValue("SA200", "SA200", SA200Symbol, "SA200"); //string description, string label, Symbol symbol, object value
            //역지변
            var SA201Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA201.gif"), UriKind.Relative);
            PictureMarkerSymbol SA201Symbol = new PictureMarkerSymbol(SA201Uri);
            UniqueValue SA201Value = new UniqueValue("SA201", "SA201", SA201Symbol, "SA201"); //string description, string label, Symbol symbol, object value
            //이토변
            var SA202Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA202.gif"), UriKind.Relative);
            PictureMarkerSymbol SA202Symbol = new PictureMarkerSymbol(SA202Uri);
            UniqueValue SA202Value = new UniqueValue("SA202", "SA202", SA202Symbol, "SA202"); //string description, string label, Symbol symbol, object value
            //배기변
            var SA203Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA203.gif"), UriKind.Relative);
            PictureMarkerSymbol SA203Symbol = new PictureMarkerSymbol(SA203Uri);
            UniqueValue SA203Value = new UniqueValue("SA203", "SA203", SA203Symbol, "SA203"); //string description, string label, Symbol symbol, object value
            //감압변
            var SA204Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA204.gif"), UriKind.Relative);
            PictureMarkerSymbol SA204Symbol = new PictureMarkerSymbol(SA204Uri);
            UniqueValue SA204Value = new UniqueValue("SA204", "SA204", SA204Symbol, "SA204"); //string description, string label, Symbol symbol, object value
            //안전변
            var SA205Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA205.gif"), UriKind.Relative);
            PictureMarkerSymbol SA205Symbol = new PictureMarkerSymbol(SA205Uri);
            UniqueValue SA205Value = new UniqueValue("SA205", "SA205", SA205Symbol, "SA205"); //string description, string label, Symbol symbol, object value
            //소화전제수변
            var SA206Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA206.gif"), UriKind.Relative);
            PictureMarkerSymbol SA206Symbol = new PictureMarkerSymbol(SA206Uri);
            UniqueValue SA206Value = new UniqueValue("SA206", "SA206", SA206Symbol, "SA206"); //string description, string label, Symbol symbol, object value

            //누수지점
            var SA300Uri = new Uri(BizUtil.GetDataFolder("style_img", "SA300.gif"), UriKind.Relative);
            PictureMarkerSymbol SA300Symbol = new PictureMarkerSymbol(SA300Uri);
            UniqueValue SA300Value = new UniqueValue("SA300", "SA300", SA300Symbol, "SA300"); //string description, string label, Symbol symbol, object value

            uniqueValueRenderer.FieldNames.Add("FTR_CDE");

            uniqueValueRenderer.UniqueValues.Add(SA003Value);
            uniqueValueRenderer.UniqueValues.Add(SA100Value);
            uniqueValueRenderer.UniqueValues.Add(SA110Value);
            uniqueValueRenderer.UniqueValues.Add(SA112Value);
            uniqueValueRenderer.UniqueValues.Add(SA114Value);
            uniqueValueRenderer.UniqueValues.Add(SA117Value);
            uniqueValueRenderer.UniqueValues.Add(SA118Value);
            uniqueValueRenderer.UniqueValues.Add(SA119Value);
            uniqueValueRenderer.UniqueValues.Add(SA120Value);
            uniqueValueRenderer.UniqueValues.Add(SA121Value);
            uniqueValueRenderer.UniqueValues.Add(SA122Value);

            uniqueValueRenderer.UniqueValues.Add(SA200Value);
            uniqueValueRenderer.UniqueValues.Add(SA201Value);
            uniqueValueRenderer.UniqueValues.Add(SA202Value);
            uniqueValueRenderer.UniqueValues.Add(SA203Value);
            uniqueValueRenderer.UniqueValues.Add(SA204Value);
            uniqueValueRenderer.UniqueValues.Add(SA205Value);
            uniqueValueRenderer.UniqueValues.Add(SA206Value);

            uniqueValueRenderer.UniqueValues.Add(SA300Value);


            /*
             */
            // 2.Line 스타일링 - 속성값에따른 라인컬러 선별매핑
            //상수관로
            SimpleLineSymbol SA001Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Blue, 2);
            UniqueValue SA001Value = new UniqueValue("SA001", "SA001", SA001Symbol, "SA001");
            //급수관로
            SimpleLineSymbol SA002Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
            UniqueValue SA002Value = new UniqueValue("SA002", "SA002", SA002Symbol, "SA002");

            uniqueValueRenderer.UniqueValues.Add(SA001Value);
            uniqueValueRenderer.UniqueValues.Add(SA002Value);






            // 3.Polygon 스타일링 - 속성값에따른 라인&내부컬러 선별매핑
            //정수장
            SimpleFillSymbol SA113Symbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, System.Drawing.Color.Blue, new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.SkyBlue, 1));
            UniqueValue SA113Value = new UniqueValue("SA113", "SA113", SA113Symbol, "SA113");


            //울산행정구역
            SimpleLineSymbol EA035Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.Red, 2);
            UniqueValue EA035Value = new UniqueValue("EA035", "EA035", EA035Symbol, "EA035");


            uniqueValueRenderer.UniqueValues.Add(SA113Value);
            uniqueValueRenderer.UniqueValues.Add(EA035Value);


        }



        #endregion






        /// FTR_CDE에서 레이어명(테이블명) 가져오기
        public string GetLayerNm(string FTR_CDE)
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
                case "SA118": layerNm = "WTL_FIRE_PS,FTR_CDE='SA118'"; break;
                case "SA119": layerNm = "WTL_FIRE_PS,FTR_CDE='SA119'"; break;
                case "SA120": layerNm = "WTL_RSRV_PS"; break;
                case "SA121": layerNm = "WTL_PRGA_PS"; break;
                case "SA122": layerNm = "WTL_META_PS"; break;
                case "SA200": layerNm = "WTL_VALV_PS,FTR_CDE='SA200'"; break;
                case "SA201": layerNm = "WTL_VALV_PS,FTR_CDE='SA201'"; break;
                case "SA202": layerNm = "WTL_VALV_PS,FTR_CDE='SA202'"; break;
                case "SA203": layerNm = "WTL_VALV_PS,FTR_CDE='SA203'"; break;
                case "SA204": layerNm = "WTL_VALV_PS,FTR_CDE='SA204'"; break;
                case "SA205": layerNm = "WTL_VALV_PS,FTR_CDE='SA205'"; break;
                case "SA206": layerNm = "WTL_VALV_PS,FTR_CDE='SA206'"; break;
                case "SA300": layerNm = "WTL_LEAK_PS"; break;

                default:
                    break;
            }

            return layerNm;
        }











        /// LocalServer에서 해당레이어의 LayerI 가져오기
        /*
        public string GetLayerId(string layerNm)
        {
            string layerId = "1";
            switch (layerNm)
            {
                case "WTL_STPI_PS":
                    layerId = "1";
                    break;
                case "WTL_SERV_PS":
                    layerId = "2";
                    break;
                case "WTL_RSRV_PS":
                    layerId = "3";
                    break;
                case "WTL_PRES_PS":
                    layerId = "4";
                    break;
                case "WTL_META_PS":
                    layerId = "5";
                    break;
                case "WTL_MANH_PS":
                    layerId = "6";
                    break;
                case "WTL_LEAK_PS":
                    layerId = "7";
                    break;
                case "WTL_HEAD_PS":
                    layerId = "8";
                    break;
                case "WTL_GAIN_PS":
                    layerId = "9";
                    break;
                case "WTL_PRGA_PS":
                    layerId = "10";
                    break;
                case "WTL_FLOW_PS":
                    layerId = "11";
                    break;
                case "WTL_SPLY_LS":
                    layerId = "12";
                    break;
                case "WTL_PIPE_LM":
                    layerId = "13";
                    break;
                case "WTL_PURI_AS":
                    layerId = "14";
                    break;
                case "BML_GADM_AS":
                    layerId = "15";
                    break;
                case "WTL_FIRE_PS,FTR_CDE='SA118'":
                    layerId = "16";
                    break;
                case "WTL_FIRE_PS,FTR_CDE='SA119'":
                    layerId = "17";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA200'":
                    layerId = "18";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA201'":
                    layerId = "19";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA202'":
                    layerId = "20";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA203'":
                    layerId = "21";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA204'":
                    layerId = "22";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA205'":
                    layerId = "23";
                    break;
                case "WTL_VALV_PS,FTR_CDE='SA206'":
                    layerId = "24";
                    break;
                default:
                    break;
            }
            return layerId;
        }
         */




        /// <summary>
        /// 레이어 보이기/끄기 - LocalServer버전
        /// </summary>
        /// <param name="_map"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        /*
        public async void ShowLocalServerLayer(MapView _mapView, string _layerNm, bool chk)
        {
            try
            {
                // 0.해당레이어 가져오기
                string filterExp = "";
                string layerNm = "";

                try
                {
                    string[] ary = _layerNm.Split(',');
                    layerNm = ary[0]; //레이어테이블명
                    filterExp = ary[1]; //필터표현식
                }
                catch (Exception e) { }

                FeatureLayer layer = layers[layerNm];
                //Type memberType = this.GetType();






                // 1.레이어 ON
                if (chk)
                {


                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        //on상태 아무것도 안함
                    }
                    else
                    {
                        if (layer != null && layer.LoadStatus == LoadStatus.Loaded) //레이어객체 있으면 단순추가
                        {
                            // 필터링 인수있으면 하위시설물으로 필터
                            if (!FmsUtil.IsNull(filterExp))
                            {
                                layer.DefinitionExpression = filterExp;
                            }
                            _mapView.Map.OperationalLayers.Add(layer);
                        }
                        else //레이어객체 없으면 Shape 로딩
                        {

                            if (LocalServer.Instance.Status == LocalServerStatus.Started)
                            {
                                // Get the path to the first layer - the local feature service url + layer ID
                                string layerUrl = _localFeatureService.Url + "/" + GetLayerId(layerNm);

                                // Create the ServiceFeatureTable
                                ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));

                                // Create the Feature Layer from the table
                                FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);
                                layers[layerNm] = myFeatureLayer; //생성한레이어를 딕셔너리에 저장

                                // 필터링 인수있으면 하위시설물으로 필터
                                if (!FmsUtil.IsNull(filterExp))
                                {
                                    myFeatureLayer.DefinitionExpression = filterExp;
                                }


                                // Add the layer to the map
                                _mapView.Map.OperationalLayers.Add(myFeatureLayer);

                                try
                                {
                                    // Wait for the layer to load
                                    await myFeatureLayer.LoadAsync();

                                    // Set the viewpoint on the MapView to show the layer data
                                    await _mapView.SetViewpointGeometryAsync(myFeatureLayer.FullExtent, 50);


                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString(), "Error");
                                }
                            }

                        }
                    }


                }
                // 2.레이어 OFF
                else
                {
                    // 필터링 인수있으면 하위시설물으로 필터
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        layer.DefinitionExpression = filterExp;
                    }

                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        _mapView.Map.OperationalLayers.Remove(layer);
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
        */





        #region ============ LocalServer (start) 관련부분 ==============
        // Hold a reference to the local feature service; the ServiceFeatureTable will be loaded from this service
        //public LocalFeatureService _localFeatureService;

        //public async void Initialize_LocalServer()
        //{

        //    try
        //    {
        //        // LocalServer must not be running when setting the data path.
        //        if (LocalServer.Instance.Status == LocalServerStatus.Started)
        //        {
        //            await LocalServer.Instance.StopAsync();
        //        }

        //        // Set the local data path - must be done before starting. On most systems, this will be C:\EsriSamples\AppData.
        //        // This path should be kept short to avoid Windows path length limitations.
        //        string tempDataPathRoot = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Windows)).FullName;
        //        string tempDataPath = Path.Combine(tempDataPathRoot, "EsriSamples", "AppData");
        //        Directory.CreateDirectory(tempDataPath); // CreateDirectory won't overwrite if it already exists.
        //        LocalServer.Instance.AppDataPath = tempDataPath;

        //        // Start the local server instance
        //        await LocalServer.Instance.StartAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(String.Format("Please ensure that local server is installed prior to using the sample. See instructions in readme.md. Message: {0}", ex.Message), "Local Server failed to start");
        //        return;
        //    }

        //    // Load the sample data and get the path
        //    string myfeatureServicePath = GetFeatureLayerPath();

        //    // Create the feature service to serve the local data
        //    _localFeatureService = new LocalFeatureService(myfeatureServicePath);

        //    // Listen to feature service status changes
        //    _localFeatureService.StatusChanged += _localFeatureService_StatusChanged;

        //    // Start the feature service
        //    try
        //    {
        //        await _localFeatureService.StartAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "The feature service failed to load");
        //    }
        //}

        //private async void _localFeatureService_StatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //    // Load the map from the service once ready
        //    if (e.Status == LocalServerStatus.Started)
        //    {
        //        // 울산행정구역 표시
        //        //ShowLocalServerLayer(mapView, "BML_GADM_AS", true);
        //        ShowShapeLayer(mapView, "BML_GADM_AS", true);
        //        /*
        //                // Get the path to the first layer - the local feature service url + layer ID
        //                string layerUrl = _localFeatureService.Url + "/0";

        //                // Create the ServiceFeatureTable
        //                ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));

        //                // Create the Feature Layer from the table
        //                FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);
        //                layers["WTL_FLOW_PS"] = myFeatureLayer;

        //                // Add the layer to the map
        //                mapView.Map.OperationalLayers.Add(myFeatureLayer);

        //                try
        //                {
        //                    // Wait for the layer to load
        //                    await myFeatureLayer.LoadAsync();

        //                    // Set the viewpoint on the MapView to show the layer data
        //                    await mapView.SetViewpointGeometryAsync(myFeatureLayer.FullExtent, 50);
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.ToString(), "Error");
        //                }
        //         */
        //    }
        //}


        //// mpk 패키지파일의 위치 가져오기
        //private static string GetFeatureLayerPath()
        //{
        //    //return DataManager.GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
        //    //return GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
        //    return BizUtil.GetDataFolder("shape", "fms.mpk");
        //}


        #endregion



    }

}

