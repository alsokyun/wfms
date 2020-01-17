using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Main.Work;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.Converters;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GTI.WFMS.Main.View
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        MainWork work = new MainWork();
        Hashtable htconditions = new Hashtable();

        protected Thread thread;  //쓰레드 선언
        protected System.Timers.Timer timer; //자동쓰레드 타이머

        DataTable dtDBInfo; //
        DataTable dtBaseSiteInfo = new DataTable(); //DB연결정보

        bool bcbSiteping = false;

        public Login()
        {
            InitializeComponent();


            //팝업창 테마적용
            if (Properties.Settings.Default.strThemeName.Equals(""))
                ThemeApply.strThemeName = "GTINavyTheme";
            else
                ThemeApply.strThemeName = Properties.Settings.Default.strThemeName;
            ThemeApply.ThemeChange(this);
            ThemeApply.Themeapply(this);

            Loaded += Login_Loaded;
            Unloaded += Login_Unloaded;
        }



        #region  ========== 이벤트핸들러 ==========

        /// <summary>
        /// 로그인처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //로그인 정보가 없는 경우 체크
                if (cbSite.Text == "" || txtID.Text == "" || pwdPW.Text == "")
                {
                    Messages.ShowInfoMsgBox("로그인 정보가 틀립니다.");
                    return;
                }

                if (dtDBInfo.Rows.Count != 1)
                {
                    Messages.ShowInfoMsgBox("해당 프로그램의 접속정보가 없습니다.");
                    return;
                }

                htconditions.Clear();
                htconditions.Add("SITE_CD", cbSite.EditValue.ToString());
                htconditions.Add("USER_ID", txtID.EditValue.ToString());
                htconditions.Add("USER_PWD", EncryptionConvert.Base64Encoding(pwdPW.EditValue.ToString()));
                htconditions.Add("SYS_CD", "000007");

                DataTable dtLoginCheck = new DataTable();
                dtLoginCheck = work.LoginCheck(htconditions);

                //사용자가 입력한 로그인 정보로 일치하는 데이터가 없을 경우
                if (dtLoginCheck.Rows.Count == 0)
                {
                    Messages.ShowInfoMsgBox("로그인 정보가 틀립니다.");
                    return;
                }

                //비밀번호 체크
                if (!dtLoginCheck.Rows[0]["USER_PWD"].ToString().Equals(htconditions["USER_PWD"].ToString()))
                {
                    Messages.ShowInfoMsgBox("로그인 정보가 틀립니다.");
                    return;
                }

                //시스템 권한 체크
                if (!dtLoginCheck.Rows[0]["SYS_CD"].ToString().Equals("1"))
                {
                    Messages.ShowInfoMsgBox("해당 시스템에 권한이 없습니다.");
                    return;
                }

                //아이디 저장 체크박스 확인 후 최근 접속아이디 및 체크 값 setting.
                Properties.Settings.Default.bSaveID = (bool)chkSaveID.EditValue;
                if ((bool)chkSaveID.EditValue)
                {
                    Properties.Settings.Default.strRecentID = txtID.Text;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.strRecentID = "";
                    Properties.Settings.Default.Save();
                }


                //로그인 성공이후
                //..로그인 성공한 아이디, 사이트, IP  저장
                Logs.strLogin_ID = txtID.Text;
                Logs.strLogin_SITE = cbSite.EditValue.ToString();
                Logs.strLogin_IP = ((IPAddress[])Dns.GetHostAddresses(""))[1].ToString();

                //..로그인 성공한 메뉴별 권한 저장
                DataTable dtPermission = new DataTable();
                dtPermission = work.Select_LoginUser_Permission(htconditions);

                foreach (DataRow dr in dtPermission.Rows)
                {
                    Logs.htPermission.Add(dr["MNU_CD"].ToString(), dr["MNU_AUTH"].ToString());
                }

                Logs.WNMSConfig.strIP = dtDBInfo.Rows[0]["IP_ADDR"].ToString();
                Logs.WNMSConfig.strPort = dtDBInfo.Rows[0]["PORT_NO"].ToString();
                Logs.WNMSConfig.strSID = dtDBInfo.Rows[0]["SID_NM"].ToString();
                Logs.WNMSConfig.strID = dtDBInfo.Rows[0]["CONN_ID"].ToString();
                Logs.WNMSConfig.strPWD = dtDBInfo.Rows[0]["CONN_PWD"].ToString();
                //Logs.configChange(infomanagerConfig);


                // App.xaml의 팝업호출지점으로 리턴
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }




        // 창닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("시스템을 종료합니다.", "InfoFacility", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        // 엔터키처리
        private void PwdPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(null, null);
            }
        }


        /// <summary>
        /// DB커넥션 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDataBaseInfoEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dtDBInfo != null)  //사업소 DB연결이 존재하면
            {
                if (txtID.Text.Equals(""))
                {
                    Messages.ShowInfoMsgBox("아이디를 입력해 주세요.");
                    return;
                }

                DBManagement dbManagement = new DBManagement(dtDBInfo, this);
                dbManagement.ShowDialog();
            }
            else
            {
                DBManagement dbManagement = new DBManagement(this);
                dbManagement.ShowDialog();
            }
        }


        /// <summary>
        /// 윈도우 마우스 드래그 이동처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdTitle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized)
                {
                    Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    WindowState = WindowState.Normal;
                }
                DragMove();
            }
        }




        private void Login_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            thread.Abort();
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            /// 2.데이터 초기화
            InitializeEvent();
            InitializeData();

            try
            {
                thread = new Thread(new ThreadStart(thread_FX));
                thread.Name = "thread";
                thread.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void thread_FX()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                CheckNetStat();
            }
            catch (Exception ex) { Messages.ErrLog(ex); }
        }

        /// <summary>
        /// 접속상태 체크
        /// 사업소정보설정폼에서 셋팅한 IP주소로 ping 체크
        /// 핑 응답이 있을시 '정상' 표시
        /// 사업소정보설정 저장값이 없으면 '비정상' 표시
        /// </summary>
        public void CheckNetStat()
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action(delegate ()
                    {
                        if (cbSite.EditValue != null)
                            bcbSiteping = true;
                        else
                            bcbSiteping = false;
                    }));

                if (bcbSiteping)
                {
                    if (dtDBInfo == null)
                    {
                        htconditions.Clear();
                        Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                            new Action(delegate ()
                            {
                                htconditions.Add("SITE_CD", cbSite.EditValue.ToString());
                            }));
                        htconditions.Add("SYS_CD", "000007");
                        dtDBInfo = work.SelectDBInfo(htconditions);
                    }

                    if (dtDBInfo.Rows.Count == 1)
                    {
                        if (!dtDBInfo.Rows[0][0].ToString().Equals(""))
                        {
                            string strSiteIP = dtDBInfo.Rows[0][0].ToString();
                            //네트워크 정상 비정상 체크
                            if (PingChecker(strSiteIP))
                            {
                                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                                    new Action(delegate ()
                                    {
                                        imgConnAbnormal.Visibility = Visibility.Collapsed;
                                    }));
                            }
                            else
                            {
                                Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                                    new Action(delegate ()
                                    {
                                        imgConnAbnormal.Visibility = Visibility.Visible;
                                    }));
                            }
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                        new Action(delegate ()
                        {
                            imgConnAbnormal.Visibility = Visibility.Visible;
                        }));
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        #endregion


        #region =========== 기능 ===============

        private void InitializeEvent()
        {
            /// 1.이벤트 초기화
            btnLogin.Click += BtnLogin_Click; //로그인버튼 이벤트 추가
            btnClose.Click += BtnClose_Click; //닫기버튼

            btnDataBaseInfoEdit.Click += BtnDataBaseInfoEdit_Click;  //DB연결 설정팝업
            pwdPW.KeyDown += PwdPW_KeyDown; //엔터키
            bdTitle.PreviewMouseDown += BdTitle_PreviewMouseDown; //창드래그
        }

        /// <summary>
        /// 데이터 초기화
        /// </summary>
        internal void InitializeData()
        {
            /// 2.데이터객체 초기화
            try
            {
                //DB연결 정보가 셋팅되어 있으면 사업소가져오기
                if (configValueCheck())
                {
                    try
                    {
                        // 사업소정보 콤보박스 바인딩
                        dtBaseSiteInfo = work.SelectBaseSiteInfo(null);

                        if (dtBaseSiteInfo.Rows.Count > 0)
                        {
                            cbSite.ItemsSource = dtBaseSiteInfo;
                            cbSite.DisplayMember = "SITE_NM";
                            cbSite.ValueMember = "SITE_CD";
                            cbSite.SelectedIndex = 0;

                            //최근 저장되어 있는 아이디 바인딩
                            if (Properties.Settings.Default.bSaveID)
                            {
                                chkSaveID.EditValue = Properties.Settings.Default.bSaveID;

                                if (!Properties.Settings.Default.strRecentID.Equals(""))
                                {
                                    txtID.Text = Properties.Settings.Default.strRecentID;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Messages.ShowErrMsgBoxLog(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }


            ///// 3.접속상태체크
            //try
            //{
            //    if (cbSite.EditValue != null) //사업소가 없으면 접속실패
            //    {
            //        if (dtDBInfo == null)
            //        {
            //            htconditions.Clear();
            //            htconditions.Add("SITE_CD", cbSite.EditValue.ToString());
            //            htconditions.Add("SYS_CD", "000007");
            //            dtDBInfo = work.SelectDBInfo(htconditions);
            //        }

            //        if (dtDBInfo.Rows.Count == 1)
            //        {
            //            if (!dtDBInfo.Rows[0][0].ToString().Equals(""))
            //            {
            //                string strSiteIP = dtDBInfo.Rows[0][0].ToString();
            //                //네트워크 정상 비정상 체크
            //                if (PingChecker(strSiteIP))
            //                {
            //                    imgConnAbnormal.Visibility = Visibility.Collapsed;
            //                    imgConnNormal.Visibility = Visibility.Visible;
            //                }
            //                else
            //                {
            //                    imgConnAbnormal.Visibility = Visibility.Visible;
            //                    imgConnNormal.Visibility = Visibility.Collapsed;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        imgConnAbnormal.Visibility = Visibility.Visible;
            //        imgConnNormal.Visibility = Visibility.Collapsed;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Messages.ShowErrMsgBoxLog(ex);
            //}

            txtID.Focus();
        }



        /// <summary>
        /// DB연결정보 셋팅 체크
        /// Property 값 6개 모두 설정되어 있을 때 true 반환.
        /// RES_DB_INS_DEFAULT = Tibero / Oracle
        /// </summary>
        /// <returns></returns>
        public bool configValueCheck()
        {
            bool bConfChk = false;

            if (!GTIFramework.Properties.Settings.Default.strIP.Equals("")
                && !GTIFramework.Properties.Settings.Default.strPort.Equals("")
                && !GTIFramework.Properties.Settings.Default.strSID.Equals("")
                && !GTIFramework.Properties.Settings.Default.strID.Equals("")
                && !GTIFramework.Properties.Settings.Default.strPWD.Equals("")
                && !GTIFramework.Properties.Settings.Default.RES_DB_INS_DEFAULT.Equals(""))
            {
                bConfChk = true;
            }
            return bConfChk;
        }


        /// <summary>
        /// 핑 체크 (유효한 ip 범위로 넘겨줘야함)
        /// 핑 관련 클래스를 통해 핑 체크를 시도한다.
        /// </summary>
        /// <param name="strip"></param>
        /// <returns></returns>
        public bool PingChecker(string strip)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(strip);

                if (reply.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return false;
            }
        }

        #endregion

        private void BtnLogin_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
