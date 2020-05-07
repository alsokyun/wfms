using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// CnstCmplSelView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstCmplSelView : Window
    {
        public CnstCmplSelView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }


        //선택된 항목으로 페이지이동
        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;

            try
            {
                string RCV_NUM = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "RCV_NUM").ToString();
                txbRCV_NUM.Text = RCV_NUM;

                //팝업호출지점으로 리턴
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();
        }

    }
}
