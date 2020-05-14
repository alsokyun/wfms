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
    /// PdjtStockView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtStockView : Window
    {
        private string PDH_NUM;

        /// <summary>
        /// 생성자
        /// </summary>
        public PdjtStockView(string _PDH_NUM)
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
            DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
         
            param.Add("sqlId", "SelectPdjtInHtPopList");
            param.Add("PDH_NUM", PDH_NUM);

            dt = new DataTable();
            dt = BizUtil.SelectList(param);

            grid.ItemsSource = dt;
        }







        //행추가
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            gv.AddNewRow();
            int idx = 0;
            try
            {
                idx = ((DataTable)grid.ItemsSource).Rows.Count-1;
            }
            catch (Exception) { }

            //grid.SetCellValue(idx,"CHK", "Y");
        }

        //행삭제
        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {

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
                                conditions.Add("sqlId", "DeletePdjtInHtPop");
                                conditions.Add("PDH_NUM", ((DataTable)grid.ItemsSource).Rows[i]["PDH_NUM"].ToString());
                                conditions.Add("IN_NUM", ((DataTable)grid.ItemsSource).Rows[i]["IN_NUM"].ToString());

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
                    InitModel();
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
            bool isChecked = false;
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                //체크여부
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


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            Hashtable param = new Hashtable();

            //그리드 저장
            DataTable dt = grid.ItemsSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                if (!"Y".Equals(row["CHK"])) continue; //체크한데이터만 저장

                param = new Hashtable();

                if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                {
                    param.Add("sqlId", "SavePdjtInHtPop");
                    param.Add("PDH_NUM", PDH_NUM);
                    param.Add("IN_NUM", row["IN_NUM"]);

                    param.Add("IN_AMT", row["IN_AMT"]);
                    param.Add("IN_YMD", row["IN_YMD"]);
                    param.Add("IN_ETC", row["IN_ETC"]);
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
            InitModel();
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




















        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }
 

        private void PdjtStockView_KeyDown(object sender, KeyEventArgs e)
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
