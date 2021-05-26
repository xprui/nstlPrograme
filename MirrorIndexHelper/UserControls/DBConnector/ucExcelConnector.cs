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
    public partial class ucExcelConnector : ucDataConnector
    {
        public ucExcelConnector()
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
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "*.xls|*.xls";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                string excelfile = ofn.FileName;
                txtDbFilePath.Text = excelfile;
                ExcelTool msExcel = new ExcelTool(excelfile);
                
                _connectionString = msExcel.ConnectionString;
                msExcel.Dispose();
            }
        }

        public override string CheckConnection()
        {
            return _connectionString;
        }
    }
}
