using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Modules.Mntc.Model
{
    public class FaqDtl : CmmDtl 
    {
      
        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        /// 
        private decimal ?  __SEQ;
        public decimal ? SEQ
        {
            get { return __SEQ; }
            set
            {
                this.__SEQ = value;
                OnPropertyChanged("SEQ");
            }
        }
        private string __FAQ_CAT_CDE;
        public string FAQ_CAT_CDE
        {
            get { return __FAQ_CAT_CDE; }
            set
            {
                this.__FAQ_CAT_CDE = value;
                OnPropertyChanged("FAQ_CAT_CDE");
            }
        }
        private string __TTL;
        public string TTL
        {
            get { return __TTL; }
            set
            {
                this.__TTL = value;
                OnPropertyChanged("TTL");
            }
        }
        private string __DEL_YN;
        public string DEL_YN
        {
            get { return __DEL_YN; }
            set
            {
                this.__DEL_YN = value;
                OnPropertyChanged("DEL_YN");
            }
        }


        private string __REG_ID;
        public string REG_ID
        {
            get { return __REG_ID; }
            set
            {
                this.__REG_ID = value;
                OnPropertyChanged("REG_ID");
            }
        }
        private string __REG_DT;
        public string REG_DT
        {
            get { return __REG_DT; }
            set
            {
                this.__REG_DT = value;
                OnPropertyChanged("REG_DT");
            }
        }
        private string __EDT_ID;
        public string EDT_ID
        {
            get { return __EDT_ID; }
            set
            {
                this.__EDT_ID = value;
                OnPropertyChanged("EDT_ID");
            }
        }
        private decimal ?  __READ_CNT;
        public decimal ? READ_CNT
        {
            get { return __READ_CNT; }
            set
            {
                this.__READ_CNT = value;
                OnPropertyChanged("READ_CNT");
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
        private string __FAQ_CUZ_CDE;
        public string FAQ_CUZ_CDE
        {
            get { return __FAQ_CUZ_CDE; }
            set
            {
                this.__FAQ_CUZ_CDE = value;
                OnPropertyChanged("FAQ_CUZ_CDE");
            }
        }

        private string __QUESTION;
        public string QUESTION
        {
            get { return __QUESTION; }
            set
            {
                this.__QUESTION = value;
                OnPropertyChanged("QUESTION");
            }
        }
        private string __REPL;
        public string REPL
        {
            get { return __REPL; }
            set
            {
                this.__REPL = value;
                OnPropertyChanged("REPL");
            }
        }
    }

}