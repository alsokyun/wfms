using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Fctl.Model
{
    public class WttAttaDt : CmmDtl, INotifyPropertyChanged
    {
        /// <summary>
        /// 인터페이스 구현부분
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private int __RNO;
        public int RNO
        {
            get { return __RNO; }
            set
            {
                this.__RNO = value;
                RaisePropertyChanged("RNO");
            }
        }
        private int? __ATT_IDN;
        public int? ATT_IDN
        {
            get { return __ATT_IDN; }
            set
            {
                this.__ATT_IDN = value;
                RaisePropertyChanged("ATT_IDN");
            }
        }
        private string __FTR_CDE;
        public string FTR_CDE
        {
            get { return __FTR_CDE; }
            set
            {
                this.__FTR_CDE = value;
                RaisePropertyChanged("FTR_CDE");
            }
        }
        private string __FTR_NAM;
        public string FTR_NAM
        {
            get { return __FTR_NAM; }
            set
            {
                this.__FTR_NAM = value;
                RaisePropertyChanged("FTR_NAM");
            }
        }
        private decimal __FTR_IDN;
        public decimal FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                RaisePropertyChanged("FTR_IDN");
            }
        }
        private string __ATT_NAM;
        public string ATT_NAM
        {
            get { return __ATT_NAM; }
            set
            {
                this.__ATT_NAM = value;
                RaisePropertyChanged("ATT_NAM");
            }
        }
        private string __ATT_DES;
        public string ATT_DES
        {
            get { return __ATT_DES; }
            set
            {
                this.__ATT_DES = value;
                RaisePropertyChanged("ATT_DES");
            }
        }
        private decimal __ATTA_SEQ;
        public decimal ATTA_SEQ
        {
            get { return __ATTA_SEQ; }
            set
            {
                this.__ATTA_SEQ = value;
                RaisePropertyChanged("ATTA_SEQ");
            }
        }
        private string __CRE_YY;
        public string CRE_YY
        {
            get { return __CRE_YY; }
            set
            {
                this.__CRE_YY = value;
                RaisePropertyChanged("CRE_YY");
            }
        }
    }
}