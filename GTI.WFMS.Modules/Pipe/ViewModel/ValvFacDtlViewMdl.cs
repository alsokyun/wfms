using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class ValvFacDtlViewMdl 
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }

        public ValvFacDtl Dtl { get; set; }


        /// 생성자
        public ValvFacDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectValvFacDtl");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                Dtl = BizUtil.SelectObject(param) as ValvFacDtl;



                //2.유지보수(탭)
                param = new Hashtable();
                param.Add("sqlId", "selectChscResSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab01List = (List<LinkFmsChscFtrRes>) BizUtil.SelectListObj<LinkFmsChscFtrRes>(param);
            }
            catch (Exception){}



        }

    }
}
