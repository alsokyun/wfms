using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class WtrSupDtl : CmmDtl
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
        private string __SAG_CDE;
        public string SAG_CDE
        {
            get { return __SAG_CDE; }
            set
            {
                this.__SAG_CDE = value;
                OnPropertyChanged("SAG_CDE");
            }
        }
        private string __SAG_NAM;
        public string SAG_NAM
        {
            get { return __SAG_NAM; }
            set
            {
                this.__SAG_NAM = value;
                OnPropertyChanged("SAG_NAM");
            }
        }
        private decimal ?  __SRV_VOL;
        public decimal ? SRV_VOL
        {
            get { return __SRV_VOL; }
            set
            {
                this.__SRV_VOL = value;
                OnPropertyChanged("SRV_VOL");
            }
        }
        private decimal ?  __HGH_WAL;
        public decimal ? HGH_WAL
        {
            get { return __HGH_WAL; }
            set
            {
                this.__HGH_WAL = value;
                OnPropertyChanged("HGH_WAL");
            }
        }
        private decimal ?  __LOW_WAL;
        public decimal ? LOW_WAL
        {
            get { return __LOW_WAL; }
            set
            {
                this.__LOW_WAL = value;
                OnPropertyChanged("LOW_WAL");
            }
        }
        private decimal ?  __ISR_VOL;
        public decimal ? ISR_VOL
        {
            get { return __ISR_VOL; }
            set
            {
                this.__ISR_VOL = value;
                OnPropertyChanged("ISR_VOL");
            }
        }
        private string __SUP_ARE;
        public string SUP_ARE
        {
            get { return __SUP_ARE; }
            set
            {
                this.__SUP_ARE = value;
                OnPropertyChanged("SUP_ARE");
            }
        }

        private int ?  __SUP_POP;
        public int ? SUP_POP
        {
            get { return __SUP_POP; }
            set
            {
                this.__SUP_POP = value;
                OnPropertyChanged("SUP_POP");
            }
        }
        private string __SCW_CDE;
        public string SCW_CDE
        {
            get { return __SCW_CDE; }
            set
            {
                this.__SCW_CDE = value;
                OnPropertyChanged("SCW_CDE");
            }
        }
        private string __SCW_NAM;
        public string SCW_NAM
        {
            get { return __SCW_NAM; }
            set
            {
                this.__SCW_NAM = value;
                OnPropertyChanged("SCW_NAM");
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
        private decimal ?  __SRV_ARA;
        public decimal ? SRV_ARA
        {
            get { return __SRV_ARA; }
            set
            {
                this.__SRV_ARA = value;
                OnPropertyChanged("SRV_ARA");
            }
        }        
    }
}
