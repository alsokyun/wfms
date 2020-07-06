using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Blk.Model;
using GTI.WFMS.Models.Cmm.Dao;
using GTI.WFMS.Models.Cmm.Model;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;

namespace GTI.WFMS.Models.Common
{
    public class BizUtil
    {
        //프로젝트간 시설물 키 전달을 위한 전역파라미터
        public static string FTR_CDE = "";
        public static string FTR_IDN = "";



        private static CommonDao dao = new CommonDao();
        private static CmmDao cmmDao = new CmmDao();


        #region =========== 조회함수 =========== 

        /// <summary>
        /// 데이터 조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static DataTable SelectList(Hashtable conditions)
        {
            return dao.SelectLIST(conditions);
        }

        /// <summary>
        /// 데이터 조회 - 복수SQL
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static Hashtable SelectLists(Hashtable conditions)
        {
            return dao.SelectLISTS(conditions); ;
        }

        public delegate void dele_callback(DataTable dt);
        /// <summary>
        /// 페이징 그리드조회
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public static void SelectListPage(Hashtable conditions, int pageIndex, dele_callback callback)
        {
            conditions.Add("page", pageIndex);
            conditions.Add("rows", FmsUtil.PageSize);
            //return dao.SelectLIST(conditions);
            callback(dao.SelectLIST(conditions));
        }


        /// <summary>
        /// 데이터 조회 - Object
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static IList<T> SelectListObj<T>(Hashtable conditions)
        {
            return dao.SelectLISTObj<T>(conditions);
        }

        /// <summary>
        /// 데이터 조회 - 클래스
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static object SelectObject(Hashtable conditions)
        {
            return dao.SelectObject(conditions);
        }


        
        #endregion



        #region =========== 저장함수 =========== 



        /// <summary>
        /// 데이터 업데이트 - 단건
        /// </summary>
        /// <param name="conditions"></param>
        public static void Update(Hashtable conditions)
        {
            try
            {
                conditions.Add("ID", Logs.strLogin_ID);
            }
            catch (Exception) { }

            dao.Update(conditions);
        }
        /// <summary>
        /// 데이터 업데이트 - 리턴
        /// </summary>
        /// <param name="conditions"></param>
        public static int InsertR(Hashtable conditions)
        {
            try
            {
                conditions.Add("ID", Logs.strLogin_ID);
            }
            catch (Exception) { }

            return (int)dao.InsertR(conditions);
        }

        /// <summary>
        /// 데이터 업데이트 - Object
        /// </summary>
        /// <param name="obj"></param>
        public static void Update2(CmmDtl obj, string sqlId)
        {
            obj.ID = Logs.strLogin_ID;
            dao.Update2(obj, sqlId);
        }

        /// <summary>
        /// 데이터 인서트 - Object
        /// </summary>
        /// <param name="obj"></param>
        public static void Insert2(CmmDtl obj, string sqlId)
        {
            obj.ID = Logs.strLogin_ID;
            dao.Insert2(obj, sqlId);
        }



        #endregion




        #region ========== Validation ===========

        /// <summary>
        /// 상세화면 필수체크
        /// </summary>
        /// <param name="obj"></param>
        public static bool ValidReq(DependencyObject obj)
        {
            //필수값체크
            foreach (TextEdit te in FmsUtil.FindVisualChildren<TextEdit>(obj))
            {
                if (!FmsUtil.IsNull(te.Tag))
                {
                    if (FmsUtil.IsNull(te.EditValue))
                    {
                        Messages.ShowInfoMsgBox(string.Format("{0}은 필수입력 항목입니다.", te.Tag.ToString()));
                        return false;
                    }
                }
            }
            foreach (ComboBoxEdit cb in FmsUtil.FindVisualChildren<ComboBoxEdit>(obj))
            {
                if (!FmsUtil.IsNull(cb.Tag))
                {
                    if (FmsUtil.IsNull(cb.EditValue))
                    {
                        Messages.ShowInfoMsgBox(string.Format("{0}은 필수입력 항목입니다.", cb.Tag.ToString()));
                        return false;
                    }
                }
            }
            return true;
        }



