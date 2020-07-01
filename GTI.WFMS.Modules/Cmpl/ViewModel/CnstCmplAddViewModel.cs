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


    public class CnstCmplAddViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> DupCommand { get; set; }

        public WserDtl Dtl
        {
            get { return dtl; }
            set
            {
                dtl = value;
                OnPropertyChanged("Dtl");
            }
        }



        #endregion


        #region ==========  Member 정의 ==========
        CnstCmplAddView cnstCmplAddView;

        Button btnSave;
        Button btnClose;
        Button btnDup;

        private WserDtl dtl;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public CnstCmplAddViewModel()
        {

            dtl = new WserDtl();

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                cnstCmplAddView = obj as CnstCmplAddView;

                btnSave = cnstCmplAddView.btnSave;
                btnClose = cnstCmplAddView.btnClose;
                btnDup = cnstCmplAddView.btnDup;


                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();


                //4.민원번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectRevNum");
                DataTable  dt = BizUtil.SelectList(param);
                string rcvNum = "";
                try
                {
                    rcvNum = dt.Rows[0]["RCV_NUM"].ToString();
                }
                catch (Exception){}
                this.Dtl.RCV_NUM = rcvNum;


            });

            //저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(cnstCmplAddView)) return;

                // 민원번호중복체크
                if (btnDup.Content.Equals("체크"))
                {
                    Messages.ShowInfoMsgBox("민원번호 (중복)체크를 하세요.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트는 따로 처리
                    this.Dtl.APL_EXP = new TextRange(cnstCmplAddView.richAPL_EXP.Document.ContentStart, cnstCmplAddView.richAPL_EXP.Document.ContentEnd).Text.Trim();
                    this.Dtl.PRO_EXP = new TextRange(cnstCmplAddView.richPRO_EXP.Document.ContentStart, cnstCmplAddView.richPRO_EXP.Document.ContentEnd).Text.Trim();
                    BizUtil.Update2(this.Dtl, "SaveCmplWserMa");
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


            //입력항목 변경되면 중복버튼 복원
            this.Dtl.DUP = "체크";
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
                param.Add("sqlId", "SelectWserDup");
                param.Add("RCV_NUM", this.Dtl.RCV_NUM);
                DataTable dt = BizUtil.SelectList(param);
                if (dt.Rows.Count > 1)
                {
                    Messages.ShowInfoMsgBox("민원번호가 중복되었습니다.");
                }
                else
                {
                    btnDup.Content = "OK";
                }
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
                BizUtil.SetCombo(cnstCmplAddView.cbAPL_HJD, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
                //민원구분
                BizUtil.SetCmbCode(cnstCmplAddView.cbAPL_CDE, "250056", "선택");
                //민원처리상태
                BizUtil.SetCmbCode(cnstCmplAddView.cbPRO_CDE, "250050", "선택");

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
