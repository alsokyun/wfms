using DevExpress.Xpf.Editors;
using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Log;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GTI.WFMS.GIS.Pop.View
{
    /// <summary>
    /// FtrPopView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FtrPopView: Window
    {
        private string ftrCde = "";
        private string ftrIdn = "";

        // 생성자
        public FtrPopView(object FTR_CDE, object FTR_IDN)
        {
            InitializeComponent();
            ThemeApply.Themeapply(this);


            //화면매핑
            ftrCde = FTR_CDE.ToString();
            ftrIdn = FTR_IDN.ToString();
            InitPage(ftrCde, ftrIdn);
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
                    }
                    break;

                case "SA003": //스탠파이프
                    if (FmsUtil.IsNull(_FTR_CDE) && FmsUtil.IsNull(_FTR_IDN))
                    {
                        this.cctl.Content = null;
                    }
                    else 
                    {
                        UC_STPI_PS uc = new UC_STPI_PS(_FTR_CDE, _FTR_IDN);
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;
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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;

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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
                        uc.btnSel.Visibility = Visibility.Hidden;

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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;

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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;

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
                        this.cctl.Content = uc;
                        uc.btnDel.Visibility = Visibility.Hidden;
                        uc.btnSave.Visibility = Visibility.Hidden;
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


        // 컨텐트 disable 처리
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            switch (ftrCde)
            {
                case "SA001": //상수관로
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_PIPE_LM)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_PIPE_LM)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_PIPE_LM)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA002": //급수관로
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_SPLY_LS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_SPLY_LS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_SPLY_LS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA003": //스탠파이프
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_STPI_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_STPI_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_STPI_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA100": //상수맨홀
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_MANH_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_MANH_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_MANH_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA110": //수원지
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_HEAD_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_HEAD_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_HEAD_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA112": //취수장
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_GAIN_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_GAIN_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_GAIN_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;


                case "SA113": //정수장
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_PURI_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_PURI_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_PURI_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA114": //배수지
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_SERV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_SERV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_SERV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA117": //유량계
                    UC_FLOW_PS uc = this.cctl.Content as UC_FLOW_PS;
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>(uc))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>(uc))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>(uc))
                    {
                        cb.IsEnabled = false;
                    }

                    break;

                case "SA118":
                case "SA119": //급수탑,소화전
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_FIRE_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_FIRE_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_FIRE_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA120": //저수조
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_RSRV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_RSRV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_RSRV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA121": //수압계
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_PRGA_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_PRGA_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_PRGA_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA122": //급수전계량기
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_META_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_META_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_META_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA200":
                case "SA201":
                case "SA202":
                case "SA203":
                case "SA204":
                case "SA205":
                    //UC_VALV_PS uc = new UC_VALV_PS(ftrCde, _FTR_IDN);
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_VALV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_VALV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_VALV_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "SA206": //가압펌프장
                    //UC_PRES_PS uc = new UC_PRES_PS(ftrCde, _FTR_IDN);
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_PRES_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_PRES_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_PRES_PS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;

                case "BZ001": //대블록
                    //UC_BLKL_AS uc = new UC_BLKL_AS(ftrCde, _FTR_IDN);
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_BLKL_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_BLKL_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_BLKL_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;
                case "BZ002": //중블록

                    //UC_BLKM_AS uc = new UC_BLKM_AS(ftrCde, _FTR_IDN);
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_BLKM_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_BLKM_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_BLKM_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;
                case "BZ003": //소블록
                    //UC_BLKS_AS uc = new UC_BLKS_AS(ftrCde, _FTR_IDN);
                    foreach (TextEdit cb in FmsUtil.FindVisualChildren<TextEdit>((UC_BLKS_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>((UC_BLKS_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    foreach (Button cb in FmsUtil.FindVisualChildren<Button>((UC_BLKS_AS)this.cctl.Content))
                    {
                        cb.IsEnabled = false;
                    }
                    break;


                default:
                    
                    break;
            }

        }

        //시설물대장
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopWinView p;
            switch (ftrCde)
            {
                case "SA001": //상수관로
                    p = new PopWinView("Pipe/View/WtlPipeDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA002": //급수관로
                    p = new PopWinView("Acmf/View/SupDutDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA003": //스탠파이프
                    p = new PopWinView("Pipe/View/StndPiDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA100": //상수맨홀
                    p = new PopWinView("Pipe/View/WtsMnhoDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA110": //수원지
                    p = new PopWinView("Fclt/View/WtrSourDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA112": //취수장
                    p = new PopWinView("Fclt/View/WtrSupDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA113": //정수장
                    p = new PopWinView("Fclt/View/FiltPltDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA114": //배수지
                    p = new PopWinView("Fclt/View/IntkStDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA117": //유량계
                    p = new PopWinView("Pipe/View/FlowMtDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA118":
                case "SA119": //급수탑,소화전
                    p = new PopWinView("Pipe/View/FireFacDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA120": //저수조
                    p = new PopWinView("Acmf/View/WtrTrkDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA121": //수압계
                    p = new PopWinView("Pipe/View/WtprMtDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA122": //급수전계량기
                    p = new PopWinView("Acmf/View/HydtMetrDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA200":
                case "SA201":
                case "SA202":
                case "SA203":
                case "SA204":
                case "SA205":
                    p = new PopWinView("Pipe/View/ValvFacDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "SA206": //가압펌프장
                    p = new PopWinView("Fclt/View/PrsPmpDtlView.xaml", ftrCde, ftrIdn);
                    break;

                case "BZ001": //대블록
                    p = new PopWinView("Blk/View/Blk01DtlView.xaml", ftrCde, ftrIdn);
                    break;
                case "BZ002": //중블록
                    p = new PopWinView("Blk/View/Blk02DtlView.xaml", ftrCde, ftrIdn);
                    break;
                case "BZ003": //소블록
                    p = new PopWinView("Blk/View/Blk03DtlView.xaml", ftrCde, ftrIdn);
                    break;


                default:
                    p = new PopWinView("Pipe/View/WtlPipeDtlView.xaml", ftrCde, ftrIdn);
                    break;
            }


            Logs.strFocusMNU_CD = "0001";//선택메뉴임시저장

            this.Close();
            if (p.ShowDialog() is bool)
            {
                //재조회
            }
        }
    }
}
