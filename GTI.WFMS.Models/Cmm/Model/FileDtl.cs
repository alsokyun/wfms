using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Cmm.Model
{
    public class FileDtl : CmmDtl, INotifyPropertyChanged
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
        private decimal ?  __FIL_SEQ;
        public decimal ? FIL_SEQ
        {
            get { return __FIL_SEQ; }
            set
            {
                this.__FIL_SEQ = value;
                OnPropertyChanged("FIL_SEQ");
            }
        }
        private string __DWN_NAM;
        public string DWN_NAM
        {
            get { return __DWN_NAM; }
            set
            {
                this.__DWN_NAM = value;
                OnPropertyChanged("DWN_NAM");
            }
        }
        private string __UPF_NAM;
        public string UPF_NAM
        {
            get { return __UPF_NAM; }
            set
            {
                this.__UPF_NAM = value;
                OnPropertyChanged("UPF_NAM");
            }
        }
        private string __FIL_PTH;
        public string FIL_PTH
        {
            get { return __FIL_PTH; }
            set
            {
                this.__FIL_PTH = value;
                OnPropertyChanged("FIL_PTH");
            }
        }
        private string __FIL_TYP;
        public string FIL_TYP
        {
            get { return __FIL_TYP; }
            set
            {
                this.__FIL_TYP = value;
                OnPropertyChanged("FIL_TYP");
            }
        }
        private string __FIL_SIZ;
        public string FIL_SIZ
        {
            get { return __FIL_SIZ; }
            set
            {
                this.__FIL_SIZ = value;
                OnPropertyChanged("FIL_SIZ");
            }
        }
        private string __FIL_RST;
        public string FIL_RST
        {
            get { return __FIL_RST; }
            set
            {
                this.__FIL_RST = value;
                OnPropertyChanged("FIL_RST");
            }
        }
        private string __CUR_TFS;
        public string CUR_TFS
        {
            get { return __CUR_TFS; }
            set
            {
                this.__CUR_TFS = value;
                OnPropertyChanged("CUR_TFS");
            }
        }
        private string __CRE_YMD;
        public string CRE_YMD
        {
            get { return __CRE_YMD; }
            set
            {
                this.__CRE_YMD = value;
                OnPropertyChanged("CRE_YMD");
            }
        }
        private string __CRE_USR;
        public string CRE_USR
        {
            get { return __CRE_USR; }
            set
            {
                this.__CRE_USR = value;
                OnPropertyChanged("CRE_USR");
            }
        }
        private string __UPD_YMD;
        public string UPD_YMD
        {
            get { return __UPD_YMD; }
            set
            {
                this.__UPD_YMD = value;
                OnPropertyChanged("UPD_YMD");
            }
        }
        private string __UPD_USR;
        public string UPD_USR
        {
            get { return __UPD_USR; }
            set
            {
                this.__UPD_USR = value;
                OnPropertyChanged("UPD_USR");
            }
        }

        public int _seq { get; set; }
    }
}
