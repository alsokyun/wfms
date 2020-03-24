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
    public class WtrTrkAddViewModel : WtrTrkDtl
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
        WtrTrkAddView wtrTrkAddView;

        ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();	    //지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();		//행정동
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();      //관리기관
        ComboBoxEdit cbBLS_CDE; DataTable dtBLS_CDE = new DataTable();      //건물유형
        

        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public WtrTrkAddViewModel()
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

                wtrTrkAddView = values[0] as WtrTrkAddView;
                cbFTR_CDE = wtrTrkAddView.cbFTR_CDE;       //지형지물
                cbHJD_CDE = wtrTrkAddView.cbHJD_CDE;       //행정동
                cbMNG_CDE = wtrTrkAddView.cbMNG_CDE;       //관리기관
                cbBLS_CDE = wtrTrkAddView.cbBLS_CDE;       //건물유형

                btnBack = wtrTrkAddView.btnBack;
                btnSave = wtrTrkAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWtrTrkFTR_IDN");

                WtrTrkDtl result = new WtrTrkDtl();
                result = BizUtil.SelectObject(param) as WtrTrkDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA120";

                this.FNS_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");

                //공통팝업창 사이즈 변경 4
                FmsUtil.popWinView.Height = 400;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        /// <summary>
        /// 저장작업FNS_YMD
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(wtrTrkAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertWtrTrkDtl");
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
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", false);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);
                
                // cbBLS_CDE 건물유형
                BizUtil.SetCmbCode(cbBLS_CDE, "BLS_CDE", true);

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
