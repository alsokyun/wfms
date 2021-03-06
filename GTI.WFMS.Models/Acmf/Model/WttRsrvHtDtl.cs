﻿using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Acmf.Model
{
    public class WttRsrvHtDtl : CmmDtl
    {
       



        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private string __FTR_CDE;
        public string FTR_CDE
        {
            get { return __FTR_CDE; }
            set
            {
                this.__FTR_CDE = value;
                OnPropertyChanged("FTR_CDE");
            }
        }
        private string __FTR_NAM;
        public string FTR_NAM
        {
            get { return __FTR_NAM; }
            set
            {
                this.__FTR_NAM = value;
                OnPropertyChanged("FTR_NAM");
            }
        }

        private decimal ?  __FTR_IDN;
        public decimal ? FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                OnPropertyChanged("FTR_IDN");
            }
        }
        private int ?  __SEQ;
        public int ? SEQ
        {
            get { return __SEQ; }
            set
            {
                this.__SEQ = value;
                OnPropertyChanged("SEQ");
            }
        }
        private decimal ?  __CLN_NUM;
        public decimal ? CLN_NUM
        {
            get { return __CLN_NUM; }
            set
            {
                this.__CLN_NUM = value;
                OnPropertyChanged("CLN_NUM");
            }
        }
        private string __CLN_YMD;
        public string CLN_YMD
        {
            get { return __CLN_YMD; }
            set
            {
                this.__CLN_YMD = value;
                OnPropertyChanged("CLN_YMD");
            }
        }
        private string __CLN_EXP;
        public string CLN_EXP
        {
            get { return __CLN_EXP; }
            set
            {
                this.__CLN_EXP = value;
                OnPropertyChanged("CLN_EXP");
            }
        }
        private string __CLN_NAM;
        public string CLN_NAM
        {
            get { return __CLN_NAM; }
            set
            {
                this.__CLN_NAM = value;
                OnPropertyChanged("CLN_NAM");
            }
        }
    }
}