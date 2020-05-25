using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    class CnstMngAddViewModel : CnstDtl
    {

        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }
        public DelegateCommand<object> DupCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        CnstMngAddView cnstMngAddView;
        ComboBoxEdit cbCTT_CDE; 
        ComboBoxEdit cbCNT_CDE; 
        Button btnBack;
        Button btnSave;
        Button btnDup;

        #endregion                    

        /// 생성자
        public CnstMngAddViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.BackCommand = new DelegateCommand<object>(OnBack);

            //입력항목 변경되면 중복버튼 복원
            this.DUP = "체크";
            PropertyChanged += delegate (object sender, PropertyChangedEventArgs args) {
                try
                {
                    btnDup.Content = "체크";
                }
                catch (Exception) { }
            };

            this.DupCommand = new DelegateCommand<object>(delegate (object obj) {
                if (btnDup.Content.Equals("OK")) return;


                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWttConsMaDup");
                param.Add("CNT_NUM", this.CNT_NUM);
                DataTable dt = BizUtil.SelectList(param);
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowInfoMsgBox("공사번호가 중복되었습니다.");
                }
                else
                {
                    btnDup.Content = "OK";
                }
            });

        }


        #region ==========  이벤트 핸들러 ==========

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            // 0.화면객체인스턴스화
            if (obj == null) return;

            cnstMngAddView = obj as CnstMngAddView;
            cbCTT_CDE = cnstMngAddView.cbCTT_CDE;
            cbCNT_CDE = cnstMngAddView.cbCNT_CDE;
            btnBack = cnstMngAddView.btnBack;
            btnSave = cnstMngAddView.btnSave;
            btnDup = cnstMngAddView.btnDup;


            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();


            //공통팝업창 사이즈 변경
            FmsUtil.popWinView.Height = 400;

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(cnstMngAddView)) return;

            // 공사번호중복체크
            if (btnDup.Content.Equals("체크"))
            {
                Messages.ShowInfoMsgBox("공사번호 (중복)체크를 하세요.");
                return;
            }


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertCnstMngDtl");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            Messages.ShowOkMsgBox();
            btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                // cbCTT_CDE 계약구분
                BizUtil.SetCmbCode(cbCTT_CDE, "250038", "선택");

                // cbCNT_CDE 공사구분
                BizUtil.SetCmbCode(cbCNT_CDE, "250039", "선택");
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
