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


        #region ========== Members ���� ==========

        // ������ ��ó - ���������Feature  
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

        private FctDtl fctDtl = new FctDtl(); //�ü����⺻����
        private Popup divLayer = new Popup(); //�ü������̾�DIV
        private PopFct popFct = new PopFct(); //�ü�������DIV
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

        /// <summary>
        /// Gets or sets the map
        /// </summary>
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

        public FctDtl FctDtl
        {
            get { return this.fctDtl; }
            set { this.fctDtl = value; }
        }


        #endregion




               
        #region ========== ������ ==========

        public MapMainViewModel()
        {

            //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
            //loadedCmd = new RelayCommand<object>(loadedMethod);
            /*
             */
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
                        _selectedLayers.Add(doc.Tag.ToString());
                    }
                    else
                    {
                        _selectedLayers.Remove(doc.Tag.ToString());
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
                // ����������� ǥ��
                //ShowLocalServerLayer(mapView, "BML_GADM_AS", true);
                ShowShapeLayer(mapView, "BML_GADM_AS", true);


                //3.�����ִ� �ü�������â �ݱ�
                popFct.IsOpen = false;

                TreeView treeLayer = obj as TreeView;

                //���̾�div üũ����
                foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
                {
                    cb.IsChecked = false;
                }
                //���õȷ��̾� ����
                _selectedLayers.Clear();
            });



            btnCmd = new RelayCommand<object>(async delegate (object obj)
            {
                Button btn = obj as Button;
                switch (btn.Content.ToString())
                {
                    case "�߰�":
                        if (_selectedLayers.Count < 1)
                        {
                            MessageBox.Show("�ü����� �����ϼ���.");
                            return;
                        }
                        else if (_selectedLayers.Count > 1)
                        {
                            MessageBox.Show("�ü����� �ϳ��� �����ϼ���.");
                            return;
                        }


                        //������ó�� ��� - SketchEditor �� GraphicOverlay�� �����Ѵ�
                        if (_selectedLayers[0].Equals("WTL_PIPE_LM") || _selectedLayers[0].Equals("WTL_SPLY_LS"))
                        {
                            try
                            {
                                // Let the user draw on the map view using the chosen sketch mode
                                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(SketchCreationMode.Polyline, true); //�ʿ� �ű�geometry ������

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
                        //����Ʈ��ó�� ���� Ŭ���ڵ鷯�� �߰���
                        else
                        {
                            //�߰�ó�� ���ڵ鷯 �߰�
                            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                            mapView.GeoViewTapped -= handlerGeoViewTapped;
                            mapView.GeoViewTapped += handlerGeoViewTappedAddFeature;
                            MessageBox.Show("�ü����� �߰��� ������ ���콺�� Ŭ���ϼ���.");
                        }

                        break;

                    case "�̵�":
                        if (_selectedFeature == null)
                        {
                            MessageBox.Show("�ü����� �����ϼ���.");
                            return;
                        }


                        MessageBox.Show("�̵��� ������ ���콺�� Ŭ���ϼ���.");
                        //�̵�ó�� ���ڵ鷯 �߰�
                        mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                        mapView.GeoViewTapped -= handlerGeoViewTapped;
                        mapView.GeoViewTapped += handlerGeoViewTappedMoveFeature;
                        break;

                    case "����":
                        if (_selectedFeature == null)
                        {
                            MessageBox.Show("�ü����� �����ϼ���.");
                            return;
                        }

                        //����ó��
                        //�����ִ� �ü�������â �ݱ�
                        popFct.IsOpen = false;

                        // Load the feature.
                        //await _selectedFeature.LoadAsync();

                        Feature back_selectedFeature = _selectedFeature;
                        if (Messages.ShowYesNoMsgBox("�ü��� ��ġ������ �����Ͻðڽ��ϱ�?") == MessageBoxResult.Yes)
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

                    case "���":
                        // Push the update to the service.
                        //ServiceFeatureTable serviceTableCancel = (ServiceFeatureTable)_selectedFeature.FeatureTable;
                        //serviceTableCancel.CancelLoad();
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
                //string layerUrl = _localFeatureService.Url + "/" + GetLayerId(_selectedLayers[0]);

                // Create the ServiceFeatureTable
                //ServiceFeatureTable serviceFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));
                //FeatureLayer featureLayer = new FeatureLayer(serviceFeatureTable);

                // Wait for the layer to load
                //await featureLayer.LoadAsync();


                FeatureTable layerTable = layers[_selectedLayers[0]].FeatureTable;


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
                //�߰����� ���ΰ�ħ
                _addedFeature.Refresh();

                MessageBox.Show("Added feature ", "Success!");

                //�̺�Ʈ�ڵ鷯����
                mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
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
            mapView.GeoViewTapped += handlerGeoViewTapped;

        }







        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 - �������˾� 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                FeatureLayer layer = new FeatureLayer(); //�̺�Ʈ�߻� ���̾�



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

                Feature identifiedFeature; //�̺�Ʈ Ÿ�ٷ��̾�



                if (IR_WTL_STPI_PS.GeoElements.Any())
                {
                    identifiedFeature = (Feature)IR_WTL_STPI_PS.GeoElements[0];
                    layer = layers["WTL_STPI_PS"]; //������ ���̾�
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




                //��������
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

                //����ó��
                layer.SelectFeature(identifiedFeature); //�ü�������Ȱ��ȭ ó��
                                                        //_selectedFeature = (ArcGISFeature)identifiedFeature; //������ó ArcGISFeature ��ȯ����
                _selectedFeature = (Feature)identifiedFeature; //������ó ArcGISFeature ��ȯ����






                //�˾����� & ��ġ
                popFct.IsOpen = false;

                popFct = new PopFct();
                popFct.PlacementRectangle = new Rect(e.Position.X + 300, e.Position.Y - 200, 250, 300);
                popFct.IsOpen = true;
                popFct.DataContext = this;



                //���õ� ���̾�� �Ӽ����� ��������
                //& ���̾����� ����
                try
                {
                    FctDtl.FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                    FctDtl.FTR_NAM = BizUtil.GetCodeNm("Select_FTR_NM", FctDtl.FTR_CDE);
                    FctDtl.FTR_IDN = Int32.Parse(identifiedFeature.GetAttributeValue("FTR_IDN").ToString());
                    FctDtl.SHT_NUM = identifiedFeature.GetAttributeValue("SHT_NUM").ToString();
                    FctDtl.HJD_CDE = identifiedFeature.GetAttributeValue("HJD_CDE").ToString();
                    FctDtl.HJD_NAM = BizUtil.GetCodeNm("Select_ADAR_NM", FctDtl.HJD_CDE);
                    FctDtl.MNG_CDE = identifiedFeature.GetAttributeValue("MNG_CDE").ToString();
                    FctDtl.MNG_NAM = BizUtil.GetCdNm("250101", FctDtl.MNG_CDE);//�������
                    FctDtl.IST_YMD = identifiedFeature.GetAttributeValue("IST_YMD").ToString();
                }
                catch (Exception) { }
                try
                {
                    FctDtl.MOP_CDE = identifiedFeature.GetAttributeValue("MOP_CDE").ToString();
                    FctDtl.MOP_NAM = BizUtil.GetCdNm("250102", FctDtl.MOP_CDE);//������
                    FctDtl.MOF_NAM = FctDtl.MOP_NAM; //������ MOF�� ��������!
                }
                catch (Exception) { }
                try
                {
                    FctDtl.MOF_CDE = identifiedFeature.GetAttributeValue("MOF_CDE").ToString();
                    FctDtl.MOF_NAM = BizUtil.GetCdNm("250004", FctDtl.MOF_CDE);//�跮������
                }
                catch (Exception) { }
            }
            catch (Exception)
            {
                Console.WriteLine("���̾� featrue click error...");
            }


        }


        #endregion






















    }
}

