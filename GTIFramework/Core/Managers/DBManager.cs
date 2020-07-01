using GTIFramework.Common.ConfigClass;
using GTIFramework.Common.Log;
using GTIFramework.Common.MessageBox;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using log4net;
using log4net.Config;
using Oracle.DataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Resources;
using Tibero.DbAccess;

namespace GTIFramework.Core.Managers
{
    public class DBManager
    {
        //맵퍼를 Key값으로 관리하기 위해 HashTable maps 생성
        private Hashtable maps = new Hashtable();

        //Log4net 설정 부분
        private ILog log = LogManager.GetLogger("dbLogger");

        object objresult;

        //Connstr (커넥션스트링) 리스트 저장
        private DomSqlMapBuilder builder = new DomSqlMapBuilder();
        private ResourceManager manager = Properties.Resources.ResourceManager;
        private Hashtable htstrconn = new Hashtable();

        public DBManager()
        {

        }

        #region 공통
        //해당 Mapper 생성
        private void GenerateMapper(string datasourceCode)
        {
            //초기 한번 실행
            if (htstrconn.Count == 0)
            {
                ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);

                foreach (DictionaryEntry resource in resourceSet)
                {
                    if (resource.Value.ToString().Contains("DBConfig/"))
                    {
                        builder = null;
                        builder = new DomSqlMapBuilder();

                        if (manager.GetString(datasourceCode) == null)
                        {
                            throw new Exception(Messages.MAPPER_DEFINE_ERROR + " [" + datasourceCode + "]");
                        }

                        htstrconn.Add(resource.Key, (builder.Configure(manager.GetString(resource.Key.ToString())) as ISqlMapper).DataSource.ConnectionString);
                    }
                }
            }

            //수정 필요??????????? 확인 20171017
            if (!maps.ContainsKey(datasourceCode + GetProcessID()))
            {
                builder = null;
                builder = new DomSqlMapBuilder();

                if (manager.GetString(datasourceCode) == null)
                {
                    throw new Exception(Messages.MAPPER_DEFINE_ERROR + " [" + datasourceCode + "]");
                }

                ISqlMapper mapper = builder.Configure(manager.GetString(datasourceCode));

                if (datasourceCode.Equals("ORACLEConfig"))
                {
                    if (mapper.IsSessionStarted)
                        mapper.CloseConnection();
                    mapper.DataSource.ConnectionString = mapper.DataSource.ConnectionString.Replace("oralceIP", Logs.useConfig.strIP).Replace("oralcePort", Logs.useConfig.strPort).Replace("oralceService", Logs.useConfig.strSID).Replace("oralceID", Logs.useConfig.strID).Replace("oralcePWD", Logs.useConfig.strPWD);
                }
                else if (datasourceCode.Equals("TIBEROConfig"))
                {
                    if (mapper.IsSessionStarted)
                        mapper.CloseConnection();
                    mapper.DataSource.ConnectionString = mapper.DataSource.ConnectionString.Replace("tiberoIP", Logs.useConfig.strIP).Replace("tiberoPort", Logs.useConfig.strPort).Replace("tiberoService", Logs.useConfig.strSID).Replace("tiberoID", Logs.useConfig.strID).Replace("tiberoPWD", Logs.useConfig.strPWD);
                }

                maps.Add(datasourceCode + GetProcessID(), mapper);
            }
            else
            {
                MapperChange(datasourceCode + GetProcessID(), datasourceCode);
            }
        }

