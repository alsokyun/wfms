using System.Collections;
using System.Data;
using GTI.WFMS.Models.Pipe.Dao;

namespace GTI.WFMS.Models.Pipe.Work
{
    public class PipeWork
    {
        PipeDao dao = new PipeDao();

        /// <summary>
        /// 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectWtlPipeList(Hashtable conditions)
        {
            return dao.SelectWtlPipeList(conditions);
        }

        /// <summary>
        /// 변류시설조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectValvFacList(Hashtable conditions)
        {
            return dao.SelectValvFacList(conditions);
        }

        /// <summary>
        /// 소방시설조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectFireFacList(Hashtable conditions)
        {
            return dao.SelectFireFacList(conditions);
        }

    }
}
