using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// MapView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapMainView : UserControl
    {
        public MapMainView()
        {
            InitializeComponent();

            //InitMap();//지도초기화

            //AddLayer();//레이어추가
        }

        // Coordinates for Ulsan
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181
        private MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857

        //private double _ulsanScale = 8762.7156655228955;
        private double _ulsanScale = 20000;


        private FeatureLayer L_FLOW_PS;







        private async void InitMap()
        {
            await mapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);
            //await mapView.SetViewpointScaleAsync(_ulsanScale); 


            //맵뷰 클릭이벤트 설정
            mapView.GeoViewTapped += handlerGeoViewTapped;
        }

        //맵뷰 클릭이벤트 핸들러
        private async void handlerGeoViewTapped(object sender, GeoViewInputEventArgs e)
        {
            // Perform the identify operation.
            IdentifyLayerResult myIdentifyResult = await mapView.IdentifyLayerAsync(L_FLOW_PS, e.Position, 20, false);

            // Return if there's nothing to show.
            if (!myIdentifyResult.GeoElements.Any())
            {
                return;
            }

            Feature identifiedFeature = (Feature)myIdentifyResult.GeoElements[0];
            string FTR_CDE = identifiedFeature.GetAttributeValue("FTR_CDE").ToString();

        }






        private async void AddLayer()
        {
            string shapefilePath = Path.Combine("D:\\DEVGTI\\layer", "WTL_FLOW_PS.shp");

            try
            {
                // Create a shapefile feature table using the path. 
                ShapefileFeatureTable WTL_FLOW_PS = await ShapefileFeatureTable.OpenAsync(shapefilePath);

                // Create a feature layer from the table and add it to the map's operational layers. 
                L_FLOW_PS = new FeatureLayer(WTL_FLOW_PS);
                mapView.Map.OperationalLayers.Add(L_FLOW_PS);

                //마커 스타일링 
                var symbolImgUri = new Uri("file:///D:/DEVGTI/style_img/SA117.gif");
                PictureMarkerSymbol markerSymbol = new PictureMarkerSymbol(symbolImgUri);
                SimpleRenderer simpleRenderer = new SimpleRenderer(markerSymbol);
                L_FLOW_PS.Renderer = simpleRenderer;

                //_map.InitialViewpoint = new Viewpoint(L_FLOW_PS.FullExtent);
            }
            catch (Exception e)
            {
                //throw e;
            }

        }
    }
}
