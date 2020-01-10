using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace GTIFramework.Common.Utils.Converters
{
    public class ExcelUtil
    {
        /// <summary>
        /// 차트/표 엑셀다운로드
        /// </summary>
        /// <param name="strFileName"> 저장할 파일명(사용자 설정) </param>
        /// <param name="strExcelFormPath"> 엑셀다운로드 폼 경로 </param>
        /// <param name="intStartCellXY"> 데이터 쓰기 시작할 엑셀 셀 좌표</param>
        /// <param name="strSearchCondition"> 조회 조건(블록명, 조회기간 등) </param>
        /// <param name="dtChartData"> 차트 표현할 DataTable </param>
        /// <param name="dtTableData"> 표 표현할 DataTable </param>

        public static void ExcelChartAndDiagram(string strFileName, string strExcelFormPath, int[] intStartCellXY, string[] strSearchCondition, DataTable dtChartData, DataTable dtTableData)
        {
            try
            {
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                wb = excelApp.Workbooks.Open(strExcelFormPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ws = (Excel.Worksheet)wb.Sheets.get_Item(1);


                #region 차트데이터

                Excel.Range rangeChart = null;

                long CDRowCNT = dtChartData.Rows.Count;
                int CDColumCNT = dtChartData.Columns.Count;

                object[,] chartDatas = new object[CDRowCNT, CDColumCNT];

                for (int i = 0; i < CDColumCNT; i++)
                {
                    for (int j = 0; j < CDRowCNT; j++)
                    {
                        chartDatas[j, i] = dtChartData.Rows[j][i].ToString();
                    }
                }

                //엑셀 데이터 쓰기 시작되는 셀 x,y (Row,Column)
                //데이터 쓰기 시작하는 x(Row)는 항상 같음.
                int intStartX = intStartCellXY[0];
                int intStartY = intStartCellXY[1];

                //엑셀 데이터 쓰기 종료되는 셀 x,y
                int intEndX = (int)CDRowCNT + intStartX - 1;
                int intEndY = (int)CDColumCNT + intStartY - 1;


                //차트 데이터 range 설정
                Excel.Range sPoint_Chart = ws.Cells[intStartX, intStartY];
                Excel.Range ePoint_Chart = ws.Cells[intEndX, intEndY];

                rangeChart = ws.get_Range(sPoint_Chart, ePoint_Chart);
                rangeChart.Value2 = chartDatas;
                //rangeChart.EntireColumn.AutoFit();
                rangeChart.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                rangeChart.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeChart.Borders.Weight = Excel.XlBorderWeight.xlThin;

                #endregion


                #region 표 데이터

                Excel.Range rangeTable = null;
                long TDRowCNT = dtTableData.Rows.Count;
                int TDColumCNT = dtTableData.Columns.Count;

                object[,] tableDatas = new object[TDRowCNT, TDColumCNT];

                for (int i = 0; i < TDColumCNT; i++)
                {
                    for (int j = 0; j < TDRowCNT; j++)
                    {
                        tableDatas[j, i] = dtTableData.Rows[j][i].ToString(); //데이터 정보
                    }
                }

                //데이터 쓰기 시작하는 x(Row)는 항상 같음. y(Column)만 설정
                int intStartY_TD = intEndY + 2;

                int intEndX_TD = (int)TDRowCNT + intStartX - 1;
                int intEndY_TD = (int)TDColumCNT + intStartY_TD - 1;

                // range 설정
                Excel.Range sPoint_table = ws.Cells[intStartX, intStartY_TD];
                Excel.Range ePoint_table = ws.Cells[intEndX_TD, intEndY_TD];

                rangeTable = ws.get_Range(sPoint_table, ePoint_table);
                rangeTable.Value2 = tableDatas;
                //rangeTable.EntireColumn.AutoFit();
                rangeTable.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                rangeTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                rangeTable.Borders.Weight = Excel.XlBorderWeight.xlThin;

                #endregion


                #region 검색조건

                Excel.Range rangeSearchCondition = null;

                object[,] SearchConditionDatas = new object[strSearchCondition.Length, 1];

                for (int i = 0; i < strSearchCondition.Length; i++)
                {
                    SearchConditionDatas[i, 0] = strSearchCondition[i];
                }

                //데이터 쓰기 시작하는 x(Row)는 항상 같음. y(Column)만 설정
                int intStartY_Condition = intEndY_TD + 1;

                int intEndX_SC = strSearchCondition.Length + intStartX - 1;
                int intEndY_SC = intStartY_Condition;

                // range 설정
                Excel.Range sPoint_SC = ws.Cells[intStartX, intStartY_Condition];
                Excel.Range ePoint_SC = ws.Cells[intEndX_SC, intEndY_SC];

                rangeSearchCondition = ws.get_Range(sPoint_SC, ePoint_SC);
                rangeSearchCondition.Value2 = SearchConditionDatas;
                //rangeSearchCondition.EntireColumn.AutoFit();


                #endregion

                wb.SaveAs(strFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //wb.Close(true);
                //excelApp.Quit();


                //ReleaseExcelObject(ws);
                //ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);

                //프로세스 Kill 후 재실행
                int intHwnd;
                GetWindowThreadProcessId(excelApp.Hwnd, out intHwnd);

                Process p = Process.GetProcessById(intHwnd);
                p.Kill();

                Process process = new Process();
                process.StartInfo.FileName = strFileName;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 집계표 엑셀다운로드
        /// </summary>
        /// <param name="strFileName"> 저장할 파일명(사용자 설정) </param>
        /// <param name="strExcelFormPath"> 엑셀다운로드 폼 경로 </param>
        /// <param name="intStartCellXY"> 데이터 쓰기 시작할 엑셀 셀 좌표</param>
        /// <param name="strSearchCondition"> 조회 조건(블록명, 조회기간 등) </param>
        /// <param name="dtChartData"> 차트 표현할 DataTable </param>
        /// <param name="dtTableData"> 표 표현할 DataTable </param>

        public static void ExcelTabulation(string strFileName, string strExcelFormPath, int[] intStartCellXY, string[] strSearchCondition, DataTable dtTableData)
        {
            try
            {
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                wb = excelApp.Workbooks.Open(strExcelFormPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ws = (Excel.Worksheet)wb.Sheets.get_Item(1);


                #region 표 데이터

                Excel.Range rangeTable = null;
                long TDRowCNT = dtTableData.Rows.Count;
                int TDColumCNT = dtTableData.Columns.Count;

                //+1 => 헤더정보
                object[,] tableDatas = new object[TDRowCNT + 1, TDColumCNT];

                for (int k = 0; k < TDColumCNT; k++)
                {
                    tableDatas[0, k] = dtTableData.Columns[k].ColumnName;
                }


                for (int i = 0; i < TDColumCNT; i++)
                {
                    for (int j = 1; j < TDRowCNT; j++)
                    {
                        tableDatas[j, i] = dtTableData.Rows[j][i].ToString(); //데이터 정보
                    }
                }

                //엑셀 데이터 쓰기 시작되는 셀 x,y (Row,Column)
                //데이터 쓰기 시작하는 x(Row)는 항상 같음.
                int intStartX = intStartCellXY[0];
                int intStartY = intStartCellXY[1];

                //엑셀 데이터 쓰기 종료되는 셀 x,y
                int intEndX = (int)TDRowCNT + intStartX - 1;
                int intEndY = (int)TDColumCNT + intStartY - 1;


                // range 설정
                Excel.Range sPoint_table = ws.Cells[intStartX, intStartY];
                Excel.Range ePoint_table = ws.Cells[intEndX, intEndY];

                rangeTable = ws.get_Range(sPoint_table, ePoint_table);
                rangeTable.Value2 = tableDatas;
                //rangeTable.EntireColumn.AutoFit();
                //rangeTable.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                rangeTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                //rangeTable.Borders.Weight = Excel.XlBorderWeight.xlThin;

                #endregion


                //#region 검색조건

                Excel.Range rangeSearchCondition = null;

                object[,] SearchConditionDatas = new object[strSearchCondition.Length, 1];

                for (int i = 0; i < strSearchCondition.Length; i++)
                {
                    SearchConditionDatas[i, 0] = strSearchCondition[i];
                }

                //원하는 셀에 
                int intStartX_Condition = 1;
                int intStartY_Condition = 15;

                int intEndX_SC = strSearchCondition.Length + intStartX_Condition - 1;
                int intEndY_SC = intStartY_Condition;

                // range 설정
                Excel.Range sPoint_SC = ws.Cells[intStartX_Condition, intStartY_Condition];
                Excel.Range ePoint_SC = ws.Cells[intEndX_SC, intEndY_SC];

                rangeSearchCondition = ws.get_Range(sPoint_SC, ePoint_SC);
                rangeSearchCondition.Value2 = SearchConditionDatas;
                //rangeSearchCondition.EntireColumn.AutoFit();


                //#endregion

                wb.SaveAs(strFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                //wb.Close(true);
                //excelApp.Quit();


                //ReleaseExcelObject(ws);
                //ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);

                //프로세스 Kill 후 재실행
                int intHwnd;
                GetWindowThreadProcessId(excelApp.Hwnd, out intHwnd);

                Process p = Process.GetProcessById(intHwnd);
                p.Kill();

                Process process = new Process();
                process.StartInfo.FileName = strFileName;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 엑셀다운로드 Universal
        /// </summary>
        /// <param name="strExcelInfo">         엑셀파일 관련 정보                                   </param>
        /// <param name="strSearchCondition">   조회 조건 관련 정보(블록명,조회기간,데이터간격 등)   </param>
        /// <param name="intSearchConditionXY"> 조회 조건 관련 정보를 쓰기 시작할 셀(x,y) 좌표       </param>
        /// <param name="dtChartData">          엑셀 차트 데이터 / null 가능                         </param>
        /// <param name="intChartXY">           차트 데이터를 쓰기 시작할 셀(x,y) 좌표 / null 가능   </param>
        /// <param name="dtTableData">          엑셀 표 데이터                                       </param>
        /// <param name="intTableXY">           표 데이터 쓰기 시작할 셀(x,y) 좌표                   </param>
        /// <param name="gridControl">          밴드&컬럼을 내보내기 위한 gridControl / null 가능    </param>
        /// <param name="bLineYN">              셀 테두리 Y/N                                        </param>
        public static void ExcelUniversal(string[] strExcelInfo, string[] strSearchCondition, int[] intSearchConditionXY, DataTable dtChartData, int[] intChartXY,
            DataTable dtTableData, int[] intTableXY, GridControl gridControl, bool bLineYN)
        {
            try
            {
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                wb = excelApp.Workbooks.Open(strExcelInfo[0], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ws = (Excel.Worksheet)wb.Sheets.get_Item(1);


                #region 표 데이터
                //밴드없는 표 데이터 (컬럼명이 헤더)
                if (gridControl == null)
                {
                    //표 데이터 내보내기 전 표 하단의 그룹데이터(합계,평균 등) 유무 확인 후 처리
                    //그룹데이터(합계,평균 등) 개수 확인용
                    int intMaxRN = 0;

                    //그룹데이터(합계,평균 등) 여부 확인 & RN 컬럼 삭제
                    foreach (DataColumn dc in dtTableData.Columns)
                    {
                        if (dc.ColumnName.Equals("RN"))
                        {
                            intMaxRN = dtTableData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x[dc.ColumnName].ToString())).Max(x => Convert.ToInt32(x[dc.ColumnName]));

                            dtTableData.Columns.Remove(dc);

                            //DataTable 수정 후 루프돌면 에러나기떄문에 break 처리
                            break;
                        }
                    }

                    Excel.Range rangeTable = null;
                    long TDRowCNT = dtTableData.Rows.Count;
                    int TDColumCNT = dtTableData.Columns.Count;

                    //+1 => 헤더정보
                    object[,] tableDatas = new object[TDRowCNT + 1, TDColumCNT];

                    for (int k = 0; k < TDColumCNT; k++)
                    {
                        tableDatas[0, k] = dtTableData.Columns[k].ColumnName;
                    }


                    for (int i = 0; i < TDColumCNT; i++)
                    {
                        for (int j = 0; j < TDRowCNT; j++)
                        {
                            tableDatas[j + 1, i] = dtTableData.Rows[j][i].ToString(); //데이터 정보
                        }
                    }

                    //테이블 데이터 쓰기 시작되는 셀 x,y (Row,Column)
                    int intStartX_TD = intTableXY[0];
                    int intStartY_TD = intTableXY[1];

                    //엑셀 데이터 쓰기 종료되는 셀 x,y
                    int intEndX_TD = (int)TDRowCNT + intStartX_TD;
                    int intEndY_TD = (int)TDColumCNT + intStartY_TD - 1;

                    // range 설정
                    Excel.Range sPoint_TD = ws.Cells[intStartX_TD, intStartY_TD];
                    Excel.Range ePoint_TD = ws.Cells[intEndX_TD, intEndY_TD];

                    rangeTable = ws.get_Range(sPoint_TD, ePoint_TD);
                    rangeTable.Value2 = tableDatas;
                    if (bLineYN)
                    {
                        rangeTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    //RN 이 존재했을경우 = 그룹데이터 존재
                    if (intMaxRN != 0)
                    {
                        //표 데이터 그룹데이터(합계,평균 등) 색 변경 범위
                        Excel.Range rangeColor = null;

                        // range 설정
                        Excel.Range sPoint_Color = ws.Cells[intEndX_TD - intMaxRN + 1, intStartY_TD];
                        Excel.Range ePoint_Color = ws.Cells[intEndX_TD, intEndY_TD];

                        rangeColor = ws.get_Range(sPoint_Color, ePoint_Color);
                        rangeColor.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
                    }


                }
                //밴드가 존재하는 표 데이터
                else
                {
                    //그리드 컨트롤에서 bands 복사하여 리스트의 인덱스 0부터 담음
                    GridControlBand[] bandsList = new GridControlBand[gridControl.Bands.Count];
                    gridControl.Bands.CopyTo(bandsList, 0);

                    //데이터 쓰기 시작하는 셀 x,y
                    int intStartX_Bnd = intTableXY[0];
                    int intStartY_Bnd = intTableXY[1];

                    //데이터 쓰기 종료되는 셀 x,y
                    int intEndX_Bnd = intTableXY[0];
                    int intEndY_Bnd = intTableXY[1];

                    foreach (GridControlBand gcb in bandsList)
                    {
                        string strBandsHeader = string.Empty;
                        bool bBandsVisible = false;

                        gridControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            strBandsHeader = gcb.Header.ToString();
                            bBandsVisible = gcb.Visible;

                        }));

                        //가시적으로 표현되는 bands만 엑셀로 내보낸다.
                        if (bBandsVisible == true)
                        {
                            Excel.Range rangeBands = null;

                            //bands에 속한 컬럼이 1개일경우
                            if (gcb.Columns.Count == 1)
                            {
                                //range 끝 좌표 x값만 +1 증가시킨 후 merge
                                Excel.Range sPoint_Bnd = ws.Cells[intStartX_Bnd, intStartY_Bnd];
                                Excel.Range ePoint_Bnd = ws.Cells[intEndX_Bnd + 1, intEndY_Bnd];

                                rangeBands = ws.get_Range(sPoint_Bnd, ePoint_Bnd);
                                ws.get_Range(sPoint_Bnd, ePoint_Bnd).Merge();
                                rangeBands.Value = strBandsHeader;

                                //다음 데이터가 쓰여질 시작 y좌표 +1
                                intStartY_Bnd++;
                                //다음 데이터가 쓰여질 끝 y좌표 +1
                                intEndY_Bnd++;
                            }
                            else if (gcb.Columns.Count > 1)
                            {
                                //셀 merge 하면서 밴드 헤더 내보냄
                                //밴드 내 컬럼 갯수만큼 범위로 잡아서 merge
                                //if(gcb == bandsList[0])
                                //{
                                intEndY_Bnd += gcb.Columns.Count - 1;
                                //}
                                //else
                                //{
                                //    intEndY_Bnd += gcb.Columns.Count -1;
                                //}


                                Excel.Range sPoint_Bnd = ws.Cells[intStartX_Bnd, intStartY_Bnd];
                                Excel.Range ePoint_Bnd = ws.Cells[intEndX_Bnd, intEndY_Bnd];

                                rangeBands = ws.get_Range(sPoint_Bnd, ePoint_Bnd);
                                ws.get_Range(sPoint_Bnd, ePoint_Bnd).Merge();
                                rangeBands.Value = strBandsHeader;

                                intEndY_Bnd++;

                                //밴드 내 컬럼 추가 로직
                                //셀 y 값 설정

                                foreach (GridColumn gc in gcb.Columns)
                                {
                                    string strColumnHeader = string.Empty;

                                    gridControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                                    {
                                        strColumnHeader = gc.Header.ToString();

                                    }));

                                    Excel.Range rangeColumns = null;

                                    Excel.Range sPoint_Col = ws.Cells[intStartX_Bnd + 1, intStartY_Bnd];
                                    Excel.Range ePoint_col = ws.Cells[intStartX_Bnd + 1, intStartY_Bnd];

                                    rangeColumns = ws.get_Range(sPoint_Col, ePoint_col);
                                    rangeColumns.Value = strColumnHeader;
                                    if (bLineYN)
                                    {
                                        rangeColumns.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                                    }


                                    intStartY_Bnd++;
                                }
                            }
                            if (bLineYN)
                            {
                                rangeBands.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            }


                        }
                    }


                    //표 데이터 내보내기 전 표 하단의 그룹데이터(합계,평균 등) 유무 확인 후 처리
                    //그룹데이터(합계,평균 등) 개수 확인용
                    int intMaxRN = 0;

                    //그룹데이터(합계,평균 등) 여부 확인 & RN 컬럼 삭제
                    foreach (DataColumn dc in dtTableData.Columns)
                    {
                        if (dc.ColumnName.Equals("RN"))
                        {
                            intMaxRN = dtTableData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x[dc.ColumnName].ToString())).Max(x => Convert.ToInt32(x[dc.ColumnName]));
                            dtTableData.Columns.Remove(dc);

                            //DataTable 수정 후 루프돌면 에러나기떄문에 break 처리
                            break;
                        }
                    }

                    //표 데이터 내보내기 부분
                    Excel.Range rangeTable = null;

                    long TDRowCNT = dtTableData.Rows.Count;
                    int TDColumCNT = dtTableData.Columns.Count;

                    object[,] tableDatas = new object[TDRowCNT, TDColumCNT];

                    for (int i = 0; i < TDColumCNT; i++)
                    {
                        for (int j = 0; j < TDRowCNT; j++)
                        {
                            tableDatas[j, i] = dtTableData.Rows[j][i].ToString(); //데이터 정보
                        }
                    }

                    //테이블 데이터 쓰기 시작되는 셀 x,y (Row,Column)
                    //밴드 이후로 조정하기 위해 +2
                    int intStartX_TD = intTableXY[0] + 2;
                    int intStartY_TD = intTableXY[1];

                    //엑셀 데이터 쓰기 종료되는 셀 x,y
                    int intEndX_TD = (int)TDRowCNT + intStartX_TD - 1;
                    int intEndY_TD = (int)TDColumCNT + intStartY_TD - 1;

                    // range 설정
                    Excel.Range sPoint_TD = ws.Cells[intStartX_TD, intStartY_TD];
                    Excel.Range ePoint_TD = ws.Cells[intEndX_TD, intEndY_TD];

                    rangeTable = ws.get_Range(sPoint_TD, ePoint_TD);
                    rangeTable.Value2 = tableDatas;
                    if (bLineYN)
                    {
                        rangeTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }


                    //RN 이 존재했을경우 = 그룹데이터 존재
                    if (intMaxRN != 0)
                    {
                        //표 데이터 그룹데이터(합계,평균 등) 색 변경 범위
                        Excel.Range rangeColor = null;

                        // range 설정
                        Excel.Range sPoint_Color = ws.Cells[intEndX_TD - intMaxRN + 1, intStartY_TD];
                        Excel.Range ePoint_Color = ws.Cells[intEndX_TD, intEndY_TD];

                        rangeColor = ws.get_Range(sPoint_Color, ePoint_Color);
                        rangeColor.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
                    }
                }
                #endregion


                #region 검색조건

                Excel.Range rangeSearchCondition = null;

                object[,] SearchConditionDatas = new object[strSearchCondition.Length, 1];

                for (int i = 0; i < strSearchCondition.Length; i++)
                {
                    SearchConditionDatas[i, 0] = strSearchCondition[i];
                }


                int intStartX_SC = intSearchConditionXY[0];
                int intStartY_SC = intSearchConditionXY[1];

                int intEndX_SC = strSearchCondition.Length + intStartX_SC - 1;
                int intEndY_SC = intStartY_SC;

                // range 설정
                Excel.Range sPoint_SC = ws.Cells[intStartX_SC, intStartY_SC];
                Excel.Range ePoint_SC = ws.Cells[intEndX_SC, intEndY_SC];

                rangeSearchCondition = ws.get_Range(sPoint_SC, ePoint_SC);
                rangeSearchCondition.Value2 = SearchConditionDatas;
                //rangeSearchCondition.EntireColumn.AutoFit();

                #endregion


                #region 차트데이터

                if (dtChartData != null)
                {
                    Excel.Range rangeChart = null;
                    long CDRowCNT = dtChartData.Rows.Count;
                    int CDColumCNT = dtChartData.Columns.Count;

                    object[,] chartDatas = new object[CDRowCNT, CDColumCNT];

                    for (int i = 0; i < CDColumCNT; i++)
                    {
                        for (int j = 0; j < CDRowCNT; j++)
                        {
                            chartDatas[j, i] = dtChartData.Rows[j][i].ToString();
                        }
                    }


                    int intStartX_CD = intChartXY[0];
                    int intStartY_CD = intChartXY[1];

                    int intEndX_CD = (int)CDRowCNT + intStartX_CD - 1;
                    int intEndY_CD = (int)CDColumCNT + intStartY_CD - 1;

                    //차트 데이터 range 설정
                    Excel.Range sPoint_CD = ws.Cells[intStartX_CD, intStartY_CD];
                    Excel.Range ePoint_CD = ws.Cells[intEndX_CD, intEndY_CD];

                    rangeChart = ws.get_Range(sPoint_CD, ePoint_CD);
                    rangeChart.Value2 = chartDatas;
                }

                #endregion


                wb.SaveAs(strExcelInfo[1], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                wb.Close(true);

                //excelApp.Quit();

                //ReleaseExcelObject(ws);
                //ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);

                //프로세스 Kill 후 재실행
                int intHwnd;
                GetWindowThreadProcessId(excelApp.Hwnd, out intHwnd);

                Process p = Process.GetProcessById(intHwnd);
                p.Kill();

                Process process = new Process();
                process.StartInfo.FileName = strExcelInfo[1];
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 엑셀 메모리 반환??
        /// garbage collector??
        /// </summary>
        /// <param name="obj"></param>
        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }


        /// <summary>
        /// DataTable을 그대로 보여주기
        /// data : DataTable
        /// strcol : data컬럼명
        /// strFileName : 저장 파일명
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strcol"></param>
        /// <param name="strFileName"></param>
        public static void ExcelWYSIWYG(DataTable data, List<string> strcol, string strFileName)
        {
            if (data == null) return;
            if (strcol.Count == 0) return;

            try
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                savefile.Title = "엑셀 다운로드";
                savefile.FileName = strFileName + ".xlsx";
                savefile.Filter = "All xlsx Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excel = new Excel.Application();
                    excel.DisplayAlerts = false;

                    Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    worksheet.Name = strFileName;

                    Excel.Range range = null;

                    long Rcnt = data.Rows.Count;
                    int Ccnt = data.Columns.Count;

                    object[,] datas = new object[Rcnt + 1, Ccnt];

                    //헤더 정보 + 데이터 정보
                    for (int i = 0; i < Ccnt; i++)
                    {
                        datas[0, i] = strcol[i]; //헤더 정보

                        for (int j = 0; j < Rcnt; j++)
                        {
                            datas[j + 1, i] = data.Rows[j][i].ToString(); //데이터 정보
                        }
                    }

                    //range 설정
                    Excel.Range sPoint = worksheet.Cells[1, 1];
                    Excel.Range ePoint = worksheet.Cells[Rcnt + 1, Ccnt];

                    range = worksheet.get_Range(sPoint, ePoint);
                    range.Value2 = datas;
                    range.EntireColumn.AutoFit();
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    //헤더 범위
                    Excel.Range HsPoint = worksheet.Cells[1, 1];
                    Excel.Range HePoint = worksheet.Cells[1, Ccnt];
                    range = worksheet.get_Range(HsPoint, HePoint);
                    range.Interior.Color = ColorTranslator.ToOle(Color.GreenYellow);

                    workbook.SaveAs(savefile.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //프로세스 Kill 후 재실행
                    int hwnd;
                    GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                    Process p = Process.GetProcessById(hwnd);
                    p.Kill();

                    Process process = new Process();
                    process.StartInfo.FileName = savefile.FileName;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //데브 WYSIWYG
        public static void DevExcelWYSIWYG(TableView[] view, string strFileName)
        {
            try
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                savefile.Title = "엑셀 다운로드";
                savefile.FileName = strFileName;
                savefile.Filter = "All xlsx Files | *.xlsx";

                int pageW = 0, pageH = 400;

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    List<TemplatedLink> links = new List<TemplatedLink>();

                    foreach (TableView tableview in view)
                    {
                        tableview.PrintAutoWidth = false;
                        PrintableControlLink print = new PrintableControlLink(tableview);
                        links.Add(print);

                        if (((DataTable)((GridControl)tableview.Parent).ItemsSource).Columns.Count * 1000 > pageW)
                        {
                            pageW = ((DataTable)((GridControl)tableview.Parent).ItemsSource).Columns.Count * 1000;
                        }
                        if (((DataTable)((GridControl)tableview.Parent).ItemsSource).Rows.Count * 100 + 400 > pageH)
                        {
                            pageH = ((DataTable)((GridControl)tableview.Parent).ItemsSource).Rows.Count * 100 + 400;
                        }
                    }

                    CompositeLink compositeLink = new CompositeLink(links);
                    compositeLink.PaperKind = System.Drawing.Printing.PaperKind.Custom;
                    compositeLink.CustomPaperSize = new Size(pageW, pageH);
                    compositeLink.CreateDocument(false);
                    compositeLink.CreatePageForEachLink();

                    XlsxExportOptionsEx option = new XlsxExportOptionsEx();
                    option.ExportMode = XlsxExportMode.SingleFilePageByPage;
                    option.ExportType = DevExpress.Export.ExportType.WYSIWYG;

                    compositeLink.ExportToXlsx(savefile.FileName, option);
                    Messages.ShowOkMsgBox();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DataTable(밴드)을 그대로 보여주기
        /// data : DataTable
        /// strBand : data밴드명(같은값 데이터일 경우 Merge)
        /// strcol : data컬럼명
        /// strFileName : 저장 파일명
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strcol"></param>
        /// <param name="strFileName"></param>
        public static void ExcelWYSIWYG(DataTable data, List<string> strBand, List<string> strcol, string strFileName)
        {
            if (data == null) return;
            if (strcol.Count == 0) return;

            try
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                savefile.Title = "엑셀 다운로드";
                savefile.FileName = strFileName + ".xlsx";
                savefile.Filter = "All xlsx Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excel = new Excel.Application();
                    excel.DisplayAlerts = false;

                    Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    worksheet.Name = strFileName;

                    Excel.Range range = null;

                    long Rcnt = data.Rows.Count;
                    int Ccnt = data.Columns.Count;

                    object[,] datas = new object[Rcnt + 2, Ccnt];

                    //밴드 정보 + 데이터 정보
                    for (int i = 0; i < Ccnt; i++)
                    {
                        datas[0, i] = strBand[i]; //헤더 정보
                    }

                    //헤더 정보 + 데이터 정보
                    for (int i = 0; i < Ccnt; i++)
                    {
                        datas[1, i] = strcol[i]; //헤더 정보

                        for (int j = 0; j < Rcnt; j++)
                        {
                            datas[j + 2, i] = data.Rows[j][i].ToString(); //데이터 정보
                        }
                    }

                    int Fcell = 0;
                    int Lcell = 0;

                    for (int i = 0; i < Ccnt - 1; i++)
                    {
                        if (!datas[0, i].Equals(datas[0, i + 1]))
                        {
                            Lcell = i;
                            Excel.Range sp = worksheet.Cells[1, Lcell + 1];
                            Excel.Range ep = worksheet.Cells[1, Fcell + 1];
                            range = worksheet.get_Range(sp, ep);
                            range.Merge(true);
                            Fcell = i + 1;
                        }
                        if (i == Ccnt - 2)
                        {
                            Lcell = i + 1;
                            Excel.Range sp = worksheet.Cells[1, Lcell + 1];
                            Excel.Range ep = worksheet.Cells[1, Fcell + 1];
                            range = worksheet.get_Range(sp, ep);
                            range.Merge(true);
                        }
                        if (datas[0, i].Equals(datas[1, i]))
                        {
                            Excel.Range sp = worksheet.Cells[1, i + 1];
                            Excel.Range ep = worksheet.Cells[2, i + 1];
                            range = worksheet.get_Range(sp, ep);
                            range.Merge(false);
                        }
                    }

                    //range 설정
                    Excel.Range sPoint = worksheet.Cells[1, 1];
                    Excel.Range ePoint = worksheet.Cells[Rcnt + 1, Ccnt];

                    range = worksheet.get_Range(sPoint, ePoint);
                    range.Value2 = datas;
                    range.EntireColumn.AutoFit();
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    //헤더 범위
                    Excel.Range HsPoint = worksheet.Cells[1, 1];
                    Excel.Range HePoint = worksheet.Cells[2, Ccnt];
                    range = worksheet.get_Range(HsPoint, HePoint);
                    range.Interior.Color = ColorTranslator.ToOle(Color.GreenYellow);

                    workbook.SaveAs(savefile.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //프로세스 Kill 후 재실행
                    int hwnd;
                    GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                    Process p = Process.GetProcessById(hwnd);
                    p.Kill();

                    Process process = new Process();
                    process.StartInfo.FileName = savefile.FileName;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// DataTable을 그대로 보여주기
        /// data : DataTable
        /// strcol : data 컬럼명
        /// strHide : data Hiden 컬럼명 strcol명과 같아야 한다.
        /// strFileName : 저장 파일명
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strcol"></param>
        /// <param name="strHide"></param>
        /// <param name="strFileName"></param>
        public static void ExcelColumnHide(DataTable data, List<string> strcol, List<string> strHide, string strFileName)
        {
            if (data == null) return;
            if (strcol.Count == 0) return;

            try
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                savefile.Title = "엑셀 다운로드";
                savefile.FileName = strFileName + ".xlsx";
                savefile.Filter = "All xlsx Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excel = new Excel.Application();
                    excel.DisplayAlerts = false;

                    Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    worksheet.Name = strFileName;

                    Excel.Range range = null;

                    long Rcnt = data.Rows.Count;
                    int Ccnt = data.Columns.Count;

                    object[,] datas = new object[Rcnt + 1, Ccnt];
                    List<int> inthide = new List<int>();

                    //헤더 정보 + 데이터 정보
                    for (int i = 0; i < Ccnt; i++)
                    {
                        datas[0, i] = strcol[i]; //헤더 정보

                        if (strHide.Contains(strcol[i]))
                        {
                            inthide.Add(i);
                        }

                        for (int j = 0; j < Rcnt; j++)
                        {
                            datas[j + 1, i] = data.Rows[j][i].ToString(); //데이터 정보
                        }
                    }

                    //range 설정
                    Excel.Range sPoint = worksheet.Cells[1, 1];
                    Excel.Range ePoint = worksheet.Cells[Rcnt + 1, Ccnt];

                    range = worksheet.get_Range(sPoint, ePoint);
                    range.Value2 = datas;
                    range.EntireColumn.AutoFit();
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    //헤더 범위
                    Excel.Range HsPoint = worksheet.Cells[1, 1];
                    Excel.Range HePoint = worksheet.Cells[1, Ccnt];
                    range = worksheet.get_Range(HsPoint, HePoint);
                    range.Interior.Color = ColorTranslator.ToOle(Color.GreenYellow);

                    //컬럼 숨기기
                    foreach (int i in inthide)
                    {
                        range = (Excel.Range)worksheet.Cells[1, i + 1];
                        range.EntireColumn.Hidden = true;
                    }

                    workbook.SaveAs(savefile.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //프로세스 Kill 후 재실행
                    int hwnd;
                    GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                    Process p = Process.GetProcessById(hwnd);
                    p.Kill();

                    Process process = new Process();
                    process.StartInfo.FileName = savefile.FileName;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().IndexOf("을(를) 사용할 수 없습니다") != -1)
                {
                    Messages.ShowErrMsgBox("엑셀을 종료하세요.");
                }
                else
                {
                    throw ex;
                }

            }
        }

        /// <summary>
        /// Excel데이터 DataTable 리턴
        /// </summary>
        /// <returns></returns>
        public static DataTable ExcelImport(string FilePath)
        {
            DataTable dtresult = new DataTable();

            try
            {
                Excel.Application excel = new Excel.Application();

                Excel.Workbook workbook = excel.Workbooks.Open(FilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets.get_Item(1);

                Excel.Range range = worksheet.UsedRange;

                object[,] valueArray = (object[,])range.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);

                dtresult = ProcessObjects(valueArray, null);

                int hwnd;
                GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                Process p = Process.GetProcessById(hwnd);
                p.Kill();

                return dtresult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Excel데이터 DataTable 리턴 시작열, 행 선택
        /// </summary>
        /// <returns></returns>
        public static DataTable ExcelImport(string FilePath, int intCol, int intRow)
        {
            DataTable dtresult = new DataTable();

            Excel.Application excel = new Excel.Application();

            try
            {
                Excel.Workbook workbook = excel.Workbooks.Open(FilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets.get_Item(1);

                object[,] headArray = null;

                if (intRow != 1)
                {
                    Excel.Range HeadsPoint = worksheet.Cells[intRow - 1, intCol];
                    Excel.Range HeadePoint = worksheet.Cells[worksheet.UsedRange.Rows.Count, worksheet.UsedRange.Columns.Count];
                    Excel.Range Headrange = worksheet.get_Range(HeadsPoint, HeadePoint);

                    headArray = (object[,])Headrange.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);
                }

                Excel.Range sPoint = worksheet.Cells[intRow, intCol];
                Excel.Range ePoint = worksheet.Cells[worksheet.UsedRange.Rows.Count, worksheet.UsedRange.Columns.Count];
                Excel.Range range = worksheet.get_Range(sPoint, ePoint);

                object[,] valueArray = (object[,])range.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);


                dtresult = ProcessObjects(headArray, valueArray);

                int hwnd;
                GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                Process p = Process.GetProcessById(hwnd);
                p.Kill();

                return dtresult;
            }
            catch (Exception ex)
            {
                int hwnd;
                GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                Process p = Process.GetProcessById(hwnd);
                p.Kill();

                throw ex;
            }
        }

        #region 삽질
        //public static void ExcelColumnHide(string strFileName, DataSet DS, List<string> strCol)
        //{
        //    SaveFileDialog savefile = new SaveFileDialog();
        //    savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //    savefile.Title = "엑셀 다운로드";
        //    savefile.FileName = strFileName + ".xlsx";
        //    savefile.Filter = "All xlsx Files | *.xlsx";

        //    if (savefile.ShowDialog() == DialogResult.OK)
        //    {
        //        //같은이름 File 삭제
        //        if (File.Exists(savefile.FileName))
        //        {
        //            File.Delete(savefile.FileName);
        //        }

        //        string TempFile = savefile.FileName;

        //        OleDbConnection OleDBConn = null;

        //        try
        //        {
        //            OleDbCommand Cmd = null;

        //            string ConnStr = 
        //                string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"{0}\";Mode=ReadWrite|Share Deny None;Extended Properties='Excel 12.0;HDR=YES';Persist Security Info=False", savefile.FileName);

        //            OleDBConn = new OleDbConnection(ConnStr);
        //            OleDBConn.Open();

        //            // Create Table(s).. : 테이블 단위 처리
        //            foreach (DataTable DT in DS.Tables)
        //            {
        //                String TableName = DT.TableName;

        //                StringBuilder FldsInfo = new StringBuilder();
        //                StringBuilder Flds = new StringBuilder();

        //                // Create Field(s) String : 현재 테이블의 Field 명 생성
        //                for (int i = 0; i < strCol.Count; i++)
        //                {
        //                    if (FldsInfo.Length > 0)
        //                    {
        //                        FldsInfo.Append(",");
        //                        Flds.Append(",");
        //                    }

        //                    FldsInfo.Append("[" + strCol[i].Replace("'", "''") + "] CHAR(255)");
        //                    Flds.Append(strCol[i].Replace("'", "''"));
        //                }

        //                // 테이블 생성
        //                Cmd = new OleDbCommand("CREATE TABLE ["+ TableName +"](" + FldsInfo.ToString() + ")", OleDBConn);
        //                Cmd.ExecuteNonQuery();

        //                // 데이터 바인딩
        //                foreach (DataRow DR in DT.Rows)
        //                {
        //                    StringBuilder Values = new StringBuilder();
        //                    foreach (DataColumn Column in DT.Columns)
        //                    {
        //                        if (Values.Length > 0) Values.Append(",");
        //                        Values.Append("'" + DR[Column.ColumnName].ToString().Replace("'", "''") + "'");
        //                    }

        //                    Cmd = new OleDbCommand(
        //                        "INSERT INTO [" + TableName + "]" +
        //                        "(" + Flds.ToString() + ") " +
        //                        "VALUES (" + Values.ToString() + ")",
        //                        OleDBConn);
        //                    Cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (OleDBConn != null) OleDBConn.Close();
        //        }
        //    }
        //}




        //public static void ExcelWYSIWYG(DataTable data, List<string> strcol, string strFileName)
        //{
        //    if (data == null) return;
        //    if (strcol.Count == 0) return;

        //    try
        //    {
        //        SaveFileDialog savefile = new SaveFileDialog();
        //        savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //        savefile.Title = "엑셀 다운로드";
        //        savefile.FileName = strFileName + ".xlsx";
        //        savefile.Filter = "All xlsx Files | *.xlsx";

        //        if (savefile.ShowDialog() == DialogResult.OK)
        //        {
        //            Excel.Application excel = new Excel.Application();
        //            excel.DisplayAlerts = false;
        //            Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);
        //            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];
        //            Excel.Range range = null;

        //            worksheet.Name = strFileName;

        //            //데이터 바인딩
        //            for (int i = 0; i < strcol.Count; i++)
        //            {
        //                //헤더 바인딩
        //                range = (Excel.Range)worksheet.Cells[1, i + 1];
        //                range.Cells.NumberFormat = "@";
        //                worksheet.Cells[1, i + 1] = strcol[i].ToString();
        //                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
        //                range.Borders.Weight = Excel.XlBorderWeight.xlThin;
        //                range.Interior.Color = ColorTranslator.ToOle(Color.GreenYellow);
        //                range.EntireColumn.AutoFit();

        //                //내용 바인딩
        //                for (int j = 0; j < data.Rows.Count; j++)
        //                {
        //                    range = (Excel.Range)worksheet.Cells[j + 2, i + 1];
        //                    range.Cells.NumberFormat = "@";
        //                    worksheet.Cells[j + 2, i + 1] = data.Rows[j][i].ToString();
        //                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
        //                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;

        //                    range.EntireColumn.AutoFit();
        //                }
        //            }

        //            excel.Visible = true;

        //            workbook.SaveAs(savefile.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing
        //                , Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        /// <summary>
        /// object[,]를 DataTable로
        /// </summary>
        /// <param name="valueArray"></param>
        /// <returns></returns>
        private static DataTable ProcessObjects(object[,] valueArray)
        {
            DataTable dt = new DataTable();

            try
            {
                //컬럼이름 생성
                for (int k = 1; k <= valueArray.GetLength(1); k++)
                {
                    dt.Columns.Add((string)valueArray[1, k]);
                }

                //데이터 바인딩
                object[] singleDValue = new object[valueArray.GetLength(1)];

                for (int i = 2; i <= valueArray.GetLength(0); i++)
                {
                    for (int j = 0; j < valueArray.GetLength(1); j++)
                    {
                        if (valueArray[i, j + 1] != null)
                        {
                            singleDValue[j] = valueArray[i, j + 1].ToString();
                        }
                        else
                        {
                            singleDValue[j] = valueArray[i, j + 1];
                        }
                    }
                    dt.LoadDataRow(singleDValue, LoadOption.PreserveChanges);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// object[,]를 DataTable로
        /// </summary>
        /// <param name="valueArray"></param>
        /// <returns></returns>
        private static DataTable ProcessObjects(object[,] headArray, object[,] valueArray)
        {
            DataTable dt = new DataTable();

            try
            {
                //컬럼이름 생성
                if (headArray != null)
                {
                    for (int k = 1; k <= headArray.GetLength(1); k++)
                    {
                        if ((string)headArray[1, k] != null)
                        {
                            dt.Columns.Add((string)headArray[1, k]);
                        }
                        else
                        {
                            dt.Columns.Add(((char)Convert.ToInt32(k + 64)).ToString());
                        }
                    }
                }
                else
                {
                    for (int k = 1; k <= valueArray.GetLength(1); k++)
                    {
                        dt.Columns.Add(((char)Convert.ToInt32(k + 64)).ToString());
                    }
                }


                //데이터 바인딩
                object[] singleDValue = new object[valueArray.GetLength(1)];

                for (int i = 1; i <= valueArray.GetLength(0); i++)
                {
                    for (int j = 0; j < valueArray.GetLength(1); j++)
                    {
                        if (valueArray[i, j + 1] != null)
                        {
                            if (valueArray[i, j + 1] is DateTime)
                                if (((DateTime)valueArray[i, j + 1]).Second != 00)
                                    singleDValue[j] = ((DateTime)valueArray[i, j + 1]).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:00");
                                else
                                    singleDValue[j] = ((DateTime)valueArray[i, j + 1]).ToString("yyyy-MM-dd HH:mm:ss");

                            else
                                singleDValue[j] = valueArray[i, j + 1].ToString();
                        }
                        else
                        {
                            singleDValue[j] = valueArray[i, j + 1];
                        }
                    }
                    dt.LoadDataRow(singleDValue, LoadOption.PreserveChanges);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region 커스텀
        /// <summary>
        /// DataTable(밴드)을 그대로 보여주기
        /// data : DataTable
        /// strBand : data밴드명(같은값 데이터일 경우 Merge)
        /// strcol : data컬럼명
        /// strFileName : 저장 파일명
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strcol"></param>
        /// <param name="strFileName"></param>
        public static void ExcelWQStatistics(Hashtable htTable, Hashtable htBand, Hashtable htcol, string strFileName)
        {
            if (htTable.Count == 0 || htBand.Count == 0 || htcol.Count == 0 || strFileName == "") return;

            try
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                savefile.Title = "엑셀 다운로드";
                savefile.FileName = strFileName + ".xlsx";
                savefile.Filter = "All xlsx Files | *.xlsx";

                if (savefile.ShowDialog() == DialogResult.OK)
                {
                    Excel.Application excel = new Excel.Application();
                    excel.DisplayAlerts = false;

                    Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    worksheet.Name = strFileName;

                    Excel.Range range = null;

                    long Rcnt = 0;
                    int Ccnt = 0;

                    for (int i = 1; i < htTable.Count + 1; i++)
                    {
                        DataTable temp = (DataTable)htTable[i.ToString()];
                        Rcnt = Rcnt + temp.Rows.Count + 2;

                        if (Ccnt < temp.Columns.Count) Ccnt = temp.Columns.Count;
                    }

                    object[,] datas = new object[Rcnt, Ccnt];

                    int Bandcnt = 0;
                    List<int> inthead = new List<int>();

                    //밴드 정보 + 데이터 정보
                    for (int i = 1; i < htBand.Count + 1; i++)
                    {
                        List<string> strband = (List<string>)htBand[i.ToString()];
                        List<string> strcol = (List<string>)htcol[i.ToString()];

                        DataTable data = (DataTable)htTable[i.ToString()];

                        for (int j = 0; j < strband.Count; j++)
                        {
                            datas[Bandcnt, j] = strband[j]; //밴드 정보
                            inthead.Add(Bandcnt);
                        }
                        Bandcnt++;

                        for (int j = 0; j < strcol.Count; j++)
                        {
                            datas[Bandcnt, j] = strcol[j]; //헤더 정보
                            inthead.Add(Bandcnt);
                        }
                        Bandcnt++;

                        for (int j = 0; j < data.Rows.Count; j++)
                        {
                            for (int k = 0; k < data.Columns.Count; k++)
                            {
                                datas[Bandcnt, k] = data.Rows[j][k].ToString(); //데이터 정보
                            }
                            Bandcnt++;
                        }
                    }

                    //range 설정
                    Excel.Range sPoint = worksheet.Cells[1, 1];
                    Excel.Range ePoint = worksheet.Cells[Rcnt, Ccnt];
                    range = worksheet.get_Range(sPoint, ePoint);
                    range.Value2 = datas;
                    range.EntireColumn.AutoFit();
                    range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    range.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    range.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    //헤더 범위
                    foreach (int inth in inthead)
                    {
                        Excel.Range HsPoint = worksheet.Cells[inth + 1, 1];
                        Excel.Range HePoint = worksheet.Cells[inth + 1, Ccnt];
                        range = worksheet.get_Range(HsPoint, HePoint);
                        range.Interior.Color = ColorTranslator.ToOle(Color.GreenYellow);

                        int Fcell = 0;
                        int Lcell = 0;

                        for (int i = 0; i < Ccnt - 1; i++)
                        {
                            if (!datas[inth, i].Equals(datas[inth, i + 1]))
                            {
                                Lcell = i;
                                Excel.Range ep = worksheet.Cells[inth + 1, Fcell + 1];
                                Excel.Range sp = worksheet.Cells[inth + 1, Lcell + 1];
                                range = worksheet.get_Range(sp, ep);
                                range.Merge(true);
                                Fcell = i + 1;
                            }
                        }
                    }

                    workbook.SaveAs(savefile.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                    //프로세스 Kill 후 재실행
                    int hwnd;
                    GetWindowThreadProcessId(excel.Hwnd, out hwnd);

                    Process p = Process.GetProcessById(hwnd);
                    p.Kill();

                    Process process = new Process();
                    process.StartInfo.FileName = savefile.FileName;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion





        /// <summary>
        /// 엑셀다운로드 그리드일반
        /// </summary>
        /// <param name="strExcelInfo"></param>
        /// <param name="titlePointXY"></param>
        /// <param name="dtTableData"></param>
        /// <param name="tablePointXY"></param>
        /// <param name="gridControl"></param>
        /// <param name="bLineYN"></param>
        public static void ExcelGrid(string strExcelFormPath, string strFileName, string title, DataTable dtTableData, int[] tablePointXY, GridControl gridControl, bool bLineYN)
        {
            try
            {
                Excel.Application excelApp = null;
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                excelApp = new Excel.Application();
                excelApp.DisplayAlerts = false;

                wb = excelApp.Workbooks.Open(strExcelFormPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ws = (Excel.Worksheet)wb.Sheets.get_Item(1);







                #region 그리드헤더정보

                //그리드 컨트롤에서 bands 복사하여 리스트의 인덱스 0부터 담음
                GridColumn[] columnList = new GridColumn[gridControl.Columns.Count];
                gridControl.Columns.CopyTo(columnList, 0);

                //데이터 쓰기 시작하는 셀 x,y
                int intStartX_Bnd = tablePointXY[0];
                int intStartY_Bnd = tablePointXY[1];

                //데이터 쓰기 종료되는 셀 x,y
                int intEndX_Bnd = tablePointXY[0];
                int intEndY_Bnd = tablePointXY[1];

                foreach (GridColumn gcol in columnList)
                {
                    string strColHeader = string.Empty;
                    bool bColVisible = false;
                    string wid = string.Empty;

                    gridControl.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        strColHeader = gcol.Header.ToString();
                        bColVisible = gcol.Visible;
                        wid = gcol.Width.ToString();
                    }));

                    //가시적으로 표현되는 컬럼만 엑셀로 내보낸다.
                    if (bColVisible == true)
                    {
                        Excel.Range rangeHeader = null;

                        //range 끝 좌표 x값만 +1 증가시킨 후 merge
                        Excel.Range sPoint = ws.Cells[intStartX_Bnd, intStartY_Bnd];
                        Excel.Range ePoint = ws.Cells[intEndX_Bnd + 1, intEndY_Bnd];

                        rangeHeader = ws.get_Range(sPoint, ePoint);
                        ws.get_Range(sPoint, ePoint).Merge();
                        rangeHeader.Value = strColHeader;

                        //다음 데이터가 쓰여질 시작 y좌표 +1
                        intStartY_Bnd++;
                        //다음 데이터가 쓰여질 끝 y좌표 +1
                        intEndY_Bnd++;

                        if (bLineYN)
                        {
                            rangeHeader.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            rangeHeader.Interior.Color = Color.Gray;
                            
                            //rangeHeader.Columns.AutoFit();
                            //컬럼width비율에 따라 폭조정
                            int width = Convert.ToInt16(wid.Replace("*",""));
                            rangeHeader.Columns.ColumnWidth = 5* width;
                            rangeHeader.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        }


                    }
                }


                #endregion




                #region 타이틀 표시
                Excel.Range sPoint_Bnd = ws.Cells[1, 1];
                Excel.Range ePoint_Bnd = ws.Cells[1, columnList.Count()];


                // range 설정
                Excel.Range titleRange = ws.get_Range(sPoint_Bnd, ePoint_Bnd); 
                ws.get_Range(sPoint_Bnd, ePoint_Bnd).Merge();
                titleRange.Value2 = title;
                titleRange.Font.Size = 30;
                titleRange.Font.FontStyle = Excel.XlThemeFont.xlThemeFontMajor;
                titleRange.Font.FontStyle = "Bold";
                titleRange.Font.Underline = Excel.XlUnderlineStyle.xlUnderlineStyleSingle;
                titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                //rangeSearchCondition.EntireColumn.AutoFit();

                #endregion



                #region 그리드데이터 부분

                //표 데이터 내보내기 전 표 하단의 그룹데이터(합계,평균 등) 유무 확인 후 처리
                //그룹데이터(합계,평균 등) 개수 확인용
                int intMaxRN = 0;

                //그룹데이터(합계,평균 등) 여부 확인 & RN 컬럼 삭제
                foreach (DataColumn dc in dtTableData.Columns)
                {
                    if (dc.ColumnName.Equals("RN"))
                    {
                        intMaxRN = dtTableData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x[dc.ColumnName].ToString())).Max(x => Convert.ToInt32(x[dc.ColumnName]));
                        dtTableData.Columns.Remove(dc);

                        //DataTable 수정 후 루프돌면 에러나기떄문에 break 처리
                        break;
                    }
                }

                //표 데이터 내보내기 부분
                Excel.Range rangeTable = null;

                long TDRowCNT = dtTableData.Rows.Count;
                int TDColumCNT = dtTableData.Columns.Count;

                object[,] tableDatas = new object[TDRowCNT, TDColumCNT];

                for (int i = 0; i < TDColumCNT; i++)
                {
                    for (int j = 0; j < TDRowCNT; j++)
                    {
                        tableDatas[j, i] = dtTableData.Rows[j][i].ToString(); //데이터 정보
                    }
                }

                //테이블 데이터 쓰기 시작되는 셀 x,y (Row,Column)
                //밴드 이후로 조정하기 위해 +2
                int intStartX_TD = tablePointXY[0] + 2;
                int intStartY_TD = tablePointXY[1];

                //엑셀 데이터 쓰기 종료되는 셀 x,y
                int intEndX_TD = (int)TDRowCNT + intStartX_TD - 1;
                int intEndY_TD = (int)TDColumCNT + intStartY_TD - 1;

                // range 설정
                Excel.Range sPoint_TD = ws.Cells[intStartX_TD, intStartY_TD];
                Excel.Range ePoint_TD = ws.Cells[intEndX_TD, intEndY_TD];

                rangeTable = ws.get_Range(sPoint_TD, ePoint_TD);
                rangeTable.Value2 = tableDatas;
                if (bLineYN)
                {
                    rangeTable.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }


                //RN 이 존재했을경우 = 그룹데이터 존재
                if (intMaxRN != 0)
                {
                    //표 데이터 그룹데이터(합계,평균 등) 색 변경 범위
                    Excel.Range rangeColor = null;

                    // range 설정
                    Excel.Range sPoint_Color = ws.Cells[intEndX_TD - intMaxRN + 1, intStartY_TD];
                    Excel.Range ePoint_Color = ws.Cells[intEndX_TD, intEndY_TD];

                    rangeColor = ws.get_Range(sPoint_Color, ePoint_Color);
                    rangeColor.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.ColorTranslator.FromHtml("#DDDDDD"));
                }
                #endregion




                wb.SaveAs(strFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                wb.Close(true);

                //excelApp.Quit();

                //ReleaseExcelObject(ws);
                //ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);

                //프로세스 Kill 후 재실행
                int intHwnd;
                GetWindowThreadProcessId(excelApp.Hwnd, out intHwnd);

                Process p = Process.GetProcessById(intHwnd);
                p.Kill();

                Process process = new Process();
                process.StartInfo.FileName = strFileName;
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
