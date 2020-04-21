using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GTI.WFMS.Models.Common;
using Esri.ArcGISRuntime.UI.Controls;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.GIS.Module;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms.Integration;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using GTIFramework.Common.MessageBox;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Security;

namespace GTI.WFMS.GIS
{
    /// <summary>
    /// Provides map data to an application
    /// </summary>
    public class MapArcObjViewModel : INotifyPropertyChanged
    {


        #region ========== Members ���� ==========

        // ������ ��ó - ���������Feature  
        public string _selectedLayerNm = "";
        public List<string> _selectedLayerNms = new List<string>();

        UserControl mapArcObjView;
        WindowsFormsHost mapHost;
        WindowsFormsHost toolbarHost;
        TreeView treeLayer;
        AxMapControl mapControl;
        AxToolbarControl toolbarControl;

        public event PropertyChangedEventHandler PropertyChanged;
        private Popup popFct = new Popup(); //�ü�������DIV
        public FmsStack<string> sts = new FmsStack<string>();

        // ���ϰ��� �������� - ȯ�漳������ �����ؾ���
        string dbShapeDir = @"" + FmsUtil.dbShapeDir;
        Thread upload_thread;



        public Dictionary<string, FeatureLayer> layers;
        public void initLayers()
        {
            layers = new Dictionary<string, FeatureLayer>()
            {
                {"WTL_FLOW_PS",  new FeatureLayer()},
                {"WTL_FIRE_PS^SA118",  new FeatureLayer()},
                {"WTL_FIRE_PS^SA119",  new FeatureLayer()},
                {"WTL_GAIN_PS",  new FeatureLayer()},
                {"WTL_HEAD_PS",  new FeatureLayer()},
                {"WTL_LEAK_PS",  new FeatureLayer()},
                {"WTL_MANH_PS",  new FeatureLayer()},
                {"WTL_META_PS",  new FeatureLayer()},
                {"WTL_PRES_PS",  new FeatureLayer()},
                {"WTL_PRGA_PS",  new FeatureLayer()},
                {"WTL_RSRV_PS",  new FeatureLayer()},
                {"WTL_SERV_PS",  new FeatureLayer()},
                {"WTL_STPI_PS",  new FeatureLayer()},
                {"WTL_VALV_PS^SA200",  new FeatureLayer()},
                {"WTL_VALV_PS^SA201",  new FeatureLayer()},
                {"WTL_VALV_PS^SA202",  new FeatureLayer()},
                {"WTL_VALV_PS^SA203",  new FeatureLayer()},
                {"WTL_VALV_PS^SA204",  new FeatureLayer()},
                {"WTL_VALV_PS^SA205",  new FeatureLayer()},
                {"WTL_VALV_PS^SA206",  new FeatureLayer()},


                {"BML_GADM_AS",  new FeatureLayer()},
                {"WTL_PURI_AS",  new FeatureLayer()},

                {"WTL_PIPE_LM",  new FeatureLayer()},
                {"WTL_SPLY_LS",  new FeatureLayer()},
            };

        }



        #endregion



        #region ========== �������̽� �������̵� ==========

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
        #endregion



        #region ==========  Properties ���� ==========
        public virtual ObservableCollection<FileInfo> ItemsFile { get; set; } //���ϰ�ü


        public RelayCommand<object> loadedCmd { get; set; } //Loaded�̺�Ʈ���� ICommand ����Ͽ� �䰴ü ���޹���
        public RelayCommand<object> chkCmd { get; set; }
        public RelayCommand<object> resetCmd { get; set; }
        public RelayCommand<object> importCmd { get; set; }
        

        //�ü����⺻����
        private CmmDtl fctDtl = new CmmDtl(); 
        public CmmDtl FctDtl
        {
            get { return this.fctDtl; }
            set
            {
                this.fctDtl = value;
                OnPropertyChanged("FctDtl");
            }
        }
        //�ü����̸����� �ҽ��̹���
        private BitmapImage bitImg;
        public BitmapImage BitImg
        {
            get { return bitImg; }
            set
            {
                this.bitImg = value;
                OnPropertyChanged("BitImg");
            }
        }

