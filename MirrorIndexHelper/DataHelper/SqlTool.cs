using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using System.Windows.Forms;
//SELECT * FROM INFORMATION_SCHEMA.COLUMNS where table_name='表名';-->得到字段信息
//SELECT * FROM INFORMATION_SCHEMA.SCHEMATA;-->得到表的权限分配

    public class SqlTool : DataHelper.IDataTool
    {
        private string _server;
        public string Server
        {
            get { return this._server; }
            set { this._server = value; }
        }
        private string _dbname;
        public string DBName
        {
            get { return this._dbname; }
            set { this._dbname = value; }
        }
        private string _user;
        public string User
        {
            get { return this._user; }
            set { this._user = value; }
        }
        private string _password;
        public string Password
        {
            get { return this._password; }
            set { this._password = value; }
        }
        private string _SqlConnectionString;
        public string ConnectionString
        {
            get { return this._SqlConnectionString; }
            set
            {
                this._SqlConnectionString = value;
                SqlConnectionStringBuilder ssb = new SqlConnectionStringBuilder(value);
                this._password = ssb.Password;
                this._user = ssb.UserID;
                this._server = ssb.DataSource;
                this._dbname = ssb.InitialCatalog;
                ssb = null;
            }
        }
        private SqlDataReader _reader;
        public SqlDataReader Reader
        {
            get { return this._reader; }
            set { this._reader = value; }
        }
        private SqlConnection _SqlConn;
        public SqlConnection Connection
        {
            get { return this._SqlConn; }
            set { this._SqlConn = value; }
        }
        private SqlCommand _SqlCmd;
        public SqlCommand Command
        {
            get { return this._SqlCmd; }
            set { this._SqlCmd = value; }
        }
        private DataTable _dt = new DataTable();
        public DataTable DaTable
        {
            get { return this._dt; }
            set { this._dt = value; }
        }
        /// <summary>
        /// 无参构造函数,引用配置中的连接串
        /// </summary>
        public SqlTool()
        {
            this._SqlConnectionString = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"].ToString();
            this._SqlConn = new SqlConnection(this._SqlConnectionString);
            this._SqlCmd = this._SqlConn.CreateCommand();

            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder(this._SqlConnectionString);
            this._server = connBuilder.DataSource;
            this._dbname = connBuilder.InitialCatalog;
            this._user = connBuilder.UserID;
            this._password = connBuilder.Password;
            connBuilder = null;
        }
        public SqlTool(string strConnectionString)
        {
            this._SqlConnectionString = strConnectionString;
            this._SqlConn = new SqlConnection(strConnectionString);
            this._SqlCmd = this.Connection.CreateCommand();
        }
        public SqlTool(string server, string dbname, string user, string pass)
        {
            this._server = server;
            this._dbname = dbname;
            this._user = user;
            this._password = pass;
            SqlConnectionStringBuilder constrbuilder = new SqlConnectionStringBuilder();
            constrbuilder.UserID = user;
            constrbuilder.Password = pass;
            constrbuilder.InitialCatalog = dbname;
            constrbuilder.IntegratedSecurity = false;//sql连接,true是windows连接
            constrbuilder.DataSource = server;
            this._SqlConnectionString = constrbuilder.ConnectionString;
            this._SqlConn = new SqlConnection(this._SqlConnectionString);
            this._SqlCmd = this.Connection.CreateCommand();
            constrbuilder = null;
            //this._server = constrbuilder.DataSource;
            //this._dbname = constrbuilder.InitialCatalog;
            //this._user = constrbuilder.UserID;
            //this._password = constrbuilder.Password;
        }
        private bool isConnectionOpen()
        {
            return this._SqlConn.State == ConnectionState.Open;
        }
        public void Dispose()
        {
            this._SqlCmd.Dispose();
            this._SqlConn.Close();
            this._SqlConn.Dispose();
        }
        public int Delete(string sql)
        {
            this._SqlCmd.CommandText = sql;
            int iDelID = -1;
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                iDelID = _SqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return iDelID;
        }
        public string Scaler(string sql)
        {
            this._SqlCmd.CommandText = sql;
            string strRes = string.Empty;
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                IDataReader reader = _SqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    strRes = reader[0].ToString();
                }
                reader.Close();
                reader.Dispose();
                //object obj = _SqlCmd.ExecuteScalar();
                //if (obj.Equals(DBNull.Value))
                //    strRes = string.Empty;
                //else
                //    strRes = obj.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return strRes;
        }
        public int Update(string sql)
        {
            this._SqlCmd.CommandText = sql;
            int iUpdateID = -1;
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                iUpdateID = _SqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return iUpdateID;
        }
        public DataTable Query(string sql)
        {
            this._SqlCmd.CommandText = sql;
            DataTable _dt = new DataTable();
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                SqlDataAdapter sqlAda = new SqlDataAdapter(this._SqlCmd.CommandText, this.Connection);
                sqlAda.Fill(_dt);
                sqlAda.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return _dt;
        }
        public int Insert(string sql)
        {
            this._SqlCmd.CommandText = sql;
            int iInsertID = -1;
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                iInsertID = _SqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return iInsertID;
        }

        public System.Data.IDataReader DoReader(string sql)
        {
            this._SqlCmd.CommandText = sql;
            this._SqlCmd.CommandTimeout = 0;
            int iInsertID = -1;
            try
            {
                if (!isConnectionOpen()) this._SqlConn.Open();
                _reader = _SqlCmd.ExecuteReader();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Dispose();
            }
            return _reader;
        }
        /// <summary>
        /// 获取库中的表名列表
        /// </summary>
        public List<string> GetTableList()
        {
            #region 放弃的方法
            /*
        List<string> lstRes = new List<string>();
        OleDbConnection olecn = new OleDbConnection("Provider=SQLOLEDB.1;Initial Catalog="+this._dbname+";Data Source="+this._server+";User ID="+this._user+"; Password="+this._password+"");
        olecn.Open();
        DataTable dt = olecn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,new object[] {null, null, null, "TABLE"});
        DataRow[] rows = dt.Select("TABLE_TYPE='TABLE'"); 
        foreach (DataRow row in rows)
        {
            lstRes.Add((string)row["TABLE_NAME"]);
        }
        
        dt.Dispose();
        olecn.Close();
        olecn.Dispose();
        return lstRes;
        */
            #endregion

            List<string> res = new List<string>();
            string tmpCmdText = _SqlCmd.CommandText;
            _SqlCmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE='BASE TABLE' order by TABLE_NAME";
            if (!isConnectionOpen()) _SqlConn.Open();
            SqlDataReader reader = _SqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    res.Add(reader["TABLE_NAME"].ToString());
                }
            }
            reader.Close();
            reader.Dispose();
            return res;
        }
        public List<string> GetViewList()
        {
            List<string> res = new List<string>();
            string tmpCmdText = _SqlCmd.CommandText;
            _SqlCmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE='VIEW' order by TABLE_NAME";
            if (!isConnectionOpen()) _SqlConn.Open();
            SqlDataReader reader = _SqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    res.Add(reader["TABLE_NAME"].ToString());
                }
            }
            reader.Close();
            reader.Dispose();
            return res;
        }

        public void DisposeDataTable()
        {
            this._dt.Dispose();
        }

        public static string GetString(IDataReader reader, string fieldname)
        {
            string strRes = string.Empty;
            strRes = Convert.ToString(reader[fieldname].Equals(DBNull.Value) ? string.Empty : reader[fieldname]);
            return strRes;
        }
        public static int GetInt32(IDataReader reader, string fieldname)
        {
            int iRes = 0;
            iRes = Convert.ToInt32(reader[fieldname].Equals(DBNull.Value) ? 0 : reader[fieldname]);
            return iRes;
        }
        public static DateTime GetDateTime(IDataReader reader, string fieldname)
        {
            string str = string.Empty;
            str = Convert.ToString(reader[fieldname].Equals(DBNull.Value) ? string.Empty : reader[fieldname]);
            if (string.IsNullOrEmpty(str)) return new DateTime();
            string[] group = str.Split(' ');
            string[] date = group[0].Split('-');
            string[] time = group[1].Split(':');
            DateTime dt = new DateTime(int.Parse(date[0]),
                                       int.Parse(date[1]),
                                       int.Parse(date[2]),
                                       int.Parse(time[0]),
                                       int.Parse(time[1]),
                                       int.Parse(time[2])
                                       );
            return dt;
        }

        public void TransData(List<string> lstSql)
        {
            if (!isConnectionOpen()) _SqlConn.Open();
            SqlTransaction trans = _SqlConn.BeginTransaction();
            _SqlCmd.Transaction = trans;
            _SqlCmd.CommandTimeout = 0;  ///20190514 超时设置
            try
            {
                for (int i = 0; i < lstSql.Count; ++i)
                {
                    _SqlCmd.CommandText = lstSql[i];
                    _SqlCmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception err)
            {
                throw new Exception(err.Message + "\r\n" + err.StackTrace);
            }
            finally
            {
                trans.Dispose();
            }
        }

        public List<string> GetDBNameList()
        {
            List<string> lstRes = new List<string>();
            string tmpCmdText = _SqlCmd.CommandText;
            _SqlCmd.CommandText = "SELECT [name]  FROM   master.dbo.sysdatabases order by name";
            if (!isConnectionOpen()) _SqlConn.Open();
            SqlDataReader reader = _SqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    lstRes.Add(reader["name"].ToString());
                }
            }
            _SqlCmd.CommandText = tmpCmdText;
            reader.Close();
            reader.Dispose();
            return lstRes;
        }

        public List<string> GetFieldNameList(string tablename)
        {
            List<string> lstRes = new List<string>();
            string strSql = "SELECT Name FROM SysColumns WHERE id=Object_Id('" + tablename + "') ";
            string tmp = _SqlCmd.CommandText;
            _SqlCmd.CommandText = strSql;
            if (!isConnectionOpen()) _SqlConn.Open();
            SqlDataReader reader = _SqlCmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                    lstRes.Add(reader["Name"].ToString());
            }
            reader.Close();
            reader.Dispose();
            _SqlCmd.CommandText = tmp;
            return lstRes;
        }
    }
