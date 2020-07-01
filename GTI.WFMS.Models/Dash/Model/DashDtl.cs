using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Dash.Model
{
    public class DashDtl : CmmDtl
    {
        

        /// <summary>                                                                 
        /// 프로퍼티 부분                                                             
        /// </summary>                                                                
        private string __NAM;
        public string NAM
        {
            get { return __NAM; }
            set
            {
                this.__NAM = value;
                OnPropertyChanged("NAM");
            }
        }       
        private decimal ?  __DATA_VAL;
        public decimal ? DATA_VAL
        {
            get { return __DATA_VAL; }
            set
            {
                this.__DATA_VAL = value;
                OnPropertyChanged("DATA_VAL");
            }
        }
        private decimal ?  __DATA_VAL2;
        public decimal ? DATA_VAL2
        {
            get { return __DATA_VAL2; }
            set
            {
                this.__DATA_VAL2 = value;
                OnPropertyChanged("DATA_VAL2");
            }
        }

        private string __USER_ID;
        public string USER_ID
        {
            get { return __USER_ID; }
            set
            {
                this.__USER_ID = value;
                OnPropertyChanged("USER_ID");
            }
        }

        private string __MNU_CD;
        public string MNU_CD
        {
            get { return __MNU_CD; }
            set
            {
                this.__MNU_CD = value;
                OnPropertyChanged("MNU_CD");
            }
        }

        private string __MNU_NM;
        public string MNU_NM
        {
            get { return __MNU_NM; }
            set
            {
                this.__MNU_NM = value;
                OnPropertyChanged("MNU_NM");
            }
        }

    }
}
