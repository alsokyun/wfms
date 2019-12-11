using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTIFramework.Common.Utils.ViewEffect
{
    public class ThemeApply
    {
        private static bool bregname = false;

        /// <summary>
        /// GTIBlueTheme, GTINavyTheme
        /// </summary>
        public static string strThemeName = string.Empty;

        /// <summary>
        /// (DependencyObject : obj)
        /// (DependencyObject : 창)
        /// 테마 이름 GTIBlueTheme, GTINavyTheme
        /// 사용 이전에 strThemeName 설정 필요
        /// GTIBlueTheme, GTINavyTheme
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="strThemeName"></param>
        public static void Themeapply(object obj)
        {
            try
            {
                if (obj != null)
                {
                    DependencyObject DO = (DependencyObject)obj;

                    Theme themeBlue = new Theme("GTIBlueTheme");
                    themeBlue.AssemblyName = "DevExpress.Xpf.Themes.GTIBlueTheme.v19.1";

                    Theme themeNavy = new Theme("GTINavyTheme");
                    themeNavy.AssemblyName = "DevExpress.Xpf.Themes.GTINavyTheme.v19.1";

                    if (!bregname)
                    {
                        Theme.RegisterTheme(themeBlue);
                        Theme.RegisterTheme(themeNavy);
                        bregname = true;
                    }

                    ThemeManager.SetThemeName(DO, strThemeName);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void ThemeChange(object obj)
        {
            try
            {
                if (obj != null)
                {
                    DependencyObject DO = (DependencyObject)obj;

                    if (strThemeName.Equals("GTIBlueTheme"))
                    {
                        Application.Current.Resources.MergedDictionaries.Clear();

                        Application.Current.Resources = Application.LoadComponent(new Uri("Styles/Blue/Global.xaml", UriKind.Relative)) as ResourceDictionary;
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Blue/Buttons.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Blue/Colors.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Blue/Controls.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Blue/Fonts.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Blue/Labels.xaml", UriKind.Relative) });
                    }
                    else if (strThemeName.Equals("GTINavyTheme"))
                    {
                        Application.Current.Resources.MergedDictionaries.Clear();

                        Application.Current.Resources = Application.LoadComponent(new Uri("Styles/Navy/Global.xaml", UriKind.Relative)) as ResourceDictionary;
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Navy/Buttons.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Navy/Colors.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Navy/Controls.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Navy/Fonts.xaml", UriKind.Relative) });
                        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/Navy/Labels.xaml", UriKind.Relative) });
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
