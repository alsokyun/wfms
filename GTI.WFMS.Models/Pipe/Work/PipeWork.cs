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

    }
}
