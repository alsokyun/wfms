using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Acmf.Model
{
    public class WtrTrkDtl : CmmDtl, INotifyPropertyChanged
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
        private string __MNG_CDE_NAM;
        public string MNG_CDE_NAM
        {
            get { return __MNG_CDE_NAM; }
            set
            {
                this.__MNG_CDE_NAM = value;
                OnPropertyChanged("MNG_CDE_NAM");
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
        private string __PMS_YMD;
        public string PMS_YMD
        {
            get { return __PMS_YMD; }
            set
            {
                this.__PMS_YMD = value;
                OnPropertyChanged("PMS_YMD");
            }
        }
        private string __BLS_CDE;
        public string BLS_CDE
        {
            get { return __BLS_CDE; }
            set
            {
                this.__BLS_CDE = value;
                OnPropertyChanged("BLS_CDE");
            }
        }
        private string __BLS_NAM;
        public string BLS_NAM
        {
            get { return __BLS_NAM; }
            set
            {
                this.__BLS_NAM = value;
                OnPropertyChanged("BLS_NAM");
            }
        }
        private string __RSR_NAM;
        public string RSR_NAM
        {
            get { return __RSR_NAM; }
            set
            {
                this.__RSR_NAM = value;
                OnPropertyChanged("RSR_NAM");
            }
        }
        private string __OWN_NAM;
        public string OWN_NAM
        {
            get { return __OWN_NAM; }
            set
            {
                this.__OWN_NAM = value;
                OnPropertyChanged("OWN_NAM");
            }
        }
        private string __OWN_ADR;
        public string OWN_ADR
        {
            get { return __OWN_ADR; }
            set
            {
                this.__OWN_ADR = value;
                OnPropertyChanged("OWN_ADR");
            }
        }
        private string __OWN_TEL;
        public string OWN_TEL
        {
            get { return __OWN_TEL; }
            set
            {
                this.__OWN_TEL = value;
                OnPropertyChanged("OWN_TEL");
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
        private string __MNG_ADR;
        public string MNG_ADR
        {
            get { return __MNG_ADR; }
            set
            {
                this.__MNG_ADR = value;
                OnPropertyChanged("MNG_ADR");
            }
        }
        private string __MNG_TEL;
        public string MNG_TEL
        {
            get { return __MNG_TEL; }
            set
            {
                this.__MNG_TEL = value;
                OnPropertyChanged("MNG_TEL");
            }
        }
        private decimal ?  __BLD_ARA;
        public decimal ? BLD_ARA
        {
            get { return __BLD_ARA; }
            set
            {
                this.__BLD_ARA = value;
                OnPropertyChanged("BLD_ARA");
            }
        }
        private decimal ?  __TBL_ARA;
        public decimal ? TBL_ARA
        {
            get { return __TBL_ARA; }
            set
            {
                this.__TBL_ARA = value;
                OnPropertyChanged("TBL_ARA");
            }
        }
        private decimal ?  __FAM_CNT;
        public decimal ? FAM_CNT
        {
            get { return __FAM_CNT; }
            set
            {
                this.__FAM_CNT = value;
                OnPropertyChanged("FAM_CNT");
            }
        }
        private string __BLD_ADR;
        public string BLD_ADR
        {
            get { return __BLD_ADR; }
            set
            {
                this.__BLD_ADR = value;
                OnPropertyChanged("BLD_ADR");
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
