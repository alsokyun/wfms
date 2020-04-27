using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.UI;
using System.IO;

namespace WGIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {
            //CreateNewMap();
        }

        private Map _map = new Map(Basemap.CreateStreets());

        /// <summary>
        /// Gets or sets the map
        /// </summary>
        public Map Map
        {
            get { return _map; }
            set { _map = value; OnPropertyChanged(); }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;



        private async void CreateNewMap()
        {
            // Get the path to a local shapefile. 
            //string shapefilePath = Path.Combine(Environment.ExpandEnvironmentVariables("%SHAPE%"), "WTL_FIRE_PS.shp");
            string shapefilePath = Path.Combine("D:\\DEVGTI\\GTI.WFMS\\shape", "WTL_FIRE_PS.shp");

            try
            {
                // Create a shapefile feature table using the path. 
                ShapefileFeatureTable WTL_FIRE_PS = await ShapefileFeatureTable.OpenAsync(shapefilePath);

                // Create a feature layer from the table and add it to the map's operational layers. 
                FeatureLayer trailsLayer = new FeatureLayer(WTL_FIRE_PS);
                this.Map.OperationalLayers.Add(trailsLayer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}