using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class PdjtHtDtl : CmmDtl
    {
   

        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private decimal? __PDH_HT_NUM;
        public decimal? PDH_HT_NUM
        {
            get { return __PDH_HT_NUM; }
            set
            {
                this.__PDH_HT_NUM = value;
                OnPropertyChanged("PDH_HT_NUM");
            }
        }
        private decimal? __PDH_NUM;
        public decimal? PDH_NUM
        {
            get { return __PDH_NUM; }
            set
            {
                this.__PDH_NUM = value;
                OnPropertyChanged("PDH_NUM");
            }
        }
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
        private decimal __SCL_NUM;
        public decimal SCL_NUM
        {
            get { return __SCL_NUM; }
            set
            {
                this.__SCL_NUM = value;
                OnPropertyChanged("SCL_NUM");
            }
        }
        private decimal __SEQ;
        public decimal SEQ
        {
            get { return __SEQ; }
            set
            {
                this.__SEQ = value;
                OnPropertyChanged("SEQ");
            }
        }
        private int? __PDH_CNT;
        public int? PDH_CNT
        {
            get { return __PDH_CNT; }
            set
            {
                this.__PDH_CNT = value;
                OnPropertyChanged("PDH_CNT");
            }
        }
        private int? __PDH_AMT;
        public int? PDH_AMT
        {
            get { return __PDH_AMT; }
            set
            {
                this.__PDH_AMT = value;
                OnPropertyChanged("PDH_AMT");
            }
        }
        private string __PDT_NAM;
        public string PDT_NAM
        {
            get { return __PDT_NAM; }
            set
            {
                this.__PDT_NAM = value;
                OnPropertyChanged("PDT_NAM");
            }
        }
        private string __PDT_MDL_STD;
        public string PDT_MDL_STD
        {
            get { return __PDT_MDL_STD; }
            set
            {
                this.__PDT_MDL_STD = value;
                OnPropertyChanged("PDT_MDL_STD");
            }
        }
     
    }
}