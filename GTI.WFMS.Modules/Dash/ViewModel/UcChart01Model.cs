using DevExpress.Mvvm;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Scheduling;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Dash.Model;
using GTI.WFMS.Modules.Dash.View;
using GTIFramework.Common.Log;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows;

namespace GTI.WFMS.Modules.Dash.ViewModel
{
    public class UcChart01Model : DashDtl
    {

        #region ==========  Properties 정의 ==========
                
        public DelegateCommand<object> LoadedCommand { get; set; }
        #endregion

        #region ==========  Member 정의 ==========
        UcChart01 dashView;
        #endregion
               
        /// 생성자
        public UcChart01Model()
        {
            // 초기이벤트
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);           
        }

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
                dashView = values[0] as UcChart01;

                // 4.초기조회
                Hashtable param = new Hashtable();

                var sYm = Convert.ToDateTime(DateTime.Today).ToString("yyyyMM");

                param.Add("sqlId", "SelectDashChart1List");
                param.Add("pYm", sYm);
                param.Add("id" , Logs.strLogin_ID);


                DataTable dt = BizUtil.SelectList(param);

                // Create an empty chart. 
                ChartControl chart1 = dashView.ctDash1;

                // Bind a chart to a data source. 
                chart1.DataSource = dt;
                
                dashView.srX_TIT11.Points.Clear();
                dashView.srX_TIT12.Points.Clear();

                dashView.srX_TIT11.Visible = false;
                dashView.srX_TIT12.Visible = true;
                foreach (DataRow row in dt.Rows)
                {
                    SeriesPoint point = new SeriesPoint(row["NAM"].ToString(), Convert.ToDouble(row["DATA_VAL"]));
                    SeriesPoint point2 = new SeriesPoint(row["NAM"].ToString(), Convert.ToDouble(row["DATA_VAL2"]));
                    dashView.srX_TIT11.Points.Add(point);
                    dashView.srX_TIT12.Points.Add(point2);
                    dashView.ctDash1.Legend.Visibility = Visibility.Collapsed;
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
