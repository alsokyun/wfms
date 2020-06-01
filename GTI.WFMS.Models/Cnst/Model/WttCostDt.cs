using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class WttCostDt : CmmDtl
    {

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
        private decimal __COST_SEQ;
        public decimal COST_SEQ
        {
            get { return __COST_SEQ; }
            set
            {
                this.__COST_SEQ = value;
                OnPropertyChanged("COST_SEQ");
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
        private int __PAY_NUM;
        public int PAY_NUM
        {
            get { return __PAY_NUM; }
            set
            {
                this.__PAY_NUM = value;
                OnPropertyChanged("PAY_NUM");
            }
        }
        private string __PTY_CDE;
        public string PTY_CDE
        {
            get { return __PTY_CDE; }
            set
            {
                this.__PTY_CDE = value;
                OnPropertyChanged("PTY_CDE");
            }
        }
        
        private string __PTY_NAM;
        public string PTY_NAM
        {
            get { return __PTY_NAM; }
            set
            {
                this.__PTY_NAM = value;
                OnPropertyChanged("PTY_NAM");
            }
        }
        private string __PAY_YMD;
        public string PAY_YMD
        {
            get { return __PAY_YMD; }
            set
            {
                this.__PAY_YMD = value;
                OnPropertyChanged("PAY_YMD");
            }
        }
        private string __PAY_YMD_FMT;
        public string PAY_YMD_FMT
        {
            get { return __PAY_YMD_FMT; }
            set
            {
                this.__PAY_YMD_FMT = value;
                OnPropertyChanged("PAY_YMD_FMT");
            }
        }
        
        private int? __PAY_AMT;
        public int? PAY_AMT
        {
            get { return __PAY_AMT; }
            set
            {
                this.__PAY_AMT = value;
                OnPropertyChanged("PAY_AMT");
            }
        }
    }

}