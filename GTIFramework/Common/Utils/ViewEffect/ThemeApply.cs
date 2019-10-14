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

        public static void Themeapply(object obj)
        {
            try
            {
                if (obj != null)
                {
                    DependencyObject DO = (DependencyObject)obj;

                    Theme theme = new Theme("GTIBlueTheme");
                    theme.AssemblyName = "DevExpress.Xpf.Themes.GTIBlueTheme.v19.1";
                    
                    if (!bregname)
                    {
                        Theme.RegisterTheme(theme);
                        bregname = true;
                    }
                    ThemeManager.SetTheme(DO, theme);
                }
            }
            catch (Exception){}
        }
    }
}
