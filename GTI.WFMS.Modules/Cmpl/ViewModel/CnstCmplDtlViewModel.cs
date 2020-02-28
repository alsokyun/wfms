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

namespace GTI.WFMS.Modules.Cmpl.ViewModel
{


    public class CnstCmplDtlViewModel 
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public LeakDtl Dtl
        {
            get {return dtl; }
            set {dtl = value; }
        }



        #endregion


        #region ==========  Member 정의 ==========
        CnstCmplDtlView cnstCmplDtlView;
        
        Button btnSave;
        Button btnClose;

        private LeakDtl dtl;
        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public CnstCmplDtlViewModel()
        {

            dtl = new LeakDtl();

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                cnstCmplDtlView = obj as CnstCmplDtlView;

                btnSave = cnstCmplDtlView.btnSave;
                btnClose = cnstCmplDtlView.btnClose;
                

                //1.초기조회
                InitModel();


                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();


            });

            //저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(cnstCmplDtlView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    BizUtil.Update2(this.Dtl, "SaveLeakDtl");
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
            

        }







        #region ============= 메소드정의 ================

        //초기모델조회
        private void InitModel()
        {
            //1.상세마스터
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectCmplDtl");
            param.Add("RCV_NUM", this.Dtl.RCV_NUM);

            LeakDtl result = new LeakDtl();
            result = BizUtil.SelectObject(param) as LeakDtl;

            //2.누수지점
            param = new Hashtable();
            param.Add("sqlId", "SelectCmplLeakList");
            param.Add("RCV_NUM", this.Dtl.RCV_NUM);

            DataTable dt = new DataTable();
            dt = BizUtil.SelectList(param);

            cnstCmplDtlView.grid.ItemsSource = dt;
        }



        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //행정구역
                BizUtil.SetCombo(cnstCmplDtlView.cbAPL_HJD, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);
                //민원구분
                BizUtil.SetCmbCode(cnstCmplDtlView.cbAPL_CDE, "APL_CDE", true, "250056");
                //민원처리상태
                BizUtil.SetCmbCode(cnstCmplDtlView.cbPRO_CDE, "PRO_CDE", true, "250050");
                
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
