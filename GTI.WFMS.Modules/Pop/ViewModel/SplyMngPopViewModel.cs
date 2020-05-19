using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GTI.WFMS.Modules.Pop.View;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class SplyMngPopViewModel : INotifyPropertyChanged
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
        #endregion



        #region ========== Members 정의 ==========
        CmmWork cmmWork = new CmmWork();

        DataTable dtresult = new DataTable(); //조회결과 데이터


        SplyMngPopView splyMngPopView;
        ComboBoxEdit cbHJD_CDE;


        #endregion
               
        /// <summary>
        /// 생성자
        /// </summary>
        public SplyMngPopViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            ResetCommand  = new DelegateCommand<object>(ResetAction);            

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
            splyMngPopView = obj as SplyMngPopView;

            cbHJD_CDE = splyMngPopView.cbHJD_CDE;


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
                conditions.Add("CNT_NUM", splyMngPopView.txtCNT_NUM.Text.Trim());
                conditions.Add("HJD_CDE", cbHJD_CDE.EditValue);
                try
                {
                    conditions.Add("BEG_YMD_FROM", splyMngPopView.dtBEG_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(splyMngPopView.dtBEG_YMD_FROM.EditValue).ToString("yyyyMMdd"));
                    conditions.Add("BEG_YMD_TO", splyMngPopView.dtBEG_YMD_TO.EditValue == null ? null : Convert.ToDateTime(splyMngPopView.dtBEG_YMD_TO.EditValue).ToString("yyyyMMdd"));
                }
                catch (Exception) { }
                try
                {
                    conditions.Add("FNS_YMD_FROM", splyMngPopView.dtFNS_YMD_FROM.EditValue == null ? null : Convert.ToDateTime(splyMngPopView.dtFNS_YMD_FROM.EditValue).ToString("yyyyMMdd"));
                    conditions.Add("FNS_YMD_TO", splyMngPopView.dtFNS_YMD_TO.EditValue == null ? null : Convert.ToDateTime(splyMngPopView.dtFNS_YMD_TO.EditValue).ToString("yyyyMMdd"));
                }
                catch (Exception) { }
                if (!BizUtil.ValidDateBtw(conditions["BEG_YMD_FROM"], conditions["BEG_YMD_TO"]))
                {
                    Messages.ShowInfoMsgBox("From/To 일자를 확인하세요");
                    return;
                }
                if (!BizUtil.ValidDateBtw(conditions["FNS_YMD_FROM"], conditions["FNS_YMD_TO"]))
                {
                    Messages.ShowInfoMsgBox("From/To 일자를 확인하세요");
                    return;
                }

                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                conditions.Add("sqlId", "SelectWttSplyMaList");

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
            splyMngPopView.txtCNT_NUM.Text = "";
            cbHJD_CDE.SelectedIndex = 0;
            splyMngPopView.dtBEG_YMD_FROM.EditValue = null;
            splyMngPopView.dtBEG_YMD_TO.EditValue = null;
            splyMngPopView.dtFNS_YMD_FROM.EditValue = null;
            splyMngPopView.dtFNS_YMD_TO.EditValue = null;
        }

        #endregion


        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try {
                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "[전체]");
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
