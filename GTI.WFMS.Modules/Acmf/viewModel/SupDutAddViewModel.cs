﻿using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Acmf.Model;
using GTI.WFMS.Modules.Acmf.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Acmf.ViewModel
{
    public class SupDutAddViewModel : SupDutDtl
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        SupDutAddView supDutAddView;
        //ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();	//지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();		//행정동
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();      //관리기관
        ComboBoxEdit cbSAA_CDE; DataTable dtSAA_CDE = new DataTable();      //관용도
        ComboBoxEdit cbJHT_CDE; DataTable dtJHT_CDE = new DataTable();      //접합종류
        

        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public SupDutAddViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.BackCommand = new DelegateCommand<object>(OnBack);
            
        }





        #region ==========  이벤트 핸들러 ==========

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            try
            {
                // 0.화면객체인스턴스화
                if (obj == null) return;
                var values = (object[])obj;

                supDutAddView = values[0] as SupDutAddView;
               // cbFTR_CDE = supDutAddView.cbFTR_CDE;     //지형지물
                cbHJD_CDE = supDutAddView.cbHJD_CDE;       //행정동
                cbMNG_CDE = supDutAddView.cbMNG_CDE;       //관리기관
                cbSAA_CDE = supDutAddView.cbSAA_CDE;       //관용도
                cbJHT_CDE = supDutAddView.cbJHT_CDE;       //접합종류

                btnBack = supDutAddView.btnBack;
                btnSave = supDutAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectSupDutFTR_IDN");

                SupDutDtl result = new SupDutDtl();
                result = BizUtil.SelectObject(param) as SupDutDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA002";

                this.IST_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");

                //공통팝업창 사이즈 변경 1
                FmsUtil.popWinView.Height = 280;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        /// <summary>
        /// 저장작업IST_YMD
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(supDutAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertSupDutDtl");
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + e.Message);
                return;
            }
            Messages.ShowOkMsgBox();

            BackCommand.Execute(null); //닫기
        }

       
        /// <summary>
        /// 뒤로가기처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnBack(object obj)
        {
            //MessageBox.Show("OnBack");
            btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                // cbFTR_CDE 지형지물
                //BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", false);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "선택");
                
                // cbSAA_CDE 관용도
                BizUtil.SetCmbCode(cbSAA_CDE, "250018", "선택");

                // cbJHT_CDE 접합종류
                BizUtil.SetCmbCode(cbJHT_CDE, "250026", "선택");

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
                        btnSave.Visibility = Visibility.Collapsed;
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
