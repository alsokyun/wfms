using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Stat.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace GTI.WFMS.Modules.Stat.ViewModel
{
    class FcltStatListViewModel : INotifyPropertyChanged
    {

        #region ==========  페이징관련 INotifyPropertyChanged  ==========

        public event PropertyChangedEventHandler PropertyChanged;

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

        #endregion

        #region ========== Members 정의 ==========        
        FcltStatListView fcltStatListView;
        GridControl grid;

        DateEdit dtSEARCH;
        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public FcltStatListViewModel()
        {

            LoadedCommand = new DelegateCommand<object>(OnLoaded);

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
                       
            dtSEARCH.EditValue = DateTime.Today;

            //1. 화면객체 인스턴스
            fcltStatListView = values[0] as FcltStatListView;
            grid = fcltStatListView.grid;

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
                Hashtable param = new Hashtable();

                string sDate = Convert.ToDateTime(DateTime.Today).ToString("yyyyMMdd");
                string sSchDate = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");
                int nYear = Int32.Parse(Convert.ToDateTime(DateTime.Today).ToString("yyyy"));
                
                param.Add("sqlId", "SelectFcltStatList");
                param.Add("searchKeyword", sDate);

                DataTable dt = BizUtil.SelectList(param);

                fcltStatListView.grdTitle.Header = sSchDate;
                fcltStatListView.grdTitle1.Header = nYear + "년";
                fcltStatListView.grdTitle2.Header = (nYear - 1) + "년";
                fcltStatListView.grdTitle3.Header = (nYear - 2) + "년";
                fcltStatListView.grdTitle4.Header = (nYear - 3) + "년";
                fcltStatListView.grdTitle5.Header = (nYear - 4) + "년";

                fcltStatListView.grid.ItemsSource = dt;

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

        }

        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
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