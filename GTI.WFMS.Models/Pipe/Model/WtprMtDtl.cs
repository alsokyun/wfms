using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Pipe.Model
{
    public class WtprMtDtl : CmmDtl
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
            get { return __IST_YMD; }
            set
            {
                this.__IST_YMD = value;
                OnPropertyChanged("IST_YMD");
            }
        }
        private string __PGA_CDE;
        public string PGA_CDE
        {
            get { return __PGA_CDE; }
            set
            {
                this.__PGA_CDE = value;
                OnPropertyChanged("PGA_CDE");
            }
        }
        private string __PGA_NAM;
        public string PGA_NAM
        {
            get { return __PGA_NAM; }
            set
            {
                this.__PGA_NAM = value;
                OnPropertyChanged("PGA_NAM");
            }
        }
        private string __MOF_CDE;
        public string MOF_CDE
        {
            get { return __MOF_CDE; }
            set
            {
                this.__MOF_CDE = value;
                OnPropertyChanged("MOF_CDE");
            }
        }
        private string __MOF_NAM;
        public string MOF_NAM
        {
            get { return __MOF_NAM; }
            set
            {
                this.__MOF_NAM = value;
                OnPropertyChanged("MOF_NAM");
            }
        }
        private decimal ?  __PGA_DIP;
        public decimal ? PGA_DIP
        {
            get { return __PGA_DIP; }
            set
            {
                this.__PGA_DIP = value;
                OnPropertyChanged("PGA_DIP");
            }
        }
        private decimal ?  __STD_SAF;
        public decimal ? STD_SAF
        {
            get { return __STD_SAF; }
            set
            {
                this.__STD_SAF = value;
                OnPropertyChanged("STD_SAF");
            }
        }
        private decimal ?  __AVG_SAF;
        public decimal ? AVG_SAF
        {
            get { return __AVG_SAF; }
            set
            {
                this.__AVG_SAF = value;
                OnPropertyChanged("AVG_SAF");
            }
        }
        private decimal ?  __MSR_SAF;
        public decimal ? MSR_SAF
        {
            get { return __MSR_SAF; }
            set
            {
                this.__MSR_SAF = value;
                OnPropertyChanged("MSR_SAF");
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
        private string __PRD_NAM;
        public string PRD_NAM
        {
            get { return __PRD_NAM; }
            set
            {
                this.__PRD_NAM = value;
                OnPropertyChanged("PRD_NAM");
            }
        }
        private string __PIP_CDE;
        public string PIP_CDE
        {
            get { return __PIP_CDE; }
            set
            {
                this.__PIP_CDE = value;
                OnPropertyChanged("PIP_CDE");
            }
        }
        private int __PIP_IDN;
        public int PIP_IDN
        {
            get { return __PIP_IDN; }
            set
            {
                this.__PIP_IDN = value;
                OnPropertyChanged("PIP_IDN");
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
        private decimal ?  __ANG_DIR;
        public decimal ? ANG_DIR
        {
            get { return __ANG_DIR; }
            set
            {
                this.__ANG_DIR = value;
                OnPropertyChanged("ANG_DIR");
            }
        }        
    }
}
