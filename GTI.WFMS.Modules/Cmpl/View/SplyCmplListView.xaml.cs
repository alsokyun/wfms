using DevExpress.Xpf.Grid;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// SplyCmplListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplyCmplListView : Page
    {
        public SplyCmplListView()
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);
        }


        //선택된 항목으로 페이지이동
        private void Grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string WSER_SEQ = "";

            GridControl gc = sender as GridControl;

            try
            {
                WSER_SEQ = ((DataRowView)gc.CurrentItem).Row["WSER_SEQ"].ToString();

                SplyCmplDtlView view = new SplyCmplDtlView(WSER_SEQ);
                if (view.ShowDialog() is bool)
                {
                    //재조회
                    btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return; //throw;
            }
            
        }

        // 등록 팝업
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SplyCmplAddView view = new SplyCmplAddView();
            if (view.ShowDialog() is bool)
            {
                //재조회
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
