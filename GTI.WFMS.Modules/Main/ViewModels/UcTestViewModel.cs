using GTIFramework.Common.MessageBox;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Main
{
    class UcTestViewModel : BindableBase
    {
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        public UcTestViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
        }

        private  void OnLoaded(object obj)
        {
            if (obj == null) return;

            try
            {
                Messages.ShowInfoMsgBox("안녕하세요");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }

        }
    }
}
