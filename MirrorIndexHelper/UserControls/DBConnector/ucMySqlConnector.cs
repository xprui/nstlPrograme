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
    public partial class ucMySqlConnector : ucDataConnector
    {
        public ucMySqlConnector()
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
        int _port = 0;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
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

        public override string CheckConnection()
        {
            string strRes = string.Empty;
            _userName = textBox_username.Text.Trim();
            _password = textBox_Password.Text.Trim();
            _serverAddress = textBox_severname.Text.Trim();
            _port = Convert.ToInt32(txtPort.Text.Trim());
            if (!string.IsNullOrEmpty(_userName))
            {
                if (!string.IsNullOrEmpty(_password))
                {
                    if (!string.IsNullOrEmpty(_serverAddress))
                    {
                        MySqlTool mysql = new MySqlTool(_serverAddress, _port, _userName, _password, comboBox_databasename.Text.Trim());
                        List<string> lst = mysql.GetDBNameList();
                        if (lst.Count > 0)
                            strRes = mysql.ConnectionString;
                        mysql.Dispose();
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
            _port = Convert.ToInt32(txtPort.Text.Trim());
            if (!string.IsNullOrEmpty(_userName))
            {
                if (!string.IsNullOrEmpty(_password))
                {
                    if (!string.IsNullOrEmpty(_serverAddress))
                    {
                        MySql.Data.MySqlClient.MySqlConnectionStringBuilder msb = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
                        msb.Password = _password;
                        msb.UserID = _userName;
                        msb.Server = _serverAddress;
                        //msb.Database = "mysql";
                        msb.Port = (uint)_port;
                        MySqlTool mysql = new MySqlTool(msb.ConnectionString);
                        List<string> lstDBName = mysql.GetDBNameList();
                        comboBox_databasename.Items.Clear();
                        foreach (string dbName in lstDBName)
                            comboBox_databasename.Items.Add(dbName);
                        //_connectionString = mssql.ConnectionString;
                    }
                }
            }
        }
    }
}
