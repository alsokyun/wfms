﻿using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GTI.WFMS.Modules.Mntc.View;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using GTIFramework.Common.Utils.Converters;
using System.Threading;
using System.Collections.Generic;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{
    public class OldMngListViewModel : INotifyPropertyChanged
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


        OldMngListView optMtListView;

        TextEdit txtFTR_IDN;
        TextEdit txtPRS_NAM;
        TextEdit txtATT_NAM;

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
        public OldMngListViewModel()
        {

            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            ResetCommand = new DelegateCommand<object>(ResetAction);
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

            //1. 화면객체 인스턴스
            optMtListView = obj as OldMngListView;

            txtFTR_IDN = optMtListView.txtFTR_IDN;
            txtPRS_NAM = optMtListView.txtPRS_NAM;
            txtATT_NAM = optMtListView.txtATT_NAM;

            grid = optMtListView.grid;


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
                conditions.Add("FTR_IDN", txtFTR_IDN.Text.Trim());
                conditions.Add("PRS_NAM", txtPRS_NAM.Text.Trim());
                conditions.Add("ATT_NAM", txtATT_NAM.Text.Trim());
                
                conditions.Add("sqlId", "SelectOldMngList");

                /*
                    조회후 페이징소스 업데이트
                 */
                int page_idx = 0;
                //페이징 버튼으로 조회
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
                    catch (Exception)
                    {
                        this.TotalCnt = 0;
                        this.ItemCnt = 0;
                    }

                    //조회결과 매핑
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
            txtFTR_IDN.Text = "";
            txtPRS_NAM.Text = "";
            txtATT_NAM.Text = "";
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
                conditions.Add("FTR_IDN", txtFTR_IDN.Text.Trim());
                conditions.Add("PRS_NAM", txtPRS_NAM.Text.Trim());
                conditions.Add("ATT_NAM", txtATT_NAM.Text.Trim());

                conditions.Add("page", 0);
                conditions.Add("rows", 1000000);

                conditions.Add("sqlId", "SelectOldMngList");


                exceldt = BizUtil.SelectList(conditions);


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
                saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_" + "시설물노후도현황.xlsx";

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
                optMtListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (optMtListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = true;
                    })));


                //엑셀 표 데이터
                DataTable dtExceltTableData = exceldt.DefaultView.ToTable(false, listCols.ToArray());

                int[] tablePointXY = { 3, 1 };


                //엑셀 유틸 호출
                //ExcelUtil.ExcelTabulation(strFileName, strExcelFormPath, startPointXY, strSearchCondition, dtExceltTableData);
                ExcelUtil.ExcelGrid(strExcelFormPath, strFileName, "시설물노후도현황", dtExceltTableData, tablePointXY, grid, true);

                optMtListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                   new Action((delegate ()
                   {
                       (optMtListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
                       Messages.ShowInfoMsgBox("엑셀 다운로드가 완료되었습니다.");
                   })));
            }
            catch (Exception ex)
            {
                optMtListView.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        (optMtListView.FindName("waitindicator") as WaitIndicator).DeferedVisibility = false;
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
            try {
                // cbPDT_CAT_CDE 소모품구분
                //BizUtil.SetCmbCode(cbPDT_CAT_CDE, "PDT_CAT_CDE", "전체");
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

        #endregion




    }

}
