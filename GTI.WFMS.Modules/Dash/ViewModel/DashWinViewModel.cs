using DevExpress.Mvvm;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Dash.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GTI.WFMS.Modules.Dash.ViewModel
{
    public class DashWinViewModel : BindableBase
    {
        #region ==========  Properties 정의 ==========
        
        public DelegateCommand<object> LoadedCommand { get; set; } //Loaded이벤트에서 ICommand 사용하여 뷰객체 전달받음
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> MenuShowHidenCommand { get; set; }


        #endregion

        #region ==========  Member 정의 ==========
        DashWinView dashWinView;

        private bool bMenuShowHiden = true; //아코디언메뉴 visible 

        #endregion

        ///생성자
        public DashWinViewModel()
        {
            // 초기이벤트
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.MenuShowHidenCommand = new DelegateCommand<object>(OnMenuShowHiden);
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

                InitModel(obj);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// 조회작업
        /// </summary>
        /// <param name="obj"></param>
        private void InitModel(object obj)
        {
            try
            {
                // 4.초기조회
                Hashtable param = new Hashtable();
                DataTable dt = new DataTable();

                String sYm = Convert.ToDateTime(DateTime.Today).ToString("yyyyMM");
                String sMenuFleNm = "";
                int nCt = 0;

                dashWinView = obj as DashWinView;                                                
                
                param.Add("sqlId", "SelectDashMenuList");
                param.Add("pYm", sYm);
                param.Add("id", Logs.strLogin_ID);

                dt = BizUtil.SelectList(param);
                dashWinView.grid.ItemsSource = dt;
                
                dashWinView.ctl1.Content = null;
                dashWinView.ctl2.Content = null;
                dashWinView.ctl3.Content = null;
                dashWinView.ctl4.Content = null;

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
                            else dashWinView.ctl1.Content = null;
                        }
                        else if (nCt == 2)
                        {
                            if (sMenuFleNm == "dashChart1") dashWinView.ctl2.Content = new UcChart01();
                            else if (sMenuFleNm == "dashChart2") dashWinView.ctl2.Content = new UcChart02();
                            else if (sMenuFleNm == "dashChart3") dashWinView.ctl2.Content = new UcChart03();
                            else if (sMenuFleNm == "dashChart4") dashWinView.ctl2.Content = new UcChart04();
                            else if (sMenuFleNm == "dashChart5") dashWinView.ctl2.Content = new UcChart05();
                            else if (sMenuFleNm == "dashChart6") dashWinView.ctl2.Content = new UcChart06();
                            else if (sMenuFleNm == "dashChart7") dashWinView.ctl2.Content = new UcChart07();
                            else dashWinView.ctl2.Content = null;
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
                            else dashWinView.ctl3.Content = null;
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
                            else dashWinView.ctl4.Content = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
                

        /// <summary>
        /// 메뉴 Show/Hide
        /// </summary>
        /// <param name="obj"></param>
        private void OnMenuShowHiden(object obj)
        {
            try
            {
                Storyboard sb;

                Button btnCtMenu = (Button)dashWinView.FindName("btnCtMenu");

                if (bMenuShowHiden)
                {
                    sb = dashWinView.FindResource("CtMenuShow") as Storyboard;
                    btnCtMenu.Style = Application.Current.Resources["btn_Quick_Slide_CLOSE"] as Style;
                }
                else
                {
                    sb = dashWinView.FindResource("CtMenuHiden") as Storyboard;
                    btnCtMenu.Style = Application.Current.Resources["btn_Quick_Slide_OPEN"] as Style;
                }

                sb.Begin(dashWinView);
                bMenuShowHiden = !bMenuShowHiden;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            Hashtable param = new Hashtable();
            int nUpdCnt = 0;

            //그리드 저장
            DataTable dt = dashWinView.grid.ItemsSource as DataTable;

            foreach (DataRow rowChk in dt.Rows)
            {
                if ("Y".Equals(rowChk["CHK"].ToString()))
                {
                    nUpdCnt++;
                }
            }

            if (nUpdCnt > 0 && nUpdCnt <= 4)
            {
                param.Add("id", Logs.strLogin_ID);
                param.Add("sqlId", "DeleteUserDashMnu");
            }
            else
            {
                if (nUpdCnt > 4)
                {
                    Messages.ShowErrMsgBox("선택은 4개를 초과하실 수 없습니다.");
                }
                else
                {
                    Messages.ShowErrMsgBox("선택하신 내용이 없습니다.(1~4개 선택가능)");
                }

                return;
            }

            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            //저장처리
            try
            {
                //사용자별 기존내용 삭제
                BizUtil.Update(param);

                foreach (DataRow row in dt.Rows)
                {
                    param = new Hashtable();

                    if ("Y".Equals(row["CHK"].ToString()))
                    {
                        param.Add("mnuCd", row["MNU_CD"].ToString());
                        param.Add("id", Logs.strLogin_ID);

                        param.Add("sqlId", "InsertUserDashMnu");
                        BizUtil.Update(param);
                    }

                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.ToString());
                return;
            }


            //저장처리 성공
            Messages.ShowOkMsgBox();

            //재조회
            InitModel(obj);

        }



        #endregion



    }
}
