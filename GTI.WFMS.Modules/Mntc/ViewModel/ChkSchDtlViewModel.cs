using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{


    public class ChkSchDtlViewModel : INotifyPropertyChanged
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
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> ApprCmd { get; set; }

        public RelayCommand<object> AddFtrSelCmd { get; set; }
        public RelayCommand<object> GrdDelCmd { get; set; }
        public RelayCommand<object> GrdSaveCmd { get; set; }



        //점검마스터
        private ChscMaDtl dtl = new ChscMaDtl();
        public ChscMaDtl Dtl
        {
            get { return dtl; }
            set
            {
                dtl = value;
                OnPropertyChanged("Dtl");
            }
        }
        //점검결과
        private ObservableCollection<ChscResultDtl> __GrdLst;
        public ObservableCollection<ChscResultDtl> GrdLst
        {
            get { return __GrdLst; }
            set
            {
                if (value == __GrdLst) return;
                __GrdLst = value;
                OnPropertyChanged("GrdLst");
            }
        }


        #endregion


        #region ==========  Member 정의 ==========
        ChkSchDtlView chkSchDtlView;
        ComboBoxEdit cbMNG_CDE; DataTable dtMNG_CDE = new DataTable();
        ComboBoxEdit cbSCL_CDE; DataTable dtSCL_CDE = new DataTable();
        
        Button btnDelete;
        Button btnSave;
        Button btnClose;
        

        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public ChkSchDtlViewModel()
        {
            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                chkSchDtlView = obj as ChkSchDtlView;

                cbMNG_CDE = chkSchDtlView.cbMNG_CDE;
                cbSCL_CDE = chkSchDtlView.cbSCL_CDE;
                
                btnDelete = chkSchDtlView.btnDelete;
                btnSave = chkSchDtlView.btnSave;
                btnClose = chkSchDtlView.btnClose;

                GrdLst = new ObservableCollection<ChscResultDtl>();

                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();


                //4.초기조히
                initModel();



            });

            //점검저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(chkSchDtlView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트 별로로 세팅
                    Dtl.CHK_CTNT = new TextRange(chkSchDtlView.richBox.Document.ContentStart, chkSchDtlView.richBox.Document.ContentEnd).Text.Trim();
                    BizUtil.Update2(Dtl, "SaveChscMaDtl");
                }
                catch (Exception )
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }

                Messages.ShowOkMsgBox();
                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            });
            
            //점검삭제
            this.DeleteCommand = new DelegateCommand<object>(delegate (object obj) {
                //0.삭제전 체크
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectChscResultList");
                param.Add("SCL_NUM", Dtl.SCL_NUM);

                Hashtable result = BizUtil.SelectLists(param);
                DataTable dt = new DataTable();

                try
                {
                    dt = result["dt"] as DataTable;
                    if (dt.Rows.Count > 0)
                    {
                        //Messages.ShowErrMsgBox("점검시설물이 존재합니다.");
                        //return;
                        foreach (DataRow row in dt.Rows)
                        {
                            //0.점검사진삭제
                            //a.FIL_SEQ 첨부파일삭제
                            BizUtil.DelFileSeq(row["FIL_SEQ"]);

                            //b.FILE_MAP 업무파일매핑삭제
                            param = new Hashtable();
                            param.Add("sqlId", "DeleteFileMap");
                            param.Add("BIZ_ID", row["FTR_CDE"].ToString() + row["FTR_IDN"].ToString());
                            param.Add("FIL_SEQ", row["FIL_SEQ"]);
                            BizUtil.Update(param);

                            //0.소모품삭제
                            PdjtHtDtl dtl = new PdjtHtDtl();
                            dtl.SCL_NUM = Convert.ToInt32(row["SCL_NUM"]) ;
                            dtl.FTR_CDE = row["FTR_CDE"].ToString();
                            dtl.FTR_IDN = Convert.ToInt32(row["FTR_IDN"]); 
                            dtl.SEQ = Convert.ToInt32(row["SEQ"]);
                            BizUtil.Update2(dtl, "DeletePdjtHt");

                            //1.데이터삭제
                            param.Clear();
                            param.Add("SCL_NUM", row["SCL_NUM"]);
                            param.Add("FTR_CDE", row["FTR_CDE"]);
                            param.Add("FTR_IDN", Convert.ToInt32(row["FTR_IDN"]));
                            param.Add("sqlId", "DeleteChscResult");
                            param.Add("SEQ", Convert.ToInt32(row["SEQ"]));
                            BizUtil.Update(param);

                        }
                    }
                }
                catch (Exception) { }

                // 1.삭제처리
                if (Messages.ShowYesNoMsgBox("점검일정을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
                try
                {
                    BizUtil.Update2(Dtl, "DeleteChscMaDtl");
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다." + ex.ToString());
                    return;
                }
                Messages.ShowOkMsgBox();

                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            });

            //점검승인
            this.ApprCmd = new DelegateCommand<object>(delegate (object obj) {
                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(chkSchDtlView)) return;


                if (Messages.ShowYesNoMsgBox("점검승인 하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    BizUtil.Update2(Dtl, "UpdateChscMaAppr");
                }
                catch (Exception )
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }

                Messages.ShowOkMsgBox();
                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            });


            //행추가(시설물선택팝업)            
            this.AddFtrSelCmd = new RelayCommand<object>(delegate (object obj) {

                try
                {
                    // 지형지물팝업 윈도우
                    FtrSelView ftrSelView = new FtrSelView(null);
                    ftrSelView.Owner = Window.GetWindow(chkSchDtlView);


                    //FTR_IDN 리턴
                    if (ftrSelView.ShowDialog() is bool)
                    {
                        string FTR_IDN = ftrSelView.txtFTR_IDN.Text;
                        string FTR_CDE = ftrSelView.txtFTR_CDE.Text;
                        string FTR_NAM = ftrSelView.txtFTR_NAM.Text;
                        string HJD_NAM = ftrSelView.txtHJD_NAM.Text;


                        //저장버튼으로 닫힘
                        if (!FmsUtil.IsNull(FTR_IDN))
                        {
                            AddFtrRow(FTR_IDN, FTR_CDE, FTR_NAM, HJD_NAM); //시설물 한건추가
                        }
                        //닫기버튼으로 닫힘
                    }

                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox(ex.ToString());
                }
            });


            // 그리드저장 
            this.GrdSaveCmd = new RelayCommand<object>(delegate (object obj) {

                bool isChecked = false;
                foreach (ChscResultDtl row in GrdLst)
                {
                    if ("Y".Equals(row.CHK))
                    {
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked)
                {
                    Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                Hashtable param = new Hashtable();

                //1.그리드 저장
                foreach (ChscResultDtl row in GrdLst)
                {
                    if (row.CHK != "Y") continue;

                    try
                    {
                        row.SCL_NUM = Dtl.SCL_NUM;
                        BizUtil.Update2(row, "SaveChscResult");
                    }
                    catch (Exception)
                    {
                        Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                        return;
                    }
                }

                //2.점검마스터상태 변경
                Hashtable pa = new Hashtable();
                pa.Add("sqlId", "UpdateChscMaRes");
                pa.Add("SCL_NUM", Dtl.SCL_NUM);
                BizUtil.Update(pa);



                //저장처리성공
                Messages.ShowOkMsgBox();

                //재조회
                initModel();
            });


            // 행삭제 GrdDelCmd 
            this.GrdDelCmd = new RelayCommand<object>(delegate (object obj) {

                //데이터 직접삭제처리
                try
                {
                    bool isChecked = false;
                    foreach (ChscResultDtl row in GrdLst)
                    {
                        if ("Y".Equals(row.CHK))
                        {
                            isChecked = true;
                            break;
                        }
                    }
                    if (!isChecked)
                    {
                        Messages.ShowInfoMsgBox("선택된 항목이 없습니다.");
                        return;
                    }

                    if (Messages.ShowYesNoMsgBox("선택 항목을 삭제 하시겠습니까?") == MessageBoxResult.Yes)
                    {
                        foreach (ChscResultDtl row in GrdLst)
                        {
                            Hashtable param = new Hashtable();
                            try
                            {
                                if ("Y".Equals(row.CHK))
                                {
                                    if (row.SEQ == 0)
                                    {
                                        //그리드행만 삭제
                                        GrdLst.RemoveAt(GrdLst.IndexOf(row));
                                        return;
                                    }
                                    else
                                    {
                                        //0.점검사진삭제
                                        //a.FIL_SEQ 첨부파일삭제
                                        BizUtil.DelFileSeq(row.FIL_SEQ);

                                        //b.FILE_MAP 업무파일매핑삭제
                                        param = new Hashtable();
                                        param.Add("sqlId", "DeleteFileMap");
                                        param.Add("BIZ_ID", row.FTR_CDE + row.FTR_IDN);
                                        param.Add("FIL_SEQ", row.FIL_SEQ);
                                        BizUtil.Update(param);

                                        //0.소모품삭제
                                        PdjtHtDtl dtl = new PdjtHtDtl();
                                        dtl.SCL_NUM = row.SCL_NUM;
                                        dtl.FTR_CDE = row.FTR_CDE;
                                        dtl.FTR_IDN = row.FTR_IDN;
                                        dtl.SEQ = row.SEQ;
                                        BizUtil.Update2(dtl, "DeletePdjtHt");

                                        //1.데이터삭제
                                        param.Clear();
                                        param.Add("SCL_NUM", row.SCL_NUM);
                                        param.Add("FTR_CDE", row.FTR_CDE);
                                        param.Add("FTR_IDN", row.FTR_IDN);
                                        param.Add("SEQ", row.SEQ);
                                        param.Add("sqlId", "DeleteChscResult");
                                        param.Add("SEQ", Convert.ToInt32(row.SEQ));
                                        BizUtil.Update(param);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Messages.ShowErrMsgBox("삭제 처리중 오류가 발생하였습니다.");
                                return;
                            }
                        }

                        Messages.ShowOkMsgBox();

                        //재조회
                        initModel();
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBoxLog(ex);
                }
            });


        }





        #region ============= 메소드정의 ================

        /// <summary>
        /// 조회작업
        /// </summary>        
        private void initModel()
        {
            // 4.초기조회
            //a.마스터
            //DataTable dt = new DataTable();
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectChscMaList");
            param.Add("SCL_NUM", Dtl.SCL_NUM);

            Dtl = BizUtil.SelectObject(param) as ChscMaDtl;

            //점검결과 다큐먼트 수동세팅
            try
            {
                chkSchDtlView.richBox.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(Dtl.CHK_CTNT.Trim());
                chkSchDtlView.richBox.Document.Blocks.Add(paragraph);
            }
            catch (Exception){}


            //b.점검결과
            param = new Hashtable();
            param.Add("sqlId", "SelectChscResultList");
            param.Add("SCL_NUM", Dtl.SCL_NUM);

            GrdLst = new ObservableCollection<ChscResultDtl>(BizUtil.SelectListObj<ChscResultDtl>(param));

            // 1.1 점검결과 첫행선택
            if (GrdLst.Count > 0)
            {
                //SEL_FTR_CDE = GrdLst[0].FTR_CDE.ToString();
                //SEL_FTR_IDN = dt.Rows[0]["FTR_IDN"].ToString();
                //SEL_SEQ = dt.Rows[0]["SEQ"].ToString();
            }
        }


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "선택");
                //점검구분
                BizUtil.SetCmbCode(cbSCL_CDE, "250105", "선택");
                

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



        //시설물 한건 row 추가
        private void AddFtrRow(string fTR_IDN, string fTR_CDE, string fTR_NAM, string hJD_NAM)
        {
            if (FmsUtil.IsNull(fTR_IDN))
            {
                Messages.ShowInfoMsgBox("관리번호가 없습니다.");
                return;
            }

            ChscResultDtl drNew = new ChscResultDtl();
            drNew.FTR_IDN = Convert.ToInt32(fTR_IDN) ;
            drNew.FTR_CDE = fTR_CDE;
            drNew.HJD_NAM = hJD_NAM;
            drNew.FTR_NAM = fTR_NAM;
            drNew.RPR_YMD = Convert.ToDateTime(DateTime.Today).ToString("yyyyMMdd");

            drNew.CHK = "Y";
            GrdLst.Add(drNew);

        }


        #endregion


    }
}
