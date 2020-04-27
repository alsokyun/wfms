using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// PdjtMngListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtMngListView: UserControl
    {

        // 생성자
        public PdjtMngListView( )
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            //초기데이터
            InitDataBinding();

            //초기조회
            SearchAction();

        }




        #region ========== 메소드 ==========


        /// <summary>
        /// 데이터바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbPDT_CAT_CDE
                BizUtil.SetCmbCode(cbPDT_CAT_CDE, "250106", "[전체]");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 조회
        /// </summary>
        private void SearchAction()
        {
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectPdjtMaMngList");

            param.Add("PDT_NAM", txtPDT_NAM.Text);
            param.Add("PDT_MDL_STD", txtPDT_MDL_STD.Text);
            param.Add("PDT_MNF", txtPDT_MNF.Text);
            param.Add("PDT_CAT_CDE", cbPDT_CAT_CDE.EditValue); //소모품

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;
        }


        #endregion



        #region ============ 이벤트핸들러 ============ 


        // 검색
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SearchAction();
        }
        //초기화
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            cbPDT_CAT_CDE.SelectedIndex = 0;
            txtPDT_MDL_STD.Text = "";
            txtPDT_MNF.Text = "";
            txtPDT_NAM.Text = "";
        }


        //입고등록
        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            string PDH_NUM = "";

            //0.체크박스 체크
            int cnt = 0;
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                //체크여부
                if ("Y".Equals(dr["CHK"]))
                {
                    cnt++;
                    try
                    {
                        PDH_NUM = dr["PDH_NUM"].ToString();
                    }
                    catch (Exception) { }
                }
            }
            if (cnt < 1)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }
            else if (cnt > 1)
            {
                Messages.ShowInfoMsgBox("입고대상 항목을 하나만 선택하세요.");
                return;
            }

            //1.입고등록 팝업호출
            PdjtStockView pdjtEnterView = new PdjtStockView(PDH_NUM);
            if (pdjtEnterView.ShowDialog() is bool)
            {
                //팝업종료 후 재조회
                SearchAction();
            }

        }



        //행추가
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            gv.AddNewRow();
        }

        //행삭제
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            //gv.DeleteRow(gv.FocusedRowHandle);

            //데이터 직접삭제처리
            try
            {
                bool isChecked = false;
                foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
                {
                    //체크여부
                    if ("Y".Equals(dr["CHK"]))
                    {
                        isChecked = true;
                        break;
                    }

                    //자식데이터여부
                    int cnt = 0;
                    try
                    {
                        cnt = Convert.ToInt32(dr["CNT"]) ;
                    }
                    catch (Exception){}
                    if (cnt > 0)
                    {
                        Messages.ShowInfoMsgBox("선택한 항목중에 입고내역이 있습니다.");
                        return;
                    }

                }
                if (!isChecked)
                {
                    Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                    return;
                }



                if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    for (int i = ((DataTable)grid.ItemsSource).Rows.Count - 1; i >= 0; i--)
                    {
                        Hashtable conditions = new Hashtable();
                        try
                        {
                            if ("Y".Equals(((DataTable)grid.ItemsSource).Rows[i]["CHK"]))
                            {
                                conditions.Clear();
                                conditions.Add("sqlId", "DeletePdjtMa");
                                conditions.Add("PDH_NUM", ((DataTable)grid.ItemsSource).Rows[i]["PDH_NUM"].ToString());

                                BizUtil.Update(conditions);

                                ((DataTable)grid.ItemsSource).Rows.RemoveAt(i);
                            }
                        }
                        catch (Exception ex)
                        {
                            Messages.ShowErrMsgBox(ex.ToString());
                        }
                    }

                    Messages.ShowOkMsgBox();
                    SearchAction();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        //그리드저장
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            Hashtable param = new Hashtable();

            //그리드 저장
            DataTable dt = grid.ItemsSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                param = new Hashtable();

                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                {
                    param.Add("sqlId", "SavePdjtMa");
                    param.Add("PDH_NUM", row["PDH_NUM"]);

                    param.Add("PDT_CAT_CDE", row["PDT_CAT_CDE"]);
                    param.Add("PDT_NAM", row["PDT_NAM"]);
                    param.Add("PDT_MDL_STD", row["PDT_MDL_STD"]);
                    param.Add("PDT_UNT", row["PDT_UNT"]);
                    param.Add("PDT_MNF", row["PDT_MNF"]);
                    param.Add("USE_YN", row["USE_YN"]);
                }
                else
                {
                    continue;
                }


                //저장처리
                try
                {
                    BizUtil.Update(param);
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.ToString());
                    return;
                }

            }
            //저장처리 성공
            Messages.ShowOkMsgBox();
            SearchAction();
        }


        //그리드내 콤보구성
        private void PDT_CAT_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            //구분
            obj.ItemsSource = BizUtil.GetCmbCode("PDT_CAT_CDE", true);
        }



        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            //CheckEdit ce = sender as CheckEdit;
            //bool chk = ce.IsChecked is bool;
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                dr["CHK"] = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                dr["CHK"] = "N";
            }
        }








        #endregion


    }
}
