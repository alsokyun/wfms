using DevExpress.Xpf.Editors;
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
using GTI.WFMS.Modules.Pop.View;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using GTIFramework.Common.Utils.Converters;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class CnstCmplSelViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> SelCmd { get; set; }
        public DelegateCommand<object> WindowMoveCommand { get; set; }

        #endregion



        #region ========== Members 정의 ==========
        CmmWork cmmWork = new CmmWork();

        DataTable dtresult = new DataTable(); //조회결과 데이터


        CnstCmplSelView cnstCmplSelView;

        ComboBoxEdit cbHJD_CDE;
        ComboBoxEdit cbAPL_CDE; 
        ComboBoxEdit cbPRO_CDE;

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
        public CnstCmplSelViewModel()
        {

            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            ResetCommand = new DelegateCommand<object>(ResetAction);
            SelCmd = new DelegateCommand<object>(SelAciton);

            // 윈도우 마우스드래그
            WindowMoveCommand = new DelegateCommand<object>(delegate (object objt)
            {
                try
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        if (cnstCmplSelView.WindowState == WindowState.Maximized)
                        {
                            cnstCmplSelView.Top = Mouse.GetPosition(cnstCmplSelView).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                            cnstCmplSelView.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(cnstCmplSelView).X + 20;

                            cnstCmplSelView.WindowState = WindowState.Normal;
                        }
                        cnstCmplSelView.DragMove();
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBoxLog(ex);
                }
            });


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
            cnstCmplSelView = obj as CnstCmplSelView;

            cbAPL_CDE = cnstCmplSelView.cbAPL_CDE;
            cbPRO_CDE = cnstCmplSelView.cbPRO_CDE;
            cbHJD_CDE = cnstCmplSelView.cbHJD_CDE;

            grid = cnstCmplSelView.grid;


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
                conditions.Add("RCV_NUM", cnstCmplSelView.txtRCV_NUM.Text.Trim());
                conditions.Add("APL_HJD", cbHJD_CDE.EditValue.ToString().Trim());
                conditions.Add("APL_CDE", cbAPL_CDE.EditValue.ToString().Trim());
                conditions.Add("PRO_CDE", cbPRO_CDE.EditValue.ToString().Trim());
                conditions.Add("RCV_YMD_FROM", cnstCmplSelView.dtRCV_YMD_FROM.EditValue == null ? "" : Convert.ToDateTime(cnstCmplSelView.dtRCV_YMD_FROM.EditValue).ToString("yyyyMMdd"));
                conditions.Add("RCV_YMD_TO", cnstCmplSelView.dtRCV_YMD_TO.EditValue == null ? "" : Convert.ToDateTime(cnstCmplSelView.dtRCV_YMD_TO.EditValue).ToString("yyyyMMdd"));
                conditions.Add("PRO_YMD_FROM", cnstCmplSelView.dtPRO_YMD_FROM.EditValue == null ? "" : Convert.ToDateTime(cnstCmplSelView.dtPRO_YMD_FROM.EditValue).ToString("yyyyMMdd"));
                conditions.Add("PRO_YMD_TO", cnstCmplSelView.dtPRO_YMD_TO.EditValue == null ? "" : Convert.ToDateTime(cnstCmplSelView.dtPRO_YMD_TO.EditValue).ToString("yyyyMMdd"));


                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                conditions.Add("sqlId", "SelectCnstCmplList");

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
            cbAPL_CDE.SelectedIndex = 0;
            cbPRO_CDE.SelectedIndex = 0;
            cbHJD_CDE.SelectedIndex = 0;
            cnstCmplSelView.txtRCV_NUM.Text = "";
            cnstCmplSelView.dtRCV_YMD_FROM.EditValue = null;
            cnstCmplSelView.dtRCV_YMD_TO.EditValue = null;
            cnstCmplSelView.dtPRO_YMD_FROM.EditValue = null;
            cnstCmplSelView.dtPRO_YMD_TO.EditValue = null;
        }



        /// <summary>
        /// 시설물선택 
        /// </summary>
        /// <param name="obj"></param>
        private void SelAciton(object obj)
        {
            int cnt = 0;
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                if ("Y".Equals(dr["CHK"]))
                {
                    cnt++;
                }
            }
            if (cnt < 1)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }
            else if (cnt > 1)
            {
                Messages.ShowInfoMsgBox("민원항목을 하나만 선택하세요.");
                return;
            }


            for (int i = 0; i < ((DataTable)grid.ItemsSource).Rows.Count; i++)
            {
                Hashtable conditions = new Hashtable();
                try
                {
                    if ("Y".Equals(((DataTable)grid.ItemsSource).Rows[i]["CHK"]))
                    {
                        //선태한 시설물을 화면에 저장
                        cnstCmplSelView.txbRCV_NUM.Text = ((DataTable)grid.ItemsSource).Rows[i]["RCV_NUM"].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.ToString());
                }
            }

            //화면닫기
            cnstCmplSelView.btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);
                // 민원구분
                BizUtil.SetCmbCode(cbAPL_CDE, "APL_CDE", true, "250056");
                // 처리상태
                BizUtil.SetCmbCode(cbPRO_CDE, "PRO_CDE", true, "250050");
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
