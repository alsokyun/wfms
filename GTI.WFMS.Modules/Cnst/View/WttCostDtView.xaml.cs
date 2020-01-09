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

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// WttCostDtView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WttCostDtView : UserControl
    {

        private string CNT_NUM;

        public WttCostDtView(string _CNT_NUM)
        {
            InitializeComponent();

            this.CNT_NUM = _CNT_NUM;
            
            
            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttCostDtList");

            param.Add("CNT_NUM", CNT_NUM);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }






        //그리드행추가시 이벤트처리
        private void AddingNewRow(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

        }

        //공통코드콤보 초기로딩
        private void PTY_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            obj.ItemsSource = BizUtil.GetCmbCode("PTY_CDE", true);
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
            //int newRowHandle = DataControlBase.NewItemRowHandle;
            //grid.SetCellValue(gv.FocusedRowHandle, "PAY_YMD", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
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
                                conditions.Add("sqlId", "DeleteWttCostDt");
                                conditions.Add("CNT_NUM", ((DataTable)grid.ItemsSource).Rows[i]["CNT_NUM"].ToString());
                                conditions.Add("COST_SEQ", ((DataTable)grid.ItemsSource).Rows[i]["COST_SEQ"].ToString());

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
            /*기존 공사비 삭제
            param.Add("CNT_NUM", CNT_NUM);
            param.Add("sqlId", "DeleteWttCostDt");
            BizUtil.Update(param);
             */

            //그리드 저장
            DataTable dt = grid.ItemsSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                param = new Hashtable();

                if (row.RowState == DataRowState.Modified )
                {
                    param.Add("sqlId", "SaveWttCostDt");
                    param.Add("CNT_NUM", CNT_NUM);
                    param.Add("COST_SEQ", Convert.ToInt32(row["COST_SEQ"]));

                    param.Add("PTY_CDE", row["PTY_CDE"].ToString());
                    param.Add("PAY_YMD", row["PAY_YMD"].ToString());
                    try
                    {
                        param.Add("PAY_AMT", Convert.ToInt32(row["PAY_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("PAY_AMT", null);
                    }
                }
                else if (row.RowState == DataRowState.Added)
                {
                    param.Add("sqlId", "SaveWttCostDt");
                    param.Add("CNT_NUM", CNT_NUM);

                    param.Add("PTY_CDE", row["PTY_CDE"].ToString());
                    param.Add("PAY_YMD", row["PAY_YMD"].ToString());
                    try
                    {
                        param.Add("PAY_AMT", Convert.ToInt32(row["PAY_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("PAY_AMT", null);
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

        private void Gv_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            //GridControl gc = sender as GridControl;
            //gc.SetCellValue(e.RowHandle, "PAY_YMD", DateTime.Now.Date);
        }

    }
}
