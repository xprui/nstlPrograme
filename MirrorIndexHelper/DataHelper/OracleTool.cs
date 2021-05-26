using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
//select sys_context('userenv','db_name') from dual -->��Ȩ��,��ѯ�뵱ǰ��½�û���صĿ���
public class OracleTool : DataHelper.IDataTool
{
    private string _connstr;
    public string ConnectionString
    {
        get { return this._connstr; }
        set { this._connstr = value; }
    }
    private OracleDataReader _reader;
    public OracleDataReader Reader
    {
        set { this._reader = value; }
        get { return this._reader; }
    }
    private OracleCommand _cmd;
    public OracleCommand Command
    {
        set { this._cmd = value; }
        get { return this._cmd; }
    }

    private OracleConnection _conn;
    public OracleConnection Connection
    {
        set { this._conn = value; }
        get { return this._conn; }
    }

    private DataTable _dt;
    public DataTable Datatable
    {
        get { return this._dt; }
        set { this._dt = value; }
    }

    public void ReleaseDatatable()
    {
        this._dt.Clear();
        this._dt.Dispose();
    }

    /// <summary>
    /// �޲ι��캯��,���������е����Ӵ�
    /// </summary>
    public OracleTool()
    {
        this._connstr = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"].ToString();
        this._conn = new OracleConnection(this._connstr);
        this._cmd = this._conn.CreateCommand();
    }
    
    /// <summary>
    /// ָ�����Ӵ��Ĺ���
    /// </summary>
    /// <param name="OracleConnectionString">���Ӵ�</param>
    public OracleTool(string OracleConnectionString)
    {
        this._connstr = OracleConnectionString;
        this._conn = new OracleConnection(this._connstr);
        this._cmd = this._conn.CreateCommand();
    }

    /// <summary>
    /// ͨ����������������Ϣ�Ĺ���
    /// </summary>
    /// <param name="DataSource">��������ָ�ʽ,һ�������ÿͻ��˺��SID_IP,һ���Ƿ�������ORACLEʵ���ķ�����</param>
    /// <param name="Username">�����û�</param>
    /// <param name="Password">��������</param>
    public OracleTool(string DataSource, string Username, string Password,string DBName,int port)
    {
        //Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=serverIP)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=MELINETS)));User Id=user;Password=pass
        this._connstr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", DataSource, port, DBName, Username, Password);
        _conn = new OracleConnection(this._connstr);
        //OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
        //ocsb.Password = Password;
        //ocsb.UserID = Username;
        ////ocsb.IntegratedSecurity = true;
        ////ocsb.PersistSecurityInfo = true;
        //ocsb.DataSource = DataSource;
        //this._connstr = ocsb.ConnectionString;
        this._conn = new OracleConnection(this._connstr);
        this._cmd = this._conn.CreateCommand();
    }

    /// <summary>
    /// �鿴�����Ƿ��ڴ�״̬
    /// </summary>
    /// <returns>���Ӵ��ڴ�״̬ʱ����true,δ�򿪷���false</returns>
    private bool IsConnectionOpen()
    {
        return this._conn.State == ConnectionState.Open;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <returns>�ɹ�����true,���򷵻�false</returns>
    public bool TestConnection()
    {
        try
        {
            this._conn.Open();
        }
        catch (Exception err)
        {
            throw err;
            return false;
        }
        return true;
    }

    public DataTable Query(string sql)
    {
        this._cmd.CommandText = sql;
        try
        {
            if (!IsConnectionOpen())
                this._conn.Open();
            OracleDataAdapter orcAda = new OracleDataAdapter(_cmd);
            orcAda.Fill(_dt);
            orcAda.Dispose();
        }
        catch (Exception err)
        {
            throw new Exception(err.Message + "\r\n" + err.StackTrace);
        }
        return _dt;
    }

    public object Scalar(string sql)
    {
        _cmd.CommandText = sql;
        object obj;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
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
        _cmd.CommandText = sql;
        object obj;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
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

    public int Update(string sql)
    {
        int ires = -1;
        _cmd.CommandText = sql;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
            ires = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return ires;
    }

    public int Insert(string sql)
    {
        int ires = -1;
        _cmd.CommandText = sql;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
            ires = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return ires;
    }

    public int Delete(string sql)
    {
        int ires = -1;
        _cmd.CommandText = sql;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
            ires = _cmd.ExecuteNonQuery();
        }
        catch (Exception err)
        {
            throw err;
        }
        return ires;
    }

    public System.Data.IDataReader DoReader(string sql)
    {
        _cmd.CommandText = sql;
        try
        {
            if (!IsConnectionOpen())
                _conn.Open();
            this._reader = _cmd.ExecuteReader();
        }
        catch (Exception err)
        {
            throw err;
        }
        return this._reader;
    }

    public List<string> GetTableList()
    {
        //TODO ��������������ر����������û�Ȩ������
        List<string> lstRes = new List<string>();
        string tmp = _cmd.CommandText;
        if (!IsConnectionOpen())
            _conn.Open();
        _cmd.CommandText = "SELECT * FROM tab WHERE tabtype = 'TABLE'";//��tab��ͼ�в�ѯ��ǰ�û���'��'
        Oracle.DataAccess.Client.OracleDataReader reader = _cmd.ExecuteReader();
        while (reader.Read())
        {
            lstRes.Add(reader[0].ToString());
        }
        reader.Close();
        reader.Dispose();
        _cmd.CommandText = tmp;
        return lstRes;
    }
    public List<string> GetViewList()
    {
        return new List<string>();
    }
    public List<string> GetFieldNameList(string tablename)
    {
        List<string> lstRes = new List<string>();
        string TEMP = _cmd.CommandText;
        _cmd.CommandText = string.Format("SELECT COLUMN_NAME FROM DBA_TAB_COLUMNS WHERE TABLE_NAME = '{0}'", tablename.ToUpper());
        IDataReader reader = _cmd.ExecuteReader();
        while (reader.Read())
        {
            lstRes.Add(reader["COLUMN_NAME"].ToString());
        }
        reader.Close();
        reader.Dispose();
        return lstRes;
    }
    //public void TransData(List<string> sqlInsert)//����д��
    //{
    //    if (!IsConnectionOpen()) this._conn.Open();
    //    OracleTransaction trans = _conn.BeginTransaction();
    //    _cmd.Transaction = trans;
    //    string[] str = new string[2];
    //    int i;
    //    try
    //    {
    //        //for (i = 0; i < sqlInsert.Count; ++i)
    //        //{
    //        //    //str = sqlInsert[i].ToString().Split("|".ToCharArray());
    //        //    _olecmd.CommandText = sqlInsert[i];
    //        //    _olecmd.ExecuteNonQuery();
    //        //}
    //        //trans.Commit();
    //        int i = 0;
    //        while(i<sqlInsert.Count)
    //        {
    //            for (int j = 0; j < 100; ++j)
    //            {
    //                _olecmd.CommandText = sqlInsert[i];
    //                _olecmd.ExecuteNonQuery();
    //                i++;
    //            }
    //            trans.Commit();
    //        }
    //    }
    //    catch (Exception err)
    //    {
    //        throw err;
    //    }
    //    finally
    //    {
    //        trans.Dispose();
    //    }
    //}

    /// <summary>
    /// ��������ռ�õ���Դ
    /// </summary>
    public void Dispose()
    {
        this._cmd.Dispose();
        this._conn.Close();
        this._conn.Dispose();
    }
}
