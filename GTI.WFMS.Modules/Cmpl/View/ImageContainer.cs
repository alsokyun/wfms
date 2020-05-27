using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using GTI.WFMS.Models.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace GTI.WFMS.Modules.Cmpl.View
{
    public class ImageContainer : ContentControlBase
    {
        public static Image imgView;
        public static Grid grdImg;
        public static Border bdImg;


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            //if (Controller.IsMouseLeftButtonDown)
            //{
            //    var layoutControl = Parent as FlowLayoutControl;
            //    if (layoutControl != null)
            //    {
            //        Controller.IsMouseEntered = false;
            //        layoutControl.MaximizedElement = layoutControl.MaximizedElement == this ? null : this;
            //    }
            //}

            string file_name = "";
            try
            {
                file_name = ((Image)e.OriginalSource).DataContext.ToString();
            }
            catch (Exception) {}

            ////이미지 미리보기
            if (FmsUtil.IsNull(file_name))
            {
                MessageBox.Show("이미지파일이 없습니다.");
                return;
            }

            string file_path = System.IO.Path.Combine(@"" + FmsUtil.fileDir, file_name);

            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                //bi.UriSource = new Uri(file_path);
                bi.UriSource = new Uri(file_name);
                bi.EndInit();


                imgView.Source = bi;
                grdImg.Visibility = Visibility.Visible;
                bdImg.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                MessageBox.Show("이미지 정보가 없습니다.");
            }

        }



        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            if (!double.IsNaN(Width) && !double.IsNaN(Height))
                if (e.NewSize.Width != e.PreviousSize.Width)
                    Height = double.NaN;
                else
                    Width = double.NaN;
        }
    }
}
