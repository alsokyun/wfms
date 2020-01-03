using GTI.WFMS.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI.WFMS.Modules.Pop.ViewModel
{
    public class FileMngViewModel 
    {
        // 파일관련 전역변수 - 환경설정으로 관리해야함
        string dir_name = @"D:\GTI\FILE";









        // 생성자
        public FileMngViewModel()
        {
            Items = new ObservableCollection<FileInfo>();



            //파일저장버튼 이벤트
            SaveCmd = new RelayCommand<object>(delegate (object obj)
            {
                UploadFileList(null);
            });

            //파일찾기버튼 이벤트
            FindFileCmd = new RelayCommand<object>(delegate (object obj)
            {
                
            });


            // 초기조회
            DataTable dt = new DataTable();

            Hashtable param = new Hashtable();
            param.Add("sqlId", "SelectFileDtl");
            param.Add("FIL_SEQ", null);

            dt = BizUtil.SelectList(param);

            Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string file_name2 = row["UPF_NAM"].ToString();
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);

                FileInfo fi = new FileInfo(file_path2);
                Items.Add(fi);
            }


        }




        #region =========  프로퍼티 ===========
        public virtual ObservableCollection<FileInfo> Items { get; set; }
        public RelayCommand<object> SaveCmd { get; set; } //뷰에서 ICommand 사용하여 뷰객체 전달받음
        public RelayCommand<object> FindFileCmd { get; set; }

        #endregion





        #region ========== 드래그앤드롭 처리함수 ==============

        public void DragRecordOver(bool dragEnter)
        {
            //Method must exist.
            Console.WriteLine("DragRecordOver");
        }


        public void DropRecord(List<FileInfo> filesData)
        {
            foreach (FileInfo fi in filesData)
                if (!Items.Any(x => x.FullName == fi.FullName))
                    Items.Add(fi);
        }

        public void StartRecordDrag(bool allowDrag)
        {
            //Method is optional
            Console.WriteLine("StartRecordDrag");
        }
        public void GiveRecordDragFeedback(bool dragMove)
        {
            //Method must exist.
            Console.WriteLine("GiveRecordDragFeedback");
        }

        public void CompleteRecordDragDrop(string filePath)
        {
            Console.WriteLine("CompleteRecordDragDrop");
            //File.WriteAllBytes(filePath, fileByte);
        }

        #endregion














        /// <summary>
        /// 물리적파일을 업로드한후, 파일테이블에 등록하고, FILE_SEQ를 생성한다
        /// </summary>
        /// <returns></returns>
        private int UploadFileList(string FILE_SEQ)
        {
            int file_seq = -1;
            List<Hashtable> FILE_DTL_LIST = new List<Hashtable>();//첨부파일상세 리스트
            Hashtable FILE_DTL = new Hashtable();//첨부파일상세



            // 0.첨부파일 수정모드인경우, 해당파일 모두삭제
            try
            {
                file_seq = Convert.ToInt16(FILE_SEQ);
            }
            catch (Exception) { }


            Hashtable param = new Hashtable();
            if (file_seq > 0)
            {
                //파일마스터 관련파일 모두 일단 삭제
                param.Add("sqlId", "DeleteFileSeq");
                BizUtil.Update(param);

                //실제파일삭제 구현(어려울시 파일남겨둘수도...)
            }



            // 1.물리적파일 저장
            if (!System.IO.Directory.Exists(dir_name))
            {
                System.IO.Directory.CreateDirectory(dir_name);
            }
            int seq = 0;
            foreach (FileInfo fi in Items)
            {
                string file_name = fi.Name + fi.Extension;
                string file_path = System.IO.Path.Combine(dir_name, file_name);
                string file_name2 = DateTime.Today.ToString("yyyyMMddhhmiss") + fi.Extension; //저장되는파일이름
                string file_path2 = System.IO.Path.Combine(dir_name, file_name2);


                fi.CopyTo(file_path2);


                //파일상세정보 추가
                FILE_DTL.Clear();
                FILE_DTL.Add("SEQ", seq++);
                FILE_DTL.Add("FIL_RST", "");//사진파일해상도
                FILE_DTL.Add("FIL_SIZ", fi.Length / 1000);
                FILE_DTL.Add("FIL_TYP", fi.Extension);
                FILE_DTL.Add("UPF_NAM", file_path2);
                FILE_DTL.Add("DWN_NAM", file_path);
                FILE_DTL.Add("CUR_TFS", "1");//?
                
                FILE_DTL_LIST.Add(FILE_DTL);


            }


            // 2.첨부파일 등록
            file_seq = SaveFileList(file_seq, FILE_DTL_LIST);





            return file_seq;
        }





        /// <summary>
        /// 첨부파일 등록
        /// </summary>
        /// <param name="file_seq"></param>
        /// <param name="fILE_DTL_LIST"></param>
        private int SaveFileList(int file_seq, List<Hashtable> FILE_DTL_LIST)
        {
            Hashtable param = new Hashtable();

            //1.마스터신규인 경우 
            if (file_seq < 0)
            {
                //마스터등록(파일마스터 채번)
                param.Clear();
                param.Add("sqlId", "InsertFileMst");
                file_seq = BizUtil.UpdateR(param);
            }



            //2.디테일등록
            foreach (Hashtable dtl in FILE_DTL_LIST)
            {
                dtl["FIL_SEQ"] = file_seq;
                dtl.Add("sqlId", "InsertFileDtl");
                BizUtil.Update(param);
            }


            return file_seq;
        }



    }
}
