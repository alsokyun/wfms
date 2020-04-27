using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Main;
using GTI.WFMS.Modules.Mntc.ViewModel;
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
    /// PdjtUseDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtUseDtlView : Window
    {
        private string PDH_NUM;

        /// <summary>
        /// 생성자
        /// </summary>
        public PdjtUseDtlView(string _PDH_NUM)
        {
            InitializeComponent();

            PDH_NUM = _PDH_NUM;

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            // 초기s조회
            InitModel();
        }




        /// 조회
        private void InitModel()
        {
            //1.소모품정보
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectPdjtMaUseHtDtlInfo");
            param.Add("PDH_NUM", PDH_NUM);

            DataTable dt = new DataTable();
            dt = BizUtil.SelectList(param);

            foreach (DataRow row in dt.Rows)
            {
                txtPDH_NUM.Text = this.PDH_NUM;
                txtPDT_CAT_CDE_NM.Text = row["PDT_CAT_CDE_NM"].ToString();
                txtPDT_MDL_STD.Text = row["PDT_MDL_STD"].ToString();
                txtPDT_MNF.Text = row["PDT_MNF"].ToString();
                txtPDT_NAM.Text = row["PDT_NAM"].ToString();
                txtPDT_UNT.Text = row["PDT_UNT"].ToString();
                break;
            }

            //2.사용현황
            param = new Hashtable();
            param.Add("sqlId", "SelectPdjtMaUseHtDtlList");
            param.Add("PDH_NUM", PDH_NUM);

            dt = new DataTable();
            dt = BizUtil.SelectList(param);

            grid.ItemsSource = dt;
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void PdjtUseDtlView_KeyDown(object sender, KeyEventArgs e)
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
    }
}
