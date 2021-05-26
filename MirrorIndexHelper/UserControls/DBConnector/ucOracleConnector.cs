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
    public partial class ucOracleConnector : ucDataConnector
    {
        public ucOracleConnector()
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
            _serverAddress = textBox_severname.Text.Trim();
            _userName = textBox_username.Text.Trim();
            _password = textBox_Password.Text.Trim();
            _port = Convert.ToInt32(txtPort.Text.Trim());
            _databaseName = txtDbName.Text.Trim();
            string strRes = string.Empty;
            OracleTool oracle = new OracleTool(_serverAddress, _userName, _password, _databaseName, _port);
            System.Data.IDataReader sreader = oracle.DoReader("select sys_context('userenv','db_name') from dual");
            if (sreader.Read())
                strRes = oracle.ConnectionString;
            sreader.Close();
            sreader.Dispose();
            oracle.Dispose();
            return strRes;
        }
    }
}
