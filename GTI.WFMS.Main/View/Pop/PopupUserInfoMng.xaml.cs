using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Main.Work;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.Converters;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
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

namespace GTI.WFMS.Main.View.Popup
{
    /// <summary>
    /// PopupUserInfoMng.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupUserInfoMng : Window
    {
        MainWin mainWin;


        DataTable dtLogUserInfo = new DataTable();

        public PopupUserInfoMng(MainWin _mainWin)
        {
            mainWin = _mainWin;
            InitializeComponent();
            ThemeApply.Themeapply(this);
            Loaded += PopupUserInfoMng_Loaded;
            ContentRendered += PopupUserInfoMng_ContentRendered;
        }

        private void PopupUserInfoMng_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeEvent();
            InitializeData();
        }

        #region 기능
        /// <summary>
        /// 데이터 초기화
        /// </summary>
        private void InitializeData()
        {
            try
            {
                Hashtable conditions = new Hashtable();
                conditions.Add("sqlId", "Select_CD_DTL_INFO_List");
                conditions.Add("MST_CD", "000004");
                //cbGrade.ItemsSource = cwork.Select_CD_DTL_INFO_List(conditions);
                cbGrade.ItemsSource = BizUtil.SelectList(conditions);

                conditions = new Hashtable();
                conditions.Add("sqlId", "Select_SITE_DEPT_INFO");
                lookUpEditDept.ItemsSource = BizUtil.SelectList(conditions);

                if (Properties.Settings.Default.strThemeName.Equals("GTINavyTheme"))
                    radionavy.IsChecked = true;
                else if (Properties.Settings.Default.strThemeName.Equals("GTIBlueTheme"))
                    radioblue.IsChecked = true;
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBoxLog(e);
            }
        }

        /// <summary>
        /// 이벤트 초기화
        /// </summary>
        private void InitializeEvent()
        {
            btnSave.Click += BtnSave_Click;
            btnClose.Click += BtnClose_Click;
            bdTitle.PreviewMouseDown += BdTitle_PreviewMouseDown;
            btnXSignClose.Click += BtnClose_Click;
        }
        #endregion

        #region 이벤트
        /// <summary>
        /// 저장 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Messages.ShowYesNoMsgBox("사용자정보를 저장 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    Hashtable htConditions = new Hashtable();

                    if (txtNM.Text.Equals(""))
                    {
                        Messages.ShowErrMsgBox("이름을 입력해주세요.");
                        txtNM.Focus();
                        return;
                    }

                    // 비밀번호 관련 입력란(현재,변경,변경확인)이 공란이면 비밀번호 변경 절차 없음.
                    if (!pwdCurrent.Text.ToString().Equals("") || !pwdChange.Text.ToString().Equals("") || !pwdChangeChk.Text.ToString().Equals(""))
                    {
                        if (pwdCurrent.Text.ToString().Equals(""))
                        {
                            Messages.ShowErrMsgBox("현재 비밀번호를 입력해주세요.");
                            pwdCurrent.Focus();
                            return;
                        }

                        if (pwdChange.Text.ToString().Equals(""))
                        {
                            Messages.ShowErrMsgBox("변경 비밀번호를 입력해주세요.");
                            pwdChange.Focus();
                            return;
                        }

                        if (pwdChangeChk.Text.ToString().Equals(""))
                        {
                            Messages.ShowErrMsgBox("변경 비밀번호 확인을 입력해주세요.");
                            pwdChangeChk.Focus();
                            return;
                        }

                        //입력한 현재 비밀번호가 일치하는지 체크
                        if (EncryptionConvert.Base64Encoding(pwdCurrent.Text.ToString()).Equals(dtLogUserInfo.Rows[0]["USER_PWD"].ToString()))
                        {
                            if (pwdChange.Text.ToString().Equals(pwdChangeChk.Text.ToString()))
                            {
                                htConditions.Add("USER_PWD", EncryptionConvert.Base64Encoding(pwdChange.EditValue.ToString()));
                            }
                            else
                            {
                                Messages.ShowErrMsgBox("변경 비밀번호가 일치하지 않습니다.");
                                pwdChange.Focus();
                                return;
                            }
                        }
                        else
                        {
                            Messages.ShowErrMsgBox("현재 비밀번호가 일치하지 않습니다.");
                            pwdCurrent.Focus();
                            return;
                        }

                    }
                    htConditions.Add("USER_ID", txtID.Text.ToString());
                    htConditions.Add("USER_NM", txtNM.Text.ToString());
                    htConditions.Add("DEPT_CD", lookUpEditDept.EditValue);
                    htConditions.Add("POS_CD", cbGrade.EditValue);
                    htConditions.Add("USER_TEL", txtPhone.Text.ToString());
                    htConditions.Add("EDT_ID", txtID.Text.ToString());
                    htConditions.Add("USE_YN", "Y");
                    htConditions.Add("DEL_YN", "N");

                    //work.Update_SYS_USER_INFO(htConditions);
                    htConditions.Add("sqlId", "Update_SYS_USER_INFO");
                    BizUtil.Update(htConditions);

                    MessageBox.Show("성공적으로 저장하였습니다.");
                    this.Close();

                    if (radioblue.IsChecked == true)
                        mainWin.Cmblue_Click(null, null);
                    else if (radionavy.IsChecked == true)
                        mainWin.Cmnavy_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 닫기 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 윈도우 창 이동 이벤트
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

        /// <summary>
        /// 로그인 중인 사용자 데이터 바인딩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupUserInfoMng_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                Hashtable htConditions = new Hashtable();
                htConditions.Add("USER_ID", Logs.strLogin_ID);
                htConditions.Add("sqlId", "Select_Log_User_Info");

                //dtLogUserInfo = work.Select_Log_User_Info(htConditions);
                dtLogUserInfo = BizUtil.SelectList(htConditions);

                if (dtLogUserInfo.Rows.Count == 1)
                {
                    txtID.Text = dtLogUserInfo.Rows[0]["USER_ID"].ToString();
                    txtNM.Text = dtLogUserInfo.Rows[0]["USER_NM"].ToString();
                    cbGrade.EditValue = dtLogUserInfo.Rows[0]["POS_CD"].ToString();
                    lookUpEditDept.EditValue = dtLogUserInfo.Rows[0]["DEPT_CD"].ToString();
                    txtPhone.Text = dtLogUserInfo.Rows[0]["USER_TEL"].ToString();
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
