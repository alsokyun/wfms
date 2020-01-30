using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduling;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fctl.Model;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{
    public class ChkSchListViewModel 
    {



        #region ==========  Properties 정의 ==========

        public virtual ObservableCollection<ResSrc> ResSrcs { get; set; } //리소스소스:스케줄의 루트요소 - 점검일정 스케줄이 유일함
        public virtual ObservableCollection<ChscMaDtl> ChscMaLst { get; set; } //스케줄 목록

        private ObservableCollection<ChscMaDtl> selectedAppointment;
        public virtual ObservableCollection<ChscMaDtl> SelectedAppointment
        {
            get {return selectedAppointment;}
            set { selectedAppointment = value; }
        }

        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SearchCmd { get; set; }
        public DelegateCommand<object> ChscResCmd { get; set; }
        public DelegateCommand<object> BackCmd { get; set; }
        

        #endregion


        #region ==========  Member 정의 ==========
        ChkSchListView chkSchListView;
        ChkSchDtlView chkSchDtlView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbSCL_CDE; DataTable dtSCL_CDE = new DataTable();
        ComboBoxEdit cbSCL_STAT_CDE; DataTable dtSCL_STAT_CDE = new DataTable();
        TextEdit txtTIT_NAM;
        TextEdit txtCKM_PEO;

        List<ChscMaDtl> newChscMaLst = new List<ChscMaDtl>(); //추가된 일정아이템
        List<ChscMaDtl> oldChscMaLst = new List<ChscMaDtl>(); //삭제된 일정아이템
        ChscMaDtl selChscMaDtl; //선택된 일정
        #endregion



        #region ========== 생성자 ==========
        /// 생성자
        public ChkSchListViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);

            this.SearchCmd = new DelegateCommand<object>(delegate (object obj) {
                //재조회
                InitModel();
            });
            this.BackCmd = new DelegateCommand<object>(delegate(object obj) {
                //달력 MonthView로 돌리기
                chkSchListView.scheduler.ActiveViewIndex = 3; //MonthView
                cbMNG_CDE.SelectedIndex = 0;
                cbSCL_CDE.SelectedIndex = 0;
                cbSCL_STAT_CDE.SelectedIndex = 0;
                txtTIT_NAM.Text = "";
                txtCKM_PEO.Text = "";
                //재조회
                InitModel();
            });          
            this.ChscResCmd = new DelegateCommand<object>(delegate(object obj) {
                /*
                 * 점검결과등록 팝업호출
                 */

                //0.일정선택체크
                if(selChscMaDtl is null)
                {
                    Messages.ShowInfoMsgBox("점검일정을 선택하세요");
                    return;
                }

                //1.점검일정팝업호출
                // 점검달력윈도우
                chkSchDtlView = new ChkSchDtlView(selChscMaDtl.SCL_NUM.ToString());
                if (chkSchDtlView.ShowDialog() is bool)
                {
                    //재조회
                    InitModel();
                }
            });

            

            //선택된일정리스트에대한 변경이벤트 설정
            selectedAppointment = new ObservableCollection<ChscMaDtl>();
            selectedAppointment.CollectionChanged += OnSelectedAppointmentChanged;
        }


        #endregion




        #region ==========  이벤트 핸들러 ==========

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            // 0.화면객체인스턴스화
            if (obj == null) return;

            chkSchListView = obj as ChkSchListView;
            cbMNG_CDE = chkSchListView.cbMNG_CDE;
            cbSCL_CDE = chkSchListView.cbSCL_CDE;
            cbSCL_STAT_CDE = chkSchListView.cbSCL_STAT_CDE;
            txtTIT_NAM = chkSchListView.txtTIT_NAM;
            txtCKM_PEO = chkSchListView.txtCKM_PEO;
            
            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            InitModel();

        }


        // 데이터 추가,삭제 이벤트 처리
        private void Lst_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                newChscMaLst.Clear();
                foreach (ChscMaDtl item in e.NewItems)
                {
                    item.PropertyChanged += ChscMaDtl_PropertyChanged;
                    newChscMaLst.Add(item);
                }
            }

            if (e.OldItems != null)
            {
                oldChscMaLst.Clear();
                foreach (ChscMaDtl item in e.OldItems)
                {
                    item.PropertyChanged -= ChscMaDtl_PropertyChanged;
                    oldChscMaLst.Add(item);
                }
            }

            //변경목록 저장
            SaveSchdList();
        }

        /// <summary>
        /// 추가된 데이터의 항목별 이벤트 변경처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChscMaDtl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "STA_YMD" || e.PropertyName == "END_YMD" || e.PropertyName == "TIT_NAM" || e.PropertyName == "CHK_CTNT"
                 || e.PropertyName == "SCL_CDE" || e.PropertyName == "SCL_STAT_CDE")
            {
                //변경저장
                ChscMaDtl dtl = sender as ChscMaDtl;
                SaveSchdDtl(dtl);
            }
        }


        //일정선택 이벤트처리
        private void OnSelectedAppointmentChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var items = sender as ObservableCollection<ChscMaDtl>;
            if (items?.Count > 0)
            {
                selChscMaDtl = items[0];
            }
            else
            {
                selChscMaDtl = null;
            }
        }



        #endregion





        #region ============= 메소드정의 ================



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



        // 초기조회 및 바인딩
        private void InitDataBinding()
        {
            try
            {
                // cbMNG_CDE
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);
                // cbSCL_CDE
                BizUtil.SetCmbCode(cbSCL_CDE, "SCL_CDE", true);
                // cbSCL_STAT_CDE
                BizUtil.SetCmbCode(cbSCL_STAT_CDE, "SCL_STAT_CDE", true);
                
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox(e.ToString());
            }
        }


        // 초기조회
        private void InitModel()
        {
            try
            {
                ChscMaLst.Clear();
            }
            catch (Exception) { }

            //리소스소스
            ResSrcs = new ObservableCollection<ResSrc>();
            ResSrcs.Add(ResSrc.Create(Id: 1, Name: "점검일정"));


            //점검스케줄
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectChscMaList");
            param.Add("MNG_CDE", cbMNG_CDE.EditValue.ToString().Trim());
            param.Add("SCL_CDE", cbSCL_CDE.EditValue.ToString().Trim());
            param.Add("SCL_STAT_CDE", cbSCL_STAT_CDE.EditValue.ToString().Trim());
            param.Add("TIT_NAM", txtTIT_NAM.Text.Trim());
            param.Add("CKM_PEO", txtCKM_PEO.Text.Trim());
            
            ChscMaLst = new ObservableCollection<ChscMaDtl>(BizUtil.SelectListObj<ChscMaDtl>(param));
            //기존목록 이벤트핸들러 등록
            /*
             */
            foreach (ChscMaDtl item in ChscMaLst)
            {
                item.PropertyChanged += ChscMaDtl_PropertyChanged;
            }

            //목록변경 이벤트핸들러 등록
            ChscMaLst.CollectionChanged += Lst_CollectionChanged;
        }



        // 점검일정 목록저장
        private void SaveSchdList()
        {
            //1.일정변경(추가)
            foreach (ChscMaDtl dtl in newChscMaLst)
            {
                BizUtil.Update2(dtl, "SaveChscMaDtl");
            }

            //2.일정변경(삭제)
            foreach (ChscMaDtl dtl in oldChscMaLst)
            {
                BizUtil.Update2(dtl, "DeleteChscMaDtl");
            }

            //재조회
            //InitModel();
        }


        // 점검일정 단건저장
        private void SaveSchdDtl(ChscMaDtl dtl)
        {
            //일정변경
            BizUtil.Update2(dtl, "SaveChscMaDtl");


            //재조회
            //InitModel();
        }

        #endregion


    }
}
