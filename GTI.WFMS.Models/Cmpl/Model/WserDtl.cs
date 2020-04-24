using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Cmpl.Model
{
    public class WserDtl : CmmDtl, INotifyPropertyChanged
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
        private decimal __WSER_SEQ;
        public decimal WSER_SEQ
        {
            get { return __WSER_SEQ; }
            set
            {
                this.__WSER_SEQ = value;
                OnPropertyChanged("WSER_SEQ");
            }
        }
        private string __RCV_YMD;
        public string RCV_YMD
        {
            get { return __RCV_YMD; }
            set
            {
                this.__RCV_YMD = value;
                OnPropertyChanged("RCV_YMD");
            }
        }
        private string __RCV_NAM;
        public string RCV_NAM
        {
            get { return __RCV_NAM; }
            set
            {
                this.__RCV_NAM = value;
                OnPropertyChanged("RCV_NAM");
            }
        }
        private string __APL_HJD;
        public string APL_HJD
        {
            get { return __APL_HJD; }
            set
            {
                this.__APL_HJD = value;
                OnPropertyChanged("APL_HJD");
            }
        }
        private string __APL_ADR;
        public string APL_ADR
        {
            get { return __APL_ADR; }
            set
            {
                this.__APL_ADR = value;
                OnPropertyChanged("APL_ADR");
            }
        }
        private string __APL_EXP;
        public string APL_EXP
        {
            get { return __APL_EXP; }
            set
            {
                this.__APL_EXP = value;
                OnPropertyChanged("APL_EXP");
            }
        }
        private string __APL_CDE;
        public string APL_CDE
        {
            get { return __APL_CDE; }
            set
            {
                this.__APL_CDE = value;
                OnPropertyChanged("APL_CDE");
            }
        }
        private string __APM_NAM;
        public string APM_NAM
        {
            get { return __APM_NAM; }
            set
            {
                this.__APM_NAM = value;
                OnPropertyChanged("APM_NAM");
            }
        }
        private string __APM_ADR;
        public string APM_ADR
        {
            get { return __APM_ADR; }
            set
            {
                this.__APM_ADR = value;
                OnPropertyChanged("APM_ADR");
            }
        }
        private string __APM_TEL;
        public string APM_TEL
        {
            get { return __APM_TEL; }
            set
            {
                this.__APM_TEL = value;
                OnPropertyChanged("APM_TEL");
            }
        }
        private string __DUR_YMD;
        public string DUR_YMD
        {
            get { return __DUR_YMD; }
            set
            {
                this.__DUR_YMD = value;
                OnPropertyChanged("DUR_YMD");
            }
        }
        private string __PRO_CDE;
        public string PRO_CDE
        {
            get { return __PRO_CDE ?? ""; }
            set
            {
                this.__PRO_CDE = value;
                OnPropertyChanged("PRO_CDE");
            }
        }
        private string __PRO_EXP;
        public string PRO_EXP
        {
            get { return __PRO_EXP; }
            set
            {
                this.__PRO_EXP = value;
                OnPropertyChanged("PRO_EXP");
            }
        }
        private string __PRO_YMD;
        public string PRO_YMD
        {
            get { return __PRO_YMD; }
            set
            {
                this.__PRO_YMD = value;
                OnPropertyChanged("PRO_YMD");
            }
        }
        private string __PRO_NAM;
        public string PRO_NAM
        {
            get { return __PRO_NAM; }
            set
            {
                this.__PRO_NAM = value;
                OnPropertyChanged("PRO_NAM");
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
    }
}
