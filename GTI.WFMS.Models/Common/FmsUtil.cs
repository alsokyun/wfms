using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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













    }
}
