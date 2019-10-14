using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Adm.Work;
using GTI.WFMS.Models.Cmm.Work;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Adm
{
    class UcCodeMngViewModel : BindableBase
    {
        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }

        /// <summary>
        /// 상위코드 그리드 변경 이벤트
        /// </summary>
        public DelegateCommand<object> MSTGridChangeCommand { get; set; }

        /// <summary>
        /// 상위코드 검색 이벤트
        /// </summary>
        public DelegateCommand<object> SearchCommand { get; set; }

        /// <summary>
        /// 상위코드 추가 이벤트
        /// </summary>
        public DelegateCommand<object> AddMSTCommand { get; set; }

        /// <summary>
        /// 상위코드 저장 이벤트
        /// </summary>
        public DelegateCommand<object> SaveMSTCommand { get; set; }

        /// <summary>
        /// 하위코드 추가 이벤트
        /// </summary>
        public DelegateCommand<object> AddDTLCommand { get; set; }

        /// <summary>
        /// 하위코드 저장 이벤트
        /// </summary>
        public DelegateCommand<object> SaveDTLCommand { get; set; }

        /// <summary>
        /// 하위코드 삭제 이벤트
        /// </summary>
        public DelegateCommand<object> DeleteCommand { get; set; }

        #endregion


        /// <summary>
        /// 생성자
        /// </summary>
        public UcCodeMngViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            MSTGridChangeCommand = new DelegateCommand<object>(MSTChangeAction);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            AddMSTCommand = new DelegateCommand<object>(AddMSTAction);
            SaveMSTCommand = new DelegateCommand<object>(SaveMSTAction);
            AddDTLCommand = new DelegateCommand<object>(AddDTLAction);
            SaveDTLCommand = new DelegateCommand<object>(SaveDTLAction);
            DeleteCommand = new DelegateCommand<object>(DeleteAction);
        }


        #region ========== Members 정의 ==========
        AdmWork admWork = new AdmWork();
        CmmWork cmmWork = new CmmWork();

        Hashtable conditions = new Hashtable();
        DataTable dtCD_MST_INFO = new DataTable();
        DataTable dtCD_DTL_INFO = new DataTable();

        UcCodeMngView ucCodeMngView;
        TextEdit txtWord;
        Button btnSearch;
        Button btnSave;
        Button btnAdd;
        GridControl MSTgrid;
        Button btnAddLow;
        Button btnDel;
        Button btnSaveLow;
        GridControl DTLgrid;

        #endregion



        #region ========== Event 정의 ==========
        private void OnLoaded(object obj)
        {
            if (obj == null) return;

            var values = (object[])obj;

            ucCodeMngView = values[0] as UcCodeMngView;
            txtWord = ucCodeMngView.txtWord;
            btnSearch = ucCodeMngView.btnSearch;
            btnSave = ucCodeMngView.btnSave;
            btnAdd = ucCodeMngView.btnAdd;
            MSTgrid = ucCodeMngView.MSTgrid;
            btnAddLow = ucCodeMngView.btnAddLow;
            btnDel = ucCodeMngView.btnDel;
            btnSaveLow = ucCodeMngView.btnSaveLow;
            DTLgrid = ucCodeMngView.DTLgrid;

            InitDataBinding();
            MSTChangeAction(null);
        }

        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>       
        private void InitDataBinding()
        {
            try
            {
                dtCD_MST_INFO = admWork.selectMstCdList(null);
                MSTgrid.ItemsSource = dtCD_MST_INFO;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 상위코드 그리드 변경 액션
        /// </summary>
        /// <param name="obj"></param>
        private void MSTChangeAction(object obj)
        {
            try
            {
                if (((DataRowView)MSTgrid.SelectedItem) != null)
                {
                    conditions.Clear();
                    conditions.Add("MST_CD", ((DataRowView)MSTgrid.SelectedItem).Row["MST_CD"].ToString());
                    conditions.Add("USE_YN", "Y");
                    dtCD_DTL_INFO = admWork.selectDtlCdList(conditions);
                    DTLgrid.ItemsSource = dtCD_DTL_INFO;
                }
                else
                {
                    DTLgrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 검색 액션
        /// </summary>
        /// <param name="obj"></param>
        private void SearchAction(object obj)
        {
            try
            {
                DataTable dtSearchCode = new DataTable();
                conditions.Clear();
                //상위코드명 검색 조건
                txtWord.Text = txtWord.Text.ToString().Trim();
                if (!txtWord.Text.ToString().Equals(""))
                {
                    conditions.Add("SEARCH_WORD", txtWord.Text.ToString());
                }
                dtSearchCode = admWork.selectMstCdList(conditions);
                MSTgrid.ItemsSource = dtSearchCode.Copy();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 상위코드 추가 액션
        /// </summary>
        /// <param name="obj"></param>
        private void AddMSTAction(object obj)
        {
            try
            {
                DataRow drNew = ((DataTable)MSTgrid.ItemsSource).NewRow();
                drNew["MST_CD"] = "";
                drNew["CD_NM"] = "";
                drNew["ETC"] = "";
                drNew["ORD"] = "0";
                drNew["USE_YN"] = "Y";
                ((DataTable)MSTgrid.ItemsSource).Rows.Add(drNew);
                MSTgrid.View.FocusedRowHandle = ((DataTable)MSTgrid.ItemsSource).Rows.Count - 1; //그리드ROW position

                //하위코드 재조회
                MSTChangeAction(null);
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 상위코드 저장 액션
        /// </summary>
        /// <param name="obj"></param>
        private void SaveMSTAction(object obj)
        {
            try
            {
                foreach (DataRow dr in ((DataTable)MSTgrid.ItemsSource).Rows)
                {
                    dr["MST_CD"] = dr["MST_CD"].ToString().Trim();
                    dr["CD_NM"] = dr["CD_NM"].ToString().Trim();
                    if (!dr["ORD"].ToString().Equals(""))
                    {
                        dr["ORD"] = dr["ORD"].ToString().Trim();
                    }
                    dr["ETC"] = dr["ETC"].ToString().Trim();

                    if (dr["MST_CD"].ToString().Equals(""))
                    {
                        Messages.ShowInfoMsgBox("상위코드ID는 필수 입력 항목입니다.");
                        return;
                    }

                    if (dr["CD_NM"].ToString().Equals(""))
                    {
                        Messages.ShowInfoMsgBox("상위코드명은 필수 입력 항목입니다.");
                        return;
                    }
                }

                if (Messages.ShowYesNoMsgBox("수정 항목을 저장 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    foreach (DataRow dr in ((DataTable)MSTgrid.ItemsSource).Rows)
                    {
                        conditions.Clear();
                        conditions.Add("MST_CD", dr["MST_CD"].ToString());
                        conditions.Add("CD_NM", dr["CD_NM"].ToString());

                        conditions.Add("USE_YN", dr["USE_YN"].ToString());//토글컨버터적용됨

                        conditions.Add("ORD", dr["ORD"].ToString());
                        conditions.Add("ETC", dr["ETC"].ToString());
                        conditions.Add("EDT_ID", Logs.strLogin_ID);

                        admWork.saveMstCd(conditions);
                    }

                    Messages.ShowOkMsgBox();
                    SearchAction(null);
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 하위코드 추가 액션
        /// </summary>
        /// <param name="obj"></param>
        private void AddDTLAction(object obj)
        {
            try
            {

                string SEL_MST_CD = "";
                try
                {
                    SEL_MST_CD = ((DataRowView)MSTgrid.SelectedItem).Row["MST_CD"].ToString();
                }
                catch (Exception)
                {
                    //throw;
                }
                
                if (SEL_MST_CD != null && !"".Equals(SEL_MST_CD))
                {
                    DataRow drNew = ((DataTable)DTLgrid.ItemsSource).NewRow();
                    drNew["MST_CD"] = SEL_MST_CD ;
                    drNew["USE_YN"] = "Y";
                    drNew["DTL_CD"] = "";
                    drNew["CD_NM"] = "";
                    drNew["ETC"] = "";
                    drNew["ORD"] = "0";
                    ((DataTable)DTLgrid.ItemsSource).Rows.Add(drNew);
                    DTLgrid.View.FocusedRowHandle = ((DataTable)DTLgrid.ItemsSource).Rows.Count - 1; //그리드ROW position
                }
                else
                {
                    Messages.ShowErrMsgBox("상위코드를 선택하세요.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }




        /// <summary>
        /// 하위코드 저장 액션
        /// </summary>
        /// <param name="obj"></param>
        private void SaveDTLAction(object obj)
        {
            try
            {
                if (((DataTable)DTLgrid.ItemsSource).Rows.Count < 1)
                {
                    Messages.ShowErrMsgBox("하위코드가 없습니다.");
                    return;
                }

                foreach (DataRow dr in ((DataTable)DTLgrid.ItemsSource).Rows)
                {
                    dr["DTL_CD"] = dr["DTL_CD"].ToString().Trim();
                    dr["CD_NM"] = dr["CD_NM"].ToString().Trim();
                    if (!dr["ORD"].ToString().Equals(""))
                    {
                        dr["ORD"] = dr["ORD"].ToString().Trim();
                    }
                    dr["ETC"] = dr["ETC"].ToString().Trim();

                    if (dr["DTL_CD"].ToString().Equals(""))
                    {
                        Messages.ShowInfoMsgBox("하위코드ID는 필수 입력 항목입니다.");
                        return;
                    }

                    if (dr["CD_NM"].ToString().Equals(""))
                    {
                        Messages.ShowInfoMsgBox("하위코드명은 필수 입력 항목입니다.");
                        return;
                    }
                }

                if (Messages.ShowYesNoMsgBox("수정 항목을 저장 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    foreach (DataRow dr in ((DataTable)DTLgrid.ItemsSource).Rows)
                    {
                        conditions.Clear();
                        conditions.Add("MST_CD", dr["MST_CD"].ToString());
                        conditions.Add("DTL_CD", dr["DTL_CD"].ToString());
                        conditions.Add("CD_NM", dr["CD_NM"].ToString());

                        conditions.Add("USE_YN", dr["USE_YN"].ToString());//토글컨버터적용됨

                        conditions.Add("ORD", dr["ORD"].ToString());
                        conditions.Add("ETC", dr["ETC"].ToString());
                        conditions.Add("EDT_ID", Logs.strLogin_ID);
                        

                        admWork.saveDtlCd(conditions);
                    }

                    Messages.ShowOkMsgBox();
                    MSTChangeAction(null);
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 하위코드 삭제 액션
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteAction(object obj)
        {
            try
            {
                bool isChecked = false;

                foreach (DataRow dr in ((DataTable)DTLgrid.ItemsSource).Rows)
                {
                    if (dr["CHK"].ToString().Equals("Y"))
                    {
                        isChecked = true;
                    }
                }

                if (isChecked)
                {
                    if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                    {
                        for (int i = ((DataTable)DTLgrid.ItemsSource).Rows.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                if (((DataTable)DTLgrid.ItemsSource).Rows[i]["CHK"].ToString().Equals("Y"))
                                {
                                    conditions.Clear();
                                    conditions.Add("MST_CD", ((DataTable)DTLgrid.ItemsSource).Rows[i]["MST_CD"].ToString());
                                    conditions.Add("DTL_CD", ((DataTable)DTLgrid.ItemsSource).Rows[i]["DTL_CD"].ToString());
                                    conditions.Add("EDT_ID", Logs.strLogin_ID);

                                    admWork.deleteDtlCd(conditions);

                                    ((DataTable)DTLgrid.ItemsSource).Rows.RemoveAt(i);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        Messages.ShowOkMsgBox();
                    }
                }
                else
                {
                    Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        #endregion

        #region ==========  Method 정의 ==========
        #endregion


    }
}
