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
    public partial class ucSqliteConnector : ucDataConnector
    {
        public ucSqliteConnector()
        {
            InitializeComponent();
        }
        string _connectionString = string.Empty;

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string sqlitefile = string.Empty;
            OpenFileDialog ofn = new OpenFileDialog();
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                sqlitefile = ofn.FileName;
                txtDbFilePath.Text = ofn.FileName;
                SqliteTool sqlite = new SqliteTool(sqlitefile);
                _connectionString = sqlite.ConnectionString;
                sqlite.Dispose();
            }
        }
        public override string CheckConnection()
        {
            return _connectionString;
        }
    }
}
