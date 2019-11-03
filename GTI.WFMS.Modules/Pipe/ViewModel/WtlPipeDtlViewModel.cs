using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    class WtlPipeDtlViewModel : PipeDtl
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }

        #endregion


        #region ==========  Member 정의 ==========
        WtlPipeDtlView wtlPipeDtlView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();
        ComboBoxEdit cbMOP_CDE; DataTable dtMOP_CDE = new DataTable();
        ComboBoxEdit cbJHT_CDE; DataTable dtJHT_CDE = new DataTable();
        ComboBoxEdit cbSAA_CDE; DataTable dtSAA_CDE = new DataTable();
        
        #endregion




        /// 생성자
        public WtlPipeDtlViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
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

            wtlPipeDtlView = values[0] as WtlPipeDtlView;
            cbMNG_CDE = wtlPipeDtlView.cbMNG_CDE;
            cbHJD_CDE = wtlPipeDtlView.cbHJD_CDE;
            cbMOP_CDE = wtlPipeDtlView.cbMOP_CDE;
            cbJHT_CDE = wtlPipeDtlView.cbJHT_CDE;
            cbSAA_CDE = wtlPipeDtlView.cbSAA_CDE;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회
            //DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWtlPipeDtl2");
            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);

            PipeDtl result = new PipeDtl();
            result = BizUtil.SelectObject(param) as PipeDtl;



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


        #endregion





        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbMNG_CDE
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMOP_CDE
                BizUtil.SetCmbCode(cbMOP_CDE, "MOP_CDE", true, "250102");

                // cbJHT_CDE
                BizUtil.SetCmbCode(cbJHT_CDE, "JHT_CDE", true);

                // cbSAA_CDE
                BizUtil.SetCmbCode(cbSAA_CDE, "SAA_CDE", true);
                
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
                        //btnAdd.Visibility = Visibility.Collapsed;
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
