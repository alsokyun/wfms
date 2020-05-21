using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class WtrSourDtl : CmmDtl, INotifyPropertyChanged
    {
        /// <summary>                                                                
        /// 인터페이스 구현부분                                                       
        /// </summary>                                                                
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



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
        private string __HEA_NAM;
        public string HEA_NAM
        {
            get { return __HEA_NAM; }
            set
            {
                this.__HEA_NAM = value;
                OnPropertyChanged("HEA_NAM");
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
        private string __IRV_NAM;
        public string IRV_NAM
        {
            get { return __IRV_NAM; }
            set
            {
                this.__IRV_NAM = value;
                OnPropertyChanged("IRV_NAM");
            }
        }
        private decimal ?  __RSV_VOL;
        public decimal ? RSV_VOL
        {
            get { return __RSV_VOL; }
            set
            {
                this.__RSV_VOL = value;
                OnPropertyChanged("RSV_VOL");
            }
        }
        private decimal ?  __RSV_ARA;
        public decimal ? RSV_ARA
        {
            get { return __RSV_ARA; }
            set
            {
                this.__RSV_ARA = value;
                OnPropertyChanged("RSV_ARA");
            }
        }
        private decimal ?  __FUL_ARA;
        public decimal ? FUL_ARA
        {
            get { return __FUL_ARA; }
            set
            {
                this.__FUL_ARA = value;
                OnPropertyChanged("FUL_ARA");
            }
        }
        private decimal ?  __THR_WAL;
        public decimal ? THR_WAL
        {
            get { return __THR_WAL; }
            set
            {
                this.__THR_WAL = value;
                OnPropertyChanged("THR_WAL");
            }
        }
        private decimal ?  __HTH_WAL;
        public decimal ? HTH_WAL
        {
            get { return __HTH_WAL; }
            set
            {
                this.__HTH_WAL = value;
                OnPropertyChanged("HTH_WAL");
            }
        }
        private decimal ?  __AVG_WAL;
        public decimal ? AVG_WAL
        {
            get { return __AVG_WAL; }
            set
            {
                this.__AVG_WAL = value;
                OnPropertyChanged("AVG_WAL");
            }
        }
        private decimal ?  __DRA_WAL;
        public decimal ? DRA_WAL
        {
            get { return __DRA_WAL; }
            set
            {
                this.__DRA_WAL = value;
                OnPropertyChanged("DRA_WAL");
            }
        }
        private decimal ?  __HDR_WAL;
        public decimal ? HDR_WAL
        {
            get { return __HDR_WAL; }
            set
            {
                this.__HDR_WAL = value;
                OnPropertyChanged("HDR_WAL");
            }
        }
        private decimal ?  __KEE_WAL;
        public decimal ? KEE_WAL
        {
            get { return __KEE_WAL; }
            set
            {
                this.__KEE_WAL = value;
                OnPropertyChanged("KEE_WAL");
            }
        }
        private decimal ?  __GUA_ARA;
        public decimal ? GUA_ARA
        {
            get { return __GUA_ARA; }
            set
            {
                this.__GUA_ARA = value;
                OnPropertyChanged("GUA_ARA");
            }
        }
        private decimal ?  __GUA_POP;
        public decimal ? GUA_POP
        {
            get { return __GUA_POP; }
            set
            {
                this.__GUA_POP = value;
                OnPropertyChanged("GUA_POP");
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
