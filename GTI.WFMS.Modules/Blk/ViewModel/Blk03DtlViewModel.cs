using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using Prism.Commands;
using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using GTI.WFMS.Modules.Blk.View;
using GTI.WFMS.Models.Blk.Model;
using System.ComponentModel;
using System.Collections.Generic;
using GTI.WFMS.Models.Cmm.Model;
using System.Collections.ObjectModel;
using System.Reflection;

namespace GTI.WFMS.Modules.Blk.ViewModel
{
    public class Blk03DtlViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 인터페이스 구현부분
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> PrintCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> BackCommand { get; set; }

        private BlkDtl dtl = new BlkDtl ();
        public BlkDtl Dtl
        {
            get { return dtl; }
            set
            {
                dtl = value;
                OnPropertyChanged("Dtl");
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
        Blk03DtlView blk03DtlView;

        ComboBoxEdit cbMNG_CDE;
        static ComboBoxEdit cbUPPER_FTR_CDE;
        ComboBoxEdit cbUPPER_FTR_IDN;

        Button btnBack;
        Button btnDelete;
        Button btnSave;
        
        #endregion




        /// 생성자
        public Blk03DtlViewModel()
        {
            LoadedCommand = new DelegateCommand<object>(OnLoaded);
            SaveCommand = new DelegateCommand<object>(OnSave);
            PrintCommand = new DelegateCommand<object>(OnPrint);
            DeleteCommand = new DelegateCommand<object>(OnDelete);
            BackCommand = new DelegateCommand<object>(OnBack);

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

                blk03DtlView = obj as Blk03DtlView;
                cbMNG_CDE = blk03DtlView.cbMNG_CDE;       //관리기관
                cbUPPER_FTR_CDE = blk03DtlView.cbUPPER_FTR_CDE;       //상위블록
                cbUPPER_FTR_IDN = blk03DtlView.cbUPPER_FTR_IDN;

                btnBack = blk03DtlView.btnBack;
                btnDelete = blk03DtlView.btnDelete;
                btnSave = blk03DtlView.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();
              
                // 4.초기조회
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectBlk03Dtl");
                param.Add("FTR_CDE", Dtl.FTR_CDE);
                param.Add("FTR_IDN", Dtl.FTR_IDN);

                Dtl = BizUtil.SelectObject(param) as BlkDtl;


                // cbUPPER_FTR_IDN 상위블록
                BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);

                // 콤보변경이벤트설정
                cbUPPER_FTR_CDE.SelectedIndexChanged += OnUpFtrCdeChanged;



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        //블록코드 변경시 이벤트핸들러
        private void OnUpFtrCdeChanged(object sender, RoutedEventArgs e)
        {
            BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);
        }



        /// <summary>
        /// 저장작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnSave(object obj)
        {

            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            if (!BizUtil.ValidReq(blk03DtlView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "updateBlk03Dtl");
            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                return;
            }
            Messages.ShowOkMsgBox();

        }

        /// <summary>
        /// 인쇄작업
        /// </summary>
        /// <param name="obj"></param>
        private void OnPrint(object obj)
        {
            // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
            //if (!BizUtil.ValidReq(fireFacDtlView)) return;

            //if (Messages.ShowYesNoMsgBox("인쇄하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                //0.Datasource 생성
                //Blk03DtlViewMdl mdl = new Blk03DtlViewMdl(Dtl.FTR_CDE, Dtl.FTR_IDN);
                //1.Report 호출
                //WtrTrkReport report = new WtrTrkReport();
                ObjectDataSource ods = new ObjectDataSource();
                ods.Name = "objectDataSource1";
                //ods.DataSource = mdl;

                //report.DataSource = ods;
                //report.ShowPreviewDialog();

            }
            catch (Exception)
            {
                Messages.ShowErrMsgBox("인쇄 처리중 오류가 발생하였습니다.");
                return;
            }
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

            param.Add("FTR_CDE", Dtl.FTR_CDE);
            param.Add("FTR_IDN", Dtl.FTR_IDN);
            param.Add("BIZ_ID", string.Concat(Dtl.FTR_CDE , Dtl.FTR_IDN) );

            Hashtable result = BizUtil.SelectLists(param);
            DataTable dt  = new DataTable();

            //try
            //{
            //    dt = result["dt"] as DataTable;
            //    if (dt.Rows.Count > 0)
            //    {
            //        Messages.ShowInfoMsgBox("파일첨부내역이 존재합니다.");
            //        return;
            //    }
            //}
            //catch (Exception) { }




            // 1.삭제처리
            if (Messages.ShowYesNoMsgBox("블록을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
            try
            {
                BizUtil.Update2(Dtl, "deleteBlk03Dtl");
            }
            catch (Exception e)
            {
                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다." + e.Message);
                return;
            }
            Messages.ShowOkMsgBox();



            BackCommand.Execute(null);

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
                // cbMNG_CDE 관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "선택");

                // cbUPPER_FTR_CDE 상위블록코드
                //Func<DataRow, bool> filter = Row => (Row.Field<string>("FTR_CDE").Contains("BZ001") || Row.Field<string>("FTR_CDE").Contains("BZ002"));//대블록,중블록
                //BizUtil.SetCombo(cbUPPER_FTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", null, filter);


                cbUPPER_FTR_CDE.DisplayMember = "FTR_NAM";
                cbUPPER_FTR_CDE.ValueMember = "FTR_CDE";
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
                        btnDelete.Visibility = Visibility.Collapsed;
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




        //블럭관리번호 콤보생성
        public static void SetFTR_IDN(string uPPER_FTR_CDE)
        {
            if (FmsUtil.IsNull(uPPER_FTR_CDE)) return;//상위코드없으면 콤보채우지 않는다

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectUpBlk");
            param.Add("FTR_CDE", uPPER_FTR_CDE);
            List<BlkDtl> lst = (List<BlkDtl>)BizUtil.SelectListObj<BlkDtl>(param);

            cbUPPER_FTR_CDE.DisplayMember = "BLK_NM";
            cbUPPER_FTR_CDE.ValueMember = "FTR_IDN";
            cbUPPER_FTR_CDE.ItemsSource = lst;
        }



        #endregion


    }
}
