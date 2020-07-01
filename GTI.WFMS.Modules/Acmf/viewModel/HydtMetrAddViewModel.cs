using DevExpress.Xpf.Editors;
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
    public class HydtMetrAddViewModel : HydtMetrDtl
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
        HydtMetrAddView hydtMetrAddView;
        ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();	    //지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();		//행정동
        ComboBoxEdit cbHOM_HJD; DataTable dtHOM_HJD = new DataTable();      //수용가행정동
        ComboBoxEdit cbSBI_CDE; DataTable dtSBI_CDE = new DataTable();      //업종
        ComboBoxEdit cbMET_MOF; DataTable dtMET_MOF = new DataTable();      //형식


        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public HydtMetrAddViewModel()
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

                hydtMetrAddView = values[0] as HydtMetrAddView;
                cbFTR_CDE = hydtMetrAddView.cbFTR_CDE;     //지형지물
                cbHJD_CDE = hydtMetrAddView.cbHJD_CDE;       //행정동
                cbHOM_HJD = hydtMetrAddView.cbHOM_HJD;       //수용가행정동
                cbSBI_CDE = hydtMetrAddView.cbSBI_CDE;       //업종
                cbMET_MOF = hydtMetrAddView.cbMET_MOF;       //형식

                btnBack = hydtMetrAddView.btnBack;
                btnSave = hydtMetrAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectHydtMetrFTR_IDN");

                HydtMetrDtl result = new HydtMetrDtl();
                result = BizUtil.SelectObject(param) as HydtMetrDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA122";//급수전계량기

                this.IST_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");
            
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
            if (!BizUtil.ValidReq(hydtMetrAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertHydtMetrDtl");
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
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM");

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");

                // cbHOM_HJD 수용가행정동
                BizUtil.SetCombo(cbHOM_HJD, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");

                // cbSBI_CDE 업종
                BizUtil.SetCmbCode(cbSBI_CDE, "250020", "선택");

                // cbMET_MOF 형식
                BizUtil.SetCmbCode(cbMET_MOF, "250004", "선택");

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
