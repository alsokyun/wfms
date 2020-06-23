using GTI.WFMS.Models.Acmf.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Link.ViewModel
{



    public class HydtMetrSubListViewModel : HydtMetrDtl
    {

        public RelayCommand<object> LoadedCommand { get; set; }
        private HydtMetrSubListView hydtMetrSubListView;

        //생성자
        public HydtMetrSubListViewModel()
        {

            this.LoadedCommand = new RelayCommand<object>(OnLoaded);

        }


        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            // 0.화면객체인스턴스화
            if (obj == null) return;
            hydtMetrSubListView = obj as HydtMetrSubListView;



            //초기조회
            Hashtable param = new Hashtable();
            param.Add("sqlId", "selectHydtMetrSubList");
            param.Add("FTR_CDE", FTR_CDE);
            param.Add("FTR_IDN", FTR_IDN);

            DataTable dt = new DataTable();
            dt = BizUtil.SelectList(param);
            hydtMetrSubListView.grid.ItemsSource = dt;
        }

    }
}
