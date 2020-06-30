using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GTI.WFMS.Models.Common;
using Esri.ArcGISRuntime.UI.Controls;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.GIS.Module;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms.Integration;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using System.Collections.ObjectModel;
using GTI.WFMS.GIS.Pop.View;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;
using GTI.WFMS.GIS.Ext;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// Provides map data to an application
    /// </summary>
    public class MapArcObjViewModel : INotifyPropertyChanged
    {


        #region ========== Members ���� ==========

        // ������ ��ó - ���������Feature  
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();

        UserControl mapArcObjView;
        WindowsFormsHost mapHost;
        WindowsFormsHost toolbarHost;
        TreeView treeLayer;
        AxMapControl mapControl;
        AxToolbarControl toolbarControl;

        public event PropertyChangedEventHandler PropertyChanged;
        private Popup popFct = new Popup(); //�ü�������DIV
        public FmsStack<string> sts = new FmsStack<string>();

        






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
        public virtual ObservableCollection<FileInfo> ItemsFile { get; set; } //���ϰ�ü


        public RelayCommand<object> loadedCmd { get; set; } //Loaded�̺�Ʈ���� ICommand ����Ͽ� �䰴ü ���޹���
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }
        public RelayCommand<object> importCmd { get; set; }
        

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
        //üũ�ڽ� ������
        private bool chkSA117;
        public bool ChkSA117
        {
            get { return chkSA117; }
            set
            {
                this.chkSA117 = value;
                OnPropertyChanged("ChkSA117");
            }

        }

        #endregion





        #region ========== ������ ==========

        public MapArcObjViewModel()
        {

            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {
                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                mapArcObjView = obj as UserControl;

                mapHost = mapArcObjView.FindName("mapHost") as WindowsFormsHost;
                toolbarHost = mapArcObjView.FindName("toolbarHost") as WindowsFormsHost;
                treeLayer = mapArcObjView.FindName("treeLayer") as TreeView;

                mapControl = mapHost.Child as AxMapControl;
                toolbarControl = toolbarHost.Child as AxToolbarControl;

                
                //0.�� �ʱ�ȭ
                initMap();

                //1.���̾��ʱ�ȭ
                CmmObj.initLayers();

                //2.UniqueRenderer �ʱ�ȭ
                CmmObj.InitUniqueValueRendererObj();

                //3.�����������ǥ��
                ShowShapeLayer("BML_GADM_AS", true );


                // ������������Ʈ
                ItemsFile = new ObservableCollection<FileInfo>();

                //��Ʈ���ʱ�ȭ(�ü�����DIV ������)
                BitImg = new BitmapImage();

                //�ʸ��콺Ŭ�� �̺�Ʈ����
                mapControl.OnMouseUp += OnMouseClick;
            });


            //���̾� ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;


                //ShowShapeLayer( doc.Tag.ToString(), chk);
                ShowShapeLayer(doc.Tag.ToString(), chk);


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


            //GIS�ʱ�ȭ
            resetCmd = new RelayCommand<object>(async delegate(object obj) {

                string stat = await resetAction(obj);
            });

            //SHP���ϰ���â
            importCmd = new RelayCommand<object>(async delegate(object obj) {

                ShpMngView view = new ShpMngView();
                if (view.ShowDialog() is bool)
                {
                    //����ȸ
                    string stat = await resetAction(null);
                }


            });

            
        }
        #endregion






        //���콺 Ŭ�� �̺�Ʈ�ڵ鷯
        private void OnMouseClick(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //IPoint pQueryPoint = new PointClass();
            IPoint pQueryPoint = mapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            var x = pQueryPoint.X;
            var y = pQueryPoint.Y;

            //Ŭ����������
            int PxTol = 6; // 6 pixels to select by
            IPoint pNextPoint = mapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x + PxTol, e.y);
            double pSrchDist = pNextPoint.X - pQueryPoint.X; // measure the distance between points PxTol apart
            IGeometry buffer = (pQueryPoint as ITopologicalOperator).Buffer(99);
            //IGeometry buffer = (pQueryPoint as ITopologicalOperator).Buffer(pSrchDist);

            var XMax = buffer.Envelope.XMax;
            var XMin = buffer.Envelope.XMin;
            var YMax = buffer.Envelope.YMax;
            var YMin = buffer.Envelope.YMin;
            Console.WriteLine("XMax- " + XMax);
            Console.WriteLine("XMin- " + XMin);
            Console.WriteLine("YMax- " + YMax);
            Console.WriteLine("YMin- " + YMin);

            /// Ȱ��ȭ�� ���̾ ���ؼ�, �ش� Ŭ�� ������ �����ϴ� �����˾��� ����
            //FeatureLayer layer = CmmObj.layers[GisCmm.GetLayerNm("SA117")].FL;
            foreach (KeyValuePair<string, FmsFeature> item in CmmObj.layers)
            {
                if (item.Key == "BML_GADM_AS") continue; //�����������̾�� ����

                if (item.Value.chk)
                {
                    findFtrByRegion(item.Value.FL, buffer);
                }
            }


        }

        //�ش� ������ �����ϴ� ���̾� ��óã�Ƽ� Ȱ��ȭ, �˾�â
        private void findFtrByRegion(IFeatureLayer layer, IGeometry region)
        {
            ISpatialFilter pSF = new SpatialFilter();
            pSF.Geometry = region;
            pSF.GeometryField = "Shape";
            pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            //pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
            //pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            //pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelEnvelopeIntersects;



            IFeatureCursor pDestCur = layer.Search(pSF, false);
            IFeature pFeat = pDestCur.NextFeature();
            while (pFeat != null)
            {
                // do something with pFeat
                //var ftr_cde = pFeat.Value[2];
                //var ftr_idn = pFeat.Value[0];
                string ftr_cde = "";
                string ftr_idn = "";

                //��ó�� �ü����ڵ�,������ȣ �˾Ƴ���
                for (int i=0; i< pFeat.Fields.FieldCount; i++)
                {
                    if ("ftr_cde".Equals(pFeat.Fields.Field[i].Name.ToLower()))
                    {
                        ftr_cde = pFeat.Value[i];
                        break;
                    }
                }
                for (int i=0; i< pFeat.Fields.FieldCount; i++)
                {
                    if ("ftr_idn".Equals(pFeat.Fields.Field[i].Name.ToLower()))
                    {
                        ftr_idn = Convert.ToString(pFeat.Value[i]);
                        break;
                    }
                }

                if (FmsUtil.IsNull(ftr_cde) || FmsUtil.IsNull(ftr_idn))
                {
                    MessageBox.Show("�ü��������� �����ϴ�.");
                    return;
                }




                //1.����Ʈó�� ���͸�
                IQueryFilter qfltr = new QueryFilter();
                qfltr.WhereClause = " FTR_CDE = '" + ftr_cde + "' AND FTR_IDN = " + ftr_idn;
                IFeatureSelection fsel = layer as IFeatureSelection;
                fsel.Clear();
                fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);
                mapControl.Refresh();


                //0.�ü����˾�ȣ��
                int ftr_cde_len = 0;
                try
                {
                    ftr_cde_len = (Convert.ToString(ftr_cde) as string).Length ;
                }
                catch (Exception){}

                if (ftr_cde_len == 5 && !FmsUtil.IsNull(ftr_idn)) //�ü����ڵ�,��ȣ�� ��ȿ�Ѱ�츸
                {
                    FtrPopView ftrPopView = new FtrPopView(ftr_cde, ftr_idn);
                    try
                    {
                        ftrPopView.lbTitle.Content = BizUtil.GetCodeNm("Select_FTR_LIST2", ftr_cde);
                    }
                    catch (Exception)
                    {
                        ftrPopView.lbTitle.Content = "�ü�������";
                    }
                    if (ftrPopView.ShowDialog() is bool)
                    {
                        //����ȸ
                        fsel.Clear();
                        mapControl.Refresh();
                    }
                }
                break;
                //pFeat = pDestCur.NextFeature(); //ù��° �ɸ� �ü����� �˾������
            }
        }







        #region ============= �����Լ� ============= 


        public void initMap()
        {
            //��������� mxd���Ϸκ���
            //string mxdfilePath = Path.Combine(BizUtil.GetDataFolder("tile", "also.mxd"));
            string mxdfilePath = FmsUtil.mdxDir +  @"\also.mxd";
            mapControl.LoadMxFile(mxdfilePath);

            
            mapControl.OnExtentUpdated += OnExtentUpdatedHandlelr;

        }
        //���������̺�Ʈ ó�� 
        private void OnExtentUpdatedHandlelr(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// ������,���̾� �ʱ�ȭ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<string> resetAction(object obj)
        {
            //0.���Ƿ��̾� Ŭ����
            mapControl.ClearLayers();
            mapControl.Refresh();


            //1.���ʱ�ȭ
            initMap();

            //2.���̾��ʱ�ȭ
            CmmObj.initLayers();

            //3.���̾�� �ʱ�ȭ
            sts.Clear();

            //4.��������ǥ��
            ShowShapeLayer("BML_GADM_AS", true );




            //�����ִ� �ü�������â �ݱ�
            popFct.IsOpen = false;

            //���̾�div üũ����
            foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
            {
                cb.IsChecked = false;
            }
            await Task.Delay(1000);
            return "ok";
        }





        /// <summary>
        /// Shape ���̾� ���̱�/���� - Shape����
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public FeatureLayer ShowShapeLayer(string _layerNm, bool chk)
        {
            try
            {
                // 0.�ش緹�̾� ��������
                string filterExp = "";
                string shapeNm = "";

                try
                {
                    string[] ary = _layerNm.Split('^');
                    shapeNm = ary[0]; //���̾����̺��
                    filterExp = "FTR_CDE='" + ary[1] + "'"; //����ǥ����
                }
                catch (Exception) { }



                //0.layers üũ����ȭ
                CmmObj.layers[_layerNm].chk = chk;


                // 1.���̾� ON
                if (chk)
                {
                    if (CmmObj.layers[_layerNm].FL.Name != "")
                    {
                        //���̾�ε� �����̸� �ʿ� �߰��� ����
                        mapControl.AddLayer(CmmObj.layers[_layerNm].FL);
                        sts.Push(_layerNm);//���̾��ε��� ������������ �Ȱ��� ���ÿ� �����س���
                    }
                    else
                    {
                        mapControl.AddShapeFile(BizUtil.GetDataFolder("shape"), shapeNm + ".shp");

                        //���̾ü ����
                        CmmObj.layers[_layerNm].FL = mapControl.get_Layer(0) as FeatureLayer; //�������̹Ƿ� �ε����� 0 
                        CmmObj.layers[_layerNm].FL.Name = CmmObj.getLayerKorNm(_layerNm);
                        sts.Push(_layerNm);//���̾��ε��� ������������ �Ȱ��� ���ÿ� �����س���


                        /* Renderer ���� */
                        IGeoFeatureLayer pGFL  = CmmObj.layers[_layerNm].FL as IGeoFeatureLayer;

                        if ("BML_GADM_AS".Equals(_layerNm)) //��������
                        {
                            //���νɺ�
                            ISimpleLineSymbol lineSymbol = new SimpleLineSymbol();
                            lineSymbol.Color = new RgbColor() { Red = 255, Green = 0, Blue = 0 };
                            lineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot;
                            lineSymbol.Width = 1.5;

                            //Fill�ɺ�
                            ISimpleFillSymbol fillSymbol = new SimpleFillSymbol();
                            fillSymbol.Color = new RgbColor() { Red = 255, Green = 255, Blue = 255 };
                            fillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;
                            
                            fillSymbol.Outline = lineSymbol;  //�ܰ����� ���νɺ��� ����

                            ISimpleRenderer pSR = pGFL.Renderer as SimpleRenderer;
                            pSR.Symbol = fillSymbol as ISymbol;

                            //�ѹݵ��������� �ִ°�츸 �����ϸ�
                            IEnvelope area = CmmObj.layers[_layerNm].FL.AreaOfInterest;
                            if (area.XMin > GisCmm.XMin && area.XMax < GisCmm.XMax
                                    && area.YMin > GisCmm.YMin && area.YMax < GisCmm.YMax)
                            {
                                mapControl.Extent = area;
                            }
                            //�ʱ����������̸� ����������
                            mapControl.MapScale = 288895;
                        }
                        else
                        {
                            //���̾��� UniqueValueRenderer ����
                            pGFL.Renderer = CmmObj.uniqueValueRendererObj as IFeatureRenderer;
                        }
                    }


                    // ���͸� �μ������� �����ü������� ����
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        //���̾����͸�
                        IFeatureLayerDefinition flayer = CmmObj.layers[_layerNm].FL as IFeatureLayerDefinition;
                        flayer.DefinitionExpression = filterExp;

                    }




                    return CmmObj.layers[_layerNm].FL;
                }
                // 2.���̾� OFF
                else
                {
                    if (CmmObj.layers[_layerNm].FL.Name != "")
                    {
                        mapControl.DeleteLayer(sts.GetStackIdx(_layerNm));
                        sts.Remove(sts.GetIdx(_layerNm));
                    }
                    else
                    {
                        //�ε����� �������� �ƹ��͵� ����
                    }
                    return null;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("���̾ �������� �ʽ��ϴ�." + ex.Message);
                return null;
            }
        }





        /// <summary>
        /// �ش�ü����� ��������ġ ã�ư���(����ȭ�鿡�� ȣ���) 
        /// </summary>
        /// <param name="FTR_CDE"></param>
        /// <param name="FTR_IDN"></param>
        /// <returns></returns>
        public async void findFtr(string FTR_CDE, string FTR_IDN)
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

            //��������
            string stat = await resetAction(null);

            //0.�ش緹�̾�ǥ�� - ���ο����ڵ����� �ε����� üũ��
            //FeatureLayer layer = ShowShapeLayer(GisCmm.GetLayerNm(FTR_CDE), true);

            //üũ�ڽ�UI�� Ŭ����Ű�� ������� ����
            FeatureLayer layer = await ClickLayer(FTR_CDE);



            //1.�ش緹�̾� ��������
            //FeatureLayer layer = CmmObj.layers[GisCmm.GetLayerNm(FTR_CDE)];



            //1.����Ʈó�� ���͸�
            IQueryFilter qfltr = new QueryFilter();
            qfltr.WhereClause = " FTR_CDE = '" + FTR_CDE + "' AND FTR_IDN = " + FTR_IDN + " ";

            IFeatureSelection fsel = layer as IFeatureSelection;
            fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);
            await Task.Delay(1000);


            //2.��ó��ü ���͸�
            IFeatureCursor cursor = layer.Search(qfltr, true);
            IFeature feature = cursor.NextFeature();

            ESRI.ArcGIS.Geometry.IPoint point = mapControl.ToMapPoint(Convert.ToInt32(GisCmm._hsCoords.X), Convert.ToInt32(GisCmm._hsCoords.Y));


            if (FTR_CDE == "SA001" || FTR_CDE == "SA002") //�������,�޼�����
            {
                ESRI.ArcGIS.Geometry.IPolyline line = (ESRI.ArcGIS.Geometry.IPolyline)feature.ShapeCopy;
                point = line.FromPoint;
                //point = mapControl.ToMapPoint(Convert.ToInt32((line.ToPoint.X + line.FromPoint.X) / 2), Convert.ToInt32((line.ToPoint.Y + line.FromPoint.Y) / 2));
            }
            else if (FTR_CDE == "SA113" || FTR_CDE == "BZ001" || FTR_CDE == "BZ002" || FTR_CDE == "BZ003") //������, ���
            {
                IArea area = (ESRI.ArcGIS.Geometry.IArea)feature.ShapeCopy;
                point = area.Centroid;
            }
            else //������ ����Ʈ �ü���
            {
                point = (ESRI.ArcGIS.Geometry.IPoint)feature.ShapeCopy;
            }


            //3. - Ÿ������ �����߽��̵�

            //IEnumFeature pFsel = (IEnumFeature)mapControl.Map.FeatureSelection;
            //pFsel.Reset(); // make sure it starts from the first feature
            //IFeature pFeat = pFsel.Next();
            //do
            //{
            //    ESRI.ArcGIS.Geometry.IGeometry pGeom = pFeat.ShapeCopy;
            //    if (pGeom.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
            //    {
            //        point = (ESRI.ArcGIS.Geometry.IPoint)pGeom;
            //        double x, y;
            //        point.QueryCoords(out x, out y); // use the coordinates from here
            //    }
            //    else if (pGeom.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            //    {
            //        ESRI.ArcGIS.Geometry.IPolyline line = (ESRI.ArcGIS.Geometry.IPolyline)pGeom;
            //        point = line.FromPoint;
            //    }
            //    else if (pGeom.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            //    {
            //        ESRI.ArcGIS.Geometry.IPolygon polygon = (ESRI.ArcGIS.Geometry.IPolygon)pGeom;
            //        point = polygon.FromPoint;
            //    }

            //    pFeat = pFsel.Next();
            //} while (pFeat != null);



            /*
            */
            IEnvelope envelope = mapControl.ActiveView.Extent;
            envelope.CenterAt(point);

            //int x = Convert.ToInt32(point.X);
            //int y = Convert.ToInt32(point.Y);
            //mapControl.ToMapPoint(x, y);
            //mapControl.ActiveView.ScreenDisplay.UpdateWindow();

            mapControl.ActiveView.Extent = envelope;
            mapControl.Refresh();
            mapControl.MapScale = 36112; // 18056; // 9028;

        }

        //������ üũ�ڽ����̾� Ŭ��
        private async Task<FeatureLayer> ClickLayer(string FTR_CDE)
        {
            foreach (Button btn in FmsUtil.FindVisualChildren<Button>(mapArcObjView))
            {
                try
                {
                    if (btn.Tag.ToString() == GisCmm.GetLayerNm(FTR_CDE))
                    {
                        CheckBox chkbox = btn.Template.FindName("chkLayer", btn) as CheckBox;
                        chkbox.IsChecked = true;
                        btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));//����Ŭ��
                        break;
                    }
                }
                catch (Exception) { }
            }
            await Task.Delay(1000);
            return CmmObj.layers[GisCmm.GetLayerNm(FTR_CDE)].FL;
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











        public class FmsStack<T>
        {
            private List<T> items = new List<T>();

            public void Push(T item)
            {
                items.Add(item);
            }
            public T Pop()
            {
                if (items.Count > 0)
                {
                    T temp = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);
                    return temp;
                }
                else
                    return default(T);
            }
            public void Remove(int itemAtPosition)
            {
                items.RemoveAt(itemAtPosition);
            }


            //������ �ε��� ��������
            public int GetIdx(T layerNm)
            {
                return items.IndexOf(layerNm);
            }

            //������ �ε��� ��������
            public int GetStackIdx(T layerNm)
            {
                List<T> rev = items.Reverse<T>().ToList<T>();
                return rev.IndexOf(layerNm);
            }

            //�����ʱ�ȭ
            public void Clear()
            {
                items.Clear();
            }


        }











    }
}

