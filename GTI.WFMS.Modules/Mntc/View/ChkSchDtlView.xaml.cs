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

            //점검결과조회
            initModel();

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












        /// <summary>
        /// 조회작업
        /// </summary>        
        private void initModel()
        {

            //b.점검결과
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectChscResultList");
            param.Add("SCL_NUM", SCL_NUM);

            GrdLst = new ObservableCollection<ChscResultDtl>(BizUtil.SelectListObj<ChscResultDtl>(param));
            grid.ItemsSource = GrdLst;

            // 1.1 점검결과 첫행선택
            if (GrdLst.Count > 0)
            {
                //SEL_FTR_CDE = GrdLst[0].FTR_CDE.ToString();
                //SEL_FTR_IDN = dt.Rows[0]["FTR_IDN"].ToString();
                //SEL_SEQ = dt.Rows[0]["SEQ"].ToString();
            }
        }





        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 지형지물팝업 윈도우
                FtrSelView ftrSelView = new FtrSelView(null);
                ftrSelView.Owner = Window.GetWindow(chkSchDtlView);


                //FTR_IDN 리턴
                if (ftrSelView.ShowDialog() is bool)
                {
                    string FTR_IDN = ftrSelView.txtFTR_IDN.Text;
                    string FTR_CDE = ftrSelView.txtFTR_CDE.Text;
                    string FTR_NAM = ftrSelView.txtFTR_NAM.Text;
                    string HJD_NAM = ftrSelView.txtHJD_NAM.Text;


                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(FTR_IDN))
                    {
                        AddFtrRow(FTR_IDN, FTR_CDE, FTR_NAM, HJD_NAM); //시설물 한건추가
                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }
        //시설물 한건 row 추가
        private void AddFtrRow(string fTR_IDN, string fTR_CDE, string fTR_NAM, string hJD_NAM)
        {
            if (FmsUtil.IsNull(fTR_IDN))
            {
                Messages.ShowInfoMsgBox("관리번호가 없습니다.");
                return;
            }

            ChscResultDtl drNew = new ChscResultDtl();
            drNew.FTR_IDN = Convert.ToInt32(fTR_IDN);
            drNew.FTR_CDE = fTR_CDE;
            drNew.HJD_NAM = hJD_NAM;
            drNew.FTR_NAM = fTR_NAM;
            drNew.RPR_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyyMMdd");

            GrdLst.Add(drNew);
            //grid.ItemsSource = GrdLst;
        }


        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            //데이터 직접삭제처리
            try
            {
                bool isChecked = false;
                foreach (ChscResultDtl row in GrdLst)
                {
                    if ("Y".Equals(row.CHK))
                    {
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked)
                {
                    Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    foreach (ChscResultDtl row in GrdLst)
                    {
                        Hashtable param = new Hashtable();
                        try
                        {
                            if ("Y".Equals(row.CHK))
                            {
                                param.Clear();
                                param.Add("sqlId", "DeleteChscResult");
                                param.Add("SCL_NUM", row.SCL_NUM);
                                param.Add("FTR_CDE", row.FTR_CDE);
                                param.Add("FTR_IDN", row.FTR_IDN);
                                param.Add("SEQ", row.SEQ);

                                if (row.SEQ == 0)
                                {
                                    //그리드행만 삭제
                                    GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                    return;
                                }
                                else
                                {
                                    //데이터삭제
                                    BizUtil.Update(param);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다. " + ex.Message);
                            return;
                        }
                    }

                    //grid.ItemsSource = GrdLst;
                    Messages.ShowOkMsgBox();

                    //재조회
                    initModel();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = false;
            foreach (ChscResultDtl row in GrdLst)
            {
                if ("Y".Equals(row.CHK))
                {
                    isChecked = true;
                    break;
                }
            }
            if (!isChecked)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            Hashtable param = new Hashtable();

            //1.그리드 저장
            foreach (ChscResultDtl row in GrdLst)
            {
                if (row.CHK != "Y") continue;

                try
                {
                    row.SCL_NUM = Convert.ToInt16(SCL_NUM) ;
                    BizUtil.Update2(row, "SaveChscResult");
                }
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }
            }

            //2.점검마스터상태 변경
            Hashtable pa = new Hashtable();
            pa.Add("sqlId", "UpdateChscMaRes");
            pa.Add("SCL_NUM", SCL_NUM);
            BizUtil.Update(pa);


            //grid.ItemsSource = GrdLst;

            //저장처리성공
            Messages.ShowOkMsgBox();

            //재조회
            initModel();
        }
    }
}
