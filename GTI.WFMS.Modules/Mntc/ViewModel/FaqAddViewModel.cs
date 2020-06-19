using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{
    public class FaqAddViewModel : FaqDtl
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
        FaqAddView faqAddView;
        ComboBoxEdit cbFTR_CDE;
        ComboBoxEdit cbFAQ_CAT_CDE; 
        ComboBoxEdit cbFAQ_CUZ_CDE; 
        
        Button btnBack;
        Button btnSave;

        #endregion




        /// 생성자
        public FaqAddViewModel()
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
            // 0.화면객체인스턴스화
            if (obj == null) return;

            faqAddView = obj as FaqAddView;
            cbFTR_CDE = faqAddView.cbFTR_CDE;
            cbFAQ_CAT_CDE = faqAddView.cbFAQ_CAT_CDE;
            cbFAQ_CUZ_CDE= faqAddView.cbFAQ_CUZ_CDE;
            btnBack = faqAddView.btnBack;
            btnSave = faqAddView.btnSave;

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();


        }









        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(faqAddView)) return;

            

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //다큐먼트는 따로 처리
                this.QUESTION = new TextRange(faqAddView.richQUESTION.Document.ContentStart, faqAddView.richQUESTION.Document.ContentEnd).Text.Trim();
                this.REPL = new TextRange(faqAddView.richREPL.Document.ContentStart, faqAddView.richREPL.Document.ContentEnd).Text.Trim();
                BizUtil.Update2(this, "SaveFaqDtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
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
                // cbFTR_CDE 시설물
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", "선택");
                // cbFAQ_CAT_CDE 구분
                BizUtil.SetCmbCode(cbFAQ_CAT_CDE, "250109", "선택");
                BizUtil.SetCmbCode(cbFAQ_CUZ_CDE, "250110", "선택");
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
