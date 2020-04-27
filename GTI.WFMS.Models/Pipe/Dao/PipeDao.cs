
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

        /// <summary>
        /// 변류시설조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectValvFacList(Hashtable conditions)
        {
            return DBManager.QueryForTable("SelectValvFacList", conditions);
        }

        /// <summary>
        /// 소방시설조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectFireFacList(Hashtable conditions)
        {
            return DBManager.QueryForTable("SelectFireFacList", conditions);
        }
    }
}
