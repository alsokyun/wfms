
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;
using System;

namespace GTI.WFMS.Models.Cmm.Dao
{
    class CmmDao
    {
        DBManager dBManager = new DBManager();

        /// <summary>
        /// 코드 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_CODE_LIST(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_CODE_LIST", conditions);
        }

        internal DataTable Select_ADAR_LIST(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_ADAR_LIST", conditions);
        }

        //지형지물 List
        internal DataTable Select_FTR_LIST(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_FTR_LIST", conditions);
        }
    }
}
