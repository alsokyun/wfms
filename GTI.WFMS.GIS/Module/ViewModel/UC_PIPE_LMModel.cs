﻿using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.GIS.Pop.ViewModel;
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
    public class UC_PIPE_LMModel : INotifyPropertyChanged
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
        public RelayCommand<object> DelCommand { get; set; }

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


        private PipeDtl fctDtl = new PipeDtl();
        public PipeDtl FctDtl
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
        UC_PIPE_LM uC_PIPE_LM;
        Button btnSave;


        #endregion




        /// 생성자
        public UC_PIPE_LMModel()
        {
            this.LoadedCommand = new RelayCommand<object>(OnLoaded);
            this.SaveCommand = new RelayCommand<object>(OnSave);          
            this.DelCommand = new RelayCommand<object>(OnDelete);
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

                uC_PIPE_LM = obj as UC_PIPE_LM;

                btnSave = uC_PIPE_LM.btnSave;
                
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
            if (!BizUtil.ValidReq(uC_PIPE_LM)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //1.시설물정보
                FctDtl.FTR_CDE = this.FTR_CDE;
                FctDtl.FTR_IDN = Convert.ToInt32(this.FTR_IDN); //신규위치 및 기존위치 정보만 있을수 있으므로 shape의 관리번호를 기준으로한다.
                BizUtil.Update2(FctDtl, "saveWtlPipeDtl");

                //2.위치정보 - 위치편집한 경우만
                if (!FmsUtil.IsNull(GisCmm.WKT_LINE))
                {
                    GisCmm.SavePolyline(FctDtl.FTR_CDE, FctDtl.FTR_IDN.ToString(), "WTL_PIPE_LM");
                    GisCmm.WKT_LINE = "";
                }

            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + e.Message);
                return;
            }

            Messages.ShowOkMsgBox();
            InitModel();
        }

        /// <summary>
        /// 삭제처리
        /// </summary>
        /// <param name="obj"></param>
        private void OnDelete(object obj)
        {
            //0.삭제전 체크
            Hashtable param = new Hashtable();
            param.Add("sqlId", "selectChscResSubList");
            param.Add("sqlId2", "SelectFileMapList");
            param.Add("sqlId3", "selectWtlLeakSubList");

            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);
            param.Add("BIZ_ID", string.Concat(this.FTR_CDE, this.FTR_IDN));

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();

            try
            {
                dt = result["dt"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowInfoMsgBox("유지보수내역이 존재합니다.");
                    return;
                }
            }
            catch (Exception) { }

            try
            {
                dt3 = result["dt3"] as DataTable;
                if (dt3.Rows.Count > 0)
                {
                    Messages.ShowInfoMsgBox("누수지점 및 복구내역이 존재합니다.");
                    return;
                }
            }
            catch (Exception) { }



            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("상수관로를 삭제하시겠습니까?") != MessageBoxResult.Yes) return;


            try
            {
                dt2 = result["dt2"] as DataTable;
                if (dt2.Rows.Count > 0)
                {
                    //Messages.ShowInfoMsgBox("파일첨부내역이 존재합니다.");
                    //return;

                    //첨부파일삭제
                    foreach (DataRow row in dt2.Rows)
                    {
                        //a.FIL_SEQ 첨부파일삭제
                        BizUtil.DelFileSeq(row["FIL_SEQ"]);

                        //b.FILE_MAP 업무파일매핑삭제
                        param = new Hashtable();
                        param.Add("sqlId", "DeleteFileMap");
                        param.Add("BIZ_ID", FTR_CDE + FTR_IDN);
                        param.Add("FIL_SEQ", row["FIL_SEQ"]);
                        BizUtil.Update(param);
                    }

                }
            }
            catch (Exception) { }



            try
            {
                BizUtil.Update2(this.FctDtl, "deleteWtlPipeDtl");
            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
                return;
            }
            // 2.위치정보 삭제처리
            ContentControl cctl = uC_PIPE_LM.Parent as ContentControl;
            EditWinViewModel editWinViewModel = ((((cctl.Parent as Grid).Parent as Grid).Parent as Grid).Parent as Window).DataContext as EditWinViewModel;
            editWinViewModel.OnDelCmd(null);


            //Messages.ShowOkMsgBox();
            //InitModel();

        }

        #endregion

        #region ============= 메소드정의 ================

        // 초기조회
        private void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectWtlPipeDtl2");
            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);

            PipeDtl result = BizUtil.SelectObject(param) as PipeDtl;
            if (result != null)
            {
                this.FctDtl = result;
            }
            else
            {
                //신규등록이면 상세화면표시
                if ("Y".Equals(uC_PIPE_LM.btnDel.Tag))
                {
                    uC_PIPE_LM.grid.Visibility = Visibility.Visible; //DB데이터가 없으면 빈페이지표시
                }
            }
        }


        /// <summary>
        /// 초기화 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                // cbHJD_CDE 행정동
                BizUtil.SetCombo(uC_PIPE_LM.cbHJD_CDE, "Select_ADAR_LIST", "HJD_CDE", "HJD_NAM", "선택");
                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(uC_PIPE_LM.cbMNG_CDE, "250101", "선택");
                // cbMOP_CDE
                BizUtil.SetCmbCode(uC_PIPE_LM.cbMOP_CDE, "250102", "선택");
                // cbJHT_CDE
                BizUtil.SetCmbCode(uC_PIPE_LM.cbJHT_CDE, "250026", "선택");
                // cbSAA_CDE
                BizUtil.SetCmbCode(uC_PIPE_LM.cbSAA_CDE, "250018", "선택");
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
