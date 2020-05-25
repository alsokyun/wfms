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
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    class SplyDtlViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> BackCommand { get; set; }
        public DelegateCommand<object> DupCommand { get; set; }
        public DelegateCommand<object> PrintCommand { get; set; }

        private SplyDtl dtl = new SplyDtl();
        public SplyDtl Dtl
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
        SplyDtlView splyDtlView;
        ComboBoxEdit cbHJD_CDE; 
        Button btnBack;
        Button btnDelete;
        Button btnSave;
        Button btnDup;
        
        #endregion




        /// 생성자
        public SplyDtlViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DeleteCommand = new DelegateCommand<object>(OnDelete);
            this.BackCommand = new DelegateCommand<object>(OnBack);
            this.PrintCommand = new DelegateCommand<object>(OnPrint);


            //입력항목 변경되면 중복버튼 복원
            Dtl.DUP = "체크";
            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args) {
                try
                {
                    btnDup.Content = "체크";
                }
                catch (Exception) { }
            };

            this.DupCommand = new DelegateCommand<object>(delegate(object obj) {
                if (btnDup.Content.Equals("OK")) return;


                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWttSplyMaDup");
                param.Add("CNT_NUM", Dtl.CNT_NUM);
                DataTable dt = BizUtil.SelectList(param);
                if (dt.Rows.Count > 1)
                {
                    Messages.ShowInfoMsgBox("공사번호가 중복되었습니다.");
                }
                else
                {
                    btnDup.Content = "OK";
                }
            });
            
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
            var values = (object[])obj;

            splyDtlView = values[0] as SplyDtlView;
            cbHJD_CDE = splyDtlView.cbHJD_CDE;
            btnBack = splyDtlView.btnBack;
            btnDelete = splyDtlView.btnDelete;
            btnSave = splyDtlView.btnSave;
            btnDup = splyDtlView.btnDup;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            //DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttSplyMaDtl");
            param.Add("CNT_NUM", Dtl.CNT_NUM);

            Dtl = BizUtil.SelectObject(param) as SplyDtl;

            ////결과를 뷰모델멤버로 매칭
            //Type dbmodel = result.GetType();
            //Type model = this.GetType();

            ////모델프로퍼티 순회
            //foreach (PropertyInfo prop in model.GetProperties())
            //{
            //    string propName = prop.Name;
            //    //db프로퍼티 순회
            //    foreach (PropertyInfo dbprop in dbmodel.GetProperties())
            //    {
            //        string colName = dbprop.Name;
            //        var colValue = dbprop.GetValue(result, null);
            //        if (colName.Equals(propName))
            //        {
            //            try { prop.SetValue(this, colValue); } catch (Exception) { }
            //        }
            //    }
            //   //Console.WriteLine(propName + " - " + prop.GetValue(this,null));
            //}

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if(!BizUtil.ValidReq(splyDtlView))  return;

            // 공사번호중복체크
            //if (btnDup.Content.Equals("체크"))
            //{
            //    Messages.ShowInfoMsgBox("공사번호 (중복)체크를 하세요.");
            //    return;
            //}
            



            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "updateSplyDtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                return;
            }
            Messages.ShowOkMsgBox();

        }

        /// <summary>
        /// 삭제처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {
            //0.삭제전 체크
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileMapList");
            param.Add("sqlId2", "SelectFileMapList");
            param.Add("CNT_NUM", Dtl.CNT_NUM);

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                dt = result["dt"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowErrMsgBox("사진첨부내역이 존재합니다.");
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
            if (Messages.ShowYesNoMsgBox("급수전대장을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(Dtl, "deleteSplyDtl");
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
                SplyDtlViewMdl mdl = new SplyDtlViewMdl(Dtl.CNT_NUM);
                //1.Report 호출

                SplyMngReport report = new SplyMngReport();
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
        #endregion





        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
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
