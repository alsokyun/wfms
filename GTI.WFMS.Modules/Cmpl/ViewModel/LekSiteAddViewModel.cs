using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Cmpl.View;
using GTI.WFMS.Models.Cmpl.Model;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace GTI.WFMS.Modules.Cmpl.ViewModel
{


    public class LekSiteAddViewModel : INotifyPropertyChanged 
    {

        /// <summary>
        /// 인터페이스 구현부분
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        

        public LeakDtl Dtl
        {
            get {return dtl; }
            set
            {
                dtl = value;
                OnPropertyChanged("Dtl");
            }
        }



        #endregion


        #region ==========  Member 정의 ==========
        LekSiteAddView lekSiteAddView;
        
        Button btnSave;
        Button btnClose;

        private LeakDtl dtl = new LeakDtl(); //민원마스터

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public LekSiteAddViewModel()
        {

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                lekSiteAddView = obj as LekSiteAddView;

                btnSave = lekSiteAddView.btnSave;
                btnClose = lekSiteAddView.btnClose;


                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

            });

            //저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(lekSiteAddView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트는 따로 처리
                    this.Dtl.REP_EXP = new TextRange(lekSiteAddView.richREP_EXP.Document.ContentStart, lekSiteAddView.richREP_EXP.Document.ContentEnd).Text;
                    this.Dtl.LEK_EXP = new TextRange(lekSiteAddView.richLEK_EXP.Document.ContentStart, lekSiteAddView.richLEK_EXP.Document.ContentEnd).Text;
                    BizUtil.Update2(this.Dtl, "SaveWtlLeakDtl");
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                    return;
                }

                Messages.ShowOkMsgBox();
                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            });


            
         
        }







        #region ============= 메소드정의 ================

               

        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //행정구역
                BizUtil.SetCombo(lekSiteAddView.cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);
                //지형지물
                BizUtil.SetCombo(lekSiteAddView.cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", false);
                //누수원인
                BizUtil.SetCmbCode(lekSiteAddView.cbLRS_CDE, "250044", true);
                //누수상태
                BizUtil.SetCmbCode(lekSiteAddView.cbLEP_CDE, "250043", true);
                //관재질
                BizUtil.SetCmbCode(lekSiteAddView.cbMOP_CDE, "250102", true);

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
