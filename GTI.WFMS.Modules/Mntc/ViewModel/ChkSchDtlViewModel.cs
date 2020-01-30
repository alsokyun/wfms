using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{


    public class ChkSchDtlViewModel : ChscMaDtl
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand<object> ApprCmd { get; set; }
        

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
                


                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();




                // 4.초기조회
                //DataTable dt = new DataTable();
                Hashtable param = new Hashtable();
                param.Add("sqlId", "SelectChscMaList");
                param.Add("SCL_NUM", this.SCL_NUM);

                ChscMaDtl result = new ChscMaDtl();
                result = BizUtil.SelectObject(param) as ChscMaDtl;



                //결과를 뷰모델멤버로 매칭
                Type dbmodel = result.GetType();
                Type model = this.GetType();

                //모델프로퍼티 순회
                foreach (PropertyInfo prop in model.GetProperties())
                {
                    string propName = prop.Name;
                    //db프로퍼티 순회
                    foreach (PropertyInfo dbprop in dbmodel.GetProperties())
                    {
                        string colName = dbprop.Name;
                        var colValue = dbprop.GetValue(result, null);
                        if (colName.Equals(propName))
                        {
                            try
                            {
                                prop.SetValue(this, Convert.ChangeType(colValue, prop.PropertyType));
                            }
                            catch (Exception)
                            {
                                //형변환오류는 세팅하지 않는다..
                            }
                        }
                    }
                    Console.WriteLine(propName + " - " + prop.GetValue(this, null));
                }
            });

            //점검저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(chkSchDtlView)) return;


                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    BizUtil.Update2(this, "SaveChscMaDtl");
                }
                catch (Exception e)
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
                param.Add("SCL_NUM", this.SCL_NUM);

                Hashtable result = BizUtil.SelectLists(param);
                DataTable dt = new DataTable();

                try
                {
                    dt = result["dt"] as DataTable;
                    if (dt.Rows.Count > 0)
                    {
                        Messages.ShowErrMsgBox("점검시설물이 존재합니다.");
                        return;
                    }
                }
                catch (Exception) { }

                // 1.삭제처리
                if (Messages.ShowYesNoMsgBox("점검일정을 삭제하시겠습니까?") != MessageBoxResult.Yes) return;
                try
                {
                    BizUtil.Update2(this, "DeleteChscMaDtl");
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
                    BizUtil.Update2(this, "UpdateChscMaAppr");
                }
                catch (Exception e)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다.");
                    return;
                }

                Messages.ShowOkMsgBox();
                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            });

        }





        #region ============= 메소드정의 ================


        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "MNG_CDE", true);
                //점검구분
                BizUtil.SetCmbCode(cbSCL_CDE, "SCL_CDE", true);
                

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
