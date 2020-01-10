using DevExpress.Mvvm;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{

    public class WttSubcDtViewModel : INotifyPropertyChanged
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


        #region ============ 프로퍼티부분 ===============
            ObservableCollection<WttSubcDt> lst;
            public ObservableCollection<WttSubcDt> Lst
            {
                get { return lst; }
                set
                {
                    if (value == lst)
                        return;
                    lst = value;
                    OnPropertyChanged("Lst");
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
        #endregion














        //그리드데이터 소스
        public DelegateCommand<object> SaveCommand { get; set; }


        public WttSubcDtViewModel()
        {
            this.SaveCommand = new DelegateCommand<object>(SaveGrid);

            //초기조회
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWttSubcDtList2");

            param.Add("CNT_NUM", CNT_NUM);

            //dt = BizUtil.SelectList(param);
            //grid.ItemsSource = dt;
            Lst = new ObservableCollection<WttSubcDt>(BizUtil.SelectListObj<WttSubcDt>(param));
        }



        /// <summary>
        /// 그리드저장
        /// </summary>
        private void SaveGrid(object obj)
        {
            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            //기존 공사비 삭제
            Hashtable param = new Hashtable();
            param.Add("sqlId", "DeleteWttSubcDt");
            param.Add("CNT_NUM", CNT_NUM);
            BizUtil.Update(param);

            //그리드 저장
            foreach (WttSubcDt row in lst)
            {
                param = new Hashtable();

                param.Add("sqlId", "SaveWttSubcDt");
                param.Add("CNT_NUM", CNT_NUM);
                param.Add("SUBC_SEQ", Convert.ToInt32(row.SUBC_SEQ));

                param.Add("SUB_NAM", row.SUB_NAM);
                param.Add("PSB_NAM", row.PSB_NAM);
                param.Add("SUB_ADR", row.SUB_ADR);
                param.Add("SUB_TEL", row.SUB_TEL);


                try
                {
                    BizUtil.Update(param);
                }
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }

            }
            //저장처리성공
            Messages.ShowOkMsgBox();
        }


    }
}