        /// <summary>
        /// 날짜 선후관계체크
        /// </summary>
        /// <param name="obj"></param>
        public static bool ValidDateBtw(object from, object to)
        {
            try
            {
                if (Convert.ToInt32(from) == 0 || Convert.ToInt32(to) == 0)     return true;

                if (Convert.ToInt32(from) > Convert.ToInt32(to))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }
        #endregion




        #region =========== 코드데이터 공통함수 =========== 

        /// <summary>
        /// 공통코드 콤보 데이터바인딩
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="MST_CD"></param>
        /// <param name="ALL 전체항목추가"></param>
        public static void SetCmbCode(ComboBoxEdit cmb, string MST_CD, string ALL)
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable(MST_CD);


            conditions.Add("MST_CD", MST_CD);
            dt = cmmDao.Select_CODE_LIST(conditions);

            /* 전체추가 */
            if (!FmsUtil.IsNull(ALL))
            {
                DataRow dr = dt.NewRow();
                dr["DTL_CD"] = "";
                dr["NM"] = ALL;
                dt.Rows.InsertAt(dr, 0);

                cmb.NullText = ALL;//널값은 NullText로 나옴
            }

            // combo Cd/Nm 필드매핑
            cmb.DisplayMember = "NM";
            cmb.ValueMember = "DTL_CD";
            cmb.ItemsSource = dt;
        }
        public static void SetCmbCode(ComboBoxEdit cmb, string MST_CD)
        {
            SetCmbCode(cmb, MST_CD, null);
        }



        /// <summary>
        /// 공통코드명
        /// </summary>
        /// <param name="MST_CD"></param>
        /// <param name="DTL_CD "></param>
        public static string GetCdNm(string MST_CD, string DTL_CD )
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable(MST_CD);

            
            conditions.Add("sqlId", "Select_CODE_NAME");
            conditions.Add("MST_CD", MST_CD);
            conditions.Add("DTL_CD", DTL_CD);
            dt = dao.SelectLIST(conditions);

            string nm = "";
            try
            {
                nm = dt.Rows[0]["NM"].ToString();
            }
            catch (Exception){}
            return nm;
        }


        /// <summary>
        /// 공통코드 콤보 데이터
        /// </summary>
        /// <param name="MST_CD"></param>
        /// <param name="NV 널항목추가"></param>
        public static DataTable GetCmbCode(string MST_CD, bool NV )
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable(MST_CD);


            conditions.Add("MST_CD", MST_CD);
            dt = cmmDao.Select_CODE_LIST(conditions);

            /* 전체추가 */
            if (NV)
            {
                DataRow dr = dt.NewRow();
                dr["DTL_CD"] = " ";
                dr["NM"] = "";
                dt.Rows.InsertAt(dr, 0);
            }

