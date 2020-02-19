using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class WttSubcDt : CmmDtl, INotifyPropertyChanged
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
        private string __CHK;
        public string CHK
        {
            get { return __CHK; }
            set
            {
                this.__CHK = value;
                OnPropertyChanged("CHK");
            }
        }
        private int __RNO;
        public int RNO
        {
            get { return __RNO; }
            set
            {
                this.__RNO = value;
                OnPropertyChanged("RNO");
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
        private decimal __SUBC_SEQ;
        public decimal SUBC_SEQ
        {
            get { return __SUBC_SEQ; }
            set
            {
                this.__SUBC_SEQ = value;
                OnPropertyChanged("SUBC_SEQ");
            }
        }
        private decimal __SUB_NUM;
        public decimal SUB_NUM
        {
            get { return __SUB_NUM; }
            set
            {
                this.__SUB_NUM = value;
                OnPropertyChanged("SUB_NUM");
            }
        }
        private string __SUB_NAM;
        public string SUB_NAM
        {
            get { return __SUB_NAM; }
            set
            {
                this.__SUB_NAM = value;
                OnPropertyChanged("SUB_NAM");
            }
        }
        private string __PSB_NAM;
        public string PSB_NAM
        {
            get { return __PSB_NAM; }
            set
            {
                this.__PSB_NAM = value;
                OnPropertyChanged("PSB_NAM");
            }
        }
        private string __SUB_ADR;
        public string SUB_ADR
        {
            get { return __SUB_ADR; }
            set
            {
                this.__SUB_ADR = value;
                OnPropertyChanged("SUB_ADR");
            }
        }
        private string __SUB_TEL;
        public string SUB_TEL
        {
            get { return __SUB_TEL; }
            set
            {
                this.__SUB_TEL = value;
                OnPropertyChanged("SUB_TEL");
            }
        }
    }
}