using Esri.ArcGISRuntime.Mapping;
using GTI.WFMS.GIS.Module.View;
using GTI.WFMS.Models.Common;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GTI.WFMS.GIS.Module
{
    /// <summary>
    /// FTR_POP.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FTR_POP : Popup
    {
        public FTR_POP(string FTR_CDE, string FTR_IDN)
        {


            InitializeComponent();



            //시설물에 해당하는 페이지 생성
            MakePage(FTR_CDE, FTR_IDN);







            //마우스드래그이벤트 위한 처리
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


        //해상시설물 페이지
        private void MakePage(string _FTR_CDE, string _FTR_IDN)
        {
            switch (_FTR_CDE)
            {
                case "SA001": //상수관로
                    {
                        ctl.Content = new UC_PIPE_LM(_FTR_CDE, _FTR_IDN);//상세페이지
                    }
                    break;

                case "SA002": //급수관로
                        ctl.Content = new UC_SPLY_LS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA003": //스탠파이프
                        ctl.Content = new UC_STPI_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA100": //상수맨홀
                        ctl.Content = new UC_MANH_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA110": //수원지
                        ctl.Content = new UC_HEAD_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA112": //취수장
                        ctl.Content = new UC_GAIN_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;


                case "SA113": //정수장
                        ctl.Content = new UC_PURI_AS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA114": //배수지
                        ctl.Content = new UC_SERV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA117": //유량계
                        ctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA118":
                case "SA119": //급수탑,소화전
                        ctl.Content = new UC_FIRE_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA120": //저수조
                        ctl.Content = new UC_RSRV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA121": //수압계
                        ctl.Content = new UC_PRGA_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA122": //급수전계량기
                        ctl.Content = new UC_META_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA200":
                case "SA201":
                case "SA202":
                case "SA203":
                case "SA204":
                case "SA205":
                        ctl.Content = new UC_VALV_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;

                case "SA206": //가압펌프장
                        ctl.Content = new UC_PRES_PS(_FTR_CDE, _FTR_IDN);//상세페이지
                    break;


                default:
                    ctl.Content = new UC_FLOW_PS(_FTR_CDE, _FTR_IDN);
                    break;
            }
        }

        //닫기
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var v in CmmRun.layers)
                {
                    v.Value.ClearSelection();
                }
                ((MapMainViewModel)this.DataContext)._selectedFeature = null;
            }
            catch (Exception){}


            this.IsOpen = false;
        }
    }
}