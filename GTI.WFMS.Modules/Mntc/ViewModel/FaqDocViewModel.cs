using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{
    public class FaqDocViewModel : FaqDtl
    {



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        FaqDocView faqDocView;
        ComboBoxEdit cbFTR_CDE;
        ComboBoxEdit cbFAQ_CAT_CDE; 
        ComboBoxEdit cbFAQ_CUZ_CDE; 
        
        Button btnBack;

        #endregion




        /// 생성자
        public FaqDocViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
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

            faqDocView = obj as FaqDocView;
            cbFTR_CDE = faqDocView.cbFTR_CDE;
            cbFAQ_CAT_CDE = faqDocView.cbFAQ_CAT_CDE;
            cbFAQ_CUZ_CDE= faqDocView.cbFAQ_CUZ_CDE;
            btnBack = faqDocView.btnBack;

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


            // 다큐먼트는 따로 조회
            Paragraph p = new Paragraph();
            try
            {
                p.Inlines.Add(this.QUESTION ?? "");
                faqDocView.richQUESTION.Document.Blocks.Clear();
                faqDocView.richQUESTION.Document.Blocks.Add(p);
            }
            catch (Exception) { }

            p = new Paragraph();
            try
            {
                p.Inlines.Add(this.REPL ?? "");
                faqDocView.richREPL.Document.Blocks.Clear();
                faqDocView.richREPL.Document.Blocks.Add(p);
            }
            catch (Exception) { }

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
