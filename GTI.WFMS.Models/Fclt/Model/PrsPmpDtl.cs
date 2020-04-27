using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Fclt.Model
{
    public class PrsPmpDtl : CmmDtl, INotifyPropertyChanged
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
        private string __SHT_NUM;
        public string SHT_NUM
        {
            get { return __SHT_NUM; }
            set
            {
                this.__SHT_NUM = value;
                OnPropertyChanged("SHT_NUM");
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
        private string __PRS_NAM;
        public string PRS_NAM
        {
            get { return __PRS_NAM; }
            set
            {
                this.__PRS_NAM = value;
                OnPropertyChanged("PRS_NAM");
            }
        }
        private decimal __PRS_ARA;
        public decimal PRS_ARA
        {
            get { return __PRS_ARA; }
            set
            {
                this.__PRS_ARA = value;
                OnPropertyChanged("PRS_ARA");
            }
        }
        private string __SAG_CDE;
        public string SAG_CDE
        {
            get { return __SAG_CDE; }
            set
            {
                this.__SAG_CDE = value;
                OnPropertyChanged("SAG_CDE");
            }
        }
        private string __SAG_NAM;
        public string SAG_NAM
        {
            get { return __SAG_NAM; }
            set
            {
                this.__SAG_NAM = value;
                OnPropertyChanged("SAG_NAM");
            }
        }
        private decimal __PRS_ALT;
        public decimal PRS_ALT
        {
            get { return __PRS_ALT; }
            set
            {
                this.__PRS_ALT = value;
                OnPropertyChanged("PRS_ALT");
            }
        }
        private decimal __PRS_VOL;
        public decimal PRS_VOL
        {
            get { return __PRS_VOL; }
            set
            {
                this.__PRS_VOL = value;
                OnPropertyChanged("PRS_VOL");
            }
        }
        private string __PRS_ARE;
        public string PRS_ARE
        {
            get { return __PRS_ARE; }
            set
            {
                this.__PRS_ARE = value;
                OnPropertyChanged("PRS_ARE");
            }
        }
        private decimal __PRS_SAH;
        public decimal PRS_SAH
        {
            get { return __PRS_SAH; }
            set
            {
                this.__PRS_SAH = value;
                OnPropertyChanged("PRS_SAH");
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
