using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.GIS.Pop.View
{
    /// <summary>
    /// ShpMnglView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShpMnglView : Window
    {

        /// <summary>
        /// 생성자
        /// </summary>
        public ShpMnglView()
        {
            InitializeComponent();


            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            // 초기조회
            InitModel();
        }




        /// 조회
        private void InitModel()
        {
            //1.시설물정보
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectOldDtl");

            DataTable dt = new DataTable();
            dt = BizUtil.SelectList(param);

            foreach (DataRow row in dt.Rows)
            {
                txtIP.Text = row["FTR_NAM"].ToString();
                txtSHP.Text = row["FTR_IDN"].ToString();
                txtUser.EditValue = row["IMP_CDE"].ToString();
                txtPW.Text = row["HJD_NAM"].ToString();


                break;
            }


            //3.점검이력
            param = new Hashtable();
            param.Add("sqlId", "SelectOptMtChkHtList");

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
        private void ShpMnglView_KeyDown(object sender, KeyEventArgs e)
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




        /// <summary>
        /// 1.Shape 임포트
        /// 2.Shape DB서버로 복사 & gisloader실행, tbloader실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            //필수체크
            if (!BizUtil.ValidReq(this)) return;

            if (Messages.ShowYesNoMsgBox("Import 하시겠습니까?") != MessageBoxResult.Yes) return;













            Messages.ShowOkMsgBox();
            InitModel();
        }
    }
}
