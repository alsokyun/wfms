using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using GTI.WFMS.GIS.Pop.View;
using GTI.WFMS.Models.Common;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// MapArcObjView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapArcObjView : UserControl
    {

        AxMapControl mapControl;
        AxToolbarControl toolbarControl;
        //AxTOCControl tocControl;

   


        public MapArcObjView()
        {
            InitializeComponent();

			CreateEngineControls ();
        }






        // Create ArcGIS Engine Controls and set them to be child of each WindowsFormsHost elements
        void CreateEngineControls()
        {
            //set Engine controls to the child of each hosts 
            mapControl = new AxMapControl();
            mapHost.Child = mapControl;
            mapControl.Dock = System.Windows.Forms.DockStyle.None;

            toolbarControl = new AxToolbarControl();
            toolbarHost.Child = toolbarControl;

            //tocControl = new AxTOCControl();
            //tocHost.Child = tocControl;
        }

        private void LoadMap()
        {
            CmmRun.InitUniqueValueRenderer();//렌더러초기생성작업


            //Buddy up controls
            //tocControl.SetBuddyControl(mapControl);
            toolbarControl.SetBuddyControl(mapControl);

            //add command and tools to the toolbar
            //toolbarControl.AddItem("esriControls.ControlsOpenDocCommand");
            //toolbarControl.AddItem("esriControls.ControlsAddDataCommand");
            //toolbarControl.AddItem("esriControls.ControlsSaveAsDocCommand");
            toolbarControl.AddItem("esriControls.ControlsMapNavigationToolbar");
            toolbarControl.AddItem("esriControls.ControlsMapIdentifyTool");
            toolbarControl.AddItem("esriControls.ControlsMapMeasureTool");
            toolbarControl.AddItem("esriControls.ControlsMapZoomToolControl");
            toolbarControl.AddItem("esriControls.ControlsMapGoToCommand");

            

            //set controls' properties
            toolbarControl.BackColor = Color.FromArgb(245, 245, 220);

            //wire up events
            mapControl.OnMouseMove += new IMapControlEvents2_Ax_OnMouseMoveEventHandler(mapControl_OnMouseMove);



            //IUniqueValueRenderer renderer = (UniqueValueRenderer)GisCmm.uniqueValueRenderer;
            //layer.Renderer = renderer as IFeatureRenderer; 


            //스케일확대
            //mapControl.Extent = WTL_FLOW_PS.AreaOfInterest;
            //mapControl.Scale(10000f);
            //mapControl.ToMapPoint(GisCmm._ulsanCoords.X, GisCmm._ulsanCoords.Y );


            //레이어명 적용
            //tocControl.Update();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMap();
        }

        private void mapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            textBlock1.Text = " X,Y Coordinates on Map: " + string.Format("{0}, {1}  {2}", e.mapX.ToString("#######.##"), e.mapY.ToString("#######.##"), mapControl.MapUnits.ToString().Substring(4));
        }

        //시설물편집창
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditWinView view = new EditWinView();
            if (view.ShowDialog() is bool)
            {
                //재조회
                mapControl.Refresh();

                //심볼초기화
                CmmObj.InitUniqueValueRendererObj();
            }
        }
    }
}
