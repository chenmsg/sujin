using System;
using System.Data;
using System.Data.OleDb;

namespace ITOrm.Core.Utility.Files
{
    public class ExcelHelper
    {

        /// <summary>
        /// 导入数据到数据集中
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="TableName"></param>
        /// <param name="tablename2">如果这个有就以他为表名，没有的话就以TableName</param>
        /// <returns></returns>
        public static DataTable InputExcel(string Path, string TableName)
        {
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter("select * from [" + TableName + "$]", conn);
                oda.Fill(ds);
                conn.Close();
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
