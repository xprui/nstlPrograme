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
    public partial class ucAccessConnector : ucDataConnector
    {
        public ucAccessConnector()
        {
            InitializeComponent();
        }

        string _mdbFullname = string.Empty;

        public string MdbFullname
        {
            get { return _mdbFullname; }
            set { _mdbFullname = value; }
        }
        string _username = string.Empty;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public override string CheckConnection()
        {
            string strRes = string.Empty;
            MdbTool access = null;
            if (!string.IsNullOrEmpty(_password))
                access.Password = _password;
            access = new MdbTool(_mdbFullname);
            access.Dispose();
            strRes = access.ConnectionString;
            return strRes;
        }
        private void btnBrowseAccess_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "*.mdb|*.mdb";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                _mdbFullname = ofn.FileName;
                txtAccessFullname.Text = _mdbFullname;
            }
        }

        private void textBox_username_TextChanged(object sender, EventArgs e)
        {
            _username = textBox_username.Text.Trim();
            _password = textBox_Password.Text.Trim();
        }
    }
}
