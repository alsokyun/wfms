using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GTI.WFMS.Main.View.Pop
{
    /// <summary>
    /// PopupQuickMenuMng.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupQuickMenuMng : Window
    {
        Hashtable htconditions = new Hashtable();
        DataTable dtresult = new DataTable();
        

        DataTable dtcbdata { get; set; }

        public PopupQuickMenuMng()
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);
            Loaded += PopupQuickMenuMng_Loaded;
            
        }

        #region 이벤트
        private void PopupQuickMenuMng_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeEvent();
            InitializeData();
        }

        /// <summary>
        /// 즐겨찾기에서 제거
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_RighttoLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QuickMNUitemMove((ObservableCollection<QuickMNU>)gcQuickR.ItemsSource, (ObservableCollection<QuickMNU>)gcQuickL.ItemsSource);
            }
            catch (Exception ex)
            {
                Messages.ShowInfoMsgBox(ex.ToString());
            }
        }

        /// <summary>
        /// 즐겨찾기에 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LefttoRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intRcnt = ((ObservableCollection<QuickMNU>)gcQuickR.ItemsSource).Count;
                int intLChkcnt = ((ObservableCollection<QuickMNU>)gcQuickL.ItemsSource).Count(item => item.CHK == true);

                if (intRcnt + intLChkcnt > 12)
                {
                    Messages.ShowInfoMsgBox("즐겨찾기 메뉴는 12개까지 등록할수 있습니다.");
                    return;
                }

                QuickMNUitemMove((ObservableCollection<QuickMNU>)gcQuickL.ItemsSource, (ObservableCollection<QuickMNU>)gcQuickR.ItemsSource);
            }
            catch (Exception ex)
            {
                Messages.ShowInfoMsgBox(ex.ToString());
            }

        }

        /// <summary>
        /// 저장 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                htconditions.Clear();
                htconditions.Add("SYS_CD", FmsUtil.sysCd);
                htconditions.Add("USER_ID", Logs.strLogin_ID);
                htconditions.Add("MNU_CD", "");
                htconditions.Add("SHRTEN_KEY", "");
                htconditions.Add("ORD", "");

                //work.Delete_BASE_FLOW_CALC_INFO(htconditions);
                htconditions.Add("sqlId", "Delete_BASE_FLOW_CALC_INFO");
                BizUtil.Update(htconditions);



                foreach (QuickMNU item in (ObservableCollection<QuickMNU>)gcQuickR.ItemsSource)
                {
                    htconditions["MNU_CD"] = item.MNU_CD;
                    htconditions["SHRTEN_KEY"] = item.SHRTEN_KEY;
                    htconditions["ORD"] = ((ObservableCollection<QuickMNU>)gcQuickR.ItemsSource).IndexOf(item);
                    //work.Insert_BASE_FLOW_CALC_INFO(htconditions);
                    htconditions["sqlId"] = "Insert_BASE_FLOW_CALC_INFO";
                    BizUtil.Update(htconditions);
                }

                Messages.ShowOkMsgBox();

                this.Close();
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
        /// 단축키 콤보박스 바인딩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbSHRTEN_KEY_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBoxEdit cbSHRTEN_KEY = sender as ComboBoxEdit;
                if (cbSHRTEN_KEY.ItemsSource == null)
                    cbSHRTEN_KEY.ItemsSource = dtcbdata.Copy();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 단축키 중복 확인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbSHRTEN_KEY_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            try
            {
                ComboBoxEdit cbSHRTEN_KEY = sender as ComboBoxEdit;

                if (cbSHRTEN_KEY.EditValue == null)
                    return;

                if (((ObservableCollection<QuickMNU>)gcQuickR.ItemsSource).Count(item => item.SHRTEN_KEY == cbSHRTEN_KEY.EditValue.ToString()) == 2)
                {
                    cbSHRTEN_KEY.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }
        #endregion

        #region 기능
        /// <summary>
        /// 데이터 초기화
        /// </summary>
        private void InitializeData()
        {
            try
            {
                htconditions.Clear();
                htconditions.Add("SYS_CD", FmsUtil.sysCd);
                htconditions.Add("USER_ID", Logs.strLogin_ID);


                htconditions.Add("sqlId", "Select_BASE_FLOW_CALC_INFO_L");
                gcQuickL.ItemsSource = QuickMNUGridLoad(BizUtil.SelectList(htconditions));
                htconditions["sqlId"] = "Select_BASE_FLOW_CALC_INFO_R";
                gcQuickR.ItemsSource = QuickMNUGridLoad(BizUtil.SelectList(htconditions));

                dtcbdata = null;
                dtcbdata = new DataTable();
                dtcbdata.Columns.Add("CD");
                dtcbdata.Columns.Add("NM");

                DataRow dradd;

                for (int i = 1; i < 13; i++)
                {
                    dradd = dtcbdata.NewRow();
                    dradd["CD"] = "F" + i.ToString();
                    dradd["NM"] = "F" + i.ToString();
                    dtcbdata.Rows.Add(dradd.ItemArray);
                }
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBoxLog(e);
            }
        }

        private ObservableCollection<QuickMNU> QuickMNUGridLoad(DataTable dtselect)
        {
            try
            {
                ObservableCollection<QuickMNU> QuickMNUList = new ObservableCollection<QuickMNU>();

                foreach (DataRow dr in dtselect.Rows)
                {
                    QuickMNUList.Add(new QuickMNU()
                    {
                        CHK = false,
                        MNU_NM = dr["MNU_NM"].ToString(),
                        MNU_CD = dr["MNU_CD"].ToString(),
                        UPPER_CD = dr["UPPER_CD"].ToString(),
                        SHRTEN_KEY = dr["SHRTEN_KEY"].ToString(),
                        STRSHOW = dr["STRSHOW"].ToString()
                    });
                }

                return QuickMNUList;
            }
            catch (Exception ex)
            {
                Messages.ErrLog(ex);
                return null;
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
            btn_LefttoRight.Click += Btn_LefttoRight_Click;
            btn_RighttoLeft.Click += Btn_RighttoLeft_Click;
        }

        /// <summary>
        /// 그리드에서의 Row 이동
        /// </summary>
        /// <param name="dtS">sources 테이블</param>
        /// <param name="dtD">destination 테이블</param>
        private void QuickMNUitemMove(ObservableCollection<QuickMNU> QuickS, ObservableCollection<QuickMNU> QuickD)
        {
            try
            {
                for (int i = QuickS.Count - 1; i >= 0; i--)
                {
                    QuickMNU item = QuickS[i];

                    if (item.CHK)
                    {
                        item.SHRTEN_KEY = null;
                        QuickD.Insert(0, item);
                        QuickS.Remove(item);
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

    #region ###########트리 데이터 Class
    public class QuickMNU
    {
        public bool CHK { get; set; }
        public string MNU_NM { get; set; }
        public string MNU_CD { get; set; }
        public string UPPER_CD { get; set; }
        public string SHRTEN_KEY { get; set; }
        public string STRSHOW { get; set; }
        public int ORD { get; set; }

        public override string ToString()
        {
            return MNU_CD;
        }
    }
    #endregion
}