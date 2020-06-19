using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// PdjtHtView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtHtView : UserControl
    {

        //private string SCL_NUM;
        //private string FTR_CDE;
        //private string FTR_IDN;
        //private string SEQ;

        #region =========== 생성자 ============

        public PdjtHtView(string _SCL_NUM, string _FTR_CDE, string _FTR_IDN, string _SEQ)
        {
            InitializeComponent();

            //모델로 파라미터전달하기위해 뷰에 매핑
            txtSCL_NUM.Text = _SCL_NUM;
            txtFTR_CDE.Text = _FTR_CDE;
            txtFTR_IDN.Text = _FTR_IDN;
            txtSEQ.Text = _SEQ;
            txtPDT_CAT_CDE.Text = "PDT001";//소모품
        }


        #endregion




        #region =========== 이벤트핸들러 ============





        //공통코드콤보 초기로딩
        private void PDH_NUM_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;

            Hashtable param = new Hashtable();
            param["sqlId"] = "SelectPdhList";
            param["ValueMember"] = "PDH_NUM";
            param["DisplayMember"] = "PDT_NAM";
            param["PDT_CAT_CDE"] = "PDT001"; //소모품
            obj.ItemsSource = BizUtil.GetCombo(param);

            obj.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        //콤보변경 이벤트핸들러 
        private void OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            DataTable dt = (DataTable)obj.ItemsSource;

            try
            {
                DataRowView rv = obj.SelectedItem as DataRowView;
                DataRow[] dr = dt.Select("PDH_NUM='" + rv.Row["PDH_NUM"].ToString() + "\'");
                string PDT_MDL_STD = "";
                PDT_MDL_STD = dr[0]["PDT_MDL_STD"].ToString();
                grid.SetCellValue(gv.FocusedRowHandle, "PDT_MDL_STD", PDT_MDL_STD);
            }
            catch (Exception){}

        }



        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            foreach (PdjtHtDtl dr in (ObservableCollection<PdjtHtDtl>)grid.ItemsSource)
            {
                dr.CHK = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (PdjtHtDtl dr in (ObservableCollection<PdjtHtDtl>)grid.ItemsSource)
            {
                dr.CHK = "N";
            }
        }


        #endregion

    }



}
