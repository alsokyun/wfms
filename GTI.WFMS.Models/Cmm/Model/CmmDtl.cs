using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Models.Cmm.Model
{
    /// <summary>
    /// 뷰모델의 공통 클래스 - 사용자ID 등 기본속성
    /// </summary>
    public class CmmDtl: INotifyPropertyChanged
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


    private string __ID;
        private string __DUP; //중복체크상태
        private string __CHK; //중복체크상태

        public string ID
        {
            get { return __ID; }
            set
            {
                this.__ID = value;
            }
        }
        public string DUP
        {
            get { return __DUP; }
            set
            {
                this.__DUP = value;
            }
        }
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
