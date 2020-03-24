using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// SplyDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplyDtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        Thread thread;
        private string _SEQ;

        public SplyDtlView(string SEQ)
        {
            InitializeComponent();

            _SEQ = SEQ;
            this.txtCNT_NUM.EditValue = SEQ;


            // 테마일괄적용...
            ThemeApply.Themeapply(this);



            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;
            btnBack.Click += delegate (object sender, RoutedEventArgs e)
            {
                NavigationService.Navigate(new SplyMngListView());
            };
            //btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));


            //탭항목 동적추가
            waitindicator.DeferedVisibility = true;
            thread = new Thread(new ThreadStart(LoadFx));
            thread.Start();

        }



        /// <summary>
        /// 엑셀다운로드 쓰레드 Function
        /// </summary>
        private void LoadFx()
        {
            try
            {
                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        waitindicator.DeferedVisibility = true;
                    })));


                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        //탭항목 동적추가
                        MakeTab(_SEQ);

                        waitindicator.DeferedVisibility = false;
                    })));


            }
            catch (Exception )
            {
                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        waitindicator.DeferedVisibility = false;
                    })));
            }
        }


        //탭항목 동적추가
        private void MakeTab(string cNT_NUM)
        {
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "사진첨부";
            tab01.Content = new PhoFileMngView(cNT_NUM);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "파일첨부";
            tab02.Content = new RefFileMngView(cNT_NUM);
            tabSubMenu.Items.Add(tab02);

        }






        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SplyMngListView());
        }

        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
