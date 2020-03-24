using DevExpress.Xpf.Accordion;
using DevExpress.Xpf.Core;
using GTI.WFMS.GIS;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Main.Work;
using GTI.WFMS.Modules.Dash.View;
using GTI.WFMS.Modules.Main;
using GTI.WFMS.Modules.Main.View;
using GTI.WNMS.Main.View.Pop;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.Main
{
    public class MainWinViewModel : BindableBase
    {
        #region ==========  멤버 정의 ========== 
        MainWork work = new MainWork();
        Hashtable htconditions = new Hashtable();
        MainWin mainwin;
        PopMain pmain = new PopMain();
        Border borderTop = new Border();
        //public static Window popWinView;
        //Assembly assembly = Assembly.GetExecutingAssembly();
        Assembly ModulesAssembly = Assembly.Load("GTI.WFMS.Modules");

        private readonly IRegionManager regionManager;

        public static DataTable dtMenuList = new DataTable(); //전체메뉴데이터
        string strSelectMenu = string.Empty; //선택된메뉴

        private bool bMenuShowHiden = true; //아코디언메뉴 visible 
        private bool bQuickShowHiden = true; //퀵메뉴 visible 


        DataTable dtQuickMenuList = new DataTable(); //즐겨찾기메뉴
        StackPanel stQuickMenu; //즐겨찾기버튼


        #endregion



        #region ==========  Properties 정의 ========== 
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        /// <summary>
        /// 메뉴클릭이벤트
        /// </summary>
        public DelegateCommand<object> MenuControlCommand { get; set; }
        /// <summary>
        /// WindowMove Event
        /// </summary>
        public DelegateCommand<object> WindowMoveCommand { get; set; }
        /// <summary>
        /// minimize Event
        /// </summary>
        public DelegateCommand<object> MinimizeCommand { get; set; }
        /// <summary>
        /// minimize Event
        /// </summary>
        public DelegateCommand<object> MaximizeCommand { get; set; }
        /// <summary>
        /// minimize Event
        /// </summary>
        public DelegateCommand<object> CloseCommand { get; set; }
        /// <summary>
        /// 아코디언 접힘버튼 이벤트
        /// </summary>
        public DelegateCommand<object> MenuShowHidenCommand { get; set; }
        /// <summary>
        /// 퀵메뉴버튼 접힘버튼 이벤트
        /// </summary>
        public DelegateCommand<object> QuickShowHidenCommand { get; set; }
        /// <summary>
        /// 퀵메뉴버튼 관리버튼 이벤트
        /// </summary>
        public DelegateCommand<object> QuickMngCommand { get; set; }

        public RelayCommand<object> CallPageCmd { get; set; }

        public RelayCommand<object> UserInfoMngCommand { get; set; }

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public MainWinViewModel(IRegionManager _regionManager)
        {
            /// 프리즘 regionManager 초기화...
            regionManager = _regionManager;
            FmsUtil.__regionManager = _regionManager; //regionManager 전역변수로 서정

            LoadedCommand = new DelegateCommand<object>(OnLoaded);

            MenuControlCommand = new DelegateCommand<object>(MenuControlAction); //아코디언메뉴클릭
            WindowMoveCommand = new DelegateCommand<object>(WindowMoveAction);
            MinimizeCommand = new DelegateCommand<object>(MinimizeAction);
            MaximizeCommand = new DelegateCommand<object>(MaximizeAction);
            CloseCommand = new DelegateCommand<object>(CloseAction);
            MenuShowHidenCommand = new DelegateCommand<object>(MenuShowHidenAction);
            QuickShowHidenCommand = new DelegateCommand<object>(QuickShowHidenAction);
            QuickMngCommand = new DelegateCommand<object>(QuickMngAction);



            UserInfoMngCommand = new RelayCommand<object>(delegate (object obj) {

                DashWinView dashWinView = new DashWinView();
                if (dashWinView.ShowDialog() is bool)
                {
                    //재조회
                }
            });

            //시설물팝업에서 시설물메뉴화면 호출작업
            CallPageCmd = new RelayCommand<object>(delegate (object obj) {

                FctDtl fctDtl = obj as FctDtl;
            });
        }



        #region ==========  Event 정의 ========== 
        /// <summary>
        /// Loaded 이벤트
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            if (obj == null) return;

            try
            {
                mainwin = obj as MainWin;
                borderTop = mainwin.FindName("mainwin") as Border;

                // 0.컨텐트 이벤트핸들러 적용 - 화면 ContentRendered 이벤트발생시
                mainwin.ContentRendered += Mainwin_ContentRendered;

                // 1.메뉴로딩
                MenuDataInit(obj);


                // 2.테마일괄적용...
                ThemeApply.Themeapply(mainwin);

                // 3.즐겨찾기
                stQuickMenu = mainwin.FindName("stQuickMenu") as StackPanel;
                QuickMnuBinding();

                // 4.기타기능처리
                Logs.progressunlimit = mainwin.FindName("progressunlimit") as ProgressBar; //로딩바설정
                Messages.AppNotificationService = mainwin.FindName("AppNotificationService") as DevExpress.Mvvm.UI.NotificationService; //알림토스트






                /* ArcGis 2D-MapView 로딩
                 */
                //regionManager.Regions["ContentRegion"].RemoveAll();
                //regionManager.RequestNavigate("ContentRegion", new Uri("Map4View", UriKind.Relative));
                //regionManager.RequestNavigate("ContentRegion", new Uri("SketchOnMap", UriKind.Relative));
                //regionManager.RequestNavigate("ContentRegion", new Uri("OfflineBasemapByReference", UriKind.Relative));
                //regionManager.RequestNavigate("ContentRegion", new Uri("Map3View", UriKind.Relative));
                //regionManager.RequestNavigate("ContentRegion", new Uri("Map2View", UriKind.Relative));
                regionManager.RequestNavigate("ContentRegion", new Uri("MapMainView", UriKind.Relative));

                //regionManager.RequestNavigate("ContentRegion", new Uri("MainWindow", UriKind.Relative));



                Messages.NotificationBox("InfoFMS", "InfoFMS에 접속하셨습니다.", "InfoFMS에 접속하셨습니다.");
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
            }
        }




        /// <summary>
        /// 탑메뉴 클릭시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            AccordionControl accrMenu = mainwin.FindName("accrMenu") as AccordionControl;

            foreach (AccordionItem item in accrMenu.Items)
            {
                //메인윈도우에 객체를 삭제/추가한다...
                mainwin.UnregisterName(item.Name);

                foreach (AccordionItem citem in item.Items)
                {
                    item.UnregisterName(citem.Name);
                }
            }

            accrMenu.Items.Clear();

            DataRow[] drmidMENU;
            DataRow[] drsmMENU;
            drmidMENU = dtMenuList.Select("MNU_STEP = '2' AND UPPER_CD ='" + ((Button)sender).Name.Replace("MN_", "").ToString() + "'", "ORD");

            //중메뉴
            foreach (DataRow r in drmidMENU)
            {
                try
                {
                    //중메뉴 권한 필터링 (N)인경우 NO
                    if (!Logs.htPermission[r["MNU_CD"].ToString()].ToString().Equals("N"))
                    {
                        AccordionItem acctwoitem = new AccordionItem
                        {
                            Name = "MN_" + r["MNU_CD"].ToString(),
                            Header = r["MNU_NM"].ToString(),
                            Foreground = new SolidColorBrush(Colors.White),
                            FontSize = 14,
                            Glyph = new BitmapImage(new Uri("/Resources/Images/MNUImage/" + r["MNU_IMG"].ToString(), UriKind.Relative))
                        };
                        if (ThemeApply.strThemeName.Equals("GTINavyTheme"))
                        {
                            acctwoitem.Glyph = new BitmapImage(new Uri("/Resources/Navy/Images/MNUImage/" + r["MNU_IMG"].ToString(), UriKind.Relative));
                        }
                        else
                        {
                            acctwoitem.Glyph = new BitmapImage(new Uri("/Resources/Blue/Images/MNUImage/" + r["MNU_IMG"].ToString(), UriKind.Relative));
                        }
                        acctwoitem.Margin = new Thickness(3, 0, 3, 0);

                        mainwin.RegisterName(acctwoitem.Name, acctwoitem); //메인윈도우에 객체를 추가한다...
                        accrMenu.Items.Add(acctwoitem);

                        drsmMENU = null;
                        drsmMENU = dtMenuList.Select("MNU_STEP = '3' AND UPPER_CD ='" + acctwoitem.Name.Replace("MN_", "").ToString() + "'", "ORD");

                        //소메뉴
                        foreach (DataRow drthree in drsmMENU)
                        {
                            try
                            {
                                //소메뉴 권한 필터링 (N)인경우 NO
                                if (!Logs.htPermission[drthree["MNU_CD"].ToString()].ToString().Equals("N"))
                                {
                                    AccordionItem accthreeitem = new AccordionItem
                                    {
                                        Name = "MN_" + drthree["MNU_CD"].ToString(),
                                        Header = " ⁃ " + drthree["MNU_NM"].ToString(),
                                        FontSize = 14,
                                        Foreground = new SolidColorBrush(Colors.White),
                                        //Background = (Brush)new BrushConverter().ConvertFrom("#195A92")
                                    };
                                    mainwin.RegisterName(accthreeitem.Name, accthreeitem);
                                    acctwoitem.Items.Add(accthreeitem);
                                }
                            }
                            catch (Exception ex)
                            {
                                Messages.ErrLog(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messages.ErrLog(ex);
                }

                //편의를 위해 확장 추후 결정
                accrMenu.ExpandAll();
            }
        }


        #endregion





        #region ==========  기능 정의 ========== 

        /// <summary>
        /// 메뉴, 기본 초기화
        /// </summary>
        private void MenuDataInit(object obj)
        {
            try
            {
                mainwin = obj as MainWin;

                Button btnUser = mainwin.FindName("btnUser") as Button;
                StackPanel spMenuArea = mainwin.FindName("spMenuArea") as StackPanel;

                btnUser.Content = Logs.strLogin_ID;


                htconditions.Clear();
                htconditions.Add("SYS_CD", "000007");

                dtMenuList = work.Select_MNU_LIST(htconditions);

                foreach (DataRow r in dtMenuList.Select("MNU_STEP = '1'", "ORD"))
                {
                    try
                    {
                        if (!Logs.htPermission[r["MNU_CD"].ToString()].ToString().Equals("N"))
                        {
                            Button btnMenu = new Button
                            {
                                Name = "MN_" + r["MNU_CD"].ToString(),
                                Content = r["MNU_NM"].ToString(),
                                Style = Application.Current.Resources["MainMNUButton"] as Style,
                                //Tag = "/Resources/Images/MNUImage/" + r["MNU_IMG"].ToString()
                            };
                            if (ThemeApply.strThemeName.Equals("GTINavyTheme"))
                            {
                                btnMenu.Tag = "/Resources/Navy/Images/MNUImage/" + r["MNU_IMG"].ToString();
                            }
                            else
                            {
                                btnMenu.Tag = "/Resources/Blue/Images/MNUImage/" + r["MNU_IMG"].ToString();
                            }

                            btnMenu.Click += btnMenu_Click;
                            mainwin.RegisterName(btnMenu.Name, btnMenu);
                            spMenuArea.Children.Add(btnMenu);
                        }
                    }
                    catch (Exception ex)
                    {
                        Messages.ErrLog(ex);
                    }
                }


                Logs.configChange(Logs.WNMSConfig);
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
            }
        }


        /// <summary>
        ///  메인화면 ContentRendered 이벤트 핸들러 - 첫번째 탑메뉴클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mainwin_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if ((mainwin.FindName("spMenuArea") as StackPanel).Children.Count > 0)
                    btnMenu_Click(((mainwin.FindName("spMenuArea") as StackPanel).Children[0] as Button), null);
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 메뉴선택 이벤트
        /// </summary>
        /// <param name="obj"></param>
        private void MenuControlAction(object obj)
        {
            AccordionControl accrMenu = mainwin.FindName("accrMenu") as AccordionControl;

            try
            {
                AccordionItem accitemSelect = accrMenu.SelectedItem as AccordionItem;

                if (accitemSelect != null)
                {
                    strSelectMenu = accitemSelect.Name.Replace("MN_", "");
                    DataRow[] dr = dtMenuList.Select("MNU_CD = '" + strSelectMenu + "' AND MNU_STEP = '3'");

                    if (dr.Length == 1)
                    {
                        if (!Logs.htPermission[strSelectMenu].ToString().Equals("N"))
                        {
                            if (!dr[0]["MNU_PATH"].ToString().Equals(""))
                            {
                                /*
                                 */


                                //Grid gridtitle = mainwin.FindName("gridtitle") as Grid;
                                //gridtitle.RowDefinitions[0].Height = new GridLength(40, GridUnitType.Pixel);

                                //regionManager.Regions["ContentRegion"].RemoveAll();
                                //regionManager.RequestNavigate("ContentRegion", new Uri(dr[0]["MNU_PATH"].ToString(), UriKind.Relative));



                                // 선택된 메뉴를 세션에 저장
                                Logs.strFocusMNU_CD = accitemSelect.Name.Split('_')[1].ToString();


                                /*
                                 * ContentsRegion표시하지않고 팝업윈도우를 호출
                                 */

                                // 0.현재열려있는 팝업을 닫는다
                                /* Popup클래그 방식
                                pmain.IsOpen = false; 
                                 */
                                try
                                {
                                    FmsUtil.popWinView.Close();
                                }
                                catch (Exception) { }



                                if (FmsUtil.IsNull(dr[0]["MNU_PATH"].ToString()))
                                {
                                    Messages.ShowErrMsgBox("잘못된 메뉴경로입니다.");
                                    return;
                                }




                                // 2.점검관리화면은 단독윈도우형태
                                if (dr[0]["MNU_PATH"].ToString().Contains("Mntc/View/ChkSchListView.xaml") )
                                {
                                    //클래스풀패키지명 만들기
                                    string className = "GTI.WFMS.Modules";
                                    var paths = dr[0]["MNU_PATH"].ToString().Split('/');
                                    foreach (string p in paths)
                                    {
                                        className += "." + p.Replace(".xaml", "");
                                    }

                                    //Type t = ModulesAssembly.GetType("GTI.WFMS.Modules.Mntc.View.ChkSchListView");
                                    Type t = ModulesAssembly.GetType(className);
                                    FmsUtil.popWinView = Activator.CreateInstance(t) as Window;
                                    //popWinView = new ChkSchListView();

                                    //공통팝업창 사이즈 초기화
                                    FmsUtil.popWinView.Height = 631;
                                    //팝업결과리턴
                                    if (FmsUtil.popWinView.ShowDialog() is bool)
                                    {
                                        //재조회
                                    }
                                }
                                // 3.일반 업무화면은 Page 형태의 팝업
                                else
                                {
                                    /* Window 공통화면 형태
                                     */
                                    FmsUtil.popWinView = new PopWinView(dr[0]["MNU_PATH"].ToString());
                                    Label lbTitle = FmsUtil.popWinView.FindName("lbTitle") as Label;//화면타이틀
                                    lbTitle.Content = dr[0]["MNU_NM"].ToString();

                                    //팝업결과리턴
                                    if (FmsUtil.popWinView.ShowDialog() is bool)
                                    {
                                        //재조회
                                    }



                                    /* Popup 공통화면 형태
                                    try
                                    {
                                        pmain = new PopMain(dr[0]["MNU_PATH"].ToString());
                                        pmain.DataContext = this;//팝업에 현재데이터컨텍스트를 전달한다...
                                    }
                                    catch (Exception)
                                    {
                                        return;
                                    }

                                    Label lbTitle = pmain.FindName("lbTitle") as Label;//화면타이틀
                                    lbTitle.Content = dr[0]["MNU_NM"].ToString();

                                    pmain.PlacementTarget = borderTop;
                                    pmain.Placement = PlacementMode.Bottom;
                                    pmain.VerticalOffset = 100;
                                    pmain.Placement = PlacementMode.Left;
                                    pmain.HorizontalOffset = 100;
                                    pmain.IsOpen = true;
                                    pmain.StaysOpen = false;
                                    //pmain.Focusable = true;
                                    //FmsUtil.__popMain = pmain; //열린팝업을 전역변수로 저장해놓음
                                     */
                                }


                            }
                            else
                            {
                                Messages.ShowInfoMsgBox("메뉴 경로가 부적합 합니다.");
                                return;
                            }
                        }
                        //else
                        //{
                        //    Messages.ShowInfoMsgBox("해당메뉴에 권한이 없습니다.");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }



        /// <summary>
        /// 창을 최대화 한다.
        /// </summary>
        /// <param name="obj">object</param>
        private void MaximizeAction(object obj)
        {
            if (mainwin.WindowState == WindowState.Maximized)
            {
                mainwin.WindowState = WindowState.Normal;
            }
            else if (mainwin.WindowState == WindowState.Normal)
            {
                mainwin.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// 창을 최소화 한다
        /// </summary>
        /// <param name="arg">object</param>
        private void MinimizeAction(object arg)
        {
            mainwin.WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// 창화면이동 
        /// (튀는 현상 수정 완료)
        /// </summary>
        /// <param name="obj"></param>
        private void WindowMoveAction(object obj)
        {
            try
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (mainwin.WindowState == WindowState.Maximized)
                    {
                        mainwin.Top = Mouse.GetPosition(mainwin).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                        mainwin.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(mainwin).X + 20;

                        mainwin.WindowState = WindowState.Normal;
                    }
                    mainwin.DragMove();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 창을 닫는다.
        /// </summary>
        /// <param name="strArg">object</param>
        private void CloseAction(object strArg)
        {
            try
            {
                if (DXMessageBox.Show("시스템을 종료합니다.", "InfoFacility", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    Hashtable logconditions = new Hashtable();
                    logconditions.Add("USER_ID", Logs.strLogin_ID);
                    logconditions.Add("MNU_CD", "9999");
                    logconditions.Add("CONN_IP", Logs.strLogin_IP);
                    //work.Insert_Log(logconditions);
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 아코디언메뉴 Show/Hiden
        /// </summary>
        /// <param name="obj"></param>
        private void MenuShowHidenAction(object obj)
        {
            Storyboard sb;

            AccordionControl accr = (AccordionControl)mainwin.FindName("accrMenu");
            Button btn = (Button)mainwin.FindName("btnMenuSlide");

            if (bMenuShowHiden)
            {
                sb = mainwin.FindResource("Menuin") as Storyboard;

                btn.Margin = new Thickness(0, 0, 18, 0);
                accr.CollapseAll();
                accr.RootItemExpandButtonPosition = ExpandButtonPosition.None;
                accr.ExpandItemOnHeaderClick = false;
            }
            else
            {
                sb = mainwin.FindResource("Menuout") as Storyboard;

                btn.Margin = new Thickness(0, 0, 6, 0);
                accr.ExpandAll();
                accr.RootItemExpandButtonPosition = ExpandButtonPosition.Right;
                accr.ExpandItemOnHeaderClick = true;
            }

            sb.Begin(mainwin);
            bMenuShowHiden = !bMenuShowHiden;
        }

        /// <summary>
        /// 퀵메뉴 Show/Hiden 이벤트
        /// </summary>
        /// <param name="obj"></param>
        private void QuickShowHidenAction(object obj)
        {
            Storyboard sb;

            Button btnQuick = (Button)mainwin.FindName("btnQuick");

            if (bQuickShowHiden)
            {
                sb = mainwin.FindResource("QuickShow") as Storyboard;
                btnQuick.Style = Application.Current.Resources["btn_Quick_Slide_CLOSE"] as Style;
            }
            else
            {
                sb = mainwin.FindResource("QuickHiden") as Storyboard;
                btnQuick.Style = Application.Current.Resources["btn_Quick_Slide_OPEN"] as Style;
            }

            sb.Begin(mainwin);
            bQuickShowHiden = !bQuickShowHiden;
        }



        /// <summary>
        /// 단축키 입력 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mainwin_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                AccordionControl accrMenu = mainwin.FindName("accrMenu") as AccordionControl;
                DataRow[] dr = null;

                if (Keyboard.IsKeyDown(Key.F1))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F1'");
                else if (Keyboard.IsKeyDown(Key.F2))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F2'");
                else if (Keyboard.IsKeyDown(Key.F3))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F3'");
                else if (Keyboard.IsKeyDown(Key.F4))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F4'");
                else if (Keyboard.IsKeyDown(Key.F5))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F5'");
                else if (Keyboard.IsKeyDown(Key.F6))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F6'");
                else if (Keyboard.IsKeyDown(Key.F7))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F7'");
                else if (Keyboard.IsKeyDown(Key.F8))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F8'");
                else if (Keyboard.IsKeyDown(Key.F9))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F9'");
                else if (Keyboard.IsKeyDown(Key.F10))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F10'");
                else if (Keyboard.IsKeyDown(Key.F11))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F11'");
                else if (Keyboard.IsKeyDown(Key.F12))
                    dr = dtQuickMenuList.Select("SHRTEN_KEY = 'F12'");

                if (dr != null && dr.Length == 1)
                {
                    dr = dtMenuList.Select("MNU_CD = '" + dr[0]["MNU_CD"].ToString() + "' AND MNU_STEP = '3'");

                    if (dr.Length == 1)
                    {
                        if (!Logs.htPermission[dr[0]["MNU_CD"]].ToString().Equals("N"))
                        {
                            if (!dr[0]["MNU_PATH"].ToString().Equals(""))
                            {
                                btnMenu_Click(mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString().Substring(0, 4)), null);
                                accrMenu.SelectedItem = mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString());
                                MenuControlAction(null);

                                if (!bQuickShowHiden)
                                    QuickShowHidenAction(null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }



        #endregion






        #region ======= 즐겨찾기관련 =========


        /// <summary>
        /// 즐겨찾기관리 
        /// </summary>
        /// <param name="obj"></param>
        private void QuickMngAction(object obj)
        {
            PopupQuickMenuMng popupQuickMng = new PopupQuickMenuMng();
            popupQuickMng.Closed += popupQuickMng_Closed;
            popupQuickMng.ShowDialog();
        }


        /// <summary>
        /// 즐겨찾기 관리 닫기 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popupQuickMng_Closed(object sender, EventArgs e)
        {
            QuickMnuBinding();
        }



        /// <summary>
        /// 즐겨찾기 바인딩
        /// </summary>
        /// 
        public void QuickMnuBinding()
        {
            try
            {
                // 즐겨찾기메뉴 조회
                Hashtable param = new Hashtable();
                param.Add("sqlId", "Select_BASE_FLOW_CALC_INFO_R");
                param.Add("SYS_CD", "000007");
                param.Add("USER_ID", Logs.strLogin_ID);

                dtQuickMenuList = BizUtil.SelectList(param);


                stQuickMenu.Children.Clear();

                foreach (DataRow dr in dtQuickMenuList.Rows)
                {
                    Button btnQuickMenu = new Button
                    {
                        Name = "MN_" + dr["MNU_CD"].ToString(),
                        Content = dr["MNU_NM"].ToString(),
                        Tag = dr["SHRTEN_KEY"].ToString()
                    };
                    btnQuickMenu.Click += BtnQuickMenu_Click;
                    btnQuickMenu.SetResourceReference(Control.StyleProperty, "Quick_Menu_Button");

                    if (btnQuickMenu.Tag.ToString().Equals(""))
                    {
                        btnQuickMenu.Tag = "Collapsed";
                    }

                    stQuickMenu.Children.Add(btnQuickMenu);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 즐겨찾기 액션
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuickMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                strSelectMenu = (sender as Button).Name.Replace("MN_", "");
                AccordionControl accrMenu = mainwin.FindName("accrMenu") as AccordionControl;

                DataRow[] dr = dtMenuList.Select("MNU_CD = '" + strSelectMenu + "' AND MNU_STEP = '3'");

                if (dr.Length == 1)
                {
                    if (!Logs.htPermission[strSelectMenu].ToString().Equals("N"))
                    {
                        if (!dr[0]["MNU_PATH"].ToString().Equals(""))
                        {
                            btnMenu_Click(mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString().Substring(0, 4)), null);
                            accrMenu.SelectedItem = mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString());
                            MenuControlAction(null);

                            if (!bQuickShowHiden)
                                QuickShowHidenAction(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }





        /// <summary>
        /// GIS에서 메뉴화면선택 수동액션
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelMenuPage(string strSelectMenu)
        {
            try
            {
                //strSelectMenu = (sender as Button).Name.Replace("MN_", "");
                AccordionControl accrMenu = mainwin.FindName("accrMenu") as AccordionControl;

                DataRow[] dr = dtMenuList.Select("MNU_CD = '" + strSelectMenu + "' AND MNU_STEP = '3'");

                if (dr.Length == 1)
                {
                    if (!Logs.htPermission[strSelectMenu].ToString().Equals("N"))
                    {
                        if (!dr[0]["MNU_PATH"].ToString().Equals(""))
                        {
                            //탑메뉴버튼 수동클릭
                            btnMenu_Click(mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString().Substring(0, 4)), null);
                            //레프트메뉴(아코디언) 표시
                            accrMenu.SelectedItem = mainwin.FindName("MN_" + dr[0]["MNU_CD"].ToString());
                            //레프트메뉴 수동선택
                            MenuControlAction(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        #endregion
    }
}