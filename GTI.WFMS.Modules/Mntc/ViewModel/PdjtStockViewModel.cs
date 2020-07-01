using DevExpress.Mvvm;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{

    public class PdjtStockViewModel : INotifyPropertyChanged
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



        public PdjtStockView pdjtStockView;


        #region ============ 프로퍼티부분 ===============
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DelCommand { get; set; }
        public DelegateCommand<object> AddCommand { get; set; }


        ObservableCollection<PdjtInDtl> __GrdLst;
        public ObservableCollection<PdjtInDtl> GrdLst
        {
            get { return __GrdLst; }
            set
            {
                if (value == __GrdLst) return;
                __GrdLst = value;
                OnPropertyChanged("GrdLst");
            }
        }
        private string __PDH_NUM;
        public string PDH_NUM
        {
            get { return __PDH_NUM; }
            set
            {
                this.__PDH_NUM = value;
                OnPropertyChanged("PDH_NUM");
            }
        }

        #endregion
        
        public PdjtStockViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DelCommand = new DelegateCommand<object>(OnDelete);
            //행추가
            this.AddCommand = new DelegateCommand<object>(delegate(object obj) {
                PdjtInDtl addrow = new PdjtInDtl();
                GrdLst.Add(addrow);
                addrow.CHK = "Y";
            });
        }



        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            // 0.화면객체인스턴스화
            if (obj == null) return;
            pdjtStockView = obj as PdjtStockView;

            //초기조회
            initModel();
        }


        /// <summary>
        /// 조회작업
        /// </summary>        
        private void initModel()
        {
            try
            {

                //초기조회
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectPdjtInHtPopList");
                param.Add("PDH_NUM", PDH_NUM);

                GrdLst = new ObservableCollection < PdjtInDtl >(BizUtil.SelectListObj<PdjtInDtl>(param));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 그리드삭제
        /// </summary>
        private void OnDelete(object obj)
        {
           //데이터 직접삭제처리
            try
            {
                bool isChecked = false;
                foreach (PdjtInDtl row in GrdLst)
                {
                    if ("Y".Equals(row.CHK))
                    {
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked)
                {
                    Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                    return;
                }
                
                if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    foreach (PdjtInDtl row in GrdLst)
                    {
                        Hashtable param = new Hashtable();
                        try
                        {
                            if ("Y".Equals(row.CHK))
                            {
                                param.Clear();
                                param.Add("sqlId", "DeletePdjtInHtPop");
                                param.Add("PDH_NUM", row.PDH_NUM);

                                if (row.IN_NUM == 0)
                                {
                                    //그리드행만 삭제
                                    GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                    return;
                                }
                                else
                                {
                                    //데이터삭제
                                    param.Add("IN_NUM", row.IN_NUM);
                                    BizUtil.Update(param);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
                            return;
                        }                        
                    }

                    Messages.ShowOkMsgBox();

                    //재조회
                    initModel();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }

        }

        /// <summary>
        /// 그리드저장
        /// </summary>
        private void OnSave(object obj)
        {
            bool isChecked = false;
            foreach (PdjtInDtl row in GrdLst)
            {
                if ("Y".Equals(row.CHK))
                {
                    isChecked = true;
                    break;
                }
            }
            if (!isChecked)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            Hashtable param = new Hashtable();

            //그리드 저장
            foreach (PdjtInDtl row in GrdLst)
            {
                if (row.CHK != "Y")     continue;

                if (FmsUtil.IsNull(row.IN_YMD))
                {
                    MessageBox.Show("입고일자는 필수입니다.");
                    return;
                }

                row.PDH_NUM = Convert.ToInt32(PDH_NUM) ;
                try
                {
                    BizUtil.Update2(row, "SavePdjtInHtPop");
                }
                catch (Exception)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }
            }

            //저장처리성공
            Messages.ShowOkMsgBox();

            //재조회
            initModel();


        }


    }
}
