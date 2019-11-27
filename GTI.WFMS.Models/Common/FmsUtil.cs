using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GTI.WFMS.Models.Common
{
    public class FmsUtil
    {
        /// <summary>
        /// 전역변수 설정
        /// </summary>
        public static int PageSize = 10; //페이지의 row 수



        /// <summary>
        /// IsNull 오브젝트형
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            bool ret = false;
            string str = "";

            try
            {
                str  = obj as string;
                ret = String.IsNullOrWhiteSpace(str);
            }
            catch (Exception e){}


            return ret;
        }

        /// <summary>
        /// Nvl 오브젝트형
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="rep"></param>
        /// <returns></returns>
        public static string Nvl(object obj, string rep)
        {
            string str = "";

            try
            {
                str = obj as string;

                if (String.IsNullOrWhiteSpace(str))
                {
                    str = rep;
                }
            }
            catch (Exception e) { }

            return str;
        }

        /// <summary>
        /// Trim 오브젝트형
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Trim(object obj)
        {
            string str = "";

            try
            {
                str = obj as string;

                str = str.Trim();
            }
            catch (Exception e) { }

            return str;
        }






        /// <summary>
        /// 해당 Popup 클래스의 인스턴스 체크
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsWindowOpen<T>(string name = "") where T : Popup
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }



        /// <summary>
        /// 하위 엘리먼트 순회처리
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)

                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }



    }
}
