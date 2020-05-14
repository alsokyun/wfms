using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Mntc.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GTI.WFMS.Modules.Mntc.ViewModel
{


    public class ChkSchAddViewModel : DependencyObject
    {


        #region ==========  Properties 정의 ==========
        /// <summary>
        /// Loaded Event
        /// </summary>
        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public ChscMaDtl Dtl
        {
            get {return dtl; }
            set {dtl = value; }
        }

        /* RichTextBox를 바인딩하기위한 부분 : 사용안함
        public FlowDocument Doc
        {
            get {return (FlowDocument)GetValue(DocProperty); }
            set {SetValue(DocProperty, value); }
        }
        public static readonly DependencyProperty DocProperty 
            = DependencyProperty.Register("Doc", typeof(FlowDocument), typeof(ChkSchAddViewModel), new PropertyMetadata(OnDocumentChanged));
        public static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
         */


        #endregion


        #region ==========  Member 정의 ==========
        ChkSchAddView chkSchAddView;
        ComboBoxEdit cbMNG_CDE; 
        ComboBoxEdit cbSCL_CDE; 
        
        Button btnSave;
        Button btnClose;

        private ChscMaDtl dtl;
        #endregion



        /// <summary>
        /// 생성자
        /// </summary>
        public ChkSchAddViewModel()
        {
            /* RichTextBox를 바인딩하기위한 부분 : 사용안함
            FlowDocument d = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            Run a = new Run();
            a.Text = "ASDFASDFASDFASDFASDF";
            paragraph.Inlines.Add(a);
            d.Blocks.Add(paragraph);

            Doc = d;
            */

            dtl = new ChscMaDtl();

            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj) {
                // 0.화면객체인스턴스화
                if (obj == null) return;

                chkSchAddView = obj as ChkSchAddView;

                cbMNG_CDE = chkSchAddView.cbMNG_CDE;
                cbSCL_CDE = chkSchAddView.cbSCL_CDE;
                
                btnSave = chkSchAddView.btnSave;
                btnClose = chkSchAddView.btnClose;
                


                //2.화면데이터객체 초기화
                InitDataBinding();


                //3.권한처리
                permissionApply();


            });

            //신규저장
            this.SaveCommand = new DelegateCommand<object>(delegate (object obj) {

                // 필수체크 (Tag에 필수체크 표시한 EditBox, ComboBox 대상으로 수행)
                if (!BizUtil.ValidReq(chkSchAddView)) return;

                //날짜체크
                if(!BizUtil.ValidDateBtw(Dtl.STA_YMD, Dtl.END_YMD))
                {
                    Messages.ShowInfoMsgBox("From/To 일자를 확인하세요");
                    return;
                }

                if (Messages.ShowYesNoMsgBox("저장하시겠습니까?") != MessageBoxResult.Yes) return;

                try
                {
                    //다큐먼트는 따로 처리
                    this.Dtl.CHK_CTNT = new TextRange(chkSchAddView.richBox.Document.ContentStart, chkSchAddView.richBox.Document.ContentEnd).Text;
                    BizUtil.Update2(this.Dtl, "SaveChscMaDtl");
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.Message);
                    return;
                }

                Messages.ShowOkMsgBox();
                //화면닫기
                btnClose.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            });
            

        }





        #region ============= 메소드정의 ================

        public string GetContent(RichTextBox box)
        {
            TextRange range = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
            return range.Text;
        }



        /// <summary>
        /// 초기조회 및 바인딩
        /// </summary>
        private void InitDataBinding()
        {
            try
            {
                //관리기관
                BizUtil.SetCmbCode(cbMNG_CDE, "250101", "[선택하세요]");
                //점검구분
                BizUtil.SetCmbCode(cbSCL_CDE, "250105", "[선택하세요]");
                

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