        #endregion





        #region ========== ������ ==========

        public MapArcObjViewModel()
        {

            loadedCmd = new RelayCommand<object>(delegate (object obj)
            {
                //�䰴ü�� �Ķ���ͷ� ���޹ޱ�
                mapArcObjView = obj as UserControl;

                mapHost = mapArcObjView.FindName("mapHost") as WindowsFormsHost;
                toolbarHost = mapArcObjView.FindName("toolbarHost") as WindowsFormsHost;
                treeLayer = mapArcObjView.FindName("treeLayer") as TreeView;

                mapControl = mapHost.Child as AxMapControl;
                toolbarControl = toolbarHost.Child as AxToolbarControl;

                
                //0.�� �ʱ�ȭ
                initMap();

                //1.���̾��ʱ�ȭ
                initLayers();

                //2.UniqueRenderer �ʱ�ȭ
                GisCm.InitUniqueValueRenderer();

                //3.�����������ǥ��
                ShowShapeLayer("BML_GADM_AS", true);


                // ������������Ʈ
                ItemsFile = new ObservableCollection<FileInfo>();

                //��Ʈ���ʱ�ȭ(�ü�����DIV ������)
                BitImg = new BitmapImage();
            });


            //���̾� ON/OFF
            chkCmd = new RelayCommand<object>(delegate (object obj)
            {
                Button doc = obj as Button;

                CheckBox chkbox = doc.Template.FindName("chkLayer", doc) as CheckBox;
                bool chk = (bool)chkbox.IsChecked;

                ShowShapeLayer( doc.Tag.ToString(), chk);


                //���õ� ���̾�����
                try
                {
                    if (chk)
                    {
                        _selectedLayerNms.Add(doc.Tag.ToString());
                        _selectedLayerNm = doc.Tag.ToString();
                    }
                    else
                    {
                        _selectedLayerNms.Remove(doc.Tag.ToString());
                        _selectedLayerNm = _selectedLayerNms.LastOrDefault();

                    }
                }
                catch (Exception) { }
            });


            //GIS�ʱ�ȭ
            resetCmd = new RelayCommand<object>(resetAction);

            //shp ����Ʈ
            importCmd = new RelayCommand<object>(importAction);
            
        }
        #endregion

        //shp ����Ʈ
        private void importAction(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Shape files |*.shp;*.dbf";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo[] files = openFileDialog.FileNames.Select(f => new FileInfo(f)).ToArray();  //��������

                foreach (FileInfo fi in files)
                {
                    try
                    {
                        //���ϰ�ü
                        ItemsFile.Add(fi);
                    }
                    catch (Exception) { }
                }

                //���Ͼ��ε����
                upload_thread = new Thread(new ThreadStart(UploadFileListFX));
                upload_thread.Start();
            }
        }



