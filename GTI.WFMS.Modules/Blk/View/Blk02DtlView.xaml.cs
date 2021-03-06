﻿using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GTI.WFMS.Modules.Blk.View
{
    /// <summary>
    /// Blk02DtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Blk02DtlView : Page
    {
        public delegate void BackCmd(object sender, RoutedEventArgs e);

        Thread thread;
        private string _FTR_CDE;
        private int _FTR_IDN;

        public Blk02DtlView(string FTR_CDE, int FTR_IDN)
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

        public Blk02DtlView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

            this.txtFTR_CDE.EditValue = BizUtil.FTR_CDE;
            this.txtFTR_IDN.EditValue = Convert.ToInt32(BizUtil.FTR_IDN);

            _FTR_CDE = BizUtil.FTR_CDE;
            _FTR_IDN = Convert.ToInt32(BizUtil.FTR_IDN);

            //전역변수 리셋
            BizUtil.FTR_CDE = "";
            BizUtil.FTR_IDN = "";

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
            tab01.Header = "파일첨부";
            tab01.Content = new RefFileMngView(FTR_CDE + FTR_IDN.ToString());
            tabSubMenu.Items.Add(tab01);

            DXTabItem tab02 = new DXTabItem();
            tab02.Header = "사진첨부";
            tab02.Content = new PhotoFileMngView(FTR_CDE + FTR_IDN.ToString());
            tabSubMenu.Items.Add(tab02);

        }

        /// <summary>
        /// 쓰레드 Function
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
            NavigationService.Navigate(new Blk02ListView());
        }

    }
}
