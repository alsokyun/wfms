using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class PdjtInDtl : CmmDtl, INotifyPropertyChanged
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
                if (!propertyName.Equals("CHK"))
                {
                    this.CHK = "Y";
                }
            }
        }



        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private decimal __IN_NUM;
        public decimal IN_NUM
        {
            get { return __IN_NUM; }
            set
            {
                this.__IN_NUM = value;
                OnPropertyChanged("IN_NUM");
            }
        }
        private decimal __PDH_NUM;
        public decimal PDH_NUM
        {
            get { return __PDH_NUM; }
            set
            {
                this.__PDH_NUM = value;
                OnPropertyChanged("PDH_NUM");
            }
        }
        private int? __IN_AMT;
        public int? IN_AMT
        {
            get { return __IN_AMT; }
            set
            {
                this.__IN_AMT = value;
                OnPropertyChanged("IN_AMT");
            }
        }
        private string __IN_YMD;
        public string IN_YMD
        {
            get { return __IN_YMD; }
            set
            {
                this.__IN_YMD = value;
                OnPropertyChanged("IN_YMD");
            }
        }
        private string __IN_ETC;
        public string IN_ETC
        {
            get { return __IN_ETC; }
            set
            {
                this.__IN_ETC = value;
                OnPropertyChanged("IN_ETC");
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
    }
}