using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    class WtlPipeAddViewModel : PipeDtl
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }


        #endregion


        #region ==========  Member 정의 ==========
        WtlPipeAddView wtlPipeAddView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();
        ComboBoxEdit cbMOP_CDE; DataTable dtMOP_CDE = new DataTable();
        ComboBoxEdit cbJHT_CDE; DataTable dtJHT_CDE = new DataTable();
        ComboBoxEdit cbSAA_CDE; DataTable dtSAA_CDE = new DataTable();
        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public WtlPipeAddViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(OnLoaded);
            this.SaveCommand = new DelegateCommand<object>(OnSave);
            this.BackCommand = new DelegateCommand<object>(OnBack);
            
        }





        #region ==========  이벤트 핸들러 ==========

        /// <summary>
        /// 로딩작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnLoaded(object obj)
        {
            //throw new NotImplementedException();

            // 0.화면객체인스턴스화
            if (obj == null) return;
            var values = (object[])obj;

            wtlPipeAddView = values[0] as WtlPipeAddView;
            cbMNG_CDE = wtlPipeAddView.cbMNG_CDE;
            cbHJD_CDE = wtlPipeAddView.cbHJD_CDE;
            cbMOP_CDE = wtlPipeAddView.cbMOP_CDE;
            cbJHT_CDE = wtlPipeAddView.cbJHT_CDE;
            cbSAA_CDE = wtlPipeAddView.cbSAA_CDE;
            btnBack = wtlPipeAddView.btnBack;
            btnSave = wtlPipeAddView.btnSave;
            

            //2.화면데이터객체 초기화
            InitDataBinding();


            //3.권한처리
            permissionApply();




            // 4.초기조회 - 신규관리번호 채번
            //DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWtlPipeFTR_IDN");
            param.Add("FTR_CDE", "SA001");

            PipeDtl result = new PipeDtl();
            result = BizUtil.SelectObject(param) as PipeDtl;

            //채번결과 매칭
            this.FTR_IDN = result.FTR_IDN;
            this.FTR_CDE = "SA001";
            this.IST_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {
            //0.Validation 체크


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertWtlPipeDtl");
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                return;
            }
            Messages.ShowOkMsgBox();


            BackCommand.Execute(null); //닫기
        }



        /// <summary>
        /// 뒤로가기처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnBack(object obj)
        {
            //MessageBox.Show("OnBack");
            btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }


        #endregion





        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbMNG_CDE
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMOP_CDE
                BizUtil.SetCmbCode(cbMOP_CDE, "MOP_CDE", true, "250102");

                // cbJHT_CDE
                BizUtil.SetCmbCode(cbJHT_CDE, "JHT_CDE", true);

                // cbSAA_CDE
                BizUtil.SetCmbCode(cbSAA_CDE, "SAA_CDE", true);
                
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }
        }


        /// <summary>
        /// 화면 권한처리
        /// </summary>
        private void permissionApply()
        {
            try
            {
                string strPermission = Logs.htPermission[Logs.strFocusMNU_CD].ToString();
                switch (strPermission)
                {
                    case "W":
                        break;
                    case "R":
                        btnSave.Visibility = Visibility.Collapsed;
                        break;
                    case "N":
                        break;
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBoxLog(ex);
            }

        }

        #endregion


    }
}
