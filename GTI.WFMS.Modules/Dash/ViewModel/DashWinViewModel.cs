using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Dash.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Dash.ViewModel
{
    public class DashWinViewModel
    {

        #region ==========  Properties 정의 ==========

        public RelayCommand<object> LoadedCommand { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> SearchCommand { get; set; }

        #endregion



        #region ==========  Member 정의 ==========

        DashWinView dashWinView;

        #endregion




        ///생성자
        public DashWinViewModel()
        {

            LoadedCommand = new RelayCommand<object>(delegate (object obj){

                if (obj == null) return;

                // 0.뷰객체를 파라미터로 전달받기
                dashWinView = obj as DashWinView;
                
                dashWinView.ctl1.Content = new UcChart01();
                dashWinView.ctl2.Content = new UcChart02();
                dashWinView.ctl3.Content = new UcChart03();
                dashWinView.ctl4.Content = new UcChart04();


            });


            SearchCommand = new RelayCommand<object>(delegate (object obj){ 
            });

        }











}
}
