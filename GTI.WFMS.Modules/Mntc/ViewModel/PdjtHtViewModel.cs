using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.View;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{

    public class PdjtHtViewModel : INotifyPropertyChanged
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



        public PdjtHtView pdjtHtView;


        #region ============ 프로퍼티부분 ===============
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DelCommand { get; set; }
        public DelegateCommand<object> AddCommand { get; set; }


        private ObservableCollection<PdjtHtDtl> __GrdLst;
        public ObservableCollection<PdjtHtDtl> GrdLst
        {
            get { return __GrdLst; }
            set
            {
                if (value == __GrdLst) return;
                __GrdLst = value;
                OnPropertyChanged("GrdLst");
            }
        }
        private string __SCL_NUM;
        public string SCL_NUM
        {
            get { return __SCL_NUM; }
            set
            {
                this.__SCL_NUM = value;
                OnPropertyChanged("SCL_NUM");
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
        private string __FTR_IDN;
        public string FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                OnPropertyChanged("FTR_IDN");
            }
        }
        private string __SEQ;
        public string SEQ
        {
            get { return __SEQ; }
            set
            {
                this.__SEQ = value;
                OnPropertyChanged("SEQ");
            }
        }
        //소모품-주유오일 구분자
        private string __PDT_CAT_CDE;
        public string PDT_CAT_CDE
        {
            get { return __PDT_CAT_CDE; }
            set
            {
                this.__PDT_CAT_CDE = value;
                OnPropertyChanged("PDT_CAT_CDE");
            }
        }

        #endregion






        public PdjtHtViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DelCommand = new DelegateCommand<object>(OnDelete);
            //행추가
            this.AddCommand = new DelegateCommand<object>(delegate(object obj) {
                PdjtHtDtl row = new PdjtHtDtl();
                row.SCL_NUM = Convert.ToInt16(SCL_NUM);
                row.FTR_CDE = FTR_CDE;
                row.FTR_IDN = Convert.ToInt16(FTR_IDN);
                row.SEQ = Convert.ToInt16(SEQ);

                GrdLst.Add(row);
                row.CHK = "Y";
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
            pdjtHtView = obj as PdjtHtView;

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
                param.Add("sqlId", "SelectPdhUseList");
                param.Add("SCL_NUM", SCL_NUM);
                param.Add("FTR_CDE", FTR_CDE);
                param.Add("FTR_IDN", FTR_IDN);
                param.Add("SEQ", SEQ);
                param.Add("PDT_CAT_CDE", PDT_CAT_CDE); //소모품

                GrdLst = new ObservableCollection<PdjtHtDtl>(BizUtil.SelectListObj<PdjtHtDtl>(param));

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
                foreach (PdjtHtDtl row in GrdLst)
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
                    foreach (PdjtHtDtl row in GrdLst)
                    {
                        Hashtable param = new Hashtable();
                        try
                        {
                            if ("Y".Equals(row.CHK))
                            {

                                if (row.SEQ == 0)
                                {
                                    //그리드행만 삭제
                                    GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                    return;
                                }
                                else
                                {
                                    //데이터삭제
                                    BizUtil.Update2(row, "DeletePdjtHt");
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
            foreach (PdjtHtDtl row in GrdLst)
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

            //필수체크
            foreach (PdjtHtDtl row in GrdLst)
            {
                if (row.CHK != "Y") continue;

                if (FmsUtil.IsNull(row.PDH_NUM))
                {
                    Messages.ShowErrMsgBox("소모품은 필수입력입니다.");
                    return;
                }
            }


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;


            //그리드 저장
            foreach (PdjtHtDtl row in GrdLst)
            {
                if (row.CHK != "Y")     continue;

                try
                {
                    BizUtil.Update2(row, "SavePdjtHt");
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
