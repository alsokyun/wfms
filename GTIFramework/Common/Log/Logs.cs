using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net;
using log4net.Config;
using GTIFramework.Common.ConfigClass;
using System.Collections;
using System.Threading;
using System.Windows.Controls;
using System.Configuration;

namespace GTIFramework.Common.Log
{
    public class Logs
    {
        public static Thread WaterRateSavethread;
        public static ProgressBar progressunlimit;

        public static Hashtable htPermission = new Hashtable();
        public static Hashtable htMenuGoParam = new Hashtable();
        public static string strFocusMNU_CD = "";

        public static string strLogin_ID = "";
        public static string strLogin_PW = "";
        public static string strLogin_SITE = "";
        public static string strLogin_IP = "";
        public static string strLogin_NM = "";

        public static ConnectConfig DefaultConfig = new ConnectConfig();
        public static ConnectConfig WNMSConfig = new ConnectConfig();
        public static ConnectConfig WQMSConfig = new ConnectConfig();
        public static ConnectConfig WPMSConfig = new ConnectConfig();
        public static ConnectConfig HMIConfig = new ConnectConfig();



        #region SMS Queue 정의
        public static List<object> SMSQueue = new List<object>();

        //정상적인 Queue Add
        public static void SMSAdd(object obj)
        {
            SMSQueue.Insert(SMSQueue.Count - 1, obj);
        }

        //긴급 먼저 처리 Queue Add
        public static void SMSAdd_interrupt(object obj)
        {
            SMSQueue.Insert(0, obj);
        }

        //처리 Queue Remove
        public static void SMSSend(object obj)
        {
            SMSQueue.RemoveAt(0);
        } 
        #endregion

        //실 접속 정보
        public static ConnectConfig useConfig = new ConnectConfig();

        //쓰레드 WatchDog
        public static Hashtable htthread = new Hashtable();

        public static void ErrLogging(Exception e)
        {
            XmlConfigurator.Configure(new FileInfo(Properties.Resources.RES_LOG_CONF));

            if (Properties.Resources.RES_LOG_ERR_YN.ToUpper().Equals("Y"))
            {
                ILog errLog = LogManager.GetLogger("errLogger");

                errLog.Debug("=Start=========================================================================================================");
                errLog.Debug("");
                errLog.Debug(e);
                errLog.Debug("");
                errLog.Debug("=End===========================================================================================================");
            }
        }

        public static void ErrLogging(string strMessage)
        {
            XmlConfigurator.Configure(new FileInfo(Properties.Resources.RES_LOG_CONF));

            if (Properties.Resources.RES_LOG_ERR_YN.ToUpper().Equals("Y"))
            {
                ILog errLog = LogManager.GetLogger("errLogger");

                errLog.Debug("=Start=========================================================================================================");
                errLog.Debug("");
                errLog.Debug(strMessage);
                errLog.Debug("");
                errLog.Debug("=End===========================================================================================================");
            }
        }

        public static void DBdefault()
        {
            // also : 초기설정값 변경하기위해 임시처리
            Properties.Settings.Default.Reset();

            DefaultConfig.strIP = Properties.Settings.Default.strIP;
            DefaultConfig.strPort = Properties.Settings.Default.strPort;
            DefaultConfig.strSID = Properties.Settings.Default.strSID;
            DefaultConfig.strID = Properties.Settings.Default.strID;
            DefaultConfig.strPWD = Properties.Settings.Default.strPWD;

            configChange(DefaultConfig);
        }

        public static void configChange(ConnectConfig tempconfig)
        {
            useConfig = tempconfig;
        }

        public static void setDBConfig(string strDBCAT)
        {
            Properties.Settings.Default.RES_DB_INS_DEFAULT = strDBCAT;
            Properties.Settings.Default.Save();
        }
    }
}
