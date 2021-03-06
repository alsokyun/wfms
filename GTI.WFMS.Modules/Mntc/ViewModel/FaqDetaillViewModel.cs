﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Core;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{

    /// <summary>
    /// RichEditor 버전
    /// </summary>
    public class FaqDetaillViewModel : FaqDtl
    {



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        FaqDetaillView faqDetaillView;
        ComboBoxEdit cbFTR_CDE;
        ComboBoxEdit cbFAQ_CAT_CDE; 
        ComboBoxEdit cbFAQ_CUZ_CDE; 
        
        Button btnBack;
        Button btnDelete;
        Button btnSave;
        RichEditControl richQUESTION;

        #endregion




        /// 생성자
        public FaqDetaillViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
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
            // 0.화면객체인스턴스화
            if (obj == null) return;

            faqDetaillView = obj as FaqDetaillView;
            cbFTR_CDE = faqDetaillView.cbFTR_CDE;
            cbFAQ_CAT_CDE = faqDetaillView.cbFAQ_CAT_CDE;
            cbFAQ_CUZ_CDE= faqDetaillView.cbFAQ_CUZ_CDE;
            btnBack = faqDetaillView.btnBack;
            btnDelete = faqDetaillView.btnDelete;
            btnSave = faqDetaillView.btnSave;
            richQUESTION = faqDetaillView.richQUESTION;

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            InitModel();

        }






        /// <summary>
        /// 초기조회
        /// </summary>
        private void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFaqDtl");
            param.Add("SEQ", this.SEQ);

            FaqDtl result = new FaqDtl();
            result = BizUtil.SelectObject(param) as FaqDtl;



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
                        try { prop.SetValue(this, colValue); } catch (Exception) { }
                    }
                }
                //Console.WriteLine(propName + " - " + prop.GetValue(this,null));
            }


            // CLOB 데이터는 OleDbConnection으로 따로 조회
            try
            {
                string sql = "";
                sql += " SELECT QUESTION, REPL FROM FAQ  WHERE SEQ = :SEQ ;";

                param = new Hashtable();
                param.Add("sql", sql);
                param.Add("SEQ", this.SEQ);

                DataTable dt = DBUtil.Select(param);

                this.QUESTION = dt.Rows[0]["QUESTION"].ToString();
                this.REPL = dt.Rows[0]["REPL"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(faqDetaillView)) return;

            

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //this.QUESTION = richQUESTION.RtfText;
                //BizUtil.Update2(this, "SaveFaqDtl");


                string sql = "";
                sql += " UPDATE FAQ SET ";
                sql += " QUESTION = :QUESTION ";
                sql += " ,REPL = :REPL ";
                sql += " ,TTL = :TTL ";
                sql += " ,FAQ_CAT_CDE= :FAQ_CAT_CDE";
                sql += " ,FAQ_CUZ_CDE= :FAQ_CUZ_CDE";
                sql += " ,FTR_CDE = :FTR_CDE";
                sql += " ,EDT_ID = :EDT_ID";
                sql += " WHERE SEQ = :SEQ ;";

                Hashtable param = new Hashtable();
                param.Add("sql", sql);
                param.Add("QUESTION", this.QUESTION);
                param.Add("REPL", this.REPL);
                param.Add("TTL", this.TTL);
                param.Add("FAQ_CAT_CDE", this.FAQ_CAT_CDE);
                param.Add("EDT_ID", Logs.strLogin_ID);
                param.Add("FTR_CDE", this.FTR_CDE);
                param.Add("FAQ_CUZ_CDE", this.FAQ_CUZ_CDE);
                param.Add("SEQ", this.SEQ);
                DBUtil.UpdateFAQ(param);


            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                InitModel();
                return;
            }

            Messages.ShowOkMsgBox();
            InitModel();
        }

        /// <summary>
        /// 삭제처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {

            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("FAQ 항목을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(this, "DeleteFaqDtl");
            }
            catch (Exception)
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
                // cbFTR_CDE 시설물
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", "선택");
                // cbFAQ_CAT_CDE 구분
                BizUtil.SetCmbCode(cbFAQ_CAT_CDE, "250109", "선택");
                BizUtil.SetCmbCode(cbFAQ_CUZ_CDE, "250110", "선택");
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
