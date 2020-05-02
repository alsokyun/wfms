using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpf.Editors;
using DevExpress.XtraReports.UI;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class ValvFacDtlViewModel : ValvFacDtl
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }

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
        ValvFacDtlView valvFacDtlView;
      
        ComboBoxEdit cbMNG_CDE; //관리기관
        ComboBoxEdit cbVAL_MOF; //형식
        ComboBoxEdit cbVAL_MOP; //관재질
        ComboBoxEdit cbSAE_CDE; //제수변회전방향
        ComboBoxEdit cbMTH_CDE; //제수변구동방법
        ComboBoxEdit cbVAL_FOR; //시설물형태
        ComboBoxEdit cbCST_CDE; //이상상태
        ComboBoxEdit cbOFF_CDE; //개폐여부
        ComboBoxEdit cbHJD_CDE; //행정동

        Button btnBack;
        Button btnDelete;
        Button btnSave;
        
        #endregion




        /// 생성자
        public ValvFacDtlViewModel()
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

                valvFacDtlView = values[0] as ValvFacDtlView;
                //cbFTR_CDE = valvFacDtlView.cbFTR_CDE;   //지형지물
                cbHJD_CDE = valvFacDtlView.cbHJD_CDE;   //행정동
                cbMNG_CDE = valvFacDtlView.cbMNG_CDE;   //관리기관
                cbVAL_MOF = valvFacDtlView.cbVAL_MOF;   //형식
                cbVAL_MOP = valvFacDtlView.cbVAL_MOP;   //관재질
                cbSAE_CDE = valvFacDtlView.cbSAE_CDE;   //제수변회전방향
                cbMTH_CDE = valvFacDtlView.cbMTH_CDE;   //제수변구동방법
                cbVAL_FOR = valvFacDtlView.cbVAL_FOR;   //시설물형태
                cbCST_CDE = valvFacDtlView.cbCST_CDE;   //이상상태
                cbOFF_CDE = valvFacDtlView.cbOFF_CDE;	//개폐여부

                btnBack = valvFacDtlView.btnBack;
                btnDelete = valvFacDtlView.btnDelete;
                btnSave = valvFacDtlView.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();
              
                // 4.초기조회
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectValvFacDtl");
                param.Add("FTR_CDE", this.FTR_CDE);
                param.Add("FTR_IDN", this.FTR_IDN);

                ValvFacDtl result = new ValvFacDtl();
                result = BizUtil.SelectObject(param) as ValvFacDtl;
                               
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
            if (!BizUtil.ValidReq(valvFacDtlView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "updateValvFacDtl");
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
            //if (!BizUtil.ValidReq(valvFacDtlView)) return;

            //if (Messages.ShowYesNoMsgBox("인쇄하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //0.Datasource 생성
                ValvFacDtlViewMdl mdl = new ValvFacDtlViewMdl(this.FTR_CDE, this.FTR_IDN);
                //1.Report 호출

                ValvFacReport report = new ValvFacReport();
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
            param.Add("sqlId2", "SelectFileMapList");

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
            if (Messages.ShowYesNoMsgBox("변류시설를 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(this, "deleteValvFacDtl");
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
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "[선택하세요]");

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "[선택하세요]");


                // cbVAL_MOF 형식
                BizUtil.SetCmbCode(cbVAL_MOF, "250016", "[선택하세요]");

                // cbVAL_MOP 관재질
                BizUtil.SetCmbCode(cbVAL_MOP, "250015", "[선택하세요]");

                // cbSAE_CDE    제수변회전방향
                BizUtil.SetCmbCode(cbSAE_CDE, "250027", "[선택하세요]");

                // cbMTH_CDE    제수변구동방법
                BizUtil.SetCmbCode(cbMTH_CDE, "250065", "[선택하세요]");

                // cbVAL_FOR    시설물형태(=구조물형태)
                BizUtil.SetCmbCode(cbVAL_FOR, "250007", "[선택하세요]");

                // cbCST_CDE    이상상태
                BizUtil.SetCmbCode(cbCST_CDE, "250062", "[선택하세요]");

                // cbOFF_CDE    개폐여부
                BizUtil.SetCmbCode(cbOFF_CDE, "250036", "[선택하세요]");

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
