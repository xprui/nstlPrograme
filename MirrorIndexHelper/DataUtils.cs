using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace MirrorIndexHelper
{
    class DataUtils
    {
        static public Dictionary<string,string> GetUnitNames()
        {
            Dictionary<string, string> objResult = new Dictionary<string, string>();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                conn.Open();
                cmd.CommandText = "select f_unitcode,f_unitname from t_unitinfo";
                IDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string code = Convert.ToString(reader["f_unitcode"]);
                    string name = Convert.ToString(reader["f_unitname"]);
                    objResult[code] = name;
                }
            }
            catch(Exception err)
            {
                throw err;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return objResult;
        }
        static public bool GetLocalPeriodicalDataCount(string untiCode,string batchNum,string year,out int count)
        {
            bool res = true;
            SqlTool mssql = new SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            SqlCommand cmd = mssql.Connection.CreateCommand();
            string sql = "select count(0) as cnt from t_periodical_eng where 1=1 ";
            sql = sql + " and f_batch=@batch and f_year>=@year\r\n";
            sql = sql + " and f_journalcode in(\r\nselect f_journalcode from t_locperlist where f_unitcode=@code and f_journalcode is not null and f_journalcode !=''\r\n)";
            try
            {
                if (mssql.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                cmd.CommandText = sql;
                cmd.CommandTimeout = 1000;
                cmd.Parameters.AddWithValue("@batch", batchNum);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@code", untiCode);
                cmd.ExecuteNonQuery();
                count =  Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception err)
            {
                res = false;
                throw err;
            }
            finally
            {
                cmd.Dispose();
                mssql.Dispose();
            }
            return res;
        }
        static  public string PickLocalPeriodicalDataToTable(string unitCode,string batchNum,string year)
        {
            string tablename = "tmp_" + unitCode + "_" + batchNum + "_" + year;
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            List<string> tables = mssql.GetTableList();
            string sql = string.Empty;
            SqlCommand cmd = mssql.Connection.CreateCommand();
            if (tables.Contains(tablename))
            {
                sql = "drop table " + tablename;
                cmd.CommandText = sql;
                try
                {
                    if (mssql.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception err)
                {
                    throw err;
                }
            }
            sql = "select f_doc_id,--f_title,f_year,f_issn,\r\n";
            //sql = sql + "replace(stuff(replace(convert(nvarchar(max),([F_Content].query('for $id in //authorlist/author   order by fn:number($id/author_sequence[1]) return $id/author_name')),0),'</author_name>',''),1,13,''),'<author_name>',';') as authors,\r\n";
            //sql = sql + "[F_Content].value('(//paper/start_page)[1]','nvarchar(100)') as start_page,\r\n";
            //sql = sql + "[F_Content].value('(//paper/end_page)[1]', 'nvarchar(100)') as end_page,\r\n";
            //sql = sql + "[F_Content].value('(//paper/total_page_number)[1]','nvarchar(100)') as total_page,\r\n";
            //sql = sql + "[F_Content].value('(//paper/issue/volume)[1]','nvarchar(200)') as volume,\r\n";
            //sql = sql + "[F_Content].value('(//paper/issue/issue)[1]', 'nvarchar(200)') as issue,\r\n";
            //sql = sql + "[F_Content].value('(//paper/journal/host_title)[1]','nvarchar(max)') as host_title,\r\n";
            sql = sql + "cast(null as int) as del_flag,cast(null as nvarchar(50)) as uniqueid\r\n";
            //sql = sql + "cast(null as nvarchar(300)) as pdf\r\n";
            sql = sql + "into " + tablename + "\r\n from t_periodical_eng \r\nwhere f_batch=@batch and f_year>=@year\r\n";
            sql = sql + " and f_journalcode in(\r\nselect f_journalcode from t_locperlist where f_unitcode=@code and f_journalcode is not null and f_journalcode !=''\r\n)";
            try
            {
                if (mssql.Connection.State != ConnectionState.Open)
                    mssql.Connection.Open();
                cmd.CommandText = sql;
                cmd.CommandTimeout = 1000;
                cmd.Parameters.AddWithValue("@batch", batchNum);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@code", unitCode);
                cmd.ExecuteNonQuery();
                sql = "alter table " + tablename + " add id int identity(1,1) primary key";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                cmd.Dispose();
                mssql.Dispose();
            }
            
            return tablename;
        }

        static public bool UpdateDBData(string sql)
        {
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            try
            {
                return mssql.Update(sql) == 1;
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                mssql.Dispose();
            }
            return false;
        }

        static public  int  GetDataCount(string sql)
        {
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            try
            {
                return Convert.ToInt32(mssql.Scaler(sql));
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                mssql.Dispose();
            }
            return 0;
        }
    }
}
