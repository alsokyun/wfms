using DevExpress.Xpf.Grid;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.GIS.Pop.View
{    
    /// <summary>
    /// CnstMngPopView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstMngPopView : Window
    {

        // 생성자
        public CnstMngPopView(string CNT_NUM)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);

            //뷰모델로 키값전달
            txtCNT_NUM.Text = CNT_NUM;
            txtRET_CNT_NAM.Text = CNT_NUM;
        }

        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridControl gc = sender as GridControl;

            try
            {
                txtRET_CNT_NAM.Text = ((DataRowView)gc.CurrentItem).Row["CNT_NUM"].ToString();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();
        }
    }
}
