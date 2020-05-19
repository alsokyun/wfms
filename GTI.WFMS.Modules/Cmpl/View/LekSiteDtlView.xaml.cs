using DevExpress.Xpf.Core;
using DevExpress.Xpf.LayoutControl;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Modules.Link.View;
using GTI.WFMS.Modules.Pop.View;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GTI.WFMS.Modules.Cmpl.View
{
    /// <summary>
    /// LekSiteDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LekSiteDtlView : Window
    {
        private string _FTR_CDE;
        private string _FTR_IDN;
        //첨부사진관련
        private string BIZ_ID;
        private string FIL_SEQ;


        /// <summary>
        /// 생성자
        /// </summary>
        public LekSiteDtlView(string FTR_CDE, string FTR_IDN)
        {
            InitializeComponent();

            // 테마일괄적용...
            ThemeApply.Themeapply(this);


            //뷰모델로 키전달하기기위 뷰에서 UpdateTrigger 발생시킴
            if (FTR_CDE != null)
            {
                _FTR_CDE = FTR_CDE;
                _FTR_IDN = FTR_IDN;
                this.txtFTR_CDE.Text = FTR_CDE;
                this.txtFTR_IDN.Text = FTR_IDN;
            }

            //사진관리 Content 생성
            //this.cctrlPhoto.Content = new ImgFileMngView(FTR_CDE + FTR_IDN);

            this.BIZ_ID = FTR_CDE + FTR_IDN;
            this.FIL_SEQ = null;

            InitModel();

        }

        //첨부사진조회
        private void InitModel()
        {
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectBizIdFileDtl");

            param.Add("BIZ_ID", this.BIZ_ID);
            DataTable dt = BizUtil.SelectList(param);

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

        //민원선택 팝업호출
        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 지형지물팝업 윈도우
                CnstCmplSelView cnstCmplSelView = new CnstCmplSelView();
                cnstCmplSelView.Owner = Window.GetWindow(this);


                //FTR_IDN 리턴
                if (cnstCmplSelView.ShowDialog() is bool)
                {
                    string RCV_NUM = cnstCmplSelView.txbRCV_NUM.Text;


                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(RCV_NUM))
                    {
                        txtRCV_NUM.EditValue = RCV_NUM;
                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }

        }

        //상수관로 시설물선택
        private void BtnFtrSel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 지형지물팝업 윈도우
                FtrSelView ftrSelView = new FtrSelView(null);
                ftrSelView.Owner = Window.GetWindow(this);


                //FTR_IDN 리턴
                if (ftrSelView.ShowDialog() is bool)
                {
                    string FTR_IDN = ftrSelView.txtFTR_IDN.Text;
                    string FTR_CDE = ftrSelView.txtFTR_CDE.Text;
                    string FTR_NAM = ftrSelView.txtFTR_NAM.Text;


                    //저장버튼으로 닫힘
                    if (!FmsUtil.IsNull(FTR_IDN))
                    {
                        txtPIP_IDN.EditValue = FTR_IDN;
                        txtPIP_CDE.Text = FTR_CDE;
                        txtPIP_NAM.EditValue = FTR_NAM;

                    }
                    //닫기버튼으로 닫힘
                }

            }
            catch (Exception ex)
            {
                Messages.ShowErrMsgBox(ex.ToString());
            }
        }



        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //팝업호출지점으로 리턴
            DialogResult = true;
            Close();

        }

       
        private void LekSiteDtlView_KeyDown(object sender, KeyEventArgs e)
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






        //첨부사진 추가
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






    /// <summary>
    /// 이미지 갤러리 표현을 위한 별도의 클래스
    /// </summary>
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

            ////이미지 미리보기
            //string file_name = "";
            //try
            //{
            //    file_name = obj as string;
            //}
            //catch (Exception) { }

            //if (FmsUtil.IsNull(file_name))
            //{
            //    Messages.ShowInfoMsgBox("이미지파일이 없습니다.");
            //    return;
            //}

            //string file_path = System.IO.Path.Combine(@"" + FmsUtil.fileDir, file_name);

            //try
            //{
            //    BitmapImage bi = new BitmapImage();
            //    bi.BeginInit();
            //    bi.UriSource = new Uri(file_path);
            //    bi.EndInit();


            //    imgView.Source = bi;
            //    grdImg.Visibility = Visibility.Visible;
            //    bdImg.Visibility = Visibility.Visible;
            //}
            //catch (Exception)
            //{
            //    Messages.ShowInfoMsgBox("이미지 정보가 없습니다.");
            //}

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
