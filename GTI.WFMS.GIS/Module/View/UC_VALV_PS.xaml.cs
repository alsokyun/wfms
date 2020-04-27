using GTI.WFMS.GIS.Pop.View;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fclt.Model;
using GTI.WFMS.Models.Pipe.Model;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.GIS.Module.View
{
    /// <summary>
    /// UC_VALV_PS.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UC_VALV_PS : UserControl
    {
        //생성자
        public UC_VALV_PS()
        {
            InitializeComponent();
        }

        //시설물 상세 생성자
        public UC_VALV_PS(string _FTR_CDE, string _FTR_IDN) : this()
        {
            txtFTR_CDE.Text = _FTR_CDE;
            txtFTR_IDN.Text = _FTR_IDN;
        }

        //신규 시설물 생성자
        public UC_VALV_PS(string _FTR_CDE) : this()
        {
            txtFTR_CDE.Text = _FTR_CDE;
            
            //신규관리번호채번
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectValvFacFTR_IDN");
            ValvFacDtl result = BizUtil.SelectObject(param) as ValvFacDtl;
            
            //채번결과 매칭
            txtFTR_IDN.Text = result.FTR_IDN.ToString();
            btnDel.Tag = "Y";//신규채번 플래그
        }




        // 공사번호선택팝업
        private void BtnSel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            String inCNT_NUM = this.txtCNT_NUM.Text; ;
            String outCNT_NUM = "";

            if (inCNT_NUM != null && inCNT_NUM != "")
            {
                if (Messages.ShowYesNoMsgBox("공사번호를 변경하시겠습니까?") != MessageBoxResult.Yes) return;
            }

            try
            {
                // 상수공사대장 윈도우
                CnstMngPopView cnstMngPopView = new CnstMngPopView("");
                cnstMngPopView.Owner = Window.GetWindow(this);

                //공사번호 리턴
                if (cnstMngPopView.ShowDialog() is bool)
                {
                    outCNT_NUM = cnstMngPopView.txtRET_CNT_NAM.Text;
                    if (outCNT_NUM != null && outCNT_NUM != "" && inCNT_NUM != outCNT_NUM)
                    {
                        this.txtCNT_NUM.Text = outCNT_NUM;
                    }

                    this.txtCNT_NUM.SelectAll();
                    this.txtCNT_NUM.Focus();
                }
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }
    }
}