using System.ComponentModel;

namespace GTI.WFMS.Models.Cmm.Model
{
    public class LinkWttMetaHt : CmmDtl
    {
      
        

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
                OnPropertyChanged("RNO");
            }
        }
        private decimal __META_SEQ;
        public decimal META_SEQ
        {
            get { return __META_SEQ; }
            set
            {
                this.__META_SEQ = value;
                OnPropertyChanged("META_SEQ");
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
        private string __BJD_CDE;
        public string BJD_CDE
        {
            get { return __BJD_CDE; }
            set
            {
                this.__BJD_CDE = value;
                OnPropertyChanged("BJD_CDE");
            }
        }
        private string __BJD_NAM;
        public string BJD_NAM
        {
            get { return __BJD_NAM; }
            set
            {
                this.__BJD_NAM = value;
                OnPropertyChanged("BJD_NAM");
            }
        }
        private string __HOM_NAM;
        public string HOM_NAM
        {
            get { return __HOM_NAM; }
            set
            {
                this.__HOM_NAM = value;
                OnPropertyChanged("HOM_NAM");
            }
        }
        private string __HOM_ADR;
        public string HOM_ADR
        {
            get { return __HOM_ADR; }
            set
            {
                this.__HOM_ADR = value;
                OnPropertyChanged("HOM_ADR");
            }
        }
        private int __CHG_NUM;
        public int CHG_NUM
        {
            get { return __CHG_NUM; }
            set
            {
                this.__CHG_NUM = value;
                OnPropertyChanged("CHG_NUM");
            }
        }
        private string __CHG_NAM;
        public string CHG_NAM
        {
            get { return __CHG_NAM; }
            set
            {
                this.__CHG_NAM = value;
                OnPropertyChanged("CHG_NAM");
            }
        }
        private string __CHG_YMD;
        public string CHG_YMD
        {
            get { return __CHG_YMD; }
            set
            {
                this.__CHG_YMD = value;
                OnPropertyChanged("CHG_YMD");
            }
        }
        private string __GCW_CDE;
        public string GCW_CDE
        {
            get { return __GCW_CDE; }
            set
            {
                this.__GCW_CDE = value;
                OnPropertyChanged("GCW_CDE");
            }
        }
        private string __GCW_NAM;
        public string GCW_NAM
        {
            get { return __GCW_NAM; }
            set
            {
                this.__GCW_NAM = value;
                OnPropertyChanged("GCW_NAM");
            }
        }
        private string __CRS_CDE;
        public string CRS_CDE
        {
            get { return __CRS_CDE; }
            set
            {
                this.__CRS_CDE = value;
                OnPropertyChanged("CRS_CDE");
            }
        }
        private string __CRS_NAM;
        public string CRS_NAM
        {
            get { return __CRS_NAM; }
            set
            {
                this.__CRS_NAM = value;
                OnPropertyChanged("CRS_NAM");
            }
        }
        private string __OME_NAM;
        public string OME_NAM
        {
            get { return __OME_NAM; }
            set
            {
                this.__OME_NAM = value;
                OnPropertyChanged("OME_NAM");
            }
        }
        private decimal? __OME_DIP;
        public decimal? OME_DIP
        {
            get { return __OME_DIP; }
            set
            {
                this.__OME_DIP = value;
                OnPropertyChanged("OME_DIP");
            }
        }
        private string __OME_MOF;
        public string OME_MOF
        {
            get { return __OME_MOF; }
            set
            {
                this.__OME_MOF = value;
                OnPropertyChanged("OME_MOF");
            }
        }
        private string __OME_MOF_NAM;
        public string OME_MOF_NAM
        {
            get { return __OME_MOF_NAM; }
            set
            {
                this.__OME_MOF_NAM = value;
                OnPropertyChanged("OME_MOF_NAM");
            }
        }
        private string __OME_TYP;
        public string OME_TYP
        {
            get { return __OME_TYP; }
            set
            {
                this.__OME_TYP = value;
                OnPropertyChanged("OME_TYP");
            }
        }
        private string __OME_COL;
        public string OME_COL
        {
            get { return __OME_COL; }
            set
            {
                this.__OME_COL = value;
                OnPropertyChanged("OME_COL");
            }
        }
        private string __OME_SEL;
        public string OME_SEL
        {
            get { return __OME_SEL; }
            set
            {
                this.__OME_SEL = value;
                OnPropertyChanged("OME_SEL");
            }
        }
        private string __OME_VAL;
        public string OME_VAL
        {
            get { return __OME_VAL; }
            set
            {
                this.__OME_VAL = value;
                OnPropertyChanged("OME_VAL");
            }
        }
        private int? __OME_CNT;
        public int? OME_CNT
        {
            get { return __OME_CNT; }
            set
            {
                this.__OME_CNT = value;
                OnPropertyChanged("OME_CNT");
            }
        }
        private string __IME_NAM;
        public string IME_NAM
        {
            get { return __IME_NAM; }
            set
            {
                this.__IME_NAM = value;
                OnPropertyChanged("IME_NAM");
            }
        }
        private decimal? __IME_DIP;
        public decimal? IME_DIP
        {
            get { return __IME_DIP; }
            set
            {
                this.__IME_DIP = value;
                OnPropertyChanged("IME_DIP");
            }
        }
        private string __IME_MOF;
        public string IME_MOF
        {
            get { return __IME_MOF; }
            set
            {
                this.__IME_MOF = value;
                OnPropertyChanged("IME_MOF");
            }
        }
        private string __IME_TYP;
        public string IME_TYP
        {
            get { return __IME_TYP; }
            set
            {
                this.__IME_TYP = value;
                OnPropertyChanged("IME_TYP");
            }
        }
        private string __IME_TYPE_NAM;
        public string IME_TYPE_NAM
        {
            get { return __IME_TYPE_NAM; }
            set
            {
                this.__IME_TYPE_NAM = value;
                OnPropertyChanged("IME_TYPE_NAM");
            }
        }        
        private string __IME_SEL;
        public string IME_SEL
        {
            get { return __IME_SEL; }
            set
            {
                this.__IME_SEL = value;
                OnPropertyChanged("IME_SEL");
            }
        }
        private string __IME_VAL;
        public string IME_VAL
        {
            get { return __IME_VAL; }
            set
            {
                this.__IME_VAL = value;
                OnPropertyChanged("IME_VAL");
            }
        }
        private string __IME_COL;
        public string IME_COL
        {
            get { return __IME_COL; }
            set
            {
                this.__IME_COL = value;
                OnPropertyChanged("IME_COL");
            }
        }
        private int __OME_NUM;
        public int OME_NUM
        {
            get { return __OME_NUM; }
            set
            {
                this.__OME_NUM = value;
                OnPropertyChanged("OME_NUM");
            }
        }
        private string __IME_NUM;
        public string IME_NUM
        {
            get { return __IME_NUM; }
            set
            {
                this.__IME_NUM = value;
                OnPropertyChanged("IME_NUM");
            }
        }
        private string __MOF_CDE;
        public string MOF_CDE
        {
            get { return __MOF_CDE; }
            set
            {
                this.__MOF_CDE = value;
                OnPropertyChanged("MOF_CDE");
            }
        }
        private string __MOF_NAM;
        public string MOF_NAM
        {
            get { return __MOF_NAM; }
            set
            {
                this.__MOF_NAM = value;
                OnPropertyChanged("MOF_NAM");
            }
        }
        private string __ATT_USR;
        public string ATT_USR
        {
            get { return __ATT_USR; }
            set
            {
                this.__ATT_USR = value;
                OnPropertyChanged("ATT_USR");
            }
        }
        private string __ATT_TIM;
        public string ATT_TIM
        {
            get { return __ATT_TIM; }
            set
            {
                this.__ATT_TIM = value;
                OnPropertyChanged("ATT_TIM");
            }
        }
   
    }
}
