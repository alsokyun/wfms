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
using GTI.WFMS.GIS.Pop.View;
using Esri.ArcGISRuntime;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapMainViewModel : LocalServerModel, INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========

        // 선택한 피처 - 서버연계된Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();
        
        
        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;
        //private Esri.ArcGISRuntime.Geometry.Geometry _geometry;


        public event PropertyChangedEventHandler PropertyChanged;




        //private FctDtl fctDtl = new FctDtl(); //시설물기본정보
        private Popup divLayer = new Popup(); //시설물레이어DIV
        private Popup popFct = new Popup(); //시설물정보DIV
        private Button ClearButton = new Button();
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
        public RelayCommand<object> resetCmd { get; set; }
        public RelayCommand<object> levelCmd { get; set; }

        
        public RelayCommand<object> completeCmd { get; set; }
        public RelayCommand<object> clearCmd { get; set; }

        public RelayCommand<object> CallPageCmd { get; set; }//시설물팝업에서 시설물메뉴화면 호출작업

        public RelayCommand<object> EditCmd { get; set; }

        


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

        public MapMainViewModel()
        {
            //string licenseKey = "runtimelite,1000,rud1244207246,none,9TJC7XLS1MJPF5KHT033"; //그린텍
            //string licenseKey = "runtimelite,1000,rud9177830334,none,A3E60RFLTFM5NERL1040"; //kyun0828 free 
            
            //ArcGISRuntimeEnvironment.SetLicense(licenseKey);


            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {

                //뷰객체를 파라미터로 전달받기
                System.Windows.Controls.Grid divGrid = obj as System.Windows.Controls.Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayer = divGrid.FindName("divLayer") as Popup;
                this.ClearButton = divGrid.FindName("ClearButton") as Button;


                //지도초기화
                InitMap();


                //시설물레이어DIV 초기화작업
                InitDivLayer();

                GisCmm.InitUniqueValueRenderer();//렌더러초기생성작업


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

                ShowLocalServerLayer(mapView, doc.Tag.ToString(), chk);
                //ShowShapeLayer(mapView, doc.Tag.ToString(), chk);


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



            // 레이어스타일 Renderer 초기화 - shape방식에서만 사용함
            //InitUniqueValueRenderer();

            levelCmd = new RelayCommand<object>(delegate (object obj)
            {
                MessageBox.Show("MapScale - " + mapView.MapScale);
            });

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
                _selectedFeature = null;
                try
                {
                    layers[_selectedLayerNm].ClearSelection();
                }
                catch (Exception) { }
                _selectedLayerNms.Clear();
                _selectedLayerNm = "";


                // 울산행정구역 표시
                ShowLocalServerLayer(mapView, "BML_GADM_AS", true ) ;
            });

            //시설물팝업에서 시설물메뉴화면 호출작업
            CallPageCmd = new RelayCommand<object>(delegate (object obj) {

                FctDtl fctDtl = obj as FctDtl;

                IRegionManager regionManager = FmsUtil.__regionManager;
                ViewsCollection views = regionManager.Regions["ContentRegion"].ActiveViews as ViewsCollection;

                foreach (var v in views)
                {
                    MapArcObjView mapMainView = v as MapArcObjView;
                    //MainWinViewModel vm = ((System.Windows.Controls.Grid)((ContentControl)mapMainView.Parent).Parent).DataContext as MainWinViewModel;
                    break;
                }

            });

            //시설물편집창
            EditCmd = new RelayCommand<object>(delegate (object obj)
            {
                EditWinView view = new EditWinView();
                if (view.ShowDialog() is bool)
                {
                    //재조회
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

                //_selectedFeature.Geometry = _geometry;
                // Apply the edit to the feature table.
                await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                _selectedFeature.Refresh();
                MessageBox.Show("Added feature ", "Success!");

            });


        }

        /// <summary>
        /// 해당시설물의 지도상위치 찾아가기(업무화면에서 호출됨) 
        /// </summary>
        /// <param name="FTR_CDE"></param>
        /// <param name="FTR_IDN"></param>
        /// <returns></returns>
        public async Task findFtrAsync(string FTR_CDE, string FTR_IDN)
        {

            string layerNm = "";
            try
            {
                layerNm = GisCmm.GetLayerNm(FTR_CDE);
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
            ///ShowShapeLayer(mapView, GisCmm.GetLayerNm(FTR_CDE), true);

            //1.해당레이어 가져오기
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];



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
                queryParams.WhereClause = " FTR_CDE = '" + _FTR_CDE + "' ";
                if (!FmsUtil.IsNull(_FTR_IDN))
                {
                    queryParams.WhereClause += " AND FTR_IDN = " + _FTR_IDN;
                }


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


        
















        #region ============= 내부함수 ============= 

        private async void InitMap()
        {
            //지도위치 및 스케일 초기화
            await mapView.SetViewpointCenterAsync(GisCmm._ulsanCoords, GisCmm._ulsanScale);

            //Base맵 초기화
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);
            //this._map.Basemap = Basemap.CreateOpenStreetMap();

            //타일맵
            TileCache tileCache = new TileCache(BizUtil.GetDataFolder("tile", "korea.tpk"));
            ArcGISTiledLayer tileLayer = new ArcGISTiledLayer(tileCache);
            this._map.Basemap = new Basemap(tileLayer);


            //울산행정구역표시
            ///ShowShapeLayer(mapView, "BML_GADM_AS", true );


            //맵뷰 클릭이벤트 설정
            //mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;

            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
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







        //맵뷰 클릭이벤트 핸들러 - 상세정보팝업 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {

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
            //팝업열기 & 위치
            popFct.IsOpen = false;

            popFct = new FTR_POP(fTR_CDE, fTR_IDN);

            //double y0 = e.Position.Y - 700; //위쪽으로 표시함
            //if (y0 < 100) y0 = 100;
            popFct.PlacementRectangle = new Rect(e.Position.X + 550, e.Position.Y - 300, 10, 10);
            popFct.IsOpen = true;
        }

        #endregion






















    }
}

