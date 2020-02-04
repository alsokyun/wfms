using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace GTI.WFMS.Modules.Cnst.View
{
    /// <summary>
    /// SplyDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplyDtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);
        public event BackCmd backEvent;

        public SplyDtlView(string CNT_NUM)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            this.txtCNT_NUM.EditValue = CNT_NUM;


            //강제이벤트 발생
            //BackCmd backCmd = new BackCmd(_backCmd);
            backEvent += new BackCmd(_backCmd);
            //backEvent(null, null);


            //정상적인 버튼클릭 이벤트
            btnBack.Click += _backCmd;
            btnBack.Click += delegate (object sender, RoutedEventArgs e)
            {
                NavigationService.Navigate(new SplyMngListView());
            };
            //btnBack.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));


            //탭항목 동적추가
            tabSubMenu.Items.Clear();

            DXTabItem tab01 = new DXTabItem();
            tab01.Header = "사진첨부";
            tab01.Content = new PhotoFileMngView(CNT_NUM);
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "파일첨부";
            tab02.Content = new RefFileMngView(CNT_NUM);
            tabSubMenu.Items.Add(tab02);
        }

        // 목록으로 뒤로가기
        private void _backCmd(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SplyMngListView());
        }
    }
}
