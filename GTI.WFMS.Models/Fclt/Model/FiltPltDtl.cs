using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class FiltPltDtl : CmmDtl
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
        private string __PUR_NAM;
        public string PUR_NAM
        {
            get { return __PUR_NAM; }
            set
            {
                this.__PUR_NAM = value;
                OnPropertyChanged("PUR_NAM");
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
        private string __SRV_NAM;
        public string SRV_NAM
        {
            get { return __SRV_NAM; }
            set
            {
                this.__SRV_NAM = value;
                OnPropertyChanged("SRV_NAM");
            }
        }
        private decimal ?  __PUR_VOL;
        public decimal ? PUR_VOL
        {
            get { return __PUR_VOL; }
            set
            {
                this.__PUR_VOL = value;
                OnPropertyChanged("PUR_VOL");
            }
        }
        private decimal ?  __PWR_VOL;
        public decimal ? PWR_VOL
        {
            get { return __PWR_VOL; }
            set
            {
                this.__PWR_VOL = value;
                OnPropertyChanged("PWR_VOL");
            }
        }
        private decimal ?  __PUR_ARA;
        public decimal ? PUR_ARA
        {
            get { return __PUR_ARA; }
            set
            {
                this.__PUR_ARA = value;
                OnPropertyChanged("PUR_ARA");
            }
        }
        private string __SAM_CDE;
        public string SAM_CDE
        {
            get { return __SAM_CDE; }
            set
            {
                this.__SAM_CDE = value;
                OnPropertyChanged("SAM_CDE");
            }
        }
        private string __SAM_NAM;
        public string SAM_NAM
        {
            get { return __SAM_NAM; }
            set
            {
                this.__SAM_NAM = value;
                OnPropertyChanged("SAM_NAM");
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
