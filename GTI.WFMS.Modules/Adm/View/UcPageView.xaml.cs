using GTI.WFMS.Modules.Adm.View;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace GTI.WFMS.Modules.Adm
{
    /// <summary>
    /// UcPageView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UcPageView : Page
    {
        public UcPageView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            //NavigationService.Navigate(new Uri("./Page1.xaml", UriKind.Relative));
            NavigationService.Navigate(new Page2());
        }
    }
}
