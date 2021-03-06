﻿using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Cnst.ViewModel
{

    public class WttChngDtViewModel : INotifyPropertyChanged
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



        public WttChngDtView wttChngDtView;


        #region ============ 프로퍼티부분 ===============
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DelCommand { get; set; }
        public DelegateCommand<object> AddCommand { get; set; }


        ObservableCollection<WttChngDt> __GrdLst;
        public ObservableCollection<WttChngDt> GrdLst
        {
            get { return __GrdLst; }
            set
            {
                if (value == __GrdLst) return;
                __GrdLst = value;
                OnPropertyChanged("GrdLst");
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
        
        public WttChngDtViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DelCommand = new DelegateCommand<object>(OnDelete);
            //행추가
            this.AddCommand = new DelegateCommand<object>(delegate(object obj) {
                WttChngDt addrow = new WttChngDt();
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
            wttChngDtView = obj as WttChngDtView;

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
                param.Add("sqlId", "SelectWttChngDtList");
                param.Add("CNT_NUM", this.CNT_NUM);

                GrdLst = new ObservableCollection<WttChngDt>(BizUtil.SelectListObj<WttChngDt>(param));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //상위화면조회
        private void parentInitModel()
        {
            CnstMngDtlView cnstMngDtlView = (((wttChngDtView.Parent as DXTabItem).Parent as DXTabControl).Parent as Grid).Parent as CnstMngDtlView;
            cnstMngDtlView.refresh();
            //cnstMngDtlView.InvalidateVisual();
            
            //CnstMngDtlViewModel vm =
            //((((wttChngDtView.Parent as DXTabItem).Parent as DXTabControl).Parent as Grid).Parent as CnstMngDtlView).DataContext as CnstMngDtlViewModel;
            //vm.InitModel();
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
                foreach (WttChngDt row in GrdLst)
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
                    foreach (WttChngDt row in GrdLst)
                    {
                        Hashtable param = new Hashtable();
                        try
                        {
                            if ("Y".Equals(row.CHK))
                            {
                                param.Clear();
                                param.Add("sqlId", "DeleteWttChngDt");
                                param.Add("CNT_NUM", CNT_NUM);

                                if (row.CHNG_SEQ == 0)
                                {
                                    //그리드행만 삭제
                                    GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                    return;
                                }
                                else
                                {
                                    //데이터삭제
                                    param.Add("CHNG_SEQ", Convert.ToInt32(row.CHNG_SEQ));
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
                    //initModel();
                    parentInitModel();
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
            foreach (WttChngDt row in GrdLst)
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
            foreach (WttChngDt row in GrdLst)
            {
                if (row.CHK != "Y")     continue;

                row.CNT_NUM = CNT_NUM;
                try
                {
                    BizUtil.Update2(row, "SaveWttChngDt2");
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
            //initModel();
            parentInitModel();


        }


    }
}
