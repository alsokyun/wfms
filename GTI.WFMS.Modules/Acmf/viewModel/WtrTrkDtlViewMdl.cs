using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Acmf.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Acmf.ViewModel
{
    public class WtrTrkDtlViewMdl : WtrTrkDtl
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }
        public List<LinkWttRsrvHt> Tab02List { get; set; }

        /// 생성자
        public WtrTrkDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWtrTrkDtl");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                WtrTrkDtl result = new WtrTrkDtl();
                result = BizUtil.SelectObject(param) as WtrTrkDtl;
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



                //2.탭1 - 유지보수
                param = new Hashtable();
                param.Add("sqlId", "selectChscResSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab01List = (List<LinkFmsChscFtrRes>) BizUtil.SelectListObj<LinkFmsChscFtrRes>(param);

                //탭2 - 청소이력
                param = new Hashtable();
                param.Add("sqlId", "selectWttRsrvHtList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab02List = (List<LinkWttRsrvHt>) BizUtil.SelectListObj<LinkWttRsrvHt>(param);
            }
            catch (Exception){}



        }

    }
}
