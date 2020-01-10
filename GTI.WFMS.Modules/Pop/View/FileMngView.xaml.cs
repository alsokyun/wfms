using DevExpress.Xpf.Core;
using GTI.WFMS.Modules.Main;
using GTIFramework.Common.Utils.ViewEffect;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// FileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileMngView : Window
    {

        public string R_FILE_SEQ { get; set; }


        /// <summary>
        /// 생성자
        /// </summary>
        public FileMngView(string FIL_SEQ)
        {


            InitializeComponent();


            // 2.테마일괄적용...
            //ThemeApply.Themeapply(this);


            //파일키저장
            txtFIL_SEQ.Text = FIL_SEQ;
            txtFIL_SEQ.Text = "-1";

        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();


            //this.IsOpen = false;
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

        // 그리드 파일 행삭제
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gv.DeleteRow(gv.FocusedRowHandle);
        }
    }
}
