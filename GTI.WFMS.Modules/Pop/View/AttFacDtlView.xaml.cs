using DevExpress.Xpf.Core;
using GTI.WFMS.Models.Main.Work;
using GTIFramework.Common.ConfigClass;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using GTIFramework.Common.Utils.ViewEffect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GTI.WFMS.Modules.Pop.View
{
    /// <summary>
    /// AttFacDtlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AttFacDtlView: Window
    {
        MainWork work = new MainWork();

        private DataTable dtDBInfo;


        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="login"></param>

        //사업소 DB연결이 없으면
        public AttFacDtlView( string _FTR_CDE, string _FTR_IDN, string _ATTA_SEQ)
        {
            // 0.생성자 기본처리
            InitializeComponent();
            ThemeApply.Themeapply(this);


            // 2.로딩이벤트 처리
            Loaded += initModel;

            btnSave.Click += BtnSave_Click;
            btnXSignClose.Click += BtnClose_Click;
            bdTitle.PreviewMouseDown += BdTitle_PreviewMouseDown;

        }






        #region ========== 이벤트핸들러 =============

        /// <summary>
        /// 로딩시 초기화작업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void initModel(object sender, RoutedEventArgs e)
        {
            //DB구분 콤보박스 초기화
            DataTable dtDBCAT = new DataTable();

            DataColumn dcDTL_CD = new DataColumn("DTL_CD", typeof(string));
            DataColumn dcNM = new DataColumn("NM", typeof(string));
            dtDBCAT.Columns.Add(dcDTL_CD);
            dtDBCAT.Columns.Add(dcNM);

            //티베로 DB 추가
            DataRow drTibero = dtDBCAT.NewRow();
            drTibero["DTL_CD"] = "000007";
            drTibero["NM"] = "Tibero";
            dtDBCAT.Rows.Add(drTibero);

            //오라클 DB 추가
            DataRow drOracle = dtDBCAT.NewRow();
            drOracle["DTL_CD"] = "000002";
            drOracle["NM"] = "Oracle";
            dtDBCAT.Rows.Add(drOracle);


        }


        /// <summary>
        /// 접속정보저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
          
        }


        /// <summary>
        /// 마우스 드래그
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BdTitle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Top = Mouse.GetPosition(this).Y - System.Windows.Forms.Cursor.Position.Y - 6;
                    this.Left = System.Windows.Forms.Cursor.Position.X - Mouse.GetPosition(this).X + 20;

                    this.WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }

        // 닫기버튼
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        #endregion





    }
}
