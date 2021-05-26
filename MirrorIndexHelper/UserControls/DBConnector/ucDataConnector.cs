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
    public partial class ucDataConnector : UserControl
    {
        public ucDataConnector()
        {
            InitializeComponent();
        }
        public virtual string CheckConnection()
        {
            return string.Empty;
        }
    }
}
