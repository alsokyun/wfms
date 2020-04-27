using DevExpress.Mvvm;
using GTI.WFMS.Modules.Main.View;
using GTIFramework.Common.MessageBox;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Main.ViewModel
{
    public class PopWinViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public DelegateCommand<object> LoadedCommand { get; set; }
        public DelegateCommand<object> WindowMoveCommand { get; set; }

        PopWinView popWinView;



        /// 생성자
        public PopWinViewModel()
        {
            // 초기이벤트
            this.LoadedCommand = new DelegateCommand<object>(delegate (object obj){

                if (obj == null) return;

                //그리드뷰인스턴스
                popWinView = obj as PopWinView;
            });

            // 윈도우 마우스드래그
            WindowMoveCommand = new DelegateCommand<object>(delegate (object objt)
            {
                try
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        if (popWinView.WindowState == WindowState.Maximized)
                        {
                            popWinView.Top = Mouse.GetPosition(popWinView).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                            popWinView.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(popWinView).X + 20;

                            popWinView.WindowState = WindowState.Normal;
                        }
                        popWinView.DragMove();
                    }
                }
                catch (Exception ex)
                {
                    Messages.ShowErrMsgBoxLog(ex);
                }
            });


        }
    }
}
