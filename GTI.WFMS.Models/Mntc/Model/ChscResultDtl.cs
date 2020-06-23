using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class ChscResultDtl : CmmDtl
    {
       


        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private decimal? __SCL_NUM;
        public decimal? SCL_NUM
        {
            get { return __SCL_NUM; }
            set
            {
                this.__SCL_NUM = value;
                OnPropertyChanged("SCL_NUM");
            }
        }
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
        private decimal __FTR_IDN;
        public decimal FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                OnPropertyChanged("FTR_IDN");
            }
        }
        private decimal? __SEQ;
        public decimal? SEQ
        {
            get { return __SEQ; }
            set
            {
                this.__SEQ = value;
                OnPropertyChanged("SEQ");
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
        private string __RPR_YMD;
        public string RPR_YMD
        {
            get { return __RPR_YMD; }
            set
            {
                this.__RPR_YMD = value;
                OnPropertyChanged("RPR_YMD");
            }
        }
        private string __RPR_CAT_CDE;
        public string RPR_CAT_CDE
        {
            get { return __RPR_CAT_CDE; }
            set
            {
                this.__RPR_CAT_CDE = value;
                OnPropertyChanged("RPR_CAT_CDE");
            }
        }
        private string __RPR_CAT_NM;
        public string RPR_CAT_NM
        {
            get { return __RPR_CAT_NM; }
            set
            {
                this.__RPR_CAT_NM = value;
                OnPropertyChanged("RPR_CAT_NM");
            }
        }
        private string __RPR_CUZ_CDE;
        public string RPR_CUZ_CDE
        {
            get { return __RPR_CUZ_CDE; }
            set
            {
                this.__RPR_CUZ_CDE = value;
                OnPropertyChanged("RPR_CUZ_CDE");
            }
        }
        private string __RPR_CUZ_NM;
        public string RPR_CUZ_NM
        {
            get { return __RPR_CUZ_NM; }
            set
            {
                this.__RPR_CUZ_NM = value;
                OnPropertyChanged("RPR_CUZ_NM");
            }
        }
        private string __RPR_USR_NM;
        public string RPR_USR_NM
        {
            get { return __RPR_USR_NM; }
            set
            {
                this.__RPR_USR_NM = value;
                OnPropertyChanged("RPR_USR_NM");
            }
        }
        private string __RPR_CTNT;
        public string RPR_CTNT
        {
            get { return __RPR_CTNT; }
            set
            {
                this.__RPR_CTNT = value;
                OnPropertyChanged("RPR_CTNT");
            }
        }
        private string __HJD_CDE;
        public string HJD_CDE
        {
            get { return __HJD_CDE; }
            set
            {
                this.__HJD_CDE = value;
                OnPropertyChanged("HJD_CDE");
            }
        }
        private string __HJD_NAM;
        public string HJD_NAM
        {
            get { return __HJD_NAM; }
            set
            {
                this.__HJD_NAM = value;
                OnPropertyChanged("HJD_NAM");
            }
        }
        private string __FIL_SEQ;
        public string FIL_SEQ
        {
            get { return __FIL_SEQ; }
            set
            {
                this.__FIL_SEQ = value;
                OnPropertyChanged("FIL_SEQ");
            }
        }
        private string __FIL_NM;
        public string FIL_NM
        {
            get { return __FIL_NM; }
            set
            {
                this.__FIL_NM = value;
                OnPropertyChanged("FIL_NM");
            }
        }

    }
}
