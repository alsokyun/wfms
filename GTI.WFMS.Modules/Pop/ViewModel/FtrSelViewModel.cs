using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class FtrSelViewModel : INotifyPropertyChanged
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
                RaisePropertyChanged("PageIndex");
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
                RaisePropertyChanged("ItemCnt");
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
                RaisePropertyChanged("TotalCnt");
            }
        }


        // pageIndex가 변경될때 이벤트연동
        protected void RaisePropertyChanged(string propertyName)
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


        FtrSelView ftrSelView;
        ComboBoxEdit cbHJD_CDE; 
        ComboBoxEdit cbFTR_CDE; 
        
        TextEdit txtFTR_IDN;
        TextEdit txtCNT_NUM;
        TextEdit txtFTR_NAM;
        TextEdit txtCNT_NAM;

        TextBlock txtFTR_CDE;
        TextBlock txtHJD_NAM;
        
        GridControl grid;

        Button btnClose;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public FtrSelViewModel()
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
                        if (ftrSelView.WindowState == WindowState.Maximized)
                        {
                            ftrSelView.Top = Mouse.GetPosition(ftrSelView).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                            ftrSelView.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(ftrSelView).X + 20;

                            ftrSelView.WindowState = WindowState.Normal;
                        }
                        ftrSelView.DragMove();
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
            var values = (object[])obj;

            //1. 화면객체 인스턴스
            ftrSelView = values[0] as FtrSelView;

            cbHJD_CDE = ftrSelView.cbHJD_CDE;
            cbFTR_CDE = ftrSelView.cbFTR_CDE;        
            txtFTR_IDN = ftrSelView.txtFTR_IDN;
            txtFTR_NAM = ftrSelView.txtFTR_NAM;
            txtFTR_CDE = ftrSelView.txtFTR_CDE;
            txtHJD_NAM = ftrSelView.txtHJD_NAM;
            txtCNT_NAM = ftrSelView.txtCNT_NAM;
            txtCNT_NUM = ftrSelView.txtCNT_NUM;
            

            btnClose = ftrSelView.btnClose;
            
            grid = ftrSelView.grid;


            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();


            //4.초기조회
            //SearchAction(null);
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
                conditions.Add("HJD_CDE", cbHJD_CDE.EditValue);
                conditions.Add("FTR_CDE", cbFTR_CDE.EditValue);                
                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("FTR_NAM", FmsUtil.Trim(txtFTR_NAM.EditValue));                
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("CNT_NAM", txtCNT_NAM.Text.Trim());
                //conditions.Add("USE_YB", "Y");
                
                conditions.Add("sqlId", "SelectFtrAllList");

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
                    catch (Exception )
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
            cbHJD_CDE.SelectedIndex = 0;
            cbFTR_CDE.SelectedIndex = 0;            
            txtFTR_IDN.Text = "";
            txtFTR_NAM.Text = "";
            txtFTR_CDE.Text = "";
            txtHJD_NAM.Text = "";
            txtCNT_NUM.Text = "";
            txtCNT_NAM.Text = "";
            
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
            if (cnt<1)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }
            else if (cnt > 1)
            {
                Messages.ShowInfoMsgBox("시설물을 하나만 선택하세요.");
                return;
            }


            for (int i=0; i< ((DataTable)grid.ItemsSource).Rows.Count; i++)
            {
                Hashtable conditions = new Hashtable();
                try
                {
                    if ("Y".Equals(((DataTable)grid.ItemsSource).Rows[i]["CHK"]))
                    {
                        //선태한 시설물을 화면에 저장
                        txtFTR_CDE.Text = ((DataTable)grid.ItemsSource).Rows[i]["FTR_CDE"].ToString();
                        txtFTR_IDN.Text = ((DataTable)grid.ItemsSource).Rows[i]["FTR_IDN"].ToString();
                        txtFTR_NAM.Text = ((DataTable)grid.ItemsSource).Rows[i]["FTR_NAM"].ToString();
                        txtHJD_NAM.Text = ((DataTable)grid.ItemsSource).Rows[i]["HJD_NAM"].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.ToString());
                }
            }

            //화면닫기
            btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
                // cbFTR_CDE 시설물구분
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", "선택");
                
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



        // 시설물 지도상 위치찾아가기
        private void cellPosMethod(object obj)
        {
            string FTR_IDN = obj as string;
            MessageBox.Show("지도상 위치찾아가기..FTR_IDN - " + FTR_IDN);
        }







        #endregion

    }
}
