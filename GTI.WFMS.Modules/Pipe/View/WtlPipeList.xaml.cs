using DevExpress.Xpf.Grid;
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

namespace GTI.WFMS.Modules.Pipe.View
{
    /// <summary>
    /// WtlPipeList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WtlPipeList : Page
    {
        public WtlPipeList()
        {
            InitializeComponent();
        }

        
        //선택된 항목으로 페이지이동
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string FTR_CDE = "";
            int FTR_IDN = 0;

            GridControl gc = sender as GridControl;
            FTR_CDE = ((DataRowView)gc.CurrentItem).Row["FTR_CDE"].ToString();
            FTR_IDN = Convert.ToInt32(((DataRowView)gc.CurrentItem).Row["FTR_IDN"])  ;
            
            ///페이지이동 - 뷰생성자로 파라미터키 전달 
            ///=> 뷰모델과바인딩된 객체값을 변경해서 뷰모델로 최종적으로 파라미터 전달
            NavigationService.Navigate(new WtlPipeDtlView(FTR_CDE, FTR_IDN));
        }
    }
}
