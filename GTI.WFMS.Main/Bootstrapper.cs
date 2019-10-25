using System;
using System.Windows;
using Prism.Unity;
using Unity;
using GTI.WFMS.Modules.Adm;

namespace GTI.WFMS.Main
{
    public class Bootstrapper : UnityBootstrapper
    {
        /// <summary>
        /// 순서 (1)
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<UcUserMngView>("UcUserMngView");
            Container.RegisterTypeForNavigation<UcCodeMngView>("UcCodeMngView");
        }

        /// <summary>
        /// 순서 (2)
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<MainWin>();
        }

        /// <summary>
        /// 순서 (3)
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="name"></param>
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name)
        {
            Type type = typeof(T);
            string strviewName = String.IsNullOrWhiteSpace(name) ? type.Name : name;
            container.RegisterType(typeof(object), typeof(T), strviewName);
        }
    }
}
