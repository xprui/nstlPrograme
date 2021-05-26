using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MirrorIndexHelper
{
    public delegate void Progress(int total, int pos);
    class MatchPeriodical
    {
        private static event Progress onProgress;
        public static event Progress OnProgress
        {
            add { onProgress += value; }
            remove { onProgress -= value; }
        }

        static public void Match(string tablename,int datacount)
        {
            int ipgs = DataUtils.GetDataCount("SELECT count(0) FROM " + tablename + " where uniqueid is not null or del_flag is not null") ;
            SqlTool mssql = new global::SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            DocumentFinder.SearchDocument finder = new DocumentFinder.SearchDocument();
            finder.Timeout = 300000;
            string sql = string.Empty;
            SqlCommand cmd = mssql.Connection.CreateCommand();
            try
            {
                if (mssql.Connection.State != ConnectionState.Open)
                    mssql.Connection.Open();
                sql = "select f_doc_id,f_title,f_year,f_issn,\r\n";
                sql = sql + "replace(stuff(replace(convert(nvarchar(max),([F_Content].query('for $id in //authorlist/author   order by fn:number($id/author_sequence[1]) return $id/author_name')),0),'</author_name>',''),1,13,''),'<author_name>',';') as authors,\r\n";
                sql = sql + "[F_Content].value('(//paper/start_page)[1]','nvarchar(100)') as start_page,\r\n";
                sql = sql + "[F_Content].value('(//paper/end_page)[1]', 'nvarchar(100)') as end_page,\r\n";
                sql = sql + "[F_Content].value('(//paper/total_page_number)[1]','nvarchar(100)') as total_page,\r\n";
                sql = sql + "[F_Content].value('(//paper/issue/volume)[1]','nvarchar(200)') as volume,\r\n";
                sql = sql + "[F_Content].value('(//paper/issue/issue)[1]', 'nvarchar(200)') as issue,\r\n";
                sql = sql + "[F_Content].value('(//paper/journal/host_title)[1]','nvarchar(max)') as host_title,\r\n";
                sql = sql + "cast(null as int) as del_flag,cast(null as nvarchar(50)) as uniqueid\r\n";
                sql = sql + " from t_periodical_eng where f_doc_id in (select f_doc_id from " + tablename + " where uniqueid is null and del_flag is null)";
                cmd.CommandText = sql;
                IDataReader reader = cmd.ExecuteReader();
                PeriodicalEng pe = null;
                while (reader.Read())
                {
                    ipgs++;
                    if (null != onProgress && ipgs % 20 == 0)
                    {
                        onProgress(datacount, ipgs);
                        if (ipgs % 100 == 0)
                        {
                            finder.Dispose();
                            finder = new DocumentFinder.SearchDocument();
                            finder.Timeout = 60000;
                        }
                    }
                    pe = new MirrorIndexHelper.PeriodicalEng();
                    //pe.F_Author = Convert.ToString(reader["authors"]);
                    pe.F_EndPage = Convert.ToString(reader["end_page"]);
                    pe.F_Issn = Convert.ToString(reader["f_issn"]);
                    pe.F_Issue = Convert.ToString(reader["issue"]);
                    System.Text.RegularExpressions.Match ma = System.Text.RegularExpressions.Regex.Match(pe.F_Issue, @"[a-z]{3}\.[\d]*\s?[a-z]*\.(?<value>[\d]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (ma.Success)
                        pe.F_Issue = ma.Groups["value"].Value.Trim();
                    pe.F_PeriodicalName = Convert.ToString(reader["host_title"]);
                    pe.F_StartPage = Convert.ToString(reader["start_page"]);
                    pe.F_Title = Convert.ToString(reader["f_title"]);
                    pe.F_TotalPage = Convert.ToString(reader["total_page"]);
                    pe.F_Volume = Convert.ToString(reader["volume"]);
                    pe.F_year = Convert.ToString(reader["f_year"]);
                    string docID = Convert.ToString(reader["f_doc_id"]);
                    string uid = string.Empty;
                    try
                    {
                        uid = finder.Search(1, pe.ToString());
                    }
                    catch(Exception matchError)
                    {
                        if (matchError.Message.IndexOf("超时") > 0)
                        {
                            finder.Dispose();
                            finder = new DocumentFinder.SearchDocument();
                            System.Threading.Thread.Sleep(500);
                            uid = finder.Search(1, pe.ToString());
                            if (!string.IsNullOrEmpty(uid))
                                DataUtils.UpdateDBData("update " + tablename + " set uniqueid='" + uid + "' where f_doc_id='" + docID + "'");
                            else
                                DataUtils.UpdateDBData("update " + tablename + " set del_flag=1 where f_doc_id='" + docID + "'");
                        }
                        else
                        {
                            DataUtils.UpdateDBData("update " + tablename + " set del_flag=1 where f_doc_id='" + docID + "'");
                        }
                        continue;
                    }
                    if (!string.IsNullOrEmpty(uid))
                        DataUtils.UpdateDBData("update " + tablename + " set uniqueid='" + uid + "' where f_doc_id='" + docID + "'");
                    else
                        DataUtils.UpdateDBData("update " + tablename + " set del_flag=1 where f_doc_id='" + docID  + "'");
                }
                reader.Close();
                reader.Dispose();
                if(null != onProgress)
                onProgress(datacount, datacount);
            }
            catch(Exception err)
            {
                finder.Search(-1, "recache");
                finder.Dispose();
                GC.Collect();
                System.Threading.Thread.Sleep(1000);
                GC.Collect();
                System.Threading.Thread.Sleep(1000);
                GC.Collect();
                System.Threading.Thread.Sleep(1000);
                GC.Collect();
                System.Threading.Thread.Sleep(1000);
                throw err;
            }
            finally
            {
                cmd.Dispose();
                mssql.Dispose();
            }
        }
    }
}
