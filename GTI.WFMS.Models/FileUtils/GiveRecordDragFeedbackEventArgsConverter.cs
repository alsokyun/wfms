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
    public class GiveRecordDragFeedbackEventArgsConverter : MarkupExtension, IEventArgsConverter {
        public object Convert(object sender, object args) {
            var e = (GiveRecordDragFeedbackEventArgs)args;
            e.UseDefaultCursors = false;
            Mouse.SetCursor(Cursors.Hand); 
            return e.Handled;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
