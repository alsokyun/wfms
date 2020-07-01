using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Mntc.Model;
using GTIFramework.Common.Utils.ViewEffect;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// PdjtMngListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdjtMngListView: UserControl
    {

        // 생성자
        public PdjtMngListView( )
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);



        }





        #region ============ 이벤트핸들러 ============ 


        //그리드내 콤보구성
        private void PDT_CAT_CDE_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit obj = sender as ComboBoxEdit;
            //구분
            obj.ItemsSource = BizUtil.GetCmbCode("250106");
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
            foreach (PdjtMaDtl dr in (ObservableCollection<PdjtMaDtl>)grid.ItemsSource)
            {
                dr.CHK = "Y";
            }
        }
        private void AllChk_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (PdjtMaDtl dr in (ObservableCollection<PdjtMaDtl>)grid.ItemsSource)
            {
                dr.CHK = "N";
            }
        }








        #endregion


    }
}
