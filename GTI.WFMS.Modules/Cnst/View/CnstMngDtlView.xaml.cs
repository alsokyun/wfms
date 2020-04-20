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

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// CnstMngDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CnstMngDtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        //public event BackCmd backEvent;

        Thread thread;
        private string _CNT_NUM;


        public CnstMngDtlView(string CNT_NUM)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            this.txtCNT_NUM.EditValue = CNT_NUM;
            _CNT_NUM = CNT_NUM;

            //강제이벤트 발생
            //BackCmd backCmd = new BackCmd(_backCmd);
            //backEvent += new BackCmd(_backCmd);
            //backEvent(null, null);


            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;




            /* 탭항목 동적추가 */
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
                        //탭항목 동적추가
                        MakeTab(_CNT_NUM);

                        waitindicator.DeferedVisibility = false;
                    })));


            }
            catch (Exception)
            {
                this.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,
                    new Action((delegate ()
                    {
                        waitindicator.DeferedVisibility = false;
                    })));
            }
        }



        // 탭항목 동적추가
        public void MakeTab(string CNT_NUM)
        {
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "공사비지급내역";
            tab01.Content = new WttCostDtView(CNT_NUM);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "설계변경내역";
            tab02.Content = new WttChngDtView(CNT_NUM);
            tabSubMenu.Items.Add(tab02);

            DXTabItem tab03 = new DXTabItem();
            tab03.Header = "하도급내역";
            tab03.Content = new WttSubcDtView(CNT_NUM);
            tabSubMenu.Items.Add(tab03);

            DXTabItem tab04 = new DXTabItem();
            tab04.Header = "하자보수내역";
            tab04.Content = new WttFlawDtView(CNT_NUM);
            tabSubMenu.Items.Add(tab04);

            DXTabItem tab05 = new DXTabItem();
            tab05.Header = "사진첨부";
            tab05.Content = new PhotoFileMngView(CNT_NUM);
            tabSubMenu.Items.Add(tab05);

            DXTabItem tab06 = new DXTabItem();
            tab06.Header = "참조자료";
            tab06.Content = new RefFileMngView(CNT_NUM);
            tabSubMenu.Items.Add(tab06);
        }

        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CnstMngListView());
        }

        //화면재로딩
        public void refresh()
        {
            //NavigationService.Refresh();
            NavigationService.Navigate(new CnstMngDtlView(_CNT_NUM));
        }

    }
}