        private void MapperChange(string strmapsNM, string datasourceCode)
        {
            if (datasourceCode.Equals("ORACLEConfig"))
            {
                if ((maps[strmapsNM] as ISqlMapper).IsSessionStarted)
                    (maps[strmapsNM] as ISqlMapper).CloseConnection();
                (maps[strmapsNM] as ISqlMapper).DataSource.ConnectionString = htstrconn[datasourceCode].ToString().Replace("oralceIP", Logs.useConfig.strIP).Replace("oralcePort", Logs.useConfig.strPort).Replace("oralceService", Logs.useConfig.strSID).Replace("oralceID", Logs.useConfig.strID).Replace("oralcePWD", Logs.useConfig.strPWD);
            }
            else if (datasourceCode.Equals("TIBEROConfig"))
            {
                if ((maps[strmapsNM] as ISqlMapper).IsSessionStarted)
                    (maps[strmapsNM] as ISqlMapper).CloseConnection();
                (maps[strmapsNM] as ISqlMapper).DataSource.ConnectionString = htstrconn[datasourceCode].ToString().Replace("tiberoIP", Logs.useConfig.strIP).Replace("tiberoPort", Logs.useConfig.strPort).Replace("tiberoService", Logs.useConfig.strSID).Replace("tiberoID", Logs.useConfig.strID).Replace("tiberoPWD", Logs.useConfig.strPWD);
            }
        }

        //현재 Process ID를 반환
        private string GetProcessID()
        {
            return System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            //return Process.GetCurrentProcess().Id.ToString();
        }

        //Select 실행 전 처리
        //Mapper가 생성되어있지 않다면 생성 후 Connection을 Open한다.
        private void PreSelectExecute(string datasourceCode)
        {
            //원래
            //if (maps.ContainsKey(datasourceCode + GetProcessID()))
            //  maps.Remove(datasourceCode + GetProcessID());

            //GenerateMapper(datasourceCode);   //Mapper가 생성되지 않았다면 생성한다.
            //OpenConnection(datasourceCode);   //Mapper의 Connection이 Open되지 않았다면 Open한다.

            //if (!maps.ContainsKey(datasourceCode + GetProcessID()))
            //  GenerateMapper(datasourceCode);   //Mapper가 생성되지 않았다면 생성한다.

            GenerateMapper(datasourceCode);
            OpenConnection(datasourceCode);   //Mapper의 Connection이 Open되지 않았다면 Open한다.
        }

        //CUD 실행 전 처리
        //PreSelectExecute를 실행 후 Transaction 처리를 추가한다.
        private void PreCUDExecute(string datasourceCode)
        {
            //원래
            //if (maps.ContainsKey(datasourceCode + GetProcessID()))
            //  maps.Remove(datasourceCode + GetProcessID());

            //GenerateMapper(datasourceCode);   //Mapper가 생성되지 않았다면 생성한다.
            //OpenConnection(datasourceCode);   //Mapper의 Connection이 Open되지 않았다면 Open한다.
            //BeginTransaction(datasourceCode);  //Mapper의 Transaction이 시작되지 않았다면 시작한다.

            //if (!maps.ContainsKey(datasourceCode + GetProcessID()))
            //  GenerateMapper(datasourceCode);   //Mapper가 생성되지 않았다면 생성한다.

            GenerateMapper(datasourceCode);   //Mapper가 생성되지 않았다면 생성한다.
            OpenConnection(datasourceCode);   //Mapper의 Connection이 Open되지 않았다면 Open한다.
            BeginTransaction(datasourceCode);  //Mapper의 Transaction이 시작되지 않았다면 시작한다.
        }

        //DB Log에 기록
        private void WriteLog(string datasourceCode, ISqlMapper mapper, string statementName, object parameterObject)
        {
            XmlConfigurator.Configure(new FileInfo(Properties.Resources.RES_LOG_CONF));

            if (Properties.Resources.RES_LOG_DB_YN.ToUpper().Equals("N")) return;

            IMappedStatement statement = mapper.GetMappedStatement(statementName);
            RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameterObject, mapper.LocalSession);
            statement.PreparedCommand.Create(request, mapper.LocalSession, statement.Statement, parameterObject);

            log.Debug("=Start===========================================[datasourceCode : " + datasourceCode + "]==========================================");
            log.Debug("");
            log.Debug(request.IDbCommand.CommandText);
            log.Debug("");
            log.Debug("==[Start Parameters]==");

            IDataParameterCollection parameters = request.IDbCommand.Parameters;

            for (int i = 0; i < parameters.Count; i++)
            {
                log.Debug("[" + i + "] " + ((DbParameter)parameters[i]).Value);
            }

