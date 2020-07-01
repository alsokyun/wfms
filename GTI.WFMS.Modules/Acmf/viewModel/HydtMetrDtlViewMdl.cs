using GTI.WFMS.Models.Acmf.Model;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Acmf.ViewModel
{
    public class HydtMetrDtlViewMdl 
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }
        public List<WttMetaHt> Tab02List { get; set; }
        public HydtMetrDtl Dtl { get; set; }

        /// 생성자
        public HydtMetrDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectHydtMetrDtl");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                Dtl = BizUtil.SelectObject(param) as HydtMetrDtl;




                //2.탭1 - 유지보수
                param = new Hashtable();
                param.Add("sqlId", "selectChscResSubList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab01List = (List<LinkFmsChscFtrRes>) BizUtil.SelectListObj<LinkFmsChscFtrRes>(param);
                
                //텝4 - 급수전계량기                
                param = new Hashtable();
                param.Add("sqlId", "selectWttMetaHtList");

                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                this.Tab02List = (List<WttMetaHt>) BizUtil.SelectListObj<WttMetaHt>(param);                
            }
            catch (Exception){}



        }

    }
}
