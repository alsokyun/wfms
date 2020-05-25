using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// ChkPlanListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChkPlanListView: UserControl
    {

        // 생성자
        public ChkPlanListView( )
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            //초기데이터
            InitDataBinding();

            dtCHK_YM.EditValue = DateTime.Today;

            //초기조회
            SearchAction();

        }




        #region ========== 메소드 ==========


        /// <summary>
        /// 데이터바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "전체");
                // 점검구분
                BizUtil.SetCmbCode(cbSCL_CDE, "250105", "전체");
                // 점검진행상태
                BizUtil.SetCmbCode(cbSCL_STAT_CDE, "250107", "전체");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }

        /// <summary>
        /// 조회
        /// </summary>
        private void SearchAction()
        {
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectChscMaList");

            param.Add("TIT_NAM", txtTIT_NAM.Text);
            param.Add("CKM_PEO", txtCKM_PEO.Text);
            
            //점검월
            param.Add("CHK_YM", Convert.ToDateTime(dtCHK_YM.EditValue).ToString("yyyyMM"));

            param.Add("MNG_CDE", cbMNG_CDE.EditValue); //관리기관
            param.Add("SCL_CDE", cbSCL_CDE.EditValue); 
            param.Add("SCL_STAT_CDE", cbSCL_STAT_CDE.EditValue); 

            
            List<ChscMaDtl> lst = (List<ChscMaDtl>)BizUtil.SelectListObj<ChscMaDtl>(param);
            grid.ItemsSource = lst;
        }


        #endregion



        #region ============ 이벤트핸들러 ============ 


        // 검색
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SearchAction();
        }
        //초기화
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            cbMNG_CDE.SelectedIndex = 0;
            cbSCL_CDE.SelectedIndex = 0;
            cbSCL_STAT_CDE.SelectedIndex = 0;
            txtTIT_NAM.Text = "";
            txtCKM_PEO.Text = "";
            dtCHK_YM.EditValue = DateTime.Today; 
        }





        //점검일정등록
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //1.점검일정 팝업호출(신규건)
            ChkSchAddView chkSchAddView = new ChkSchAddView();
            if (chkSchAddView.ShowDialog() is bool)
            {
                //팝업종료 후 재조회
                SearchAction();
            }
        }



        // 점검일정 상세팝업
        private void Gv_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;
            try
            {
                string SCL_NUM = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "SCL_NUM").ToString();

                //1.점검일정 팝업호출
                ChkSchDtlView chkSchDtlView = new ChkSchDtlView(SCL_NUM);
                if (chkSchDtlView.ShowDialog() is bool)
                {
                    //팝업종료 후 재조회
                    SearchAction();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        

     






        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            //CheckEdit ce = sender as CheckEdit;
            //bool chk = ce.IsChecked is bool;
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                dr["CHK"] = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dr in ((DataTable)grid.ItemsSource).Rows)
            {
                dr["CHK"] = "N";
            }
        }




        // 엔터키 조회
        private void Enter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SearchAction();
            }
        }





        #endregion

 
    }
}
