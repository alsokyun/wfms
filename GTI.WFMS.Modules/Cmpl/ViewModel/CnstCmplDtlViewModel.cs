﻿using DevExpress.Mvvm;
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


    public class CnstCmplDtlViewModel : INotifyPropertyChanged 
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
        

        public WserDtl Dtl
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
        CnstCmplDtlView cnstCmplDtlView;
        
        Button btnSave;
        Button btnClose;

        private WserDtl dtl = new WserDtl(); //민원마스터
        private DataTable dt = new DataTable(); //민원누수지점내역

        string _WSER_SEQ;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public CnstCmplDtlViewModel()
        {

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                cnstCmplDtlView = obj as CnstCmplDtlView;

                btnSave = cnstCmplDtlView.btnSave;
                btnClose = cnstCmplDtlView.btnClose;

                _WSER_SEQ = cnstCmplDtlView.txtWSER_SEQ.Text;

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
                if (!BizUtil.ValidReq(cnstCmplDtlView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트는 따로 처리
                    this.Dtl.APL_EXP = new TextRange(cnstCmplDtlView.richAPL_EXP.Document.ContentStart, cnstCmplDtlView.richAPL_EXP.Document.ContentEnd).Text.Trim();
                    this.Dtl.PRO_EXP = new TextRange(cnstCmplDtlView.richPRO_EXP.Document.ContentStart, cnstCmplDtlView.richPRO_EXP.Document.ContentEnd).Text.Trim();
                    BizUtil.Update2(this.Dtl, "SaveCmplWserMa");
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

                if (dt.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("해당민원 누수지점 내역이 존재합니다.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("민원을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    BizUtil.Update2(this.Dtl, "DeleteWserMa");
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
            param.Add("sqlId", "SelectWttWserMa");
            param.Add("WSER_SEQ", _WSER_SEQ);

            WserDtl result = new WserDtl();
            result = BizUtil.SelectObject(param) as WserDtl;
            this.Dtl = result;

            //다큐먼트는 따로 처리
            Paragraph p = new Paragraph();
            try
            {
                p.Inlines.Add(this.Dtl.APL_EXP ?? "");
                cnstCmplDtlView.richAPL_EXP.Document.Blocks.Clear();
                cnstCmplDtlView.richAPL_EXP.Document.Blocks.Add(p);
            }
            catch (Exception){}

            p = new Paragraph();
            try
            {
                p.Inlines.Add(this.Dtl.PRO_EXP.Trim());
                cnstCmplDtlView.richPRO_EXP.Document.Blocks.Clear();
                cnstCmplDtlView.richPRO_EXP.Document.Blocks.Add(p);
            }
            catch (Exception){}


            //2.누수지점
            param = new Hashtable();
            param.Add("sqlId", "SelectCmplLeakList");
            param.Add("RCV_NUM", this.Dtl.RCV_NUM);

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
                BizUtil.SetCombo(cnstCmplDtlView.cbAPL_HJD, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
                //민원구분
                BizUtil.SetCmbCode(cnstCmplDtlView.cbAPL_CDE, "250056", "선택");
                //민원처리상태
                BizUtil.SetCmbCode(cnstCmplDtlView.cbPRO_CDE, "250050", "선택");
                
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
