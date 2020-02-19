using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class WttCostDt : CmmDtl, INotifyPropertyChanged
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
        private decimal __COST_SEQ;
        public decimal COST_SEQ
        {
            get { return __COST_SEQ; }
            set
            {
                this.__COST_SEQ = value;
                OnPropertyChanged("COST_SEQ");
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
        private int __PAY_NUM;
        public int PAY_NUM
        {
            get { return __PAY_NUM; }
            set
            {
                this.__PAY_NUM = value;
                OnPropertyChanged("PAY_NUM");
            }
        }
        private string __PTY_CDE;
        public string PTY_CDE
        {
            get { return __PTY_CDE; }
            set
            {
                this.__PTY_CDE = value;
                OnPropertyChanged("PTY_CDE");
            }
        }
        
        private string __PTY_NAM;
        public string PTY_NAM
        {
            get { return __PTY_NAM; }
            set
            {
                this.__PTY_NAM = value;
                OnPropertyChanged("PTY_NAM");
            }
        }
        private string __PAY_YMD;
        public string PAY_YMD
        {
            get { return __PAY_YMD; }
            set
            {
                this.__PAY_YMD = value;
                OnPropertyChanged("PAY_YMD");
            }
        }
        private string __PAY_YMD_FMT;
        public string PAY_YMD_FMT
        {
            get { return __PAY_YMD_FMT; }
            set
            {
                this.__PAY_YMD_FMT = value;
                OnPropertyChanged("PAY_YMD_FMT");
            }
        }
        
        private int __PAY_AMT;
        public int PAY_AMT
        {
            get { return __PAY_AMT; }
            set
            {
                this.__PAY_AMT = value;
                OnPropertyChanged("PAY_AMT");
            }
        }
    }

}