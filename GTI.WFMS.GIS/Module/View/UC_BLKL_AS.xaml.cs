using GTI.WFMS.Models.Blk.Model;
using GTI.WFMS.Models.Common;
using GTI.WFMS.Models.Fclt.Model;
using System.Collections;
using System.Windows.Controls;

namespace GTI.WFMS.GIS.Module.View
{
    /// <summary>
    /// WTL_PIPE_LM.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UC_BLKL_AS : UserControl
    {
        //생성자
        public UC_BLKL_AS()
        {
            InitializeComponent();
        }

        //시설물 상세 생성자
        public UC_BLKL_AS(string _FTR_CDE, string _FTR_IDN) : this()
        {
            txtFTR_CDE.Text = _FTR_CDE;
            txtFTR_IDN.Text = _FTR_IDN;
        }

        //신규 시설물 생성자
        public UC_BLKL_AS(string _FTR_CDE) : this()
        {
            txtFTR_CDE.Text = _FTR_CDE;
            
            //신규관리번호채번
            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectBlk01FTR_IDN");
            BlkDtl result = BizUtil.SelectObject(param) as BlkDtl;
            
            //채번결과 매칭
            txtFTR_IDN.Text = result.FTR_IDN.ToString();
            btnDel.Tag = "Y";//신규채번 플래그
        }




    }
}