
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;
using System;

namespace GTI.WFMS.Models.Adm.Dao
{
    class AdmDao
    {
        DBManager dBManager = new DBManager();
        /// <summary>
        /// 사용자조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable selectUsrList(Hashtable conditions)
        {
            return dBManager.QueryForTable("selectUsrList", conditions);
        }

        public void Delete_SYS_USER_INFO(Hashtable conditions)
        {
            dBManager.QueryForUpdate("Delete_SYS_USER_INFO", conditions);
        }

        public DataTable Select_SYS_USER_INFO_Check(Hashtable conditions)
        {
            return dBManager.QueryForTable("Select_SYS_USER_INFO_Check", conditions);
        }

        public void Insert_SYS_USER_INFO(Hashtable conditions)
        {
            dBManager.QueryForInsert("Insert_SYS_USER_INFO", conditions);
        }

        public void Update_SYS_USER_INFO(Hashtable conditions)
        {
            dBManager.QueryForUpdate("Update_SYS_USER_INFO", conditions);
        }

        public DataTable selectMstCdList(Hashtable conditions)
        {
            return dBManager.QueryForTable("selectMstCdList", conditions);
        }

        public DataTable selectDtlCdList(Hashtable conditions)
        {
            return dBManager.QueryForTable("selectDtlCdList", conditions);
        }

        internal void saveMstCd(Hashtable conditions)
        {
            dBManager.QueryForUpdate("saveMstCd", conditions);
        }

        internal void saveDtlCd(Hashtable conditions)
        {
            dBManager.QueryForUpdate("saveDtlCd", conditions);
        }

        internal void deleteDtlCd(Hashtable conditions)
        {
            dBManager.QueryForUpdate("deleteDtlCd", conditions);
        }
    }
}