        // ���ε� �������ڵ鷯
        private void UploadFileListFX()
        {
            try
            {
                //�ε���..
                mapArcObjView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (mapArcObjView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));


                //���ε����...
                UploadFileList();



                mapArcObjView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       //db �����Ŀ�����ũ��Ʈ ���� - Ƽ���� gisLoader, tbloader 
                       ExPsScript(@"d:\shape.ps1");


                       (mapArcObjView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowOkMsgBox();
                       //�˾��ݱ�
                       //btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                   })));


            }
            catch (Exception ex)
            {
                mapArcObjView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (mapArcObjView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }







        /// <summary>
        /// ������������ ���ε�����, �������̺��� ����ϰ�, _FIL_SEQ�� �����Ѵ�
        /// </summary>
        /// <returns></returns>
        private void UploadFileList()
        {
            // 1.shp ���������� ����

            /// Items�� �߰��� ���ϰ�ü���̴�
            foreach (FileInfo fi in ItemsFile)
            {
                string shp_file_path = Path.Combine(BizUtil.GetDataFolder("shape"), fi.Name);
                string db_shp_file_path = Path.Combine(FmsUtil.dbShapeDir, fi.Name);

                try
                {
                    // 1.shp���� ���α׷� ��ġ�� ����
                    fi.CopyTo(shp_file_path, true);

                    // 2.shp���� db���� ��ġ�� ���ݺ���
                    ExPsScript(@"d:\shape_copy.ps1");



                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.Message);
                }


            }

        }





        /// <summary>
        /// �Ŀ��� - db ���ݽ�ũ��Ʈ ���� - Ƽ���� gisLoader, tbloader 
        /// </summary>
        /// <param name="scriptfile"></param>
        private void ExPsScript(string scriptfile)
        {
            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            RunspaceInvoke scriptInvoker = new RunspaceInvoke();
            scriptInvoker.Invoke("Set-ExecutionPolicy RemoteSigned");

            Pipeline pipeline = runspace.CreatePipeline();

            //Here's how you add a new script with arguments
            Command myCommand = new Command(scriptfile);
            //CommandParameter testParam = new CommandParameter("key", "value");
            //myCommand.Parameters.Add(testParam);

            pipeline.Commands.Add(myCommand);

            // Execute PowerShell script
            pipeline.Invoke();
        }



        /// <summary>
        /// �Ŀ��� - Ŀ�ǵ������ ���ɼ��� - �Ķ���͸� �����ϱ�����.. ����ó�� �����۾��ʿ�
        /// </summary>
        private static void ExPsSessionCmd()
        {
            // Username and Password for the remote machine.
            var userName = "administrator";
            string pw = "greenTech!@3";

            // Creates a secure string for the password
            SecureString securePassword = new SecureString();
            foreach (char c in pw)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();

            // Creates a PSCredential object
            PSCredential creds = new PSCredential(userName, securePassword);




            // Creates the runspace for PowerShell
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // Create the PSSession connection to the remote machine.
            //ComputerName
            string computerName = "172.16.0.4";

            PowerShell powershell = PowerShell.Create();
            PSCommand command = new PSCommand();
            command.AddCommand("New-PSSession");
            command.AddParameter("ComputerName", computerName);
            command.AddParameter("Credential", creds);
            powershell.Commands = command;
            runspace.Open();
            powershell.Runspace = runspace;
            Collection<PSObject> result = powershell.Invoke();



            // Takes the PSSession object and places it into a PowerShell variable
            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", "session");
            command.AddParameter("Value", result[0]);
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();



            // Calls the Copy-Item cmdlet as a script and passes the PSSession, Path and destination parameters to it
            string path = @"D:\SHAPE\shape_fms\WTL_SPLY_LS.shp";
            string destination = @"D:\shape\WTL_SPLY_LS.shp";

            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript("Copy-Item -Path " + path + " -Destination " + destination + " -ToSession $session");
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();

            // ��������
            powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            powershell.AddScript("Remove-PSSession $session");
            powershell.Invoke();
        }













        #region ============= �����Լ� ============= 


        public void initMap()
        {
            //��������� mxd���Ϸκ���
            string mxdfilePath = Path.Combine(BizUtil.GetDataFolder("tile", "also.mxd"));
            mapControl.LoadMxFile(mxdfilePath);

        }


        private void resetAction(object obj)
        {
            //0.���Ƿ��̾� Ŭ����
            mapControl.ClearLayers();

            //1.���ʱ�ȭ
            initMap();

            //2.���̾��ʱ�ȭ
            initLayers();

            //3.���̾�� �ʱ�ȭ
            sts.Clear();

            //4.�����������ǥ��
            ShowShapeLayer("BML_GADM_AS", true);




            //�����ִ� �ü�������â �ݱ�
            popFct.IsOpen = false;

            //���̾�div üũ����
            foreach (CheckBox cb in FmsUtil.FindVisualChildren<CheckBox>(treeLayer))
            {
                cb.IsChecked = false;
            }
        }





        /// <summary>
        /// Shape ���̾� ���̱�/���� - Shape����
        /// </summary>
        /// <param name="_mapView"></param>
        /// <param name="layer"></param>
        /// <param name="chk"></param>
        public void ShowShapeLayer(string _layerNm, bool chk)
        {
            try
            {
                // 0.�ش緹�̾� ��������
                string filterExp = "";
                string shapeNm = "";

                try
                {
                    string[] ary = _layerNm.Split('^');
                    shapeNm = ary[0]; //���̾����̺���
                    filterExp = "FTR_CDE='" + ary[1] + "'"; //����ǥ����
                }
                catch (Exception) { }


                



                // 1.���̾� ON
                if (chk)
                {
                    if (layers[_layerNm].Name != "")
                    {
                        //���̾�ε� �����̸� �ʿ� �߰��� ����
                        mapControl.AddLayer(layers[_layerNm]);
                        sts.Push(_layerNm);//���̾��ε��� ������������ �Ȱ��� ���ÿ� �����س���
                    }
                    else
                    {
                        mapControl.AddShapeFile(BizUtil.GetDataFolder("shape"), shapeNm + ".shp");

                        //���̾ü ����
                        layers[_layerNm] = mapControl.get_Layer(0) as FeatureLayer; //�������̹Ƿ� �ε����� 0 
                        layers[_layerNm].Name = GisCm.getLayerKorNm(_layerNm);
                        sts.Push(_layerNm);//���̾��ε��� ������������ �Ȱ��� ���ÿ� �����س���


                        /* Renderer ���� */
                        IGeoFeatureLayer pGFL  = layers[_layerNm] as IGeoFeatureLayer;

                        if ("BML_GADM_AS".Equals(_layerNm)) //�����������
                        {
                            //���νɺ�
                            ISimpleLineSymbol lineSymbol = new SimpleLineSymbol();
                            lineSymbol.Color = new RgbColor() { Red = 255, Green = 0, Blue = 0 };
                            lineSymbol.Style = esriSimpleLineStyle.esriSLSDashDotDot;
                            lineSymbol.Width = 1.5;

                            //Fill�ɺ�
                            ISimpleFillSymbol fillSymbol = new SimpleFillSymbol();
                            fillSymbol.Color = new RgbColor() { Red = 255, Green = 255, Blue = 255 };
                            fillSymbol.Style = esriSimpleFillStyle.esriSFSHollow;
                            
                            fillSymbol.Outline = lineSymbol;  //�ܰ����� ���νɺ��� ����

                            ISimpleRenderer pSR = pGFL.Renderer as SimpleRenderer;
                            pSR.Symbol = fillSymbol as ISymbol;
                        }
                        else
                        {
                            //���̾��� UniqueValueRenderer ����
                            pGFL.Renderer = GisCm.uniqueValueRenderer as IFeatureRenderer;
                        }
                    }


                    // ���͸� �μ������� �����ü������� ����
                    if (!FmsUtil.IsNull(filterExp))
                    {
                        //���̾����͸�
                        IFeatureLayerDefinition flayer = layers[_layerNm] as IFeatureLayerDefinition;
                        flayer.DefinitionExpression = filterExp;

                    }

                    //mapControl.Extent = layers[_layerNm].AreaOfInterest;
                }
                // 2.���̾� OFF
                else
                {
                    if (layers[_layerNm].Name != "")
                    {
                        mapControl.DeleteLayer(sts.GetStackIdx(_layerNm));
                        sts.Remove(sts.GetIdx(_layerNm));
                    }
                    else
                    {
                        //�ε����� �������� �ƹ��͵� ����
                    }
                }



                
            }
            catch (Exception)
            {
                MessageBox.Show("���̾ �������� �ʽ��ϴ�.");
            }
        }





        /// <summary>
        /// �ش�ü����� ��������ġ ã�ư���(����ȭ�鿡�� ȣ���) 
        /// </summary>
        /// <param name="FTR_CDE"></param>
        /// <param name="FTR_IDN"></param>
        /// <returns></returns>
        public void findFtr(string FTR_CDE, string FTR_IDN)
        {

            string layerNm = "";
            try
            {
                layerNm = GisCmm.GetLayerNm(FTR_CDE);
                if ("".Equals(layerNm))
                {
                    MessageBox.Show("�߸��� ���̾��Դϴ�.");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("�߸��� ���̾��Դϴ�.");
                return;
            }

            //��������
            resetAction(null);

            //0.�ش緹�̾�ǥ�� - ���ο����ڵ����� �ε����� üũ��
            ShowShapeLayer(GisCmm.GetLayerNm(FTR_CDE), true);

            //1.�ش緹�̾� ��������
            FeatureLayer layer = layers[GisCmm.GetLayerNm(FTR_CDE)];




            //1.����Ʈó�� ���͸�
            IQueryFilter qfltr = new QueryFilter();
            qfltr.WhereClause = " FTR_CDE = '" + FTR_CDE + "' AND FTR_IDN = " + FTR_IDN; 

            IFeatureSelection fsel = layer as IFeatureSelection;
            fsel.SelectFeatures(qfltr, esriSelectionResultEnum.esriSelectionResultAdd, true);

            //2.��ó��ü ���͸� - Ÿ������ �����߽��̵�
            IFeatureCursor cursor = layer.Search(qfltr, false);
            IFeature feature = cursor.NextFeature(); 

            ESRI.ArcGIS.Geometry.IPoint point;

            if (FTR_CDE == "SA001" || FTR_CDE == "SA002") //�������,�޼�����
            {
                ESRI.ArcGIS.Geometry.IPolyline line = (ESRI.ArcGIS.Geometry.IPolyline)feature.ShapeCopy;
                point = line.FromPoint;
            }
            else if(FTR_CDE == "SA113") //������
            {
                ESRI.ArcGIS.Geometry.IPolygon polygon = (ESRI.ArcGIS.Geometry.IPolygon)feature.ShapeCopy;
                point = polygon.FromPoint;
            }
            else //������ ����Ʈ �ü���
            {
                point = (ESRI.ArcGIS.Geometry.IPoint)feature.ShapeCopy;
            }


            var envelope = mapControl.ActiveView.Extent;
            envelope.CenterAt(point);

            mapControl.Extent = envelope;
            mapControl.Refresh();

            //int x = Convert.ToInt32(point.X);
            //int y = Convert.ToInt32(point.Y);
            //mapControl.ToMapPoint(x, y);
            //mapControl.ActiveView.ScreenDisplay.UpdateWindow();

            mapControl.MapScale = 100000;
        }






        // �ü��������˾� ���̱�
        private void ShowFtrPop(string fTR_CDE, string fTR_IDN, GeoViewInputEventArgs e)
        {
            //�˾����� & ��ġ
            popFct.IsOpen = false;

            popFct = new FTR_POP(fTR_CDE, fTR_IDN);

            //double y0 = e.Position.Y - 700; //�������� ǥ����
            //if (y0 < 100) y0 = 100;
            popFct.PlacementRectangle = new Rect(e.Position.X + 550, e.Position.Y - 300, 10, 10);
            popFct.IsOpen = true;
        }





        #endregion











        public class FmsStack<T>
        {
            private List<T> items = new List<T>();

            public void Push(T item)
            {
                items.Add(item);
            }
            public T Pop()
            {
                if (items.Count > 0)
                {
                    T temp = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);
                    return temp;
                }
                else
                    return default(T);
            }
            public void Remove(int itemAtPosition)
            {
                items.RemoveAt(itemAtPosition);
            }


            //������ �ε��� ��������
            public int GetIdx(T layerNm)
            {
                return items.IndexOf(layerNm);
            }

            //������ �ε��� ��������
            public int GetStackIdx(T layerNm)
            {
                List<T> rev = items.Reverse<T>().ToList<T>();
                return rev.IndexOf(layerNm);
            }

            //�����ʱ�ȭ
            public void Clear()
            {
                items.Clear();
            }


        }











    }
}
