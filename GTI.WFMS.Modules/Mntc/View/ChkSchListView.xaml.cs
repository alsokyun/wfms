using DevExpress.Xpf.Scheduling;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Mntc.View
{
    /// <summary>
    /// ChkSchListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChkSchListView: Window
    {

        // 생성자
        public ChkSchListView( )
        {
            InitializeComponent();
            //ThemeApply.Themeapply(this);

        }



        // 스케줄 선택시 이벤트처리
        private void schedulerControl_DependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                SchedulerControl scheduler = sender as SchedulerControl;
                if (e.Property == SchedulerControl.SelectedIntervalProperty)
                {
                    Console.WriteLine(string.Format("SelectedInterval is changed. New value is {0} \r\n", e.NewValue));
                }
                if (e.Property == SchedulerControl.SelectedResourceProperty)
                {
                    Console.WriteLine(string.Format("SelectedResource is changed. New value is {0} \r\n", ((ResourceItem)e.NewValue).Caption));
                }
            }
            catch (Exception){}
        }




        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void SchedulerDateNavigatorStyleSettings_CustomizeSpecialDates(object sender, CustomizeSpecialDatesEventArgs e)
        {

        }
    }
}
