using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Pipe.Model
{
    public class WtsMnhoDtl : CmmDtl, INotifyPropertyChanged
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
        private string __IST_YMD;
        public string IST_YMD
        {
            get { return __IST_YMD; }
            set
            {
                this.__IST_YMD = value;
                OnPropertyChanged("IST_YMD");
            }
        }
        private string __MAN_STD;
        public string MAN_STD
        {
            get { return __MAN_STD; }
            set
            {
                this.__MAN_STD = value;
                OnPropertyChanged("MAN_STD");
            }
        }
        private string __SOM_CDE;
        public string SOM_CDE
        {
            get { return __SOM_CDE; }
            set
            {
                this.__SOM_CDE = value;
                OnPropertyChanged("SOM_CDE");
            }
        }
        private string __SOM_NAM;
        public string SOM_NAM
        {
            get { return __SOM_NAM; }
            set
            {
                this.__SOM_NAM = value;
                OnPropertyChanged("SOM_NAM");
            }
        }
        private string __MHS_CDE;
        public string MHS_CDE
        {
            get { return __MHS_CDE; }
            set
            {
                this.__MHS_CDE = value;
                OnPropertyChanged("MHS_CDE");
            }
        }
        private string __MHS_NAM;
        public string MHS_NAM
        {
            get { return __MHS_NAM; }
            set
            {
                this.__MHS_NAM = value;
                OnPropertyChanged("MHS_NAM");
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
        private decimal ?  __ANG_DIR;
        public decimal ? ANG_DIR
        {
            get { return __ANG_DIR; }
            set
            {
                this.__ANG_DIR = value;
                OnPropertyChanged("ANG_DIR");
            }
        }
    }
}
