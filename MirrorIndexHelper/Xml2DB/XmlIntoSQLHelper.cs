using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    /// <summary>
    /// XML入库的操作类
    /// </summary>
    class XmlIntoSQLHelper
    {
        public Dictionary<string, LiteratureType> dicLiterInfos = null;
        public Dictionary<string,LiteratureType> GetAllLiterInfos()
        {
            if(null == dicLiterInfos)
            {
                dicLiterInfos = new Dictionary<string, LiteratureType>();
                SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
                string sql = "select f_code,f_type,f_tablename from t_literaturetype";
                System.Data.IDataReader reader = mssql.DoReader(sql);
                while(reader.Read())
                {
                    LiteratureType lt = new LiteratureType();
                    lt.F_Code = Convert.ToString(reader["f_code"]);
                    lt.F_TableName = Convert.ToString(reader["f_tablename"]);
                    lt.F_Type = Convert.ToString(reader["f_type"]);
                    dicLiterInfos[lt.F_Type] = lt;
                }
                reader.Close();
                reader.Dispose();
                mssql.Dispose();
            }
            return dicLiterInfos;
        }
        public LiteratureType GetLiterInfoByType(string cnType)
        {
            if (null == dicLiterInfos)
                GetAllLiterInfos();
            if (!dicLiterInfos.ContainsKey(cnType))
                throw new Exception("没有匹配的文献类型");
            return dicLiterInfos[cnType];
        }

        public void TransXmlDataIntoSQL(List<string> sqlInsertQueries)
        {
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            mssql.TransData(sqlInsertQueries);
            mssql.Dispose();
        }

        public void UpdateInsertData(string sql)
        {
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            mssql.Command.CommandTimeout = 10000000;
            mssql.Update(sql);
            mssql.Dispose();
        }
    }
}
