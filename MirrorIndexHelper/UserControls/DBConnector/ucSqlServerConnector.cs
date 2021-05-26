using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LuceneHelper.UserControls
{
    public partial class ucSqlServerConnector : ucDataConnector
    {
        public ucSqlServerConnector()
        {
            InitializeComponent();
        }

        string _userName = string.Empty;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        string _serverAddress = string.Empty;

        public string ServerAddress
        {
            get { return _serverAddress; }
            set { _serverAddress = value; }
        }
        string _databaseName = string.Empty;

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        string _connectionString = string.Empty;
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        private void comboBox_databasename_SelectedIndexChanged(object sender, EventArgs e)
        {
            _databaseName = comboBox_databasename.Text.Trim();
            //_databaseName = comboBox_databasename.Text = "NstlMilitaryMirror";   //20180312 添加默认值
        }
        public override string CheckConnection()
        {
            string strRes = string.Empty;
            _userName = textBox_username.Text.Trim();
            _password = textBox_Password.Text.Trim();
            _serverAddress = textBox_severname.Text.Trim();
            if (!string.IsNullOrEmpty(_userName))
            {
                if (!string.IsNullOrEmpty(_password))
                {
                    if (!string.IsNullOrEmpty(_serverAddress))
                    {
                        //SqlTool mssql = new SqlTool(_serverAddress, comboBox_databasename.Text.Trim(), _userName, _password);
                        SqlTool mssql = new SqlTool(_serverAddress, "NstlMilitaryMirror", _userName, _password);             //20180312 添加默认值
                        strRes = mssql.ConnectionString;
                    }
                }
            }
            return strRes;
        }
        private void comboBox_databasename_Click(object sender, EventArgs e)
        {
            _userName = textBox_username.Text.Trim();
            _password = textBox_Password.Text.Trim();
            _serverAddress = textBox_severname.Text.Trim();
            if (!string.IsNullOrEmpty(_userName))
            {
                if (!string.IsNullOrEmpty(_password))
                {
                    if (!string.IsNullOrEmpty(_serverAddress))
                    {
                        SqlTool mssql = new SqlTool(_serverAddress, "Master", _userName, _password);
                        try
                        {
                            List<string> lstDBName = mssql.GetDBNameList();
                            if (lstDBName.Count > 0)
                            {
                                lstDBName.Sort();
                                comboBox_databasename.Items.Clear();
                                foreach (string dbName in lstDBName)
                                    comboBox_databasename.Items.Add(dbName);
                                //_connectionString = mssql.ConnectionString;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("连接数据服务器的信息有错误,请检查");
                        }
                        finally
                        {
                            mssql.Dispose();
                        }
                    }
                }
            }
        }
    }
}
