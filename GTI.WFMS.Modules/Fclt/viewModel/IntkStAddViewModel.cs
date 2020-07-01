﻿using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fclt.Model;
using GTI.WFMS.Modules.Fclt.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Fclt.ViewModel
{
    public class IntkStAddViewModel : IntkStDtl
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
        IntkStAddView intkStAddView;
        //ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();	//지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();		//행정동
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();      //관리기관
        ComboBoxEdit cbWSR_CDE; DataTable dtWSR_CDE = new DataTable();      //수원구분
        ComboBoxEdit cbWRW_CDE; DataTable dtWRW_CDE = new DataTable();      //도수방법
        ComboBoxEdit cbWGW_CDE; DataTable dtWGW_CDE = new DataTable();      //취수방법


        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public IntkStAddViewModel()
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

                intkStAddView = values[0] as IntkStAddView;
               // cbFTR_CDE = intkStAddView.cbFTR_CDE;     //지형지물
                cbHJD_CDE = intkStAddView.cbHJD_CDE;       //행정동
                cbMNG_CDE = intkStAddView.cbMNG_CDE;       //관리기관
                cbWSR_CDE = intkStAddView.cbWSR_CDE;       //수원구분
                cbWRW_CDE = intkStAddView.cbWRW_CDE;       //도수방법
                cbWGW_CDE = intkStAddView.cbWGW_CDE;       //취소방법

                btnBack = intkStAddView.btnBack;
                btnSave = intkStAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectIntkStFTR_IDN");

                IntkStDtl result = new IntkStDtl();
                result = BizUtil.SelectObject(param) as IntkStDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA112";

                //this.FNS_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");

                //공통팝업창 사이즈 변경 3
                FmsUtil.popWinView.Height = 360;

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
            if (!BizUtil.ValidReq(intkStAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertIntkStDtl");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
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
                
                // cbWSR_CDE 수원구분
                BizUtil.SetCmbCode(cbWSR_CDE, "250058", "선택");

                // cbWRW_CDEE 도수방법
                BizUtil.SetCmbCode(cbWRW_CDE, "250046", "선택");

                // cbWRW_CDEE 취소방법
                BizUtil.SetCmbCode(cbWGW_CDE, "250069", "선택");

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
