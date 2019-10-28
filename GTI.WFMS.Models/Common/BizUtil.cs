using DevExpress.Xpf.Editors;
using GTI.WFMS.Models.Cmm.Dao;
using System.Collections;
using System.Data;

namespace GTI.WFMS.Models.Common
{
    public class BizUtil
    {
        static CommonDao dao = new CommonDao();
        static CmmDao cmmDao = new CmmDao();


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
        /// 데이터 업데이트 - 단건
        /// </summary>
        /// <param name="conditions"></param>
        public static void Update(Hashtable conditions)
        {
            dao.Update(conditions);
        }


        /// <summary>
        /// 공통코드 콤보 데이터바인딩
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="MST_CD"></param>
        /// <param name="ALL 전체항목추가"></param>
        public static void SetCmbCode(ComboBoxEdit cmb, string MST_CD, bool ALL)
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable(MST_CD);


            conditions.Add("MST_CD", MST_CD);
            dt = cmmDao.Select_CODE_LIST(conditions);

            /* 전체추가 */
            if (ALL)
            {
                DataRow dr = dt.NewRow();
                dr["DTL_CD"] = " ";
                dr["NM"] = "전체";
                dt.Rows.InsertAt(dr, 0);
            }

            // combo code/value 필드매핑
            cmb.DisplayMember = "NM";
            cmb.ValueMember = "DTL_CD";

            cmb.ItemsSource = dt;
            cmb.SelectedIndex = 0;
        }
        public static void SetCmbCode(ComboBoxEdit cmb, string MST_CD)
        {
            SetCmbCode(cmb, MST_CD, false);
        }


        /// <summary>
        /// 일반콤보 데이터바이딩
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="sqlId"></param>
        /// <param name="ValueMember"></param>
        /// <param name="DisplayMember"></param>
        /// <param name="ALL 전체항목추가"></param>
        public static void SetCombo(ComboBoxEdit cmb, string sqlId, string ValueMember, string DisplayMember, bool ALL)
        {
            Hashtable conditions = new Hashtable();
            DataTable dt = new DataTable();


            conditions.Add("sqlId", sqlId);
            dt = dao.SelectLIST(conditions);

            /* 전체추가 */
            if (ALL)
            {
                DataRow dr = dt.NewRow();
                dr[ValueMember] = " ";
                dr[DisplayMember] = "전체";
                dt.Rows.InsertAt(dr, 0);
            }

            // combo객체 Code/Name 필드매핑
            cmb.DisplayMember = DisplayMember;
            cmb.ValueMember = ValueMember;

            cmb.ItemsSource = dt;
            cmb.SelectedIndex = 0;
        }
        public static void SetCombo(ComboBoxEdit cmb, string sqlId, string ValueMember, string DisplayMember)
        {
            SetCombo(cmb, sqlId, ValueMember, DisplayMember, false);
        }


        }
    }
