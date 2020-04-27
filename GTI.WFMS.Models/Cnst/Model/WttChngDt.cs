using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Cnst.Model
{
    public class WttChngDt : CmmDtl, INotifyPropertyChanged
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
            //컬럼변경시 체크박스 
            if (propertyName != "CHK")
            {
                this.CHK = "Y";
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
        private decimal __CHNG_SEQ;
        public decimal CHNG_SEQ
        {
            get { return __CHNG_SEQ; }
            set
            {
                this.__CHNG_SEQ = value;
                OnPropertyChanged("CHNG_SEQ");
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
        private string __CHG_YMD_FMT;
        public string CHG_YMD_FMT
        {
            get { return __CHG_YMD_FMT; }
            set
            {
                this.__CHG_YMD_FMT = value;
                OnPropertyChanged("CHG_YMD_FMT");
            }
        }
        private int __INC_AMT;
        public int INC_AMT
        {
            get { return __INC_AMT; }
            set
            {
                this.__INC_AMT = value;
                OnPropertyChanged("INC_AMT");
            }
        }
        private int __IGV_AMT;
        public int IGV_AMT
        {
            get { return __IGV_AMT; }
            set
            {
                this.__IGV_AMT = value;
                OnPropertyChanged("IGV_AMT");
            }
        }
        private int __CHG_AMT;
        public int CHG_AMT
        {
            get { return __CHG_AMT; }
            set
            {
                this.__CHG_AMT = value;
                OnPropertyChanged("CHG_AMT");
            }
        }
        private string __CHG_DES;
        public string CHG_DES
        {
            get { return __CHG_DES; }
            set
            {
                this.__CHG_DES = value;
                OnPropertyChanged("CHG_DES");
            }
        }
        private string __CGV_DES;
        public string CGV_DES
        {
            get { return __CGV_DES; }
            set
            {
                this.__CGV_DES = value;
                OnPropertyChanged("CGV_DES");
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
        private string __ATT_TIM_FMT;
        public string ATT_TIM_FMT
        {
            get { return __ATT_TIM_FMT; }
            set
            {
                this.__ATT_TIM_FMT = value;
                OnPropertyChanged("ATT_TIM_FMT");
            }
        }
    }

}