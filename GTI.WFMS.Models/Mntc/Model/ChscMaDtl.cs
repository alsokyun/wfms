using DevExpress.Mvvm.POCO;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using System;
using System.ComponentModel;
using System.Globalization;

namespace GTI.WFMS.Models.Mntc.Model
{
    public class ChscMaDtl : CmmDtl
    {
        /// <summary>
        /// 인터페이스 구현부분
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        Date3StrConverter date3StrConverter = new Date3StrConverter();


        // dxmvvm 모델형태로 생성자 구현
        //public ChscMaDtl()
        //{
        //    ViewModelSource.Create(() => new ChscMaDtl());
        //}

        /// 스케줄 기본프로퍼티
        public virtual bool AllDay { get; set; }
        public virtual string RecurrenceInfo { get; set; }
        public virtual string ReminderInfo { get; set; }
        private int?  resourceId;
        public virtual int?  ResourceId
        {
            get { return 1; }
            set { this.resourceId = value; }
        }
        public virtual int Type { get; set; }



        private DateTime __StartTime;
        public virtual DateTime StartTime
        {
            get
            {
                try
                {
                    __StartTime = DateTime.ParseExact(STA_YMD, "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                catch (Exception){}
                return __StartTime;
            }
            set
            {
                this.__StartTime = value;
                RaisePropertyChanged("StartTime");
                this.STA_YMD = __StartTime.ToString("yyyyMMdd");
            }
        }
        private string __STA_YMD;
        public string STA_YMD
        {
            get { return __STA_YMD; }
            set
            {
                this.__STA_YMD = value;
                RaisePropertyChanged("STA_YMD");
            }
        }

        private DateTime __EndTime;
        public virtual DateTime EndTime
        {
            get
            {
                try
                {
                    __EndTime = DateTime.ParseExact(END_YMD, "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                catch (Exception){}
                return __EndTime;
            }
            set
            {
                this.__EndTime = value;
                RaisePropertyChanged("EndTime");
                this.END_YMD = __EndTime.ToString("yyyyMMdd");
            }
        }
        private string __END_YMD;
        public string END_YMD
        {
            get { return __END_YMD; }
            set
            {
                this.__END_YMD = value;
                RaisePropertyChanged("END_YMD");
            }
        }



        /// <summary>
        /// 프로퍼티 부분
        /// </summary>
        private decimal ?  __SCL_NUM;
        public decimal ? SCL_NUM
        {
            get { return __SCL_NUM; }
            set
            {
                this.__SCL_NUM = value;
                RaisePropertyChanged("SCL_NUM");
            }
        }
        private string __SCL_CDE;
        public string SCL_CDE
        {
            get { return __SCL_CDE; }
            set
            {
                this.__SCL_CDE = value;
                RaisePropertyChanged("SCL_CDE");
            }
        }
        private string __SCL_NM;
        public string SCL_NM
        {
            get { return __SCL_NM; }
            set
            {
                this.__SCL_NM = value;
                RaisePropertyChanged("SCL_NM");
            }
        }
        private string __SCL_STAT_CDE;
        public string SCL_STAT_CDE
        {
            get { return __SCL_STAT_CDE; }
            set
            {
                this.__SCL_STAT_CDE = value;
                RaisePropertyChanged("SCL_STAT_CDE");
            }
        }
        private string __SCL_STAT_NM;
        public string SCL_STAT_NM
        {
            get { return __SCL_STAT_NM; }
            set
            {
                this.__SCL_STAT_NM = value;
                RaisePropertyChanged("SCL_STAT_NM");
            }
        }
        private string __COLOR;
        public string COLOR
        {
            get { return __COLOR; }
            set
            {
                this.__COLOR = value;
                RaisePropertyChanged("COLOR");
            }
        }
        private string __MNG_CDE;
        public string MNG_CDE
        {
            get { return __MNG_CDE; }
            set
            {
                this.__MNG_CDE = value;
                RaisePropertyChanged("MNG_CDE");
            }
        }
        private string __TIT_NAM;
        public string TIT_NAM
        {
            get { return __TIT_NAM; }
            set
            {
                this.__TIT_NAM = value;
                RaisePropertyChanged("TIT_NAM");
            }
        }
        private string __CKM_GRP_NM;
        public string CKM_GRP_NM
        {
            get { return __CKM_GRP_NM; }
            set
            {
                this.__CKM_GRP_NM = value;
                RaisePropertyChanged("CKM_GRP_NM");
            }
        }
        private string __CKM_PEO;
        public string CKM_PEO
        {
            get { return __CKM_PEO; }
            set
            {
                this.__CKM_PEO = value;
                RaisePropertyChanged("CKM_PEO");
            }
        }
        private string __CHK_CTNT;
        public string CHK_CTNT
        {
            get { return __CHK_CTNT; }
            set
            {
                this.__CHK_CTNT = value;
                RaisePropertyChanged("CHK_CTNT");
            }
        }
        private string __CHK_APR_YMD;
        public string CHK_APR_YMD
        {
            get
            {
                try
                {
                    return date3StrConverter.Convert(__CHK_APR_YMD, null, null, null) as string ; //yyyy-MM-dd 형태변환
                }
                catch (Exception)
                {
                    return __CHK_APR_YMD;
                }
            }
            set
            {
                try
                {
                    this.__CHK_APR_YMD = date3StrConverter.ConvertBack(value, null, null, null) as string; //yyyyMMdd 형태변환
                }
                catch (Exception)
                {
                    this.__CHK_APR_YMD = value;
                }
                RaisePropertyChanged("CHK_APR_YMD");
            }
        }
        private string __CHK_APR_USR;
        public string CHK_APR_USR
        {
            get { return __CHK_APR_USR; }
            set
            {
                this.__CHK_APR_USR = value;
                RaisePropertyChanged("CHK_APR_USR");
            }
        }
        private string __CHK_CMP_YMD;
        public string CHK_CMP_YMD
        {
            get
            {
                try
                {
                    return date3StrConverter.Convert(__CHK_CMP_YMD, null, null, null) as string; //yyyy-MM-dd 형태변환
                }
                catch (Exception)
                {
                    return __CHK_CMP_YMD;
                }
            }
            set
            {
                try
                {
                    this.__CHK_CMP_YMD = date3StrConverter.ConvertBack(value, null, null, null) as string; //yyyyMMdd 형태변환
                }
                catch (Exception)
                {
                    this.__CHK_CMP_YMD = value;
                }
                RaisePropertyChanged("CHK_CMP_YMD");
            }
        }
        private string __CHK_RESULT_YMD;
        public string CHK_RESULT_YMD
        {
            get
            {
                try
                {
                    return date3StrConverter.Convert(__CHK_RESULT_YMD, null, null, null) as string; //yyyy-MM-dd 형태변환
                }
                catch (Exception)
                {
                    return __CHK_RESULT_YMD;
                }
            }
            set
            {
                try
                {
                    this.__CHK_RESULT_YMD = date3StrConverter.ConvertBack(value, null, null, null) as string; //yyyyMMdd 형태변환
                }
                catch (Exception)
                {
                    this.__CHK_RESULT_YMD = value;
                }
                RaisePropertyChanged("CHK_RESULT_YMD");
            }
        }
        private string __USER_NM;
        public string USER_NM
        {
            get { return __USER_NM; }
            set
            {
                this.__USER_NM = value;
                RaisePropertyChanged("USER_NM");
            }
        }
        private string __CRE_YMD;
        public string CRE_YMD
        {
            get { return __CRE_YMD; }
            set
            {
                this.__CRE_YMD = value;
                RaisePropertyChanged("CRE_YMD");
            }
        }
        private string __CRE_USR;
        public string CRE_USR
        {
            get { return __CRE_USR; }
            set
            {
                this.__CRE_USR = value;
                RaisePropertyChanged("CRE_USR");
            }
        }
        private string __UPD_YMD;
        public string UPD_YMD
        {
            get { return __UPD_YMD; }
            set
            {
                this.__UPD_YMD = value;
                RaisePropertyChanged("UPD_YMD");
            }
        }
        private string __UPD_USR;
        public string UPD_USR
        {
            get { return __UPD_USR; }
            set
            {
                this.__UPD_USR = value;
                RaisePropertyChanged("UPD_USR");
            }
        }
        private string __CHK_GRP_NM;
        public string CHK_GRP_NM
        {
            get { return __CHK_GRP_NM; }
            set
            {
                this.__CHK_GRP_NM = value;
                RaisePropertyChanged("CHK_GRP_NM");
            }
        }


    }
}