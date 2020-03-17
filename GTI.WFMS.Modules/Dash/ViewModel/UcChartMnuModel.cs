using DevExpress.Mvvm;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Dash.Model;
using GTI.WFMS.Modules.Dash.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Dash.ViewModel
{
    public class UcChartMnuModel : DashDtl
    {

        #region ==========  Properties 정의 ==========

        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> MenuShowHidenCommand { get; set; }
        #endregion

        #region ==========  Member 정의 ==========
        UcChartMnu ucChartMnu;
        
        #endregion

        /// 생성자
        public UcChartMnuModel()
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
        /// 메뉴 Show/Hide
        /// </summary>
        /// <param name="obj"></param>
        private void OnMenuShowHiden(object obj)
        {
            try
            {
               
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
                ucChartMnu = obj as UcChartMnu;

                // 4.초기조회
                Hashtable param = new Hashtable();
                var sYm = Convert.ToDateTime(DateTime.Today).ToString("yyyyMM");

                param.Add("sqlId", "SelectDashMenuList");
                param.Add("pYm", sYm);
                param.Add("id", Logs.strLogin_ID);

                DataTable dt = BizUtil.SelectList(param);

                ucChartMnu.grid.ItemsSource = dt;
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
            DataTable dt = ucChartMnu.grid.ItemsSource as DataTable;

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
