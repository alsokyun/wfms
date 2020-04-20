using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// RefFileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RefFileMngView : UserControl
    {

        private string BIZ_ID;
        private FileMngView fileMngView; //첨부파일팝업


        // 뷰생성자
        public RefFileMngView(string _BIZ_ID)
        {
            InitializeComponent();

            this.BIZ_ID = _BIZ_ID;


            //초기조회
            InitModel();

        }




        //초기조회
        private void InitModel()
        {
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileMapList");

            param.Add("BIZ_ID", BIZ_ID);
            param.Add("GRP_TYP", "112"); //일반파일


            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;

        }




        /// <summary>
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



        //첨부파일 모듈호출
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // 파일첨부윈도우
                fileMngView = new FileMngView(this.BIZ_ID, null);
                fileMngView.Owner = Window.GetWindow(this) ;

                
                //FIL_SEQ 리턴
                if (fileMngView.ShowDialog() is bool)
                {
                    string FIL_SEQ = fileMngView.txtFIL_SEQ.Text;

                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(FIL_SEQ))
                    {
                        //AddFilSeqRow(FIL_SEQ); //첨부파일 한건추가
                        //조회그리드형으로 변경
                        InitModel();
                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }


            //gv.AddNewRow();
            //int newRowHandle = DataControlBase.NewItemRowHandle;
            //grid.SetCellValue(gv.FocusedRowHandle, "PAY_YMD", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        }


        //첨부파일 한건 row 추가
        private void AddFilSeqRow(string fIL_SEQ)
        {
            if (FmsUtil.IsNull(fIL_SEQ))
            {
                Messages.ShowInfoMsgBox("파일ID가 없습니다.");
                return;
            }

            DataRow drNew = ((DataTable)grid.ItemsSource).NewRow();
            drNew["FIL_SEQ"] = fIL_SEQ;
            drNew["TIT_NAM"] = "";
            drNew["CRE_YMD"] = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");
            drNew["CRE_USR"] = "";
            drNew["CTNT"] = "";

            ((DataTable)grid.ItemsSource).Rows.Add(drNew);
            grid.View.FocusedRowHandle = ((DataTable)grid.ItemsSource).Rows.Count - 1; //그리드ROW position
        }



        // 첨부파일 한건추가


        //행삭제 - FIL_SEQ 직접삭제처리
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
                                conditions.Add("sqlId", "DeleteFileMap");
                                conditions.Add("BIZ_ID", ((DataTable)grid.ItemsSource).Rows[i]["BIZ_ID"].ToString());
                                conditions.Add("FIL_SEQ", ((DataTable)grid.ItemsSource).Rows[i]["FIL_SEQ"].ToString());

                                BizUtil.Update(conditions);

                                ((DataTable)grid.ItemsSource).Rows.RemoveAt(i);
                            }
                        }
                        catch (Exception )
                        {
                        }
                    }

                    Messages.ShowOkMsgBox();
                    InitModel();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
                InitModel();

            }
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

                // 파일첨부윈도우
                FileMngView fileMngView = new FileMngView(BIZ_ID, FIL_SEQ);
                fileMngView.Owner = Window.GetWindow(this);


                //FIL_SEQ 리턴
                if (fileMngView.ShowDialog() is bool)
                {
                    FIL_SEQ = fileMngView.txtFIL_SEQ.Text;

                    //조회그리드형으로 변경
                    InitModel();
                }



                //팝업열기 & 위치
                //fileMngView.IsOpen = false;

                //fileMngView = new FileMngView(FIL_SEQ);
                //fileMngView.PlacementRectangle = new Rect(100, 100, 550, 400);
                //fileMngView.IsOpen = true;
                //fileMngView.DataContext = this;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());

            }
        }

    }
}
