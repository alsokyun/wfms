using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class CnstDtl : CmmDtl, INotifyPropertyChanged
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
        private string __CNT_NAM;
        public string CNT_NAM
        {
            get { return __CNT_NAM; }
            set
            {
                this.__CNT_NAM = value;
                OnPropertyChanged("CNT_NAM");
            }
        }
        private string __CNT_LOC;
        public string CNT_LOC
        {
            get { return __CNT_LOC; }
            set
            {
                this.__CNT_LOC = value;
                OnPropertyChanged("CNT_LOC");
            }
        }
        private string __CNT_CDE;
        public string CNT_CDE
        {
            get { return __CNT_CDE; }
            set
            {
                this.__CNT_CDE = value;
                OnPropertyChanged("CNT_CDE");
            }
        }
        private string __CNT_CDE_NAM;
        public string CNT_CDE_NAM
        {
            get { return __CNT_CDE_NAM; }
            set
            {
                this.__CNT_CDE_NAM = value;
                OnPropertyChanged("CNT_CDE_NAM");
            }
        }
        private string __DSN_NAM;
        public string DSN_NAM
        {
            get { return __DSN_NAM; }
            set
            {
                this.__DSN_NAM = value;
                OnPropertyChanged("DSN_NAM");
            }
        }
        private decimal __DSN_AMT;
        public decimal DSN_AMT
        {
            get { return __DSN_AMT; }
            set
            {
                this.__DSN_AMT = value;
                OnPropertyChanged("DSN_AMT");
            }
        }
        private string __CTT_CDE;
        public string CTT_CDE
        {
            get { return __CTT_CDE; }
            set
            {
                this.__CTT_CDE = value;
                OnPropertyChanged("CTT_CDE");
            }
        }
        private string __CTT_NAM;
        public string CTT_NAM
        {
            get { return __CTT_NAM; }
            set
            {
                this.__CTT_NAM = value;
                OnPropertyChanged("CTT_NAM");
            }
        }
        private decimal __DPC_AMT;
        public decimal DPC_AMT
        {
            get { return __DPC_AMT; }
            set
            {
                this.__DPC_AMT = value;
                OnPropertyChanged("DPC_AMT");
            }
        }
        private decimal __DGV_AMT;
        public decimal DGV_AMT
        {
            get { return __DGV_AMT; }
            set
            {
                this.__DGV_AMT = value;
                OnPropertyChanged("DGV_AMT");
            }
        }
        private decimal __DET_AMT;
        public decimal DET_AMT
        {
            get { return __DET_AMT; }
            set
            {
                this.__DET_AMT = value;
                OnPropertyChanged("DET_AMT");
            }
        }
        private decimal __NAT_AMT;
        public decimal NAT_AMT
        {
            get { return __NAT_AMT; }
            set
            {
                this.__NAT_AMT = value;
                OnPropertyChanged("NAT_AMT");
            }
        }
        private decimal __COU_AMT;
        public decimal COU_AMT
        {
            get { return __COU_AMT; }
            set
            {
                this.__COU_AMT = value;
                OnPropertyChanged("COU_AMT");
            }
        }
        private decimal __CIT_AMT;
        public decimal CIT_AMT
        {
            get { return __CIT_AMT; }
            set
            {
                this.__CIT_AMT = value;
                OnPropertyChanged("CIT_AMT");
            }
        }
        private decimal __BND_AMT;
        public decimal BND_AMT
        {
            get { return __BND_AMT; }
            set
            {
                this.__BND_AMT = value;
                OnPropertyChanged("BND_AMT");
            }
        }
        private decimal __CSS_AMT;
        public decimal CSS_AMT
        {
            get { return __CSS_AMT; }
            set
            {
                this.__CSS_AMT = value;
                OnPropertyChanged("CSS_AMT");
            }
        }
        private string __KWN_EXP;
        public string KWN_EXP
        {
            get { return __KWN_EXP; }
            set
            {
                this.__KWN_EXP = value;
                OnPropertyChanged("KWN_EXP");
            }
        }
        private string __HNG_EXP;
        public string HNG_EXP
        {
            get { return __HNG_EXP; }
            set
            {
                this.__HNG_EXP = value;
                OnPropertyChanged("HNG_EXP");
            }
        }
        private string __SHN_EXP;
        public string SHN_EXP
        {
            get { return __SHN_EXP; }
            set
            {
                this.__SHN_EXP = value;
                OnPropertyChanged("SHN_EXP");
            }
        }
        private string __MOK_EXP;
        public string MOK_EXP
        {
            get { return __MOK_EXP; }
            set
            {
                this.__MOK_EXP = value;
                OnPropertyChanged("MOK_EXP");
            }
        }
        private string __BID_YMD;
        public string BID_YMD
        {
            get { return __BID_YMD; }
            set
            {
                this.__BID_YMD = value;
                OnPropertyChanged("BID_YMD");
            }
        }
        private decimal __EST_AMT;
        public decimal EST_AMT
        {
            get { return __EST_AMT; }
            set
            {
                this.__EST_AMT = value;
                OnPropertyChanged("EST_AMT");
            }
        }
        private string __CTT_YMD;
        public string CTT_YMD
        {
            get { return __CTT_YMD; }
            set
            {
                this.__CTT_YMD = value;
                OnPropertyChanged("CTT_YMD");
            }
        }
        private decimal __TCT_AMT;
        public decimal TCT_AMT
        {
            get { return __TCT_AMT; }
            set
            {
                this.__TCT_AMT = value;
                OnPropertyChanged("TCT_AMT");
            }
        }
        private decimal __CPC_AMT;
        public decimal CPC_AMT
        {
            get { return __CPC_AMT; }
            set
            {
                this.__CPC_AMT = value;
                OnPropertyChanged("CPC_AMT");
            }
        }
        private decimal __CGV_AMT;
        public decimal CGV_AMT
        {
            get { return __CGV_AMT; }
            set
            {
                this.__CGV_AMT = value;
                OnPropertyChanged("CGV_AMT");
            }
        }
        private decimal __CET_AMT;
        public decimal CET_AMT
        {
            get { return __CET_AMT; }
            set
            {
                this.__CET_AMT = value;
                OnPropertyChanged("CET_AMT");
            }
        }
        private string __GCN_NAM;
        public string GCN_NAM
        {
            get { return __GCN_NAM; }
            set
            {
                this.__GCN_NAM = value;
                OnPropertyChanged("GCN_NAM");
            }
        }
        private string __POC_NAM;
        public string POC_NAM
        {
            get { return __POC_NAM; }
            set
            {
                this.__POC_NAM = value;
                OnPropertyChanged("POC_NAM");
            }
        }
        private string __GCN_ADR;
        public string GCN_ADR
        {
            get { return __GCN_ADR; }
            set
            {
                this.__GCN_ADR = value;
                OnPropertyChanged("GCN_ADR");
            }
        }
        private string __GCN_TEL;
        public string GCN_TEL
        {
            get { return __GCN_TEL; }
            set
            {
                this.__GCN_TEL = value;
                OnPropertyChanged("GCN_TEL");
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
        private string __SVS_NSM;
        public string SVS_NSM
        {
            get { return __SVS_NSM; }
            set
            {
                this.__SVS_NSM = value;
                OnPropertyChanged("SVS_NSM");
            }
        }
        private string __RFN_YMD;
        public string RFN_YMD
        {
            get { return __RFN_YMD; }
            set
            {
                this.__RFN_YMD = value;
                OnPropertyChanged("RFN_YMD");
            }
        }
        private string __FCH_YMD;
        public string FCH_YMD
        {
            get { return __FCH_YMD; }
            set
            {
                this.__FCH_YMD = value;
                OnPropertyChanged("FCH_YMD");
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
        private string __CNT_DES;
        public string CNT_DES
        {
            get { return __CNT_DES; }
            set
            {
                this.__CNT_DES = value;
                OnPropertyChanged("CNT_DES");
            }
        }
        private string __GVR_DES;
        public string GVR_DES
        {
            get { return __GVR_DES; }
            set
            {
                this.__GVR_DES = value;
                OnPropertyChanged("GVR_DES");
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