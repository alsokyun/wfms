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
