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


        #region ========== Members ���� ==========

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public MapView mapView; //���� MapView


        // ������ ��ó - ���������Feature  
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

        //private FctDtl fctDtl = new FctDtl(); //�ü����⺻����
        private Popup divLayer = new Popup(); //�ü������̾�DIV
        //private PopFct popFct = new PopFct(); //�ü�������DIV
        private Popup popFct = new Popup(); //�ü�������DIV
        private Button ClearButton = new Button();
        private TextBox txtFTR_CDE = new TextBox();
        private TextBox txtFTR_IDN = new TextBox();
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



        #region ==========  Properties ���� ==========

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

        public RelayCommand<object> loadedCmd { get; set; } //Loaded�̺�Ʈ���� ICommand ����Ͽ� �䰴ü ���޹���
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> toggleCmd { get; set; }
        public RelayCommand<object> closeCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }

        public RelayCommand<object> btnCmd { get; set; }
        
        public RelayCommand<object> completeCmd { get; set; }
        public RelayCommand<object> clearCmd { get; set; }
        public RelayCommand<object> findCmd { get; set; }

        public RelayCommand<object> CallPageCmd { get; set; }//�ü����˾����� �ü����޴�ȭ�� ȣ���۾�
        public RelayCommand<object> ChgImgCmd { get; set; }//�ü����˾����� ���������Ϻ����۾�


        //�ü����⺻����
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
        //�ü����̸����� �ҽ��̹���
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





        #region ========== ������ ==========

        public MapLayerViewModel()
        {
            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {

                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                System.Windows.Controls.Grid divGrid = obj as System.Windows.Controls.Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayer = divGrid.FindName("divLayer") as Popup;
                this.ClearButton = divGrid.FindName("ClearButton") as Button;

                txtFTR_CDE = divGrid.FindName("txtFTR_CDE") as TextBox;
                txtFTR_IDN = divGrid.FindName("txtFTR_IDN") as TextBox;

                //�����ʱ�ȭ
                InitMap();


                //�ü������̾�DIV �ʱ�ȭ�۾�
                InitDivLayer();

                InitUniqueValueRenderer();//�������ʱ�����۾�


                //��Ʈ���ʱ�ȭ(�ü�����DIV ������)
                BitImg = new BitmapImage();
            });


            //���̾� ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;
                //IEnumerable<CheckBox> collection = doc.Children.OfType<CheckBox>();
                //CheckBox chkbox = collection.First();

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;

                //ShowLocalServerLayer(mapView, doc.Tag.ToString(), chk);
                ShowShapeLayer(mapView, doc.Tag.ToString(), chk);


                //���õ� ���̾�����
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

            //�˾����̾� ���ó��
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

            //�˾����̾� ���ó��
            closeCmd = new RelayCommand<object>(delegate (object obj)
            {
                Popup divLayerInfo = obj as Popup;

                divLayerInfo.IsOpen = false;
            });


            // ���̾Ÿ�� Renderer �ʱ�ȭ - shape��Ŀ����� �����
            //InitUniqueValueRenderer();

            //GIS�ʱ�ȭ
            resetCmd = new RelayCommand<object>(delegate (object obj)
            {
                //0.���ʱ�ȭ
                InitMap();

                //1.���ü��� ��⵿
                //Initialize_LocalServer();

                //2.���̾� Ŭ����
                mapView.Map.OperationalLayers.Clear();


                //3.�����ִ� �ü�������â �ݱ�
                popFct.IsOpen = false;

                TreeView treeLayer = obj as TreeView;

                //���̾�div üũ����
                foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
                {
                    cb.IsChecked = false;
                }
                //���õȷ��̾� ����
                _selectedLayerNms.Clear();
                _selectedLayerNm = "";
            });

            //�ü����˾����� �ü����޴�ȭ�� ȣ���۾�
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

            //����ã���ư �̺�Ʈ
            ChgImgCmd = new RelayCommand<object>(delegate (object obj)
            {
                // ���޵� �Ķ���� 
                if (obj == null)
                {
                    Messages.ShowErrMsgBox("�ü����ڵ尡 �������� �ʽ��ϴ�.");
                    return;
                }
                string _FTR_CDE = obj as string;

                // UniqueValueRenderer �ڿ�����
                //uniqueValueRenderer = new UniqueValueRenderer();
                //layers[_selectedLayerNm].ResetRenderer();

                // ����Ž���� ����
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    //������ ���ϰ��
                    string icon_foler = Path.Combine(BizUtil.GetDataFolder(), "style_img");


                    FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //��������
                    foreach (FileInfo fi in files)
                    {
                        try
                        {

                            //�ش��̹��������� FTR_CDE ex)SA117 �̸������Ϸ� ����
                            fi.CopyTo( Path.Combine(icon_foler, _FTR_CDE), true );
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox(ex.Message);
                        }
                        finally
                        {
                            //1.������ �籸��
                            InitUniqueValueRenderer();

                            //2.���̾��� ������ �缼��
                            foreach (string sel in _selectedLayerNms)
                            {
                                layers[sel].Renderer = uniqueValueRenderer.Clone();
                                layers[sel].RetryLoadAsync();
                            }

                            //3.�˾��̹����ҽ� ������Ʈ
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
                    case "�ü�������":
                        //�ü��������˾�ȣ��



                        break;


                    default:
                        break;
                }
            });


            //����Ŭ����ó��
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
                //�߰��� ���� ����ó��

                _selectedFeature.Geometry = _geometry;
                // Apply the edit to the feature table.
                await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                _selectedFeature.Refresh();
                MessageBox.Show("Added feature ", "Success!");

            });

            //�ü���ã��
            //findCmd = new RelayCommand<object>(FindAction);

        }

        /// <summary>
        /// �ش�ü����� ��������ġ ã�ư���
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
                    MessageBox.Show("�߸��� ���̾��Դϴ�.");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("�߸��� ���̾��Դϴ�.");
                return;
            }


            //0.�ش緹�̾�ǥ�� - ���ο����ڵ����� �ε����� üũ��
            ShowShapeLayer(mapView, GetLayerNm(FTR_CDE), true);

            //1.�ش緹�̾� ��������
            FeatureLayer layer = layers[GetLayerNm(FTR_CDE)];



            // Remove any previous feature selections that may have been made.
            layer.ClearSelection();

            // Begin query process.
            await QueryStateFeature(FTR_CDE, FTR_IDN, layer);

        }



        #endregion


        // ���̿��� �ش� Feature ã��
        private async Task QueryStateFeature(string _FTR_CDE, string _FTR_IDN, FeatureLayer _featureLayer)
        {
            try
            {
                // 0.Feature ���̺� ��������
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
                    Messages.ShowErrMsgBox("��ȣ�� �޸� ���� ����..");
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
                        //envBuilder.UnionOf(feature.Geometry.Extent); //������ ��ó���� ��ġ��

                        // Select each feature.
                        _featureLayer.SelectFeature(feature);
                        //�ش���ó�� �̵�
                        await mapView.SetViewpointCenterAsync(feature.Geometry.Extent.GetCenter(), 40000);
                    }

                    // Zoom to the extent of the selected feature(s).
                    //await mapView.SetViewpointGeometryAsync(envBuilder.ToGeometry(), 50);
                    
                }
                else
                {
                    MessageBox.Show("�ش� �ü��� ��ġ�� ã�� �� �����ϴ�.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex, "Sample error");
            }
        }


        //�ü������̾�DIV �ʱ�ȭ�۾�
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



















        #region ============= �����Լ� ============= 

        private async void InitMap()
        {
            //������ġ �� ������ �ʱ�ȭ
            await mapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);

            //Base�� �ʱ�ȭ
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);
            //this._map.Basemap = Basemap.CreateOpenStreetMap();

            //Ÿ�ϸ�
            TileCache tileCache = new TileCache(BizUtil.GetDataFolder("tile", "korea.tpk"));
            ArcGISTiledLayer tileLayer = new ArcGISTiledLayer(tileCache);
            this._map.Basemap = new Basemap(tileLayer);


            //�����������ǥ��
            ShowShapeLayer(mapView, "BML_GADM_AS", true);


            //�ʺ� Ŭ���̺�Ʈ ����
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;


            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
        }



        // ��ó�߰�
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


                //��ó�߰�
                Feature _addedFeature = layerTable.CreateFeature();
                _addedFeature.Geometry = destinationPoint;

                //�Ӽ��߰�
                //Field Field_FTR_CDE = new Field(FieldType.Text, "FTR_CDE", "�ü����ڵ�", 50);
                //Field Field_FTR_IDN = new Field(FieldType.Int32, "FTR_IDN", "������ȣ", 10);
                //Field Field_SHT_NUM = new Field(FieldType.Text, "SHT_NUM", "������ȣ", 50);

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
                //�߰����� ���ΰ�ħ
                _addedFeature.Refresh();

                MessageBox.Show("Added feature ", "Success!");

                //�̺�Ʈ�ڵ鷯����
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


        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 -  �̵�ó��(ServiceFeature)
        public async void handlerGeoViewTappedMoveFeature_org(object sender, GeoViewInputEventArgs e)
        {

            //�̵�ó��
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

                    // Push the update to the service. - Save��ư���� ��������
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

        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 -  �̵�ó��(shape����)
        public async void handlerGeoViewTappedMoveFeature(object sender, GeoViewInputEventArgs e)
        {

            //�̵�ó��
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


                    //������ó�� ��� - �����̵� ������ �����
                    if (_selectedFeature.Geometry is Polyline)
                    {
                        polyline = (Polyline)_selectedFeature.Geometry;

                        List<MapPoint> points = new List<MapPoint>();
                        foreach (var part in polyline.Parts)
                        {
                            //������ ù��°���� �������� �̵�
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
                    //������ ��ó�ΰ�� - �����̵� �������� �����
                    else if (_selectedFeature.Geometry is Polygon)
                    {
                        polygon = (Polygon)_selectedFeature.Geometry;

                        List<MapPoint> points = new List<MapPoint>();
                        foreach (var part in polygon.Parts)
                        {
                            //������ ù��°���� �������� �̵�
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
                    //����Ʈ ��ó�� ���� ��ġ�� �����ϸ��
                    else
                    {
                        _selectedFeature.Geometry = mapPoint;
                    }


                    if (Messages.ShowYesNoMsgBox("�ü��� ��ġ�̵��� �����Ͻðڽ��ϱ�?") == MessageBoxResult.Yes)
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

            //�̺�Ʈ�ڵ鷯����
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;


        }







        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 - �������˾� 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // PopFct ��ü �������� üũ���� - �����ȵ�
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




                // ��ġ���ش��ϴ� ��óã�� ���
                // Perform the identify operation.
                IdentifyLayerResult IR_SEL = await mapView.IdentifyLayerAsync(layers[_selectedLayerNm], e.Position, 5, false);

                // �̺�Ʈ Ÿ����ó
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




                // ������������
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

                // ����ó��
                layers[_selectedLayerNm].SelectFeature(identifiedFeature); //�ü�������Ȱ��ȭ ó��
                //_selectedFeature = (ArcGISFeature)identifiedFeature; //������ó ArcGISFeature ��ȯ����
                _selectedFeature = (Feature)identifiedFeature; //������ó ArcGISFeature ��ȯ����





                //0.���õ� ���̾�� �Ӽ����� ��������
                string _FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                string _FTR_IDN = identifiedFeature.GetAttributeValue("FTR_IDN").ToString();


                //1.DB�ü�����ȸ �� ���̾����� ����
                if (FmsUtil.IsNull(_FTR_CDE) || FmsUtil.IsNull(_FTR_IDN))
                {
                    Messages.ShowErrMsgBox("�ü��� DB������ �����ϴ�.");
                    return;
                }

                // �ü��������˾� ���̱�
                ShowFtrPop(_FTR_CDE, _FTR_IDN, e);

            }
            catch (Exception)
            {
                Console.WriteLine("���̾� featrue click error...");
            }

        }



        // �ü��������˾� ���̱�
        private void ShowFtrPop(string fTR_CDE, string fTR_IDN, GeoViewInputEventArgs e)
        {
            Hashtable param = new Hashtable();
            switch (fTR_CDE)
            {
                case "SA001": //�������
                    //�˾����� & ��ġ
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

                case "SA002": //�޼�����
                    //�˾����� & ��ġ
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

                case "SA003": //����������
                    //�˾����� & ��ġ
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

                case "SA100": //�����Ȧ
                    //�˾����� & ��ġ
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

                case "SA110": //������
                    //�˾����� & ��ġ
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

                case "SA112": //�����
                    //�˾����� & ��ġ
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

                case "SA113": //������
                    //�˾����� & ��ġ
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

                case "SA114": //�����
                    //�˾����� & ��ġ
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


                case "SA117": //������
                    //�˾����� & ��ġ
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

                case "SA118": case "SA119": //��ȭ��,�޼�ž
                    //�˾����� & ��ġ
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


                case "SA120": //������
                    //�˾����� & ��ġ
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


                case "SA121": //���а�
                    //�˾����� & ��ġ
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


                case "SA122": //�޼����跮��
                    //�˾����� & ��ġ
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


                case "SA200": case "SA201": case "SA202": case "SA203": case "SA204": case "SA205": case "SA206": //�����ü�
                    //�˾����� & ��ġ
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


            //�������̹��� ����
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

