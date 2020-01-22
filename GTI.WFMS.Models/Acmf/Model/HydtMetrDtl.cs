using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class HydtMetrDtl : CmmDtl, INotifyPropertyChanged
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
        private string __HOM_NUM;
        public string HOM_NUM
        {
            get { return __HOM_NUM; }
            set
            {
                this.__HOM_NUM = value;
                OnPropertyChanged("HOM_NUM");
            }
        }
        private string __HOM_NAM;
        public string HOM_NAM
        {
            get { return __HOM_NAM; }
            set
            {
                this.__HOM_NAM = value;
                OnPropertyChanged("HOM_NAM");
            }
        }
        private string __HOM_CDE;
        public string HOM_CDE
        {
            get { return __HOM_CDE; }
            set
            {
                this.__HOM_CDE = value;
                OnPropertyChanged("HOM_CDE");
            }
        }
        private string __HOM_CDE_NAM;
        public string HOM_CDE_NAM
        {
            get { return __HOM_CDE_NAM; }
            set
            {
                this.__HOM_CDE_NAM = value;
                OnPropertyChanged("HOM_CDE_NAM");
            }
        }
        private string __HOM_ADR;
        public string HOM_ADR
        {
            get { return __HOM_ADR; }
            set
            {
                this.__HOM_ADR = value;
                OnPropertyChanged("HOM_ADR");
            }
        }
        private decimal __HOM_CNT;
        public decimal HOM_CNT
        {
            get { return __HOM_CNT; }
            set
            {
                this.__HOM_CNT = value;
                OnPropertyChanged("HOM_CNT");
            }
        }
        private string __SBI_CDE;
        public string SBI_CDE
        {
            get { return __SBI_CDE; }
            set
            {
                this.__SBI_CDE = value;
                OnPropertyChanged("SBI_CDE");
            }
        }
        private string __SBI_NAM;
        public string SBI_NAM
        {
            get { return __SBI_NAM; }
            set
            {
                this.__SBI_NAM = value;
                OnPropertyChanged("SBI_NAM");
            }
        }
        private decimal __MET_DIP;
        public decimal MET_DIP
        {
            get { return __MET_DIP; }
            set
            {
                this.__MET_DIP = value;
                OnPropertyChanged("MET_DIP");
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
        private string __PRD_NUM;
        public string PRD_NUM
        {
            get { return __PRD_NUM; }
            set
            {
                this.__PRD_NUM = value;
                OnPropertyChanged("PRD_NUM");
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
        private string __PIP_NAM;
        public string PIP_NAM
        {
            get { return __PIP_NAM; }
            set
            {
                this.__PIP_NAM = value;
                OnPropertyChanged("PIP_NAM");
            }
        }
        private decimal __PIP_IDN;
        public decimal PIP_IDN
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
        private string __MET_NUM;
        public string MET_NUM
        {
            get { return __MET_NUM; }
            set
            {
                this.__MET_NUM = value;
                OnPropertyChanged("MET_NUM");
            }
        }
        private string __MET_MOF;
        public string MET_MOF
        {
            get { return __MET_MOF; }
            set
            {
                this.__MET_MOF = value;
                OnPropertyChanged("MET_MOF");
            }
        }
        private string __HOM_HJD;
        public string HOM_HJD
        {
            get { return __HOM_HJD; }
            set
            {
                this.__HOM_HJD = value;
                OnPropertyChanged("HOM_HJD");
            }
        }
    }
}
