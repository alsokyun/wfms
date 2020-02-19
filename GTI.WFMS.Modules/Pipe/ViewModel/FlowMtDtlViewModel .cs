using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.Report;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class FlowMtDtlViewModel : FlowMtDtl
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> PrintCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        FlowMtDtlView flowMtDtlView;
      
        //ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();	//지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();	//행정동
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();		//관리기관
        ComboBoxEdit cbGAG_CDE; DataTable dtGAG_CDE = new DataTable();      //유량계종류
        ComboBoxEdit cbMOF_CDE; DataTable dtMOF_CDE = new DataTable();      //형식

        Button btnBack;
        Button btnDelete;
        Button btnSave;
        
        #endregion




        /// 생성자
        public FlowMtDtlViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.PrintCommand = new DelegateCommand<object>(OnPrint);
            this.DeleteCommand = new DelegateCommand<object>(OnDelete);
            this.BackCommand = new DelegateCommand<object>(OnBack);
            
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
                var values = (object[])obj;

                flowMtDtlView = values[0] as FlowMtDtlView;
                //cbFTR_CDE = flowMtDtlView.cbFTR_CDE;     //지형지물
                cbHJD_CDE = flowMtDtlView.cbHJD_CDE;     //행정동
                cbMNG_CDE = flowMtDtlView.cbMNG_CDE;       //관리기관
                cbGAG_CDE = flowMtDtlView.cbGAG_CDE;       //유량계종류
                cbMOF_CDE = flowMtDtlView.cbMOF_CDE;       //형식

                btnBack = flowMtDtlView.btnBack;
                btnDelete = flowMtDtlView.btnDelete;
                btnSave = flowMtDtlView.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();
              
                // 4.초기조회
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectFlowMtDtl");
                param.Add("FTR_CDE", this.FTR_CDE);
                param.Add("FTR_IDN", this.FTR_IDN);

                FlowMtDtl result = new FlowMtDtl();
                result = BizUtil.SelectObject(param) as FlowMtDtl;
                               
                //결과를 뷰모델멤버로 매칭
                Type dbmodel = result.GetType();
                Type model = this.GetType();

                //모델프로퍼티 순회
                foreach (PropertyInfo prop in model.GetProperties())
                {
                    string propName = prop.Name;
                    //db프로퍼티 순회
                    foreach (PropertyInfo dbprop in dbmodel.GetProperties())
                    {
                        string colName = dbprop.Name;
                        var colValue = dbprop.GetValue(result, null);
                        if (colName.Equals(propName))
                        {
                            prop.SetValue(this, Convert.ChangeType(colValue, prop.PropertyType));
                        }
                    }
                    Console.WriteLine(propName + " - " + prop.GetValue(this, null));
                }
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
            if (!BizUtil.ValidReq(flowMtDtlView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "updateFlowMtDtl");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }
            Messages.ShowOkMsgBox();

        }

        /// <summary>
        /// 인쇄작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnPrint(object obj)
        {
            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            //if (!BizUtil.ValidReq(flowMtDtlView)) return;

            //if (Messages.ShowYesNoMsgBox("인쇄하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //0.Datasource 생성
                FlowMtDtlViewMdl mdl = new FlowMtDtlViewMdl(this.FTR_CDE, this.FTR_IDN);
                //1.Report 호출
                FlowMtReport report = new FlowMtReport();
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
            param.Add("sqlId" , "selectChscResSubList");
            param.Add("sqlId2", "selectFileMapList");

            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);
            param.Add("BIZ_ID", string.Concat(this.FTR_CDE , this.FTR_IDN) );

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt  = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                dt = result["dt"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("유지보수내역이 존재합니다.");
                    return;
                }
            }
            catch (Exception) { }
            try
            {
                dt2 = result["dt2"] as DataTable;
                if (dt2.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("파일첨부내역이 존재합니다.");
                    return;
                }
            }
            catch (Exception) { }



            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("변로를 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(this, "deleteFlowMtDtl");
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
                // cbFTR_CDE 지형지물
                //BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", false);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbGAG_CDE 유량계종류
                BizUtil.SetCmbCode(cbGAG_CDE, "GAG_CDE", true);

                // cbMOF_CDE 형식
                BizUtil.SetCmbCode(cbMOF_CDE, "MOF_CDE", true, "250035");

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
