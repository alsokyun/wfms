using System.Collections;
using System.Data;
using GTI.WFMS.Models.Cmm.Dao;

namespace GTI.WFMS.Models.Cmm.Work
{
    public class CmmWork
    {
        CmmDao dao = new CmmDao();

        /// <summary>
        /// 코드 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_CODE_LIST(Hashtable conditions)
        {
            return dao.Select_CODE_LIST(conditions);
        }

        
        public DataTable Select_ADAR_LIST(Hashtable conditions)
        {
            return dao.Select_ADAR_LIST(conditions);
        }

        //지형지물 List
        public DataTable Select_FTR_LIST(Hashtable conditions)
        {
            return dao.Select_FTR_LIST(conditions);
        }

    }
}
