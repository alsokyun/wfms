using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;
using GTIFramework.Common.Log;

namespace GTI.WFMS.Models.Main.Dao
{
    class MainDao
    {
        DBManager dBManager = new DBManager();

        /// <summary>
        /// DataBase 연결 테스트
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_SYSDATE(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_SYSDATE", conditions);
        }

        /// <summary>
        /// DataBase 연결 테스트 Oracle
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_SYSDATE2(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_SYSDATE2", conditions);
        }

        internal DataTable Select_MNU_LIST(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_MNU_LIST", conditions);
        }

        internal DataTable SelectBaseSiteInfo(Hashtable conditions)
        {
            return dBManager.QueryForTable("SelectBaseSiteInfo", conditions);
        }

        internal DataTable SelectDBInfo(Hashtable conditions)
        {
            return dBManager.QueryForTable("SelectDBInfo", conditions);
        }

        internal DataTable LoginCheck(Hashtable htconditions)
        {
            return dBManager.QueryForTable("LoginCheck", htconditions);
        }

        internal DataTable Select_LoginUser_Permission(Hashtable htconditions)
        {
            return dBManager.QueryForTable("Select_LoginUser_Permission", htconditions);
        }
    }
}
