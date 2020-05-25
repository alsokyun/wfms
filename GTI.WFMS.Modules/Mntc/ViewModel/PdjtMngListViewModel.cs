using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Cnst.Model;
using GTI.WFMS.Modules.Cnst.View;
using GTI.WFMS.Modules.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{

    public class PdjtMngListViewModel : INotifyPropertyChanged
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



        public PdjtMngListView pdjtMngListView;


        #region ============ 프로퍼티부분 ===============
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DelCommand { get; set; }
        public DelegateCommand<object> SearchCommand { get; set; }
        public DelegateCommand<object> ResetCommand { get; set; }
        public DelegateCommand<object> AddCommand { get; set; }
        public DelegateCommand<object> EnterCommand { get; set; }


        ObservableCollection<PdjtMaDtl> __GrdLst;
        public ObservableCollection<PdjtMaDtl> GrdLst
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
        
        public PdjtMngListViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.DelCommand = new DelegateCommand<object>(OnDelete);
            this.SearchCommand = new DelegateCommand<object>(SearchAction);
            this.ResetCommand = new DelegateCommand<object>(ResetAction);
            this.EnterCommand = new DelegateCommand<object>(EnterAction);
            //행추가
            this.AddCommand = new DelegateCommand<object>(delegate(object obj) {
                PdjtMaDtl addrow = new PdjtMaDtl();
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
            pdjtMngListView = obj as PdjtMngListView;


            //데이터바인딩
            InitDataBinding();


            //초기조회
            SearchAction(null);
        }


        /// <summary>
        /// 조회작업
        /// </summary>        
        private void SearchAction(object obj)
        {
            try
            {
                DataTable dt = new DataTable();

                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectPdjtMaMngList");

                param.Add("PDT_NAM", pdjtMngListView.txtPDT_NAM.Text);
                param.Add("PDT_MDL_STD", pdjtMngListView.txtPDT_MDL_STD.Text);
                param.Add("PDT_MNF", pdjtMngListView.txtPDT_MNF.Text);
                param.Add("PDT_CAT_CDE", pdjtMngListView.cbPDT_CAT_CDE.EditValue); //소모품

                GrdLst = new ObservableCollection<PdjtMaDtl>(BizUtil.SelectListObj<PdjtMaDtl>(param));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //상위화면조회
        private void parentInitModel()
        {
            CnstMngDtlView cnstMngDtlView = (((pdjtMngListView.Parent as DXTabItem).Parent as DXTabControl).Parent as Grid).Parent as CnstMngDtlView;
            cnstMngDtlView.refresh();
            //cnstMngDtlView.InvalidateVisual();
            
            //CnstMngDtlViewModel vm =
            //((((pdjtMngListView.Parent as DXTabItem).Parent as DXTabControl).Parent as Grid).Parent as CnstMngDtlView).DataContext as CnstMngDtlViewModel;
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
                foreach (PdjtMaDtl row in GrdLst)
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

                //자식데이터여부
                foreach (PdjtMaDtl row in GrdLst)
                {
                    if ("Y".Equals(row.CHK))
                    {
                        int cnt = 0;
                        try
                        {
                            cnt = Convert.ToInt32(row.CNT);
                        }
                        catch (Exception) { }
                        if (cnt > 0)
                        {
                            Messages.ShowInfoMsgBox("선택한 항목중에 입고내역이 있습니다.");
                            return;
                        }
                    }
                }



                if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    foreach (PdjtMaDtl row in GrdLst)
                    {
                        Hashtable param = new Hashtable();
                        try
                        {
                            if ("Y".Equals(row.CHK))
                            {
                                param.Clear();
                                param.Add("sqlId", "DeletePdjtMa");

                                if (row.PDH_NUM == 0)
                                {
                                    //그리드행만 삭제
                                    GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                    return;
                                }
                                else
                                {
                                    //데이터삭제
                                    param.Add("PDH_NUM", row.PDH_NUM);
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
                    SearchAction(null);
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
            foreach (PdjtMaDtl row in GrdLst)
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
            foreach (PdjtMaDtl row in GrdLst)
            {
                if (row.CHK != "Y")     continue;

                try
                {
                    BizUtil.Update2(row, "SavePdjtMa");
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                    return;
                }
            }

            //저장처리성공
            Messages.ShowOkMsgBox();

            //재조회
            SearchAction(null);
        }




        #region 내부메소드

        /// <summary>
        /// 데이터바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbPDT_CAT_CDE
                BizUtil.SetCmbCode(pdjtMngListView.cbPDT_CAT_CDE, "250106", "전체");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="obj"></param>
        private void ResetAction(object obj)
        {
            pdjtMngListView.cbPDT_CAT_CDE.SelectedIndex = 0;
            pdjtMngListView.txtPDT_MDL_STD.Text = "";
            pdjtMngListView.txtPDT_MNF.Text = "";
            pdjtMngListView.txtPDT_NAM.Text = "";
        }

        /// <summary>
        /// 입고등록
        /// </summary>
        /// <param name="obj"></param>
        private void EnterAction(object obj)
        {
            string PDH_NUM = "";

            //0.체크박스 체크
            int cnt = 0;
            foreach (PdjtMaDtl dr in GrdLst)
            {
                //체크여부
                if ("Y".Equals(dr.CHK))
                {
                    cnt++;
                    try
                    {
                        PDH_NUM = dr.PDH_NUM.ToString();
                    }
                    catch (Exception) { }
                }
            }
            if (cnt < 1)
            {
                Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                return;
            }
            else if (cnt > 1)
            {
                Messages.ShowInfoMsgBox("입고대상 항목을 하나만 선택하세요.");
                return;
            }

            //1.입고등록 팝업호출
            PdjtStockView pdjtEnterView = new PdjtStockView(PDH_NUM);
            if (pdjtEnterView.ShowDialog() is bool)
            {
                //팝업종료 후 재조회
                SearchAction(null);
            }
        }


        #endregion

    }
}
