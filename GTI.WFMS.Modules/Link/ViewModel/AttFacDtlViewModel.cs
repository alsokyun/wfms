using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fctl.Model;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GTI.WFMS.Modules.Link.ViewModel
{
    public class AttFacDtlViewModel : WttAttaDt
    {



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        AttFacDtlView attFacDtlView;
        ComboBoxEdit cbCRE_YY; DataTable dtCRE_YY;
        Button btnDelete;
        Button btnSave;
        Button btnClose;
        
        #endregion




        /// 생성자
        public AttFacDtlViewModel()
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

            attFacDtlView = obj as AttFacDtlView;
            cbCRE_YY = attFacDtlView.cbCRE_YY;
            btnDelete = attFacDtlView.btnDelete;
            btnSave = attFacDtlView.btnSave;
            btnClose = attFacDtlView.btnClose;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            //DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectCmmWttAttaDt");
            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);
            param.Add("ATTA_SEQ", this.ATTA_SEQ);

            WttAttaDt result = new WttAttaDt();
            result = BizUtil.SelectObject(param) as WttAttaDt;

            if(result != null)
            {
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
                   Console.WriteLine(propName + " - " + prop.GetValue(this,null));
                }
            }


            //다큐먼트는 따로 처리
            Paragraph p = new Paragraph();
            try
            {
                p.Inlines.Add(this.ATT_DES.Trim());
                attFacDtlView.txtATT_DES.Document.Blocks.Clear();
                attFacDtlView.txtATT_DES.Document.Blocks.Add(p);
            }
            catch (Exception) { }


            //5.신규/수정구분처리
            if (ATTA_SEQ < 0)
            {
                attFacDtlView.txtFTR_NAM.Text = BizUtil.GetCodeNm("Select_FTR_LIST2", FTR_CDE);
            }

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if(!BizUtil.ValidReq(attFacDtlView))  return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;


            try
            {
                //다큐먼트는 따로 처리
                this.ATT_DES = new TextRange(attFacDtlView.txtATT_DES.Document.ContentStart, attFacDtlView.txtATT_DES.Document.ContentEnd).Text;
                BizUtil.Update2(this, "SaveWttAttaDt");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }

            Messages.ShowOkMsgBox();
            //화면닫기
            btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }

        /// <summary>
        /// 삭제처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {

            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("부속세부시설현황을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(this, "DeleteWttAttaDt");
            }
            catch (Exception )
            {
                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
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
                // cbCRE_YY 년도
                dtCRE_YY = new DataTable();
                dtCRE_YY.Columns.Add("CRE_YY", typeof(String));
                dtCRE_YY.Columns.Add("CRE_YY_NM", typeof(String));

                DataRow dr = dtCRE_YY.NewRow();
                dr["CRE_YY"] = " ";
                dr["CRE_YY_NM"] = "선택";
                dtCRE_YY.Rows.InsertAt(dr, 0);

                int yy_to = 2030;
                int i = 0;
                for (int y=2000; y<yy_to; y++)
                {
                    dr = dtCRE_YY.NewRow();
                    dr["CRE_YY"] = y.ToString();
                    dr["CRE_YY_NM"] = y.ToString() + "년";
                    dtCRE_YY.Rows.InsertAt(dr,++i);
                }


                // combo객체 Cd/Nm 필드매핑
                cbCRE_YY.DisplayMember = "CRE_YY_NM";
                cbCRE_YY.ValueMember = "CRE_YY";

                cbCRE_YY.ItemsSource = dtCRE_YY;
                cbCRE_YY.SelectedIndex = 0;

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
