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
    /// WttChngDt.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WttChngDtView : UserControl
    {

        private string CNT_NUM;

        public WttChngDtView(string _CNT_NUM)
        {
            InitializeComponent();

            this.CNT_NUM = _CNT_NUM;
            
            
            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttChngDtList");

            param.Add("CNT_NUM", CNT_NUM);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }






        //그리드행추가시 이벤트처리
        private void AddingNewRow(object sender, System.ComponentModel.AddingNewEventArgs e)
        {

        }


        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {

        }


        //행추가
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            gv.AddNewRow();
            int newRowHandle = DataControlBase.NewItemRowHandle;
            grid.SetCellValue(0, "CHG_YMD", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
            grid.SetCellValue(0, "ATT_TIM", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        }
        //행삭제
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            gv.DeleteRow(gv.FocusedRowHandle);
        }


        //그리드저장
        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            //기존 공사비 삭제
            Hashtable param = new Hashtable();
            param.Add("sqlId", "DeleteWttChngDt");
            param.Add("CNT_NUM", CNT_NUM);
            BizUtil.Update(param);

            //그리드 저장
            DataTable dt = grid.ItemsSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                param = new Hashtable();
                if (row.RowState == DataRowState.Modified)
                {
                    param.Add("sqlId", "SaveWttChngDt");
                    param.Add("CNT_NUM", CNT_NUM);
                    param.Add("CHNG_SEQ", Convert.ToInt32(row["CHNG_SEQ"]));

                    param.Add("CHG_YMD", row["CHG_YMD"].ToString());
                    param.Add("ATT_TIM", row["ATT_TIM"].ToString());
                    param.Add("CGV_DES", row["CGV_DES"].ToString());

                    try {
                        param.Add("INC_AMT", Convert.ToInt32(row["INC_AMT"]));
                    } catch (Exception) {
                        param.Add("INC_AMT", null);
                    }
                    try {
                        param.Add("IGV_AMT", Convert.ToInt32(row["IGV_AMT"]));
                    } catch (Exception) {
                        param.Add("IGV_AMT", null);
                    }
                    try {
                        param.Add("CHG_AMT", Convert.ToInt32(row["CHG_AMT"]));
                    } catch (Exception) {
                        param.Add("CHG_AMT", null);
                    }

                }
                else if (row.RowState == DataRowState.Added)
                {
                    param.Add("sqlId", "SaveWttChngDt");
                    param.Add("CNT_NUM", CNT_NUM);

                    param.Add("CHG_YMD", row["CHG_YMD"].ToString());
                    param.Add("ATT_TIM", row["ATT_TIM"].ToString());
                    param.Add("CGV_DES", row["CGV_DES"].ToString());

                    try
                    {
                        param.Add("INC_AMT", Convert.ToInt32(row["INC_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("INC_AMT", null);
                    }
                    try
                    {
                        param.Add("IGV_AMT", Convert.ToInt32(row["IGV_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("IGV_AMT", null);
                    }
                    try
                    {
                        param.Add("CHG_AMT", Convert.ToInt32(row["CHG_AMT"]));
                    }
                    catch (Exception)
                    {
                        param.Add("CHG_AMT", null);
                    }
                }
                else
                {
                    continue;
                }

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

            Messages.ShowOkMsgBox();
        }
    }
}
