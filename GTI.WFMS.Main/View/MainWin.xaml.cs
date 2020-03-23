using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
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
using System.Windows.Shapes;

namespace GTI.WFMS.Main
{
    /// <summary>
    /// MainWin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWin : Window
    {
        public MainWin()
        {
            InitializeComponent();
        }

        private void Border_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                #region #### 테마 변경
                ContextMenu cm = new ContextMenu();

                MenuItem cmnow = new MenuItem();
                cmnow.Header = "현재 : " + Properties.Settings.Default.strThemeName;
                cm.Items.Add(cmnow);

                MenuItem cmblue = new MenuItem();
                cmblue.Click += Cmblue_Click;
                cmblue.Header = "블루로 변경";
                cm.Items.Add(cmblue);

                MenuItem cmnavy = new MenuItem();
                cmnavy.Click += Cmnavy_Click;
                cmnavy.Header = "네이비로 변경";
                cm.Items.Add(cmnavy);

                cm.IsOpen = true;
                #endregion
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        private void Cmnavy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.strThemeName = "GTINavyTheme";
                Properties.Settings.Default.Save();



                ThemeApply.strThemeName = "GTINavyTheme";
                ThemeApply.ThemeChange(this);
                //메뉴 Image 변경
                foreach (var item in spMenuArea.Children)
                {
                    if (item is Button)
                    {
                        (item as Button).Tag = (item as Button).Tag.ToString().Replace("Blue", "Navy");
                        (item as Button).Style = Application.Current.Resources["MainMNUButton"] as Style;
                    }
                }
                ThemeApply.Themeapply(this);


                ((sender as MenuItem).Parent as ContextMenu).IsOpen = false;
            }
            catch (Exception )
            {
            }
        }

        private void Cmblue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.strThemeName = "GTIBlueTheme";
                Properties.Settings.Default.Save();



                ThemeApply.strThemeName = "GTIBlueTheme";
                ThemeApply.ThemeChange(this);
                //메뉴 Image 변경
                foreach (var item in spMenuArea.Children)
                {
                    if (item is Button)
                    {
                        (item as Button).Tag = (item as Button).Tag.ToString().Replace("Navy", "Blue");
                        (item as Button).Style = Application.Current.Resources["MainMNUButton"] as Style;
                    }
                }
                ThemeApply.Themeapply(this);

                ((sender as MenuItem).Parent as ContextMenu).IsOpen = false;
            }
            catch (Exception )
            {
            }
        }
    }
}
