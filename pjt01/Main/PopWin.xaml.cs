using pjt01.Adm.View;
using System;
using System.Collections.Generic;
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

namespace pjt01.Main
{
    /// <summary>
    /// PopWin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopWin : Window
    {
        private string v; //화면컨트롤명

        public PopWin()
        {
            InitializeComponent();
        }

        public PopWin(string v) : this()
        {

            this.v = v;
            //this.srcFrm.Source = new Uri("../Adm/View/Page1.xaml", UriKind.Relative);
            this.srcFrm.Navigate(new Page1());
        }
    }
}
