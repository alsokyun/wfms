using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Pipe.Model
{
    public class PipeDtl : CmmDtl
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
        private int __FTR_IDN;
        public int FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                OnPropertyChanged("FTR_IDN");
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
        private string __SHT_NUM;
        public string SHT_NUM
        {
            get { return __SHT_NUM; }
            set
            {
                this.__SHT_NUM = value;
                OnPropertyChanged("SHT_NUM");
            }
        }
        private string __MNG_CDE;
        public string MNG_CDE
        {
            get { return __MNG_CDE; }
            set
            {
                this.__MNG_CDE = value;
                OnPropertyChanged("MNG_CDE");
            }
        }
        private string __MNG_NAM;
        public string MNG_NAM
        {
            get { return __MNG_NAM; }
            set
            {
                this.__MNG_NAM = value;
                OnPropertyChanged("MNG_NAM");
            }
        }
        private string __IST_YMD;
        public string IST_YMD
        {
            get 
            {
                return __IST_YMD;
            }
            set
            {
                this.__IST_YMD = value;
                OnPropertyChanged("IST_YMD");
            }
        }
        private string __SAA_CDE;
        public string SAA_CDE
        {
            get { return __SAA_CDE; }
            set
            {
                this.__SAA_CDE = value;
                OnPropertyChanged("SAA_CDE");
            }
        }
        private string __SAA_NAM;
        public string SAA_NAM
        {
            get { return __SAA_NAM; }
            set
            {
                this.__SAA_NAM = value;
                OnPropertyChanged("SAA_NAM");
            }
        }
        private string __MOP_CDE;
        public string MOP_CDE
        {
            get { return __MOP_CDE; }
            set
            {
                this.__MOP_CDE = value;
                OnPropertyChanged("MOP_CDE");
            }
        }
        private string __MOP_NAM;
        public string MOP_NAM
        {
            get { return __MOP_NAM; }
            set
            {
                this.__MOP_NAM = value;
                OnPropertyChanged("MOP_NAM");
            }
        }
        private string __JHT_CDE;
        public string JHT_CDE
        {
            get { return __JHT_CDE; }
            set
            {
                this.__JHT_CDE = value;
                OnPropertyChanged("JHT_CDE");
            }
        }
        private string __JHT_NAM;
        public string JHT_NAM
        {
            get { return __JHT_NAM; }
            set
            {
                this.__JHT_NAM = value;
                OnPropertyChanged("JHT_NAM");
            }
        }
        private decimal ?  __LOW_DEP;
        public decimal ? LOW_DEP
        {
            get { return __LOW_DEP; }
            set
            {
                this.__LOW_DEP = value;
                OnPropertyChanged("LOW_DEP");
            }
        }
        private decimal ?  __HGH_DEP;
        public decimal ? HGH_DEP
        {
            get { return __HGH_DEP; }
            set
            {
                this.__HGH_DEP = value;
                OnPropertyChanged("HGH_DEP");
            }
        }
        private string __CNT_NUM;
        public string CNT_NUM
        {
            get { return __CNT_NUM; }
            set
            {
                this.__CNT_NUM = value;
                OnPropertyChanged("CNT_NUM");
            }
        }
        private string __SYS_CHK;
        public string SYS_CHK
        {
            get { return __SYS_CHK; }
            set
            {
                this.__SYS_CHK = value;
                OnPropertyChanged("SYS_CHK");
            }
        }
        private string __SYS_CHK_NAM;
        public string SYS_CHK_NAM
        {
            get { return __SYS_CHK_NAM; }
            set
            {
                this.__SYS_CHK_NAM = value;
                OnPropertyChanged("SYS_CHK_NAM");
            }
        }
        private string __PIP_LBL;
        public string PIP_LBL
        {
            get { return __PIP_LBL; }
            set
            {
                this.__PIP_LBL = value;
                OnPropertyChanged("PIP_LBL");
            }
        }
        private decimal ?  __PIP_DIP;
        public decimal ? PIP_DIP
        {
            get { return __PIP_DIP; }
            set
            {
                this.__PIP_DIP = value;
                OnPropertyChanged("PIP_DIP");
            }
        }
        private decimal ?  __PIP_LEN;
        public decimal ? PIP_LEN
        {
            get { return __PIP_LEN; }
            set
            {
                this.__PIP_LEN = value;
                OnPropertyChanged("PIP_LEN");
            }
        }


    }
}
