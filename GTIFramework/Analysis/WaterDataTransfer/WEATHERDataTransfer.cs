using GTIFramework.Common.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GTIFramework.Analysis.WaterDataTransfer
{
    public class WEATHERDataTransfer
    {
        public WEATHERDataTransfer()
        {

        }

        XmlDocument xmlDoc = null;

        /// <summary>
        /// 초단기실황조회(매 1시) API업데이트 시간 40분 (수집O)
        /// 초단기예보조회(매 30시) API업데이트 시간 40분 (수집O)
        /// 동네예보조회 (02, 05, 08, 11, 14, 17, 20, 23 (#3시간 단위)) API업데이트 시간 40분 (수집X)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public DataTable SelectForecast(string url)
        {
            DataTable dtResult = new DataTable();

            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(url);

                if (xmlDoc != null)
                {
                    dtResult = XmlParsing(xmlDoc);
                }
            }
            catch (Exception ex)
            {
                dtResult = null;
                Messages.ErrLog(ex);
            }
            finally
            {
                xmlDoc = null;
            }

            return dtResult;
        }

        /// <summary>
        /// XmlParsing
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        private DataTable XmlParsing(XmlDocument xmlDoc)
        {
            XmlNodeList xmlNode = null;
            DataTable resultlist = new DataTable();

            try
            {
                xmlNode = xmlDoc.SelectNodes("/response/body/items/item");

                foreach (XmlNode items in xmlNode)
                {
                    if (xmlNode.Item(0).Equals(items))
                    {
                        foreach (XmlElement item in items)
                        {
                            resultlist.Columns.Add(item.Name.ToUpper());
                        }
                    }

                    DataRow row = null;

                    foreach (XmlElement item in items)
                    {
                        if (row == null)
                        {
                            row = resultlist.NewRow();
                        }

                        row[item.Name.ToUpper()] = item.InnerText;
                    }

                    if (row != null)
                    {
                        resultlist.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                resultlist = null;
                Messages.ErrLog(ex);
            }
            finally
            {
                xmlDoc = null;
                xmlNode = null;
            }

            return resultlist;
        }
    }
}
