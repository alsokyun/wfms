using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;

namespace GTI.WFMS.Models.FileUtils
{
    public class DropRecordEventArgsConverter : MarkupExtension, IEventArgsConverter
    {
        /// <summary>
        ///     Return file drop data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Convert(object sender, object args)
        {
            var e = (DropRecordEventArgs) args;
            var filesData = new List<FileInfo>();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files == null || files.Length == 0)
                    return null;
                foreach (var file in files)
                    filesData.Add(new FileInfo(file));
                e.Handled = true;
            } 
            return filesData;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}