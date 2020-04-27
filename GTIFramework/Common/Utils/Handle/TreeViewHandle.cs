using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GTIFramework.Common.Utils.Handle
{
    public class TreeViewHandle
    {
        /// <summary>
        /// 인덱스로 Hierarchy [0]:ID, [1]:PID, [2]:NAME (사용안함 GridControl TreeListView 사용)
        /// </summary>
        /// <param name="treeData"></param>
        /// <param name="tv"></param>
        public static void HierarchyConverter(DataTable treeData, TreeView tv)
        {
            if (!(treeData.Rows.Count == 0))
            {
                foreach (DataRow r in treeData.Rows)
                {
                    //최상위 item
                    if (r[1].Equals("") || r[1].Equals(DBNull.Value))
                    {
                        TreeViewItem pitem = new TreeViewItem()
                        {
                            Name = r[0].ToString(),
                            Header = r[2].ToString(),
                        };

                        tv.RegisterName(pitem.Name, pitem);
                        tv.Items.Add(pitem);
                    }
                    //하위 item
                    else
                    {
                        if (tv.FindName(r[1].ToString()) != null)
                        {
                            TreeViewItem ptvitem = tv.FindName(r[1].ToString()) as TreeViewItem;

                            TreeViewItem citem = new TreeViewItem()
                            {
                                Name = r[0].ToString(),
                                Header = r[2].ToString(),
                            };

                            tv.RegisterName(citem.Name, citem);
                            ptvitem.Items.Add(citem);
                        }
                    }
                }
            }
        }
    }
}
