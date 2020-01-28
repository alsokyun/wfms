using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduling;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
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
        private string SEL_FTR_CDE; //시설물코드
        private string SEL_FTR_IDN; //관리번호
        private string SEL_SEQ; //점검결과순번


        #region ======= 생성자 ========
        // 생성자
        public ChkSchDtlView( string _SCL_NUM)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            this.SCL_NUM = _SCL_NUM;

            //초기조회
            InitMOdel();

        }

        #endregion








        #region ======= 이벤트핸들러 ========

        /// <summary>
        //공통코드콤보 초기로딩 이벤트핸들러
        private void RPR_CAT_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            obj.ItemsSource = BizUtil.GetCmbCode("RPR_CAT_CDE", true);
        }
        private void RPR_CUZ_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            obj.ItemsSource = BizUtil.GetCmbCode("RPR_CUZ_CDE", true);
        }




        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
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



        //지형지물선택팝업창 호출
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // 지형지물팝업 윈도우
                FtrSelView ftrSelView = new FtrSelView(null);
                ftrSelView.Owner = Window.GetWindow(this);


                //FTR_IDN 리턴
                if (ftrSelView.ShowDialog() is bool)
                {
                    //string FTR_IDN = ftrSelView.txtFTR_IDN.Text;
                    //string FTR_CDE = ftrSelView.txtFTR_CDE.Text;
                    //string FTR_NAM = ftrSelView.txtFTR_NAM.Text;
                    //string HJD_NAM = ftrSelView.txtHJD_NAM.Text;

                    string FTR_IDN = "117";
                    string FTR_CDE = "SA117";
                    string FTR_NAM = "유량계";
                    string HJD_NAM = "북정동";

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





        //행삭제 - 시설물 직접삭제처리
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {

            //데이터 직접삭제처리
            try
            {
                bool isChecked = false;
                foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
                {
                    if ("Y".Equals(dr["CHK"]))
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


                // 첨부파일, 소모품, 주유오일 데이터체크





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
                                conditions.Add("sqlId", "DeleteChscResult");
                                conditions.Add("SCL_NUM", ((DataTable)grid.ItemsSource).Rows[i]["SCL_NUM"].ToString());
                                conditions.Add("FTR_CDE", ((DataTable)grid.ItemsSource).Rows[i]["FTR_CDE"].ToString());
                                conditions.Add("FTR_IDN", ((DataTable)grid.ItemsSource).Rows[i]["FTR_IDN"].ToString());
                                conditions.Add("SEQ", ((DataTable)grid.ItemsSource).Rows[i]["SEQ"].ToString());

                                BizUtil.Update(conditions);

                                ((DataTable)grid.ItemsSource).Rows.RemoveAt(i);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    Messages.ShowOkMsgBox();
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
                    param.Add("sqlId", "SaveChscResult");
                    param.Add("SCL_NUM", SCL_NUM);
                    param.Add("FTR_CDE", row["FTR_CDE"].ToString());
                    param.Add("FTR_IDN", row["FTR_IDN"]);
                    param.Add("SEQ", row["SEQ"]);

                    param.Add("RPR_YMD", row["RPR_YMD"].ToString());
                    param.Add("RPR_CAT_CDE", row["RPR_CAT_CDE"].ToString());
                    param.Add("RPR_CUZ_CDE", row["RPR_CUZ_CDE"].ToString());
                    param.Add("RPR_USR_NM", row["RPR_USR_NM"].ToString());
                    param.Add("RPR_CTNT", row["RPR_CTNT"].ToString());
                    param.Add("FIL_SEQ", row["FIL_SEQ"].ToString());
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

        }

        // 그리드 row 선택이벤트 핸들러
        private void Gv_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            SEL_FTR_CDE = grid.GetCellValue(gv.FocusedRowHandle, "FTR_CDE").ToString();
            SEL_FTR_IDN = grid.GetCellValue(gv.FocusedRowHandle, "FTR_IDN").ToString();
            SEL_SEQ = grid.GetCellValue(gv.FocusedRowHandle, "SEQ").ToString();

            // 선택한 시설물로 탭 새로구성
            InitTab();
        }


        #endregion









        #region ========= 메소드 ===========

        //초기조회
        private void InitMOdel()
        {

            // 1.점검결과
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectChscResultList");
            param.Add("SCL_NUM", this.SCL_NUM);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

            // 1.1 점검결과 첫행선택
            if (dt != null && dt.Rows.Count > 0)
            {
                SEL_FTR_CDE = dt.Rows[0]["FTR_CDE"].ToString();
                SEL_FTR_IDN = dt.Rows[0]["FTR_IDN"].ToString();
                SEL_SEQ = dt.Rows[0]["SEQ"].ToString();
            }



            /* 
             * 2.탭항목 동적추가
             */
            InitTab();
        }


        // 탭항목 동적추가
        private void InitTab()
        {
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "점검사진";
            //tab01.Content = new WttCostDtView(SCL_NUM);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "소모품사용";
            tab02.Content = new PdjtHtView(SCL_NUM, SEL_FTR_CDE, SEL_FTR_IDN, SEL_SEQ);
            tabSubMenu.Items.Add(tab02);

            DXTabItem tab03 = new DXTabItem();
            tab03.Header = "주유/오일사용";
            //tab03.Content = new WttChngDtView(SCL_NUM);
            tabSubMenu.Items.Add(tab03);
        }



        //시설물 한건 row 추가
        private void AddFtrRow(string fTR_IDN, string fTR_CDE, string fTR_NAM, string hJD_NAM)
        {
            if (FmsUtil.IsNull(fTR_IDN))
            {
                Messages.ShowInfoMsgBox("관리번호가 없습니다.");
                return;
            }

            DataRow drNew = ((DataTable)grid.ItemsSource).NewRow();
            drNew["FTR_IDN"] = fTR_IDN;
            drNew["FTR_CDE"] = fTR_CDE;
            drNew["HJD_NAM"] = hJD_NAM;
            drNew["FTR_NAM"] = fTR_NAM;
            drNew["RPR_YMD"] = Convert.ToDateTime(DateTime.Today).ToString("yyyyMMdd");
            drNew["RPR_CAT_CDE"] = "";
            drNew["RPR_CUZ_CDE"] = "";
            drNew["RPR_USR_NM"] = "";
            drNew["RPR_CTNT"] = "";

            ((DataTable)grid.ItemsSource).Rows.Add(drNew);
            grid.View.FocusedRowHandle = ((DataTable)grid.ItemsSource).Rows.Count - 1; //그리드ROW position
        }



        #endregion













        #region ========= 팝업윈도우 공통 ===========



        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
