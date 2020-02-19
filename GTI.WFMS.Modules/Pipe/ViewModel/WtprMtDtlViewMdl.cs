﻿using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class WtprMtDtlViewMdl : WtprMtDtl
    {
        public List<LinkFmsChscFtrRes> Tab01List { get; set; }

        /// 생성자
        public WtprMtDtlViewMdl(string FTR_CDE, int FTR_IDN)
        {
            try
            {
                // 1.상세마스터
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectWtprMtDtl");
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);

                WtprMtDtl result = new WtprMtDtl();
                result = BizUtil.SelectObject(param) as WtprMtDtl;
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