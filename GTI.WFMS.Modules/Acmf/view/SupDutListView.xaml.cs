using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTI.WFMS.Modules.Acmf.View
{
    /// <summary>
    /// SupDutListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SupDutListView : Page
    {
        public SupDutListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }


        // 등록 팝업
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {            
            NavigationService.Navigate(new SupDutAddView());
        }

        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TableView tv = sender as TableView;
            try
            {
                string FTR_CDE = tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_CDE").ToString();
                int FTR_IDN = Convert.ToInt32(tv.Grid.GetCellValue(e.HitInfo.RowHandle, "FTR_IDN"));

                ///페이지이동 - 뷰생성자로 파라미터키 전달 
                ///=> 뷰모델과바인딩된 객체값을 변경해서 뷰모델로 최종적으로 파라미터 전달
                NavigationService.Navigate(new SupDutDtlView(FTR_CDE, FTR_IDN));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
