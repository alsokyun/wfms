using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.GIS.Ext
{
    public class FmsFeature
    {
        private FeatureLayer fl = new FeatureLayer();
        public FeatureLayer FL { get; set; }

        public bool chk { get; set; }
    }
}
