using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;

namespace GTI.WFMS.Models.FileUtils
{
    public class StartRecordDragEventArgsConverter : MarkupExtension, IEventArgsConverter {
        public object Convert(object sender, object args)
        {
            var e = (StartRecordDragEventArgs) args;
            e.AllowedEffects = DragDropEffects.Copy;
            foreach (FileInfo fileInfo in e.Records) {
                if (fileInfo.Extension == ".sln")
                {
                    e.AllowDrag = false;
                    e.Handled = true;
                }
            }
            return e.AllowDrag;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
