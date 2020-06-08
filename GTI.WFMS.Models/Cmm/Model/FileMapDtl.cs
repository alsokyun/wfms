using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Cmm.Model
{
    public class FileMapDtl : CmmDtl
    {



        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private string __BIZ_ID;
        public string BIZ_ID
        {
            get { return __BIZ_ID; }
            set
            {
                this.__BIZ_ID = value;
                OnPropertyChanged("BIZ_ID");
            }
        }
        private decimal ?  __FIL_SEQ;
        public decimal ? FIL_SEQ
        {
            get { return __FIL_SEQ; }
            set
            {
                this.__FIL_SEQ = value;
                OnPropertyChanged("FIL_SEQ");
            }
        }
        private string __TIT_NAM;
        public string TIT_NAM
        {
            get { return __TIT_NAM; }
            set
            {
                this.__TIT_NAM = value;
                OnPropertyChanged("TIT_NAM");
            }
        }
        private string __GRP_TYP;
        public string GRP_TYP
        {
            get { return __GRP_TYP; }
            set
            {
                this.__GRP_TYP = value;
                OnPropertyChanged("GRP_TYP");
            }
        }
        private string __CRE_USR;
        public string CRE_USR
        {
            get { return __CRE_USR; }
            set
            {
                this.__CRE_USR = value;
                OnPropertyChanged("CRE_USR");
            }
        }
        private string __CRE_YMD;
        public string CRE_YMD
        {
            get { return __CRE_YMD; }
            set
            {
                this.__CRE_YMD = value;
                OnPropertyChanged("CRE_YMD");
            }
        }
        private string __UPD_USR;
        public string UPD_USR
        {
            get { return __UPD_USR; }
            set
            {
                this.__UPD_USR = value;
                OnPropertyChanged("UPD_USR");
            }
        }
        private string __UPD_YMD;
        public string UPD_YMD
        {
            get { return __UPD_YMD; }
            set
            {
                this.__UPD_YMD = value;
                OnPropertyChanged("UPD_YMD");
            }
        }
        private string __CTNT;
        public string CTNT
        {
            get { return __CTNT; }
            set
            {
                this.__CTNT = value;
                OnPropertyChanged("CTNT");
            }
        }
    }
}