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
using System.Collections;
using Prism.Regions;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapLayerViewModel : LyrModel, INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public MapView mapView; //뷰의 MapView


        // 선택한 피처 - 서버연계된Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();
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

        //private FctDtl fctDtl = new FctDtl(); //시설물기본정보
        private Popup divLayer = new Popup(); //시설물레이어DIV
        //private PopFct popFct = new PopFct(); //시설물정보DIV
        private Popup popFct = new Popup(); //시설물정보DIV
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

        public Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };
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

        public RelayCommand<object> CallPageCmd { get; set; }//시설물팝업에서 시설물메뉴화면 호출작업
        public RelayCommand<object> ChgImgCmd { get; set; }//시설물팝업에서 아이콘파일변경작업


        //시설물기본정보
        private CmmDtl fctDtl = new CmmDtl(); 
        public CmmDtl FctDtl
        {
            get { return this.fctDtl; }
            set
            {
                this.fctDtl = value;
                OnPropertyChanged("FctDtl");
            }
        }
        //시설물미리보기 소스이미지
        private BitmapImage bitImg;
        public BitmapImage BitImg
        {
            get { return bitImg; }
            set
            {
                this.bitImg = value;
                OnPropertyChanged("BitImg");
            }
        }

        #endregion





        #region ========== 생성자 ==========

        public MapLayerViewModel()
        {
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


                //비트맵초기화(시설물상세DIV 아이콘)
                BitImg = new BitmapImage();
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
                        _selectedLayerNms.Add(doc.Tag.ToString());
                        _selectedLayerNm = doc.Tag.ToString();
                    }
                    else
                    {
                        _selectedLayerNms.Remove(doc.Tag.ToString());
                        _selectedLayerNm = _selectedLayerNms.LastOrDefault();

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


                //3.열여있는 시설물정보창 닫기
                popFct.IsOpen = false;

                TreeView treeLayer = obj as TreeView;

                //레이어div 체크해제
                foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
                {
                    cb.IsChecked = false;
                }
                //선택된레이어 해제
                _selectedLayerNms.Clear();
                _selectedLayerNm = "";
            });

            //시설물팝업에서 시설물메뉴화면 호출작업
            CallPageCmd = new RelayCommand<object>(delegate (object obj) {

                FctDtl fctDtl = obj as FctDtl;

                IRegionManager regionManager = FmsUtil.__regionManager;
                ViewsCollection views = regionManager.Regions["ContentRegion"].ActiveViews as ViewsCollection;

                foreach (var v in views)
                {
                    MapLayerView mapMainView = v as MapLayerView;
                    //MainWinViewModel vm = ((System.Windows.Controls.Grid)((ContentControl)mapMainView.Parent).Parent).DataContext as MainWinViewModel;
                    break;
                }

            });

            //파일찾기버튼 이벤트
            ChgImgCmd = new RelayCommand<object>(delegate (object obj)
            {
                // 전달된 파라미터 
                if (obj == null)
                {
                    Messages.ShowErrMsgBox("시설물코드가 존재하지 않습니다.");
                    return;
                }
                string _FTR_CDE = obj as string;

                // UniqueValueRenderer 자원해제
                //uniqueValueRenderer = new UniqueValueRenderer();
                //layers[_selectedLayerNm].ResetRenderer();

                // 파일탐색기 열기
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    //아이콘 파일경로
                    string icon_foler = Path.Combine(BizUtil.GetDataFolder(), "style_img");


                    FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //파일인포
                    foreach (FileInfo fi in files)
                    {
                        try
                        {

                            //해당이미지파일을 FTR_CDE ex)SA117 이름의파일로 복사
                            fi.CopyTo( Path.Combine(icon_foler, _FTR_CDE), true );
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox(ex.Message);
                        }
                        finally
                        {
                            //1.렌더러 재구성
                            InitUniqueValueRenderer();

                            //2.레이어의 렌더러 재세팅
                            foreach (string sel in _selectedLayerNms)
                            {
                                layers[sel].Renderer = uniqueValueRenderer.Clone();
                                layers[sel].RetryLoadAsync();
                            }

                            //3.팝업이미지소스 업데이트
                            BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();

                        }
                    }
                }
            });



            btnCmd = new RelayCommand<object>(async delegate (object obj)
            {
                Button btn = obj as Button;
                switch (btn.Content.ToString())
                {
                    case "시설물편집":
                        //시설물편집팝업호출



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
                //FeatureLayer __featureLayer = _featureLayer.Clone() as FeatureLayer;
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
            mapView.GeoViewTapped -= handlerGeoViewTapped;
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
                //string layerUrl = _localFeatureService.Url + "/" + GetLayerId(_selectedLayer);

                // Create the ServiceFeatureTable
                //ServiceFeatureTable serviceFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));
                //FeatureLayer featureLayer = new FeatureLayer(serviceFeatureTable);

                // Wait for the layer to load
                //await featureLayer.LoadAsync();


                FeatureTable layerTable = layers[_selectedLayerNm].FeatureTable;


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
                    //ftr_cde = _selectedLayer.Split(',')[0];
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
                mapView.GeoViewTapped -= handlerGeoViewTapped;
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
            mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;


        }







        //맵뷰 클릭이벤트 핸들러 - 상세정보팝업 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
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




                // 위치에해당하는 피처찾은 결과
                // Perform the identify operation.
                IdentifyLayerResult IR_SEL = await mapView.IdentifyLayerAsync(layers[_selectedLayerNm], e.Position, 5, false);

                // 이벤트 타겟피처
                Feature identifiedFeature; 
                if (IR_SEL.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_SEL.GeoElements[0];
                }
                else
                {
                    // Return if there's nothing to show.
                    return;
                }




                // 기존선택해제
                if (_selectedFeature != null)
                {
                    try
                    {
                        // Reset the selection.
                        layers[_selectedLayerNm].ClearSelection();
                        _selectedFeature = null;
                    }
                    catch (Exception) { }
                }

                // 선택처리
                layers[_selectedLayerNm].SelectFeature(identifiedFeature); //시설물선택활성화 처리
                //_selectedFeature = (ArcGISFeature)identifiedFeature; //선택피처 ArcGISFeature 변환저장
                _selectedFeature = (Feature)identifiedFeature; //선택피처 ArcGISFeature 변환저장





                //0.선택된 레이어에서 속성정보 가져오기
                string _FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                string _FTR_IDN = identifiedFeature.GetAttributeValue("FTR_IDN").ToString();


                //1.DB시설물조회 후 레이어정보 세팅
                if (FmsUtil.IsNull(_FTR_CDE) || FmsUtil.IsNull(_FTR_IDN))
                {
                    Messages.ShowErrMsgBox("시설물 DB정보가 없습니다.");
                    return;
                }

                // 시설물정보팝업 보이기
                ShowFtrPop(_FTR_CDE, _FTR_IDN, e);

            }
            catch (Exception)
            {
                Console.WriteLine("레이어 featrue click error...");
            }

        }



        // 시설물정보팝업 보이기
        private void ShowFtrPop(string fTR_CDE, string fTR_IDN, GeoViewInputEventArgs e)
        {
            Hashtable param = new Hashtable();
            switch (fTR_CDE)
            {
                case "SA001": //상수관로
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_PIPE_LM();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtlPipeDtl2");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA002": //급수관로
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_SPLY_LS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectSupDutDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA003": //스탠파이프
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_STPI_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectStndPiDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA100": //상수맨홀
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_MANH_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtsMnhoDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA110": //수원지
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_HEAD_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtrSourDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA112": //취수장
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_GAIN_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectIntkStDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA113": //정수장
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_PURI_AS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectFiltPltDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                case "SA114": //배수지
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_PIPE_LM();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtrSupDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;


                case "SA117": //유량계
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_FLOW_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectFlowMtDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;

                    break;

                case "SA118": case "SA119": //소화전,급수탑
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_FIRE_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectFireFacDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;

                    break;


                case "SA120": //저수조
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_RSRV_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtrTrkDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;


                case "SA121": //수압계
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_PRGA_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectWtprMtDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;


                case "SA122": //급수전계량기
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_META_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectHydtMetrDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;


                case "SA200": case "SA201": case "SA202": case "SA203": case "SA204": case "SA205": case "SA206": //변류시설
                    //팝업열기 & 위치
                    popFct.IsOpen = false;

                    popFct = new WTL_VALV_PS();
                    popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                    popFct.IsOpen = true;
                    popFct.DataContext = this;

                    param = new Hashtable();
                    param.Add("FTR_CDE", fTR_CDE);
                    param.Add("FTR_IDN", fTR_IDN);
                    param.Add("sqlId", "SelectValvFacDtl");

                    this.FctDtl = BizUtil.SelectObject(param) as CmmDtl;
                    break;

                default:
                    break;

            }


            //아이콘이미지 설정
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.UriSource = new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), fTR_CDE), UriKind.Relative);
            //bi.EndInit();
            //BitImg = bi;
            BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), fTR_CDE))).Clone();

        }


        #endregion






















    }
}