            log.Debug("==[End Parameters]==");
            log.Debug("");
            log.Debug("=End===========================================[datasourceCode : " + datasourceCode + "]============================================");
        }
        #endregion

        #region Connection 처리 원형
        //Open Connection 원형
        private void OpenConnectionCore(ISqlMapper mapper)
        {
            if (!mapper.IsSessionStarted)
                mapper.OpenConnection();
        }

        //Begin Transaction 원형
        private void BeginTransactionCore(ISqlMapper mapper)
        {
            if (mapper.IsSessionStarted)
            {
                ISqlMapSession session = mapper.LocalSession;
                if (!session.IsTransactionStart)
                    session.BeginTransaction();
            }
        }

        //Commit Transaction 원형
        private void CommitTransactionCore(ISqlMapper mapper)
        {
            if (mapper.IsSessionStarted)
            {
                ISqlMapSession session = mapper.LocalSession;
                if (session.IsTransactionStart)
                    session.CommitTransaction();
            }
        }

        //Rollback Transaction 원형
        private void RollbackTransactionCore(ISqlMapper mapper)
        {
            if (mapper.IsSessionStarted)
            {
                ISqlMapSession session = mapper.LocalSession;
                if (session.IsTransactionStart)
                    session.RollBackTransaction();
            }
        }

        //Close Connection 원형
        private void CloseConnectionCore(ISqlMapper mapper)
        {
            if (mapper.IsSessionStarted)
                mapper.CloseConnection();
        }

        #endregion

        #region Connection 처리

        //Default Datasource Open Connection
        private void OpenConnection()
        {
            OpenConnectionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
            OpenConnectionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
        }

        //#########################################Another Datasource Open Connection
        private void OpenConnection(string datasourceCode)
        {
            OpenConnectionCore((ISqlMapper)maps[datasourceCode + GetProcessID()]);
        }

        //Default Datasource Begin Transaction
        private void BeginTransaction()
        {
            BeginTransactionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
        }

        //#########################################Another Datasource Begin Transaction
        private void BeginTransaction(string datasourceCode)
        {
            BeginTransactionCore((ISqlMapper)maps[datasourceCode + GetProcessID()]);
        }

        //Default Datasource Commit Transaction
        private void CommitTransaction()
        {
            CommitTransactionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
        }

        //#########################################Another Datasource Commit Transaction
        private void CommitTransaction(string datasourceCode)
        {
            CommitTransactionCore((ISqlMapper)maps[datasourceCode]);
        }

        //Default Datasource Rollback Transaction
        private void RollbackTransaction()
        {
            RollbackTransactionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
        }

        //#########################################Another Datasource Rollback Transaction
        private void RollbackTransaction(string datasourceCode)
        {
            RollbackTransactionCore((ISqlMapper)maps[datasourceCode]);
        }

        //Default Datasource Close Connection
        private void CloseConnection()
        {
            CloseConnectionCore((ISqlMapper)maps[Properties.Settings.Default.RES_DB_INS_DEFAULT + GetProcessID()]);
        }

        //#########################################Another Datasource Close Connection
        private void CloseConnection(string datasourceCode)
        {
            CloseConnectionCore((ISqlMapper)maps[datasourceCode]);
        }

        //Mapper pool에 있는 모든 mapper를 Close 처리
        private void CloseAll()
        {
            foreach (string key in maps.Keys)
            {
                CloseConnection(key);
            }
        }

        //Mapper pool에 있는 모든 mapper를 Close 처리
        public void publicCloseAll()
        {
            foreach (string key in maps.Keys)
            {
                CloseConnection(key);
            }
        }

        //#########################################Mapper pool에 있는 해당 Process에 관련된 모든 mapper를 Close 처리
        private void CloseAll(string prcID)
        {
            foreach (string key in maps.Keys)
            {
                if (key.Contains(prcID))
                    CloseConnection(key);
            }
        }

        //Mapper pool에 있는 모든 mapper를 Commit 처리
        private void CommitAll()
        {
            foreach (string key in maps.Keys)
            {
                CommitTransaction(key);
            }
        }
        //#########################################Mapper pool에 있는 해당 Process에 관련된 모든 mapper를 Commit 처리
        private void CommitAll(string prcID)
        {
            foreach (string key in maps.Keys)
            {
                if (key.Contains(prcID)) CommitTransaction(key);
            }
        }

        //Mapper pool에 있는 모든 mapper를 Rollback 처리
        private void RollbackAll()
        {
            foreach (string key in maps.Keys)
            {
                RollbackTransaction(key);
            }
        }

        //#########################################Mapper pool에 있는 해당 Process에 관련된 모든 mapper를 RollbackAll 처리
        private void RollbackAll(string prcID)
        {
            foreach (string key in maps.Keys)
            {
                if (key.Contains(prcID)) RollbackTransaction(key);
            }
        }

        //Mapper pool에 있는 모든 mapper를 삭제 처리
        private void DeleteAll()
        {
            ArrayList deleteKey = new ArrayList();

            foreach (string key in maps.Keys)
            {
                deleteKey.Add(key);
            }

            foreach (string key in deleteKey)
            {
                maps.Remove(key);
            }
        }

        //#########################################Mapper pool에 있는 해당 Process에 관련된 모든 mapper를 삭제 처리
        private void DeleteAll(string prcID)
        {
            ArrayList deleteKey = new ArrayList();

            foreach (string key in maps.Keys)
            {
                if (key.Contains(prcID)) deleteKey.Add(key);
            }

            foreach (string key in deleteKey)
            {
                maps.Remove(key);
            }

            //Debug.WriteLine("Connection Deleted!!!");
            //WritePoolDataToConsole();
        }

        #endregion

        #region SQL 처리 원형

        //Select DataTable 원형
        private DataTable QueryForTableCore(string statementName, object parameterObject, string datasourceCode)
        {
            DataTable dataTable = null;

            try
            {
                PreSelectExecute(datasourceCode);    //Select PreProcess 실행

                ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

                WriteLog(datasourceCode, mapper, statementName, parameterObject);

                dataTable = new DataTable(statementName);

                ISqlMapSession session = mapper.LocalSession;

                IMappedStatement statement = mapper.GetMappedStatement(statementName);
                RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameterObject, session);
                statement.PreparedCommand.Create(request, session, statement.Statement, parameterObject);

                using (request.IDbCommand)
                {
                    dataTable.Load(request.IDbCommand.ExecuteReader());
                }

                if (dataTable.Columns.Count != 0)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        col.AllowDBNull = true;
                        col.ReadOnly = false;
                        col.MaxLength = -1;
                    }
                }

                mapper.CloseConnection();
                //CloseAll(Process.GetCurrentProcess().Id.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }

            return dataTable;
        }

        ////Select List 원형
        private ArrayList QueryForListCore(string statementName, object parameterObject, string datasourceCode)
        {
            PreSelectExecute(datasourceCode);    //Select PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());

            return (ArrayList)mapper.QueryForList(statementName, parameterObject);
        }

        //Select One Line 원형
        private Hashtable QueryForOnelineCore(string statementName, object parameterObject, string datasourceCode)
        {
            Hashtable result = new Hashtable();

            PreSelectExecute(datasourceCode);    //Select PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            object tmpResult = mapper.QueryForObject(statementName, parameterObject);
            if (tmpResult != null) result = (Hashtable)tmpResult;

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());

            return result;
        }

        //Select object 원형
        private object QueryForObjectCore(string statementName, object parameterObject, string datasourceCode)
        {
            PreSelectExecute(datasourceCode);    //Select PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());

            return mapper.QueryForObject(statementName, parameterObject);
        }

        //Insert 원형
        private object QueryForInsertCore(string statementName, object parameterObject, string datasourceCode)
        {
            PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());
            //CommitAll();

            return mapper.Insert(statementName, parameterObject);
        }

        //Update 원형
        private object QueryForUpdateCore(string statementName, object parameterObject, string datasourceCode)
        {
            PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());
            //CommitAll();

            return mapper.Update(statementName, parameterObject);
        }

        //Delete 원형
        private object QueryForDeleteCore(string statementName, object parameterObject, string datasourceCode)
        {
            PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            mapper.CloseConnection();
            //CloseAll(Process.GetCurrentProcess().Id.ToString());
            //CommitAll();

            return mapper.Delete(statementName, parameterObject);
        }

        #endregion

        #region SQL 처리

        //Select DataTable (Default Datasource)
        public DataTable QueryForTable(string statementName, object parameterObject)
        {
            return QueryForTableCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Select DataTable (Another Datasource)
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public DataTable QueryForTable(string statementName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForTableCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return (DataTable)objresult;
        }

        //Select List (Default Datasource)
        public ArrayList QueryForList(string statementName, object parameterObject)
        {
            return QueryForListCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Select List (Another Datasource)
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public ArrayList QueryForList(string statementName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForListCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return (ArrayList)objresult;
        }

        //Select One Line (Default Datasource)
        public Hashtable QueryForOneline(string statementName, object parameterObject)
        {
            return QueryForOnelineCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Select One Line (Another Datasource)
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public Hashtable QueryForOneline(string statementName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForOnelineCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return (Hashtable)objresult;
        }

        //Select object (Default Datasource)
        public object QueryForObject(string statementName, object parameterObject)
        {
            return QueryForObjectCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Select object (Another Datasource)
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public object QueryForObject(string statementName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForObjectCore(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return objresult;
        }

        //Insert (Default Datasource)
        public object QueryForInsert(string stateName, object parameterObject)
        {
            return QueryForInsertCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Insert (Another Datasource)
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public object QueryForInsert(string stateName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForInsertCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return objresult;
        }

        //Update (Default Datasource)
        public object QueryForUpdate(string stateName, object parameterObject)
        {
            return QueryForUpdateCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Update (Another Datasource)
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public object QueryForUpdate(string stateName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForUpdateCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return objresult;
        }

        //Delete (Default Datasource)
        public object QueryForDelete(string stateName, object parameterObject)
        {
            return QueryForDeleteCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        /// <summary>
        /// //Delete (Another Datasource)
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        /// <returns></returns>
        public object QueryForDelete(string stateName, object parameterObject, ConnectConfig tempConfig)
        {
            objresult = null;

            Logs.configChange(tempConfig);

            objresult = QueryForDeleteCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);

            Logs.configChange(Logs.DefaultConfig);

            return objresult;
        }

        #endregion

        #region 벌크Insert (우선 보류)
        private void QueryForBulkInsertCore(string statementName, ArrayList parameterObject, string datasourceCode)
        {
            if (datasourceCode.Equals("ORACLEConfig"))
            {
                PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
                ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];
                IMappedStatement statement = mapper.GetMappedStatement(statementName);
                WriteLog(datasourceCode, mapper, statementName, parameterObject);
                RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameterObject, mapper.LocalSession);
                statement.PreparedCommand.Create(request, mapper.LocalSession, statement.Statement, parameterObject);

                OracleCommand commandOracle = new OracleCommand();
                commandOracle.CommandText = request.IDbCommand.CommandText;

                commandOracle.Connection = (OracleConnection)mapper.LocalSession.Connection;
                commandOracle.ArrayBindCount = ((List<string>)parameterObject[0]).Count;

                for (int i = 0; i < parameterObject.Count; i++)
                {
                    OracleParameter tmpParamList = new OracleParameter(i + 1 + "", OracleDbType.Varchar2);
                    tmpParamList.Direction = ParameterDirection.Input;
                    tmpParamList.Value = ((List<string>)parameterObject[i]).ToArray();
                    commandOracle.Parameters.Add(tmpParamList);
                }

                commandOracle.ExecuteNonQuery();

                CommitTransaction();
            }
            else if (datasourceCode.Equals("TIBEROConfig"))
            {
                try
                {
                    PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
                    ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];
                    IMappedStatement statement = mapper.GetMappedStatement(statementName);
                    WriteLog(datasourceCode, mapper, statementName, parameterObject);
                    RequestScope request = statement.Statement.Sql.GetRequestScope(statement, parameterObject, mapper.LocalSession);
                    statement.PreparedCommand.Create(request, mapper.LocalSession, statement.Statement, parameterObject);

                    OleDbCommandTbr commandTB = new OleDbCommandTbr();
                    commandTB.CommandType = CommandType.Text;
                    commandTB.CommandText = request.IDbCommand.CommandText;
                    commandTB.Connection = new OleDbConnectionTbr(mapper.DataSource.ConnectionString);
                    commandTB.Connection.Open();

                    //commandTB.Parameters.isContainArray = true;

                    for (int i = 0; i < parameterObject.Count; i++)
                    {
                        //tmpParamList = null;
                        OleDbParameterTbr tmpParamList = new OleDbParameterTbr(i + 1 + "", OleDbTypeTbr.VarChar);
                        tmpParamList.Direction = ParameterDirection.Input;
                        tmpParamList.Value = ((List<string>)parameterObject[i]).ToArray();
                        //commandTB.Parameters.Add(tmpParamList);
                        commandTB.Parameters.parametersTbr.Add(tmpParamList);
                    }

                    commandTB.ExecuteNonQuery();

                    CommitTransaction();
                }
                catch (Exception ex)
                { }
            }
        }

        //Oracle Bulk Insert (Default Datasource)
        public void QueryForBulkInsert(string stateName, ArrayList parameterObject)
        {
            QueryForBulkInsertCore(stateName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }

        //Oracle Bulk Insert (Another Datasource)
        public void QueryForBulkInsert(string stateName, ArrayList parameterObject, string datasourceCode)
        {
            QueryForBulkInsertCore(stateName, parameterObject, datasourceCode);
        }
        #endregion

        #region Insert 반복
        public void QueryForInsertForeach(string stateName, Hashtable parameterObject, ArrayList arr)
        {
            try
            {
                QueryForInsertForeachCore(stateName, parameterObject, arr, Properties.Settings.Default.RES_DB_INS_DEFAULT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Insert 반복 Hashtable 0번은 ArrayList NAME : arr
        /// 1번은 기존 Hashtable
        /// </summary>
        /// <param name="statementName"></param>
        /// <param name="parameterObject"></param>
        /// <param name="datasourceCode"></param>
        private void QueryForInsertForeachCore(string statementName, Hashtable parameterObject, ArrayList arr, string datasourceCode)
        {
            try
            {
                PreCUDExecute(datasourceCode);    //CUD PreProcess 실행
                ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

                Hashtable conditions = new Hashtable();

                int cnt = 0;

                foreach (Hashtable ht in arr)
                {
                    try
                    {
                        conditions.Clear();

                        if (parameterObject != null)
                        {
                            foreach (DictionaryEntry entry in (parameterObject as Hashtable))
                            {
                                if (!conditions.ContainsKey(entry.Key))
                                {
                                    conditions.Add(entry.Key, entry.Value);
                                }
                            }
                        }

                        foreach (DictionaryEntry entry in ht)
                        {
                            if (!conditions.ContainsKey(entry.Key))
                            {
                                conditions.Add(entry.Key, entry.Value);
                            }
                        }

                        //WriteLog(datasourceCode, mapper, statementName, conditions);

                        if (cnt == 0)
                            BeginTransaction();

                        mapper.Insert(statementName, conditions);
                        cnt++;

                        if (cnt == 1000)
                        {
                            CommitTransaction();
                            cnt = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                CommitTransaction();
                mapper.CloseConnection();
                CloseAll(Process.GetCurrentProcess().Id.ToString());
                CommitAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region FMS 추가
        //Select ListObj (Default Datasource)
        public IList<T> QueryForListObj<T>(string statementName, object parameterObject)
        {
            return QueryForListObjCore<T>(statementName, parameterObject, Properties.Settings.Default.RES_DB_INS_DEFAULT);
        }
        ////Select ListObj 원형
        private IList<T> QueryForListObjCore<T>(string statementName, object parameterObject, string datasourceCode)
        {
            PreSelectExecute(datasourceCode);       //Select PreProcess 실행
            ISqlMapper mapper = (ISqlMapper)maps[datasourceCode + GetProcessID()];

            WriteLog(datasourceCode, mapper, statementName, parameterObject);

            CloseAll(Process.GetCurrentProcess().Id.ToString());

            return (IList<T>)mapper.QueryForList<T>(statementName, parameterObject);
        }

        #endregion
    }
}
