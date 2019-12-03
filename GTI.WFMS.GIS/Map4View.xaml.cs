using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SelectionMode = Esri.ArcGISRuntime.Mapping.SelectionMode;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// FeatureService로부터 로컬에 Geodatabase를 생성하고, 피처를  수정한후  Geodatabased와 FeatureService에 동기화처리
    ///  - 프로그레스바
    ///  - EditState 개념
    ///  - TileCache tpk 맵구성
    ///  - GraphicsOverlay 맵에추가
    /// </summary>
    public partial class Map4View : UserControl
    {

        // Enumeration to track which phase of the workflow the sample is in.
        private enum EditState
        {
            NotReady, // Geodatabase has not yet been generated.
            Editing, // A feature is in the process of being moved.
            Ready // The geodatabase is ready for synchronization or further edits.
        }

        // URI for a feature service that supports geodatabase generation.
        private Uri _featureServiceUri;

        // Path to the geodatabase file on disk.
        private string _gdbPath;

        // Task to be used for generating the geodatabase.
        private GeodatabaseSyncTask _gdbSyncTask;

        // Flag to indicate which stage of the edit process we're in.
        private EditState _readyForEdits = EditState.NotReady;

        // Hold a reference to the generated geodatabase.
        private Geodatabase _resultGdb;

        public Map4View()
        {
            InitializeComponent();

            // Create the UI, setup the control references and execute initialization.
            Initialize();

            Initialize_LocalServer();
        }

        private void Initialize()
        {

            // Assign the map to the MapView.
            MyMapView.Map = new Map(Basemap.CreateOpenStreetMap());

            MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
            double _ulsanScale = 150000;
            MyMapView.SetViewpointCenterAsync(_ulsanCoords, _ulsanScale);



            // Set up an event handler for when the viewpoint (extent) changes.
            MyMapView.ViewpointChanged += MapViewExtentChanged;


        }

        private async void GeoViewTapped(object sender, Esri.ArcGISRuntime.UI.Controls.GeoViewInputEventArgs e)
        {
            try
            {
                // Disregard if not ready for edits.
                if (_readyForEdits == EditState.NotReady) { return; }

                // If an edit is in process, finish it.
                if (_readyForEdits == EditState.Editing)
                {
                    // Hold a list of any selected features.
                    List<Feature> selectedFeatures = new List<Feature>();

                    // Get all selected features then clear selection.
                    foreach (FeatureLayer layer in MyMapView.Map.OperationalLayers)
                    {
                        // Get the selected features.
                        FeatureQueryResult layerFeatures = await layer.GetSelectedFeaturesAsync();

                        // FeatureQueryResult implements IEnumerable, so it can be treated as a collection of features.
                        selectedFeatures.AddRange(layerFeatures);

                        // Clear the selection.
                        layer.ClearSelection();
                    }

                    // Update all selected features' geometry.
                    foreach (Feature feature in selectedFeatures)
                    {
                        // Get a reference to the correct feature table for the feature.
                        GeodatabaseFeatureTable table = (GeodatabaseFeatureTable)feature.FeatureTable;

                        // Ensure the geometry type of the table is point.
                        if (table.GeometryType != GeometryType.Point)
                        {
                            continue;
                        }

                        // Set the new geometry.
                        feature.Geometry = e.Location;

                        try
                        {
                            // Update the feature in the table.
                            await table.UpdateFeatureAsync(feature);
                        }
                        catch (ArcGISException)
                        {
                            MessageBox.Show("Feature must be within extent of geodatabase.");
                        }
                    }

                    // Update the edit state.
                    _readyForEdits = EditState.Ready;

                    // Enable the sync button.
                    MySyncButton.IsEnabled = true;

                    // Update the help label.
                    MyHelpLabel.Text = "4. Click 'Sync Geodatabase' or edit more features";
                }
                // Otherwise, start an edit.
                else
                {
                    // Define a tolerance for use with identifying the feature.
                    double tolerance = 15 * MyMapView.UnitsPerPixel;

                    // Define the selection envelope.
                    Envelope selectionEnvelope = new Envelope(e.Location.X - tolerance, e.Location.Y - tolerance, e.Location.X + tolerance, e.Location.Y + tolerance);

                    // Define query parameters for feature selection.
                    QueryParameters query = new QueryParameters()
                    {
                        Geometry = selectionEnvelope
                    };

                    // Track whether any selections were made.
                    bool selectedFeature = false;

                    // Select the feature in all applicable tables.
                    foreach (FeatureLayer layer in MyMapView.Map.OperationalLayers)
                    {
                        FeatureQueryResult res = await layer.SelectFeaturesAsync(query, SelectionMode.New);
                        selectedFeature = selectedFeature || res.Any();
                    }

                    // Only update state if a feature was selected.
                    if (selectedFeature)
                    {
                        // Set the edit state.
                        _readyForEdits = EditState.Editing;

                        // Update the help label.
                        MyHelpLabel.Text = "3. Tap on the map to move the point";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void UpdateMapExtent()
        {
            // Return if mapview is null.
            if (MyMapView == null) { return; }

            // Get the new viewpoint.
            Viewpoint myViewPoint = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry);

            // Return if viewpoint is null.
            if (myViewPoint == null) { return; }

            // Get the updated extent for the new viewpoint.
            Envelope extent = myViewPoint.TargetGeometry as Envelope;

            // Return if extent is null.
            if (extent == null) { return; }

            // Create an envelope that is a bit smaller than the extent.
            EnvelopeBuilder envelopeBldr = new EnvelopeBuilder(extent);
            envelopeBldr.Expand(0.80);

            // Get the (only) graphics overlay in the map view.
            GraphicsOverlay extentOverlay = MyMapView.GraphicsOverlays.FirstOrDefault();

            // Return if the extent overlay is null.
            if (extentOverlay == null) { return; }

            // Get the extent graphic.
            Graphic extentGraphic = extentOverlay.Graphics.FirstOrDefault();

            // Create the extent graphic and add it to the overlay if it doesn't exist.
            if (extentGraphic == null)
            {
                extentGraphic = new Graphic(envelopeBldr.ToGeometry());
                extentOverlay.Graphics.Add(extentGraphic);
            }
            else
            {
                // Otherwise, simply update the graphic's geometry.
                extentGraphic.Geometry = envelopeBldr.ToGeometry();
            }
        }

        private async Task StartGeodatabaseGeneration()
        {
            // Create a task for generating a geodatabase (GeodatabaseSyncTask).
            _gdbSyncTask = await GeodatabaseSyncTask.CreateAsync(_featureServiceUri);

            // Get the (only) graphic in the map view.
            Graphic redPreviewBox = MyMapView.GraphicsOverlays.First().Graphics.First();

            // Get the current extent of the red preview box.
            Envelope extent = redPreviewBox.Geometry as Envelope;

            // Get the default parameters for the generate geodatabase task.
            GenerateGeodatabaseParameters generateParams = await _gdbSyncTask.CreateDefaultGenerateGeodatabaseParametersAsync(extent);

            // Create a generate geodatabase job.
            GenerateGeodatabaseJob generateGdbJob = _gdbSyncTask.GenerateGeodatabase(generateParams, _gdbPath);

            // Handle the progress changed event with an inline (lambda) function to show the progress bar.
            generateGdbJob.ProgressChanged += (sender, e) =>
            {
                // Update the progress bar.
                UpdateProgressBar(generateGdbJob.Progress);
            };

            // Show the progress bar.
            MyProgressBar.Visibility = Visibility.Visible;

            // Start the job.
            generateGdbJob.Start();

            // Get the result of the job.
            _resultGdb = await generateGdbJob.GetResultAsync();

            // Hide the progress bar.
            MyProgressBar.Visibility = Visibility.Collapsed;

            // Do the rest of the work.
            HandleGenerationCompleted(generateGdbJob);
        }

        private async void HandleGenerationCompleted(GenerateGeodatabaseJob job)
        {
            // If the job completed successfully, add the geodatabase to the map, replacing the layer from the service.
            if (job.Status == JobStatus.Succeeded)
            {
                // Remove the existing layers.
                MyMapView.Map.OperationalLayers.Clear();

                // Loop through all feature tables in the geodatabase and add a new layer to the map.
                foreach (GeodatabaseFeatureTable table in _resultGdb.GeodatabaseFeatureTables)
                {
                    // Skip non-point tables.
                    await table.LoadAsync();
                    if (table.GeometryType != GeometryType.Point)
                    {
                        continue;
                    }

                    // Create a new feature layer for the table.
                    FeatureLayer layer = new FeatureLayer(table);

                    // Add the new layer to the map.
                    MyMapView.Map.OperationalLayers.Add(layer);
                }

                // Enable editing features.
                _readyForEdits = EditState.Ready;

                // Update help label.
                MyHelpLabel.Text = "2. Tap a point feature to select";
            }
            else
            {
                // Create a message to show the user.
                string message = "Generate geodatabase job failed";

                // Show an error message (if there is one).
                if (job.Error != null)
                {
                    message += ": " + job.Error.Message;
                }
                else
                {
                    // If no error, show messages from the job.
                    foreach (JobMessage m in job.Messages)
                    {
                        // Get the text from the JobMessage and add it to the output string.
                        message += "\n" + m.Message;
                    }
                }

                // Show the message.
                MessageBox.Show(message);
            }
        }

        private async Task SyncGeodatabase()
        {
            // Return if not ready.
            if (_readyForEdits != EditState.Ready) { return; }

            // Disable the sync button.
            MySyncButton.IsEnabled = false;

            // Create parameters for the sync task.
            SyncGeodatabaseParameters parameters = new SyncGeodatabaseParameters()
            {
                GeodatabaseSyncDirection = SyncDirection.Bidirectional,
                RollbackOnFailure = false
            };

            // Get the layer ID for each feature table in the geodatabase, then add to the sync job.
            foreach (GeodatabaseFeatureTable table in _resultGdb.GeodatabaseFeatureTables)
            {
                // Get the ID for the layer.
                long id = table.ServiceLayerId;

                // Create the SyncLayerOption.
                SyncLayerOption option = new SyncLayerOption(id);

                // Add the option.
                parameters.LayerOptions.Add(option);
            }

            // Create job.
            SyncGeodatabaseJob job = _gdbSyncTask.SyncGeodatabase(parameters, _resultGdb);

            // Subscribe to progress updates.
            job.ProgressChanged += (o, e) =>
            {
                // Update the progress bar.
                UpdateProgressBar(job.Progress);
            };

            // Show the progress bar.
            MyProgressBar.Visibility = Visibility.Visible;

            // Start the sync job.
            job.Start();

            // Wait for the result.
            await job.GetResultAsync();

            // Hide the progress bar.
            MyProgressBar.Visibility = Visibility.Hidden;

            // Do the remainder of the work.
            HandleSyncCompleted(job);

            // Re-enable the sync button.
            MySyncButton.IsEnabled = true;
        }

        private void HandleSyncCompleted(SyncGeodatabaseJob job)
        {
            // Tell the user about job completion.
            if (job.Status == JobStatus.Succeeded)
            {
                // Update the progress bar's value.
                UpdateProgressBar(0);

                MessageBox.Show("Sync task completed");
            }

            // See if the job failed.
            if (job.Status == JobStatus.Failed)
            {
                // Create a message to show the user.
                string message = "Sync geodatabase job failed";

                // Show an error message (if there is one).
                if (job.Error != null)
                {
                    message += ": " + job.Error.Message;
                }
                else
                {
                    // If no error, show messages from the job.
                    foreach (JobMessage m in job.Messages)
                    {
                        // Get the text from the JobMessage and add it to the output string.
                        message += "\n" + m.Message;
                    }
                }

                // Show the message.
                MessageBox.Show(message);
            }
        }

        private async void GenerateButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Fix the selection graphic extent.
            MyMapView.ViewpointChanged -= MapViewExtentChanged;

            // Update the Geodatabase path for the new run.
            try
            {
                _gdbPath = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), "geofms.geodatabase");

                // Prevent duplicate clicks.
                MyGenerateButton.IsEnabled = false;

                // Call the cross-platform geodatabase generation method.
                await StartGeodatabaseGeneration();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MapViewExtentChanged(object sender, EventArgs e)
        {
            // Call the cross-platform map extent update method.
            UpdateMapExtent();
        }

        private void UpdateProgressBar(int progress)
        {
            // Due to the nature of the threading implementation,
            //     the dispatcher needs to be used to interact with the UI.
            // The dispatcher takes an Action, provided here as a lambda function.
            Dispatcher.Invoke(() =>
            {
                // Update the progress bar value.
                MyProgressBar.Value = progress;
            });
        }

        private async void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SyncGeodatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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





        #region ============ LocalServer (start) 관련부분 ==============

        // Hold a reference to the local feature service; the ServiceFeatureTable will be loaded from this service
        public LocalFeatureService _localFeatureService;

        public async void Initialize_LocalServer()
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
                //서버올라오면 geodatabase 동기화연결 시작...
                _featureServiceUri = _localFeatureService.Url;

                try
                {
                    // Create a task for generating a geodatabase (GeodatabaseSyncTask).
                    _gdbSyncTask = await GeodatabaseSyncTask.CreateAsync(_featureServiceUri);

                    // Add all graphics from the service to the map.
                    foreach (IdInfo layer in _gdbSyncTask.ServiceInfo.LayerInfos)
                    {
                        // Get the URL for this particular layer.
                        Uri onlineTableUri = new Uri(_featureServiceUri + "/" + layer.Id);

                        // Create the ServiceFeatureTable.
                        ServiceFeatureTable onlineTable = new ServiceFeatureTable(onlineTableUri);

                        // Wait for the table to load.
                        await onlineTable.LoadAsync();

                        // Skip tables that aren't for point features.{
                        if (onlineTable.GeometryType != GeometryType.Point)
                        {
                            continue;
                        }

                        // Add the layer to the map's operational layers if load succeeds.
                        if (onlineTable.LoadStatus == LoadStatus.Loaded)
                        {
                            MyMapView.Map.OperationalLayers.Add(new FeatureLayer(onlineTable));
                        }
                    }

                    // Update the graphic - needed in case the user decides not to interact before pressing the button.
                    UpdateMapExtent();

                    // Enable the generate button.
                    MyGenerateButton.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }

            }
        }


        // mpk 패키지파일의 위치 가져오기
        private static string GetFeatureLayerPath()
        {
            //return DataManager.GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
            //return GetDataFolder("4e94fec734434d1288e6ebe36c3c461f", "PointsOfInterest.mpk");
            return GetDataFolder("shape", "fms.mpk");
        }









        #endregion



    }
}
