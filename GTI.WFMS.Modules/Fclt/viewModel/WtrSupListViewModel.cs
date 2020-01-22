﻿using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Fclt.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using GTIFramework.Common.Utils.Converters;
using System.Collections.Generic;

namespace GTI.WFMS.Modules.Fclt.ViewModel
{
    class WtrSupListViewModel : INotifyPropertyChanged
    {

        #region ==========  페이징관련 INotifyPropertyChanged  ==========

        public event PropertyChangedEventHandler PropertyChanged;


        /// 조회결과 리스트데이터
        public ObservableCollection<DataTable> PagedCollection { get; set; }

        // 페이징인덱스 뷰와동기화처리를 위한 이벤트설정
        int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                if (value == pageIndex) return;
                pageIndex = value;
                OnPropertyChanged("PageIndex");
            }
        }

        // 총페이지개수 : 뷰와동기화처리를 위한 이벤트설정
        int itemCnt = 0;
        public int ItemCnt
        {
            get { return itemCnt; }
            set
            {
                if (value == itemCnt) return;
                itemCnt = value;
                OnPropertyChanged("ItemCnt");
            }
        }

        // 총페이지개수 : 뷰와동기화처리를 위한 이벤트설정
        int totalCnt = 0;
        public int TotalCnt
        {
            get { return totalCnt; }
            set
            {
                if (value == totalCnt) return;
                totalCnt = value;
                OnPropertyChanged("TotalCnt");
            }
        }


        // pageIndex가 변경될때 이벤트연동
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }

        public DelegateCommand<object> SearchCommand { get; set; }
        public DelegateCommand<object> ResetCommand { get; set; }
        public DelegateCommand<object> ExcelCmd { get; set; }
        
        public DelegateCommand<object> btnCmd { get; set; }

        #endregion



        #region ========== Members 정의 ==========
        CmmWork cmmWork = new CmmWork();

        DataTable dtresult = new DataTable(); //조회결과 데이터


        WtrSupListView wtrSupListView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();
        ComboBoxEdit cbSAG_CDE; DataTable dsSAG_CDE = new DataTable();
        ComboBoxEdit cbSCW_CDE; DataTable dsSCW_CDE = new DataTable();

        TextEdit txtFTR_IDN;
        TextEdit txtCNT_NUM;
        TextEdit txtSHT_NUM;

        TextEdit txtSRV_NAM;
        TextEdit txtPUR_NAM;
               
        DateEdit dtFNS_YMD_FROM;
        DateEdit dtFNS_YMD_TO;

        GridControl grid;

        //엑셀다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread thread;
        string strFileName;
        string strExcelFormPath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Excel/FmsBaseExcel.xlsx";
        DataTable exceldt;

        GridColumn[] columnList;
        List<string> listCols;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public WtrSupListViewModel()
        {

            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            ResetCommand = new DelegateCommand<object>(ResetAction);

            btnCmd = new DelegateCommand<object>(btnMethod);
            ExcelCmd = new DelegateCommand<object>(ExcelDownAction);


            // 조회데이터 초기화
            this.PagedCollection = new ObservableCollection<DataTable>();
            // 프로퍼티변경이벤트 처리핸들러 등록
            PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case "PageIndex":
                        if (PageIndex < 0) //초기이벤트는 걸른다
                        {
                            this.pageIndex = 0;
                        }
                        else
                        {
                            SearchAction(PageIndex);
                        }
                        break;
                    default:
                        break;
                }
            };

        }






        #region ========== Event 정의 ==========
        /// <summary>
        /// 로드 바인딩
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;

            //1. 화면객체 인스턴스
            wtrSupListView = values[0] as WtrSupListView;

            cbMNG_CDE = wtrSupListView.cbMNG_CDE;      //0.관리기관
            cbHJD_CDE = wtrSupListView.cbHJD_CDE;      //2.행정동
            cbSAG_CDE = wtrSupListView.cbSAG_CDE;      //8.관리방법
            cbSCW_CDE = wtrSupListView.cbSCW_CDE;      //9.배수지제어방법

            txtFTR_IDN = wtrSupListView.txtFTR_IDN;    //1.관리번호           
            txtCNT_NUM = wtrSupListView.txtCNT_NUM;    //3.공사번호
            txtSHT_NUM = wtrSupListView.txtSHT_NUM;    //4.도엽번호
            txtSRV_NAM = wtrSupListView.txtSRV_NAM;    //7.배수지명            
            txtPUR_NAM = wtrSupListView.txtPUR_NAM;    //10.정수장명

            dtFNS_YMD_FROM = wtrSupListView.dtFNS_YMD_FROM;    //5.준공일자(이상)
            dtFNS_YMD_TO = wtrSupListView.dtFNS_YMD_TO;        //6.준공일자(이하)
            dtFNS_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
            dtFNS_YMD_TO.DisplayFormatString = "yyyy-MM-dd";
                      

            //dtFNS_YMD_FROM.EditValue = DateTime.Today.AddYears(-10);
            //dtFNS_YMD_TO.EditValue = DateTime.Today;

            grid = wtrSupListView.grid;


            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();
            
            //4.초기조회
            SearchAction(null);
        }


        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="obj"></param>
        private void SearchAction(object obj)
        {
            try
            {
                //if (treeList.FocusedNode == null) return;

                Hashtable conditions = new Hashtable();
                conditions.Add("MNG_CDE", cbMNG_CDE.EditValue.ToString().Trim());
                conditions.Add("HJD_CDE", cbHJD_CDE.EditValue.ToString().Trim());
                conditions.Add("SAG_CDE", cbSAG_CDE.EditValue.ToString().Trim());
                conditions.Add("SCW_CDE", cbSCW_CDE.EditValue.ToString().Trim());

                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("SHT_NUM", txtSHT_NUM.Text.Trim());
                conditions.Add("SRV_NAM", txtSRV_NAM.Text.Trim());
                conditions.Add("PUR_NAM", txtPUR_NAM.Text.Trim());
                

                try
                {
                    conditions.Add("FNS_YMD_FROM", dtFNS_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("FNS_YMD_TO", dtFNS_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception e) { }

                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                conditions.Add("sqlId", "SelectWtrSupList");
    
                /*
                    조회후 페이징소스 업데이트
                 */
                int page_idx = 0;
                //페이지버튼으로 조회
                if (obj is int)
                {
                    page_idx = (int)obj;
                }
                //조회버튼으로 조회는 버튼위치(PageIndex) 초기화
                else
                {
                    PageIndex = -1;
                }
                BizUtil.SelectListPage(conditions, page_idx, delegate (DataTable dt) {
                    // TotalCnt 설정
                    try
                    {
                        this.TotalCnt = Convert.ToInt32(dt.Rows[0]["ROWCNT"]);
                        this.ItemCnt = (int)Math.Ceiling((double)this.TotalCnt / FmsUtil.PageSize);
                    }
                    catch (Exception e)
                    {
                        this.TotalCnt = 0;
                        this.ItemCnt = 0;
                    }

                    this.PagedCollection.Clear();
                    this.PagedCollection.Add(dt);
                });


            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="obj"></param>
        private void ResetAction(object obj)
        {
            cbMNG_CDE.SelectedIndex = 0;
            cbHJD_CDE.SelectedIndex = 0;
            cbSAG_CDE.SelectedIndex = 0;
            cbSCW_CDE.SelectedIndex = 0;           

            txtFTR_IDN.Text = "";
            txtCNT_NUM.Text = "";
            txtSHT_NUM.Text = "";
            txtSRV_NAM.Text = "";
            txtPUR_NAM.Text = "";
            
            dtFNS_YMD_FROM.EditValue = null;
            dtFNS_YMD_TO.EditValue = null;

        }

        /// <summary>
        /// 엑셀다운로드
        /// </summary>
        /// <param name="obj"></param>
        private void ExcelDownAction(object obj)
        {

            try
            {
                /// 데이터조회
                Hashtable conditions = new Hashtable();
                conditions.Add("MNG_CDE", cbMNG_CDE.EditValue.ToString().Trim());
                conditions.Add("HJD_CDE", cbHJD_CDE.EditValue.ToString().Trim());
                conditions.Add("SAG_CDE", cbSAG_CDE.EditValue.ToString().Trim());
                conditions.Add("SCW_CDE", cbSCW_CDE.EditValue.ToString().Trim());                

                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("SHT_NUM", txtSHT_NUM.Text.Trim());
                conditions.Add("SRV_NAM", txtSRV_NAM.Text.Trim());
                conditions.Add("PUR_NAM", txtPUR_NAM.Text.Trim());

                
                try
                {
                    conditions.Add("FNS_YMD_FROM", dtFNS_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("FNS_YMD_TO", dtFNS_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception e) { }
                
                conditions.Add("page", 0);
                conditions.Add("rows", 1000000);

                conditions.Add("sqlId", "SelectWtrSupList");

                exceldt = BizUtil.SelectList(conditions);

                saveFileDialog = null;
                saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "저장경로를 지정하세요.";

                //초기 파일명 지정
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_" + "배수지목록.xlsx";

                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.Filter = "Excel|*.xlsx";

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
                wtrSupListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                new Action((delegate ()
                {
                    (wtrSupListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                })));
                
                //엑셀 표 데이터
                int[] tablePointXY = { 3, 1 };
                DataTable dtExceltTableData = exceldt.DefaultView.ToTable(false, listCols.ToArray());

                //엑셀 유틸 호출
                //ExcelUtil.ExcelTabulation(strFileName, strExcelFormPath, startPointXY, strSearchCondition, dtExceltTableData);
                ExcelUtil.ExcelGrid(strExcelFormPath, strFileName, "배수지목록", dtExceltTableData, tablePointXY, grid, true);

                wtrSupListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (wtrSupListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowInfoMsgBox("엑셀 다운로드가 완료되었습니다.");
                   })));
            }
            catch (Exception ex)
            {
                wtrSupListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (wtrSupListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                        Messages.ShowErrMsgBoxLog(ex);
                    })));
            }
        }

        #endregion


        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {


                cbMNG_CDE = wtrSupListView.cbMNG_CDE;      //0.관리기관
                cbHJD_CDE = wtrSupListView.cbHJD_CDE;      //2.행정동
                cbSAG_CDE = wtrSupListView.cbSAG_CDE;      //8.관리방법
                cbSCW_CDE = wtrSupListView.cbSCW_CDE;      //9.배수지제어방법

                txtFTR_IDN = wtrSupListView.txtFTR_IDN;    //1.관리번호           
                txtCNT_NUM = wtrSupListView.txtCNT_NUM;    //3.공사번호
                txtSHT_NUM = wtrSupListView.txtSHT_NUM;    //4.도엽번호
                txtSRV_NAM = wtrSupListView.txtSRV_NAM;    //7.배수지명
                txtPUR_NAM = wtrSupListView.txtPUR_NAM;    //10.유효저수량(t)

                dtFNS_YMD_FROM = wtrSupListView.dtFNS_YMD_FROM;    //5.준공일자(이상)
                dtFNS_YMD_TO = wtrSupListView.dtFNS_YMD_TO;        //6.준공일자(이하)
                dtFNS_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
                dtFNS_YMD_TO.DisplayFormatString = "yyyy-MM-dd";

                // cbMNG_CDE    0.관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbHJD_CDE    2.행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbSAG_CDE    7.관리방법
                BizUtil.SetCmbCode(cbSAG_CDE, "SAG_CDE", true);

                // cbSCW_CDE    9.배수지제어방법
                BizUtil.SetCmbCode(cbSCW_CDE, "SCW_CDE", true);

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 화면 권한처리
        /// </summary>
        private void permissionApply()
        {
            try
            {
                string strPermission = Logs.htPermission[Logs.strFocusMNU_CD].ToString();
                switch (strPermission)
                {
                    case "W":
                        break;
                    case "R":
                        //btnAdd.Visibility = Visibility.Collapsed;
                        break;
                    case "N":
                        break;
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }

        }







        /// <summary>
        /// SQL DataRow -> 모델클래스 생성기
        /// </summary>
        /// <param name="obj"></param>
        private void btnMethod(object obj)
        {


            string name_space = "GTI.WFMS.Modules.Fclt.Model";
            string class_name = "ValvFacDtl";

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWtrSupList");
            param.Add("FTR_CDE", "SA114");
            param.Add("FTR_IDN", "10");
            DataTable dt = BizUtil.SelectList(param);
            DataRow dr = dt.Rows[0];

            String sb = "";
            sb += "namespace " + name_space + "\r\n";
            sb += "{ " + "\r\n";
            sb += " public class " + class_name + ": CmmDtl, INotifyPropertyChanged" + "\r\n";
            sb += " { " + "\r\n";
            sb += "     /// <summary>" + "\r\n";
            sb += "     /// 인터페이스 구현부분" + "\r\n";
            sb += "     /// </summary>" + "\r\n";
            sb += "     public event PropertyChangedEventHandler PropertyChanged;" + "\r\n";
            sb += "     protected void OnPropertyChanged(string propertyName)" + "\r\n";
            sb += "         { " + "\r\n";
            sb += "             if (PropertyChanged != null)" + "\r\n";
            sb += "             { " + "\r\n";
            sb += "                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));" + "\r\n";
            sb += "             } " + "\r\n";
            sb += "         } " + "\r\n";

            sb += "\r\n";
            sb += "\r\n";
            sb += "\r\n";

            sb += "     /// <summary>" + "\r\n";
            sb += "     /// 프로퍼티 부분" + "\r\n";
            sb += "     /// </summary>" + "\r\n";
            foreach (DataColumn col in dt.Columns)
            {
                string value = dr[col].ToString();

                //type 결정
                string type_name = "string";
                if (col.ColumnName.Contains("_AMT") || col.ColumnName.Contains("_DIP") || col.ColumnName.Contains("_DIR") )
                {
                    type_name = "decimal";
                }
                else
                {
                    switch (dr[col].GetType().Name.ToLower())
                    {
                        case "string":
                            type_name = "string";
                            break;
                        case "int":
                            type_name = "int";
                            break;
                        case "decimal":
                            type_name = "decimal";
                            break;
                        case "double":
                            type_name = "double";
                            break;
                        default:
                            break;
                    }
                }


                sb += "     private " + type_name + " __" + col + ";" + "\r\n";
                sb += "     public " + type_name + " " + col + "\r\n";
                sb += "     { " + "\r\n";
                sb += "         get { return __" + col + "; }" + "\r\n";
                sb += "         set " + "\r\n";
                sb += "         { " + "\r\n";
                sb += "         this.__" + col + " = value;" + "\r\n";
                sb += "         OnPropertyChanged(\"" + col + "\"); " + "\r\n";
                sb += "         } " + "\r\n";
                sb += "     } " + "\r\n";
            }

            sb += " } " + "\r\n";
            sb += "} " + "\r\n";


            Console.WriteLine("=========class string===========");
            Console.Write(sb);
            Console.WriteLine("=========class string===========");


            MessageBox.Show("모델생성 -> Console 확인");

        }
        #endregion

    }

}