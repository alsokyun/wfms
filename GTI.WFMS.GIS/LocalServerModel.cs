using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using GTI.WFMS.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// MapViewModel 에서 레이어관련 항목 및 처리
    /// </summary>
    public class LocalServerModel
    {
        //생성자
        public LocalServerModel()
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

        public MapView mapView; //뷰의 MapView



        public Dictionary<string, FeatureLayer> layers = new Dictionary<string, FeatureLayer>()
        {
            {"WTL_FLOW_PS",  new FeatureLayer()},
            {"WTL_FIRE_PS^SA118",  new FeatureLayer()},
            {"WTL_FIRE_PS^SA119",  new FeatureLayer()},
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
            {"WTL_VALV_PS^SA200",  new FeatureLayer()},
            {"WTL_VALV_PS^SA201",  new FeatureLayer()},
            {"WTL_VALV_PS^SA202",  new FeatureLayer()},
            {"WTL_VALV_PS^SA203",  new FeatureLayer()},
            {"WTL_VALV_PS^SA204",  new FeatureLayer()},
            {"WTL_VALV_PS^SA205",  new FeatureLayer()},
            {"WTL_VALV_PS^SA206",  new FeatureLayer()},


            {"BML_GADM_AS",  new FeatureLayer()},
            {"WTL_PURI_AS",  new FeatureLayer()},

            {"WTL_PIPE_LM",  new FeatureLayer()},
            {"WTL_SPLY_LS",  new FeatureLayer()},
        };























        /// LocalServer에서 해당레이어의 LayerI 가져오기
        public string GetLayerId(string layerNm)
        {
            string layerId = "1";
            switch (layerNm)
            {
                case "WTL_PIPE_LM":
                    layerId = "0";
                    break;
                case "WTL_SPLY_LS":
                    layerId = "1";
                    break;
                case "WTL_STPI_PS":
                    layerId = "2";
                    break;
                case "WTL_MANH_PS":
                    layerId = "3";
                    break;
                case "WTL_HEAD_PS":
                    layerId = "4";
                    break;
                case "WTL_GAIN_PS":
                    layerId = "5";
                    break;
                case "WTL_PURI_AS":
                    layerId = "6";
                    break;
                case "WTL_SERV_PS":
                    layerId = "7";
                    break;
                case "WTL_FLOW_PS":
                    layerId = "8";
                    break;
                case "WTL_FIRE_PS^SA118":
                case "WTL_FIRE_PS^SA119":
                    layerId = "9";
                    break;
                case "WTL_RSRV_PS":
                    layerId = "10";
                    break;
                case "WTL_PRGA_PS":
                    layerId = "11";
                    break;
                case "WTL_META_PS":
                    layerId = "12";
                    break;
                case "WTL_VALV_PS^SA200":
                case "WTL_VALV_PS^SA201":
                case "WTL_VALV_PS^SA202":
                case "WTL_VALV_PS^SA203":
                case "WTL_VALV_PS^SA204":
                case "WTL_VALV_PS^SA205":
                case "WTL_VALV_PS^SA206":
                    layerId = "13";
                    break;
                case "WTL_PRES_PS":
                    layerId = "14";
                    break;
                case "BML_GADM_AS":
                    layerId = "15";
                    break;

                default:
                    break;
            }
            return layerId;
        }




        /// <summary>
        /// 레이어 보이기/끄기 - LocalServer버전
        /// </summary>
        /// <param name="_map"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public async void ShowLocalServerLayer(MapView _mapView, string _layerNm, bool chk)
        {
            try
            {
                // 0.해당레이어 가져오기
                string filterExp = "";
                string shapeNm = "";

                try
                {
                    string[] ary = _layerNm.Split('^');
                    shapeNm = ary[0]; //레이어테이블명
                    filterExp = "FTR_CDE='" + ary[1] + "'"; //필터표현식
                }
                catch (Exception) { }

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
                                string layerUrl = _localFeatureService.Url + "/" + GetLayerId(_layerNm);

                                // Create the ServiceFeatureTable
                                ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));

                                // Create the Feature Layer from the table
                                FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);
                                layers[_layerNm] = myFeatureLayer; //생성한레이어를 딕셔너리에 저장

                                // 필터링 인수있으면 하위시설물으로 필터
                                if (!FmsUtil.IsNull(filterExp))
                                {
                                    myFeatureLayer.DefinitionExpression = filterExp;
                                }


                                //렌더러는 레이어 각각 할당해야하므로 렌더러복사하여 할당
                                myFeatureLayer.Renderer = CmmRun.uniqueValueRenderer.Clone();

                                // Add the layer to the map
                                _mapView.Map.OperationalLayers.Add(myFeatureLayer);

                                try
                                {
                                    // Wait for the layer to load
                                    await myFeatureLayer.LoadAsync();

                                    //한반도이내인 경우만
                                    if (myFeatureLayer.FullExtent.XMax < 14566451.0 && myFeatureLayer.FullExtent.YMax < 5362210.0
                                        && myFeatureLayer.FullExtent.XMin > 13691400.0 && myFeatureLayer.FullExtent.YMin < 3797264.0)
                                    {
                                        // Set the viewpoint on the MapView to show the layer data
                                        await _mapView.SetViewpointGeometryAsync(myFeatureLayer.FullExtent, 50);
                                    }


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
            catch (Exception )
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }





        #region ============ LocalServer (start) 관련부분 ==============
        // Hold a reference to the local feature service; the ServiceFeatureTable will be loaded from this service
        public LocalFeatureService _localFeatureService;
        
        public async void Initialize_LocalServer()
        {

            try
            {
                // 1.LocalServer 모듈 시작...
                // LocalServer must not be running when setting the data path.
                if (LocalServer.Instance.Status == LocalServerStatus.Started)
                {
                    await LocalServer.Instance.StopAsync();
                }

                if (LocalServer.Instance.Status == LocalServerStatus.Stopped)
                {
                    // Set the local data path - must be done before starting. On most systems, this will be C:\EsriSamples\AppData.
                    // This path should be kept short to avoid Windows path length limitations.
                    string tempDataPathRoot = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Windows)).FullName;
                    string tempDataPath = BizUtil.GetDataFolder("EsriLocalServer", "AppData");
                    
                    Directory.CreateDirectory(tempDataPath); // CreateDirectory won't overwrite if it already exists.
                    LocalServer.Instance.AppDataPath = tempDataPath;

                    // Start the local server instance
                    await LocalServer.Instance.StartAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Please ensure that local server is installed prior to using the sample. See instructions in readme.md. Message: {0}", ex.Message), "Local Server failed to start");
                return;
            }


            // 2.서버상에 LocalFeatureService 서비스시작..
            // Load the sample data and get the path
            if (_localFeatureService == null)
            {
                string myfeatureServicePath = GetFeatureLayerPath();

                // Create the feature service to serve the local data
                _localFeatureService = new LocalFeatureService(myfeatureServicePath);
                //_localFeatureService.MaxRecords = 3000; //피처서비스 최대 표현레코드 수

                // Listen to feature service status changes
                _localFeatureService.StatusChanged += _localFeatureService_StatusChanged;

                // Start the feature service
                try
                {
                    await _localFeatureService.StartAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "The feature service failed to load");
                }
            }
        }

        private void _localFeatureService_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Load the map from the service once ready
            if (e.Status == LocalServerStatus.Started)
            {
                // 행정구역 표시
                ShowLocalServerLayer(mapView, "BML_GADM_AS", true );
                /*
                        // Get the path to the first layer - the local feature service url + layer ID
                        string layerUrl = _localFeatureService.Url + "/0";

                        // Create the ServiceFeatureTable
                        ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));

                        // Create the Feature Layer from the table
                        FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);
                        layers["WTL_FLOW_PS"] = myFeatureLayer;

                        // Add the layer to the map
                        mapView.Map.OperationalLayers.Add(myFeatureLayer);

                        try
                        {
                            // Wait for the layer to load
                            await myFeatureLayer.LoadAsync();

                            // Set the viewpoint on the MapView to show the layer data
                            await mapView.SetViewpointGeometryAsync(myFeatureLayer.FullExtent, 50);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error");
                        }
                 */
                //MessageBox.Show(_localFeatureService.Url.ToString());

            }
        }


        // mpk 패키지파일의 위치 가져오기
        private static string GetFeatureLayerPath()
        {
            //return DataManager.GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
            //return GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
            return BizUtil.GetDataFolder("tile", "wfms.mpk");
        }


        #endregion



    }

}

