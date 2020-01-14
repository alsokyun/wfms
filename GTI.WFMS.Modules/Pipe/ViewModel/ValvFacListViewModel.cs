﻿using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Pipe.Work;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pipe.View;
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

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    class ValvFacListViewModel : INotifyPropertyChanged
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
        PipeWork pipeWork = new PipeWork();
        CmmWork cmmWork = new CmmWork();

        DataTable dtresult = new DataTable(); //조회결과 데이터


        ValvFacListView valvFacListView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();
        ComboBoxEdit cbVAL_MOP; DataTable dtVAL_MOP = new DataTable();
        ComboBoxEdit cbVAL_MOF; DataTable dtVAL_MOF = new DataTable();
        ComboBoxEdit cbVAL_FOR; DataTable dtVAL_FOR = new DataTable();

        TextEdit txtFTR_IDN;
        TextEdit txtCNT_NUM;
        TextEdit txtSHT_NUM;
        TextEdit txtVAL_DIP;

        DateEdit dtIST_YMD_FROM;
        DateEdit dtIST_YMD_TO;

        GridControl grid;

        //엑셀다운로드 관련
        System.Windows.Forms.SaveFileDialog saveFileDialog;
        Thread thread;
        string strFileName;
        string strExcelFormPath = AppDomain.CurrentDomain.BaseDirectory + "/Resources/Excel/FmsBaseExcel.xlsx";
        DataTable exceldt;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public ValvFacListViewModel()
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
            valvFacListView = values[0] as ValvFacListView;

            cbMNG_CDE = valvFacListView.cbMNG_CDE;      //0.관리기관
            cbHJD_CDE = valvFacListView.cbHJD_CDE;      //2.행정동
            cbVAL_MOF = valvFacListView.cbVAL_MOF;      //7.형식
            cbVAL_MOP = valvFacListView.cbVAL_MOP;      //8.관재질
            cbVAL_FOR = valvFacListView.cbVAL_FOR;      //10.시설물형태

            txtFTR_IDN = valvFacListView.txtFTR_IDN;    //1.관리번호           
            txtCNT_NUM = valvFacListView.txtCNT_NUM;    //3.공사번호
            txtSHT_NUM = valvFacListView.txtSHT_NUM;    //4.도엽번호
            txtVAL_DIP = valvFacListView.txtVAL_DIP;    //9.구경

            dtIST_YMD_FROM = valvFacListView.dtIST_YMD_FROM;    //5.설치일자(이상)
            dtIST_YMD_TO = valvFacListView.dtIST_YMD_TO;        //6.설치일자(이하)
            dtIST_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
            dtIST_YMD_TO.DisplayFormatString = "yyyy-MM-dd";

            //dtIST_YMD_FROM.EditValue = DateTime.Today.AddYears(-10);
            //dtIST_YMD_TO.EditValue = DateTime.Today;

            grid = valvFacListView.grid;


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
                conditions.Add("VAL_MOF", cbVAL_MOF.EditValue.ToString().Trim());
                conditions.Add("VAL_MOP", cbVAL_MOP.EditValue.ToString().Trim());

                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("SHT_NUM", txtSHT_NUM.Text.Trim());
                conditions.Add("VAL_DIP", txtVAL_DIP.Text.Trim());

                try
                {
                    conditions.Add("IST_YMD_FROM", dtIST_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtIST_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("IST_YMD_TO", dtIST_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtIST_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception e) { }

                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                //dtresult = pipeWork.SelectWtlPipeList(conditions);
                conditions.Add("sqlId", "SelectValvFacList");
                //dtresult = BizUtil.SelectList(conditions);
                //grid.ItemsSource = dtresult;

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
            cbVAL_MOF.SelectedIndex = 0;
            cbVAL_MOP.SelectedIndex = 0;

            txtFTR_IDN.Text = "";
            txtCNT_NUM.Text = "";
            txtSHT_NUM.Text = "";
            txtVAL_DIP.Text = "";

            //dtIST_YMD_FROM.EditValue = DateTime.Today.AddYears(-10);
            //dtIST_YMD_TO.EditValue = DateTime.Today;
            dtIST_YMD_FROM.EditValue = null;
            dtIST_YMD_TO.EditValue = null;

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
                conditions.Add("VAL_MOF", cbVAL_MOF.EditValue.ToString().Trim());
                conditions.Add("VAL_MOP", cbVAL_MOP.EditValue.ToString().Trim());

                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("SHT_NUM", txtSHT_NUM.Text.Trim());
                conditions.Add("VAL_DIP", txtVAL_DIP.Text.Trim());

                try
                {
                    conditions.Add("IST_YMD_FROM", dtIST_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtIST_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("IST_YMD_TO", dtIST_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtIST_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception e) { }
                
                conditions.Add("page", 0);
                conditions.Add("rows", 1000000);

                conditions.Add("sqlId", "SelectValvFacList");

                exceldt = BizUtil.SelectList(conditions);

                saveFileDialog = null;
                saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "저장경로를 지정하세요.";

                //초기 파일명 지정
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_" + "변류시설목록.xlsx";

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
                valvFacListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                new Action((delegate ()
                {
                    (valvFacListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                })));



                //엑셀 표 데이터
                DataTable dtExceltTableData = exceldt.DefaultView.ToTable(false, new string[] { "IS_GEOMETRY", "FTR_NAM", "MNG_NAM", "FTR_IDN", "HJD_NAM", "CNT_NUM", "SHT_NUM", "IST_YMD", "MOF_NAM", "MOP_NAM", "VAL_DIP", "FOR_NAM" });

                int[] tablePointXY = { 3, 1 };
                

                //엑셀 유틸 호출
                //ExcelUtil.ExcelTabulation(strFileName, strExcelFormPath, startPointXY, strSearchCondition, dtExceltTableData);
                ExcelUtil.ExcelGrid(strExcelFormPath, strFileName, "변류시설목록", dtExceltTableData, tablePointXY, grid, true);

                valvFacListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (valvFacListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowInfoMsgBox("엑셀 다운로드가 완료되었습니다.");
                   })));
            }
            catch (Exception ex)
            {
                valvFacListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (valvFacListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
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


                cbMNG_CDE = valvFacListView.cbMNG_CDE;      //0.관리기관
                cbHJD_CDE = valvFacListView.cbHJD_CDE;      //2.행정동
                cbVAL_MOF = valvFacListView.cbVAL_MOF;      //7.형식
                cbVAL_MOP = valvFacListView.cbVAL_MOP;      //8.관재질
                cbVAL_FOR = valvFacListView.cbVAL_FOR;      //10.시설물형태

                txtFTR_IDN = valvFacListView.txtFTR_IDN;    //1.관리번호           
                txtCNT_NUM = valvFacListView.txtCNT_NUM;    //3.공사번호
                txtSHT_NUM = valvFacListView.txtSHT_NUM;    //4.도엽번호
                txtVAL_DIP = valvFacListView.txtVAL_DIP;    //9.구경

                dtIST_YMD_FROM = valvFacListView.dtIST_YMD_FROM;    //5.설치일자(이상)
                dtIST_YMD_TO = valvFacListView.dtIST_YMD_TO;        //6.설치일자(이하)
                dtIST_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
                dtIST_YMD_TO.DisplayFormatString = "yyyy-MM-dd";

                // cbMNG_CDE    0.관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbHJD_CDE    2.행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbVAL_MOF    7.형식
                BizUtil.SetCmbCode(cbVAL_MOF, "MOF_CDE", true, "250016");

                // cbVAL_MOP    8.관재질
                BizUtil.SetCmbCode(cbVAL_MOP, "MOP_CDE", true, "250015");

                // cbVAL_FOR    10.시설물형태
                BizUtil.SetCmbCode(cbVAL_FOR, "FOR_CDE", true);
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


            string name_space = "GTI.WFMS.Modules.Pipe.Model";
            string class_name = "ValvFacDtl";

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectValvFacDtl");
            param.Add("FTR_CDE", "SA200");
            param.Add("FTR_IDN", "30739");
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