using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
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
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    public class CnstMngDtlViewMdl : CnstDtl
    {
        public List<WttCostDt> Tab01List { get; set; }
        public List<WttChngDt> Tab02List { get; set; }
        public List<WttSubcDt> Tab03List { get; set; }
        public List<WttFlawDt> Tab04List { get; set; }

        /// 생성자
        public CnstMngDtlViewMdl(string CNT_NUM)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWttConsMaDtl");
                param.Add("CNT_NUM", CNT_NUM);

                CnstDtl result = new CnstDtl();
                result = BizUtil.SelectObject(param) as CnstDtl;
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



                //2.Tab(4)

                //공사비지급내역
                param = new Hashtable();
                param.Add("CNT_NUM", CNT_NUM);
                param.Add("sqlId", "SelectWttCostDtList");
                this.Tab01List = (List<WttCostDt>) BizUtil.SelectListObj<WttCostDt>(param);

                //설계변경내역
                param = new Hashtable();
                param.Add("CNT_NUM", CNT_NUM);
                param.Add("sqlId", "SelectWttChngDtList");
                this.Tab02List = (List<WttChngDt>) BizUtil.SelectListObj<WttChngDt>(param);

                //공사하도급내역
                param = new Hashtable();
                param.Add("CNT_NUM", CNT_NUM);
                param.Add("sqlId", "SelectWttSubcDtList2");
                this.Tab03List = (List<WttSubcDt>)BizUtil.SelectListObj<WttSubcDt>(param);

                //하자보수목록
                param = new Hashtable();
                param.Add("CNT_NUM", CNT_NUM);
                param.Add("sqlId", "SelectWttFlawDtList");
                this.Tab04List = (List<WttFlawDt>)BizUtil.SelectListObj<WttFlawDt>(param);
            }
            catch (Exception){}



        }

    }
}
