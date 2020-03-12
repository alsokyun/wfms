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
using System.Windows;
using GTI.WFMS.Models.Cmm.Model;
using GTIFramework.Common.MessageBox;
using System.IO;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.Symbology;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using GTI.WFMS.GIS.Pop.View;
using System.Collections;
using System.Data;
using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class EditWinViewModel : LyrModel, INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========

        // 선택한 피처 - 서버연계된Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();
        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;


        public event PropertyChangedEventHandler PropertyChanged;

        EditWinView editWinView;
        ComboBoxEdit cbFTR_CDE;
        string NEW_FTR_IDN = "";//신규추가채번된 시설물의 관리번호 
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

        public RelayCommand<object> LoadedCommand { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> SearchCommand { get; set; }
        
        public RelayCommand<object> AddCmd { get; set; }
        public RelayCommand<object> EditCmd { get; set; }
        public RelayCommand<object> DelCmd { get; set; }
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
        private string fTR_CDE;
        public string FTR_CDE
        {
            get { return fTR_CDE; }
            set { fTR_CDE = value; OnPropertyChanged("FTR_CDE"); }
        }
        private string fTR_IDN;
        public string FTR_IDN
        {
            get { return fTR_IDN; }
            set { fTR_IDN = value; OnPropertyChanged("FTR_IDN");}
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

        public EditWinViewModel()
        {
            LoadedCommand = new RelayCommand<object>(delegate (object obj)
            {

                // 0.뷰객체를 파라미터로 전달받기
                editWinView = obj as EditWinView;
                this.mapView = editWinView.FindName("mapView") as MapView;

                // 1.지도초기화
                InitMap();
                //렌더러초기생성작업
                GisCmm.InitUniqueValueRenderer();

                // 2.화면 및 콤보초기화
                cbFTR_CDE = editWinView.cbFTR_CDE;
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", true);
                cbFTR_CDE.EditValueChanged += cbFTR_CDEHandler; //콤보변경핸들러

                //비트맵초기화(시설물상세DIV 아이콘)
                BitImg = new BitmapImage();



            });



            //시설물조회 후 해당레이어 표시
            SearchCommand = new RelayCommand<object>(SearchAction);




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
                            fi.CopyTo(Path.Combine(icon_foler, _FTR_CDE), true);
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox(ex.Message);
                        }
                        finally
                        {
                            //1.렌더러 재구성
                            GisCmm.InitUniqueValueRenderer();

                            //2.레이어의 렌더러 재세팅
                            foreach (string sel in _selectedLayerNms)
                            {
                                layers[sel].Renderer = GisCmm.uniqueValueRenderer.Clone();
                                layers[sel].RetryLoadAsync();
                            }

                            //3.팝업이미지소스 업데이트
                            BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();

                        }
                    }
                }
            });



            AddCmd = new RelayCommand<object>(async delegate (object obj)
            {


                if (_selectedLayerNms.Count < 1)
                {
                    MessageBox.Show("시설물을 선택하세요.");
                    return;
                }
                else if (_selectedLayerNms.Count > 1)
                {
                    MessageBox.Show("시설물을 하나만 선택하세요.");
                    return;
                }


                //라인피처인 경우 - SketchEditor 를 GraphicOverlay에 생성한다
                if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS"))
                {
                    try
                    {
                        // Let the user draw on the map view using the chosen sketch mode
                        Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(SketchCreationMode.Polyline, true); //맵에 신규geometry 얻어오기

                        // Create and add a graphic from the geometry the user drew
                        SimpleLineSymbol symbol;
                        if (_selectedLayerNm.Equals("WTL_PIPE_LM"))
                        {
                            symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                        }
                        else
                        {
                            symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                        }

                        Graphic graphic = new Graphic(geometry, symbol);
                        _sketchOverlay.Graphics.Add(graphic);

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
                    ////화면초기화
                    InitModel();

                    ////신규시설물 페이지전환
                    editWinView.cctl.Content = new UC_FLOW_PS(FTR_CDE);
                    NEW_FTR_IDN = ((UC_FLOW_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();

                    //추가처리 탭핸들러 추가
                    mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                    mapView.GeoViewTapped -= handlerGeoViewTapped;
                    mapView.GeoViewTapped += handlerGeoViewTappedAddFeature;
                    MessageBox.Show("시설물을 추가할 지점을 마우스로 클릭하세요.");
                }

            });


            EditCmd = new RelayCommand<object>(delegate (object obj)
            {
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

            });


            DelCmd = new RelayCommand<object>(async delegate (object obj)
            {
                if (_selectedFeature == null)
                {
                    MessageBox.Show("시설물을 선택하세요.");
                    return;
                }

                //삭제처리

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
            });

        }

        #endregion






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



            //맵뷰 클릭이벤트 설정
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;


            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
        }





        // 시설물코드 변경
        private void cbFTR_CDEHandler(object sender, EditValueChangedEventArgs e)
        {
            // 0.편집화면초기화
            InitModel();
            //시설물레이어 초기화
            _selectedLayerNm = "";
            _selectedLayerNms.Clear();

            // 1.새로운 레이어 시작
            string ftr_cde = e.NewValue.ToString();
            this.FTR_CDE = ftr_cde;

            if (!FmsUtil.IsNull(ftr_cde))
            {
                //선택된 레이어 기록
                _selectedLayerNm = GisCmm.GetLayerNm(ftr_cde);
                _selectedLayerNms.Add(GisCmm.GetLayerNm(ftr_cde));
            }

            // 2.선택된 레이어의 시설물 페이지로  초기화
            switch (ftr_cde)
            {
                case "SA117":
                    editWinView.cctl.Content = new UC_FLOW_PS();
                    break;
                default:
                    editWinView.cctl.Content = null;
                    break;
            }

            // 시설물검색
            SearchAction(null);

        }


        // 화면리셋
        private void InitModel()
        {
            // 화면초기화
            BitImg = new BitmapImage();
            editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
            NEW_FTR_IDN = "";


            // 맵초기화 - 맵초기화하면 tap 이벤트핸들러 사라져버림
            //InitMap();

            //레이어 클리어
            mapView.Map.OperationalLayers.Clear();

            //선택된레이어 해제
            _selectedFeature = null;
            try
            {
                layers[_selectedLayerNm].ClearSelection();
            }
            catch (Exception) { }
        }


        //시설물 shape 검색
        private void SearchAction(object obj)
        {
            if (FmsUtil.IsNull(FTR_CDE))
            {
                Messages.ShowInfoMsgBox("시설물을 선택하세요.");
                return;
            }

            //기존항목 삭제
            editWinView.cctl.Content = null;

            string ftr_idn = editWinView.txtFTR_IDN.Text;

            //레이어표시 - FTR_IDN 조건 필터링
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, ftr_idn);
        }

        #endregion
















        #region ========== GIS관련 처리 ==============








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

                _addedFeature.SetAttributeValue("FTR_CDE", this.FTR_CDE);
                _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToDouble(NEW_FTR_IDN));




                await layerTable.AddFeatureAsync(_addedFeature);
                //추가내용 새로고침
                _addedFeature.Refresh();

                MessageBox.Show("Added feature ", "Success!");

                //이벤트핸들러원복
                mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                mapView.GeoViewTapped -= handlerGeoViewTapped;
                mapView.GeoViewTapped += handlerGeoViewTapped;

                /*
                 * 시설물DB 저장
                 */
                Messages.ShowInfoMsgBox("위치정보가 추가되었습니다. 해당시설물의 상세정보를 저장하세요");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
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




                // 0.기존선택해제
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


                // 1.선택처리
                _selectedFeature = (Feature)identifiedFeature; //선택피처 ArcGISFeature 변환저장
                //_selectedFeature = (ArcGISFeature)identifiedFeature; //선택피처 ArcGISFeature 변환저장

                // 선택된 레이어에서 속성정보 가져오기
                string _FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                string _FTR_IDN = identifiedFeature.GetAttributeValue("FTR_IDN").ToString();

                // DB시설물조회 후 해당시설물정보 UserControl로 전환
                if (FmsUtil.IsNull(_FTR_CDE) || FmsUtil.IsNull(_FTR_IDN))
                {
                    Messages.ShowErrMsgBox("시설물 DB정보가 없습니다.");
                    return;
                }
                else if (_FTR_IDN == this.FTR_IDN)
                {
                    //기존시설물이면 토글처리（선택처리하지 않음）
                    //this.FTR_CDE= "";
                    this.FTR_IDN = null;
                }
                else
                {
                    //시설물선택활성화 처리
                    layers[_selectedLayerNm].SelectFeature(identifiedFeature); 
                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl 시설물정보 재로딩
                switch (cbFTR_CDE.EditValue)
                {
                    case "SA117":
                        editWinView.cctl.Content = new UC_FLOW_PS(this.FTR_CDE, this.FTR_IDN);
                        break;
                    default:
                        editWinView.cctl.Content = new UC_FLOW_PS(this.FTR_CDE, this.FTR_IDN);
                        break;
                }


                // 아이콘 이미지소스 업데이트
                BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();




            }
            catch (Exception)
            {
                Console.WriteLine("레이어 featrue click error...");
            }

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
            //레이어표시 - FTR_IDN 조건 필터링
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, null);

            //1.해당레이어 가져오기
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];



            // Remove any previous feature selections that may have been made.
            layer.ClearSelection();

            // Begin query process.
            await QueryStateFeature(FTR_CDE, FTR_IDN, layer);

        }



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
                queryParams.WhereClause = "upper(FTR_CDE) = '" + _FTR_CDE + "' AND FTR_IDN = " + _FTR_IDN;


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


        #endregion































    }
}

