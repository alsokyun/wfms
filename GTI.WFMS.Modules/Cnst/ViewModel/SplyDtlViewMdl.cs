using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using System;
using System.Collections;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{
    public class SplyDtlViewMdl 
    {
        public SplyDtl Dtl { get; set; }

        /// 생성자
        public SplyDtlViewMdl(string CNT_NUM)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWttSplyMaDtl");
                param.Add("CNT_NUM", CNT_NUM);

                Dtl = BizUtil.SelectObject(param) as SplyDtl;

            }
            catch (Exception){}



        }

    }
}
