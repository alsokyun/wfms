using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.Report;
using GTI.WFMS.Modules.Cnst.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    public class CnstMngDtlViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> PrintCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }

        private CnstDtl dtl = new CnstDtl();
        public CnstDtl Dtl
        {
            get { return dtl; }
            set
            {
                dtl = value;
                OnPropertyChanged("Dtl");
            }
        }
        public List<WttCostDt> Tab01List { get; set; }
        public List<WttChngDt> Tab02List { get; set; }
        public List<WttSubcDt> Tab03List { get; set; }
        public List<WttFlawDt> Tab04List { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        CnstMngDtlView cnstMngDtlView;
        ComboBoxEdit cbCTT_CDE; DataTable dtCTT_CDE = new DataTable();
        ComboBoxEdit cbCNT_CDE; DataTable dtCNT_CDE = new DataTable();
        Button btnBack;
        Button btnDelete;
        Button btnSave;
        
        #endregion                    

        /// 생성자
        public CnstMngDtlViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SaveCommand = new DelegateCommand<object>(OnSave);
            PrintCommand = new DelegateCommand<object>(OnPrint);
            DeleteCommand = new DelegateCommand<object>(OnDelete);
            BackCommand = new DelegateCommand<object>(OnBack);
            
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

            cnstMngDtlView = obj as CnstMngDtlView;
            cbCTT_CDE = cnstMngDtlView.cbCTT_CDE;
            cbCNT_CDE = cnstMngDtlView.cbCNT_CDE;
            btnBack = cnstMngDtlView.btnBack;
            btnDelete = cnstMngDtlView.btnDelete;
            btnSave = cnstMngDtlView.btnSave;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            InitModel();
        }


        // 초기조회
        public void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttConsMaDtl");
            param.Add("CNT_NUM", Dtl.CNT_NUM);

            Dtl = BizUtil.SelectObject(param) as CnstDtl;
        }


        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(cnstMngDtlView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "updateCnstMngDtl");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }

            Messages.ShowOkMsgBox();
            InitModel();
        }

        /// <summary>
        /// 인쇄작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnPrint(object obj)
        {
            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            //if (!BizUtil.ValidReq(fireFacDtlView)) return;

            //if (Messages.ShowYesNoMsgBox("인쇄하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {

                //0.Datasource 생성
                CnstMngDtlViewMdl mdl = new CnstMngDtlViewMdl(Dtl.CNT_NUM);
                //1.Report 호출
                
                CnstMngReport report = new CnstMngReport();
                ObjectDataSource ods = new ObjectDataSource();
                ods.Name = "objectDataSource1";
                ods.DataSource = mdl;

                report.DataSource = ods;
                report.ShowPreviewDialog();
                

            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("인쇄 처리중 오류가 발생하였습니다.");
                return;
            }
        }

        /// <summary>
        /// 삭제처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {
            //0.삭제전 체크
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttCostDtList");//공사비지급내역
            param.Add("sqlId2", "SelectWttChngDtList");//설계변경내역
            param.Add("sqlId3", "SelectWttSubcDtList");//공사하도급내역
            param.Add("sqlId4", "SelectWttFlawDtList");//하자보수목록
            param.Add("sqlId5", "SelectFileMapList");//첨부파일사진목록
            param.Add("CNT_NUM", Dtl.CNT_NUM);
            param.Add("BIZ_ID", Dtl.CNT_NUM);

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();

            /*
            try
            {
                dt = result["dt"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("공사비지급내역이 존재합니다.");
                    //return;
                }
            }
            catch (Exception) { }
            try
            {
                dt2 = result["dt2"] as DataTable;
                if (dt2.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("설계변경내역이 존재합니다.");
                    //return;
                }
            }
            catch (Exception) { }
            try
            {
                dt3 = result["dt3"] as DataTable;
                if (dt3.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("공사하도급내역이 존재합니다.");
                    //return;
                }
            }
            catch (Exception) { }
            try
            {
                dt4 = result["dt4"] as DataTable;
                if (dt3.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("하자보수내역이 존재합니다.");
                    //return;
                }
            }
            catch (Exception) { }
            try
            {
                dt5 = result["dt5"] as DataTable;
                if (dt5.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("첨부파일/사진 내역이 존재합니다.");
                    //return;
                }
            }
            catch (Exception) { }
            */


            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("공사대장을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                //1.공사비지급내역 삭제
                param = new Hashtable();
                param.Add("sqlId", "DeleteWttCostDt");
                param.Add("CNT_NUM", Dtl.CNT_NUM);
                BizUtil.Update(param);

                //2.설계변경내역 삭제
                param = new Hashtable();
                param.Add("sqlId", "DeleteWttChngDt");
                param.Add("CNT_NUM", Dtl.CNT_NUM);
                BizUtil.Update(param);

                //3.공사하도급내역 삭제
                param = new Hashtable();
                param.Add("sqlId", "DeleteWttSubcDt");
                param.Add("CNT_NUM", Dtl.CNT_NUM);
                BizUtil.Update(param);

                //4.하자보수내역 삭제
                param = new Hashtable();
                param.Add("sqlId", "DeleteWttFlawDt");
                param.Add("CNT_NUM", Dtl.CNT_NUM);
                BizUtil.Update(param);

                //5.첨부파일,사진삭제
                foreach (DataRow row in dt5.Rows)
                {
                    //a.FIL_SEQ 첨부파일삭제
                    BizUtil.DelFileSeq(row["FIL_SEQ"]);

                    //b.FILE_MAP 업무파일매핑삭제
                    param = new Hashtable();
                    param.Add("sqlId", "DeleteFileMap");
                    param.Add("BIZ_ID", Dtl.CNT_NUM);
                    param.Add("FIL_SEQ", row["FIL_SEQ"]);
                    BizUtil.Update(param);
                }


                //마스터삭제
                BizUtil.Update2(Dtl, "deleteCnstMngDtl");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
                return;
            }
            Messages.ShowOkMsgBox();



            BackCommand.Execute(null);

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
                // cbCTT_CDE 계약구분
                BizUtil.SetCmbCode(cbCTT_CDE, "250038", "선택");

                // cbCNT_CDE 공사구분
                BizUtil.SetCmbCode(cbCNT_CDE, "250039", "선택");
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
                        btnDelete.Visibility = Visibility.Collapsed;
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
