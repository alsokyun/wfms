using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// FtrSelView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FtrSelView : Window
    {

        /// <summary>
        /// 생성자
        /// </summary>
        public FtrSelView(string FTR_IDN)
        {

            InitializeComponent();


            // 2.테마일괄적용...
            ThemeApply.Themeapply(this);

        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();
        }

        //그리드 더블클릭
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridControl gc = sender as GridControl;

            try
            {
                txtFTR_CDE.Text = ((DataRowView)gc.CurrentItem).Row["FTR_CDE"].ToString();
                txtFTR_NAM.Text = ((DataRowView)gc.CurrentItem).Row["FTR_NAM"].ToString();
                txtHJD_NAM.Text = ((DataRowView)gc.CurrentItem).Row["HJD_NAM"].ToString();
                txtFTR_IDN.Text = ((DataRowView)gc.CurrentItem).Row["FTR_IDN"].ToString();

                //팝업호출지점으로 리턴
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
