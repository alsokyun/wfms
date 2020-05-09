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
        private int FTR_IDN;



        public AttFacListView(string _FTR_CDE, int _FTR_IDN)
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
            param.Add("sqlId", "SelectCmmWttAttaDt");

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
                AttFacDtlView attFacDtlView = new AttFacDtlView(FTR_CDE, FTR_IDN, -1);
                attFacDtlView.Owner = Window.GetWindow(this) ;

                
                //FIL_SEQ 리턴
                if (attFacDtlView.ShowDialog() is bool)
                {
                    //부속시설재조회
                    initModel();
                }

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



        

        private void Gv_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;
            try
            {
                string ATTA_SEQ = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "ATTA_SEQ").ToString();
                string FTR_CDE = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_CDE").ToString();
                string FTR_IDN = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_IDN").ToString();
                // 부속세부시설윈도우
                AttFacDtlView attFacDtlView = new AttFacDtlView(FTR_CDE, Convert.ToInt32(FTR_IDN), Convert.ToInt32(ATTA_SEQ));
                attFacDtlView.Owner = Window.GetWindow(this);


                //FIL_SEQ 리턴
                if (attFacDtlView.ShowDialog() is bool)
                {
                    //부속시설재조회
                    initModel();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
