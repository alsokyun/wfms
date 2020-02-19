using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class WtlPipeDtlViewMdl : PipeDtl
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }
        public List<LinkWtlLeakPs> Tab03List { get; set; }

        /// 생성자
        public WtlPipeDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWtlPipeDtl2");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

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
                    Console.WriteLine(propName + " - " + prop.GetValue(this, null));
                }



                //2.유지보수(탭1)
                param = new Hashtable();
                param.Add("sqlId", "selectChscResSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab01List = (List<LinkFmsChscFtrRes>)BizUtil.SelectListObj<LinkFmsChscFtrRes>(param);

                //3.누수지점(탭3)
                param = new Hashtable();
                param.Add("sqlId", "selectWtlLeakSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab03List = (List<LinkWtlLeakPs>)BizUtil.SelectListObj<LinkWtlLeakPs>(param);
            }
            catch (Exception){}



        }

    }
}
