﻿using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Point = System.Windows.Point;

namespace GTI.WFMS.GIS.sample
{
    /// <summary>
    /// 오프라인 맵저장
    /// </summary>
    public partial class SketchOnMap : UserControl
    {

        // Graphics overlay to host sketch graphics
        private GraphicsOverlay _sketchOverlay;

        public SketchOnMap()
        {
            InitializeComponent();

            // Call a function to set up the map and sketch editor
            Initialize();
        }

        private void Initialize()
        {
            // Create a light gray canvas map
            Map myMap = new Map(Basemap.CreateLightGrayCanvas());

            // Create graphics overlay to display sketch geometry
            _sketchOverlay = new GraphicsOverlay();
            MyMapView.GraphicsOverlays.Add(_sketchOverlay);

            // Assign the map to the MapView
            MyMapView.Map = myMap;

            // Fill the combo box with choices for the sketch modes (shapes)
            SketchModeComboBox.ItemsSource = System.Enum.GetValues(typeof(SketchCreationMode));
            SketchModeComboBox.SelectedIndex = 0;

            // Set the sketch editor as the page's data context
            DataContext = MyMapView.SketchEditor;
        }

        #region Graphic and symbol helpers
        private Graphic CreateGraphic(Esri.ArcGISRuntime.Geometry.Geometry geometry)
        {
            // Create a graphic to display the specified geometry
            Symbol symbol = null;
            switch (geometry.GeometryType)
            {
                // Symbolize with a fill symbol
                case GeometryType.Envelope:
                case GeometryType.Polygon:
                    {
                        symbol = new SimpleFillSymbol()
                        {
                            Color = Color.Red,
                            Style = SimpleFillSymbolStyle.Solid
                        };
                        break;
                    }
                // Symbolize with a line symbol
                case GeometryType.Polyline:
                    {
                        symbol = new SimpleLineSymbol()
                        {
                            Color = Color.Red,
                            Style = SimpleLineSymbolStyle.Solid,
                            Width = 5d
                        };
                        break;
                    }
                // Symbolize with a marker symbol
                case GeometryType.Point:
                case GeometryType.Multipoint:
                    {

                        symbol = new SimpleMarkerSymbol()
                        {
                            Color = Color.Red,
                            Style = SimpleMarkerSymbolStyle.Circle,
                            Size = 15d
                        };
                        break;
                    }
            }

            // pass back a new graphic with the appropriate symbol
            return new Graphic(geometry, symbol);
        }

        private async Task<Graphic> GetGraphicAsync()
        {
            // Wait for the user to click a location on the map
            Geometry mapPoint = await MyMapView.SketchEditor.StartAsync(SketchCreationMode.Point, false);

            // Convert the map point to a screen point
            Point screenCoordinate = MyMapView.LocationToScreen((MapPoint)mapPoint);

            // Identify graphics in the graphics overlay using the point
            IReadOnlyList<IdentifyGraphicsOverlayResult> results = await MyMapView.IdentifyGraphicsOverlaysAsync(screenCoordinate, 2, false);

            // If results were found, get the first graphic
            Graphic graphic = null;
            IdentifyGraphicsOverlayResult idResult = results.FirstOrDefault();
            if (idResult != null && idResult.Graphics.Count > 0)
            {
                graphic = idResult.Graphics.FirstOrDefault();
            }

            // Return the graphic (or null if none were found)
            return graphic;
        }
        #endregion

        private async void DrawButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = (SketchCreationMode)SketchModeComboBox.SelectedItem;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await MyMapView.SketchEditor.StartAsync(creationMode, true); //맵에 신규geometry 얻어오기

                // Create and add a graphic from the geometry the user drew
                Graphic graphic = CreateGraphic(geometry);
                _sketchOverlay.Graphics.Add(graphic);

                // Enable/disable the clear and edit buttons according to whether or not graphics exist in the overlay
                ClearButton.IsEnabled = _sketchOverlay.Graphics.Count > 0;
                EditButton.IsEnabled = _sketchOverlay.Graphics.Count > 0;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("Error drawing graphic shape: " + ex.Message);
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            // Remove all graphics from the graphics overlay
            _sketchOverlay.Graphics.Clear();

            // Disable buttons that require graphics
            ClearButton.IsEnabled = false;
            EditButton.IsEnabled = false;
        }

        private async void EditButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Allow the user to select a graphic
                Graphic editGraphic = await GetGraphicAsync();
                if (editGraphic == null) { return; }

                // Let the user make changes to the graphic's geometry, await the result (updated geometry)
                Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(editGraphic.Geometry); //맵에 기존geometry 얻어오기

                // Display the updated geometry in the graphic
                editGraphic.Geometry = newGeometry;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel editing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("Error editing shape: " + ex.Message);
            }
        }

    }
}
