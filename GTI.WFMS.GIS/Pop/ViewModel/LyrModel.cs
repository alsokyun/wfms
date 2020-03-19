using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    /// <summary>
    /// MapViewModel 에서 레이어관련 항목 및 처리
    /// </summary>
    public class LyrModel
    {
        //생성자
        public LyrModel()
        {
            // ArcGIS LocalServer start..
            //Initialize_LocalServer();
        }


        public Map _map = new Map(SpatialReference.Create(3857));
        //private Map _map = new Map(Basemap.CreateStreets());
        //private Map _map = new Map(Basemap.CreateNationalGeographic());
        //private Map _map = new Map(Basemap.CreateImageryWithLabels());
        //private Map _map = new Map(SpatialReference.Create(5181));
        //private Map _map = new Map(SpatialReference.Create(3857)) { MinScale = 7000000.0 };

        public MapView mapView; //뷰의 MapView


        /* 
         * 레이어객체리스트
        public FeatureLayer BML_GADM_AS = new FeatureLayer();//울산광역행정구역
        public FeatureLayer WTL_FLOW_PS = new FeatureLayer();
        public FeatureLayer WTL_FIRE_PS = new FeatureLayer();
         */

        public Dictionary<string, FeatureLayer> layers = new Dictionary<string, FeatureLayer>()
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











        #region =========== shape 레이어 구성부분 ==============

        /// <summary>
        /// Shape 레이어 보이기/끄기 - Shape버전
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public async void ShowShapeLayerFilter(MapView _mapView, string _layerNm, bool chk, string _FTR_IDN)
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
                catch (Exception ) { }
                
                //FTR_IDN 필터추가
                if (!FmsUtil.IsNull(_FTR_IDN))
                {
                    if (FmsUtil.IsNull(filterExp))
                    {
                        filterExp += "FTR_IDN = " + _FTR_IDN;
                    }
                    else
                    {
                        filterExp += " AND FTR_IDN = " + _FTR_IDN;
                    }
                }


                FeatureLayer layer = layers[_layerNm];
                //Type memberType = this.GetType();



                // 1.레이어 ON
                if (chk)
                {
                    // 필터링 인수있으면 하위시설물(관리번호, 전체)으로 필터
                    layer.DefinitionExpression = filterExp;


                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        //on상태 아무것도 안함
                    }
                    else
                    {
                        if (layer != null && layer.LoadStatus == LoadStatus.Loaded) //레이어객체 있으면 단순추가
                        {
                            _mapView.Map.OperationalLayers.Add(layer);
                        }
                        else //레이어객체 없으면 Shape 로딩
                        {
                            string shapefilePath = Path.Combine(BizUtil.GetDataFolder("shape", shapeNm + ".shp"));
                            try
                            {
                                ShapefileFeatureTable layerTable = await ShapefileFeatureTable.OpenAsync(shapefilePath);


                                layer = new FeatureLayer(layerTable); /////// 신규레이어 생성 /////// 
                                layer.DefinitionExpression = filterExp;// 필터링 인수있으면 하위시설물(관리번호, 전체)으로 필터

                                layers[_layerNm] = layer; /////// 딕셔너리에 자동으로 저장되지는 않을것임 /////// 


                                layer.Renderer = GisCmm.uniqueValueRenderer.Clone(); //렌더러는 레이어 각각 할당해야하므로 렌더러복사하여 할당
                                _mapView.Map.OperationalLayers.Add(layer);

                            }
                            catch (Exception e)
                            {
                                Messages.ShowErrMsgBox(e.Message);
                            }
                        }
                    }

                    // Zoom the map to the extent of the shapefile.
                    //await _mapView.SetViewpointGeometryAsync(layer.FullExtent, 50);

                }
                // 2.레이어 OFF
                else
                {
                    if (_mapView.Map.OperationalLayers.Contains(layer))
                    {
                        _mapView.Map.OperationalLayers.Remove(layer);
                    }
                    else
                    {
                        //off상태 아무것도 안함
                    }

                    // 필터링 인수있으면 하위시설물으로 필터 리셋
                    layer.DefinitionExpression = "";
                    
                }




            }
            catch (Exception )
            {
                MessageBox.Show("레이어가 존재하지 않습니다.");
            }
        }







        #endregion








    }

}

