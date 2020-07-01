using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using GTI.WFMS.Modules.Blk.View;
using GTI.WFMS.Models.Blk.Model;
using System.ComponentModel;
using System.Collections.Generic;

namespace GTI.WFMS.Modules.Blk.ViewModel
{
    public class Blk02AddViewModel : INotifyPropertyChanged
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

                if (propertyName == "UPPER_FTR_CDE")
                {
                    BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);
                }
            }
        }


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }

        private BlkDtl dtl = new BlkDtl ();
        public BlkDtl Dtl
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
        Blk02AddView blk02AddView;

        ComboBoxEdit cbMNG_CDE;
        ComboBoxEdit cbFTR_CDE;
        ComboBoxEdit cbUPPER_FTR_CDE;
        ComboBoxEdit cbUPPER_FTR_IDN;

        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public Blk02AddViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SaveCommand = new DelegateCommand<object>(OnSave);
            BackCommand = new DelegateCommand<object>(OnBack);
            
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

                blk02AddView = obj as Blk02AddView;
                cbMNG_CDE = blk02AddView.cbMNG_CDE;       //관리기관
                cbUPPER_FTR_CDE = blk02AddView.cbUPPER_FTR_CDE;       //상위블록
                cbFTR_CDE = blk02AddView.cbFTR_CDE;       //지형지물
                cbUPPER_FTR_IDN = blk02AddView.cbUPPER_FTR_IDN;       

                btnBack = blk02AddView.btnBack;
                btnSave = blk02AddView.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번 - Merge문 사용해서 채번안함


                //채번결과 매칭
                //this.FTR_IDN = result.FTR_IDN;
                Dtl.FTR_CDE = "BZ002"; //중블록

                //공통팝업창 사이즈 변경 4
                FmsUtil.popWinView.Height = 280;
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
            if (!BizUtil.ValidReq(blk02AddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "updateBlk02Dtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
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
                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "선택");

                // cbFTR_CDE 지형지물
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM");

                // cbUPPER_FTR_CDE 상위블록코드
                Func<DataRow, bool> filter = Row => (Row.Field<string>("FTR_CDE").Contains("BZ001"));//대블록
                BizUtil.SetCombo(cbUPPER_FTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", null, filter);

                // cbUPPER_FTR_IDN 상위블록
                BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);
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
