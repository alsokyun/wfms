using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTI.WFMS.Models.Adm.Dao;

namespace GTI.WFMS.Models.Adm.Work
{
    public class AdmWork
    {
        AdmDao dao = new AdmDao();

        /// <summary>
        /// 사용자리스트
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable selectUsrList(Hashtable conditions)
        {
            DataTable dt = new DataTable();
            dt = dao.selectUsrList(conditions);

            return dt;
        }

        /// <summary>
        /// 사용자삭제
        /// </summary>
        /// <param name="conditions"></param>
        public void Delete_SYS_USER_INFO(Hashtable conditions)
        {
            dao.Delete_SYS_USER_INFO(conditions);
        }

        /// <summary>
        /// 사용자존재 확인
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable Select_SYS_USER_INFO_Check(Hashtable conditions)
        {
            return dao.Select_SYS_USER_INFO_Check(conditions);
        }

        /// <summary>
        /// 사용자추가
        /// </summary>
        /// <param name="conditions"></param>
        public void Insert_SYS_USER_INFO(Hashtable conditions)
        {
            dao.Insert_SYS_USER_INFO(conditions);
        }

        /// <summary>
        /// 사용자 수정
        /// </summary>
        /// <param name="conditions"></param>
        public void Update_SYS_USER_INFO(Hashtable conditions)
        {
            dao.Update_SYS_USER_INFO(conditions);
        }

        /// <summary>
        /// 상위코드조회
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public DataTable selectMstCdList(Hashtable conditions)
        {
            DataTable dt = new DataTable();
            dt = dao.selectMstCdList(conditions);

            return dt;
        }
        /// <summary>
        /// 하위코드조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable selectDtlCdList(Hashtable conditions)
        {
            DataTable dt = new DataTable();
            dt = dao.selectDtlCdList(conditions);

            return dt;
        }

        /// <summary>
        /// 상위코드저장
        /// </summary>
        /// <param name="conditions"></param>
        public void saveMstCd(Hashtable conditions)
        {
            dao.saveMstCd(conditions);
        }

        /// <summary>
        /// 상세코드저장
        /// </summary>
        /// <param name="conditions"></param>
        public void saveDtlCd(Hashtable conditions)
        {
            dao.saveDtlCd(conditions);
        }
        /// <summary>
        /// 상세코드 삭제
        /// </summary>
        /// <param name="conditions"></param>
        public void deleteDtlCd(Hashtable conditions)
        {
            dao.deleteDtlCd(conditions);
        }
    }
}