            return dt;
        }
        public static DataTable GetCmbCode(string MST_CD)
        {
            return GetCmbCode(MST_CD, false);
        }





        /// <summary>
        /// 일반콤보 데이터바이딩
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="sqlId"></param>
        /// <param name="ValueMember"></param>
        /// <param name="DisplayMember"></param>
        /// <param name="ALL 전체항목추가"></param>
        public static void SetCombo(ComboBoxEdit cmb, string sqlId, string ValueMember, string DisplayMember, string ALL, Func<DataRow, bool> filter)
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable();


            conditions.Add("sqlId", sqlId);
            DataTable _dt = new DataTable();
            _dt = dao.SelectLIST(conditions);

            //필터식적용
            //foreach (DataRow row in _dt.AsEnumerable().Where(filter))
            //{
            //    dt.ImportRow(row);
            //}
            dt = _dt.AsEnumerable().Where(filter).CopyToDataTable();


            /* 전체추가 */
            if (!FmsUtil.IsNull(ALL))
            {
                DataRow dr = dt.NewRow();
                dr[ValueMember] = "";
                dr[DisplayMember] = ALL;
                dt.Rows.InsertAt(dr, 0);

                cmb.NullText = ALL;//널값은 NullText로 나옴
            }

            // combo객체 Cd/Nm 필드매핑
            cmb.DisplayMember = DisplayMember;
            cmb.ValueMember = ValueMember;
            cmb.ItemsSource = dt;

            try
            {
                cmb.SelectedIndex = 0;
            }
            catch (Exception){}
        }
        //필터없는형태
        public static void SetCombo(ComboBoxEdit cmb, string sqlId, string ValueMember, string DisplayMember, string ALL)
        {
            SetCombo(cmb, sqlId, ValueMember, DisplayMember, ALL, Row => true);
        }
        //전체여부없는형태
        public static void SetCombo(ComboBoxEdit cmb, string sqlId, string ValueMember, string DisplayMember)
        {
            SetCombo(cmb, sqlId, ValueMember, DisplayMember, null);
        }



        /// <summary>
        /// 일반코드명
        /// </summary>
        /// <param name="sqlId"></param>
        /// <param name="DTL_CD "></param>
        public static string GetCodeNm(string sqlId, string CODE)
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable();


            conditions.Add("sqlId", sqlId);
            conditions.Add("CODE", CODE);
            dt = dao.SelectLIST(conditions);

            string nm = "";
            try
            {
                nm = dt.Rows[0]["NAME"].ToString();
            }
            catch (Exception) { }
            return nm;
        }


        /// <summary>
        /// 공통코드 콤보 데이터
        /// </summary>
        /// <param name="MST_CD"></param>
        /// <param name="NV 널항목추가"></param>
        public static DataTable GetCombo(Hashtable param, bool ALL)
        {
            string ValueMember = param["ValueMember"].ToString();
            string DisplayMember = param["DisplayMember"].ToString();

            DataTable dt = new DataTable();
            dt = dao.SelectLIST(param);

            /* 전체추가 */
            if (ALL)
            {
                DataRow dr = dt.NewRow();
                dr[ValueMember] = " ";
                dr[DisplayMember] = "전체";
                dt.Rows.InsertAt(dr, 0);
            }

            return dt;
        }
        public static DataTable GetCombo(Hashtable param)
        {
            return GetCombo(param, false);
        }

        #endregion







        #region 첨부파일관련

        // 파일마스터키 FIL_SEQ에 해당하는 물리파일 삭제, 첨부테이블삭제
        public static void DelFileSeq(object filSeq)
        {
            //1.물리파일삭제
            //0.첨부 디테일파일정보가져오기
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileDtl2");
            param.Add("FIL_SEQ", filSeq);

            List<FileDtl> lst = new List<FileDtl>(BizUtil.SelectListObj<FileDtl>(param));
            foreach (FileDtl dtl in lst)
            {
                string del_file_path = System.IO.Path.Combine(FmsUtil.fileDir, dtl.UPF_NAM);
                try
                {
                    FileInfo fi = new FileInfo(del_file_path);
                    fi.Delete();
                }
                catch (Exception) { }
            }


            //2.첨부테이블삭제
            //a.디테일
            param = new Hashtable();
            param.Add("sqlId", "DeleteFileSeq");
            param.Add("FIL_SEQ", filSeq);
            BizUtil.Update(param);
            //b.마스터
            param = new Hashtable();
            param.Add("sqlId", "DeleteFileMst");
            param.Add("FIL_SEQ", filSeq);
            BizUtil.Update(param);
        }


        #endregion







        #region ============ 시스템파일 환경 ==============


        /// <summary>
        /// Gets the data folder where locally provisioned data is stored.
        /// </summary>
        public static string GetDataFolder()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string sampleDataFolder = Path.Combine(appDataFolder, "WFMSData");

            if (!Directory.Exists(sampleDataFolder)) { Directory.CreateDirectory(sampleDataFolder); }

            return sampleDataFolder;
        }

        /// <summary>
        /// Gets the path to an item on disk. 
        /// The item must have already been downloaded for the path to be valid.
        /// </summary>
        /// <param name="itemId">ID of the portal item.</param>
        internal static string GetDataFolder(string itemId)
        {
            return Path.Combine(GetDataFolder(), itemId);
        }

        /// <summary>
        /// Gets the path to an item on disk. 
        /// The item must have already been downloaded for the path to be valid.
        /// </summary>
        /// <param name="itemId">ID of the portal item.</param>
        /// <param name="pathParts">Components of the path.</param>
        public static string GetDataFolder(string itemId, params string[] pathParts)
        {
            return Path.Combine(GetDataFolder(itemId), Path.Combine(pathParts));
        }



        //블럭관리번호 콤보생성
        public static void SetFTR_IDN(string uPPER_FTR_CDE, ComboBoxEdit cmb)
        {
            if (FmsUtil.IsNull(uPPER_FTR_CDE)) return;//상위코드없으면 콤보채우지 않는다

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectUpBlk");
            param.Add("FTR_CDE", uPPER_FTR_CDE);
            List<BlkDtl> lst = (List<BlkDtl>)BizUtil.SelectListObj<BlkDtl>(param);

            cmb.DisplayMember = "BLK_NM";
            cmb.ValueMember = "FTR_IDN";
            cmb.ItemsSource = lst;
        }






        #endregion

    }
}