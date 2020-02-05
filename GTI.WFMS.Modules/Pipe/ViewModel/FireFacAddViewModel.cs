using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTI.WFMS.Modules.Pipe.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Pipe.ViewModel
{
    public class FireFacAddViewModel : FireFacDtl
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
        FireFacAddView fireFacAddView;
        ComboBoxEdit cbFTR_CDE; DataTable dtFTR_CDE = new DataTable();		//지형지물
        ComboBoxEdit cbHJD_CDE; DataTable dtHJD_CDE = new DataTable();		//행정동
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();		//관리기관
        ComboBoxEdit cbMOF_CDE; DataTable dtMOF_CDE = new DataTable();		//형식

        Button btnBack;
        Button btnSave;
        
        #endregion




        /// 생성자
        public FireFacAddViewModel()
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
            try
            {
                // 0.화면객체인스턴스화
                if (obj == null) return;
                var values = (object[])obj;

                fireFacAddView = values[0] as FireFacAddView;
                cbFTR_CDE = fireFacAddView.cbFTR_CDE;   //지형지물
                cbHJD_CDE = fireFacAddView.cbHJD_CDE;   //행정동
                cbMNG_CDE = fireFacAddView.cbMNG_CDE;   //관리기관
                cbMOF_CDE = fireFacAddView.cbMOF_CDE;   //형식

                btnBack = fireFacAddView.btnBack;
                btnSave = fireFacAddView.btnSave;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();

                // 4.초기조회 - 신규관리번호 채번
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectFireFacFTR_IDN");

                FireFacDtl result = new FireFacDtl();
                result = BizUtil.SelectObject(param) as FireFacDtl;


                //채번결과 매칭
                this.FTR_IDN = result.FTR_IDN;
                this.FTR_CDE = "SA119";

                this.IST_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(fireFacAddView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(this, "insertFireFacDtl");
            }
            catch (Exception )
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
                // cbFTR_CDE 지형지물
                //BizUtil.SetCombo(cbFTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", false);

                DataTable dt = new DataTable();

                dt.Columns.Add("FTR_CDE", typeof(String));
                dt.Columns.Add("FTR_NAM", typeof(String));

                DataRow dr = dt.NewRow();
                dr["FTR_CDE"] = "SA118";
                dr["FTR_NAM"] = "급수전";
                dt.Rows.InsertAt(dr, 0);

                dr = dt.NewRow();
                dr["FTR_CDE"] = "SA119";
                dr["FTR_NAM"] = "소화전";
                dt.Rows.InsertAt(dr, 0);


                // combo객체 Cd/Nm 필드매핑
                cbFTR_CDE.DisplayMember = "FTR_NAM";
                cbFTR_CDE.ValueMember = "FTR_CDE";

                cbFTR_CDE.ItemsSource = dt;
                cbFTR_CDE.SelectedIndex = 0;

                // cbHJD_CDE 행정동
                BizUtil.SetCombo(cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);


                // cbMOF_CDE 형식
                BizUtil.SetCmbCode(cbMOF_CDE, "MOF_CDE", true, "250019");

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
