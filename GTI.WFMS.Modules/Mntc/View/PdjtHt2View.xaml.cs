using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// PdjtHt2View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtHt2View : UserControl
    {

        private string SCL_NUM;
        private string FTR_CDE;
        private string FTR_IDN;
        private string SEQ;



        #region =========== 생성자 ============

        public PdjtHt2View(string _SCL_NUM, string _FTR_CDE, string _FTR_IDN, string _SEQ)
        {
            InitializeComponent();

            this.SCL_NUM = _SCL_NUM;
            this.FTR_CDE = _FTR_CDE;
            this.FTR_IDN = _FTR_IDN;
            this.SEQ = _SEQ;
            
            
            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectPdhUseList");

            param.Add("SCL_NUM", SCL_NUM);
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);
            param.Add("SEQ", SEQ);
            param.Add("PDT_CAT_CDE", "PDT02"); //소모품
            

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }

        #endregion




        #region =========== 이벤트핸들러 ============

        //그리드행추가시 이벤트처리
        private void AddingNewRow(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

        }

        //공통코드콤보 초기로딩
        private void PDH_NUM_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;

            Hashtable param = new Hashtable();
            param["sqlId"] = "SelectPdhList";
            param["ValueMember"] = "PDH_NUM";
            param["DisplayMember"] = "PDT_NAM";
            param["PDT_CAT_CDE"] = "PDT02"; //오일류
            obj.ItemsSource = BizUtil.GetCombo(param);

            obj.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        //콤보변경 이벤트핸들러 
        private void OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            DataTable dt = (DataTable)obj.ItemsSource;

            DataRowView rv = obj.SelectedItem as DataRowView;
            DataRow[] dr = dt.Select("PDH_NUM='" + rv.Row["PDH_NUM"].ToString() + "\'");
            string PDT_MDL_STD = "";
            try
            {
                PDT_MDL_STD = dr[0]["PDT_MDL_STD"].ToString();
            }
            catch (Exception){}

            grid.SetCellValue(gv.FocusedRowHandle, "PDT_MDL_STD", PDT_MDL_STD);
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
                                conditions.Add("sqlId", "DeletePdjtHt");
                                conditions.Add("SCL_NUM", ((DataTable)grid.ItemsSource).Rows[i]["SCL_NUM"].ToString());
                                conditions.Add("FTR_CDE", ((DataTable)grid.ItemsSource).Rows[i]["FTR_CDE"].ToString());
                                conditions.Add("FTR_IDN", ((DataTable)grid.ItemsSource).Rows[i]["FTR_IDN"].ToString());
                                conditions.Add("SEQ", ((DataTable)grid.ItemsSource).Rows[i]["SEQ"].ToString());
                                conditions.Add("PDH_HT_NUM", ((DataTable)grid.ItemsSource).Rows[i]["PDH_HT_NUM"].ToString());

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
                    param.Add("sqlId", "SavePdjtHt");
                    param.Add("SCL_NUM", SCL_NUM);
                    param.Add("FTR_CDE", FTR_CDE);
                    param.Add("FTR_IDN", FTR_IDN);
                    param.Add("SEQ", SEQ);
                    try
                    {
                        param.Add("PDH_NUM", Convert.ToInt32(row["PDH_NUM"]));
                    }
                    catch (Exception){}
                    try
                    {
                        param.Add("PDH_HT_NUM", Convert.ToInt32(row["PDH_HT_NUM"]));
                    }
                    catch (Exception){}

                    try
                    {
                        param.Add("PDH_AMT", Convert.ToInt32(row["PDH_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("PAY_AMT", null);
                    }
                    try
                    {
                        param.Add("PDH_CNT", Convert.ToInt32(row["PDH_CNT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("PDH_CNT", null);
                    }
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
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }

            }
            //저장처리 성공
            Messages.ShowOkMsgBox();

        }


        #endregion

    }



}
