using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using GTI.WFMS.Models.Common;
using System;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// GIS 전역변수
    /// </summary>
    public class GisCmm
    {

        // Coordinates for Ulsan
        public static MapPoint _ulsanCoords = new MapPoint(14389882.070911, 4239809.084922, SpatialReferences.WebMercator); //3857
        //private MapPoint _londonCoords = new MapPoint(-13881.7678417696, 6710726.57374296, SpatialReferences.WebMercator);
        //private MapPoint _ulsanCoords = new MapPoint(394216.933974, 223474.303376, SpatialReferences.WebMercator); //5181

        //private double _ulsanScale = 8762.7156655228955;
        //private double _ulsanScale = 150000;
        public static double _ulsanScale = 500000;
        public static double _ulsanScale2 = 150000;










    }
}
