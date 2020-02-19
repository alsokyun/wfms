using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.View;
using GTI.WFMS.Modules.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{
    public class FaqDtlViewModel : FaqDtl
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
        FaqDtlView faqDtlView;
        ComboBoxEdit cbFTR_CDE;
        ComboBoxEdit cbFAQ_CAT_CDE; 
        ComboBoxEdit cbFAQ_CUZ_CDE; 
        
        Button btnBack;
        Button btnDelete;
        Button btnSave;
        RichEditControl richQUESTION;

        #endregion




        /// 생성자
        public FaqDtlViewModel()
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

            faqDtlView = obj as FaqDtlView;
            cbFTR_CDE = faqDtlView.cbFTR_CDE;
            cbFAQ_CAT_CDE = faqDtlView.cbFAQ_CAT_CDE;
            cbFAQ_CUZ_CDE= faqDtlView.cbFAQ_CUZ_CDE;
            btnBack = faqDtlView.btnBack;
            btnDelete = faqDtlView.btnDelete;
            btnSave = faqDtlView.btnSave;
            richQUESTION = faqDtlView.richQUESTION;

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
                        prop.SetValue(this, Convert.ChangeType(colValue, prop.PropertyType));
                    }
                }
                //Console.WriteLine(propName + " - " + prop.GetValue(this,null));
            }
        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(faqDtlView)) return;

            

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //this.QUESTION = richQUESTION.RtfText;
                BizUtil.Update2(this, "SaveFaqDtl");
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
                BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", true);
                // cbFAQ_CAT_CDE 구분
                BizUtil.SetCmbCode(cbFAQ_CAT_CDE, "FAQ_CAT_CDE", true, "250109");
                BizUtil.SetCmbCode(cbFAQ_CUZ_CDE, "FAQ_CUZ_CDE", true, "250110");
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
