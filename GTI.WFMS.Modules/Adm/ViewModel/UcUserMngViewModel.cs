﻿using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Adm.Work;
using GTI.WFMS.Models.Cmm.Work;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.Converters;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Adm
{
    class UcUserMngViewModel: BindableBase
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }

        /// <summary>
        /// btnSearch 이벤트
        /// </summary>
        public DelegateCommand<object> SearchCommand { get; set; }
        /// <summary>
        /// btnAdd 이벤트
        /// </summary>
        public DelegateCommand<object> btnAddClickCommnad { get; set; }

        /// <summary>
        /// btnSave 이벤트
        /// </summary>
        public DelegateCommand<object> btnSaveClickCommnad { get; set; }

        /// <summary>
        /// btnDel 이벤트
        /// </summary>
        public DelegateCommand<object> btnDelClickCommnad { get; set; }

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public UcUserMngViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);
            btnAddClickCommnad = new DelegateCommand<object>(btnAddClickAction);
            btnSaveClickCommnad = new DelegateCommand<object>(btnSaveClickAction);
            btnDelClickCommnad = new DelegateCommand<object>(btnDelClickAction);
        }



        #region ========== Members 정의 ==========
        AdmWork admWork = new AdmWork();
        CmmWork cmmWork = new CmmWork();

        UcUserMngView ucUserMngView;
        TextEdit txtWord;
        ComboBoxEdit cbGBN;
        GridControl grid;
        //구분 콤보박스 데이터
        DataTable dtcbGBN = new DataTable();
        //직급 콤보박스 데이터
        DataTable dtcbRank = new DataTable();
        //부서 콤보박스 데이터
        DataTable dtcbDept = new DataTable();

        #region 입력항목
        TextEdit txtID;
        TextEdit txtName;
        PasswordBoxEdit pwPW;
        ComboBoxEdit cbDept;
        ComboBoxEdit cbRank;
        TextEdit txtTelNum;
        ToggleSwitch tgUse;
        TextEdit txtRemark;

        Button btnAdd;
        Button btnDel;
        Button btnSave;
        #endregion

        #endregion




        #region ========== Event 정의 ==========
        /// <summary>
        /// 로드 바인딩
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {

            if (obj == null) return;

            var values = (object[])obj;

            try
            {
                //1.화면객체 초기화
                ucUserMngView = values[0] as UcUserMngView;

                //cbGBN = values[1] as ComboBoxEdit;
                //txtWord = values[2] as TextEdit;
                //grid = values[4] as GridControl;
                cbGBN = ucUserMngView.cbGBN;
                txtWord = ucUserMngView.txtWord;
                grid = ucUserMngView.grid;

                txtID = ucUserMngView.txtID;
                txtName = ucUserMngView.txtName;
                pwPW = ucUserMngView.pwPW;
                cbDept = ucUserMngView.cbDept;
                cbRank = ucUserMngView.cbRank;
                txtTelNum = ucUserMngView.txtTelNum;
                tgUse = ucUserMngView.tgUse;
                txtRemark = ucUserMngView.txtRemark;

                btnAdd = ucUserMngView.btnAdd;
                btnDel = ucUserMngView.btnDel;
                btnSave = ucUserMngView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();

                //3.권한처리

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="obj"></param>
        private void SearchAction(object obj)
        {
            try
            {
                //if (treeList.FocusedNode == null) return;

                DataTable dtresult = new DataTable();

                Hashtable conditions = new Hashtable();
                conditions.Add("WORD", txtWord.Text.Replace(" ", ""));
                conditions.Add("GBN", cbGBN.EditValue);

                dtresult = admWork.selectUsrList(conditions);

                foreach (DataRow dr in dtresult.Rows)
                {
                    dr["USER_PWD"] = EncryptionConvert.Base64Decoding(dr["USER_PWD"].ToString());
                }

                grid.ItemsSource = dtresult;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        ///  행추가
        /// </summary>
        /// <param name="obj"></param>
        private void btnAddClickAction(object obj)
        {
            try
            {
                grid.SelectedItem = null;

                txtID.Clear();
                txtID.IsEnabled = true;
                txtName.Clear();
                pwPW.Clear();
                //lookUpEditDept.EditValue = ((DataRowView)treeList.FocusedNode.Content).Row["DEPT_CD"].ToString();
                cbDept.SelectedIndex = -1;
                cbRank.SelectedIndex = -1;
                txtTelNum.Clear();
                tgUse.IsChecked = true;
                txtRemark.Clear();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }
        
        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="obj"></param>
        private void btnDelClickAction(object obj)
        {
            try
            {
                if (grid.SelectedItem == null)
                {
                    Messages.ShowInfoMsgBox("선택된 사용자가 없습니다.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("선택한 사용자를 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                {
                    Hashtable conditions = new Hashtable();
                    conditions.Add("USER_ID", ((DataRowView)grid.SelectedItem).Row["USER_ID"].ToString());
                    conditions.Add("EDT_ID", Logs.strLogin_ID);

                    admWork.Delete_SYS_USER_INFO(conditions);

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
        /// 저장처리
        /// </summary>
        /// <param name="obj"></param>
        private void btnSaveClickAction(object obj)
        {
            try
            {
                #region ##null 확인 아이디 부서 이름 비밀번호
                if (txtID.Text.Trim().Equals(""))
                {
                    Messages.ShowErrMsgBox("아이디를 입력해주세요.");
                    txtID.Focus();
                    return;
                }

                if (txtName.Text.Trim().Equals(""))
                {
                    Messages.ShowErrMsgBox("이름을 입력해주세요.");
                    txtName.Focus();
                    return;
                }

                if (pwPW.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("비밀번호를 입력해주세요.");
                    pwPW.Focus();
                    return;
                }

                if (cbDept.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("부서를 입력해주세요.");
                    cbDept.Focus();
                    return;
                }

                if (cbRank.Text.Equals(""))
                {
                    Messages.ShowErrMsgBox("직급을 입력해주세요.");
                    cbRank.Focus();
                    return;
                }
                #endregion

                Hashtable conditions = new Hashtable();
                conditions.Add("USER_ID", txtID.Text.Trim());
                conditions.Add("DEPT_CD", cbDept.EditValue);
                conditions.Add("USER_NM", txtName.Text.Trim());
                conditions.Add("USER_TEL", txtTelNum.Text);
                conditions.Add("USER_PWD", EncryptionConvert.Base64Encoding(pwPW.Text.ToString()));
                conditions.Add("POS_CD", cbRank.EditValue);
                conditions.Add("ETC", txtRemark.Text.Trim());
                if (tgUse.IsChecked == true)
                    conditions.Add("USE_YN", "Y");
                else
                    conditions.Add("USE_YN", "N");
                conditions.Add("EDT_ID", Logs.strLogin_ID);

                ///추가
                if (grid.SelectedItem == null)
                {
                    if (admWork.Select_SYS_USER_INFO_Check(conditions).Rows.Count == 0)
                    {
                        admWork.Insert_SYS_USER_INFO(conditions);

                        txtID.IsEnabled = false;
                        Messages.ShowOkMsgBox();

                        SearchAction(null);
                    }
                    else
                    {
                        Messages.ShowInfoMsgBox("아이디가 존재합니다.");
                    }
                }
                //업데이트
                else
                {
                    admWork.Update_SYS_USER_INFO(conditions);

                    Messages.ShowOkMsgBox();

                    SearchAction(null);

                    foreach (DataRowView drv in ((DataTable)grid.ItemsSource).DefaultView)
                    {
                        if (drv["USER_ID"].ToString().Equals(conditions["USER_ID"].ToString()))
                        {
                            grid.SelectedItem = drv;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        #endregion



        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                if (dtcbGBN.Columns.Count == 0)
                {
                    // 구분콤보
                    dtcbGBN.Columns.Add("CODE");
                    dtcbGBN.Columns.Add("NAME");
                    DataRow drgbn1 = dtcbGBN.NewRow();
                    drgbn1["CODE"] = "*";
                    drgbn1["NAME"] = "전체";
                    dtcbGBN.Rows.Add(drgbn1);

                    DataRow drgbn2 = dtcbGBN.NewRow();
                    drgbn2["CODE"] = "NM";
                    drgbn2["NAME"] = "이름";
                    dtcbGBN.Rows.Add(drgbn2);

                    DataRow drgbn3 = dtcbGBN.NewRow();
                    drgbn3["CODE"] = "ID";
                    drgbn3["NAME"] = "아이디";
                    dtcbGBN.Rows.Add(drgbn3);

                    cbGBN.ItemsSource = dtcbGBN;

                    cbGBN.SelectedIndex = 0;

                    Hashtable conditions = new Hashtable();

                    // 직급콤보 MSTCD = 000004
                    conditions.Add("MST_CD", "POSI");
                    dtcbRank = cmmWork.Select_CODE_LIST(conditions);
                    cbRank.ItemsSource = dtcbRank;

                    // 부서콤보 
                    conditions.Clear();
                    conditions.Add("MST_CD", "DEPT");
                    dtcbDept = cmmWork.Select_CODE_LIST(conditions);
                    cbDept.ItemsSource = dtcbDept;

                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 화면 권한처리
        /// </summary>
        private void permissionApply()
        {
            string strPermission = Logs.htPermission[Logs.strFocusMNU_CD].ToString();

            switch (strPermission)
            {
                case "W":
                    break;
                case "R":
                    btnAdd.Visibility = Visibility.Collapsed;
                    btnDel.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    break;
                case "N":
                    break;
            }
        }

        #endregion


    }
}
