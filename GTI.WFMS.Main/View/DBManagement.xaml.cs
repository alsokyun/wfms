using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Main.Work;
using GTIFramework.Common.ConfigClass;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GTI.WFMS.Main.View
{
    /// <summary>
    /// DBManagement.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DBManagement : Window
    {
        MainWork work = new MainWork();

        private Login login;
        private DataTable dtDBInfo;

        ConnectConfig oldConfig = new ConnectConfig();
        ConnectConfig userConfig = new ConnectConfig();
        string strOldDBConfig = string.Empty; //DB종류

        bool bConnTest = false;


        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="login"></param>

        //사업소 DB연결이 없으면
        public DBManagement(Login login)
        {
            // 0.생성자 기본처리
            InitializeComponent();
            ThemeApply.Themeapply(this);
            this.login = login;



            // 1.기존접속정보 저장
            oldConfig.strIP = GTIFramework.Properties.Settings.Default.strIP;
            oldConfig.strPort = GTIFramework.Properties.Settings.Default.strPort;
            oldConfig.strSID = GTIFramework.Properties.Settings.Default.strSID;
            oldConfig.strID = GTIFramework.Properties.Settings.Default.strID;
            oldConfig.strPWD = GTIFramework.Properties.Settings.Default.strPWD;
            strOldDBConfig = GTIFramework.Properties.Settings.Default.RES_DB_INS_DEFAULT;


            // 2.로딩이벤트 처리
            Loaded += DBManagement_Loaded;

            btnConnTest.Click += BtnConnTest_Click;
            btnSave.Click += BtnSave_Click;
            btnClose.Click += BtnClose_Click;
            btnXSignClose.Click += BtnClose_Click;
            bdTitle.PreviewMouseDown += BdTitle_PreviewMouseDown;

        }

        //사업소 DB연결이 존재하면
        public DBManagement(DataTable dtDBInfo, Login login) : this(login)
        {
            this.dtDBInfo = dtDBInfo;

        }






        #region ========== 이벤트핸들러 =============

        /// <summary>
        /// 로딩시 초기화작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBManagement_Loaded(object sender, RoutedEventArgs e)
        {
            //DB구분 콤보박스 초기화
            DataTable dtDBCAT = new DataTable();

            DataColumn dcDTL_CD = new DataColumn("DTL_CD", typeof(string));
            DataColumn dcNM = new DataColumn("NM", typeof(string));
            dtDBCAT.Columns.Add(dcDTL_CD);
            dtDBCAT.Columns.Add(dcNM);

            //티베로 DB 추가
            DataRow drTibero = dtDBCAT.NewRow();
            drTibero["DTL_CD"] = "000007";
            drTibero["NM"] = "Tibero";
            dtDBCAT.Rows.Add(drTibero);

            //오라클 DB 추가
            DataRow drOracle = dtDBCAT.NewRow();
            drOracle["DTL_CD"] = "000002";
            drOracle["NM"] = "Oracle";
            dtDBCAT.Rows.Add(drOracle);

            cbDBCAT.ItemsSource = dtDBCAT;
            cbDBCAT.SelectedIndex = 0;

        }


        /// <summary>
        /// 접속테스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnTest_Click(object sender, RoutedEventArgs e)
        {
            if (cbDBCAT.EditValue != null)
            {
                //Tibero
               if (cbDBCAT.EditValue.Equals("000007"))
                {
                    Logs.setDBConfig("TIBEROConfig");
                }
                //Oracle
                else if (cbDBCAT.EditValue.Equals("000002"))
                {
                    Logs.setDBConfig("ORACLEConfig");
                }

                connectionionCheck();
            }
        }

        /// <summary>
        /// 접속정보저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool bValichk = true;

                #region 값 확인
                if (cbDBCAT.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("구분을 입력해주세요.");
                    cbDBCAT.Focus();
                    bValichk = false;
                    return;
                }
                if (txtIP.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("IP주소를 입력해주세요.");
                    txtIP.Focus();
                    bValichk = false;
                    return;
                }
                if (txtPort.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("PORT번호를 입력해주세요.");
                    txtPort.Focus();
                    bValichk = false;
                    return;
                }
                if (txtSID.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("SID이름을 입력해주세요.");
                    txtSID.Focus();
                    bValichk = false;
                    return;
                }
                if (txtConnID.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("접속 아이디를 입력해주세요.");
                    txtConnID.Focus();
                    bValichk = false;
                    return;
                }
                if (pwdConnPW.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("접속 비밀번호를 입력해주세요.");
                    pwdConnPW.Focus();
                    bValichk = false;
                    return;
                }
                #endregion

                if (bValichk)
                {
                    //접속 테스트 여부 화인
                    if (!bConnTest)
                    {
                        Messages.ShowInfoMsgBox("접속테스트를 확인하세요.");
                        btnConnTest.Focus();
                        return;
                    }

                    if (DXMessageBox.Show("저장하시겠습니까?", "InfoFacility", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        GTIFramework.Properties.Settings.Default.strIP = txtIP.EditValue.ToString();
                        GTIFramework.Properties.Settings.Default.strPort = txtPort.EditValue.ToString();
                        GTIFramework.Properties.Settings.Default.strSID = txtSID.EditValue.ToString();
                        GTIFramework.Properties.Settings.Default.strID = txtConnID.EditValue.ToString();
                        GTIFramework.Properties.Settings.Default.strPWD = pwdConnPW.EditValue.ToString();

                        if (cbDBCAT.EditValue.Equals("000007"))
                        {
                            GTIFramework.Properties.Settings.Default.RES_DB_INS_DEFAULT = "TIBEROConfig";
                        }
                        else if (cbDBCAT.EditValue.Equals("000002"))
                        {
                            GTIFramework.Properties.Settings.Default.RES_DB_INS_DEFAULT = "ORACLEConfig";
                        }

                        GTIFramework.Properties.Settings.Default.Save();

                        //접속정보 변경
                        Logs.DBdefault();

                        Messages.ShowInfoMsgBox("저장되었습니다.");




                        //로그인폼 refresh
                        this.login.InitializeData();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdTitle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    this.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    this.WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }

        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("작성중인 내용은 저장되지 않습니다.", "InfoFacility", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                Logs.configChange(oldConfig);
                Logs.setDBConfig(strOldDBConfig);
                this.Close();
            }
        }



        #endregion





        #region ======== 기능 ======== 
        /// <summary>
        /// 입력받은 DB 연결정보로 연결 테스트
        /// 티베로DB 테스트시 시도되는 포트번호가 열려있지만 티베로 포트(8629)가 아니라면 실패 응답까지 시간이 많이 소요됨. 
        /// 보완 필요
        /// </summary>
        private void connectionionCheck()
        {
            try
            {
                userConfig.strIP = txtIP.EditValue.ToString();
                userConfig.strPort = txtPort.EditValue.ToString();
                userConfig.strSID = txtSID.EditValue.ToString();
                userConfig.strID = txtConnID.EditValue.ToString();
                userConfig.strPWD = pwdConnPW.EditValue.ToString();
                Logs.configChange(userConfig);

                DataTable dtSysdate = work.Select_SYSDATE(null);
                if (dtSysdate.Rows.Count > 0)
                {
                    bConnTest = true;
                    Messages.ShowInfoMsgBox("접속 성공!");
                }
                else
                {
                    bConnTest = false;
                    Messages.ShowInfoMsgBox("접속 실패");
                }
            }
            catch (Exception ex)
            {
                bConnTest = false;
                Messages.ShowInfoMsgBox("접속 실패");
                Messages.ErrLog(ex);
            }
        }
        #endregion
    }
}
