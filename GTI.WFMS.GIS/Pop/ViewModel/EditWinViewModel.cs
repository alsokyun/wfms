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


                mapView.SketchEditor.GeometryChanged += OnGeometryChanged;
                mapView.SketchEditor.SelectedVertexChanged += OnSelectedVertexChanged;

            });



            //�ü�����ȸ �� �ش緹�̾� ǥ��
            SearchCommand = new RelayCommand<object>(delegate(object obj) {

                if (FmsUtil.IsNull(FTR_CDE))
                {
                    Messages.ShowInfoMsgBox("�ü����� �����ϼ���.");
                    return;
                }

                //�����׸� �ʱ�ȭ
                InitModel();

                //���������� �ʱ�ȭ
                InitPage(cbFTR_CDE.EditValue.ToString(), null, null);

                SearchAction(obj);
            });




            //�����ܺ��� (����ã��)
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



                //Polygon ��ó�� ��� - SketchEditor �� GraphicOverlay�� �����Ѵ�
                //������ó�� ��� - SketchEditor �� GraphicOverlay�� �����Ѵ�
                if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS") || _selectedLayerNm.Equals("WTL_PURI_AS"))
                {
                    try
                    {
                        SketchCreationMode creationMode = SketchCreationMode.Polyline;
                        Symbol symbol;
                        if (_selectedLayerNm.Equals("WTL_PURI_AS"))
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

                        MessageBox.Show("�ü����� �߰��� ������ ���콺�� Ŭ���ϼ���.(�����Ϸ�� ����Ŭ��)");

                        // Let the user draw on the map view using the chosen sketch mode
                        Esri.ArcGISRuntime.Geometry.Geometry geometry = await mapView.SketchEditor.StartAsync(creationMode, false); //�ʿ� �ű�geometry ������


                        //ȭ�鿡 �׷���ǥ��(�����)
                        Graphic graphic = new Graphic(geometry, symbol);
                        _sketchOverlay.Graphics.Add(graphic);



                        // 0.ȭ���ʱ�ȭ
                        editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
                        NEW_FTR_IDN = "";

                        //���õȷ��̾� ����
                        _selectedFeature = null;
                        try
                        {
                            layers[_selectedLayerNm].ClearSelection();
                        }
                        catch (Exception) { }

                        //�űԽü��� ��������ȯ
                        InitPage(cbFTR_CDE.EditValue.ToString(), FTR_CDE, null);


                        // 1.���̾ ��ġ�߰�
                        AddFeatureToLayer(geometry);


                        // 2.�ü���DB ����
                        Messages.ShowInfoMsgBox("��ġ������ �߰��Ǿ����ϴ�. �ش�ü����� �������� �����ϼ���");

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
                    editWinView.txtFTR_IDN.EditValue = ""; //FTR_IDN = ""
                    NEW_FTR_IDN = "";

                    //���õȷ��̾� ����
                    _selectedFeature = null;
                    try
                    {
                        layers[_selectedLayerNm].ClearSelection();
                    }
                    catch (Exception) { }

                    ////�űԽü��� ��������ȯ
                    InitPage(cbFTR_CDE.EditValue.ToString(), FTR_CDE, null);
                    

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

                    MessageBox.Show("�����Ǿ����ϴ�.");
                    // �������ʱ�ȭ
                    InitPage(cbFTR_CDE.EditValue.ToString(), null, null);

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
            await mapView.SetViewpointCenterAsync(GisCmm._ulsanCoords, GisCmm._ulsanScale2);

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
            editWinView.txtFTR_IDN.EditValue = ""; 

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
            InitPage(ftr_cde, null, null);

            // �ü����˻� 
            if (_selectedLayerNm.Equals("WTL_PIPE_LM") || _selectedLayerNm.Equals("WTL_SPLY_LS"))
            {
                //��뷮�����ʹ� �ڵ��˻� ����
            }
            else
            {
                SearchAction(null);
            }

        }


        // ȭ�鸮��
        private void InitModel()
        {
            // ȭ���ʱ�ȭ
            BitImg = new BitmapImage();
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

            //�׷��Ƚ� Ŭ����
            //_sketchOverlay.ClearSelection();
            _sketchOverlay.Graphics.Clear();

        }


        /// <summary>
        /// UserControl �ü��������� �ε�
        /// </summary>
        /// <param name="CBO_FTR_CDE"></param>
        /// <param name="_FTR_CDE"></param>
        /// <param name="_FTR_IDN"></param>
        private void InitPage(string CBO_FTR_CDE, string _FTR_CDE, string _FTR_IDN)
        {
            switch (CBO_FTR_CDE)
            {
                case "SA001": //�������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PIPE_LM(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_PIPE_LM)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PIPE_LM(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA002": //�޼�����
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_SPLY_LS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_SPLY_LS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_SPLY_LS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA003": //����������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_STPI_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_STPI_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_STPI_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA100": //�����Ȧ
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_MANH_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_MANH_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_MANH_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA110": //������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_HEAD_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_HEAD_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_HEAD_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA112": //�����
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_GAIN_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_GAIN_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_GAIN_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;


                case "SA113": //������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PURI_AS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_PURI_AS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PURI_AS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA114": //�����
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_SERV_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_SERV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_SERV_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;



                case "SA117": //������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_FLOW_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA118": case "SA119": //�޼�ž,��ȭ��
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_FIRE_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_FIRE_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_FIRE_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA120": //������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_RSRV_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_RSRV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_RSRV_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA121": //���а�
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PRES_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_PRES_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PRES_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA122": //�޼����跮��
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_META_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_META_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_META_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA200": case "SA201": case "SA202": case "SA203": case "SA204": case "SA205":
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_VALV_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_VALV_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_VALV_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;

                case "SA206": //����������
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        editWinView.cctl.Content = new UC_PRGA_PS(_FTR_CDE);//�ű�������
                        NEW_FTR_IDN = ((UC_PRGA_PS)editWinView.cctl.Content).txtFTR_IDN.EditValue.ToString();
                    }
                    else
                    {
                        editWinView.cctl.Content = new UC_PRGA_PS(_FTR_CDE, _FTR_IDN);//��������
                    }
                    break;








                default:
                    editWinView.cctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);
                    break;
            }
        }



        //�ü��� shape �˻�
        private void SearchAction(object obj)
        {
            string ftr_idn = editWinView.txtFTR_IDN.Text;

            //���̾�ǥ�� - FTR_IDN ���� ���͸�
            ShowShapeLayerFilter(mapView, GisCmm.GetLayerNm(FTR_CDE), true, ftr_idn);

            //��ȸ�� ��ó �ڵ�����
            SelectFct(FTR_CDE,  ftr_idn, layers[_selectedLayerNm]);

        }




        #endregion














        // ������ȣ�� �ش�Feature ��üã��
        public async void SelectFct(string _FTR_CDE, string _FTR_IDN, FeatureLayer _featureLayer)
        {
            // 0.Feature ���̺� ��������
            //FeatureLayer __featureLayer = _featureLayer.Clone() as FeatureLayer;
            FeatureTable _featureTable = _featureLayer.FeatureTable;



            // Create a query parameters that will be used to Query the feature table.
            QueryParameters queryParams = new QueryParameters();


            // Construct and assign the where clause that will be used to query the feature table.
            queryParams.WhereClause = " FTR_CDE = '" + _FTR_CDE + "' ORDER BY FTR_IDN DESC";
            if (!FmsUtil.IsNull(_FTR_IDN))
            {
                queryParams.WhereClause = " FTR_CDE = '" + _FTR_CDE + "' AND FTR_IDN = " + _FTR_IDN;
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



                // Loop over each feature from the query result.
                foreach (Feature feature in features)
                {
                    //ù��°���� ����ó��
                    ShowFctPage(feature);
                    break;
                }
            }
        }



        //�ʺ� Feature���� ó�� - �������˾� 
        public async void ShowFctPage(Feature identifiedFeature)
        {
            try
            {
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
                    return ;
                }
                //else if (_FTR_IDN == this.FTR_IDN)
                //{
                //    //�����ü����̸� ���ó��������ó������ ������
                //    //this.FTR_CDE= "";
                //    this.FTR_IDN = null;
                //}
                else
                {
                    //�ü�������Ȱ��ȭ ó��
                    layers[_selectedLayerNm].SelectFeature(identifiedFeature);
                    //�ش���ó�� �̵�
                    await mapView.SetViewpointCenterAsync(identifiedFeature.Geometry.Extent.GetCenter(), 40000);

                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl �ü������� ��ε�
                InitPage(cbFTR_CDE.EditValue.ToString(), this.FTR_CDE, this.FTR_IDN);


                // ������ �̹����ҽ� ������Ʈ
                try
                {
                    BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();
                }
                catch (Exception){}

            }
            catch (Exception e)
            {
                Console.WriteLine("���̾� featrue click error..." + e.Message);
            }

        }
















        #region ========== GIS���� ó�� ==============








        // ��ó�߰�
        public void handlerGeoViewTappedAddFeature(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                // Get the MapPoint from the EventArgs for the tap.
                MapPoint destinationPoint = e.Location;
                // Normalize the point - needed when the tapped location is over the international date line.
                destinationPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(destinationPoint);


                // 1.���̾ ��ġ�߰�
                AddFeatureToLayer(destinationPoint);


                //MessageBox.Show("Added feature ", "Success!");

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

        /// <summary>
        /// ���̾ Feature(MapPoint, PolyLine, Polygon)�� �߰�
        /// </summary>
        /// <param name="geometry"></param>
        private async void AddFeatureToLayer(Geometry geometry)
        {
            FeatureTable layerTable = layers[_selectedLayerNm].FeatureTable;


            //��ó�߰�
            Feature _addedFeature = layerTable.CreateFeature();
            _addedFeature.Geometry = geometry;

            //�Ӽ��߰�
            _addedFeature.SetAttributeValue("FTR_CDE", this.FTR_CDE);
            _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToDouble(NEW_FTR_IDN));

            try
            {
                await layerTable.AddFeatureAsync(_addedFeature);
            }
            catch (Exception)
            {
                //������ȣ Ÿ���� �����̿��� �������� �ٽ� �õ�
                _addedFeature.SetAttributeValue("FTR_IDN", Convert.ToInt32(NEW_FTR_IDN));
                await layerTable.AddFeatureAsync(_addedFeature);
            }
            
            
            //�߰����� ���ΰ�ħ
            _addedFeature.Refresh();
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
                //else if (_FTR_IDN == this.FTR_IDN)
                //{
                //    //�����ü����̸� ���ó��������ó������ ������
                //    //this.FTR_CDE= "";
                //    this.FTR_IDN = null;
                //}
                else
                {
                    //�ü�������Ȱ��ȭ ó��
                    layers[_selectedLayerNm].SelectFeature(identifiedFeature); 
                    //this.FTR_CDE = _FTR_CDE;
                    this.FTR_IDN = _FTR_IDN;
                }



                // UserControl �ü������� ��ε�
                InitPage(cbFTR_CDE.EditValue.ToString(), this.FTR_CDE, this.FTR_IDN);


                // ������ �̹����ҽ� ������Ʈ
                BitImg = new BitmapImage(new Uri(Path.Combine(Path.Combine(BizUtil.GetDataFolder(), "style_img"), _FTR_CDE))).Clone();




            }
            catch (Exception)
            {
                Console.WriteLine("���̾� featrue click error...");
            }

        }







        #endregion































    }
}

