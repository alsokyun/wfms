using DevExpress.Mvvm;
using DevExpress.Xpf.Charts;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Dash.Model;
using GTI.WFMS.Modules.Dash.View;
using GTIFramework.Common.Log;
using System;
using System.Collections;
using System.Data;

namespace GTI.WFMS.Modules.Dash.ViewModel
{
    public class UcChart04Model : DashDtl
    {

        #region ==========  Properties 정의 ==========
                
        public DelegateCommand<object> LoadedCommand { get; set; }
        #endregion

        #region ==========  Member 정의 ==========
        UcChart04 ucChart04;
        #endregion

        /// 생성자
        public UcChart04Model() =>
            // 초기이벤트
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);

        #region ==========  이벤트 핸들러 ==========

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            try
            {
                // 0.화면객체인스턴스화
                if (obj == null) return;
                var values = (object[])obj;

                //1. 화면객체 인스턴스
                ucChart04 = values[0] as UcChart04;

                // 4.초기조회
                Hashtable param = new Hashtable();

                var sYm = Convert.ToDateTime(DateTime.Today).ToString("yyyyMM");

                param.Add("sqlId", "SelectDashChart4List");
                param.Add("pYm", sYm);
                param.Add("id" , Logs.strLogin_ID);


                DataTable dt = BizUtil.SelectList(param);

                // Create an empty chart. 
                ChartControl chart1 = ucChart04.ctDash;

                // Bind a chart to a data source. 
                chart1.DataSource = dt;
                
                ucChart04.srXSER1.Points.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    SeriesPoint point = new SeriesPoint(row["NAM"].ToString(), Convert.ToDouble(row["DATA_VAL"]));
                    ucChart04.srXSER1.Points.Add(point);
                }

              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
        #endregion
    }
}
