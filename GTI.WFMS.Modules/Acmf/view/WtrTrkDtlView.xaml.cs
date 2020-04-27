using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Acmf.View
{
    /// <summary>
    /// WtrTrkDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WtrTrkDtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);

        Thread thread;
        private string _FTR_CDE;
        private int _FTR_IDN;

        public WtrTrkDtlView(string FTR_CDE, int FTR_IDN)
        {
            _FTR_CDE = FTR_CDE;
            _FTR_IDN = FTR_IDN;

            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            this.txtFTR_CDE.EditValue = FTR_CDE;
            this.txtFTR_IDN.EditValue = FTR_IDN;

            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;

            //탭항목 동적추가
            waitindicator.DeferedVisibility = true;
            thread = new Thread(new ThreadStart(LoadFx));
            thread.Start();
        }

        private void MakeChild(string FTR_CDE, int FTR_IDN)
        {
            //탭항목 동적추가
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "유지보수";
            tab01.Content = new ChscResSubListView(FTR_CDE, FTR_IDN);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "사진첨부";
            tab02.Content = new PhotoFileMngView(FTR_CDE + FTR_IDN.ToString());
            tabSubMenu.Items.Add(tab02);

            DXTabItem tab03 = new DXTabItem();
            tab03.Header = "파일첨부";
            tab03.Content = new RefFileMngView(FTR_CDE + FTR_IDN.ToString());
            tabSubMenu.Items.Add(tab03);

            DXTabItem tab04 = new DXTabItem();
            tab04.Header = "청소이력";
            tabSubMenu.Items.Add(tab04);

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
                       //탭항목 동적추가
                       MakeChild(_FTR_CDE, _FTR_IDN);

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

        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WtrTrkListView());
        }

    }
}
