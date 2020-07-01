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

namespace GTI.WFMS.Modules.Blk.ViewModel
{
    public class Blk02DtlViewModel : INotifyPropertyChanged 
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

                if (propertyName == "UPPER_FTR_CDE")
                {
                    BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);
                }
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


        #endregion


        #region ==========  Member 정의 ==========
        Blk02DtlView blk02DtlView;

        ComboBoxEdit cbMNG_CDE; 
        ComboBoxEdit cbUPPER_FTR_CDE; 
        ComboBoxEdit cbUPPER_FTR_IDN; 

        Button btnBack;
        Button btnDelete;
        Button btnSave;
        
        #endregion




        /// 생성자
        public Blk02DtlViewModel()
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

                blk02DtlView = obj as Blk02DtlView;
                cbMNG_CDE = blk02DtlView.cbMNG_CDE;       //관리기관
                cbUPPER_FTR_CDE = blk02DtlView.cbUPPER_FTR_CDE;       //상위블록
                cbUPPER_FTR_IDN = blk02DtlView.cbUPPER_FTR_IDN;       //상위블록

                btnBack = blk02DtlView.btnBack;
                btnDelete = blk02DtlView.btnDelete;
                btnSave = blk02DtlView.btnSave;
                
                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();
              
                // 4.초기조회
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectBlk02Dtl");
                param.Add("FTR_CDE", Dtl.FTR_CDE);
                param.Add("FTR_IDN", Dtl.FTR_IDN);

                Dtl = BizUtil.SelectObject(param) as BlkDtl;
                               
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
            if (!BizUtil.ValidReq(blk02DtlView)) return;


            if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

            try
            {
                BizUtil.Update2(Dtl, "updateBlk02Dtl");
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
                //Blk02DtlViewMdl mdl = new Blk02DtlViewMdl(Dtl.FTR_CDE, Dtl.FTR_IDN);
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
                BizUtil.Update2(Dtl, "deleteBlk02Dtl");
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
                Func<DataRow, bool> filter = Row => (Row.Field<string>("FTR_CDE").Contains("BZ001") );//대블록
                BizUtil.SetCombo(cbUPPER_FTR_CDE, "Select_FTR_LIST", "FTR_CDE", "FTR_NAM", null, filter);

                // cbUPPER_FTR_IDN 상위블록
                BizUtil.SetFTR_IDN(Dtl.UPPER_FTR_CDE, cbUPPER_FTR_IDN);

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



        #endregion


    }
}
