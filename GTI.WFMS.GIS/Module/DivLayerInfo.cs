using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GTI.WFMS.GIS.Module
{
    public class DivLayerInfo : Popup
    {

        public DivLayerInfo()
        {

            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };
            //ContentCanvas.Children.Add(thumb);

            MouseDown += (sender, e) =>
            {
                thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };
        }

    }

}
