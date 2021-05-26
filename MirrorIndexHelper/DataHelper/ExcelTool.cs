using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OleDb;
using System.Data;
using System.Collections;

    public class ExcelTool : DataHelper.IDataTool
    {
        #region 类的属性成员
        private static bool _headerIsFieldName = false;
        /// <summary>
        /// 第一行是否为列标题
        /// </summary>
        public static bool HeaderIsFieldName
        {
            get { return _headerIsFieldName; }
            set { _headerIsFieldName = value; }
        }
        private string _dbname;
        public string DBName
        {
            get { return this._dbname; }
            set
            {
                this._dbname = value;
                this._connstr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + value + ";extended properties=\"Excel 8.0;HDR=" + (_headerIsFieldName ? "YES" : "NO") + ";\"";
                this._olecn = new OleDbConnection(this._connstr);
                this._olecmd = this._olecn.CreateCommand();
            }
        }
        private string _connstr;
        public string ConnectionString
        {
            get { return _connstr; }
            set
            {
                _connstr = value;
                this._olecn = new OleDbConnection(value);
                this._olecmd = this._olecn.CreateCommand();
            }
        }
        string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        private OleDbConnection _olecn;
        public OleDbConnection OleCn
        {
            get { return _olecn; }
            set { _olecn = value; }
        }
        private OleDbCommand _olecmd;
        public OleDbCommand OleCmd
        {
            set { _olecmd = value; }
            get { return _olecmd; }
        }
        private DataTable _dt = new DataTable();
        public DataTable dataTable
        {
            get { return this._dt; }
            set { this._dt = value; }
        }
        #endregion
        public ExcelTool(string MdbFilename)
        {
            _connstr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + MdbFilename + "; extended properties=\"Excel 8.0;HDR=" + (_headerIsFieldName ? "YES" : "NO") + ";\"";
            this._olecn = new OleDbConnection(_connstr);
            OleDbConnectionStringBuilder oleCSB = new OleDbConnectionStringBuilder();
            this._olecmd = this._olecn.CreateCommand();
        }
        private bool isConnectionOpen()
        {
            return this._olecn.State == ConnectionState.Open;
        }
        public void Dispose()
        {
            this._olecmd.Dispose();
            this._olecn.Close();
            this._olecn.Dispose();
        }
        public int Update(string sql)
        {
            this._olecmd.CommandText = sql;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                return this._olecmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public string Scaler(string sql)
        {
            this._olecmd.CommandText = sql;
            string strRes = string.Empty;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                strRes = this._olecmd.ExecuteScalar().ToString();
            }
            catch(Exception err)
            {
                throw err;
            }
            return strRes;
        }
        public int Delete(string sql)
        {
            this._olecmd.CommandText = sql;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                return this._olecmd.ExecuteNonQuery();
            }
            catch(Exception err)
            {
                throw err;
            }
        }
        public int Insert(string sql)
        {
            this._olecmd.CommandText = sql;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                return this._olecmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public System.Data.IDataReader DoReader(string sql)
        {
            this._olecmd.CommandText = sql;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                OleDbDataReader reader = _olecmd.ExecuteReader();
                return reader;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <param name="sqlInsert">写入数据的SQL语句列表</param>
        public void TransData(List<string> sqlInsert)//批量写入
        {
            if (!isConnectionOpen()) this._olecn.Open();
            OleDbTransaction trans = _olecn.BeginTransaction();
            _olecmd.Transaction = trans;
            string[] str = new string[2];
            int i;
            try
            {
                for (i = 0; i < sqlInsert.Count; ++i)
                {
                    //str = sqlInsert[i].ToString().Split("|".ToCharArray());
                    _olecmd.CommandText = sqlInsert[i];
                    _olecmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                trans.Dispose();
            }
        }
        /// <summary>
        /// 执行Select查询,返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Query(string sql)
        {
            this._olecmd.CommandText = sql;
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                OleDbDataAdapter oleada = new OleDbDataAdapter(_olecmd);
                oleada.Fill(this._dt);
                oleada.Dispose();
            }
            catch (Exception err)
            {
                throw err;
            }
            return this._dt;
        }
        public string[] GetTableColumn(string TableName)
        {
            DataTable dt = new DataTable();
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                dt = _olecn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, TableName, null });
            }
            catch (Exception err)
            {
                throw err;
            }
            int iCnt = dt.Rows.Count;
            string[] strTable = new string[iCnt];
            int m = dt.Columns.IndexOf("COLUMN_NAME");
            for (int i = 0; i < iCnt; i++)
            {
                DataRow m_DataRow = dt.Rows[i];
                strTable[i] = m_DataRow.ItemArray.GetValue(m).ToString();
            }
            return strTable;
        }

        /// <summary>
        /// 以字串数组形式返回给定MDB文件中的表名
        /// </summary>
        /// <returns></returns>
        public string[] GetTables()
        {
            DataTable dt = new DataTable();
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                dt = _olecn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            }
            catch (Exception e)
            {
                throw e;
            }
            int iCnt = dt.Rows.Count;
            String[] strTableName = new string[iCnt];
            for (int i = 0; i < iCnt; ++i)
                strTableName[i] = dt.Rows[i]["Table_name"].ToString();
            dt.Dispose();
            return strTableName;
        }

        public List<string> GetTableList()
        {
            List<string> lstRes = new List<string>();
            try
            {
                if (!isConnectionOpen()) this._olecn.Open();
                System.Data.DataTable dt = _olecn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    lstRes.Add(dt.Rows[i][2].ToString().Trim() + "\r\n");
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return lstRes;
        }
    public List<string> GetViewList()
    {
        return new List<string>();
    }
        /// <summary>
        /// 判断已经连接的数据库中否存在在表tablename
        /// </summary>
        /// <param name="tablename">需要判断的表名</param>
        /// <returns>存在表tablename时返回true,否则返回false</returns>
    public bool isTableExists(string tablename)
        {
            string[] tablelist = GetTables();
            ArrayList al = new ArrayList(tablelist);
            return al.Contains(tablename);
        }

        public List<string> GetFieldNameList(string tablename)
        {
            List<string> lstRes = new List<string>();
            string tmp = _olecmd.CommandText;
            _olecmd.CommandText = "select top 1 * from [" + tablename.Replace("$", string.Empty) + "$]";
            IDataReader reader = _olecmd.ExecuteReader();
            for (int i = 0; i < reader.FieldCount; ++i)
                lstRes.Add(reader.GetName(i));
            reader.Close();
            reader.Dispose();
            return lstRes;
        }
        

    }

