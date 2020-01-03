using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
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

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// RefFileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RefFileMngView : UserControl
    {

        private string BIZ_ID;
        private FileMngView fileMngView; //첨부파일팝업



        public RefFileMngView(string _BIZ_ID)
        {
            InitializeComponent();

            this.BIZ_ID = _BIZ_ID;

            fileMngView = new FileMngView(null);

            //초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileMapList");

            param.Add("BIZ_ID", BIZ_ID);

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

        }


        //첨부파일 모듈호출
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //팝업열기 & 위치
                fileMngView.IsOpen = false;

                fileMngView = new FileMngView(null);
                fileMngView.PlacementRectangle = new Rect(100, 100, 655, 405);
                fileMngView.IsOpen = true;
                //fileMngView.DataContext = this;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }


            //gv.AddNewRow();
            //int newRowHandle = DataControlBase.NewItemRowHandle;
            //grid.SetCellValue(gv.FocusedRowHandle, "PAY_YMD", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
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

            /*기존 공사비 삭제
             */
            Hashtable param = new Hashtable();
            param.Add("sqlId", "DeleteFileMng");
            param.Add("BIZ_ID", BIZ_ID);
            BizUtil.Update(param);

            //그리드 저장
            DataTable dt = grid.ItemsSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                param = new Hashtable();

                if (row.RowState == DataRowState.Modified )
                {
                    param.Add("sqlId", "SaveFileMng");
                    param.Add("BIZ_ID", BIZ_ID);
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
                    param.Add("sqlId", "SaveFileMng");
                    param.Add("BIZ_ID", BIZ_ID);

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



        //선택한 첨부파일에 대한 파일창열기
        //선택된 항목으로 페이지이동
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string FIL_SEQ = "";
            GridControl gc = sender as GridControl;

            try
            {
                FIL_SEQ = ((DataRowView)gc.CurrentItem).Row["FIL_SEQ"].ToString();

                //팝업열기 & 위치
                fileMngView.IsOpen = false;

                fileMngView = new FileMngView(FIL_SEQ);
                fileMngView.PlacementRectangle = new Rect(100, 100, 550, 400);
                fileMngView.IsOpen = true;
                fileMngView.DataContext = this;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }


    }
}
