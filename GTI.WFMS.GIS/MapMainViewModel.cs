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

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapMainViewModel : LayerModel, INotifyPropertyChanged
    {
        private bool tg = false;
        private bool tg1 = false;
        private bool tg2 = false;

        public MapMainViewModel()
        {
            //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
            //loadedCmd = new RelayCommand<object>(loadedMethod);
            /*
             */
            loadedCmd = new RelayCommand<object>(delegate(object obj) {

                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                this.mapView = obj as MapView;

                //�����ʱ�ȭ
                InitMap();
            });

            btnCmd = new RelayCommand<object>(delegate (object obj) {
                Button btn = obj as Button;
                tg = !tg;
                ShowShapeLayer(_map, btn.Name, tg);
            });
            btn1Cmd = new RelayCommand<object>(delegate (object obj) {
                tg1 = !tg1;
                ShowShapeLayer(_map, "WTL_FLOW_PS", tg1);
            });
            btn2Cmd = new RelayCommand<object>(delegate (object obj) {
                tg2 = !tg2;
                ShowShapeLayer(_map, "WTL_FIRE_PS", tg2);
            });


            // ���̾Ÿ�� Renderer �ʱ�ȭ
            InitUniqueValueRenderer();

            // ����������� ǥ��
            //AddLayer(_map);
        }



        #region ==========  Properties ���� ==========

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

        public RelayCommand<object> loadedCmd { get; set; } //Loaded�̺�Ʈ���� ICommand ����Ͽ� �䰴ü ���޹���
        public RelayCommand<object> btnCmd { get; set; } 
        public RelayCommand<object> btn1Cmd { get; set; } 
        public RelayCommand<object> btn2Cmd { get; set; } 
        
        #endregion




        #region ========== Members ���� ==========

        public event PropertyChangedEventHandler PropertyChanged;


        private Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };

        private MapView mapView = new MapView(); //���� MapView

        // Coordinates for Ulsan
        private MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        private double _ulsanScale = 150000;

        #endregion



        #region ========== �������̽� �������̵� ==========

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


















        #region ============= �����Լ� ============= 

        private async void InitMap()
        {
            //������ġ �� ������ �ʱ�ȭ
            await mapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);

            //Base�� �ʱ�ȭ
            this._map.Basemap = Basemap.CreateOpenStreetMap();
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);


            //�ʺ� Ŭ���̺�Ʈ ����
            mapView.GeoViewTapped += handlerGeoViewTapped;
        }



        private void loadedMethod(object obj)
        {
            //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
            this.mapView = obj as MapView;

            //�����ʱ�ȭ
            InitMap();
        }



        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            // Perform the identify operation.
            IdentifyLayerResult IdentifyResultL_FLOW_PS = await mapView.IdentifyLayerAsync(layers["WTL_FLOW_PS"], e.Position, 20, false);
            IdentifyLayerResult IdentifyResultL_FIRE_PS = await mapView.IdentifyLayerAsync(layers["WTL_FIRE_PS"], e.Position, 20, false);
            IdentifyLayerResult IdentifyResultL_HADM_AS = await mapView.IdentifyLayerAsync(layers["BML_GADM_AS"], e.Position, 20, false);

            Feature identifiedFeature; //�̺�Ʈ Ÿ�ٷ��̾�


            if (IdentifyResultL_FLOW_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IdentifyResultL_FLOW_PS.GeoElements[0];
            }
            else if (IdentifyResultL_FIRE_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IdentifyResultL_FIRE_PS.GeoElements[0];
            }
            else if (IdentifyResultL_HADM_AS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IdentifyResultL_HADM_AS.GeoElements[0];
            }
            else
            {
                // Return if there's nothing to show.
                return;
            }


            string FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
            Console.WriteLine("FTR_CDE - " + FTR_CDE);

        }



        #endregion

    }
}

