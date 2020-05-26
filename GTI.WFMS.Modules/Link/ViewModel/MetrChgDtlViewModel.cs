using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Acmf.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Link.ViewModel
{
    public class MetrChgDtlViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> DeleteCommand { get; set; }
        public WttMetaHt Dtl
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
        private WttMetaHt dtl = new WttMetaHt();

        MetrChgDtlView metrChgDtlView;
        ComboBoxEdit cbGCW_CDE; 
        ComboBoxEdit cbOME_MOF; 
        Button btnSave;
        Button btnDel;
        Button btnClose;
        #endregion




        /// 생성자
        public MetrChgDtlViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DeleteCommand = new DelegateCommand<object>(OnDelete);

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

            metrChgDtlView = obj as MetrChgDtlView;
            cbGCW_CDE = metrChgDtlView.cbGCW_CDE;
            cbOME_MOF = metrChgDtlView.cbOME_MOF;
            
            btnSave = metrChgDtlView.btnSave;
            btnDel = metrChgDtlView.btnDel;
            btnClose = metrChgDtlView.btnClose;


            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            //4.신규/상세 모드
            if (Dtl.META_SEQ < 0)
            {
                //채번
                Hashtable param = new Hashtable();
                param.Add("FTR_CDE", Dtl.FTR_CDE);
                param.Add("FTR_IDN", Dtl.FTR_IDN);
                param.Add("sqlId", "selectMETA_SEQ");

                WttMetaHt res = new WttMetaHt();
                res = BizUtil.SelectObject(param) as WttMetaHt;

                //채번결과 매칭
                Dtl.META_SEQ = res.META_SEQ;

                metrChgDtlView.btnDel.Visibility = Visibility.Collapsed;

                //시설물명 가져오기
                Dtl.FTR_NAM = BizUtil.GetCodeNm("Select_FTR_LIST2", Dtl.FTR_CDE);
            }
            else
            {
                Hashtable param = new Hashtable();
                param.Add("sqlId", "selectWttMetaHtList");
                param.Add("FTR_CDE", Dtl.FTR_CDE);
                param.Add("FTR_IDN", Dtl.FTR_IDN);
                param.Add("META_SEQ", Dtl.META_SEQ);

                Dtl = BizUtil.SelectObject(param) as WttMetaHt;

                metrChgDtlView.btnDel.Visibility = Visibility.Visible;
            }


        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if(!BizUtil.ValidReq(metrChgDtlView))  return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "SaveWttMetaHt");
            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }


            Messages.ShowOkMsgBox();
            //화면닫기
            btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }


        /// <summary>
        /// 삭제작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {

            if (Messages.ShowYesNoMsgBox("삭제하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "DeleteWttMetaHt");
            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }


            Messages.ShowOkMsgBox();
            //화면닫기
            btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
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
                BizUtil.SetCmbCode(cbGCW_CDE, "250041", "선택");

                BizUtil.SetCmbCode(cbOME_MOF, "250004", "선택");
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
                        //btnSave.Visibility = Visibility.Collapsed;
                        //btnDel.Visibility = Visibility.Collapsed;
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
