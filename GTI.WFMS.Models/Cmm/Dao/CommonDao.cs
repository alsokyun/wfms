
using System.Data;
using System.Collections;

using GTIFramework.Core.Managers;
using System;

namespace GTI.WFMS.Models.Cmm.Dao
{
    public class CommonDao
    {
        /// <summary>
        /// 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public DataTable SelectLIST(Hashtable conditions)
        {
            string sqlId = conditions["sqlId"].ToString();
            return DBManager.QueryForTable(sqlId, conditions);
        }

        /// <summary>
        /// 데이터 조회 - 복수sql
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public Hashtable SelectLISTS(Hashtable conditions)
        {
            Hashtable result = new Hashtable();
            DataTable ret = new DataTable();
            try
            {
                string sqlId = conditions["sqlId"].ToString();
                ret = DBManager.QueryForTable(sqlId, conditions);
                result.Add("dt", ret);
            }
            catch (Exception e) { }
            try
            {
                string sqlId = conditions["sqlId2"].ToString();
                ret = DBManager.QueryForTable(sqlId, conditions);
                result.Add("dt2", ret);
            }
            catch (Exception e) { }
            try
            {
                string sqlId = conditions["sqlId3"].ToString();
                ret = DBManager.QueryForTable(sqlId, conditions);
                result.Add("dt3", ret);
            }
            catch (Exception e) { }

            return result;
        }

        /// <summary>
        /// 데이터 조회 - 클래스
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public ArrayList SelectLIST2(Hashtable conditions)
        {
            string sqlId = conditions["sqlId"].ToString();
            return DBManager.QueryForList(sqlId, conditions);
        }

        /// <summary>
        /// 데이터 조회 - Object
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public object SelectObject(Hashtable conditions)
        {
            string sqlId = conditions["sqlId"].ToString();
            return DBManager.QueryForObject(sqlId, conditions);
        }

        /// <summary>
        /// 데이터 업데이트 단건
        /// </summary>
        /// <param name="conditions"></param>
        public void Update(Hashtable conditions)
        {
            string sqlId = conditions["sqlId"].ToString();
            DBManager.QueryForUpdate(sqlId, conditions);
        }

        /// <summary>
        /// 데이터 업데이트 단건 Object
        /// </summary>
        /// <param name="obj"></param>
        internal void Update2(object obj, string sqlId)
        {
            DBManager.QueryForUpdate(sqlId, obj);
        }
    }
}
