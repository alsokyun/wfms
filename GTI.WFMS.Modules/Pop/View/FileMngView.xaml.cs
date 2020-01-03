using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// FileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileMngView : Popup
    {




        /// <summary>
        /// 생성자
        /// </summary>
        public FileMngView(string FIL_SEQ)
        {


            InitializeComponent();


            //파일키저장
            //txtFIL_SEQ.Text = FIL_SEQ;


            /*마우스드래그이벤트 위한 처리
             */
            var thumb = new Thumb
            {
                Width = 0,
                Height = 0,
            };
            gridContent.Children.Add(thumb);

            MouseDown += (sender, e) =>
            {
                thumb.RaiseEvent(e);
            };

            thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };


        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        public void OnGiveRecordDragFeedback(object sender, GiveRecordDragFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Link)
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                e.UseDefaultCursors = true;
            }
            e.Handled = true;
        }

    }
}
