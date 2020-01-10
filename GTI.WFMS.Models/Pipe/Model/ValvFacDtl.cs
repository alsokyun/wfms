using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Pipe.Model
{
    public class ValvFacDtl : CmmDtl, INotifyPropertyChanged
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
        private string __VAL_MOP;
        public string VAL_MOP
        {
            get { return __VAL_MOP; }
            set
            {
                this.__VAL_MOP = value;
                OnPropertyChanged("VAL_MOP");
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
        private decimal __VAL_DIP;
        public decimal VAL_DIP
        {
            get { return __VAL_DIP; }
            set
            {
                this.__VAL_DIP = value;
                OnPropertyChanged("VAL_DIP");
            }
        }
        private string __SAE_CDE;
        public string SAE_CDE
        {
            get { return __SAE_CDE; }
            set
            {
                this.__SAE_CDE = value;
                OnPropertyChanged("SAE_CDE");
            }
        }
        private string __SAE_NAM;
        public string SAE_NAM
        {
            get { return __SAE_NAM; }
            set
            {
                this.__SAE_NAM = value;
                OnPropertyChanged("SAE_NAM");
            }
        }
        private decimal __TRO_CNT;
        public decimal TRO_CNT
        {
            get { return __TRO_CNT; }
            set
            {
                this.__TRO_CNT = value;
                OnPropertyChanged("TRO_CNT");
            }
        }
        private decimal __CRO_CNT;
        public decimal CRO_CNT
        {
            get { return __CRO_CNT; }
            set
            {
                this.__CRO_CNT = value;
                OnPropertyChanged("CRO_CNT");
            }
        }
        private string __MTH_CDE;
        public string MTH_CDE
        {
            get { return __MTH_CDE; }
            set
            {
                this.__MTH_CDE = value;
                OnPropertyChanged("MTH_CDE");
            }
        }
        private string __MTH_NAM;
        public string MTH_NAM
        {
            get { return __MTH_NAM; }
            set
            {
                this.__MTH_NAM = value;
                OnPropertyChanged("MTH_NAM");
            }
        }
        private string __VAL_FOR;
        public string VAL_FOR
        {
            get { return __VAL_FOR; }
            set
            {
                this.__VAL_FOR = value;
                OnPropertyChanged("VAL_FOR");
            }
        }
        private string __FOR_NAM;
        public string FOR_NAM
        {
            get { return __FOR_NAM; }
            set
            {
                this.__FOR_NAM = value;
                OnPropertyChanged("FOR_NAM");
            }
        }
        private string __VAL_STD;
        public string VAL_STD
        {
            get { return __VAL_STD; }
            set
            {
                this.__VAL_STD = value;
                OnPropertyChanged("VAL_STD");
            }
        }
        private decimal __VAL_SAF;
        public decimal VAL_SAF
        {
            get { return __VAL_SAF; }
            set
            {
                this.__VAL_SAF = value;
                OnPropertyChanged("VAL_SAF");
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
        private string __CST_CDE;
        public string CST_CDE
        {
            get { return __CST_CDE; }
            set
            {
                this.__CST_CDE = value;
                OnPropertyChanged("CST_CDE");
            }
        }
        private string __CST_NAM;
        public string CST_NAM
        {
            get { return __CST_NAM; }
            set
            {
                this.__CST_NAM = value;
                OnPropertyChanged("CST_NAM");
            }
        }
        private string __OFF_CDE;
        public string OFF_CDE
        {
            get { return __OFF_CDE; }
            set
            {
                this.__OFF_CDE = value;
                OnPropertyChanged("OFF_CDE");
            }
        }
        private string __OFF_NAM;
        public string OFF_NAM
        {
            get { return __OFF_NAM; }
            set
            {
                this.__OFF_NAM = value;
                OnPropertyChanged("OFF_NAM");
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
        private decimal __ANG_DIR;
        public decimal ANG_DIR
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
