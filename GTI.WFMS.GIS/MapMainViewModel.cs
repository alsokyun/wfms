using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using GTI.WFMS.Models.Common;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Data;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.GIS.Module;
using GTIFramework.Common.MessageBox;
using System.IO;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using System.Threading.Tasks;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapMainViewModel : LayerModel, INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========

        // 선택한 피처 - 서버연계된Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public List<string> _selectedLayers = new List<string>();
        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;
        private Esri.ArcGISRuntime.Geometry.Geometry _geometry;


        public event PropertyChangedEventHandler PropertyChanged;



        // Coordinates for Ulsan
        private MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        //private double _ulsanScale = 150000;
        private double _ulsanScale = 500000;

        private FctDtl fctDtl = new FctDtl(); //시설물기본정보
        private Popup divLayer = new Popup(); //시설물레이어DIV
        private PopFct popFct = new PopFct(); //시설물정보DIV
        private Button ClearButton = new Button();
        private TextBox txtFTR_CDE = new TextBox();
        private TextBox txtFTR_IDN = new TextBox();
        #endregion



        #region ========== 인터페이스 오버라이딩 ==========

        /// <summary>
        /// Raises the <see cref="MapViewModel.PropertyChanged" /> event
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion



        #region ==========  Properties 정의 ==========

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        public RelayCommand<object> loadedCmd { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> toggleCmd { get; set; }
        public RelayCommand<object> closeCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }

        public RelayCommand<object> btnCmd { get; set; }
        public RelayCommand<object> completeCmd { get; set; }
        public RelayCommand<object> clearCmd { get; set; }
        public RelayCommand<object> findCmd { get; set; }

        public FctDtl FctDtl
        {
            get { return this.fctDtl; }
            set { this.fctDtl = value; }
        }


        #endregion




               
        #region ========== 생성자 ==========

        public MapMainViewModel()
        {

            //뷰객체를 파라미터로 전달받기
            //loadedCmd = new RelayCommand<object>(loadedMethod);
            /*
             */
            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {

                //뷰객체를 파라미터로 전달받기
                System.Windows.Controls.Grid divGrid = obj as System.Windows.Controls.Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayer = divGrid.FindName("divLayer") as Popup;
                this.ClearButton = divGrid.FindName("ClearButton") as Button;

                txtFTR_CDE = divGrid.FindName("txtFTR_CDE") as TextBox;
                txtFTR_IDN = divGrid.FindName("txtFTR_IDN") as TextBox;

                //지도초기화
                InitMap();


                //시설물레이어DIV 초기화작업
                InitDivLayer();

                InitUniqueValueRenderer();//렌더러초기생성작업

            });


            //레이어 ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;
                //IEnumerable<CheckBox> collection = doc.Children.OfType<CheckBox>();
                //CheckBox chkbox = collection.First();

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;

                //ShowLocalServerLayer(mapView, doc.Tag.ToString(), chk);
                ShowShapeLayer(mapView, doc.Tag.ToString(), chk);


                //선택된 레이어저장
                try
                {
                    if (chk)
                    {
                        _selectedLayers.Add(doc.Tag.ToString());
                    }
                    else
                    {
                        _selectedLayers.Remove(doc.Tag.ToString());
                    }
                }
                catch (Exception) { }
            });

            //팝업레이어 토글처리
            toggleCmd = new RelayCommand<object>(delegate (object obj)
            {
                StackPanel spLayer = divLayer.FindName("spLayer") as StackPanel;
                System.Windows.Controls.Grid gridTitle = divLayer.FindName("gridTitle") as System.Windows.Controls.Grid;

                spLayer.Visibility = spLayer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                if (spLayer.Visibility == Visibility.Collapsed)
                {
                    divLayer.Height = gridTitle.Height;
                }
                else
                {
                    divLayer.Height = gridTitle.Height + spLayer.Height;
                }
            });

            //팝업레이어 토글처리
            closeCmd = new RelayCommand<object>(delegate (object obj)
            {
                Popup divLayerInfo = obj as Popup;

                divLayerInfo.IsOpen = false;
            });


            // 레이어스타일 Renderer 초기화 - shape방식에서만 사용함
            //InitUniqueValueRenderer();

            //GIS초기화
            resetCmd = new RelayCommand<object>(delegate (object obj)
            {
                //0.맵초기화
                InitMap();

                //1.로컬서버 재기동
                //Initialize_LocalServer();

                //2.레이어 클리어
                mapView.Map.OperationalLayers.Clear();
                // 울산행정구역 표시
                //ShowLocalServerLayer(mapView, "BML_GADM_AS", true);
                ShowShapeLayer(mapView, "BML_GADM_AS", true);


                //3.열여있는 시설물정보창 닫기
                popFct.IsOpen = false;

                TreeView treeLayer = obj as TreeView;

                //레이어div 체크해제
                foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
                {
                    cb.IsChecked = false;
                }
                //선택된레이어 해제
                _selectedLayers.Clear();
            });



            btnCmd = new RelayCommand<object>(async delegate (object obj)
            {
                Button btn = obj as Button;
                switch (btn.Content.ToString())
                {
                    case "추가":
                        if (_selectedLayers.Count < 1)
                        {
                            MessageBox.Show("시설물을 선택하세요.");
                            return;
                        }
                        else if (_selectedLayers.Count > 1)
                        {
                            MessageBox.Show("시설물을 하나만 선택하세요.");
                            return;
                        }


                        //라인피처인 경우 - SketchEditor 를 GraphicOverlay에 생성한다
                        if (_selectedLayers[0].Equals("WTL_PIPE_LM") || _selectedLayers[0].Equals("WTL_SPLY_LS"))
                        {
                            try
                            {
                                // Let the user draw on the map view using the chosen sketch mode
                                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(SketchCreationMode.Polyline, true); //맵에 신규geometry 얻어오기

                                // Create and add a graphic from the geometry the user drew
                                SimpleLineSymbol symbol;
                                if (_selectedLayers[0].Equals("WTL_PIPE_LM"))
                                {
                                    symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                                }
                                else
                                {
                                    symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                                }

                                Graphic graphic = new Graphic(geometry, symbol);
                                _sketchOverlay.Graphics.Add(graphic);

                                // Enable/disable the clear and edit buttons according to whether or not graphics exist in the overlay
                                ClearButton.IsEnabled = _sketchOverlay.Graphics.Count > 0;
                            }
                            catch (TaskCanceledException)
                            {
                                // Ignore ... let the user cancel drawing
                            }
                            catch (Exception ex)
                            {
                                // Report exceptions
                                MessageBox.Show("Error drawing graphic shape: " + ex.Message);
                            }
                        }
                        //포인트피처의 경우는 클릭핸들러만 추가함
                        else
                        {
                            //추가처리 탭핸들러 추가
                            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                            mapView.GeoViewTapped -= handlerGeoViewTapped;
                            mapView.GeoViewTapped += handlerGeoViewTappedAddFeature;
                            MessageBox.Show("시설물을 추가할 지점을 마우스로 클릭하세요.");
                        }

                        break;

                    case "이동":
                        if (_selectedFeature == null)
                        {
                            MessageBox.Show("시설물을 선택하세요.");
                            return;
                        }


                        MessageBox.Show("이동할 지점을 마우스로 클릭하세요.");
                        //이동처리 탭핸들러 추가
                        mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                        mapView.GeoViewTapped -= handlerGeoViewTapped;
                        mapView.GeoViewTapped += handlerGeoViewTappedMoveFeature;
                        break;

                    case "삭제":
                        if (_selectedFeature == null)
                        {
                            MessageBox.Show("시설물을 선택하세요.");
                            return;
                        }

                        //삭제처리
                        //열여있는 시설물정보창 닫기
                        popFct.IsOpen = false;

                        // Load the feature.
                        //await _selectedFeature.LoadAsync();

                        Feature back_selectedFeature = _selectedFeature;
                        if (Messages.ShowYesNoMsgBox("시설물 위치정보를 삭제하시겠습니까?") == MessageBoxResult.Yes)
                        {
                            // Apply the edit to the feature table.
                            await _selectedFeature.FeatureTable.DeleteFeatureAsync(_selectedFeature);
                            _selectedFeature.Refresh();
                        }
                        else
                        {
                            await _selectedFeature.FeatureTable.AddFeatureAsync(back_selectedFeature);
                            _selectedFeature.Refresh();
                        }

                        break;

                    case "취소":
                        // Push the update to the service.
                        //ServiceFeatureTable serviceTableCancel = (ServiceFeatureTable)_selectedFeature.FeatureTable;
                        //serviceTableCancel.CancelLoad();
                        break;
                    default:
                        break;
                }
            });


            //도형클리어처리
            clearCmd = new RelayCommand<object>(delegate (object obj)
            {
                // Remove all graphics from the graphics overlay
                _sketchOverlay.Graphics.Clear();

                // Disable buttons that require graphics
                ClearButton.IsEnabled = false;
            });
            completeCmd = new RelayCommand<object>(async delegate (object obj)
            {
                mapView.SketchEditor.Stop();
                //추가된 도형 저장처리

                _selectedFeature.Geometry = _geometry;
                // Apply the edit to the feature table.
                await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                _selectedFeature.Refresh();
                MessageBox.Show("Added feature ", "Success!");

            });

            //시설물찾기
            //findCmd = new RelayCommand<object>(FindAction);

        }

        /// <summary>
        /// 해당시설물의 지도상위치 찾아가기
        /// </summary>
        /// <param name="FTR_CDE"></param>
        /// <param name="FTR_IDN"></param>
        /// <returns></returns>
        public async Task findFtrAsync(string FTR_CDE, string FTR_IDN)
        {

            string layerNm = "";
            try
            {
                layerNm = GetLayerNm(FTR_CDE);
                if ("".Equals(layerNm))
                {
                    MessageBox.Show("잘못된 레이어입니다.");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("잘못된 레이어입니다.");
                return;
            }


            //0.해당레이어표시 - 내부에서자동으로 로딩여부 체크함
            ShowShapeLayer(mapView, GetLayerNm(FTR_CDE), true);

            //1.해당레이어 가져오기
            FeatureLayer layer = layers[GetLayerNm(FTR_CDE)];



            // Remove any previous feature selections that may have been made.
            layer.ClearSelection();

            // Begin query process.
            await QueryStateFeature(FTR_CDE, FTR_IDN, layer);

        }



        #endregion


        // 레이에서 해당 Feature 찾기
        private async Task QueryStateFeature(string _FTR_CDE, string _FTR_IDN, FeatureLayer _featureLayer)
        {
            try
            {
                // 0.Feature 테이블 가져오기
                FeatureTable _featureTable = _featureLayer.FeatureTable;



                // Create a query parameters that will be used to Query the feature table.
                QueryParameters queryParams = new QueryParameters();


                // Construct and assign the where clause that will be used to query the feature table.
                queryParams.WhereClause = "upper(FTR_CDE) = '" + _FTR_CDE + "' AND FTR_IDN = " + _FTR_IDN ;


                List<Feature> features;
                try
                {
                    // Query the feature table.
                    FeatureQueryResult queryResult = await _featureTable.QueryFeaturesAsync(queryParams);

                    // Cast the QueryResult to a List so the results can be interrogated.
                    features = queryResult.ToList();
                }
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("보호된 메모리 접근 에러..");
                    return;
                }

                if (features.Any())
                {
                    // Create an envelope.
                    EnvelopeBuilder envBuilder = new EnvelopeBuilder(SpatialReferences.WebMercator);



                    // Loop over each feature from the query result.
                    foreach (Feature feature in features)
                    {
                        // Add the extent of each matching feature to the envelope.
                        //envBuilder.UnionOf(feature.Geometry.Extent); //복수의 피처영역 합치기

                        // Select each feature.
                        _featureLayer.SelectFeature(feature);
                        //해당피처로 이동
                        await mapView.SetViewpointCenterAsync(feature.Geometry.Extent.GetCenter(), 40000);
                    }

                    // Zoom to the extent of the selected feature(s).
                    //await mapView.SetViewpointGeometryAsync(envBuilder.ToGeometry(), 50);
                    
                }
                else
                {
                    MessageBox.Show("해당 시설물 위치를 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex, "Sample error");
            }
        }


        //시설물레이어DIV 초기화작업
        private void InitDivLayer()
        {
            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };

            StackPanel stContent = this.divLayer.FindName("stContent") as StackPanel;
            stContent.Children.Add(thumb);

            this.divLayer.MouseDown += (sender, e) =>
            {
                thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
            {
                this.divLayer.HorizontalOffset += e.HorizontalChange;
                this.divLayer.VerticalOffset += e.VerticalChange;
            };

        }



















        #region ============= 내부함수 ============= 

        private async void InitMap()
        {
            //지도위치 및 스케일 초기화
            await mapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);

            //Base맵 초기화
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);
            //this._map.Basemap = Basemap.CreateOpenStreetMap();

            //타일맵
            TileCache tileCache = new TileCache(BizUtil.GetDataFolder("tile", "korea.tpk"));
            ArcGISTiledLayer tileLayer = new ArcGISTiledLayer(tileCache);
            this._map.Basemap = new Basemap(tileLayer);


            //울산행정구역표시
            ShowShapeLayer(mapView, "BML_GADM_AS", true);


            //맵뷰 클릭이벤트 설정
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped += handlerGeoViewTapped;


            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
        }



        // 피처추가
        public async void handlerGeoViewTappedAddFeature(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Get the MapPoint from the EventArgs for the tap.
                MapPoint destinationPoint = e.Location;
                // Normalize the point - needed when the tapped location is over the international date line.
                destinationPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(destinationPoint);



                // Get the path to the first layer - the local feature service url + layer ID
                //string layerUrl = _localFeatureService.Url + "/" + GetLayerId(_selectedLayers[0]);

                // Create the ServiceFeatureTable
                //ServiceFeatureTable serviceFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));
                //FeatureLayer featureLayer = new FeatureLayer(serviceFeatureTable);

                // Wait for the layer to load
                //await featureLayer.LoadAsync();


                FeatureTable layerTable = layers[_selectedLayers[0]].FeatureTable;


                //피처추가
                Feature _addedFeature = layerTable.CreateFeature();
                _addedFeature.Geometry = destinationPoint;

                //속성추가
                //Field Field_FTR_CDE = new Field(FieldType.Text, "FTR_CDE", "시설물코드", 50);
                //Field Field_FTR_IDN = new Field(FieldType.Int32, "FTR_IDN", "관리번호", 10);
                //Field Field_SHT_NUM = new Field(FieldType.Text, "SHT_NUM", "도엽번호", 50);

                string ftr_cde = "SA118";
                try
                {
                    //ftr_cde = _selectedLayers[0].Split(',')[0];
                }
                catch (Exception){}
                
                _addedFeature.SetAttributeValue("FTR_CDE", ftr_cde);
                _addedFeature.SetAttributeValue("FTR_IDN", 999f);
                _addedFeature.SetAttributeValue("SHT_NUM", "99999");
                _addedFeature.SetAttributeValue("SHT_NUM", "99999");
                _addedFeature.SetAttributeValue("HJD_CDE", "3171033000");
                _addedFeature.SetAttributeValue("MNG_CDE", "MNG401");




                await layerTable.AddFeatureAsync(_addedFeature);
                //추가내용 새로고침
                _addedFeature.Refresh();

                MessageBox.Show("Added feature ", "Success!");

                //이벤트핸들러원복
                mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                mapView.GeoViewTapped += handlerGeoViewTapped;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }

        }


        //맵뷰 클릭이벤트 핸들러 -  이동처리(ServiceFeature)
        public async void handlerGeoViewTappedMoveFeature_org(object sender, GeoViewInputEventArgs e)
        {

            //이동처리
            if (_selectedFeature != null)
            {
                try
                {
                    // Get the MapPoint from the EventArgs for the tap.
                    MapPoint destinationPoint = e.Location;

                    // Normalize the point - needed when the tapped location is over the international date line.
                    destinationPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(destinationPoint);

                    // Load the feature.
                    //await _selectedFeature.LoadAsync();

                    // Update the geometry of the selected feature.
                    _selectedFeature.Geometry = destinationPoint;

                    // Apply the edit to the feature table.
                    await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                    _selectedFeature.Refresh();

                    // Push the update to the service. - Save버튼에서 최종저장
                    //ServiceFeatureTable serviceTable = (ServiceFeatureTable)_selectedFeature.FeatureTable;
                    //await serviceTable.ApplyEditsAsync();
                    //MessageBox.Show("Moved feature " + _selectedFeature.Attributes["objectid"], "Success!");


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error when moving feature.");
                }

            }
        }

        //맵뷰 클릭이벤트 핸들러 -  이동처리(shape파일)
        public async void handlerGeoViewTappedMoveFeature(object sender, GeoViewInputEventArgs e)
        {

            //이동처리
            if (_selectedFeature != null)
            {
                try
                {
                    // Get the MapPoint from the EventArgs for the tap.
                    MapPoint mapPoint = e.Location;
                    // Normalize the point - needed when the tapped location is over the international date line.
                    mapPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(mapPoint);
                    Polyline polyline;
                    Polygon polygon;


                    // Update the geometry of the selected feature.
                    Feature back_selectedFeature = _selectedFeature;


                    //라인피처인 경우 - 평행이동 라인을 만든다
                    if (_selectedFeature.Geometry is Polyline)
                    {
                        polyline = (Polyline)_selectedFeature.Geometry;

                        List<MapPoint> points = new List<MapPoint>();
                        foreach (var part in polyline.Parts)
                        {
                            //라인의 첫번째점을 기준으로 이동
                            double dx = mapPoint.X - part.Points[0].X;
                            double dy = mapPoint.Y - part.Points[0].Y;

                            foreach (var pt in part.Points)
                            {
                                MapPoint mpt = new MapPoint(pt.X + dx, pt.Y + dy, SpatialReferences.WebMercator);
                                points.Add(mpt);
                            }
                        }

                        _selectedFeature.Geometry = new Polyline(points);
                    }
                    //폴리곤 피처인경우 - 평행이동 폴리곤을 만든다
                    else if (_selectedFeature.Geometry is Polygon)
                    {
                        polygon = (Polygon)_selectedFeature.Geometry;

                        List<MapPoint> points = new List<MapPoint>();
                        foreach (var part in polygon.Parts)
                        {
                            //라인의 첫번째점을 기준으로 이동
                            double dx = mapPoint.X - part.Points[0].X;
                            double dy = mapPoint.Y - part.Points[0].Y;

                            foreach (var pt in part.Points)
                            {
                                MapPoint mpt = new MapPoint(pt.X + dx, pt.Y + dy, SpatialReferences.WebMercator);
                                points.Add(mpt);
                            }
                        }

                        _selectedFeature.Geometry = new Polygon(points);
                    }
                    //포인트 피처인 경우는 위치만 변경하면됨
                    else
                    {
                        _selectedFeature.Geometry = mapPoint;
                    }


                    if (Messages.ShowYesNoMsgBox("시설물 위치이동을 저장하시겠습니까?") == MessageBoxResult.Yes)
                    {
                        // Apply the edit to the feature table.
                        await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                        _selectedFeature.Refresh();
                        MessageBox.Show("Moved feature ", "Success!");
                    }
                    else
                    {
                        await _selectedFeature.FeatureTable.UpdateFeatureAsync(back_selectedFeature);
                        _selectedFeature.Refresh();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error when moving feature.");
                }

            }

            //이벤트핸들러원복
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped += handlerGeoViewTapped;

        }







        //맵뷰 클릭이벤트 핸들러 - 상세정보팝업 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                FeatureLayer layer = new FeatureLayer(); //이벤트발생 레이어



                // PopFct 객체 생성여부 체크관련 - 검증안됨
                /*
                if (App.Current.Windows.Count > 1)
                {
                    // do your code here, when you have more than one window open state this code will execute
                }

                if (FmsUtil.IsWindowOpen<Popup>())
                {
                    PopFct pop = new PopFct();
                    (Activator.GetObject((new PopFct()).GetType(), null) as PopFct).IsOpen = false;
                }
                 */




                /*
                divLayerInfo.PlacementRectangle = new Rect(e.Position.X, e.Position.Y, 250, 300);
                divLayerInfo.IsOpen = true;
                 */

                // Perform the identify operation.
                IdentifyLayerResult IR_WTL_MANH_PS = await mapView.IdentifyLayerAsync(layers["WTL_MANH_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_STPI_PS = await mapView.IdentifyLayerAsync(layers["WTL_STPI_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_HEAD_PS = await mapView.IdentifyLayerAsync(layers["WTL_HEAD_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_GAIN_PS = await mapView.IdentifyLayerAsync(layers["WTL_GAIN_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_SERV_PS = await mapView.IdentifyLayerAsync(layers["WTL_SERV_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_FLOW_PS = await mapView.IdentifyLayerAsync(layers["WTL_FLOW_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_FIRE_PS_SA118 = await mapView.IdentifyLayerAsync(layers["WTL_FIRE_PS,FTR_CDE='SA118'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_FIRE_PS_SA119 = await mapView.IdentifyLayerAsync(layers["WTL_FIRE_PS,FTR_CDE='SA119'"], e.Position, 20, false);

                IdentifyLayerResult IR_WTL_VALV_PS_SA200 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA200'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA201 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA201'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA202 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA202'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA203 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA203'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA204 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA204'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA205 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA205'"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_VALV_PS_SA206 = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS,FTR_CDE='SA206'"], e.Position, 20, false);
                
                IdentifyLayerResult IR_WTL_RSRV_PS = await mapView.IdentifyLayerAsync(layers["WTL_RSRV_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_PRGA_PS = await mapView.IdentifyLayerAsync(layers["WTL_PRGA_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_META_PS = await mapView.IdentifyLayerAsync(layers["WTL_META_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_LEAK_PS = await mapView.IdentifyLayerAsync(layers["WTL_LEAK_PS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_PIPE_LM = await mapView.IdentifyLayerAsync(layers["WTL_PIPE_LM"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_SPLY_LS = await mapView.IdentifyLayerAsync(layers["WTL_SPLY_LS"], e.Position, 20, false);
                IdentifyLayerResult IR_WTL_PURI_AS = await mapView.IdentifyLayerAsync(layers["WTL_PURI_AS"], e.Position, 20, false);

                Feature identifiedFeature; //이벤트 타겟레이어



                if (IR_WTL_STPI_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_STPI_PS.GeoElements[0];
                    layer = layers["WTL_STPI_PS"]; //선택한 레이어
                }
                else if (IR_WTL_MANH_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_MANH_PS.GeoElements[0];
                    layer = layers["WTL_MANH_PS"];
                }
                else if (IR_WTL_HEAD_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_HEAD_PS.GeoElements[0];
                    layer = layers["WTL_HEAD_PS"];
                }
                else if (IR_WTL_GAIN_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_GAIN_PS.GeoElements[0];
                    layer = layers["WTL_GAIN_PS"];
                }
                else if (IR_WTL_SERV_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_SERV_PS.GeoElements[0];
                    layer = layers["WTL_SERV_PS"];
                }
                else if (IR_WTL_FLOW_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_FLOW_PS.GeoElements[0];
                    layer = layers["WTL_FLOW_PS"];
                }
                else if (IR_WTL_FIRE_PS_SA118.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_FIRE_PS_SA118.GeoElements[0];
                    layer = layers["WTL_FIRE_PS,FTR_CDE='SA118'"];
                }
                else if (IR_WTL_FIRE_PS_SA119.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_FIRE_PS_SA119.GeoElements[0];
                    layer = layers["WTL_FIRE_PS,FTR_CDE='SA119'"];
                }
                else if (IR_WTL_VALV_PS_SA200.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA200.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA200'"];
                }
                else if (IR_WTL_VALV_PS_SA201.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA201.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA201'"];
                }
                else if (IR_WTL_VALV_PS_SA202.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA202.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA202'"];
                }
                else if (IR_WTL_VALV_PS_SA203.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA203.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA203'"];
                }
                else if (IR_WTL_VALV_PS_SA204.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA204.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA204'"];
                }
                else if (IR_WTL_VALV_PS_SA205.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA205.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA205'"];
                }
                else if (IR_WTL_VALV_PS_SA206.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_VALV_PS_SA206.GeoElements[0];
                    layer = layers["WTL_VALV_PS,FTR_CDE='SA206'"];
                }
                else if (IR_WTL_RSRV_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_RSRV_PS.GeoElements[0];
                    layer = layers["WTL_RSRV_PS"];
                }
                else if (IR_WTL_PRGA_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_PRGA_PS.GeoElements[0];
                    layer = layers["WTL_PRGA_PS"];
                }
                else if (IR_WTL_META_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_META_PS.GeoElements[0];
                    layer = layers["WTL_META_PS"];
                }
                else if (IR_WTL_LEAK_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_LEAK_PS.GeoElements[0];
                    layer = layers["WTL_LEAK_PS"];
                }
                else if (IR_WTL_PIPE_LM.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_PIPE_LM.GeoElements[0];
                    layer = layers["WTL_PIPE_LM"];
                }
                else if (IR_WTL_SPLY_LS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_SPLY_LS.GeoElements[0];
                    layer = layers["WTL_SPLY_LS"];
                }
                else if (IR_WTL_PURI_AS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_PURI_AS.GeoElements[0];
                    layer = layers["WTL_PURI_AS"];
                }
                else
                {
                    // Return if there's nothing to show.
                    return;
                }




                //선택해제
                if (_selectedFeature != null)
                {
                    try
                    {
                        // Reset the selection.
                        layer.ClearSelection();
                        _selectedFeature = null;
                    }
                    catch (Exception) { }
                }

                //선택처리
                layer.SelectFeature(identifiedFeature); //시설물선택활성화 처리
                                                        //_selectedFeature = (ArcGISFeature)identifiedFeature; //선택피처 ArcGISFeature 변환저장
                _selectedFeature = (Feature)identifiedFeature; //선택피처 ArcGISFeature 변환저장






                //팝업열기 & 위치
                popFct.IsOpen = false;

                popFct = new PopFct();
                popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                popFct.IsOpen = true;
                popFct.DataContext = this;



                //선택된 레이어에서 속성정보 가져오기
                //& 레이어정보 세팅
                try
                {
                    FctDtl.FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                    FctDtl.FTR_NAM = BizUtil.GetCodeNm("Select_FTR_NM", FctDtl.FTR_CDE);
                    FctDtl.FTR_IDN = Int32.Parse(identifiedFeature.GetAttributeValue("FTR_IDN").ToString());
                    FctDtl.SHT_NUM = identifiedFeature.GetAttributeValue("SHT_NUM").ToString();
                    FctDtl.HJD_CDE = identifiedFeature.GetAttributeValue("HJD_CDE").ToString();
                    FctDtl.HJD_NAM = BizUtil.GetCodeNm("Select_ADAR_NM", FctDtl.HJD_CDE);
                    FctDtl.MNG_CDE = identifiedFeature.GetAttributeValue("MNG_CDE").ToString();
                    FctDtl.MNG_NAM = BizUtil.GetCdNm("250101", FctDtl.MNG_CDE);//관리기관
                    FctDtl.IST_YMD = identifiedFeature.GetAttributeValue("IST_YMD").ToString();
                }
                catch (Exception) { }
                try
                {
                    FctDtl.MOP_CDE = identifiedFeature.GetAttributeValue("MOP_CDE").ToString();
                    FctDtl.MOP_NAM = BizUtil.GetCdNm("250102", FctDtl.MOP_CDE);//관재질
                    FctDtl.MOF_NAM = FctDtl.MOP_NAM; //무조건 MOF에 형식저장!
                }
                catch (Exception) { }
                try
                {
                    FctDtl.MOF_CDE = identifiedFeature.GetAttributeValue("MOF_CDE").ToString();
                    FctDtl.MOF_NAM = BizUtil.GetCdNm("250004", FctDtl.MOF_CDE);//계량기형식
                }
                catch (Exception) { }
            }
            catch (Exception)
            {
                Console.WriteLine("레이어 featrue click error...");
            }


        }


        #endregion






















    }
}

