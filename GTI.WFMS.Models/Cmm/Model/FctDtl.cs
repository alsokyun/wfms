using GTI.WFMS.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Cmm.Model
{
    public class FctDtl : INotifyPropertyChanged
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
            get 
            {
                if (FmsUtil.IsNull(__IST_YMD))
                {
                    return "";
                }
                try
                {
                    return Convert.ToDateTime(__IST_YMD).ToString("yyyy-MM-dd");
                }
                catch (Exception) { }
                try
                {
                    return DateTime.ParseExact(__IST_YMD,"yyyyMMdd",null).ToString("yyyy-MM-dd");
                }
                catch (Exception) { }
                return "";
            }
            set
            {
                this.__IST_YMD = value;
                OnPropertyChanged("IST_YMD");
            }
        }

        private string __MOP_CDE;
        public string MOP_CDE
        {
            get { return __MOP_CDE; }
            set
            {
                this.__MOP_CDE = value;
                OnPropertyChanged("MOP_CDE");
            }
        }
        private string __MOP_NAM;
        public string MOP_NAM
        {
            get { return __MOP_NAM; }
            set
            {
                this.__MOP_NAM = value;
                OnPropertyChanged("MOP_NAM");
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
