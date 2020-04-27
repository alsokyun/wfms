using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GTI.WFMS.GIS.Pop.View;
using DevExpress.Xpf.Grid;

namespace GTI.WFMS.GIS.Pop.ViewModel
{
    public class CnstMngPopViewModel : INotifyPropertyChanged
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
        public RelayCommand<object> LoadedCommand { get; set; }
        public RelayCommand<object> SearchCommand { get; set; }
        public RelayCommand<object> ResetCommand { get; set; }        
        #endregion



        #region ========== Members 정의 ==========
        CmmWork cmmWork = new CmmWork();

        DataTable dtresult = new DataTable(); //조회결과 데이터


        CnstMngPopView cnstMngPopView;

        ComboBoxEdit cbCNT_CDE; DataTable dtCNT_CDE = new DataTable();
        ComboBoxEdit cbCTT_CDE; DataTable dtCTT_CDE = new DataTable();
        
        TextEdit txtCNT_NUM;
        TextEdit txtCNT_NAM;
        TextEdit txtCNT_LOC;
        TextEdit txtTCT_AMT_FROM;
        TextEdit txtTCT_AMT_TO;
        
        DateEdit dtBEG_YMD_FROM;
        DateEdit dtBEG_YMD_TO;
        DateEdit dtFNS_YMD_FROM;
        DateEdit dtFNS_YMD_TO;

        GridControl grid;

        #endregion
               
        /// <summary>
        /// 생성자
        /// </summary>
        public CnstMngPopViewModel()
        {
            LoadedCommand = new RelayCommand<object>(OnLoaded);
            SearchCommand = new RelayCommand<object>(SearchAction);
            ResetCommand  = new RelayCommand<object>(ResetAction);            

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
            cnstMngPopView = values[0] as CnstMngPopView;

            txtCNT_NUM = cnstMngPopView.txtCNT_NUM;
            txtCNT_NAM = cnstMngPopView.txtCNT_NAM;
            txtCNT_LOC = cnstMngPopView.txtCNT_LOC;
            txtTCT_AMT_FROM = cnstMngPopView.txtTCT_AMT_FROM;
            txtTCT_AMT_TO = cnstMngPopView.txtTCT_AMT_TO;
            cbCNT_CDE = cnstMngPopView.cbCNT_CDE;
            cbCTT_CDE = cnstMngPopView.cbCTT_CDE;

            dtBEG_YMD_FROM = cnstMngPopView.dtBEG_YMD_FROM;
            dtBEG_YMD_TO = cnstMngPopView.dtBEG_YMD_TO;
            dtBEG_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
            dtBEG_YMD_TO.DisplayFormatString = "yyyy-MM-dd";
  
            dtFNS_YMD_FROM = cnstMngPopView.dtFNS_YMD_FROM;
            dtFNS_YMD_TO = cnstMngPopView.dtFNS_YMD_TO;
            dtFNS_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
            dtFNS_YMD_TO.DisplayFormatString = "yyyy-MM-dd";

            grid = cnstMngPopView.grid;

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
                Hashtable conditions = new Hashtable();
                conditions.Add("CNT_CDE", cbCNT_CDE.EditValue);
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("CNT_NAM", txtCNT_NAM.Text.Trim());
                conditions.Add("TCT_AMT_FROM", txtTCT_AMT_FROM.EditValue);
                conditions.Add("TCT_AMT_TO", txtTCT_AMT_TO.EditValue);

                try
                {
                    conditions.Add("BEG_YMD_FROM", dtBEG_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtBEG_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("BEG_YMD_TO", dtBEG_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtBEG_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception ) { }
                try
                {
                    conditions.Add("FNS_YMD_FROM", dtFNS_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("FNS_YMD_TO", dtFNS_YMD_TO.EditValue == null ? null : Convert.ToDateTime(dtFNS_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception ) { }
                conditions.Add("CTT_CDE", cbCTT_CDE.EditValue);
                conditions.Add("CNT_LOC", txtCNT_LOC.Text.Trim());

                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                conditions.Add("sqlId", "SelectWttConsMaList");

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
                    catch (Exception )
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
            cbCNT_CDE.SelectedIndex = 0;
            cbCTT_CDE.SelectedIndex = 0;
            txtCNT_NAM.Text = "";
            txtCNT_NUM.Text = "";
            txtTCT_AMT_FROM.Text = "";
            txtTCT_AMT_TO.Text = "";
            txtCNT_LOC.Text = "";

            dtBEG_YMD_FROM.EditValue = null;
            dtBEG_YMD_TO.EditValue = null;
            dtFNS_YMD_FROM.EditValue = null;
            dtFNS_YMD_TO.EditValue = null;

        }

        #endregion


        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try {
                // cbCNT_CDE
                BizUtil.SetCmbCode(cbCNT_CDE, "250039", "[선택하세요]");
                // cbCTT_CDE
                BizUtil.SetCmbCode(cbCTT_CDE, "250038", "[선택하세요]");

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
