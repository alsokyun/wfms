using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Pipe.Model
{
    public class StndPiDtl : CmmDtl
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
                       
        private string __VAL_MOF;
        public string VAL_MOF
        {
            get { return __VAL_MOF; }
            set
            {
                this.__VAL_MOF = value;
                OnPropertyChanged("VAL_MOF");
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
        private string __STP_MOP;
        public string STP_MOP
        {
            get { return __STP_MOP; }
            set
            {
                this.__STP_MOP = value;
                OnPropertyChanged("STP_MOP");
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

        private decimal ?  __STD_DIP;
        public decimal ? STD_DIP
        {
            get { return __STD_DIP; }
            set
            {
                this.__STD_DIP = value;
                OnPropertyChanged("STD_DIP");
            }
        }
        private decimal ?  __STP_ALT;
        public decimal ? STP_ALT
        {
            get { return __STP_ALT; }
            set
            {
                this.__STP_ALT = value;
                OnPropertyChanged("STP_ALT");
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
