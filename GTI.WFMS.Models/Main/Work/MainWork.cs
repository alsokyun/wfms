using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTI.WFMS.Models.Main.Dao;

namespace GTI.WFMS.Models.Main.Work
{
    public class MainWork
    {
        MainDao dao = new MainDao();

        /// <summary>
        /// DataBase 연결 테스트
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_SYSDATE(Hashtable conditions)
        {
            return dao.Select_SYSDATE(conditions);
        }

        /// <summary>
        /// DataBase 연결 테스트 Oracle
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_SYSDATE2(Hashtable conditions)
        {
            return dao.Select_SYSDATE2(conditions);
        }

        public DataTable Select_MNU_LIST(Hashtable conditions)
        {
            return dao.Select_MNU_LIST(conditions);
        }
    }
}
