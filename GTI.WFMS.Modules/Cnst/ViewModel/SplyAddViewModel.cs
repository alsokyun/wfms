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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    class SplyAddViewModel : SplyDtl
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
        SplyAddView splyAddView;
        ComboBoxEdit cbHJD_CDE; 
        Button btnBack;
        Button btnSave;
        Button btnDup;
        
        #endregion




        /// 생성자
        public SplyAddViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.BackCommand = new DelegateCommand<object>(OnBack);


            //입력항목 변경되면 중복버튼 복원
            this.DUP = "체크";
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args) {
                try
                {
                    btnDup.Content = "체크";
                }
                catch (Exception) { }
            };

            this.DupCommand = new DelegateCommand<object>(delegate(object obj) {
                if (btnDup.Content.Equals("OK")) return;


                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWttSplyMaDup");
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
            //throw new NotImplementedException();

            // 0.화면객체인스턴스화
            if (obj == null) return;

            splyAddView = obj as SplyAddView;
            cbHJD_CDE = splyAddView.cbHJD_CDE;
            btnBack = splyAddView.btnBack;
            btnSave = splyAddView.btnSave;
            btnDup = splyAddView.btnDup;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();


            //공통팝업창 사이즈 변경
            FmsUtil.popWinView.Height = 240;

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if(!BizUtil.ValidReq(splyAddView))  return;

            // 공사번호중복체크
            if (btnDup.Content.Equals("체크"))
            {
                Messages.ShowInfoMsgBox("공사번호 (중복)체크를 하세요.");
                return;
            }
            



            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "InsertSplyDtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                return;
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
                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "[선택하세요]");
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
