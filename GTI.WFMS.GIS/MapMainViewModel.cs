using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System.IO;
using GTI.WFMS.Models.Common;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.Data;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapMainViewModel : LayerModel, INotifyPropertyChanged
    {
        private bool tg = false;

        public MapMainViewModel()
        {
            //뷰객체를 파라미터로 전달받기
            //loadedCmd = new RelayCommand<object>(loadedMethod);
            /*
             */
            loadedCmd = new RelayCommand<object>(delegate(object obj) {

                //뷰객체를 파라미터로 전달받기
                Grid divGrid = obj as Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayerInfo = divGrid.FindName("divLayerInfo") as Popup;

                //지도초기화
                InitMap();
            });


            //레이어 ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj) {
                Button doc = obj as Button;
                //IEnumerable<CheckBox> collection = doc.Children.OfType<CheckBox>();
                //CheckBox chkbox = collection.First();
                
                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked ;
                
                ShowShapeLayer(_map, doc.Tag.ToString(), chk);
            });

            //팝업레이어 토글처리
            toggleCmd = new RelayCommand<object>(delegate (object obj) {
                Popup divLayer = obj as Popup;
                StackPanel spLayer = divLayer.FindName("spLayer") as StackPanel;
                DockPanel docDiv = divLayer.FindName("docDiv") as DockPanel;
                
                spLayer.Visibility = spLayer.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                if(spLayer.Visibility == Visibility.Collapsed)
                {
                    divLayer.Height = docDiv.Height;
                }
                else
                {
                    divLayer.Height = docDiv.Height + spLayer.Height;
                }
            });

            //팝업레이어 토글처리
            closeCmd = new RelayCommand<object>(delegate (object obj)
            {
                Popup divLayerInfo = obj as Popup;

                divLayerInfo.IsOpen = false;
            });


            // 레이어스타일 Renderer 초기화
            InitUniqueValueRenderer();

            // 울산행정구역 표시
            ShowShapeLayer(_map, "BML_GADM_AS", true);
        }



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
        

        public RelayCommand<object> btnCmd { get; set; }

        public FctDtl FctDtl
        {
            get { return this.fctDtl; }
            set { this.fctDtl = value; }
        }


        #endregion




        #region ========== Members 정의 ==========

        public event PropertyChangedEventHandler PropertyChanged;


        private Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };

        private MapView mapView = new MapView(); //뷰의 MapView

        // Coordinates for Ulsan
        private MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        private double _ulsanScale = 150000;

        private FctDtl fctDtl = new FctDtl(); //시설물기본정보
        private Popup divLayerInfo = new Popup(); //시설물기본정보팝업

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


















        #region ============= 내부함수 ============= 

        private async void InitMap()
        {
            //지도위치 및 스케일 초기화
            await mapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);

            //Base맵 초기화
            this._map.Basemap = Basemap.CreateOpenStreetMap();
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);


            //맵뷰 클릭이벤트 설정
            mapView.GeoViewTapped += handlerGeoViewTapped;
        }



        private void loadedMethod(object obj)
        {
            //뷰객체를 파라미터로 전달받기
            this.mapView = obj as MapView;

            //지도초기화
            InitMap();
        }



        //맵뷰 클릭이벤트 핸들러
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            //팝업열기 & 위치
            divLayerInfo.PlacementRectangle = new Rect(e.Position.X, e.Position.Y, 250, 300);
            divLayerInfo.IsOpen = true; 


            // Perform the identify operation.
            IdentifyLayerResult IR_WTL_MANH_PS = await mapView.IdentifyLayerAsync(layers["WTL_MANH_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_STPI_PS = await mapView.IdentifyLayerAsync(layers["WTL_STPI_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_HEAD_PS = await mapView.IdentifyLayerAsync(layers["WTL_HEAD_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_GAIN_PS = await mapView.IdentifyLayerAsync(layers["WTL_GAIN_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_SERV_PS = await mapView.IdentifyLayerAsync(layers["WTL_SERV_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_FLOW_PS = await mapView.IdentifyLayerAsync(layers["WTL_FLOW_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_FIRE_PS = await mapView.IdentifyLayerAsync(layers["WTL_FIRE_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_VALV_PS = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS"], e.Position, 20, false);
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
            }
            else if (IR_WTL_MANH_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_MANH_PS.GeoElements[0];
            }
            else if (IR_WTL_HEAD_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_HEAD_PS.GeoElements[0];
            }
            else if (IR_WTL_GAIN_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_GAIN_PS.GeoElements[0];
            }
            else if (IR_WTL_SERV_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_SERV_PS.GeoElements[0];
            }
            else if (IR_WTL_FLOW_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_FLOW_PS.GeoElements[0];
            }
            else if (IR_WTL_FIRE_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_FIRE_PS.GeoElements[0];
            }
            else if (IR_WTL_VALV_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_VALV_PS.GeoElements[0];
            }
            else if (IR_WTL_RSRV_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_RSRV_PS.GeoElements[0];
            }
            else if (IR_WTL_PRGA_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_PRGA_PS.GeoElements[0];
            }
            else if (IR_WTL_META_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_META_PS.GeoElements[0];
            }
            else if (IR_WTL_LEAK_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_LEAK_PS.GeoElements[0];
            }
            else if (IR_WTL_PIPE_LM.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_PIPE_LM.GeoElements[0];
            }
            else if (IR_WTL_SPLY_LS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_SPLY_LS.GeoElements[0];
            }
            else if (IR_WTL_PURI_AS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_PURI_AS.GeoElements[0];
            }
            else
            {
                // Return if there's nothing to show.
                return;
            }


            

            //선택된 레이어에서 속성정보 가져오기
            //& 레이어정보 세팅
            FctDtl.FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
            FctDtl.FTR_NAM = BizUtil.GetCodeNm("Select_FTR_NM", FctDtl.FTR_CDE);
            FctDtl.FTR_IDN = Int32.Parse(identifiedFeature.GetAttributeValue("FTR_IDN").ToString());
            FctDtl.SHT_NUM = identifiedFeature.GetAttributeValue("SHT_NUM").ToString();
            FctDtl.HJD_CDE = identifiedFeature.GetAttributeValue("HJD_CDE").ToString();
            FctDtl.HJD_NAM = BizUtil.GetCodeNm("Select_ADAR_NM", FctDtl.HJD_CDE);
            FctDtl.MNG_CDE = identifiedFeature.GetAttributeValue("MNG_CDE").ToString();
            FctDtl.MNG_NAM = BizUtil.GetCdNm("250101", FctDtl.MNG_CDE);//관리기관

            FctDtl.IST_YMD = identifiedFeature.GetAttributeValue("IST_YMD").ToString();
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



        #endregion

    }
}

