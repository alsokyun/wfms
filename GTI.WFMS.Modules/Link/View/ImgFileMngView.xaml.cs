using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.Modules.Link.View
{
    /// <summary>
    /// ImgFileMngView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImgFileMngView : UserControl
    {
        private string BIZ_ID;
        private string FIL_SEQ;


        //초기조회
        private DataTable dt = new DataTable();

        // 뷰생성자
        public ImgFileMngView(string _BIZ_ID)
        {
            InitializeComponent();

            this.BIZ_ID = _BIZ_ID;
            this.FIL_SEQ = null;

            InitModel();

        }

        // 뷰생성자
        public ImgFileMngView()
        {
            InitializeComponent();

            //this.BIZ_ID = _BIZ_ID;
            this.FIL_SEQ = null;

            InitModel();

        }

        private void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectBizIdFileDtl");

            param.Add("BIZ_ID", this.BIZ_ID);
            dt = BizUtil.SelectList(param);
            
            string UriPrefix = @"" + FmsUtil.fileDir;        
            var result = new List<BitmapImage>();

            this.FIL_SEQ = null;

            if (dt.Rows.Count > 0)
            { 
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    string ImgPathName = row["UPF_NAM"].ToString();

                    this.FIL_SEQ = row["FIL_SEQ"].ToString();

                    FileInfo fi = new FileInfo(UriPrefix + "\\" + ImgPathName);
                    //FileInfo.Exists로 파일 존재유무 확인 "
                    if (fi.Exists)
                    {
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
                }
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


        //첨부파일 모듈호출
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 파일첨부윈도우
                FilePopView fileMngView = new FilePopView(this.FIL_SEQ);
                fileMngView.Owner = Window.GetWindow(this);
                
                //FIL_SEQ 리턴
                if (fileMngView.ShowDialog() is bool)
                {
                    string pFIL_SEQ = fileMngView.txtFIL_SEQ.Text;
                    string sToDay = DateTime.Now.ToString("yyyyMMdd");

                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(pFIL_SEQ))
                    {
                        if (FmsUtil.IsNull(this.FIL_SEQ))
                        {
                            Hashtable param = new Hashtable();

                            param.Add("sqlId", "SaveFileMap");
                            param.Add("BIZ_ID", this.BIZ_ID);
                            param.Add("FIL_SEQ", Convert.ToInt32(pFIL_SEQ));
                            param.Add("GRP_TYP", "111");
                            param.Add("TIT_NAM", "사진첨부");
                            param.Add("UPD_YMD", sToDay);
                            param.Add("UPD_USR", Logs.strLogin_ID);
                            param.Add("CTNT", "");

                            param.Add("CRE_YMD", sToDay);
                            param.Add("CRE_USR", Logs.strLogin_ID);

                            //저장처리
                            try
                            {
                                BizUtil.Update(param);

                            }
                            catch (Exception ex)
                            {
                                Messages.ShowErrMsgBox("저장 처리중 오류가 발생하였습니다." + ex.ToString());
                                return;
                            }
                        }

                        InitModel();
                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }


            //gv.AddNewRow();
            //int newRowHandle = DataControlBase.NewItemRowHandle;
            //grid.SetCellValue(gv.FocusedRowHandle, "PAY_YMD", Convert.ToDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        }

    }

    //public class ImageContainer : ContentControlBase
    //{
    //    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseLeftButtonUp(e);
    //        if (Controller.IsMouseLeftButtonDown)
    //        {
    //            var layoutControl = Parent as FlowLayoutControl;
    //            if (layoutControl != null)
    //            {
    //                Controller.IsMouseEntered = false;
    //                layoutControl.MaximizedElement = layoutControl.MaximizedElement == this ? null : this;
    //            }
    //        }

    //    }
    //    protected override void OnSizeChanged(SizeChangedEventArgs e)
    //    {
    //        base.OnSizeChanged(e);
    //        if (!double.IsNaN(Width) && !double.IsNaN(Height))
    //            if (e.NewSize.Width != e.PreviousSize.Width)
    //                Height = double.NaN;
    //            else
    //                Width = double.NaN;
    //    }
    //}
}
