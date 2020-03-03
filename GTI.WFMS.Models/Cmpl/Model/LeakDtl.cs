using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Cmpl.Model
{
    public class LeakDtl : CmmDtl, INotifyPropertyChanged
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
        private decimal __FTR_IDN;
        public decimal FTR_IDN
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
        private string __RCV_NUM;
        public string RCV_NUM
        {
            get { return __RCV_NUM; }
            set
            {
                this.__RCV_NUM = value;
                OnPropertyChanged("RCV_NUM");
            }
        }
        private string __LEK_YMD;
        public string LEK_YMD
        {
            get { return __LEK_YMD; }
            set
            {
                this.__LEK_YMD = value;
                OnPropertyChanged("LEK_YMD");
            }
        }
        private string __LEK_LOC;
        public string LEK_LOC
        {
            get { return __LEK_LOC; }
            set
            {
                this.__LEK_LOC = value;
                OnPropertyChanged("LEK_LOC");
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
        private string __LRS_CDE;
        public string LRS_CDE
        {
            get { return __LRS_CDE; }
            set
            {
                this.__LRS_CDE = value;
                OnPropertyChanged("LRS_CDE");
            }
        }
        private string __LRS_NAM;
        public string LRS_NAM
        {
            get { return __LRS_NAM; }
            set
            {
                this.__LRS_NAM = value;
                OnPropertyChanged("LRS_NAM");
            }
        }
        private string __LEP_CDE;
        public string LEP_CDE
        {
            get { return __LEP_CDE; }
            set
            {
                this.__LEP_CDE = value;
                OnPropertyChanged("LEP_CDE");
            }
        }
        private string __REP_YMD;
        public string REP_YMD
        {
            get { return __REP_YMD; }
            set
            {
                this.__REP_YMD = value;
                OnPropertyChanged("REP_YMD");
            }
        }
        private string __REP_EXP;
        public string REP_EXP
        {
            get { return __REP_EXP; }
            set
            {
                this.__REP_EXP = value;
                OnPropertyChanged("REP_EXP");
            }
        }
        private string __MAT_DES;
        public string MAT_DES
        {
            get { return __MAT_DES; }
            set
            {
                this.__MAT_DES = value;
                OnPropertyChanged("MAT_DES");
            }
        }
        private string __REP_NAM;
        public string REP_NAM
        {
            get { return __REP_NAM; }
            set
            {
                this.__REP_NAM = value;
                OnPropertyChanged("REP_NAM");
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
        private decimal __PIP_DIP;
        public decimal PIP_DIP
        {
            get { return __PIP_DIP; }
            set
            {
                this.__PIP_DIP = value;
                OnPropertyChanged("PIP_DIP");
            }
        }
        private string __LEK_EXP;
        public string LEK_EXP
        {
            get { return __LEK_EXP; }
            set
            {
                this.__LEK_EXP = value;
                OnPropertyChanged("LEK_EXP");
            }
        }
    }
}