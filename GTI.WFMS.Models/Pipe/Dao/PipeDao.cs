
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;

namespace GTI.WFMS.Models.Pipe.Dao
{
    class PipeDao
    {
        /// <summary>
        /// 상수관로조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectWtlPipeList(Hashtable conditions)
        {
            return DBManager.QueryForTable("SelectWtlPipeList", conditions);
        }

    }
}
