using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// PhotoFileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PhotoFileMngView : UserControl
    {
        private string BIZ_ID;

        //초기조회
        private DataTable dt = new DataTable();

        // 뷰생성자
        public PhotoFileMngView(string _BIZ_ID)
        {
            InitializeComponent();

            this.BIZ_ID = _BIZ_ID;
            
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectBizIdFileDtl");

            param.Add("BIZ_ID", this.BIZ_ID);

            dt = BizUtil.SelectList(param);
            //FlowLayoutControl.ItemsSource = dt;

            string UriPrefix = @"D:\GTI\FILE";            
            var result = new List<BitmapImage>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string ImgPathName = row["DWN_NAM"].ToString();

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                //bi.CacheOption = BitmapCacheOption.OnDemand;
                //bi.CreateOptions = BitmapCreateOptions.DelayCreation;
                //bi.DecodePixelHeight = 125;       //원본이미지 수정
                //bi.DecodePixelWidth  = 125;       //원본이미지 수정됨
                //bi.Rotation = Rotation.Rotate90;  //회전
                bi.UriSource = new Uri(UriPrefix + "\\" + ImgPathName);
                bi.EndInit();

                result.Add(bi); 
            }

            layoutImages.ItemsSource = result;
        }
        
        void layoutImagesItemsSizeChanged(object sender, ValueChangedEventArgs<Size> e)
        {
            Size size = layoutImages.MaximizedElementOriginalSize;
            if (!double.IsInfinity(e.NewValue.Width))
                size.Height = double.NaN;
            else
                size.Width = double.NaN;
            layoutImages.MaximizedElementOriginalSize = size;
        }

    }

    public class ImageContainer : ContentControlBase
    {
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (Controller.IsMouseLeftButtonDown)
            {
                var layoutControl = Parent as FlowLayoutControl;
                if (layoutControl != null)
                {
                    Controller.IsMouseEntered = false;
                    layoutControl.MaximizedElement = layoutControl.MaximizedElement == this ? null : this;
                }
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
