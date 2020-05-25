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


    public class LekSiteDtlViewModel : INotifyPropertyChanged 
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
        public DelegateCommand<object> DelCommand { get; set; }
        

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
        LekSiteDtlView lekSiteDtlView;
        
        Button btnSave;
        Button btnClose;

        private LeakDtl dtl = new LeakDtl(); //민원마스터

        string _FTR_CDE;
        string _FTR_IDN;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public LekSiteDtlViewModel()
        {

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                lekSiteDtlView = obj as LekSiteDtlView;

                btnSave = lekSiteDtlView.btnSave;
                btnClose = lekSiteDtlView.btnClose;

                _FTR_CDE = lekSiteDtlView.txtFTR_CDE.Text;
                _FTR_IDN = lekSiteDtlView.txtFTR_IDN.Text;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                //4.초기조회
                InitModel();

            });

            //저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(lekSiteDtlView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트는 따로 처리
                    this.Dtl.REP_EXP = new TextRange(lekSiteDtlView.richREP_EXP.Document.ContentStart, lekSiteDtlView.richREP_EXP.Document.ContentEnd).Text.Trim();
                    this.Dtl.LEK_EXP = new TextRange(lekSiteDtlView.richLEK_EXP.Document.ContentStart, lekSiteDtlView.richLEK_EXP.Document.ContentEnd).Text.Trim();
                    BizUtil.Update2(this.Dtl, "SaveWtlLeakDtl");
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                    return;
                }

                Messages.ShowOkMsgBox();
                InitModel();
                //화면닫기
                //btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            });


            
            //삭제
            this.DelCommand = new DelegateCommand<object>(delegate (object obj) {
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectBizIdFileDtl");
                param.Add("BIZ_ID", _FTR_CDE + _FTR_IDN);
                DataTable dt = BizUtil.SelectList(param);

                if (dt.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("누수사진 내역이 존재합니다.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("누수지점을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    BizUtil.Update2(this.Dtl, "DeleteWtlLeakDtl");
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

        //초기모델조회
        private void InitModel()
        {
            //1.상세마스터
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWtlLeakDtl");
            param.Add("FTR_CDE", _FTR_CDE);
            param.Add("FTR_IDN", _FTR_IDN);

            LeakDtl result = new LeakDtl();
            result = BizUtil.SelectObject(param) as LeakDtl;
            this.Dtl = result;

            //다큐먼트는 따로 처리
            Paragraph p = new Paragraph();
            try
            {
                p.Inlines.Add(this.Dtl.REP_EXP ?? "");
                lekSiteDtlView.richREP_EXP.Document.Blocks.Clear();
                lekSiteDtlView.richREP_EXP.Document.Blocks.Add(p);
            }
            catch (Exception){}

            p = new Paragraph();
            try
            {
                p.Inlines.Add(this.Dtl.LEK_EXP ?? "");
                lekSiteDtlView.richLEK_EXP.Document.Blocks.Clear();
                lekSiteDtlView.richLEK_EXP.Document.Blocks.Add(p);
            }
            catch (Exception){}



        }



        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //행정구역
                BizUtil.SetCombo(lekSiteDtlView.cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
                //누수원인
                BizUtil.SetCmbCode(lekSiteDtlView.cbLRS_CDE, "250044", "선택");
                //누수상태
                BizUtil.SetCmbCode(lekSiteDtlView.cbLEP_CDE, "250043", "선택");
                //관재질
                BizUtil.SetCmbCode(lekSiteDtlView.cbMOP_CDE, "250102", "선택");

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
