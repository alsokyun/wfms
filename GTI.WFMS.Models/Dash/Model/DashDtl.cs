using GTI.WFMS.Models.Cmm.Model;
using System.ComponentModel;

namespace GTI.WFMS.Models.Dash.Model
{
    public class DashDtl : CmmDtl, INotifyPropertyChanged
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
        private decimal __DATA_VAL;
        public decimal DATA_VAL
        {
            get { return __DATA_VAL; }
            set
            {
                this.__DATA_VAL = value;
                OnPropertyChanged("DATA_VAL");
            }
        }
        private decimal __DATA_VAL2;
        public decimal DATA_VAL2
        {
            get { return __DATA_VAL2; }
            set
            {
                this.__DATA_VAL2 = value;
                OnPropertyChanged("DATA_VAL2");
            }
        }
        
    }
}
