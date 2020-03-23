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

        /// <summary>
        /// OleDB 업데이트 
        /// - param에 등록된 변수가 SQL 에 등록된 변수와 순서가 맞아야하는 이슈가 있어서, 현재는 하드코딩으로 변수매핑했음... 차후에 자동 매핑으로 변경해야함..
        /// </summary>
        /// <param name="param"></param>
        public static void Update(Hashtable param)
        {
            string cmdtxt = "";
            try
            {
                cmdtxt = param["sql"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            //커넥션새성 및 오픈
            string strIP = GTIFramework.Properties.Settings.Default.strIP;
            string strPort = GTIFramework.Properties.Settings.Default.strPort;
            string strSID = GTIFramework.Properties.Settings.Default.strSID;
            string strID = GTIFramework.Properties.Settings.Default.strID;
            string strPWD = GTIFramework.Properties.Settings.Default.strPWD;

            OleDbConnection conn
                = new OleDbConnection("Provider=tbprov.Tbprov.6;" +
                "Data Source =" + strIP + "," + strPort + "," + strSID + ";user id =" + strID + ";password =" + strPWD + ";" +
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


                oda.UpdateCommand.Parameters.Add(new OleDbParameter("QUESTION", OleDbType.LongVarChar));
                oda.UpdateCommand.Parameters["QUESTION"].Value = param["QUESTION"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("REPL", OleDbType.LongVarChar));
                oda.UpdateCommand.Parameters["REPL"].Value = param["REPL"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("TTL", OleDbType.VarChar));
                oda.UpdateCommand.Parameters["TTL"].Value = param["TTL"];
                oda.UpdateCommand.Parameters.Add(new OleDbParameter("SEQ", OleDbType.Numeric));
                oda.UpdateCommand.Parameters["SEQ"].Value = Convert.ToInt16(param["SEQ"]);


                //foreach (DictionaryEntry d in param)
                //{
                //    switch (d.Value.GetType().Name.ToLower())
                //    {
                //        case "string":
                //            if (d.Key.ToString() == "QUESTION" || d.Key.ToString() == "REPL")
                //            {
                //                oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.LongVarChar)).Value = d.Value;

                //            }
                //            else
                //            {
                //                oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.VarChar)).Value = d.Value;
                //            }
                //            break;
                //        case "decimal":
                //            oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.Numeric)).Value = Convert.ToInt16(d.Value);
                //            break;
                //        //case "int":
                //        //    oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.Numeric));
                //        //    oda.UpdateCommand.Parameters[d.Key.ToString()].Value = Convert.ToInt32(d.Value);
                //        //    break;
                //        //case "double":
                //        //    oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.Numeric)).Value = d.Value;
                //        //    break;
                //        //default:
                //        //    oda.UpdateCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.VarChar)).Value = d.Value;
                //        //    break;
                //    }
                //}


                //foreach (string key in param.Keys)
                //{
                //    switch (param[key].GetType().Name.ToLower())
                //    {
                //        case "string":
                //            if (key == "QUESTION" || key == "REPL")
                //            {
                //                oda.UpdateCommand.Parameters.Add(new OleDbParameter(key, OleDbType.LongVarChar)).Value = param[key];

                //            }
                //            else
                //            {
                //                oda.UpdateCommand.Parameters.Add(new OleDbParameter(key, OleDbType.VarChar)).Value = param[key];
                //            }
                //            break;
                //        case "decimal":
                //            oda.UpdateCommand.Parameters.Add(new OleDbParameter(key, OleDbType.Numeric)).Value = Convert.ToInt16(param[key]);
                //            break;
                //    }
                //}




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



        /// <summary>
        /// OleDbConnection 사용한 Select
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable Select(Hashtable param)
        {
            string strIP = GTIFramework.Properties.Settings.Default.strIP;
            string strPort = GTIFramework.Properties.Settings.Default.strPort;
            string strSID = GTIFramework.Properties.Settings.Default.strSID;
            string strID = GTIFramework.Properties.Settings.Default.strID;
            string strPWD = GTIFramework.Properties.Settings.Default.strPWD;
            OleDbConnection conn
                = new OleDbConnection("Provider=tbprov.Tbprov.6;" +
                "Data Source ="+ strIP + ","+ strPort + ","+ strSID + ";user id ="+ strID + ";password ="+ strPWD + ";" +
                "Connection Pooling = 1;Cache Authentication = False; ");
            conn.Open();


            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;


            string cmdtxt = "";
            try
            {
                cmdtxt = param["sql"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd.CommandText = cmdtxt;

            //COMMAND 생성,실행(조회)
            OleDbDataAdapter oda = new OleDbDataAdapter();

            oda.SelectCommand = cmd;
            foreach (DictionaryEntry d in param)
            {
                oda.SelectCommand.Parameters.Add(new OleDbParameter(d.Key.ToString(), OleDbType.VarChar)).Value = d.Value;
            }



            //조회결과
            DataSet dset = new DataSet();
            oda.Fill(dset);
            DataTable table = dset.Tables[0];
            //DataRow[] rows = table.Select();
            //Console.WriteLine(rows[0]["c1"].ToString());

            conn.Close();

            return table;

        }




        /*
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

        */



    }
}
