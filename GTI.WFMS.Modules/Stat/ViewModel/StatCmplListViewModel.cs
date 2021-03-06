﻿using DevExpress.Xpf.Grid;
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
    class StatCmplListViewModel : INotifyPropertyChanged
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
        
        StatCmplListView statCmplListView;
        GridControl grid;

        #endregion



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        
        #endregion



        #region ========== Members 정의 ==========
        DataTable dtresult = new DataTable(); //조회결과 데이터                      
        
        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public StatCmplListViewModel()
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

            //1. 화면객체 인스턴스
            statCmplListView = values[0] as StatCmplListView;
            grid = statCmplListView.grid;

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

                param.Add("sqlId", "SelectStatCmplList");
                param.Add("searchKeyword", sDate);

                DataTable dt = BizUtil.SelectList(param);

                statCmplListView.grid.ItemsSource = dt;

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
        #endregion


        #region ============= 메소드정의 ================


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