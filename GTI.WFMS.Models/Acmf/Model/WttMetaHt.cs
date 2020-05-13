using GTI.WFMS.Models.Cmm.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Acmf.Model
{
    public class WttMetaHt : CmmDtl, INotifyPropertyChanged
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
        private decimal __CHG_NUM;
        public decimal CHG_NUM
        {
            get { return __CHG_NUM; }
            set
            {
                this.__CHG_NUM = value;
                OnPropertyChanged("CHG_NUM");
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
        private string __OME_NUM;
        public string OME_NUM
        {
            get { return __OME_NUM; }
            set
            {
                this.__OME_NUM = value;
                OnPropertyChanged("OME_NUM");
            }
        }
        private decimal __OME_DIP;
        public decimal OME_DIP
        {
            get { return __OME_DIP; }
            set
            {
                this.__OME_DIP = value;
                OnPropertyChanged("OME_DIP");
            }
        }
        private decimal __OME_CNT;
        public decimal OME_CNT
        {
            get { return __OME_CNT; }
            set
            {
                this.__OME_CNT = value;
                OnPropertyChanged("OME_CNT");
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

    }
}