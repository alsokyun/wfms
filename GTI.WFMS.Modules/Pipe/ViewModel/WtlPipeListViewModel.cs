using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Cmm.Work;
using GTI.WFMS.Models.Pipe.Work;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    class WtlPipeListViewModel
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

        public DelegateCommand<object> btnCmd { get; set; }
        #endregion



        #region ========== Members 정의 ==========
        PipeWork pipeWork = new PipeWork();
        CmmWork cmmWork = new CmmWork();

        WtlPipeList wtlPipeList;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();
        ComboBoxEdit cbMOP_CDE; DataTable dtMOP_CDE = new DataTable();
        ComboBoxEdit cbJHT_CDE; DataTable dtJHT_CDE = new DataTable();

        TextEdit txtFTR_IDN;
        TextEdit txtCNT_NUM;
        TextEdit txtSHT_NUM;
        TextEdit txtPIP_DIP;

        DateEdit dtIST_YMD_FROM;
        DateEdit dtIST_YMD_TO;

        GridControl grid;

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public WtlPipeListViewModel()
        {

            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SearchCommand = new DelegateCommand<object>(SearchAction);

            btnCmd = new DelegateCommand<object>(btnMethod);

        }



        #region ========== Event 정의 ==========
        /// <summary>
        /// 로드 바인딩
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            if (obj == null) return;
            var values = (object[])obj;

            //1. 화면객체 인스턴스
            wtlPipeList = values[0] as WtlPipeList ;

            cbMNG_CDE = wtlPipeList.cbMNG_CDE;
            cbHJD_CDE = wtlPipeList.cbHJD_CDE;
            cbMOP_CDE = wtlPipeList.cbMOP_CDE;
            cbJHT_CDE = wtlPipeList.cbJHT_CDE;
            txtFTR_IDN = wtlPipeList.txtFTR_IDN;
            txtCNT_NUM = wtlPipeList.txtCNT_NUM;
            txtSHT_NUM = wtlPipeList.txtSHT_NUM;
            txtPIP_DIP = wtlPipeList.txtPIP_DIP;

            dtIST_YMD_FROM = wtlPipeList.dtIST_YMD_FROM;
            dtIST_YMD_TO = wtlPipeList.dtIST_YMD_TO;
            dtIST_YMD_FROM.DisplayFormatString = "yyyy-MM-dd";
            dtIST_YMD_TO.DisplayFormatString = "yyyy-MM-dd";
            dtIST_YMD_FROM.EditValue = DateTime.Today.AddYears(-3);
            dtIST_YMD_TO.EditValue = DateTime.Today;

            grid = wtlPipeList.grid;


            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();
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
                conditions.Add("MNG_CDE", cbMNG_CDE.EditValue.ToString().Trim());
                conditions.Add("HJD_CDE", cbHJD_CDE.EditValue.ToString().Trim());
                conditions.Add("MOP_CDE", cbMOP_CDE.EditValue.ToString().Trim());
                conditions.Add("JHT_CDE", cbJHT_CDE.EditValue.ToString().Trim());
                conditions.Add("FTR_IDN", FmsUtil.Trim(txtFTR_IDN.EditValue));
                conditions.Add("CNT_NUM", txtCNT_NUM.Text.Trim());
                conditions.Add("SHT_NUM", txtSHT_NUM.Text.Trim());
                conditions.Add("PIP_DIP", txtPIP_DIP.Text.Trim());
                try
                {
                    conditions.Add("IST_YMD_FROM", Convert.ToDateTime(dtIST_YMD_FROM.EditValue).ToString("yyyy-MM-dd"));
                    conditions.Add("IST_YMD_TO", Convert.ToDateTime(dtIST_YMD_TO.EditValue).ToString("yyyy-MM-dd"));
                }
                catch (Exception e) { }

                conditions.Add("firstIndex", 0);
                conditions.Add("lastIndex", 1000);

                //dtresult = pipeWork.SelectWtlPipeList(conditions);
                conditions.Add("sqlId", "SelectWtlPipeList");
                dtresult = BizUtil.SelectList(conditions);
                grid.ItemsSource = dtresult;
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
            try { 
                // cbMNG_CDE
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMOP_CDE
                BizUtil.SetCmbCode(cbMOP_CDE, "MOP_CDE", true);

                // cbJHT_CDE
                BizUtil.SetCmbCode(cbJHT_CDE, "JHT_CDE", true);
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
            try
            {
                string strPermission = Logs.htPermission[Logs.strFocusMNU_CD].ToString();
                switch (strPermission)
                {
                    case "W":
                        break;
                    case "R":
                        //btnAdd.Visibility = Visibility.Collapsed;
                        break;
                    case "N":
                        break;
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }

        }



        private void btnMethod(object obj)
        {
            MessageBox.Show("btn01Method");
        }
        #endregion

    }

}
