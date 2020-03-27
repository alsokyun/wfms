using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace GTI.WFMS.Models.Common
{
    public static class FmsUtil
    {
        /// <summary>
        /// 전역변수 설정
        /// </summary>
        public static int PageSize = 10; //페이지의 row 수

        public static IRegionManager __regionManager;
        public static Popup __popMain;//열린팝업을 전역변수로 저장해놓음,사용안함
        public static Window popWinView;//팝업업무기본창 전역변수로 저장해놓음 - 사이즈변경위해

        /// <summary>
        /// 프로그램설정값으로부터 전역변수로 설정
        /// </summary>
        public static string sysCd;     //시스템코드
        public static string fileDir;   //파일저장경로


        /// <summary>
        /// IsNull 오브젝트형
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull(object obj)
        {
            bool ret = false;
            string str = "";

            if(obj == null)
            {
                return true;
            }

            try
            {
                str  = obj.ToString();
                ret = String.IsNullOrWhiteSpace(str);
            }
            catch (Exception ){
                ret = true;
            }


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
            catch (Exception ) { }

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
            catch (Exception ) { }

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



        /// <summary>
        /// 모델복사 Create a Generic Extension
        /// </summary>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("This type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
                return default(T);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// ContentControl 에 걸려있는 EventName 이벤트를 모두 제거
        /// </summary>
        /// <param name="b"></param>
        /// <param name="EventName"></param>
        public static void RemoveEvent(Control b, string EventName)
        {
            FieldInfo f1 = typeof(Control).GetField(EventName, BindingFlags.Static | BindingFlags.NonPublic);
            if (f1 != null)
            {
                object obj = f1.GetValue(b);

                PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);

                list.RemoveHandler(obj, list[obj]);
            }
        }


    }
}
