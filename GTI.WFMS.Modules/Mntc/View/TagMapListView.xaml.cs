using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.Converters;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// TagMapList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TagMapListView: UserControl
    {
        //엑셀다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread thread;
        string strFileName;
        string strExcelFormPath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Excel/FmsBaseExcel.xlsx";
        DataTable exceldt;
        GridColumn[] columnList;
        List<string> listCols;


        // 생성자
        public TagMapListView( )
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
                //BizUtil.SetCmbCode(cbPDT_CAT_CDE, "PDT_CAT_CDE", "[전체]");
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
            param.Add("sqlId", "SelectFtrTagMapList");

            param.Add("FTR_IDN", txtFTR_IDN.Text);
            param.Add("PRS_NAM", txtPRS_NAM.Text);
            param.Add("ATT_NAM", txtATT_NAM.Text);
            param.Add("TAG_ID", txtTAG_ID.Text);

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
            txtFTR_IDN.Text = "";
            txtPRS_NAM.Text = "";
            txtATT_NAM.Text = "";
            txtTAG_ID.Text = "";
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
                    param.Add("sqlId", "SaveFtrTagMap");
                    param.Add("G2_ID", row["G2_ID"]);
                    param.Add("FTR_CDE", row["FTR_CDE"]);
                    param.Add("FTR_IDN", row["FTR_IDN"]);

                    param.Add("TAG_ID", row["TAG_ID"]);
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


      

        




        #endregion



        /// <summary>
        /// 엑셀다운로드
        /// </summary>
        /// <param name="obj"></param>
        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                /// 데이터조회
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectFtrTagMapList");

                param.Add("FTR_IDN", txtFTR_IDN.Text);
                param.Add("PRS_NAM", txtPRS_NAM.Text);
                param.Add("ATT_NAM", txtATT_NAM.Text);
                param.Add("TAG_ID", txtTAG_ID.Text);

                param.Add("page", 0);
                param.Add("rows", 1000000);



                exceldt = BizUtil.SelectList(param);


                //그리드헤더정보 추출
                columnList = new GridColumn[grid.Columns.Count];
                grid.Columns.CopyTo(columnList, 0);
                listCols = new List<string>(); //컬럼헤더정보 가져오기
                foreach (GridColumn gcol in columnList)
                {
                    try
                    {
                        if ("PrintN".Equals(gcol.Tag.ToString())) continue; //엑셀출력제외컬럼
                    }
                    catch (Exception) { }

                    listCols.Add(gcol.FieldName.ToString());
                }


                saveFileDialog = null;
                saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "저장경로를 지정하세요.";

                //초기 파일명 지정
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_" + "태그매핑현황.xlsx";

                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Filter = "Excel|*.xlsx";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strFileName = saveFileDialog.FileName;
                    thread = new Thread(new ThreadStart(ExcelExportFX));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 엑셀다운로드 쓰레드 Function
        /// </summary>
        private void ExcelExportFX()
        {
            try
            {
                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        waitindicator.DeferedVisibility = true;
                    })));


                //엑셀 표 데이터
                DataTable dtExceltTableData = exceldt.DefaultView.ToTable(false, listCols.ToArray());

                int[] tablePointXY = { 3, 1 };


                //엑셀 유틸 호출
                //ExcelUtil.ExcelTabulation(strFileName, strExcelFormPath, startPointXY, strSearchCondition, dtExceltTableData);
                ExcelUtil.ExcelGrid(strExcelFormPath, strFileName, "태그매핑현황", dtExceltTableData, tablePointXY, grid, true);

                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       waitindicator.DeferedVisibility = false;
                       Messages.ShowInfoMsgBox("엑셀 다운로드가 완료되었습니다.");
                   })));
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        waitindicator.DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }



        // 검색조건 엔터키처리
        private void Txt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SearchAction();
            }
        }
    }
}
