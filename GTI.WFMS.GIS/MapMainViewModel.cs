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
using GTI.WFMS.GIS.Module;
using Esri.ArcGISRuntime.LocalServices;
using GTIFramework.Common.MessageBox;
using System.Drawing;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapMainViewModel : LayerModel, INotifyPropertyChanged
    {

        public MapMainViewModel()
        {


            //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
            //loadedCmd = new RelayCommand<object>(loadedMethod);
            /*
             */
            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {

                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                Grid divGrid = obj as Grid;

                this.mapView = divGrid.FindName("mapView") as MapView;
                this.divLayer = divGrid.FindName("divLayer") as Popup;

                //�����ʱ�ȭ
                InitMap();

                
                //�ü������̾�DIV �ʱ�ȭ�۾�
                InitDivLayer();
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
            });

            //�˾����̾� ���ó��
            toggleCmd = new RelayCommand<object>(delegate (object obj)
            {
                StackPanel spLayer = divLayer.FindName("spLayer") as StackPanel;
                Grid gridTitle = divLayer.FindName("gridTitle") as Grid;

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
                ShowLocalServerLayer(mapView, "BML_GADM_AS", true);

                //3.�����ִ� �ü�������â �ݱ�
                popFct.IsOpen = false;



                //���̾�div üũ����
                foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(divLayer))
                {
                    cb.IsChecked = false;
                }

            });



            btnCmd = new RelayCommand<object>(async delegate (object obj)
            {
                Button btn = obj as Button;
                switch (btn.Content.ToString())
                {
                    case "�߰�":
                        //�߰�ó�� ���ڵ鷯 �߰�
                        mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                        mapView.GeoViewTapped -= handlerGeoViewTapped;
                        mapView.GeoViewTapped += handlerGeoViewTappedAddFeature;
                        MessageBox.Show("�ü����� �߰��� ������ ���콺�� Ŭ���ϼ���.");
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
                        await _selectedFeature.LoadAsync();

                        // Apply the edit to the feature table.
                        await _selectedFeature.FeatureTable.DeleteFeatureAsync(_selectedFeature);
                        _selectedFeature.Refresh();


                        break;

                    case "����":
                        if(Messages.ShowYesNoMsgBox("����� �ü�����ġ������ �����Ͻðڽ��ϱ�?") == MessageBoxResult.No)
                        {
                            return;
                        }

                        // Push the update to the service.
                        ServiceFeatureTable serviceTable = (ServiceFeatureTable)_selectedFeature.FeatureTable;
                        await serviceTable.ApplyEditsAsync();
                        MessageBox.Show("Success!");

                        //�̺�Ʈ�ڵ鷯����
                        mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
                        mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
                        mapView.GeoViewTapped += handlerGeoViewTapped;
                        break;
                    case "���":
                        // Push the update to the service.
                        ServiceFeatureTable serviceTableCancel = (ServiceFeatureTable)_selectedFeature.FeatureTable;
                        serviceTableCancel.CancelLoad();
                        break;
                    default:
                        break;
                }


            });

        }

        private string mode = "NORMAL";


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

        public FctDtl FctDtl
        {
            get { return this.fctDtl; }
            set { this.fctDtl = value; }
        }


        #endregion




        #region ========== Members ���� ==========

        public event PropertyChangedEventHandler PropertyChanged;



        // Coordinates for Ulsan
        private MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        private double _ulsanScale = 150000;

        private FctDtl fctDtl = new FctDtl(); //�ü����⺻����
        private Popup divLayer = new Popup(); //�ü������̾�DIV
        private PopFct popFct = new PopFct(); //�ü�������DIV
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
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped += handlerGeoViewTapped;


        }



        // ��ó�߰�
        public async void handlerGeoViewTappedAddFeature(object sender, GeoViewInputEventArgs e)
        {
            // Get the MapPoint from the EventArgs for the tap.
            MapPoint destinationPoint = e.Location;
            // Normalize the point - needed when the tapped location is over the international date line.
            destinationPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(destinationPoint);



            // Get the path to the first layer - the local feature service url + layer ID
            string layerUrl = _localFeatureService.Url + "/" + GetLayerId("WTL_HEAD_PS");

            // Create the ServiceFeatureTable
            ServiceFeatureTable serviceFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));
            FeatureLayer featureLayer = new FeatureLayer(serviceFeatureTable);

            // Wait for the layer to load
            await featureLayer.LoadAsync();

            //��ó�߰�
            ArcGISFeature _addedFeature = (ArcGISFeature)serviceFeatureTable.CreateFeature();
            _addedFeature.Geometry = destinationPoint;

            //�Ӽ��߰�
            //Field Field_FTR_CDE = new Field(FieldType.Text, "FTR_CDE", "�ü����ڵ�", 50);
            //Field Field_FTR_IDN = new Field(FieldType.Int32, "FTR_IDN", "������ȣ", 10);
            //Field Field_SHT_NUM = new Field(FieldType.Text, "SHT_NUM", "������ȣ", 50);

            _addedFeature.SetAttributeValue("FTR_CDE", "SA110");
            _addedFeature.SetAttributeValue("FTR_IDN", 99999);
            _addedFeature.SetAttributeValue("SHT_NUM", "99999");
            _addedFeature.SetAttributeValue("SHT_NUM", "99999");
            _addedFeature.SetAttributeValue("HJD_CDE", "3171033000");
            _addedFeature.SetAttributeValue("MNG_CDE", "MNG401");
            



            await serviceFeatureTable.AddFeatureAsync(_addedFeature);

            //�߰����� ����ó��
            //await featureLayer.LoadAsync();
            await serviceFeatureTable.ApplyEditsAsync();

            //�߰����� ���ΰ�ħ
            //featureLayer.SelectFeature(_addedFeature);
            _addedFeature.Refresh(); 

            MessageBox.Show("Added feature ", "Success!");

            //�̺�Ʈ�ڵ鷯����
            mapView.GeoViewTapped -= handlerGeoViewTappedMoveFeature;
            mapView.GeoViewTapped -= handlerGeoViewTappedAddFeature;
            mapView.GeoViewTapped += handlerGeoViewTapped;
        }


        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 -  �̵�ó��
        public async void handlerGeoViewTappedMoveFeature(object sender, GeoViewInputEventArgs e)
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
                    await _selectedFeature.LoadAsync();

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







        //�ʺ� Ŭ���̺�Ʈ �ڵ鷯 - �������˾� 
        public async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
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
            IdentifyLayerResult IR_WTL_FIRE_PS = await mapView.IdentifyLayerAsync(layers["WTL_FIRE_PS"], e.Position, 20, false);
            IdentifyLayerResult IR_WTL_VALV_PS = await mapView.IdentifyLayerAsync(layers["WTL_VALV_PS"], e.Position, 20, false);
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
            else if (IR_WTL_FIRE_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_FIRE_PS.GeoElements[0];
                layer = layers["WTL_FIRE_PS"];
            }
            else if (IR_WTL_VALV_PS.GeoElements.Any())
            {
                identifiedFeature = (Feature)IR_WTL_VALV_PS.GeoElements[0];
                layer = layers["WTL_VALV_PS"];
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
            _selectedFeature = (ArcGISFeature)identifiedFeature; //������ó ArcGISFeature ��ȯ����






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
            catch (Exception ) { }
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

        // ���������Feature  
        public ArcGISFeature _selectedFeature;

        #endregion






















    }
}

