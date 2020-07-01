using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Input;

namespace GTI.WFMS.GIS.Pop.View
{
    /// <summary>
    /// FtrPopView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FtrPopView: Window
    {
        private string v; //화면컨트롤명

        // 생성자
        public FtrPopView(object FTR_CDE, object FTR_IDN)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);



            InitPage(FTR_CDE.ToString(), FTR_IDN.ToString());
        }













        /// <summary>
        /// UserControl 시설물페이지 로딩
        /// </summary>
        /// <param name="CBO_FTR_CDE"></param>
        /// <param name="_FTR_CDE"></param>
        /// <param name="_FTR_IDN"></param>
        private void InitPage(string _FTR_CDE, string _FTR_IDN)
        {

            switch (_FTR_CDE)
            {
                case "SA001": //상수관로
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_PIPE_LM uc = new UC_PIPE_LM(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception){}
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA002": //급수관로
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_SPLY_LS uc = new UC_SPLY_LS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA003": //스탠파이프
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else if (FmsUtil.IsNull(_FTR_IDN))
                    {
                        UC_STPI_PS uc = new UC_STPI_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA100": //상수맨홀
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_MANH_PS uc = new UC_MANH_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA110": //수원지
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_HEAD_PS uc = new UC_HEAD_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA112": //취수장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_GAIN_PS uc = new UC_GAIN_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;


                case "SA113": //정수장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_PURI_AS uc = new UC_PURI_AS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA114": //배수지
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_SERV_PS uc = new UC_SERV_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;



                case "SA117": //유량계
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_FLOW_PS uc = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA118":
                case "SA119": //급수탑,소화전
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_FIRE_PS uc = new UC_FIRE_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA120": //저수조
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_RSRV_PS uc = new UC_RSRV_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA121": //수압계
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_PRGA_PS uc = new UC_PRGA_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA122": //급수전계량기
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_META_PS uc = new UC_META_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA200":
                case "SA201":
                case "SA202":
                case "SA203":
                case "SA204":
                case "SA205":
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_VALV_PS uc = new UC_VALV_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "SA206": //가압펌프장
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_PRES_PS uc = new UC_PRES_PS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;

                case "BZ001": //대블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_BLKL_AS uc = new UC_BLKL_AS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;
                case "BZ002": //중블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_BLKM_AS uc = new UC_BLKM_AS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;
                case "BZ003": //소블록
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else
                    {
                        UC_BLKS_AS uc = new UC_BLKS_AS(_FTR_CDE, _FTR_IDN);
                        try
                        {
                            uc.btnDel.Visibility = Visibility.Collapsed;
                            uc.btnSave.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception) { }
                        this.cctl.Content = uc;
                    }
                    break;


                default:
                    this.cctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);
                    break;
            }

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

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

        }

    }
}
