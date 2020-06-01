using GTI.WFMS.Models.Cmm.Model;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class SplyDtl : CmmDtl
    {


        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
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
        private string __BEG_YMD;
        public string BEG_YMD
        {
            get { return __BEG_YMD; }
            set
            {
                this.__BEG_YMD = value;
                OnPropertyChanged("BEG_YMD");
            }
        }
        private string __FNS_YMD;
        public string FNS_YMD
        {
            get { return __FNS_YMD; }
            set
            {
                this.__FNS_YMD = value;
                OnPropertyChanged("FNS_YMD");
            }
        }
        private int ?  __GVR_AMT;
        public int ? GVR_AMT
        {
            get { return __GVR_AMT; }
            set
            {
                this.__GVR_AMT = value;
                OnPropertyChanged("GVR_AMT");
            }
        }
        private int ?  __PRV_AMT;
        public int ? PRV_AMT
        {
            get { return __PRV_AMT; }
            set
            {
                this.__PRV_AMT = value;
                OnPropertyChanged("PRV_AMT");
            }
        }
        private int ?  __TAX_AMT;
        public int ? TAX_AMT
        {
            get { return __TAX_AMT; }
            set
            {
                this.__TAX_AMT = value;
                OnPropertyChanged("TAX_AMT");
            }
        }
        private int ?  __ROR_AMT;
        public int ? ROR_AMT
        {
            get { return __ROR_AMT; }
            set
            {
                this.__ROR_AMT = value;
                OnPropertyChanged("ROR_AMT");
            }
        }
        private int ?  __DEF_AMT;
        public int ? DEF_AMT
        {
            get { return __DEF_AMT; }
            set
            {
                this.__DEF_AMT = value;
                OnPropertyChanged("DEF_AMT");
            }
        }
        private int ?  __GFE_AMT;
        public int ? GFE_AMT
        {
            get { return __GFE_AMT; }
            set
            {
                this.__GFE_AMT = value;
                OnPropertyChanged("GFE_AMT");
            }
        }
        private int ?  __FFE_AMT;
        public int ? FFE_AMT
        {
            get { return __FFE_AMT; }
            set
            {
                this.__FFE_AMT = value;
                OnPropertyChanged("FFE_AMT");
            }
        }
        private int ?  __DIV_AMT;
        public int ? DIV_AMT
        {
            get { return __DIV_AMT; }
            set
            {
                this.__DIV_AMT = value;
                OnPropertyChanged("DIV_AMT");
            }
        }
        private int ?  __ETC_AMT;
        public int ? ETC_AMT
        {
            get { return __ETC_AMT; }
            set
            {
                this.__ETC_AMT = value;
                OnPropertyChanged("ETC_AMT");
            }
        }
        private int ?  __TOT_AMT;
        public int ? TOT_AMT
        {
            get { return __TOT_AMT; }
            set
            {
                this.__TOT_AMT = value;
                OnPropertyChanged("TOT_AMT");
            }
        }
        private string __RCP_YMD;
        public string RCP_YMD
        {
            get { return __RCP_YMD; }
            set
            {
                this.__RCP_YMD = value;
                OnPropertyChanged("RCP_YMD");
            }
        }
        private string __OPR_NAM;
        public string OPR_NAM
        {
            get { return __OPR_NAM; }
            set
            {
                this.__OPR_NAM = value;
                OnPropertyChanged("OPR_NAM");
            }
        }
        private string __SVS_NAM;
        public string SVS_NAM
        {
            get { return __SVS_NAM; }
            set
            {
                this.__SVS_NAM = value;
                OnPropertyChanged("SVS_NAM");
            }
        }
        private string __FNS_NAM;
        public string FNS_NAM
        {
            get { return __FNS_NAM; }
            set
            {
                this.__FNS_NAM = value;
                OnPropertyChanged("FNS_NAM");
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
        private int ?  __DFE_AMT;
        public int ? DFE_AMT
        {
            get { return __DFE_AMT; }
            set
            {
                this.__DFE_AMT = value;
                OnPropertyChanged("DFE_AMT");
            }
        }
        private string __FCH_NAM;
        public string FCH_NAM
        {
            get { return __FCH_NAM; }
            set
            {
                this.__FCH_NAM = value;
                OnPropertyChanged("FCH_NAM");
            }
        }

        private string __SUR_NAM;
        public string SUR_NAM
        {
            get { return __SUR_NAM; }
            set
            {
                this.__SUR_NAM = value;
                OnPropertyChanged("SUR_NAM");
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

    }

}