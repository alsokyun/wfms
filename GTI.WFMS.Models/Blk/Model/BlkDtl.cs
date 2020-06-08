using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Models.Blk.Model
{
    public class BlkDtl : CmmDtl
    {

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
        private string __BLK_NM;
        public string BLK_NM
        {
            get { return __BLK_NM; }
            set
            {
                this.__BLK_NM = value;
                OnPropertyChanged("BLK_NM");
            }
        }
        private string __WSUPP_LINE_NM;
        public string WSUPP_LINE_NM
        {
            get { return __WSUPP_LINE_NM; }
            set
            {
                this.__WSUPP_LINE_NM = value;
                OnPropertyChanged("WSUPP_LINE_NM");
            }
        }
        private int? __MAX_SUPP_QTY;
        public int? MAX_SUPP_QTY
        {
            get { return __MAX_SUPP_QTY; }
            set
            {
                this.__MAX_SUPP_QTY = value;
                OnPropertyChanged("MAX_SUPP_QTY");
            }
        }
        private int? __WSUPP_PEPL_CNT;
        public int? WSUPP_PEPL_CNT
        {
            get { return __WSUPP_PEPL_CNT; }
            set
            {
                this.__WSUPP_PEPL_CNT = value;
                OnPropertyChanged("WSUPP_PEPL_CNT");
            }
        }
        private int? __FAM_CNT;
        public int? FAM_CNT
        {
            get { return __FAM_CNT; }
            set
            {
                this.__FAM_CNT = value;
                OnPropertyChanged("FAM_CNT");
            }
        }
        //private string __EDT_DT;
        //public string EDT_DT
        //{
        //    get { return __EDT_DT; }
        //    set
        //    {
        //        this.__EDT_DT = value;
        //        OnPropertyChanged("EDT_DT");
        //    }
        //}
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
    }
}