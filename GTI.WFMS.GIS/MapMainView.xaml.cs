using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;

using System;
using System.IO;
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


            Initialize_LocalServer();
        }












        #region ============ LocalServer 관련부분 ==============

        // Hold a reference to the local feature service; the ServiceFeatureTable will be loaded from this service
        private LocalFeatureService _localFeatureService;

        private async void Initialize_LocalServer()
        {

            try
            {
                // LocalServer must not be running when setting the data path.
                if (LocalServer.Instance.Status == LocalServerStatus.Started)
                {
                    await LocalServer.Instance.StopAsync();
                }

                // Set the local data path - must be done before starting. On most systems, this will be C:\EsriSamples\AppData.
                // This path should be kept short to avoid Windows path length limitations.
                string tempDataPathRoot = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Windows)).FullName;
                string tempDataPath = Path.Combine(tempDataPathRoot, "EsriSamples", "AppData");
                Directory.CreateDirectory(tempDataPath); // CreateDirectory won't overwrite if it already exists.
                LocalServer.Instance.AppDataPath = tempDataPath;

                // Start the local server instance
                await LocalServer.Instance.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Please ensure that local server is installed prior to using the sample. See instructions in readme.md. Message: {0}", ex.Message), "Local Server failed to start");
                return;
            }

            // Load the sample data and get the path
            string myfeatureServicePath = GetFeatureLayerPath();

            // Create the feature service to serve the local data
            _localFeatureService = new LocalFeatureService(myfeatureServicePath);

            // Listen to feature service status changes
            _localFeatureService.StatusChanged += _localFeatureService_StatusChanged;

            // Start the feature service
            try
            {
                await _localFeatureService.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "The feature service failed to load");
            }
        }

        private async void _localFeatureService_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Load the map from the service once ready
            if (e.Status == LocalServerStatus.Started)
            {
                // Get the path to the first layer - the local feature service url + layer ID
                string layerUrl = _localFeatureService.Url + "/0";

                // Create the ServiceFeatureTable
                ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(new Uri(layerUrl));

                // Create the Feature Layer from the table
                FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);

                // Add the layer to the map
                mapView.Map.OperationalLayers.Add(myFeatureLayer);

                try
                {
                    // Wait for the layer to load
                    await myFeatureLayer.LoadAsync();

                    // Set the viewpoint on the MapView to show the layer data
                    await mapView.SetViewpointGeometryAsync(myFeatureLayer.FullExtent, 50);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
        }

        private static string GetFeatureLayerPath()
        {
            //return DataManager.GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
            return GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
        }





        /// <summary>
        /// Gets the data folder where locally provisioned data is stored.
        /// </summary>
        internal static string GetDataFolder()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string sampleDataFolder = Path.Combine(appDataFolder, "ArcGISRuntimeSampleData");

            if (!Directory.Exists(sampleDataFolder)) { Directory.CreateDirectory(sampleDataFolder); }

            return sampleDataFolder;
        }

        /// <summary>
        /// Gets the path to an item on disk. 
        /// The item must have already been downloaded for the path to be valid.
        /// </summary>
        /// <param name="itemId">ID of the portal item.</param>
        internal static string GetDataFolder(string itemId)
        {
            return Path.Combine(GetDataFolder(), itemId);
        }

        /// <summary>
        /// Gets the path to an item on disk. 
        /// The item must have already been downloaded for the path to be valid.
        /// </summary>
        /// <param name="itemId">ID of the portal item.</param>
        /// <param name="pathParts">Components of the path.</param>
        internal static string GetDataFolder(string itemId, params string[] pathParts)
        {
            return Path.Combine(GetDataFolder(itemId), Path.Combine(pathParts));
        }

        #endregion

    }
}
