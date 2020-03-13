using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Dash.View;
using GTIFramework.Common.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
            // 4.초기조회
            Hashtable param = new Hashtable();
            DataTable dt = new DataTable();

            String sYm = Convert.ToDateTime(DateTime.Today).ToString("yyyyMM");
            String sMenuFleNm = "";
            int nCt = 0;

            try
            {
                param.Add("sqlId", "SelectDashMenuList");
                param.Add("pYm", sYm);
                param.Add("id", Logs.strLogin_ID);
                dt = BizUtil.SelectList(param);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            LoadedCommand = new RelayCommand<object>(delegate (object obj){

                if (obj == null) return;

                // 0.뷰객체를 파라미터로 전달받기
                dashWinView = obj as DashWinView;
                foreach (DataRow row in dt.Rows)
                {                    
                    if ("Y".Equals(row["MNU_USE_YN"].ToString()))
                    {
                        nCt++;
                        sMenuFleNm = row["MNU_FLE_NM"].ToString();
                                       
                        if (nCt == 1)
                        {
                            if (sMenuFleNm == "dashChart1") dashWinView.ctl1.Content = new UcChart01();
                            else if (sMenuFleNm == "dashChart2") dashWinView.ctl1.Content = new UcChart02();
                            else if (sMenuFleNm == "dashChart3") dashWinView.ctl1.Content = new UcChart03();
                            else if (sMenuFleNm == "dashChart4") dashWinView.ctl1.Content = new UcChart04();
                            else if (sMenuFleNm == "dashChart5") dashWinView.ctl1.Content = new UcChart05();
                            else if (sMenuFleNm == "dashChart6") dashWinView.ctl1.Content = new UcChart06();
                            else if (sMenuFleNm == "dashChart7") dashWinView.ctl1.Content = new UcChart07();
                            else dashWinView.ctl1.Content = "";
                        }
                        else if (nCt == 2)
                        {
                            if (sMenuFleNm == "dashChart1") dashWinView.ctl2.Content = new UcChart01();
                            else if(sMenuFleNm == "dashChart2") dashWinView.ctl2.Content = new UcChart02();
                            else if (sMenuFleNm == "dashChart3") dashWinView.ctl2.Content = new UcChart03();
                            else if (sMenuFleNm == "dashChart4") dashWinView.ctl2.Content = new UcChart04();
                            else if (sMenuFleNm == "dashChart5") dashWinView.ctl2.Content = new UcChart05();
                            else if (sMenuFleNm == "dashChart6") dashWinView.ctl2.Content = new UcChart06();
                            else if (sMenuFleNm == "dashChart7") dashWinView.ctl2.Content = new UcChart07();
                            else dashWinView.ctl2.Content = "";
                        }
                        else if (nCt == 3)
                        {
                            if (sMenuFleNm == "dashChart1") dashWinView.ctl3.Content = new UcChart01();
                            else if (sMenuFleNm == "dashChart2") dashWinView.ctl3.Content = new UcChart02();
                            else if (sMenuFleNm == "dashChart3") dashWinView.ctl3.Content = new UcChart03();
                            else if (sMenuFleNm == "dashChart4") dashWinView.ctl3.Content = new UcChart04();
                            else if (sMenuFleNm == "dashChart5") dashWinView.ctl3.Content = new UcChart05();
                            else if (sMenuFleNm == "dashChart6") dashWinView.ctl3.Content = new UcChart06();
                            else if (sMenuFleNm == "dashChart7") dashWinView.ctl3.Content = new UcChart07();
                            else dashWinView.ctl3.Content = "";
                        }
                        else if (nCt == 4)
                        {
                            if (sMenuFleNm == "dashChart1") dashWinView.ctl4.Content = new UcChart01();
                            else if (sMenuFleNm == "dashChart2") dashWinView.ctl4.Content = new UcChart02();
                            else if (sMenuFleNm == "dashChart3") dashWinView.ctl4.Content = new UcChart03();
                            else if (sMenuFleNm == "dashChart4") dashWinView.ctl4.Content = new UcChart04();
                            else if (sMenuFleNm == "dashChart5") dashWinView.ctl4.Content = new UcChart05();
                            else if (sMenuFleNm == "dashChart6") dashWinView.ctl4.Content = new UcChart06();
                            else if (sMenuFleNm == "dashChart7") dashWinView.ctl4.Content = new UcChart07();
                            else dashWinView.ctl4.Content = "";
                        }
                    }                    
                }

            });


            SearchCommand = new RelayCommand<object>(delegate (object obj){ 
            });

        }











}
}
