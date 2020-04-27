using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// LekSiteDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LekSiteDtlView : Window
    {
        private string _FTR_CDE;
        private string _FTR_IDN;
        


        /// <summary>
        /// 생성자
        /// </summary>
        public LekSiteDtlView(string FTR_CDE, string FTR_IDN)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            //뷰모델로 키전달하기기위 뷰에서 UpdateTrigger 발생시킴
            if (FTR_CDE != null)
            {
                _FTR_CDE = FTR_CDE;
                _FTR_IDN = FTR_IDN;
                this.txtFTR_CDE.Text = FTR_CDE;
                this.txtFTR_IDN.Text = FTR_IDN;
            }

            //사진관리 Content 생성
            this.cctrlPhoto.Content = new ImgFileMngView(FTR_CDE + FTR_IDN);
        }



        //민원선택 팝업호출
        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 지형지물팝업 윈도우
                CnstCmplSelView cnstCmplSelView = new CnstCmplSelView();
                cnstCmplSelView.Owner = Window.GetWindow(this);


                //FTR_IDN 리턴
                if (cnstCmplSelView.ShowDialog() is bool)
                {
                    string RCV_NUM = cnstCmplSelView.txbRCV_NUM.Text;


                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(RCV_NUM))
                    {
                        txtRCV_NUM.EditValue = RCV_NUM;
                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }

        }

        //상수관로 시설물선택
        private void BtnFtrSel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 지형지물팝업 윈도우
                FtrSelView ftrSelView = new FtrSelView(null);
                ftrSelView.Owner = Window.GetWindow(this);


                //FTR_IDN 리턴
                if (ftrSelView.ShowDialog() is bool)
                {
                    string FTR_IDN = ftrSelView.txtFTR_IDN.Text;
                    string FTR_CDE = ftrSelView.txtFTR_CDE.Text;
                    string FTR_NAM = ftrSelView.txtFTR_NAM.Text;


                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(FTR_IDN))
                    {
                        txtPIP_IDN.EditValue = FTR_IDN;
                        txtPIP_CDE.Text = FTR_CDE;
                        txtPIP_NAM.EditValue = FTR_NAM;

                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void LekSiteDtlView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    this.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    this.WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }



    }
}
