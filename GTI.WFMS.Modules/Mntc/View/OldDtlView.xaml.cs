using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Main;
using GTI.WFMS.Modules.Mntc.ViewModel;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// OldDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OldDtlView : Window
    {
        private string G2_ID;
        private string FTR_CDE;
        private string FTR_IDN;
        private string TAG_ID;

        /// <summary>
        /// 생성자
        /// </summary>
        public OldDtlView(string _G2_ID)
        {
            InitializeComponent();

            G2_ID = _G2_ID;

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            //콤보세팅
            BizUtil.SetCmbCode(cbIMP_CDE, "250111", false);

            // 초기조회
            InitModel();
        }




        /// 조회
        private void InitModel()
        {
            //1.시설물정보
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectOldDtl");
            param.Add("G2_ID", G2_ID);

            DataTable dt = new DataTable();
            dt = BizUtil.SelectList(param);

            foreach (DataRow row in dt.Rows)
            {
                txtFTR_NAM.Text = row["FTR_NAM"].ToString();
                txtFTR_IDN.Text = row["FTR_IDN"].ToString();
                cbIMP_CDE.EditValue = row["IMP_CDE"].ToString();
                txtHJD_NAM.Text = row["HJD_NAM"].ToString();
                txtLMT_YEAR.Text = row["LMT_YEAR"].ToString();
                txCRE_YY.Text = row["CRE_YY"].ToString();
                txtOLD_LVL.Text = row["OLD_LVL"].ToString();
                txtCHK_PRD.Text = row["CHK_PRD"].ToString();

                FTR_CDE = row["FTR_CDE"].ToString();
                FTR_IDN = row["FTR_IDN"].ToString();
                try
                {
                    TAG_ID = row["TAG_ID"].ToString();
                }
                catch (Exception){}

                break;
            }


            //3.점검이력
            param = new Hashtable();
            param.Add("sqlId", "SelectOptMtChkHtList");
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);

            dt = new DataTable();
            dt = BizUtil.SelectList(param);

            grid2.ItemsSource = dt;
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void OldDtlView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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


        //주요도 저장
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                Hashtable param = new Hashtable();
                param.Add("sqlId", "UpdateOldMng");
                param.Add("G2_ID", G2_ID);
                param.Add("IMP_CDE", cbIMP_CDE.EditValue);

                BizUtil.Update(param);
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.Message);
            }

            Messages.ShowOkMsgBox();
            InitModel();
        }
    }
}
