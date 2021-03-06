﻿using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class ValvFacAddViewModel : ValvFacDtl
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
        ValvFacAddView valvFacAddView;
        ComboBoxEdit cbFTR_CDE; //지형지물
        ComboBoxEdit cbHJD_CDE; //행정동
        ComboBoxEdit cbMNG_CDE; //관리기관
        ComboBoxEdit cbVAL_MOF; //형식
        ComboBoxEdit cbVAL_MOP; //관재질
        ComboBoxEdit cbSAE_CDE; //제수변회전방향
        ComboBoxEdit cbMTH_CDE; //제수변구동방법
        ComboBoxEdit cbVAL_FOR; //시설물형태
        ComboBoxEdit cbCST_CDE; //이상상태
        ComboBoxEdit cbOFF_CDE; //개폐여부

        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public ValvFacAddViewModel()
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

                valvFacAddView = values[0] as ValvFacAddView;
                cbFTR_CDE = valvFacAddView.cbFTR_CDE;   //지형지물
                cbHJD_CDE = valvFacAddView.cbHJD_CDE;   //행정동
                cbMNG_CDE = valvFacAddView.cbMNG_CDE;   //관리기관
                cbVAL_MOF = valvFacAddView.cbVAL_MOF;   //형식
                cbVAL_MOP = valvFacAddView.cbVAL_MOP;   //관재질
                cbSAE_CDE = valvFacAddView.cbSAE_CDE;   //제수변회전방향
                cbMTH_CDE = valvFacAddView.cbMTH_CDE;   //제수변구동방법
                cbVAL_FOR = valvFacAddView.cbVAL_FOR;   //시설물형태
                cbCST_CDE = valvFacAddView.cbCST_CDE;   //이상상태
                cbOFF_CDE = valvFacAddView.cbOFF_CDE;   //개폐여부

                btnBack = valvFacAddView.btnBack;
                btnSave = valvFacAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectValvFacFTR_IDN");

                ValvFacDtl result = new ValvFacDtl();
                result = BizUtil.SelectObject(param) as ValvFacDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA200";

                this.IST_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");

                //공통팝업창 사이즈 원복
                FmsUtil.popWinView.Height = 440;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(valvFacAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertValvFacDtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
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
                Func<DataRow, bool> filter = Row => (Row.Field<string>("FTR_CDE").Contains("SA2") && !Row.Field<string>("FTR_CDE").Contains("SA206"));
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", null, filter);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "선택");


                // cbVAL_MOF 형식
                BizUtil.SetCmbCode(cbVAL_MOF, "250016", "선택");

                // cbVAL_MOP 관재질
                BizUtil.SetCmbCode(cbVAL_MOP, "250015", "선택");

                // cbSAE_CDE    제수변회전방향
                BizUtil.SetCmbCode(cbSAE_CDE, "250027", "선택");

                // cbMTH_CDE    제수변구동방법
                BizUtil.SetCmbCode(cbMTH_CDE, "250065", "선택");

                // cbVAL_FOR    시설물형태(=구조물형태)
                BizUtil.SetCmbCode(cbVAL_FOR, "250007", "선택");

                // cbCST_CDE    이상상태
                BizUtil.SetCmbCode(cbCST_CDE, "250062", "선택");

                // cbOFF_CDE    개폐여부
                BizUtil.SetCmbCode(cbOFF_CDE, "250036", "선택");

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
