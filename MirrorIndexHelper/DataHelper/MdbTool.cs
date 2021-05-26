using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ADOX;

/*
 * 1. 添加引用，在“Add reference”对话框里切换到Com页面，选择“Microsoft ADO Ext. 2.8 for DDL and Security”
 * 2.文件的开头using ADOX名字空间
 */
class MdbTool : DataHelper.IDataTool
{
    #region 类的属性成员
    private string _dbname;
    public string DBName
    {
        get { return this._dbname; }
        set 
        { 
            this._dbname = value;
            this._connstr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + value;
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

    /// <summary>
    /// 仅限准备新建数据时使用这个空构造,或使用后对属性DBName或Connstr属性赋值
    /// </summary>
    public MdbTool()
    {

    }
    public MdbTool(string MdbFilename)
    {
        if (!string.IsNullOrEmpty(_password))
        {
            _connstr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + MdbFilename + "; Jet OLEDB:Database Password=" + _password + ";";
            this._olecn = new OleDbConnection(_connstr);
        }
        else
        {
            _connstr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + MdbFilename;
            this._olecn = new OleDbConnection(_connstr);
        }
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
        List<string> lstRes = new List<string>();
        for (int i = 0; i < iCnt; ++i)
            lstRes.Add(dt.Rows[i]["Table_name"].ToString());
        dt.Dispose();
        return lstRes;
    }
    public List<string> GetViewList()
    {
        return new List<string>();
    }
    public List<string> GetFieldNameList(string tablename)
    {
        List<string> lstRes = new List<string>();
        DataTable dt = new DataTable();
        try
        {
            if (!isConnectionOpen()) this._olecn.Open();
            dt = _olecn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tablename, null });
        }
        catch (Exception err)
        {
            throw err;
        }
        int iCnt = dt.Rows.Count;
        int m = dt.Columns.IndexOf("COLUMN_NAME");
        for (int i = 0; i < iCnt; i++)
        {
            DataRow m_DataRow = dt.Rows[i];
            lstRes.Add(m_DataRow.ItemArray.GetValue(m).ToString());
        }
        return lstRes;
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
    /// <summary>
    /// 创建数据库,调用成功后会建立与新库的连接
    /// </summary>
    /// <param name="dbfullname">保存数据库的全路径</param>
    /// <returns>创建成功返回true,否则返回false</returns>
    public bool CreateDatabase(string dbfullname)
    {
        try
        {
            #region 创建库
            ADOX.Catalog cat = new Catalog();
            cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbfullname + ";Jet OLEDB:Engine Type=5");
            cat = null;
            #endregion
            this.DBName = dbfullname;
        }
        catch(Exception err)
        {
            throw err;
        }
        return true;
    }
}
#region
/*

 Access各类型连接字符串汇总
 
标准链接：


"Driver= {MicrosoftAccessDriver(*.mdb)};DBQ=C:\App1\你的数据库名.mdb;Uid=你的用户名;Pwd=你的密码;" 


如果ACCESS数据库未设置用户名和密码，请留空。下同。

WorkGroup方式（工作组方式）连接：


"Driver={Microsoft Access Driver (*.mdb)}; Dbq=C:\App1\你的数据库名.mdb; SystemDB=C:\App1\你的数据库名.mdw;"  


采用独占方式进行连接：


"Driver={Microsoft Access Driver (*.mdb)}; DBQ=C:\App1\你的数据库名.mdb; Exclusive=1; Uid=你的用户名; Pwd=你的密码;"  


MS ACCESS OLEDB & OleDbConnection （.NET下的OleDb接口）进行链接

普通方式（最常用）连接ACCESS数据库：


"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\App1\你的数据库名.mdb; User Id=admin; Password="  


使用工作组方式（系统数据库）连接ACCESS数据库：


"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\App1\你的数据库名.mdb; Jet OLEDB:System Database=c:\App1\你的系统数据库名.mdw"  


连接到带有密码的ACCESS数据库：


"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\App1\你的数据库名.mdb; Jet OLEDB:Database Password=你的密码"  


连接到处于局域网主机上的ACCESS数据库：


"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=\\Server_Name\Share_Name\Share_Path\你的数据库名.mdb"  


连接到处于远程服务器上的ACCESS数据库：


"Provider=MS Remote; Remote Server=http://远程服务器IP; Remote Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\App1\你的数据库名.mdb" 
*/
#endregion