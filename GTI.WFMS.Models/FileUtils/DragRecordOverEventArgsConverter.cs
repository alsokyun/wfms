using System;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;

namespace GTI.WFMS.Models.FileUtils
{
    public class DragRecordOverEventArgsConverter : MarkupExtension, IEventArgsConverter {
        /// <summary>
        /// Allow file drop feature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Convert(object sender, object args) {
            DragRecordOverEventArgs e = (DragRecordOverEventArgs)args;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            return e.Handled;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
