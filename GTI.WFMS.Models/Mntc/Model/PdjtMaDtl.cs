using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class PdjtMaDtl : CmmDtl, INotifyPropertyChanged
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
                
                //컬럼변경시 체크박스 
                if (propertyName != "CHK")
                {
                    this.CHK = "Y";
                }
            }
        }



        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private decimal ?  __PDH_NUM;
        public decimal ? PDH_NUM
        {
            get { return __PDH_NUM; }
            set
            {
                this.__PDH_NUM = value;
                OnPropertyChanged("PDH_NUM");
            }
        }
        private string __PDT_CAT_CDE;
        public string PDT_CAT_CDE
        {
            get { return __PDT_CAT_CDE; }
            set
            {
                this.__PDT_CAT_CDE = value;
                OnPropertyChanged("PDT_CAT_CDE");
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
        private string __PDE_NAM;
        public string PDE_NAM
        {
            get { return __PDE_NAM; }
            set
            {
                this.__PDE_NAM = value;
                OnPropertyChanged("PDE_NAM");
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
        private string __PDT_MNF;
        public string PDT_MNF
        {
            get { return __PDT_MNF; }
            set
            {
                this.__PDT_MNF = value;
                OnPropertyChanged("PDT_MNF");
            }
        }
        private string __PDT_MDL;
        public string PDT_MDL
        {
            get { return __PDT_MDL; }
            set
            {
                this.__PDT_MDL = value;
                OnPropertyChanged("PDT_MDL");
            }
        }
        private string __PDT_UNT;
        public string PDT_UNT
        {
            get { return __PDT_UNT; }
            set
            {
                this.__PDT_UNT = value;
                OnPropertyChanged("PDT_UNT");
            }
        }
        private string __USE_YN;
        public string USE_YN
        {
            get { return __USE_YN; }
            set
            {
                this.__USE_YN = value;
                OnPropertyChanged("USE_YN");
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
        private string __UDT_YMD;
        public string UDT_YMD
        {
            get { return __UDT_YMD; }
            set
            {
                this.__UDT_YMD = value;
                OnPropertyChanged("UDT_YMD");
            }
        }
        private string __UDT_USR;
        public string UDT_USR
        {
            get { return __UDT_USR; }
            set
            {
                this.__UDT_USR = value;
                OnPropertyChanged("UDT_USR");
            }
        }
        private string __CHK;
        public string CHK
        {
            get { return __CHK; }
            set
            {
                this.__CHK = value;
                OnPropertyChanged("CHK");
            }
        }
        private decimal ?  __CNT;
        public decimal ? CNT
        {
            get { return __CNT; }
            set
            {
                this.__CNT = value;
                OnPropertyChanged("CNT");
            }
        }
    }
}