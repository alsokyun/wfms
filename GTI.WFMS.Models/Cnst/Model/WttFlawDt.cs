using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class WttFlawDt : CmmDtl
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
        private decimal? __FLAW_SEQ;
        public decimal? FLAW_SEQ
        {
            get { return __FLAW_SEQ; }
            set
            {
                this.__FLAW_SEQ = value;
                OnPropertyChanged("FLAW_SEQ");
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
        private int __RPR_NUM;
        public int RPR_NUM
        {
            get { return __RPR_NUM; }
            set
            {
                this.__RPR_NUM = value;
                OnPropertyChanged("RPR_NUM");
            }
        }        
        private string __FLA_YMD;
        public string FLA_YMD
        {
            get { return __FLA_YMD; }
            set
            {
                this.__FLA_YMD = value;
                OnPropertyChanged("FLA_YMD");
            }
        }
        private string __FLA_YMD_FMT;
        public string FLA_YMD_FMT
        {
            get { return __FLA_YMD_FMT; }
            set
            {
                this.__FLA_YMD_FMT = value;
                OnPropertyChanged("FLA_YMD_FMT");
            }
        }
        private string __RPR_YMD;
        public string RPR_YMD
        {
            get { return __RPR_YMD; }
            set
            {
                this.__RPR_YMD = value;
                OnPropertyChanged("RPR_YMD");
            }
        }
        private string __RPR_YMD_FMT;
        public string RPR_YMD_FMT
        {
            get { return __RPR_YMD_FMT; }
            set
            {
                this.__RPR_YMD_FMT = value;
                OnPropertyChanged("RPR_YMD_FMT");
            }
        }
        private string __RPR_DES;
        public string RPR_DES
        {
            get { return __RPR_DES; }
            set
            {
                this.__RPR_DES = value;
                OnPropertyChanged("RPR_DES");
            }
        }
    }

}