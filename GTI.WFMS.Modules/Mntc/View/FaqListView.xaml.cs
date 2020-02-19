using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// FaqListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FaqListView : Page
    {
        public FaqListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }


        //선택된 항목으로 페이지이동
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string SEQ = "";

            GridControl gc = sender as GridControl;

            try
            {
                SEQ = ((DataRowView)gc.CurrentItem).Row["SEQ"].ToString();

                ///페이지이동 - 뷰생성자로 파라미터키 전달 
                ///=> 뷰모델과바인딩된 객체값을 변경해서 뷰모델로 최종적으로 파라미터 전달
                NavigationService.Navigate(new FaqDtlView(SEQ));
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return; //throw;
            }
            
        }

        // 등록 팝업
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new WtlPipeAddView());
        }
    }
}
