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


        #region ========== Members 정의 ==========

        // 선택한 피처 - 서버연계된Feature  
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();

        UserControl mapArcObjView;
        WindowsFormsHost mapHost;
        WindowsFormsHost toolbarHost;
        TreeView treeLayer;
        AxMapControl mapControl;
        AxToolbarControl toolbarControl;

        public event PropertyChangedEventHandler PropertyChanged;
        private Popup popFct = new Popup(); //시설물정보DIV
        public FmsStack<string> sts = new FmsStack<string>();

        






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
        public virtual ObservableCollection<FileInfo> ItemsFile { get; set; } //파일객체


        public RelayCommand<object> loadedCmd { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }
        public RelayCommand<object> importCmd { get; set; }
        

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
        //체크박스 데이터
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





        #region ========== 생성자 ==========

        public MapArcObjViewModel()
        {

            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {
                //뷰객체를 파라미터로 전달받기
                mapArcObjView = obj as UserControl;

                mapHost = mapArcObjView.FindName("mapHost") as WindowsFormsHost;
                toolbarHost = mapArcObjView.FindName("toolbarHost") as WindowsFormsHost;
                treeLayer = mapArcObjView.FindName("treeLayer") as TreeView;

                mapControl = mapHost.Child as AxMapControl;
                toolbarControl = toolbarHost.Child as AxToolbarControl;

                
                //0.맵 초기화
                initMap();

                //1.레이어초기화
                CmmObj.initLayers();

                //2.UniqueRenderer 초기화
                CmmObj.InitUniqueValueRendererObj();

                //3.행정구역경계표시
                ShowShapeLayer("BML_GADM_AS", true );


                // 파일인포리스트
                ItemsFile = new ObservableCollection<FileInfo>();

                //비트맵초기화(시설물상세DIV 아이콘)
                BitImg = new BitmapImage();

                //맵마우스클릭 이벤트설정
                mapControl.OnMouseUp += OnMouseClick;
            });


            //레이어 ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;


                //ShowShapeLayer( doc.Tag.ToString(), chk);
                ShowShapeLayer(doc.Tag.ToString(), chk);


                //선택된 레이어저장
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


            //GIS초기화
            resetCmd = new RelayCommand<object>(async delegate(object obj) {

                string stat = await resetAction(obj);
            });

            //SHP파일관리창
            importCmd = new RelayCommand<object>(async delegate(object obj) {

                ShpMngView view = new ShpMngView();
                if (view.ShowDialog() is bool)
                {
                    //재조회
                    string stat = await resetAction(null);
                }


            });

            
        }
        #endregion






        //마우스 클릭 이벤트핸들러
        private void OnMouseClick(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //IPoint pQueryPoint = new PointClass();
            IPoint pQueryPoint = mapControl.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
            var x = pQueryPoint.X;
            var y = pQueryPoint.Y;

            //클릭범위보정
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

            /// 활성화된 레이어에 대해서, 해당 클릭 영역에 존재하는 피쳐팝업을 띄운다
            //FeatureLayer layer = CmmObj.layers[GisCmm.GetLayerNm("SA117")].FL;
            foreach (KeyValuePair<string, FmsFeature> item in CmmObj.layers)
            {
                if (item.Key == "BML_GADM_AS") continue; //행정구역레이어는 제외

                if (item.Value.chk)
                {
                    findFtrByRegion(item.Value.FL, buffer);
                }
            }


        }

        //해당 영역에 존재하는 레이어 피처찾아서 활성화, 팝업창
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

                //피처의 시설물코드,관리번호 알아내기
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
                    MessageBox.Show("시설물정보가 없습니다.");
                    return;
                }




                //1.셀렉트처리 필터링
                IQueryFilter qfltr = new QueryFilter();
                qfltr.WhereClause = " FTR_CDE = '" + ftr_cde + "' AND FTR_IDN = " + ftr_idn;
                IFeatureSelection fsel = layer as IFeatureSelection;
                fsel.Clear();
                fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);
                mapControl.Refresh();


                //0.시설물팝업호출
                int ftr_cde_len = 0;
                try
                {
                    ftr_cde_len = (Convert.ToString(ftr_cde) as string).Length ;
                }
                catch (Exception){}

                if (ftr_cde_len == 5 && !FmsUtil.IsNull(ftr_idn)) //시설물코드,번호가 유효한경우만
                {
                    FtrPopView ftrPopView = new FtrPopView(ftr_cde, ftr_idn);
                    try
                    {
                        ftrPopView.lbTitle.Content = BizUtil.GetCodeNm("Select_FTR_LIST2", ftr_cde);
                    }
                    catch (Exception)
                    {
                        ftrPopView.lbTitle.Content = "시설물정보";
                    }
                    if (ftrPopView.ShowDialog() is bool)
                    {
                        //재조회
                        fsel.Clear();
                        mapControl.Refresh();
                    }
                }
                break;
                //pFeat = pDestCur.NextFeature(); //첫번째 걸린 시설물만 팝업띄우자
            }
        }







        #region ============= 내부함수 ============= 


        public void initMap()
        {
            //배경지도는 mxd파일로부터
            //string mxdfilePath = Path.Combine(BizUtil.GetDataFolder("tile", "also.mxd"));
            string mxdfilePath = FmsUtil.mdxDir +  @"\also.mxd";
            mapControl.LoadMxFile(mxdfilePath);

            
            mapControl.OnExtentUpdated += OnExtentUpdatedHandlelr;

        }
        //영역변경이벤트 처리 
        private void OnExtentUpdatedHandlelr(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 지도맵,레이어 초기화
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<string> resetAction(object obj)
        {
            //0.맵의레이어 클리어
            mapControl.ClearLayers();
            mapControl.Refresh();


            //1.맵초기화
            initMap();

            //2.레이어초기화
            CmmObj.initLayers();

            //3.레이어스택 초기화
            sts.Clear();

            //4.행정구역표시
            ShowShapeLayer("BML_GADM_AS", true );




            //열여있는 시설물정보창 닫기
            popFct.IsOpen = false;

            //레이어div 체크해제
            foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
            {
                cb.IsChecked = false;
            }
            await Task.Delay(1000);
            return "ok";
        }





        /// <summary>
        /// Shape 레이어 보이기/끄기 - Shape버전
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public FeatureLayer ShowShapeLayer(string _layerNm, bool chk)
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



                //0.layers 체크동기화
                CmmObj.layers[_layerNm].chk = chk;


                // 1.레이어 ON
                if (chk)
                {
                    if (CmmObj.layers[_layerNm].FL.Name != "")
                    {
                        //레이어로딩 상태이면 맵에 추가만 해줌
                        mapControl.AddLayer(CmmObj.layers[_layerNm].FL);
                        sts.Push(_layerNm);//레이어인덱스 가져오기위해 똑같이 스택에 저장해놓음
                    }
                    else
                    {
                        mapControl.AddShapeFile(BizUtil.GetDataFolder("shape"), shapeNm + ".shp");

                        //레이어객체 저장
                        CmmObj.layers[_layerNm].FL = mapControl.get_Layer(0) as FeatureLayer; //스택형이므로 인덱스는 0 
                        CmmObj.layers[_layerNm].FL.Name = CmmObj.getLayerKorNm(_layerNm);
                        sts.Push(_layerNm);//레이어인덱스 가져오기위해 똑같이 스택에 저장해놓음


                        /* Renderer 적용 */
                        IGeoFeatureLayer pGFL  = CmmObj.layers[_layerNm].FL as IGeoFeatureLayer;

                        if ("BML_GADM_AS".Equals(_layerNm)) //행정구역
                        {
                            //라인심볼
                            ISimpleLineSymbol lineSymbol = new SimpleLineSymbol();
                            lineSymbol.Color = new RgbColor() { Red = 255, Green = 0, Blue = 0 };
                            lineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot;
                            lineSymbol.Width = 1.5;

                            //Fill심볼
                            ISimpleFillSymbol fillSymbol = new SimpleFillSymbol();
                            fillSymbol.Color = new RgbColor() { Red = 255, Green = 255, Blue = 255 };
                            fillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;
                            
                            fillSymbol.Outline = lineSymbol;  //외각선은 라인심볼로 지정

                            ISimpleRenderer pSR = pGFL.Renderer as SimpleRenderer;
                            pSR.Symbol = fillSymbol as ISymbol;

                            //한반도영역내에 있는경우만 스케일링
                            IEnvelope area = CmmObj.layers[_layerNm].FL.AreaOfInterest;
                            if (area.XMin > GisCmm.XMin && area.XMax < GisCmm.XMax
                                    && area.YMin > GisCmm.YMin && area.YMax < GisCmm.YMax)
                            {
                                mapControl.Extent = area;
                            }
                            //초기행정구역이면 스케일조정
                            mapControl.MapScale = 288895;
                        }
                        else
                        {
                            //레이어의 UniqueValueRenderer 적용
                            pGFL.Renderer = CmmObj.uniqueValueRendererObj as IFeatureRenderer;
                        }
                    }


                    // 필터링 인수있으면 하위시설물으로 필터
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        //레이어필터링
                        IFeatureLayerDefinition flayer = CmmObj.layers[_layerNm].FL as IFeatureLayerDefinition;
                        flayer.DefinitionExpression = filterExp;

                    }




                    return CmmObj.layers[_layerNm].FL;
                }
                // 2.레이어 OFF
                else
                {
                    if (CmmObj.layers[_layerNm].FL.Name != "")
                    {
                        mapControl.DeleteLayer(sts.GetStackIdx(_layerNm));
                        sts.Remove(sts.GetIdx(_layerNm));
                    }
                    else
                    {
                        //로딩되지 않은상태 아무것도 안함
                    }
                    return null;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("레이어가 존재하지 않습니다." + ex.Message);
                return null;
            }
        }





        /// <summary>
        /// 해당시설물의 지도상위치 찾아가기(업무화면에서 호출됨) 
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
                    MessageBox.Show("잘못된 레이어입니다.");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("잘못된 레이어입니다.");
                return;
            }

            //지도리셋
            string stat = await resetAction(null);

            //0.해당레이어표시 - 내부에서자동으로 로딩여부 체크함
            //FeatureLayer layer = ShowShapeLayer(GisCmm.GetLayerNm(FTR_CDE), true);

            //체크박스UI를 클릭시키는 방식으로 변경
            FeatureLayer layer = await ClickLayer(FTR_CDE);



            //1.해당레이어 가져오기
            //FeatureLayer layer = CmmObj.layers[GisCmm.GetLayerNm(FTR_CDE)];



            //1.셀렉트처리 필터링
            IQueryFilter qfltr = new QueryFilter();
            qfltr.WhereClause = " FTR_CDE = '" + FTR_CDE + "' AND FTR_IDN = " + FTR_IDN + " ";

            IFeatureSelection fsel = layer as IFeatureSelection;
            fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);
            await Task.Delay(1000);


            //2.피처객체 필터링
            IFeatureCursor cursor = layer.Search(qfltr, true);
            IFeature feature = cursor.NextFeature();

            ESRI.ArcGIS.Geometry.IPoint point = mapControl.ToMapPoint(Convert.ToInt32(GisCmm._hsCoords.X), Convert.ToInt32(GisCmm._hsCoords.Y));


            if (FTR_CDE == "SA001" || FTR_CDE == "SA002") //상수관로,급수관로
            {
                ESRI.ArcGIS.Geometry.IPolyline line = (ESRI.ArcGIS.Geometry.IPolyline)feature.ShapeCopy;
                point = line.FromPoint;
                //point = mapControl.ToMapPoint(Convert.ToInt32((line.ToPoint.X + line.FromPoint.X) / 2), Convert.ToInt32((line.ToPoint.Y + line.FromPoint.Y) / 2));
            }
            else if (FTR_CDE == "SA113" || FTR_CDE == "BZ001" || FTR_CDE == "BZ002" || FTR_CDE == "BZ003") //정수장, 블록
            {
                IArea area = (ESRI.ArcGIS.Geometry.IArea)feature.ShapeCopy;
                point = area.Centroid;
            }
            else //나머지 포이트 시설물
            {
                point = (ESRI.ArcGIS.Geometry.IPoint)feature.ShapeCopy;
            }


            //3. - 타겟으로 지도중심이동

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

        //강제로 체크박스레이어 클릭
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
                        btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));//강제클릭
                        break;
                    }
                }
                catch (Exception) { }
            }
            await Task.Delay(1000);
            return CmmObj.layers[GisCmm.GetLayerNm(FTR_CDE)].FL;
        }






        // 시설물정보팝업 보이기
        private void ShowFtrPop(string fTR_CDE, string fTR_IDN, GeoViewInputEventArgs e)
        {
            //팝업열기 & 위치
            popFct.IsOpen = false;

            popFct = new FTR_POP(fTR_CDE, fTR_IDN);

            //double y0 = e.Position.Y - 700; //위쪽으로 표시함
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


            //스택형 인덱스 가져오기
            public int GetIdx(T layerNm)
            {
                return items.IndexOf(layerNm);
            }

            //스택형 인덱스 가져오기
            public int GetStackIdx(T layerNm)
            {
                List<T> rev = items.Reverse<T>().ToList<T>();
                return rev.IndexOf(layerNm);
            }

            //스택초기화
            public void Clear()
            {
                items.Clear();
            }


        }











    }
}

