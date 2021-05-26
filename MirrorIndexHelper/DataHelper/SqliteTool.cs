using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Text;

public class SqliteTool : DataHelper.IDataTool
{
    private SQLiteCommand _cmd;
    public SQLiteCommand Command
    {
        get { return this._cmd; }
        set { this._cmd = value; }
    }
    private SQLiteConnection _conn;
    public SQLiteConnection Connection
    {
        get { return this._conn; }
        set { this._conn = value; }
    }
    private SQLiteDataReader _reader;
    public SQLiteDataReader Reader
    {
        get { return this._reader; }
        set { this._reader = value; }
    }
    private string _connstr;
    public string ConnectionString
    {
        get { return this._connstr; }
        set { this._connstr = value; }
    }
    private DataTable _dt;
    public DataTable Datatable
    {
        get { return this._dt; }
        set { this._dt = value; }
    }

    public SqliteTool(string filename)
    {
        //Data　Source=
        this._connstr = "Data Source=" + filename + ";Version=3;Pooling=true;FailIfMissing=false";
        this._conn = new SQLiteConnection(this._connstr);
        this._cmd = this._conn.CreateCommand();
    }
    public SqliteTool(string connstr,bool UserDefineConnectionString = true)
    {
        this._connstr = connstr;
        this._conn = new SQLiteConnection(this._connstr);
        this._cmd = this._conn.CreateCommand();
    }
    public void Dispose()
    {
        this._cmd.Dispose();
        if (this._reader != null)
        {
            this._reader.Close();
            this._reader.Dispose();
        }
        this._conn.Close();
        this._conn.Dispose();
    }
    private bool IsConnectionOpen()
    {
        return this._conn.State == ConnectionState.Open;
    }
    public DataTable Query(string sql)
    {
        this._cmd.CommandText = sql;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            SQLiteDataAdapter ada = new SQLiteDataAdapter(this._cmd);
            ada.Fill(_dt);
            ada.Dispose();
        }
        catch (Exception err)
        {
            throw err;
        }
        return _dt;
    }
    public int Update(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            iRes = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return iRes;
    }
    public int Insert(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            iRes = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return iRes;
    }
    public int Delete(string sql)
    {
        this._cmd.CommandText = sql;
        int iRes = -1;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            iRes = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return iRes;
    }
    public object Scalar(string sql)
    {
        this._cmd.CommandText = sql;
        object obj;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            obj = _cmd.ExecuteScalar();
        }
        catch (Exception err)
        {
            throw err;
        }
        return obj;
    }
    public string Scaler(string sql)
    {
        this._cmd.CommandText = sql;
        object obj;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            obj = _cmd.ExecuteScalar();
        }
        catch (Exception err)
        {
            throw err;
        }
        if (null != obj)
            return obj.ToString();
        else
            return string.Empty;
    }
    public IDataReader DoReader(string sql)
    {
        this._cmd.CommandText = sql;
        if (!IsConnectionOpen())
            this._conn.Open();
        try
        {
            _reader = _cmd.ExecuteReader();
        }
        catch (Exception err)
        {
            throw err;
        }
        return _reader;
    }
    public List<string> GetTableList()
    {
        //SELECT name FROM sqlite_master WHERE type = "table"
        List<string> lstRes = new List<string>();
        string tmpsql = _cmd.CommandText;
        _cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = \"table\"";
        if (!IsConnectionOpen()) _conn.Open();
        try
        {
            IDataReader ireader = _cmd.ExecuteReader();
            while (ireader.Read())
                lstRes.Add(Convert.ToString(ireader["name"].Equals(DBNull.Value) ? string.Empty : ireader["name"]));
            ireader.Close();
            ireader.Dispose();
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
    public List<string> GetFieldNameList(string tablename)
    {
        List<string> lstRes = new List<string>();
        string temp = _cmd.CommandText;
        _cmd.CommandText = string.Format("PRAGMA table_info({0})", tablename);
        IDataReader reader = _cmd.ExecuteReader();
        while (reader.Read())
        {
            lstRes.Add(reader["name"].ToString());
        }
        reader.Close();
        reader.Dispose();
        _cmd.CommandText = temp;
        return lstRes;
    }
    /// <summary>
    /// 获取索引列表
    /// </summary>
    /// <returns>表名和对应的多个索引对</returns>
    //public Dictionary<string, List<string>> GetIndexList()
    //{
    //    Dictionary<string, List<string>> dicRes = new Dictionary<string, List<string>>();
    //    List<string> tablenamelist = GetTableNameList();
    //    List<string> lstIndex = new List<string>();
    //    if (!IsConnectionOpen()) _conn.Open();
    //    foreach (string str in tablenamelist)
    //    {
    //        lstIndex.Clear();
    //        _cmd.CommandText = "select name from sqlite_master where type='index' and tbl_name='" + str + "'";
    //        _reader = _cmd.ExecuteReader();
    //        while (_reader.Read())
    //        {
    //            lstIndex.Add( Convert.ToString(_reader["name"].Equals(DBNull.Value) ? string.Empty : _reader["name"]));
    //        }
    //        dicRes.Add(str,lstIndex);
    //    }
    //    return dicRes;
    //}
}
