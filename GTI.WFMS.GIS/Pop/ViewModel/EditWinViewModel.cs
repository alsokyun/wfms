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


        #region ========== Members ���� ==========

        // ������ ��ó - ���������Feature  
        //public ArcGISFeature _selectedFeature;
        public Feature _selectedFeature;
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();
        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;


        public event PropertyChangedEventHandler PropertyChanged;

        EditWinView editWinView;
        ComboBoxEdit cbFTR_CDE;
        string NEW_FTR_IDN = "";//�ű��߰�ä���� �ü����� ������ȣ 
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

        public RelayCommand<object> LoadedCommand { get; set; } //Loaded�̺�Ʈ���� ICommand ����Ͽ� �䰴ü ���޹���
        public RelayCommand<object> SearchCommand { get; set; }
        
        public RelayCommand<object> AddCmd { get; set; }
        public RelayCommand<object> EditCmd { get; set; }
        public RelayCommand<object> DelCmd { get; set; }
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

        public EditWinViewModel()
        {
            LoadedCommand = new RelayCommand<object>(delegate (object obj)
            {

                // 0.�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                editWinView = obj as EditWinView;
                this.mapView = editWinView.FindName("mapView") as MapView;

                // 1.�����ʱ�ȭ
                InitMap();
                //�������ʱ�����۾�
                GisCmm.InitUniqueValueRenderer();

                // 2.ȭ�� �� �޺��ʱ�ȭ
                cbFTR_CDE = editWinView.cbFTR_CDE;
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", true);
                cbFTR_CDE.EditValueChanged += cbFTR_CDEHandler; //�޺������ڵ鷯

                //��Ʈ���ʱ�ȭ(�ü�����DIV ������)
                BitImg = new BitmapImage();



            });



            //�ü�����ȸ �� �ش緹�̾� ǥ��
            SearchCommand = new RelayCommand<object>(SearchAction);




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
                            fi.CopyTo(Path.Combine(icon_foler, _FTR_CDE), true);
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox(ex.Message);
                        }
                        finally
                        {
                            //1.������ �籸��
                            GisCmm.InitUniqueValueRenderer();

                            //2.���̾��� ������ �缼��
                            foreach (string sel in _selectedLayerNms)
                            {
                                layers[sel].Renderer = GisCmm.uniqueValueRenderer.Clone();
                                layers[sel].RetryLoadAsync();
                            }

                            //3.�˾��̹����ҽ� ������Ʈ
                            BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();

                        }
                    }
                }
            });



            AddCmd = new RelayCommand<object>(async delegate (object obj)
            {


                if (_selectedLayerNms.Count < 1)
                {
                    MessageBox.Show("�ü����� �����ϼ���.");
                    return;
                }
                else if (_selectedLayerNms.Count > 1)
                {
                    MessageBox.Show("�ü����� �ϳ��� �����ϼ���.");
                    return;
                }


                //������ó�� ��� - SketchEditor �� GraphicOverlay�� �����Ѵ�
                if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS"))
                {
                    try
                    {
                        // Let the user draw on the map view using the chosen sketch mode
                        Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(SketchCreationMode.Polyline, true); //�ʿ� �ű�geometry ������

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
                //����Ʈ��ó�� ���� Ŭ���ڵ鷯�� �߰���
                else
                {
                    ////ȭ���ʱ�ȭ
                    InitModel();

                    ////�űԽü��� ��������ȯ
                    editWinView.cctl.Content = new UC_FLOW_PS(FTR_CDE);
                    NEW_FTR_IDN = ((UC_FLOW_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();

                    //�߰�ó�� ���ڵ鷯 �߰�
                    mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                    mapView.GeoViewTapped -= handlerGeoViewTapped;
                    mapView.GeoViewTapped += handlerGeoViewTappedAddFeature;
                    MessageBox.Show("�ü����� �߰��� ������ ���콺�� Ŭ���ϼ���.");
                }

            });


            EditCmd = new RelayCommand<object>(delegate (object obj)
            {
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

            });


            DelCmd = new RelayCommand<object>(async delegate (object obj)
            {
                if (_selectedFeature == null)
                {
                    MessageBox.Show("�ü����� �����ϼ���.");
                    return;
                }

                //����ó��

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
            });

        }

        #endregion






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



            //�ʺ� Ŭ���̺�Ʈ ����
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped -= handlerGeoViewTapped;
            mapView.GeoViewTapped += handlerGeoViewTapped;


            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            mapView.GraphicsOverlays.Add(_sketchOverlay);
        }





        // �ü����ڵ� ����
        private void cbFTR_CDEHandler(object sender, EditValueChangedEventArgs e)
        {
            // 0.����ȭ���ʱ�ȭ
            InitModel();
            //�ü������̾� �ʱ�ȭ
            _selectedLayerNm = "";
            _selectedLayerNms.Clear();

            // 1.���ο� ���̾� ����
            string ftr_cde = e.NewValue.ToString();
            this.FTR_CDE = ftr_cde;

            if (!FmsUtil.IsNull(ftr_cde))
            {
                //���õ� ���̾� ���
                _selectedLayerNm = GisCmm.GetLayerNm(ftr_cde);
                _selectedLayerNms.Add(GisCmm.GetLayerNm(ftr_cde));
            }

            // 2.���õ� ���̾��� �ü��� ��������  �ʱ�ȭ
            switch (ftr_cde)
            {
                case "SA117":
                    editWinView.cctl.Content = new UC_FLOW_PS();
                    break;
                default:
                    editWinView.cctl.Content = null;
                    break;
            }

            // �ü����˻�
            SearchAction(null);

        }


        // ȭ�鸮��
        private void InitModel()
        {
            // ȭ���ʱ�ȭ
            BitImg = new BitmapImage();
            editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
            NEW_FTR_IDN = "";


            // ���ʱ�ȭ - ���ʱ�ȭ�ϸ� tap �̺�Ʈ�ڵ鷯 ���������
            //InitMap();

            //���̾� Ŭ����
            mapView.Map.OperationalLayers.Clear();

            //���õȷ��̾� ����
            _selectedFeature = null;
            try
            {
                layers[_selectedLayerNm].ClearSelection();
            }
            catch (Exception) { }
        }


        //�ü��� shape �˻�
        private void SearchAction(object obj)
        {
            if (FmsUtil.IsNull(FTR_CDE))
            {
                Messages.ShowInfoMsgBox("�ü����� �����ϼ���.");
                return;
            }

            //�����׸� ����
            editWinView.cctl.Content = null;

            string ftr_idn = editWinView.txtFTR_IDN.Text;

            //���̾�ǥ�� - FTR_IDN ���� ���͸�
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, ftr_idn);
        }

        #endregion
















        #region ========== GIS���� ó�� ==============








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

                _addedFeature.SetAttributeValue("FTR_CDE", this.FTR_CDE);
                _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToDouble(NEW_FTR_IDN));




                await layerTable.AddFeatureAsync(_addedFeature);
                //�߰����� ���ΰ�ħ
                _addedFeature.Refresh();

                MessageBox.Show("Added feature ", "Success!");

                //�̺�Ʈ�ڵ鷯����
                mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                mapView.GeoViewTapped -= handlerGeoViewTapped;
                mapView.GeoViewTapped += handlerGeoViewTapped;

                /*
                 * �ü���DB ����
                 */
                Messages.ShowInfoMsgBox("��ġ������ �߰��Ǿ����ϴ�. �ش�ü����� �������� �����ϼ���");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
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




                // 0.������������
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


                // 1.����ó��
                _selectedFeature = (Feature)identifiedFeature; //������ó ArcGISFeature ��ȯ����
                //_selectedFeature = (ArcGISFeature)identifiedFeature; //������ó ArcGISFeature ��ȯ����

                // ���õ� ���̾�� �Ӽ����� ��������
                string _FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();
                string _FTR_IDN = identifiedFeature.GetAttributeValue("FTR_IDN").ToString();

                // DB�ü�����ȸ �� �ش�ü������� UserControl�� ��ȯ
                if (FmsUtil.IsNull(_FTR_CDE) || FmsUtil.IsNull(_FTR_IDN))
                {
                    Messages.ShowErrMsgBox("�ü��� DB������ �����ϴ�.");
                    return;
                }
                else if (_FTR_IDN == this.FTR_IDN)
                {
                    //�����ü����̸� ���ó��������ó������ ������
                    //this.FTR_CDE= "";
                    this.FTR_IDN = null;
                }
                else
                {
                    //�ü�������Ȱ��ȭ ó��
                    layers[_selectedLayerNm].SelectFeature(identifiedFeature); 
                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl �ü������� ��ε�
                switch (cbFTR_CDE.EditValue)
                {
                    case "SA117":
                        editWinView.cctl.Content = new UC_FLOW_PS(this.FTR_CDE, this.FTR_IDN);
                        break;
                    default:
                        editWinView.cctl.Content = new UC_FLOW_PS(this.FTR_CDE, this.FTR_IDN);
                        break;
                }


                // ������ �̹����ҽ� ������Ʈ
                BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();




            }
            catch (Exception)
            {
                Console.WriteLine("���̾� featrue click error...");
            }

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
            //���̾�ǥ�� - FTR_IDN ���� ���͸�
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, null);

            //1.�ش緹�̾� ��������
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];



            // Remove any previous feature selections that may have been made.
            layer.ClearSelection();

            // Begin query process.
            await QueryStateFeature(FTR_CDE, FTR_IDN, layer);

        }



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


        #endregion































    }
}

