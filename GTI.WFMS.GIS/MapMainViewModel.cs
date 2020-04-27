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


        #region ========== Members ���� ==========

        // ������ ��ó - ���������Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();
        
        
        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;
        //private Esri.ArcGISRuntime.Geometry.Geometry _geometry;


        public event PropertyChangedEventHandler PropertyChanged;




        //private FctDtl fctDtl = new FctDtl(); //�ü����⺻����
        private Popup divLayer = new Popup(); //�ü������̾�DIV
        private Popup popFct = new Popup(); //�ü�������DIV
        private Button ClearButton = new Button();
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
        public RelayCommand<object> resetCmd { get; set; }
        public RelayCommand<object> levelCmd { get; set; }

        
        public RelayCommand<object> completeCmd { get; set; }
        public RelayCommand<object> clearCmd { get; set; }

        public RelayCommand<object> CallPageCmd { get; set; }//�ü����˾����� �ü����޴�ȭ�� ȣ���۾�

        public RelayCommand<object> EditCmd { get; set; }

        


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

        public MapMainViewModel()
        {
            //string licenseKey = "runtimelite,1000,rud1244207246,none,9TJC7XLS1MJPF5KHT033"; //�׸���
            //string licenseKey = "runtimelite,1000,rud9177830334,none,A3E60RFLTFM5NERL1040"; //kyun0828 free 
            
            //ArcGISRuntimeEnvironment.SetLicense(licenseKey);


            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {

                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                System.Windows.Controls.Grid divGrid = obj as System.Windows.Controls.Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayer = divGrid.FindName("divLayer") as Popup;
                this.ClearButton = divGrid.FindName("ClearButton") as Button;


                //�����ʱ�ȭ
                InitMap();


                //�ü������̾�DIV �ʱ�ȭ�۾�
                InitDivLayer();

                GisCmm.InitUniqueValueRenderer();//�������ʱ�����۾�


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

                ShowLocalServerLayer(mapView, doc.Tag.ToString(), chk);
                //ShowShapeLayer(mapView, doc.Tag.ToString(), chk);


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



            // ���̾Ÿ�� Renderer �ʱ�ȭ - shape��Ŀ����� �����
            //InitUniqueValueRenderer();

            levelCmd = new RelayCommand<object>(delegate (object obj)
            {
                MessageBox.Show("MapScale - " + mapView.MapScale);
            });

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
                _selectedFeature = null;
                try
                {
                    layers[_selectedLayerNm].ClearSelection();
                }
                catch (Exception) { }
                _selectedLayerNms.Clear();
                _selectedLayerNm = "";


                // ����������� ǥ��
                ShowLocalServerLayer(mapView, "BML_GADM_AS", true ) ;
            });

            //�ü����˾����� �ü����޴�ȭ�� ȣ���۾�
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

            //�ü�������â
            EditCmd = new RelayCommand<object>(delegate (object obj)
            {
                EditWinView view = new EditWinView();
                if (view.ShowDialog() is bool)
                {
                    //����ȸ
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

                //_selectedFeature.Geometry = _geometry;
                // Apply the edit to the feature table.
                await _selectedFeature.FeatureTable.UpdateFeatureAsync(_selectedFeature);
                _selectedFeature.Refresh();
                MessageBox.Show("Added feature ", "Success!");

            });


        }

        /// <summary>
        /// �ش�ü����� ��������ġ ã�ư���(����ȭ�鿡�� ȣ���) 
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
            ///ShowShapeLayer(mapView, GisCmm.GetLayerNm(FTR_CDE), true);

            //1.�ش緹�̾� ��������
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];



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


        
















        #region ============= �����Լ� ============= 

        private async void InitMap()
        {
            //������ġ �� ������ �ʱ�ȭ
            await mapView.SetViewpointCenterAsync(GisCmm._ulsanCoords, GisCmm._ulsanScale);

            //Base�� �ʱ�ȭ
            Console.WriteLine("this._map.SpatialReference - " + this._map.SpatialReference);
            //this._map.Basemap = Basemap.CreateOpenStreetMap();

            //Ÿ�ϸ�
            TileCache tileCache = new TileCache(BizUtil.GetDataFolder("tile", "korea.tpk"));
            ArcGISTiledLayer tileLayer = new ArcGISTiledLayer(tileCache);
            this._map.Basemap = new Basemap(tileLayer);


            //�����������ǥ��
            ///ShowShapeLayer(mapView, "BML_GADM_AS", true );


            //�ʺ� Ŭ���̺�Ʈ ����
            //mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;

            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
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







        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 - �������˾� 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            try
            {

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
            //�˾����� & ��ġ
            popFct.IsOpen = false;

            popFct = new FTR_POP(fTR_CDE, fTR_IDN);

            //double y0 = e.Position.Y - 700; //�������� ǥ����
            //if (y0 < 100) y0 = 100;
            popFct.PlacementRectangle = new Rect(e.Position.X + 550, e.Position.Y - 300, 10, 10);
            popFct.IsOpen = true;
        }

        #endregion






















    }
}

