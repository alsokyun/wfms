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
    /// AttFacListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AttFacListView : UserControl
    {

        private string FTR_CDE;
        private string FTR_IDN;
        private FileMngView fileMngView; //첨부파일팝업



        public AttFacListView(string _FTR_CDE, string _FTR_IDN)
        {
            InitializeComponent();

            this.FTR_CDE = _FTR_CDE;
            this.FTR_IDN = _FTR_IDN;


            initModel(); //초기조회
        }







        // 초기 데이터모델 조회
        private void initModel()
        {

            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectCmmWttAttaDtList");

            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);

            dt = BizUtil.SelectList(param);
            grid.ItemsSource = dt;
        }





        //부속시설등록팝업 호출
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // 파일첨부윈도우
                FileMngView fileMngView = new FileMngView(null);
                fileMngView.Owner = Window.GetWindow(this) ;

                
                //FIL_SEQ 리턴
                if (fileMngView.ShowDialog() is bool)
                {
                    string FIL_SEQ = fileMngView.txtFIL_SEQ.Text;

                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(FIL_SEQ))
                    {
                        //부속시설재조회
                        initModel();
                    }
                    //닫기버튼으로 닫힘
                }


                //팝업열기 & 위치
                //fileMngView.IsOpen = false;

                //fileMngView = new FileMngView(null);
                //fileMngView.PlacementRectangle = new Rect(100, 100, 655, 405);
                //fileMngView.IsOpen = true;

                //fileMngView.DataContext = this;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
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
                FileMngView fileMngView = new FileMngView(FIL_SEQ);
                fileMngView.Owner = Window.GetWindow(this);


                //FIL_SEQ 리턴
                if (fileMngView.ShowDialog() is bool)
                {
                    FIL_SEQ = fileMngView.txtFIL_SEQ.Text;

                    //AddFilSeqRow(FIL_SEQ); //첨부파일 한건추가할 필요없음
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
