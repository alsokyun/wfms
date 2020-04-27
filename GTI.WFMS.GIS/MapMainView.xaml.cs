using GTI.WFMS.GIS.Pop.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
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


            //StartLocalMapService();
        }












        /*
                private async void StartLocalMapService()
                {
                    string path = GetDataFolder("ea619b4f0f8f4d108c5b87e90c1b5be0");
                    string filename = "mjrroads.shp";
                    // Start a service from the blank MPK
                    string mapServiceUrl = GetDataFolder("ea619b4f0f8f4d108c5b87e90c1b5be0", "mpk_blank.mpk");

                    // Create the local map service
                    _localMapService = new LocalMapService(mapServiceUrl);

                    // Create the shapefile workspace
                    ShapefileWorkspace shapefileWorkspace = new ShapefileWorkspace("shp_wkspc", path);

                    // Create the layer source that represents the shapefile on disk
                    TableSublayerSource source = new TableSublayerSource(shapefileWorkspace.Id, filename);

                    // Create a sublayer instance from the table source
                    _shapefileSublayer = new ArcGISMapImageSublayer(0, source);

                    // Add the dynamic workspace to the map service
                    _localMapService.SetDynamicWorkspaces(new List<DynamicWorkspace>() { shapefileWorkspace });

                    // Subscribe to notifications about service status changes
                    _localMapService.StatusChanged += _localMapService_StatusChanged;

                    try
                    {
                        // Start the map service
                        await _localMapService.StartAsync();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString(), "Error");
                    }
                }


                private async void _localMapService_StatusChanged(object sender, StatusChangedEventArgs e)
                {
                    // Add the shapefile layer to the map once the service finishes starting
                    if (e.Status == LocalServerStatus.Started)
                    {
                        // Create the imagery layer
                        ArcGISMapImageLayer imageryLayer = new ArcGISMapImageLayer(_localMapService.Url);

                        // Subscribe to image layer load status change events
                        // Only set up the sublayer renderer for the shapefile after the parent layer has finished loading
                        imageryLayer.LoadStatusChanged += (q, ex) =>
                        {
                            // Add the layer to the map once loaded
                            if (ex.Status == Esri.ArcGISRuntime.LoadStatus.Loaded)
                            {
                                // Create a default symbol style
                                SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 3);

                                // Apply the symbol style with a renderer
                                _shapefileSublayer.Renderer = new SimpleRenderer(lineSymbol);

                                // Add the shapefile sublayer to the imagery layer
                                imageryLayer.Sublayers.Add(_shapefileSublayer);
                            }
                        };

                        try
                        {
                            // Load the layer
                            await imageryLayer.LoadAsync();

                            // Clear any existing layers
                            mapView.Map.OperationalLayers.Clear();

                            // Add the image layer to the map
                            mapView.Map.OperationalLayers.Add(imageryLayer);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error");
                        }
                    }
                }


        */


    }
}
