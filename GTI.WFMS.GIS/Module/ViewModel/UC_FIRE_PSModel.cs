﻿using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Pipe.Model;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.GIS.Module.ViewModel
{
    public class UC_FIRE_PSModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propNm)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propNm));
            }
        }



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public RelayCommand<object> LoadedCommand { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }

        /// <summary>
        /// 시설물 키 : FctDtl과 별도로 관리됨
        /// </summary>
        private string __FTR_CDE;
        public string FTR_CDE
        {
            get { return __FTR_CDE; }
            set
            {
                this.__FTR_CDE = value;
                OnPropertyChanged("FTR_CDE");
            }
        }
        private string __FTR_IDN;
        public string FTR_IDN
        {
            get { return __FTR_IDN; }
            set
            {
                this.__FTR_IDN = value;
                OnPropertyChanged("FTR_IDN");
            }
        }


        private FireFacDtl fctDtl = new FireFacDtl();
        public FireFacDtl FctDtl
        {
            get{ return fctDtl;}
            set
            {
                fctDtl = value;
                OnPropertyChanged("FctDtl");
            }
        }

        #endregion


        #region ==========  Member 정의 ==========
        UC_FIRE_PS uC_FIRE_PS;
        Button btnSave;
        ComboBoxEdit cbFTR_CDE;

        #endregion




        /// 생성자
        public UC_FIRE_PSModel()
        {
            this.LoadedCommand = new RelayCommand<object>(OnLoaded);
            this.SaveCommand = new RelayCommand<object>(OnSave);          
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

                uC_FIRE_PS = obj as UC_FIRE_PS;

                btnSave = uC_FIRE_PS.btnSave;
                cbFTR_CDE = uC_FIRE_PS.cbFTR_CDE;

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                //permissionApply();

                // 4.초기조회
                InitModel();
                               
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
            if (!BizUtil.ValidReq(uC_FIRE_PS)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                FctDtl.FTR_CDE = this.FTR_CDE;
                FctDtl.FTR_IDN = Convert.ToInt32(this.FTR_IDN); //신규위치 및 기존위치 정보만 있을수 있으므로 shape의 관리번호를 기준으로한다.
                BizUtil.Update2(FctDtl, "SaveFireFacDtl");
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + e.Message);
                return;
            }

            Messages.ShowOkMsgBox();
            InitModel();
        }



        #endregion

        #region ============= 메소드정의 ================

        // 초기조회
        private void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFireFacDtl");
            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);

            FireFacDtl result = BizUtil.SelectObject(param) as FireFacDtl;
            if (result != null)
            {
                this.FctDtl = result;
            }
        }


        /// <summary>
        /// 초기화 바인딩
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
                //cbFTR_CDE.SelectedIndex = 0;


                // cbHJD_CDE 행정동
                BizUtil.SetCombo(uC_FIRE_PS.cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", true);

                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(uC_FIRE_PS.cbMNG_CDE, "MNG_CDE", true);

                // cbMOF_CDE 형식
                BizUtil.SetCmbCode(uC_FIRE_PS.cbMOF_CDE, "MOF_CDE", true, "250019");

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