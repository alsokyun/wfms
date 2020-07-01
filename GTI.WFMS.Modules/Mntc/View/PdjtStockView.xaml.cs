using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTI.WFMS.Modules.Main;
using GTI.WFMS.Modules.Mntc.ViewModel;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// PdjtStockView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtStockView : Window
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public PdjtStockView(string _PDH_NUM)
        {
            InitializeComponent();

            txtPDH_NUM.Text = _PDH_NUM; //모델로 파라미터전달..

            // 테마일괄적용...
            ThemeApply.Themeapply(this);

        }



        


        /// <summary>
        /// 헤더 All 체크
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllChk_Checked(object sender, RoutedEventArgs e)
        {
            //CheckEdit ce = sender as CheckEdit;
            //bool chk = ce.IsChecked is bool;
            foreach (PdjtInDtl dr in (ObservableCollection<PdjtInDtl>)grid.ItemsSource)
            {
                dr.CHK = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (PdjtInDtl dr in (ObservableCollection<PdjtInDtl>)grid.ItemsSource)
            {
                dr.CHK = "N";
            }
        }




















        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }
 

        private void PdjtStockView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    this.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    this.WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }
    }
}
