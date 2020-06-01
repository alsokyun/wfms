using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class IntkStDtl : CmmDtl
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
        private string __FNS_YMD;
        public string FNS_YMD
        {
            get { return __FNS_YMD; }
            set
            {
                this.__FNS_YMD = value;
                OnPropertyChanged("FNS_YMD");
            }
        }
        private string __GAI_NAM;
        public string GAI_NAM
        {
            get { return __GAI_NAM; }
            set
            {
                this.__GAI_NAM = value;
                OnPropertyChanged("GAI_NAM");
            }
        }
        private string __WSR_CDE;
        public string WSR_CDE
        {
            get { return __WSR_CDE; }
            set
            {
                this.__WSR_CDE = value;
                OnPropertyChanged("WSR_CDE");
            }
        }
        private string __WSR_NAM;
        public string WSR_NAM
        {
            get { return __WSR_NAM; }
            set
            {
                this.__WSR_NAM = value;
                OnPropertyChanged("WSR_NAM");
            }
        }
        private string __WSS_NAM;
        public string WSS_NAM
        {
            get { return __WSS_NAM; }
            set
            {
                this.__WSS_NAM = value;
                OnPropertyChanged("WSS_NAM");
            }
        }
        private decimal ?  __AGA_VOL;
        public decimal ? AGA_VOL
        {
            get { return __AGA_VOL; }
            set
            {
                this.__AGA_VOL = value;
                OnPropertyChanged("AGA_VOL");
            }
        }
        private decimal ?  __HGA_VOL;
        public decimal ? HGA_VOL
        {
            get { return __HGA_VOL; }
            set
            {
                this.__HGA_VOL = value;
                OnPropertyChanged("HGA_VOL");
            }
        }
        private decimal ?  __PMP_CNT;
        public decimal ? PMP_CNT
        {
            get { return __PMP_CNT; }
            set
            {
                this.__PMP_CNT = value;
                OnPropertyChanged("PMP_CNT");
            }
        }
        private decimal ?  __GAI_ARA;
        public decimal ? GAI_ARA
        {
            get { return __GAI_ARA; }
            set
            {
                this.__GAI_ARA = value;
                OnPropertyChanged("GAI_ARA");
            }
        }
        private string __WRW_CDE;
        public string WRW_CDE
        {
            get { return __WRW_CDE; }
            set
            {
                this.__WRW_CDE = value;
                OnPropertyChanged("WRW_CDE");
            }
        }
        private string __WRW_NAM;
        public string WRW_NAM
        {
            get { return __WRW_NAM; }
            set
            {
                this.__WRW_NAM = value;
                OnPropertyChanged("WRW_NAM");
            }
        }
        private string __WGW_CDE;
        public string WGW_CDE
        {
            get { return __WGW_CDE; }
            set
            {
                this.__WGW_CDE = value;
                OnPropertyChanged("WGW_CDE");
            }
        }
        private string __WGW_NAM;
        public string WGW_NAM
        {
            get { return __WGW_NAM; }
            set
            {
                this.__WGW_NAM = value;
                OnPropertyChanged("WGW_NAM");
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
    }
}
