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

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapArcObjViewModel : INotifyPropertyChanged
    {


        #region ========== Members 정의 ==========

        // 선택한 피처 - 서버연계된Feature  
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();

        WindowsFormsHost mapHost;
        WindowsFormsHost toolbarHost;
        TreeView treeLayer;

        AxMapControl mapControl;
        AxToolbarControl toolbarControl;


        public event PropertyChangedEventHandler PropertyChanged;

        private Popup popFct = new Popup(); //시설물정보DIV

        public FmsStack<string> sts = new FmsStack<string>();


        public Dictionary<string, FeatureLayer> layers;
        public void initLayers()
        {
            layers = new Dictionary<string, FeatureLayer>()
            {
                {"WTL_FLOW_PS",  new FeatureLayer()},
                {"WTL_FIRE_PS^SA118",  new FeatureLayer()},
                {"WTL_FIRE_PS^SA119",  new FeatureLayer()},
                {"WTL_GAIN_PS",  new FeatureLayer()},
                {"WTL_HEAD_PS",  new FeatureLayer()},
                {"WTL_LEAK_PS",  new FeatureLayer()},
                {"WTL_MANH_PS",  new FeatureLayer()},
                {"WTL_META_PS",  new FeatureLayer()},
                {"WTL_PRES_PS",  new FeatureLayer()},
                {"WTL_PRGA_PS",  new FeatureLayer()},
                {"WTL_RSRV_PS",  new FeatureLayer()},
                {"WTL_SERV_PS",  new FeatureLayer()},
                {"WTL_STPI_PS",  new FeatureLayer()},
                {"WTL_VALV_PS^SA200",  new FeatureLayer()},
                {"WTL_VALV_PS^SA201",  new FeatureLayer()},
                {"WTL_VALV_PS^SA202",  new FeatureLayer()},
                {"WTL_VALV_PS^SA203",  new FeatureLayer()},
                {"WTL_VALV_PS^SA204",  new FeatureLayer()},
                {"WTL_VALV_PS^SA205",  new FeatureLayer()},
                {"WTL_VALV_PS^SA206",  new FeatureLayer()},


                {"BML_GADM_AS",  new FeatureLayer()},
                {"WTL_PURI_AS",  new FeatureLayer()},

                {"WTL_PIPE_LM",  new FeatureLayer()},
                {"WTL_SPLY_LS",  new FeatureLayer()},
            };

        }



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


        public RelayCommand<object> loadedCmd { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }



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

        #endregion





        #region ========== 생성자 ==========

        public MapArcObjViewModel()
        {

            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {
                //뷰객체를 파라미터로 전달받기
                UserControl mapArcObjView = obj as UserControl;

                mapHost = mapArcObjView.FindName("mapHost") as WindowsFormsHost;
                toolbarHost = mapArcObjView.FindName("toolbarHost") as WindowsFormsHost;
                treeLayer = mapArcObjView.FindName("treeLayer") as TreeView;

                mapControl = mapHost.Child as AxMapControl;
                toolbarControl = toolbarHost.Child as AxToolbarControl;

                
                //0.맵 초기화
                initMap();

                //1.레이어초기화
                initLayers();

                //2.UniqueRenderer 초기화
                GisCm.InitUniqueValueRenderer();

                //3.울산행정구역표시
                ShowShapeLayer("BML_GADM_AS", true);


                
                //비트맵초기화(시설물상세DIV 아이콘)
                BitImg = new BitmapImage();
            });


            //레이어 ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;

                ShowShapeLayer( doc.Tag.ToString(), chk);


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
            resetCmd = new RelayCommand<object>(resetAction);

        }




        #endregion


















        #region ============= 내부함수 ============= 


        public void initMap()
        {
            //배경지도는 mxd파일로부터
            string mxdfilePath = Path.Combine(BizUtil.GetDataFolder("shape", "also.mxd"));
            mapControl.LoadMxFile(mxdfilePath);

        }


        private void resetAction(object obj)
        {
            //0.맵의레이어 클리어
            mapControl.ClearLayers();

            //1.맵초기화
            initMap();

            //2.레이어초기화
            initLayers();

            //3.레이어스택 초기화
            sts.Clear();

            //4.울산행정구역표시
            ShowShapeLayer("BML_GADM_AS", true);




            //열여있는 시설물정보창 닫기
            popFct.IsOpen = false;

            //레이어div 체크해제
            foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
            {
                cb.IsChecked = false;
            }
        }





        /// <summary>
        /// Shape 레이어 보이기/끄기 - Shape버전
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public void ShowShapeLayer(string _layerNm, bool chk)
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


                



                // 1.레이어 ON
                if (chk)
                {
                    if (layers[_layerNm].Name != "")
                    {
                        //레이어로딩 상태이면 맵에 추가만 해줌
                        mapControl.AddLayer(layers[_layerNm]);
                        sts.Push(_layerNm);//레이어인덱스 가져오기위해 똑같이 스택에 저장해놓음
                    }
                    else
                    {
                        mapControl.AddShapeFile(BizUtil.GetDataFolder("shape"), shapeNm + ".shp");

                        //레이어객체 저장
                        layers[_layerNm] = mapControl.get_Layer(0) as FeatureLayer; //스택형이므로 인덱스는 0 
                        layers[_layerNm].Name = GisCm.getLayerKorNm(_layerNm);
                        sts.Push(_layerNm);//레이어인덱스 가져오기위해 똑같이 스택에 저장해놓음


                        /* Renderer 적용 */
                        IGeoFeatureLayer pGFL  = layers[_layerNm] as IGeoFeatureLayer;

                        if ("BML_GADM_AS".Equals(_layerNm)) //울산행정구역
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
                        }
                        else
                        {
                            //레이어의 UniqueValueRenderer 적용
                            pGFL.Renderer = GisCm.uniqueValueRenderer as IFeatureRenderer;
                        }
                    }


                    // 필터링 인수있으면 하위시설물으로 필터
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        //레이어필터링
                        IFeatureLayerDefinition flayer = layers[_layerNm] as IFeatureLayerDefinition;
                        flayer.DefinitionExpression = filterExp;

                    }

                    //mapControl.Extent = layers[_layerNm].AreaOfInterest;
                }
                // 2.레이어 OFF
                else
                {
                    if (layers[_layerNm].Name != "")
                    {
                        mapControl.DeleteLayer(sts.GetStackIdx(_layerNm));
                        sts.Remove(sts.GetIdx(_layerNm));
                    }
                    else
                    {
                        //로딩되지 않은상태 아무것도 안함
                    }
                }



                
            }
            catch (Exception)
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }





        /// <summary>
        /// 해당시설물의 지도상위치 찾아가기(업무화면에서 호출됨) 
        /// </summary>
        /// <param name="FTR_CDE"></param>
        /// <param name="FTR_IDN"></param>
        /// <returns></returns>
        public void findFtr(string FTR_CDE, string FTR_IDN)
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
            resetAction(null);

            //0.해당레이어표시 - 내부에서자동으로 로딩여부 체크함
            ShowShapeLayer(GisCmm.GetLayerNm(FTR_CDE), true);

            //1.해당레이어 가져오기
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];




            //1.셀렉트처리 필터링
            IQueryFilter qfltr = new QueryFilter();
            qfltr.WhereClause = " FTR_CDE = '" + FTR_CDE + "' AND FTR_IDN = " + FTR_IDN; 

            IFeatureSelection fsel = layer as IFeatureSelection;
            fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);

            //2.피처객체 필터링 - 타겟으로 지도중심이동
            IFeatureCursor cursor = layer.Search(qfltr, false);
            IFeature feature = cursor.NextFeature(); 

            ESRI.ArcGIS.Geometry.IPoint point;

            if (FTR_CDE == "SA001" || FTR_CDE == "SA002") //상수관로,급수관로
            {
                ESRI.ArcGIS.Geometry.IPolyline line = (ESRI.ArcGIS.Geometry.IPolyline)feature.ShapeCopy;
                point = line.FromPoint;
            }
            else if(FTR_CDE == "SA113") //정수장
            {
                ESRI.ArcGIS.Geometry.IPolygon polygon = (ESRI.ArcGIS.Geometry.IPolygon)feature.ShapeCopy;
                point = polygon.FromPoint;
            }
            else //나머지 포이트 시설물
            {
                point = (ESRI.ArcGIS.Geometry.IPoint)feature.ShapeCopy;
            }


            var envelope = mapControl.ActiveView.Extent;
            envelope.CenterAt(point);

            mapControl.Extent = envelope;
            mapControl.Refresh();

            //int x = Convert.ToInt32(point.X);
            //int y = Convert.ToInt32(point.Y);
            //mapControl.ToMapPoint(x, y);
            //mapControl.ActiveView.ScreenDisplay.UpdateWindow();

            mapControl.MapScale = 100000;
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

