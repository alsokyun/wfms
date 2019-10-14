
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;
using System;

namespace GTI.WFMS.Models.Adm.Dao
{
    class AdmDao
    {
        /// <summary>
        /// 사용자조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable selectUsrList(Hashtable conditions)
        {
            return DBManager.QueryForTable("selectUsrList", conditions);
        }

        public void Delete_SYS_USER_INFO(Hashtable conditions)
        {
            DBManager.QueryForUpdate("Delete_SYS_USER_INFO", conditions);
        }

        public DataTable Select_SYS_USER_INFO_Check(Hashtable conditions)
        {
            return DBManager.QueryForTable("Select_SYS_USER_INFO_Check", conditions);
        }

        public void Insert_SYS_USER_INFO(Hashtable conditions)
        {
            DBManager.QueryForInsert("Insert_SYS_USER_INFO", conditions);
        }

        public void Update_SYS_USER_INFO(Hashtable conditions)
        {
            DBManager.QueryForUpdate("Update_SYS_USER_INFO", conditions);
        }

        public DataTable selectMstCdList(Hashtable conditions)
        {
            return DBManager.QueryForTable("selectMstCdList", conditions);
        }

        public DataTable selectDtlCdList(Hashtable conditions)
        {
            return DBManager.QueryForTable("selectDtlCdList", conditions);
        }

        internal void saveMstCd(Hashtable conditions)
        {
            DBManager.QueryForUpdate("saveMstCd", conditions);
        }

        internal void saveDtlCd(Hashtable conditions)
        {
            DBManager.QueryForUpdate("saveDtlCd", conditions);
        }

        internal void deleteDtlCd(Hashtable conditions)
        {
            DBManager.QueryForUpdate("deleteDtlCd", conditions);
        }
    }
}
