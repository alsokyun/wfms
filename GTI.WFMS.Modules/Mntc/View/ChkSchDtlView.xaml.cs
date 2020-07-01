using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduling;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Link.View;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// ChkSchDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChkSchDtlView: Window
    {
        private string SCL_NUM; //점검번호
        private ObservableCollection<ChscResultDtl> GrdLst;


        #region ======= 생성자 ========
        // 생성자
        public ChkSchDtlView( string _SCL_NUM)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            this.SCL_NUM = _SCL_NUM;
            txtSCL_NUM.EditValue = _SCL_NUM; //뷰의 바인딩을 통해 뷰모델값 변경동기화

            //미리보기컨트롤 매핑
            ImageContainer.grdImg = grdImg;
            ImageContainer.imgView = imgView;
            ImageContainer.bdImg = bdImg;

        }

        #endregion








        #region ======= 이벤트핸들러 ========

        /// <summary>
        //공통코드콤보 초기로딩 이벤트핸들러
        private void RPR_CAT_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            obj.ItemsSource = BizUtil.GetCmbCode("250103");
        }
        private void RPR_CUZ_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            obj.ItemsSource = BizUtil.GetCmbCode("250104");
        }




        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            foreach (ChscResultDtl dr in (ObservableCollection<ChscResultDtl>)grid.ItemsSource)
            {
                dr.CHK = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (ChscResultDtl dr in (ObservableCollection<ChscResultDtl>)grid.ItemsSource)
            {
                dr.CHK = "N";
            }
        }



        // 그리드 row 선택이벤트 핸들러
        private void Grid_CurrentItemChanged(object sender, DevExpress.Xpf.Grid.CurrentItemChangedEventArgs e)
        {
            //SEL_FTR_CDE = ((DataRowView)e.NewItem).Row["FTR_CDE"].ToString();
            //SEL_FTR_IDN = ((DataRowView)e.NewItem).Row["FTR_IDN"].ToString();
            //SEL_SEQ = ((DataRowView)e.NewItem).Row["SEQ"].ToString();

            ChscResultDtl row = (ChscResultDtl)e.NewItem;

            // 선택한 시설물로 탭 새로구성
            InitTab(row.FTR_CDE, row.FTR_IDN.ToString(), row.SEQ.ToString());
        }


        #endregion









        #region ========= 메소드 ===========

        // 탭항목 동적추가
        private void InitTab(string SEL_FTR_CDE, string SEL_FTR_IDN, string SEL_SEQ)
        {
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "점검사진";
            tab01.Content = new ImgFileMngView(SCL_NUM.ToString() +  SEL_FTR_CDE + SEL_FTR_IDN);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "소모품사용";
            tab02.Content = new PdjtHtView(SCL_NUM, SEL_FTR_CDE, SEL_FTR_IDN, SEL_SEQ);
            tabSubMenu.Items.Add(tab02);

            DXTabItem tab03 = new DXTabItem();
            tab03.Header = "주유/오일사용";
            tab03.Content = new PdjtHt2View(SCL_NUM, SEL_FTR_CDE, SEL_FTR_IDN, SEL_SEQ);
            tabSubMenu.Items.Add(tab03);
        }







        #endregion













        #region ========= 팝업윈도우 공통 ===========



        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Esc
        private void ChkSchDtlView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (bdImg.Visibility == Visibility.Visible)
                {
                    grdImg.Visibility = Visibility.Hidden;
                    bdImg.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Close();
                }
            }
        }
        //미리보기 off
        private void BtnOff_Click(object sender, RoutedEventArgs e)
        {
            grdImg.Visibility = Visibility.Hidden;
            bdImg.Visibility = Visibility.Hidden;
        }



        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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


        #endregion






    }
}
