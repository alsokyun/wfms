using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using GTI.WFMS.Models.Common;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Data;
using System.Linq;
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
using System.Data;
using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;
using System.Windows.Controls;
using Esri.ArcGISRuntime;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class EditWinViewModel : INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========
        public Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };

        public MapView mapView; //뷰의 MapView


        /* 
         * 레이어객체리스트
        public FeatureLayer BML_GADM_AS = new FeatureLayer();//광역행정구역
        public FeatureLayer WTL_FLOW_PS = new FeatureLayer();
        public FeatureLayer WTL_FIRE_PS = new FeatureLayer();
         */



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

        public RelayCommand<object> chkCmd { get; set; }



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



        private void OnGeometryChanged(object sender, GeometryChangedEventArgs e)
        {
            if (e.NewGeometry.Dimension == GeometryDimension.Area)
            {
                //mapView.SketchEditor.CompleteCommand.Execute(null);
            }
        }

        private void OnSelectedVertexChanged(object sender, VertexChangedEventArgs e)
        {
            if (e.NewVertex.PointIndex == 5)
            {

            }
        }


        #region ========== 생성자 ==========

        public EditWinViewModel()
        {
            //string licenseKey = "runtimelite,1000,rud1244207246,none,9TJC7XLS1MJPF5KHT033"; //그린텍
            //string licenseKey = "runtimelite,1000,rud9177830334,none,A3E60RFLTFM5NERL1040"; //kyun0828 free 

            //ArcGISRuntimeEnvironment.SetLicense(licenseKey);



            LoadedCommand = new RelayCommand<object>(delegate (object obj)
            {

                // 0.뷰객체를 파라미터로 전달받기
                editWinView = obj as EditWinView;
                this.mapView = editWinView.FindName("mapView") as MapView;

                // 1.지도초기화
                InitMap();

                //레이어초기화
                CmmRun.initLayers();

                //렌더러초기생성작업
                CmmRun.InitUniqueValueRenderer();

                // 2.화면 및 콤보초기화
                cbFTR_CDE = editWinView.cbFTR_CDE;
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", "선택");
                cbFTR_CDE.EditValueChanged += cbFTR_CDEHandler; //콤보변경핸들러

                //비트맵초기화(시설물상세DIV 아이콘)
                BitImg = new BitmapImage();

                mapView.SketchEditor.GeometryChanged += OnGeometryChanged;
                mapView.SketchEditor.SelectedVertexChanged += OnSelectedVertexChanged;

            });



            //시설물조회 후 해당레이어 표시
            SearchCommand = new RelayCommand<object>(delegate(object obj) {

                if (FmsUtil.IsNull(FTR_CDE))
                {
                    Messages.ShowInfoMsgBox("시설물을 선택하세요.");
                    return;
                }

                //기존항목 초기화
                ResetLayer();

                //기존페이지 초기화
                InitPage(cbFTR_CDE.EditValue.ToString(), null, null);

                SearchLayer(obj);
            });




            //아이콘변경 (파일찾기)
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
                            CmmRun.InitUniqueValueRenderer();

                            //2.레이어의 렌더러 재세팅
                            foreach (string sel in _selectedLayerNms)
                            {
                                CmmRun.layers[sel].Renderer = CmmRun.uniqueValueRenderer.Clone();
                                CmmRun.layers[sel].RetryLoadAsync();
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



                //Polygon 피처인 경우 - SketchEditor 를 GraphicOverlay에 생성한다
                //라인피처인 경우 - SketchEditor 를 GraphicOverlay에 생성한다
                if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS") || _selectedLayerNm.Equals("WTL_PURI_AS")
                    || _selectedLayerNm.Equals("WTL_LBLK_AS") || _selectedLayerNm.Equals("WTL_MBLK_AS") || _selectedLayerNm.Equals("WTL_SBLK_AS"))
                {
                    try
                    {
                        SketchCreationMode creationMode = SketchCreationMode.Polyline;
                        Symbol symbol;
                        if (_selectedLayerNm.Equals("WTL_PURI_AS") || _selectedLayerNm.Equals("WTL_LBLK_AS") || _selectedLayerNm.Equals("WTL_MBLK_AS") || _selectedLayerNm.Equals("WTL_SBLK_AS"))
                        {
                            creationMode = SketchCreationMode.Polygon;
                            symbol = new SimpleFillSymbol() { Color = System.Drawing.Color.SkyBlue, Style = SimpleFillSymbolStyle.Solid };
                        }
                        else if (_selectedLayerNm.Equals("WTL_PIPE_LM"))
                        {
                            creationMode = SketchCreationMode.Polyline;
                            symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                        }
                        else
                        {
                            creationMode = SketchCreationMode.Polyline;
                            symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.SkyBlue, 2);
                        }

                        MessageBox.Show("시설물을 추가할 지점을 마우스로 클릭하세요.(편집완료는 더블클릭)");

                        // Let the user draw on the map view using the chosen sketch mode
                        Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(creationMode, false); //맵에 신규geometry 얻어오기


                        //화면에 그래픽표시(참고용)
                        Graphic graphic = new Graphic(geometry, symbol);
                        _sketchOverlay.Graphics.Add(graphic);



                        // 0.화면초기화
                        editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
                        NEW_FTR_IDN = "";

                        //선택된레이어 해제
                        _selectedFeature = null;
                        try
                        {
                            CmmRun.layers[_selectedLayerNm].ClearSelection();
                        }
                        catch (Exception) { }

                        //신규시설물 페이지전환
                        InitPage(cbFTR_CDE.EditValue.ToString(), FTR_CDE, null);


                        // 1.레이어에 위치추가
                        AddFeatureToLayer(geometry);


                        // 2.시설물DB 저장
                        Messages.ShowInfoMsgBox("위치정보가 추가되었습니다. 해당시설물의 상세정보를 저장하세요");

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
                    editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
                    NEW_FTR_IDN = "";

                    //선택된레이어 해제
                    _selectedFeature = null;
                    try
                    {
                        CmmRun.layers[_selectedLayerNm].ClearSelection();
                    }
                    catch (Exception) { }

                    ////신규시설물 페이지전환
                    InitPage(cbFTR_CDE.EditValue.ToString(), FTR_CDE, null);
                    

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


            DelCmd = new RelayCommand<object>(OnDelCmd);


            //레이어 ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;

                //레이어표시 - FTR_IDN 조건 필터링없음
                ShowShapeLayerFilter(mapView, doc.Tag.ToString(), chk, null);

                //조회된 피처 자동선택
                string ftr_cde = doc.Tag.ToString() == "WTL_PIPE_LM" ? "SA001" : "SA002";
                SelectFct(ftr_cde, "", CmmRun.layers[doc.Tag.ToString()]);
            });


        }

        //위치삭제처리
        public async void OnDelCmd(object obj)
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
                //_selectedFeature.Refresh();   // Refresh하면 SHP파일이 정상적으로 저장되지않음!!!

                MessageBox.Show("삭제되었습니다.");

                // 페이지초기화
                InitPage(cbFTR_CDE.EditValue.ToString(), null, null);
            }
            else
            {
                await _selectedFeature.FeatureTable.AddFeatureAsync(back_selectedFeature);
                _selectedFeature.Refresh();
            }
        }



        #endregion






        #region ============= 내부함수 ============= 

        private async void InitMap()
        {
            //지도위치 및 스케일 초기화
            await mapView.SetViewpointCenterAsync(GisCmm.fmsCoords, GisCmm.fmsScale);

            //Base맵 초기화
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);
            //this._map.Basemap = Basemap.CreateOpenStreetMap();

            //타일맵
            //TileCache tileCache = new TileCache(BizUtil.GetDataFolder("tile", "korea.tpk"));
            string tile_paht = Path.Combine(FmsUtil.mdxDir , "korea.tpk") ;   
            TileCache tileCache = new TileCache(tile_paht);
            ArcGISTiledLayer tileLayer = new ArcGISTiledLayer(tileCache);
            this.Map.Basemap = new Basemap(tileLayer);



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
            ResetLayer();

            editWinView.txtFTR_IDN.EditValue = "";

            //시설물레이어 초기화
            _selectedLayerNm = "";
            _selectedLayerNms.Clear();



            // 0,상수관로,급수관로 체크되어있으면 다시시표시
            foreach (Button btn in FmsUtil.FindVisualChildren<Button>(editWinView))
            {
                try
                {
                    CheckBox chkbox = btn.Template.FindName("chkLayer", btn) as CheckBox;
                    if (chkbox.IsChecked is true)
                    {
                        ShowShapeLayerFilter(mapView, btn.Tag.ToString(), true, "");
                    }
                }
                catch (Exception) { }
            }





            // 1.새로운 레이어 시작
            string ftr_cde = e.NewValue.ToString();
            this.FTR_CDE = ftr_cde;

            if (!FmsUtil.IsNull(ftr_cde))
            {
                //선택된 레이어 기록
                _selectedLayerNm = GisCmm.GetLayerNm(ftr_cde);
                _selectedLayerNms.Add(GisCmm.GetLayerNm(ftr_cde));
            }
            else
            {
                return;
            }

            // 2.선택된 레이어의 시설물 페이지로  초기화
            InitPage(ftr_cde, null, null);

            // 시설물검색 
            if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS"))
            {
                //대용량데이터는 자동검색 제외
                SearchLayer(null); 
            }
            else
            {
                SearchLayer(null); 
            }

        }


        // 화면리셋
        private void ResetLayer()
        {
            // 화면초기화
            BitImg = new BitmapImage();
            NEW_FTR_IDN = "";


            // 맵초기화 - 맵초기화하면 tap 이벤트핸들러 사라져버림
            //InitMap();

            //레이어 클리어
            mapView.Map.OperationalLayers.Clear();

            //선택된레이어 해제
            _selectedFeature = null;
            try
            {
                CmmRun.layers[_selectedLayerNm].ClearSelection();
            }
            catch (Exception) { }

            //그래픽스 클리어
            //_sketchOverlay.ClearSelection();
            _sketchOverlay.Graphics.Clear();

        }


        /// <summary>
        /// UserControl 시설물페이지 로딩
        /// </summary>
        /// <param name="CBO_FTR_CDE"></param>
        /// <param name="_FTR_CDE"></param>
        /// <param name="_FTR_IDN"></param>
        private void InitPage(string CBO_FTR_CDE, string _FTR_CDE, string _FTR_IDN)
        {
            editWinView.divBtn.Visibility = Visibility.Collapsed; //이동버튼 숨김

            switch (CBO_FTR_CDE)
            {
                case "SA001": //상수관로
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PIPE_LM(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_PIPE_LM)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PIPE_LM(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA002": //급수관로
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_SPLY_LS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_SPLY_LS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_SPLY_LS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA003": //스탠파이프
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_STPI_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_STPI_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_STPI_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA100": //상수맨홀
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_MANH_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_MANH_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_MANH_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA110": //수원지
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_HEAD_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_HEAD_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_HEAD_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA112": //취수장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_GAIN_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_GAIN_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_GAIN_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;


                case "SA113": //정수장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PURI_AS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_PURI_AS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PURI_AS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA114": //배수지
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_SERV_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_SERV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_SERV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;



                case "SA117": //유량계
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_FLOW_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA118": case "SA119": //급수탑,소화전
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_FIRE_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_FIRE_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_FIRE_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA120": //저수조
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_RSRV_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_RSRV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_RSRV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA121": //수압계
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PRGA_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_PRGA_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PRGA_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA122": //급수전계량기
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_META_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_META_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_META_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA200": case "SA201": case "SA202": case "SA203": case "SA204": case "SA205":
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_VALV_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_VALV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_VALV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA206": //가압펌프장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PRES_PS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_PRES_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PRES_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "BZ001": //대블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_BLKL_AS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_BLKL_AS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_BLKL_AS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;
                case "BZ002": //중블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_BLKM_AS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_BLKM_AS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_BLKM_AS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;
                case "BZ003": //소블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_BLKS_AS(_FTR_CDE);//신규페이지
                        NEW_FTR_IDN = ((UC_BLKS_AS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_BLKS_AS(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;


                default:
                    editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);
                    break;
            }

            editWinView.divBtn.Visibility = Visibility.Visible; //이동버튼 보임

        }



        //시설물 shape 검색
        private void SearchLayer(object obj)
        {
            string ftr_idn = editWinView.txtFTR_IDN.Text;

            //레이어표시 - FTR_IDN 조건 필터링
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, ftr_idn);

            //조회된 피처 자동선택
            SelectFct(FTR_CDE,  ftr_idn, CmmRun.layers[_selectedLayerNm]);

        }




        #endregion













            
        // 관리번호로 해당Feature 객체찾기
        public async void SelectFct(string _FTR_CDE, string _FTR_IDN, FeatureLayer _featureLayer)
        {
            // 0.Feature 테이블 가져오기
            //FeatureLayer __featureLayer = _featureLayer.Clone() as FeatureLayer;
            FeatureTable _featureTable = _featureLayer.FeatureTable;



            // Create a query parameters that will be used to Query the feature table.
            QueryParameters queryParams = new QueryParameters();


            // Construct and assign the where clause that will be used to query the feature table.
            queryParams.WhereClause = " FTR_CDE = '" + _FTR_CDE + "' ORDER BY FTR_IDN DESC";
            if (!FmsUtil.IsNull(_FTR_IDN))
            {
                queryParams.WhereClause = " FTR_CDE = '" + _FTR_CDE + "' AND FTR_IDN like '%' ||  " + _FTR_IDN + " || '%'";
            }


            List<Feature> features;
            try
            {
                // Query the feature table.
                FeatureQueryResult queryResult = await _featureTable.QueryFeaturesAsync(queryParams);

                // Cast the QueryResult to a List so the results can be interrogated.
                features = queryResult.ToList();
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox(e.Message);
                return;
            }

            if (features.Any())
            {
                // Create an envelope.
                EnvelopeBuilder envBuilder = new EnvelopeBuilder(SpatialReferences.WebMercator);



                if (features.Count == 1)
                {
                    //한건인 경우 선택처리
                    ShowFctPage(features[0]); 
                }
                else
                {
                    //피쳐영역 Extent 위치이동
                    foreach (Feature feature in features)
                    {
                        envBuilder.UnionOf(feature.Geometry.Extent); //복수의 피처영역 합치기
                    }
                    // Zoom to the extent of the selected feature(s).
                    await mapView.SetViewpointGeometryAsync(envBuilder.ToGeometry(), 50);
                }
            }
        }



        //맵뷰 Feature선택 처리 - 상세정보팝업 
        public async void ShowFctPage(Feature identifiedFeature)
        {
            try
            {
                // 0.기존선택해제
                if (_selectedFeature != null)
                {
                    try
                    {
                        // Reset the selection.
                        CmmRun.layers[_selectedLayerNm].ClearSelection();
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
                    return ;
                }
                //else if (_FTR_IDN == this.FTR_IDN)
                //{
                //    //기존시설물이면 토글처리（선택처리하지 않음）
                //    //this.FTR_CDE= "";
                //    this.FTR_IDN = null;
                //}
                else
                {
                    //시설물선택활성화 처리
                    CmmRun.layers[_selectedLayerNm].SelectFeature(identifiedFeature);
                    //해당피처로 이동
                    await mapView.SetViewpointCenterAsync(identifiedFeature.Geometry.Extent.GetCenter(), 40000);

                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl 시설물정보 재로딩
                InitPage(cbFTR_CDE.EditValue.ToString(), this.FTR_CDE, this.FTR_IDN);


                // 아이콘 이미지소스 업데이트
                try
                {
                    BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();
                }
                catch (Exception){}

            }
            catch (Exception e)
            {
                Console.WriteLine("레이어 featrue click error..." + e.Message);
            }

        }
















        #region ========== GIS관련 처리 ==============








        // 피처추가
        public void handlerGeoViewTappedAddFeature(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Get the MapPoint from the EventArgs for the tap.
                MapPoint destinationPoint = e.Location;
                // Normalize the point - needed when the tapped location is over the international date line.
                destinationPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(destinationPoint);


                // 1.레이어에 위치추가
                AddFeatureToLayer(destinationPoint);


                //MessageBox.Show("Added feature ", "Success!");

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

        /// <summary>
        /// 레이어에 Feature(MapPoint, PolyLine, Polygon)를 추가
        /// </summary>
        /// <param name="geometry"></param>
        private async void AddFeatureToLayer(Geometry geometry)
        {
            FeatureTable layerTable = CmmRun.layers[_selectedLayerNm].FeatureTable;
            if (layerTable is null)
            {
                layerTable = await InitLayerTable(_selectedLayerNm); 
            }

            //피처추가
            Feature _addedFeature = layerTable.CreateFeature();
            _addedFeature.Geometry = geometry;

            //속성추가
            _addedFeature.SetAttributeValue("FTR_CDE", this.FTR_CDE);
            _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToDouble(NEW_FTR_IDN));

            try
            {
                await layerTable.AddFeatureAsync(_addedFeature);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //관리번호 타입이 정수이여서 에러나면 다시 시도
                _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToInt32(NEW_FTR_IDN));
                try
                {
                    await layerTable.AddFeatureAsync(_addedFeature);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            finally
            {
                _addedFeature.Refresh();
            }

            //위치정보 WKT 만들기
            if (geometry is Polyline)
            {
                Polyline line = geometry as Polyline;
                GisCmm.WKT_LINE += "LINESTRING(";
                foreach (var p in line.Parts[0].Points)
                {
                    GisCmm.WKT_LINE += (p.X + " " + p.Y + ",");
                }
                GisCmm.WKT_LINE = GisCmm.WKT_LINE.Substring(0,GisCmm.WKT_LINE.Length-1) + ")";
            }
            else if (geometry is Polygon)
            {
                Polygon gon = geometry as Polygon;
                GisCmm.WKT_POLYGON += "MULTIPOLYGON(((";
                foreach (var p in gon.Parts[0].Points)
                {
                    GisCmm.WKT_POLYGON += (p.X + " " + p.Y + ",");
                }
                //첫포인트로 폴리곤 마무리
                foreach (var p in gon.Parts[0].Points)
                {
                    GisCmm.WKT_POLYGON += (p.X + " " + p.Y + ",");
                    break;
                }

                GisCmm.WKT_POLYGON = GisCmm.WKT_POLYGON.Substring(0, GisCmm.WKT_POLYGON.Length - 1) + ")))";
            }
            else if (geometry is MapPoint )
            {
                MapPoint point = geometry as MapPoint;
                GisCmm.WKT_POINT = "POINT("+ point.X + " " + point.Y + ")";
            }
            
            
            //추가내용 새로고침
            _addedFeature.Refresh();
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
                        //await _selectedFeature.FeatureTable.UpdateFeatureAsync(back_selectedFeature);
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
                IdentifyLayerResult IR_SEL = await mapView.IdentifyLayerAsync(CmmRun.layers[_selectedLayerNm], e.Position, 5, false);

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
                        CmmRun.layers[_selectedLayerNm].ClearSelection();
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
                //else if (_FTR_IDN == this.FTR_IDN)
                //{
                //    //기존시설물이면 토글처리（선택처리하지 않음）
                //    //this.FTR_CDE= "";
                //    this.FTR_IDN = null;
                //}
                else
                {
                    //시설물선택활성화 처리
                    CmmRun.layers[_selectedLayerNm].SelectFeature(identifiedFeature); 
                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl 시설물정보 재로딩
                InitPage(cbFTR_CDE.EditValue.ToString(), this.FTR_CDE, this.FTR_IDN);


                // 아이콘 이미지소스 업데이트
                BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();




            }
            catch (Exception)
            {
                Console.WriteLine("레이어 featrue click error...");
            }

        }







        #endregion












        #region =========== shape 레이어 구성부분 ==============
        public async Task<ShapefileFeatureTable> InitLayerTable(string _layerNm)
        {
            string shapeNm = "";
            try
            {
                string[] ary = _layerNm.Split('^');
                shapeNm = ary[0]; //레이어테이블명


                string shapefilePath = Path.Combine(BizUtil.GetDataFolder("shape", shapeNm + ".shp"));
                ShapefileFeatureTable layerTable = await ShapefileFeatureTable.OpenAsync(shapefilePath);
                return layerTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Shape 레이어 보이기/끄기 - Shape버전
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public async void ShowShapeLayerFilter(MapView _mapView, string _layerNm, bool chk, string _FTR_IDN)
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

                //FTR_IDN 필터추가
                if (!FmsUtil.IsNull(_FTR_IDN))
                {
                    if (FmsUtil.IsNull(filterExp))
                    {
                        filterExp += "FTR_IDN LIKE '%' || " + _FTR_IDN + " ||  '%'";
                    }
                    else
                    {
                        filterExp += " AND FTR_IDN LIKE '%' ||  " + _FTR_IDN + " ||  '%'";
                    }
                }


                FeatureLayer layer = CmmRun.layers[_layerNm];
                //Type memberType = this.GetType();



                // 1.레이어 ON
                if (chk)
                {
                    // 필터링 인수있으면 하위시설물(관리번호, 전체)으로 필터
                    layer.DefinitionExpression = filterExp;


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
                            string shapefilePath = Path.Combine(BizUtil.GetDataFolder("shape", shapeNm + ".shp"));
                            try
                            {
                                ShapefileFeatureTable layerTable = await ShapefileFeatureTable.OpenAsync(shapefilePath);


                                layer = new FeatureLayer(layerTable); /////// 신규레이어 생성 /////// 
                                layer.DefinitionExpression = filterExp;// 필터링 인수있으면 하위시설물(관리번호, 전체)으로 필터

                                CmmRun.layers[_layerNm] = layer; /////// 딕셔너리에 자동으로 저장되지는 않을것임 /////// 


                                layer.Renderer = CmmRun.uniqueValueRenderer.Clone(); //렌더러는 레이어 각각 할당해야하므로 렌더러복사하여 할당
                                _mapView.Map.OperationalLayers.Add(layer);

                            }
                            catch (Exception e)
                            {
                                Messages.ShowErrMsgBox(e.Message);
                            }
                        }
                    }

                    // Zoom the map to the extent of the shapefile.
                    //await _mapView.SetViewpointGeometryAsync(layer.FullExtent, 50);

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
                    layer.DefinitionExpression = "";

                }




            }
            catch (Exception)
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }







        #endregion























    }
}

