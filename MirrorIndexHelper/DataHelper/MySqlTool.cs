using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
public class MySqlTool : DataHelper.IDataTool
{
    private DataTable _dt = new DataTable();
    public DataTable dataTable
    {
        get { return this._dt; }
        set { this._dt = value; }
    }

    private string _cnstr;
    public string ConnectionString
    {
        get { return _cnstr; }
        set { this._cnstr = value; }
    }

    //private MySqlDataReader _reader;
    //public MySqlDataReader Reader
    //{
    //    get { return this._reader; }
    //    set { this._reader = value; }
    //}

    private MySqlConnection _conn;
    public MySqlConnection Connection
    {
        get { return this._conn; }
        set { this._conn = value; }
    }

    private MySqlCommand _cmd;
    public MySqlCommand Command
    {
        get { return this._cmd; }
        set { this._cmd = value; }
    }

    public MySqlTool(string CnStr)
    {
        this._cnstr = CnStr;
        _conn = new MySqlConnection(_cnstr);
        _cmd = _conn.CreateCommand();
    }

    public MySqlTool(string server, int port, string userid, string password,string database)
    {
        MySqlConnectionStringBuilder csBuilder = new MySqlConnectionStringBuilder();
        if (port < 1) port = 3306;
        csBuilder.Password = password;
        csBuilder.UserID = userid;
        csBuilder.Server = server;
        csBuilder.Port = (uint)port;
        csBuilder.Database = database;
        csBuilder.Pooling = false;
        _cnstr = csBuilder.ConnectionString;
        _conn = new MySqlConnection(_cnstr);
        _cmd = _conn.CreateCommand();
    }

    private bool isConnected()
    {
        return _conn.State == ConnectionState.Open;
    }

    public void Dispose()
    {
        _cmd.Dispose();
        //_reader.Close();
        //_reader.Dispose();
        _conn.Close();
        _conn.Dispose();
    }

    public System.Data.IDataReader DoReader(string sql)
    {
        this._cmd.CommandText = sql;
        MySqlDataReader reader = null;
        try
        {
            if (!isConnected()) _conn.Open();
            reader = _cmd.ExecuteReader();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
        return reader;
    }

    public DataTable Query(string sql)
    {
        this._cmd.CommandText = sql;
        try
        {
            if (!isConnected()) _conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(_cmd);
            da.Fill(this._dt);
            return _dt;
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
    }

    public string Scaler(string sql)
    {
        this._cmd.CommandText = sql;
        object objRes = new object();
        try
        {
            if (!isConnected()) _conn.Open();
            objRes = _cmd.ExecuteScalar();
            return objRes.ToString();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
    }

    public int Update(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        try
        {
            if (!isConnected()) _conn.Open();
            iRes = _cmd.ExecuteNonQuery();
            return iRes;
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
    }

    public int Insert(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        try
        {
            if (!isConnected()) _conn.Open();
            iRes = _cmd.ExecuteNonQuery();
            return iRes;
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
    }

    public int Delete(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        try
        {
            if (!isConnected()) _conn.Open();
            iRes = _cmd.ExecuteNonQuery();
            return iRes;
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
    }

    public bool TransData(List<string> lstSql)
    {
        if(!isConnected())_conn.Open();
        MySqlTransaction trans = _conn.BeginTransaction();
        _cmd.Transaction = trans;
        try
        {
            for (int i = 0; i < lstSql.Count; ++i)
            {
                _cmd.CommandText = lstSql[i];
                _cmd.ExecuteNonQuery();
            }
            trans.Commit();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
            return false;
        }
        finally
        {
            trans.Dispose();
        }
        return true;
    }

    public List<string> GetDBNameList()
    {
        List<string> lstRes = new List<string>();
        try
        {
            if (!isConnected()) _conn.Open();
            string tmpSqlText = _cmd.CommandText;
            _cmd.CommandText = "select distinct TABLE_SCHEMA from information_schema.tables order by TABLE_SCHEMA";
            MySqlDataReader tmpReader = _cmd.ExecuteReader();
            if (tmpReader.HasRows)
            {
                while (tmpReader.Read())
                {
                    string dbname = tmpReader["TABLE_SCHEMA"].ToString();
                    lstRes.Add(dbname);
                }
            }
            _cmd.CommandText = tmpSqlText;
            tmpReader.Close();
            tmpReader.Dispose();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
        return lstRes;
    }

    public List<string> GetTableList()
    {
        List<string> lstRes = new List<string>();
        try
        {
            if (!isConnected()) _conn.Open();
            string tmpSqlText = _cmd.CommandText;
            _cmd.CommandText = "show tables";
            MySqlDataReader tmpReader = _cmd.ExecuteReader();
            if (tmpReader.HasRows)
            {
                while (tmpReader.Read())
                {
                    string dbname = tmpReader[0].ToString();
                    lstRes.Add(dbname);
                }
            }
            _cmd.CommandText = tmpSqlText;
            tmpReader.Close();
            tmpReader.Dispose();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
        return lstRes;
    }
    public List<string> GetViewList()
    {
        return new List<string>();
    }
    public List<string> GetFieldNameList(string tablename)
    {
        List<string> lstRes = new List<string>();
        string temp = _cmd.CommandText;
        _cmd.CommandText = string.Format("select column_name from information_schema.columns where TABLE_NAME='{0}'", tablename);
        IDataReader reader = _cmd.ExecuteReader();
        while (reader.Read())
        {
            lstRes.Add(reader["column_name"].ToString());
        }
        reader.Close();
        reader.Dispose();
        _cmd.CommandText = temp;
        return lstRes;
    }
}

