﻿using GTIFramework.Common.Utils.ViewEffect;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// SplyCmplDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplyCmplDtlView : Window
    {
        private string _WSER_SEQ;


        /// <summary>
        /// 생성자
        /// </summary>
        public SplyCmplDtlView(string WSER_SEQ)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            //뷰모델로 키전달하기기위 뷰에서 UpdateTrigger 발생시킴
            if (WSER_SEQ != null)
            {
                _WSER_SEQ = WSER_SEQ;
                this.txtWSER_SEQ.Text = WSER_SEQ;
            }
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void SplyCmplDtlView_KeyDown(object sender, KeyEventArgs e)
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
