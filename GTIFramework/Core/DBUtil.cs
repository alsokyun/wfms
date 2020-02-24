using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tibero.DbAccess;

namespace GTIFramework.Core
{
    public class DBUtil
    {

        public static void UpdateCLOB(Hashtable param)
        {
            string cmdtxt = "";
            cmdtxt += " UPDATE FAQ SET ";
            cmdtxt += " QUESTION = '" + param["QUESTION"] + "'";
            //cmdtxt += " , REPL = " + param["REPL"];
            cmdtxt += " WHERE SEQ = " + param["SEQ"];

            OleDbConnection conn
                = new OleDbConnection("Provider=tbprov.Tbprov.6;" +
                "Data Source =211.105.28.40,8629,tibero;user id =infofms;password =infofms;" +
                "Connection Pooling = 1;Cache Authentication = False; ");


            try
            {
                conn.Open();

                //어댑터생성
                OleDbDataAdapter oda = new OleDbDataAdapter();

                oda.UpdateCommand = conn.CreateCommand();
                oda.UpdateCommand.CommandText = cmdtxt;

                oda.UpdateCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public static void Update(Hashtable param)
        {
            string cmdtxt = "";
            cmdtxt += " UPDATE FAQ SET ";
            cmdtxt += " QUESTION = :QUESTION ";
            cmdtxt += " ,REPL = :REPL ";
            cmdtxt += " ,TTL = :TTL ";
            //cmdtxt += " , REPL = " + param["REPL"];
            cmdtxt += " WHERE SEQ = :SEQ";


            //커넥션새성 및 오픈
            OleDbConnection conn
                = new OleDbConnection("Provider=tbprov.Tbprov.6;" +
                "Data Source =211.105.28.40,8629,tibero;user id =infofms;password =infofms;" +
                "Connection Pooling = 1;Cache Authentication = False; ");

            conn.Open();


            //어댑터생성
            OleDbDataAdapter oda = new OleDbDataAdapter();

            //커맨드생성

            oda.UpdateCommand = conn.CreateCommand();
            oda.UpdateCommand.CommandTimeout = 10000;

            //트랜잭션 시작
            OleDbTransaction trans = conn.BeginTransaction();

            try
            {
                oda.UpdateCommand.Transaction = trans;
                oda.UpdateCommand.CommandText = cmdtxt;

                //oda.UpdateCommand.Parameters.AddRange(new OleDbParameter[] {
                //    new OleDbParameter("@QUESTION", param["QUESTION"]),
                //    new OleDbParameter("@SEQ", param["SEQ"]),
                //});

                //oda.UpdateCommand.Parameters.Add(new OleDbParameter("QUESTION", OleDbType.LongVarChar, 1000000000, ParameterDirection.Input, true, 1, 10, "QUESTION", DataRowVersion.Default, param["QUESTION"]));
                //oda.UpdateCommand.Parameters.Add(new OleDbParameter("TTL", OleDbType.VarChar, 30, ParameterDirection.Input, true, 1, 10, "TTL", DataRowVersion.Default, param["TTL"]));
                //oda.UpdateCommand.Parameters.Add(new OleDbParameter("SEQ", OleDbType.Numeric, 10, ParameterDirection.Input, true, 1, 10, "SEQ", DataRowVersion.Default, param["SEQ"]));

                //oda.UpdateCommand.Parameters.AddWithValue("QUESTION", param["QUESTION"]);
                //oda.UpdateCommand.Parameters.AddWithValue("SEQ", param["SEQ"]);


                oda.UpdateCommand.Parameters.Add(new OleDbParameter("QUESTION", OleDbType.LongVarChar)).Value = param["QUESTION"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("REPL", OleDbType.LongVarChar)).Value = param["REPL"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("TTL", OleDbType.VarChar)).Value = param["TTL"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("SEQ", OleDbType.Numeric)).Value = Convert.ToInt16(param["SEQ"]);



                oda.UpdateCommand.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        public static void UpdateTbr(Hashtable param)
        {
            string cmdtxt = "";
            cmdtxt += " UPDATE FAQ SET ";
            cmdtxt += " QUESTION = :QUESTION ";
            cmdtxt += " ,REPL = :REPL  ";
            cmdtxt += " ,TTL = :TTL ";
            //cmdtxt += " , REPL = " + param["REPL"];
            cmdtxt += " WHERE SEQ = :SEQ";


            //커넥션새성 및 오픈
            OleDbConnectionTbr conn
                = new OleDbConnectionTbr("Provider=tbprov.Tbprov.6;" +
                "Data Source =211.105.28.40,8629,tibero;user id =infofms;password =infofms;" +
                "Connection Pooling = 1;Cache Authentication = False; ");

            conn.Open();


            //어댑터생성
            OleDbDataAdapter oda = new OleDbDataAdapter();
            //트랜잭션 시작
            OleDbTransaction trans = conn.BeginTransaction();

            try
            {
                OleDbCommandTbr cmd = new OleDbCommandTbr();
                cmd.Connection = conn;
                cmd.Transaction = trans;

                OleDbParameterTbr p = new OleDbParameterTbr("QUESTION", param["QUESTION"]);
                p.Direction = ParameterDirection.Input;
                p.OleDbType = OleDbTypeTbr.LongVarChar;
                cmd.Parameters.Add(p);

                p = new OleDbParameterTbr("REPL", param["REPL"]);
                p.Direction = ParameterDirection.Input;
                p.OleDbType = OleDbTypeTbr.LongVarChar;
                cmd.Parameters.Add(p);

                p = new OleDbParameterTbr("TTL", param["TTL"]);
                p.Direction = ParameterDirection.Input;
                p.OleDbType = OleDbTypeTbr.VarChar;
                cmd.Parameters.Add(p);

                p = new OleDbParameterTbr("SEQ", param["SEQ"]);
                p.Direction = ParameterDirection.Input;
                p.OleDbType = OleDbTypeTbr.Numeric;
                cmd.Parameters.Add(p);


                cmd.ExecuteNonQuery();

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }



        public static void Select(Hashtable param)
        {

            OleDbConnection conn
                = new OleDbConnection("Provider=tbprov.Tbprov.6;" +
                "Data Source =211.105.28.40,8629,tibero;user id =infofms;password =infofms;" +
                "Connection Pooling = 1;Cache Authentication = False; ");
            conn.Open();


            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            string cmdtxt = "";
            cmdtxt += " SELECT * FROM FAQ ";
            cmd.CommandText = cmdtxt;

            //COMMAND 생성,실행(조회)
            OleDbDataAdapter oda = new OleDbDataAdapter(cmd);


            //조회결과
            DataSet dset = new DataSet();
            oda.Fill(dset);
            DataTable table = dset.Tables[0];
            DataRow[] rows = table.Select();
            //Console.WriteLine(rows[0]["c1"].ToString());

            conn.Close();

        }
    }
}
