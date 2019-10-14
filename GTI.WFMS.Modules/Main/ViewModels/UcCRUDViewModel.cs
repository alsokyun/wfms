using GTI.WFMS.Models.Main.Work;
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
    class UcCRUDViewModel : BindableBase
    {
        MainWork work = new MainWork();

        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        #endregion

        #region ========== Members 정의 ==========
        UcCRUDView ucCRUDView;
        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public UcCRUDViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
        }

        #region ========== Event 정의 ==========
        /// <summary>
        /// 로드 바인딩
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            if (obj == null) return;

            var values = (object[])obj;

            try
            {
                ucCRUDView = values[0] as UcCRUDView;
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        } 
        #endregion
    }
}
