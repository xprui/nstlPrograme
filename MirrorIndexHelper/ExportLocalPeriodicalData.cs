using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper
{
    class ExportLocalPeriodicalData
    {
        private static event Progress onProgress;
        public static event Progress OnProgress
        {
            add { onProgress += value; }
            remove { onProgress -= value; }
        }

        static public void ExportToSqlite(string tablename,string s3dbFile)
        {
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            
            SqliteTool sqlite = new SqliteTool(s3dbFile);
            string insert = "insert into t_localjournal(doc_id,f_uniqueid)values('{0}','{1}');";
            int itotal = Convert.ToInt32(mssql.Scaler("select count(0) from " + tablename + " where uniqueid is not null"));
            System.Data.IDataReader  reader = mssql.DoReader("select f_doc_id,uniqueid from " + tablename + " where uniqueid is not null and (del_flag is null or del_flag!=1)");
            int ipgs = 0;
            while(reader.Read())
            {
                ipgs++;
                if(ipgs % 20 ==0 && null != onProgress)
                    onProgress(itotal, ipgs);
                string docid = Convert.ToString(reader["f_doc_id"]);
                string uid = Convert.ToString(reader["uniqueid"]);
                sqlite.Insert(string.Format(insert, docid, uid));
            }
            reader.Close();
            reader.Dispose();
            sqlite.Dispose();
            mssql.Dispose();
        }
    }
}
