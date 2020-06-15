using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.GIS.Pop.ViewModel;
using GTI.WFMS.Models.Blk.Model;
using GTI.WFMS.Models.Cmm.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fclt.Model;
using GTI.WFMS.Models.Pipe.Model;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.GIS.Module.ViewModel
{
    public class UC_BLKS_ASModel : INotifyPropertyChanged
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


        private BlkDtl fctDtl = new BlkDtl();
        public BlkDtl FctDtl
        {
            get{ return fctDtl;}
            set
            {
                fctDtl = value;
                OnPropertyChanged("FctDtl");
            }
        }
        private List<FctDtl> itemLst = new List<FctDtl>();
        public List<FctDtl> ItemLst
        {
            get { return itemLst; }
            set
            {
                itemLst = value;
                OnPropertyChanged("ItemLst");
            }
        }

        #endregion


        #region ==========  Member 정의 ==========
        UC_BLKS_AS uC_BLKS_AS;
        Button btnSave;


        #endregion




        /// 생성자
        public UC_BLKS_ASModel()
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

                uC_BLKS_AS = obj as UC_BLKS_AS;

                btnSave = uC_BLKS_AS.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                //permissionApply();

                // 4.초기조회
                InitModel();

                // cbUPPER_FTR_IDN 상위블록
                BizUtil.SetFTR_IDN(FctDtl.UPPER_FTR_CDE, uC_BLKS_AS.cbUPPER_FTR_IDN);

                // 콤보변경이벤트설정
                uC_BLKS_AS.cbUPPER_FTR_CDE.SelectedIndexChanged += OnUpFtrCdeChanged;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        //블록코드 변경시 이벤트핸들러
        private void OnUpFtrCdeChanged(object sender, RoutedEventArgs e)
        {
            BizUtil.SetFTR_IDN(FctDtl.UPPER_FTR_CDE, uC_BLKS_AS.cbUPPER_FTR_IDN);
        }


        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(uC_BLKS_AS)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //1.시설물정보
                FctDtl.FTR_CDE = this.FTR_CDE;
                FctDtl.FTR_IDN = Convert.ToInt32(this.FTR_IDN); //신규위치 및 기존위치 정보만 있을수 있으므로 shape의 관리번호를 기준으로한다.
                BizUtil.Update2(FctDtl, "updateBlk03Dtl");

                //2.위치정보 - 위치편집한 경우만
                if (!FmsUtil.IsNull(GisCmm.WKT_POLYGON))
                {
                    GisCmm.SavePolygon(FctDtl.FTR_CDE, FctDtl.FTR_IDN.ToString(), "WTL_PURI_AS");
                    GisCmm.WKT_POLYGON = "";
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
            param.Add("sqlId", "SelectFileMapList");

            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);
            param.Add("BIZ_ID", string.Concat(this.FTR_CDE, this.FTR_IDN));

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt = new DataTable();

            try
            {
                dt = result["dt"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    Messages.ShowInfoMsgBox("파일첨부내역이 존재합니다.");
                    return;
                }
            }
            catch (Exception) { }
            


            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("소블록을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(this.FctDtl, "deleteBlk03Dtl");
            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
                return;
            }
            // 2.위치정보 삭제처리
            ContentControl cctl = uC_BLKS_AS.Parent as ContentControl;
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
            param.Add("sqlId", "SelectBlk03Dtl");
            param.Add("FTR_CDE", this.FTR_CDE);
            param.Add("FTR_IDN", this.FTR_IDN);

            BlkDtl result = BizUtil.SelectObject(param) as BlkDtl;
            if (result != null)
            {
                this.FctDtl = result;
            }
            else
            {
                //신규등록이면 상세화면표시
                if ("Y".Equals(uC_BLKS_AS.btnDel.Tag))
                {
                    uC_BLKS_AS.grid.Visibility = Visibility.Hidden; //DB데이터가 없으면 빈페이지표시
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
                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(uC_BLKS_AS.cbMNG_CDE, "250101", "선택");

                uC_BLKS_AS.cbUPPER_FTR_CDE.DisplayMember = "FTR_NAM";
                uC_BLKS_AS.cbUPPER_FTR_CDE.ValueMember = "FTR_CDE";
                Hashtable param = new Hashtable();
                param.Add("sqlId", "Select_FTR_LIST3");
                param.Add("FTR_CDE", "BZ");
                List<FctDtl> lst = BizUtil.SelectListObj<FctDtl>(param) as List<FctDtl>;

                ItemLst = lst.FindAll(f => !f.FTR_CDE.Contains("BZ003"));

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
