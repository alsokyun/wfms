using Esri.ArcGISRuntime.Mapping;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTI.WFMS.GIS.Module
{
    /// <summary>
    /// WTL_META_PS.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WTL_META_PS : Popup
    {
        public WTL_META_PS()
        {


            InitializeComponent();



            //마우스드래그이벤트 위한 처리
            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };
            gridContent.Children.Add(thumb);

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

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in ((MapMainViewModel)this.DataContext).layers)
            {
                v.Value.ClearSelection();
            }
            ((MapMainViewModel)this.DataContext)._selectedFeature = null;




            this.IsOpen = false;
        }
    }
}