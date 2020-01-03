using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Cmm.Model
{
    /// <summary>
    /// 뷰모델의 공통 클래스 - 사용자ID 등 기본속성
    /// </summary>
    public class CmmDtl
    {
        private string __ID;
        private string __DUP; //중복체크상태

        public string ID
        {
            get { return __ID; }
            set
            {
                this.__ID = value;
            }
        }
        public string DUP
        {
            get { return __DUP; }
            set
            {
                this.__DUP = value;
            }
        }


    }
}
