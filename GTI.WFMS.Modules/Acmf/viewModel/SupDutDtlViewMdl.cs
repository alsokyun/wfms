using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Acmf.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Acmf.ViewModel
{
    public class SupDutDtlViewMdl : SupDutDtl
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }
        public List<HydtMetrDtl> Tab02List { get; set; }

        /// 생성자
        public SupDutDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectSupDutDtl");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                SupDutDtl result = new SupDutDtl();
                result = BizUtil.SelectObject(param) as SupDutDtl;
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
                    Console.WriteLine(propName + " - " + prop.GetValue(this, null));
                }



                //2.텝1 - 유지보tn
                param = new Hashtable();
                param.Add("sqlId", "selectChscResSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab01List = (List<LinkFmsChscFtrRes>)BizUtil.SelectListObj<LinkFmsChscFtrRes>(param);

                //텝4 - 급수전계량기                
                param = new Hashtable();
                param.Add("sqlId", "SelectHydtMetrDtl");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab02List = (List<HydtMetrDtl>) BizUtil.SelectListObj<HydtMetrDtl>(param);
            }
            catch (Exception) { }



        }

    }
}
