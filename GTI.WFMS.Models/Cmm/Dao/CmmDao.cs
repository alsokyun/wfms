
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;

namespace GTI.WFMS.Models.Cmm.Dao
{
    class CmmDao
    {
        /// <summary>
        /// 코드 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_CODE_LIST(Hashtable conditions)
        {
            return DBManager.QueryForTable("Select_CODE_LIST", conditions);
        }

    }
}
