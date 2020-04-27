using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;

using GTIFramework.Common.Log;
using GTIFramework.Common.Utils.ViewEffect;

namespace GTIFramework.Common.MessageBox
{
    public class Messages
    {
        public static DevExpress.Mvvm.UI.NotificationService AppNotificationService;
        private static CustomNotificationViewModel customnoti = new CustomNotificationViewModel();

        public static string MAPPER_DEFINE_ERROR = "정의되지 않은 코드입니다.";
        private static string strOkMsg = "정상적으로 처리되었습니다.";
        private static string strErrMsg = "오류가 발생했습니다. \n담당자에게 문의바랍니다.";

        /// <summary>
        /// 정상처리
        /// </summary>
        public static void ShowOkMsgBox()
        {
            DXMessageBox.Show(Messages.strOkMsg, "확인", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// YesNo 확인 처리
        /// </summary>
        /// <param name="srt"></param>
        /// <returns></returns>
        public static MessageBoxResult ShowYesNoMsgBox(string srt)
        {
            return DXMessageBox.Show(srt, "확인", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }


        /// <summary>
        /// 에러처리 NoLoging
        /// </summary>
        public static void ShowErrMsgBox()
        {
            DXMessageBox.Show(Messages.strErrMsg, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 에러처리 NoLoging, 전달Message 입력
        /// </summary>
        public static void ShowErrMsgBox(String str)
        {
            DXMessageBox.Show(str, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 정보 전달Message 입력
        /// </summary>
        public static void ShowInfoMsgBox(String str)
        {
            DXMessageBox.Show(str, "확인", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 에러처리 Loging
        /// </summary>
        /// <param name="e"></param>
        public static void ShowErrMsgBoxLog(Exception e)
        {
            try
            {
                Logs.ErrLogging(e);

                if (e.Message.Contains("파일은 다른 프로세스에서 사용 중이므로 프로세스에서 액세스할 수 없습니다."))
                {
                    DXMessageBox.Show("해당 파일이 사용중 입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (e.Message.Contains("TNS") || e.Message.Contains("oracle provider"))
                {
                    DXMessageBox.Show("DB서버와 연결이 끊어졌습니다. \n네트워크를 확인 후 프로그램을 다시 실행해 주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //Environment.Exit(0);
                }
                else
                {
                    DXMessageBox.Show(Messages.strErrMsg, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 에러처리 Loging, 전달Message 입력
        /// </summary>
        /// <param name="e"></param>
        /// <param name="strErrContent"></param>
        public static void ShowErrMsgBoxLog(Exception e, string str)
        {
            Logs.ErrLogging(e);
            DXMessageBox.Show(str, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// 에러처리 Loging
        /// </summary>
        /// <param name="e"></param>
        /// <param name="strErrContent"></param>
        public static void ErrLog(Exception e)
        {
            Logs.ErrLogging(e);
        }

        /// <summary>
        /// 에러처리 string Loging
        /// </summary>
        /// <param name="e"></param>
        public static void ErrLog(string strMessage)
        {
            Logs.ErrLogging(strMessage);
        }

        public static void NotificationBox(string strTitle, string strLine1, string strLine2)
        {
            DevExpress.Mvvm.INotification notification;

            if (ThemeApply.strThemeName.Equals("GTINavyTheme"))
                customnoti.ImageSur = "/Resources/Navy/Images/Image/Tilte_notification.png";
            else
                customnoti.ImageSur = "/Resources/Blue/Images/Image/Tilte_notification.png";

            customnoti.Caption = strTitle;
            customnoti.Content1 = strLine1;
            customnoti.Content2 = strLine2;
            notification = AppNotificationService.CreateCustomNotification(customnoti);

            notification.ShowAsync();
        }
    }

    public class CustomNotificationViewModel
    {
        public virtual string ImageSur { get; set; }
        public virtual string Caption { get; set; }
        public virtual string Content1 { get; set; }
        public virtual string Content2 { get; set; }
    }

}
