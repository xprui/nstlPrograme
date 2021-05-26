using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using MirrorIndexHelper;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.KTDictSeg;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using MirrorIndexHelper.Xml2DB;
using System.IO;
namespace LuceneHelper
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        /// <summary>
        /// 数据源连接器
        /// </summary>
        UserControls.ucDataConnector dataConnectorUI = new UserControls.ucDataConnector();
        /// <summary>
        /// 数据源操作类实体
        /// </summary>
        DataHelper.IDataTool dataTool = null;
        /// <summary>
        /// 建立索引用的分词器
        /// </summary>
        Lucene.Net.Analysis.Analyzer analyzerCreate = null;
        Lucene.Net.Analysis.Analyzer analyzerSet = null;         //20180312    增加自动判断分词
        /// <summary>
        /// 索引保存路径
        /// </summary>
        string strIndexSavePath = string.Empty;
        bool blnTest = false;//测试索引标识
        int iTestRcdCnt = 0;//用以测试的记录条数
        string sql = string.Empty;
        private string _connstring = string.Empty;//数据库连接串
        private const int BUFFERSIZE = 100;//写索引/目的数据库的缓存记录数大小
        bool blnGroup = false;
        private bool stopflag;//停止标识
        bool blnImportXML = false;
        Dictionary<string, string> dicUnitCode2Name = new Dictionary<string, string>();


        private void cmbDataSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string datasource = cmbDataSourceType.Text.Trim().ToLower();
            try
            {
                if (gpConfigDataSource.Controls.Count > 0)
                    gpConfigDataSource.Controls.RemoveAt(0);
            }
            catch (Exception err)
            {
                throw err;
            }
            if (datasource == "sqlserver")
            {
                dataConnectorUI = new UserControls.ucSqlServerConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
            else if (datasource == "access")
            {
                dataConnectorUI = new UserControls.ucAccessConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
            else if (datasource == "excel")
            {
                dataConnectorUI = new UserControls.ucExcelConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
            else if (datasource == "mysql")
            {
                dataConnectorUI = new UserControls.ucMySqlConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
            else if (datasource == "oracle")
            {
                dataConnectorUI = new UserControls.ucOracleConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
            else if (datasource == "sqlite")
            {
                dataConnectorUI = new UserControls.ucSqliteConnector();
                dataConnectorUI.Dock = DockStyle.Fill;
                this.gpConfigDataSource.Controls.Add(dataConnectorUI);
            }
        }

        private void btnConfirmDataSource_Click(object sender, EventArgs e)
        {
            if (this.dataConnectorUI is UserControls.ucAccessConnector)
            {
                dataConnectorUI = (UserControls.ucAccessConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new MdbTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucExcelConnector)
            {
                dataConnectorUI = (UserControls.ucExcelConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new ExcelTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucMySqlConnector)
            {
                dataConnectorUI = (UserControls.ucMySqlConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new MySqlTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucOracleConnector)
            {
                dataConnectorUI = (UserControls.ucOracleConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new OracleTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucSqliteConnector)
            {
                dataConnectorUI = (UserControls.ucSqliteConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new SqliteTool(_connstring, true);
            }
            else if (this.dataConnectorUI is UserControls.ucSqlServerConnector)
            {
                dataConnectorUI = (UserControls.ucSqlServerConnector)dataConnectorUI;
                _connstring = dataConnectorUI.CheckConnection();
                dataTool = new SqlTool(_connstring);
            }
            if (string.IsNullOrEmpty(_connstring))
            {
                MessageBox.Show("请先配置好可用的数据源");
                return;
            }

            List<string> lstTables = dataTool.GetTableList();
            lstTables.AddRange(dataTool.GetViewList());
            comboBox_tablename.Items.Clear();
            foreach (string table in lstTables)
                comboBox_tablename.Items.Add(table);
            tbCreate.SelectedIndex = 1;

            string dpath = System.Configuration.ConfigurationManager.AppSettings["DefaultIndexRootPath"];
            if (!string.IsNullOrEmpty(dpath) && System.IO.Directory.Exists(dpath))
                textBox_savepath.Text = dpath;

            string where = System.Configuration.ConfigurationManager.AppSettings["LastWhereLogic"];
            if (!string.IsNullOrEmpty(where))
                txtWhere.Text = where;
        }

        private void comboBox_tablename_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tablename = comboBox_tablename.Text.Trim();
            AnalyzerChEn ace = new AnalyzerChEn();                        //20180312 增加自动选择分词方法
            analyzerSet = ace.getAnalyzerChoose(tablename);               //20180312 增加自动选择分词方法
            String analyzerStr = ace.getAnalyzerValue(tablename);          //20180312 增加自动选择分词方法
            if (analyzerStr.Equals("ch")) {                                //20180312 增加自动选择分词方法,自动显示在下拉菜单中，当前选择的分词方法
                comboBox_analyzer.SelectedIndex = 0;                       //20180312 增加自动选择分词方法
            }                                                              //20180312 增加自动选择分词方法
            else { comboBox_analyzer.SelectedIndex = 1; }                  //20180312 增加自动选择分词方法


            List<string> lstFields = dataTool.GetFieldNameList(tablename);
            this.dataGridView1.Rows.Clear();
            Application.DoEvents();
            dataGridView1.Rows.Add(lstFields.Count);
            int i = 0;
            foreach (string field in lstFields)
            {
                dataGridView1.Rows[i].Cells[0].Value = true;
                dataGridView1.Rows[i].Cells[1].Value = field;
                dataGridView1.Rows[i].Cells[2].Value = field;
                dataGridView1.Rows[i].Cells[3].Value = "TOKENIZED";
                dataGridView1.Rows[i].Cells[4].Value = "YES";
                i++;
            }
        }

        private void button_browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(textBox_savepath.Text) && System.IO.Directory.Exists(textBox_savepath.Text))
                fbd.SelectedPath = textBox_savepath.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                strIndexSavePath = fbd.SelectedPath;
                textBox_savepath.Text = fbd.SelectedPath;
            }
        }

        private void comboBox_analyzer_TextChanged(object sender, EventArgs e)
        {
            #region 建立分词器
            switch (comboBox_analyzer.Text)
            {//使用选择的分词器
                case "KTDictSegAnalyzer(CN)":
                    analyzerCreate = new KTDictSegAnalyzer();
                    break;
                case "StandardAnalyzer":
                        analyzerCreate = new StandardAnalyzer();
                        break;
                case "KeywordAnalyzer":
                    analyzerCreate = new KeywordAnalyzer();
                    break;
                case "SimpleAnalyzer":
                    analyzerCreate = new SimpleAnalyzer();
                    break;
                case "StopAnalyzer":
                        analyzerCreate = new StopAnalyzer();
                        break;
                case "WhitespaceAnalyzer":
                    analyzerCreate = new WhitespaceAnalyzer();
                    break;
                case "NSTLAnalyzer(EN)":
                        break;
                default:
                    analyzerCreate = new KTDictSegAnalyzer();
                    break;
            }
            #endregion
        }

        private void chkTestIndex_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTestIndex.Checked)
            {
                txtRcdCnt.Enabled = true;
                blnTest = true;
            }
            else
            {
                txtRcdCnt.Enabled = false;
                blnTest = false;
            }
        }

        private void btnAddNewLine_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();
        }

        private void btnExportConfig_Click(object sender, EventArgs e)
        {
            string fn = string.Empty;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.txt|*.txt";
            bool blnExportOK = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                fn = sfd.FileName;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fn, false, Encoding.Default);
                int iRowsCount = dataGridView1.Rows.Count;
                string[,] sValue = new string[iRowsCount, 5];
                try
                {
                    for (int i = 0; i < iRowsCount; ++i)
                    {
                        for (int j = 0; j < 5; ++j)
                            sValue[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        sw.WriteLine(sValue[i, 0] + "^" + sValue[i, 1] + "^" + sValue[i, 2] + "^" + sValue[i, 3] + "^" + sValue[i, 4]);
                    }
                }
                catch (Exception err)
                {
                    blnExportOK = false;
                    MessageBox.Show(err.Message + "\r\n导出失败!");
                }
                finally
                {
                    sw.Close(); sw.Dispose();
                }
                if (blnExportOK)
                    MessageBox.Show("导出成功!");
            }
        }

        private void btnImportConfig_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "*.txt|*.txt";
            if (ofn.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                string fn = ofn.FileName;
                System.IO.StreamReader sr = new System.IO.StreamReader(fn, Encoding.Default);
                int i = 0;
                bool blnImportOK = true;
                try
                {
                    do
                    {
                        if (i >= dataGridView1.Rows.Count)
                            dataGridView1.Rows.Add();
                        string sFields = sr.ReadLine();
                        string[] sField = sFields.Split('^');
                        dataGridView1.Rows[i].Cells[0].Value = sField[0].ToLower() == "false" ? false : true;
                        dataGridView1.Rows[i].Cells[1].Value = sField[1];
                        dataGridView1.Rows[i].Cells[2].Value = sField[2];
                        dataGridView1.Rows[i].Cells[3].Value = sField[3];
                        dataGridView1.Rows[i].Cells[4].Value = sField[4];
                        i++;
                    } while (!sr.EndOfStream); //while (i < dataGridView1.Rows.Count && !sr.EndOfStream) ;
                }
                catch (Exception err)
                {
                    blnImportOK = false;
                    MessageBox.Show(err.Message + "\r\n导入失败!");
                }
                finally
                {
                    sr.Close(); sr.Dispose();
                }
                if (blnImportOK) MessageBox.Show("导入成功!");
            }
        }

        private void btnCreateIndex_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.Now;

            #region 导出数据到lucene
            int toluceneprogress = 0;
            //int tolucenecount = databasemanager.Getrowscount(Connectionstring, comboBox_tablename.Text);

            progressBar1.Minimum = 0;
            //progressBar1.Maximum = tolucenecount;

            #region 取得配置信息
            List<Fieldattribute> afieldlist = new List<Fieldattribute>();//索引字段列表
            for (int i = 0; i < dataGridView1.Rows.Count; i++)//循环处理每一个字段
            {
                if ((bool)dataGridView1.Rows[i].Cells[0].Value)//对选中的字段赋值
                {
                    Fieldattribute afield = new Fieldattribute();
                    afield.Orgfieldname = dataGridView1.Rows[i].Cells[1].Value.ToString();//源
                    afield.Tagertfieldname = dataGridView1.Rows[i].Cells[2].Value.ToString();//目标
                    afield.Indextype = dataGridView1.Rows[i].Cells[3].Value.ToString();//索引方式
                    afield.Storetype = dataGridView1.Rows[i].Cells[4].Value.ToString();//存储
                    afieldlist.Add(afield);//添加到索引字段列表中
                }
            }
            #endregion

            #region 导出到lucene
            sql = "select " + Exportconfigmanager.Getsqlcmdstr(afieldlist) + " from " + comboBox_tablename.Text + " where 1=1 ";//读取要索引的SQL表记录
            string where = txtWhere.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(txtWhere.Text.Trim()))
            {
                if (where.IndexOf("delete") > 0 || where.IndexOf("update") > 0 || where.IndexOf("insert")>0
                    || where.IndexOf("drop") > 0 || where.IndexOf("truncate") > 0)
                {
                    MessageBox.Show("附件条件中包含影响源数据的操作","你想干啥？");
                    return;
                }
                sql += " and " + txtWhere.Text;
            }
            if (blnTest)
            {
                iTestRcdCnt = int.Parse(txtRcdCnt.Text.Trim());
                if (!sql.StartsWith("select top"))
                    sql = sql.ToLower().Replace("select", "select top " + iTestRcdCnt.ToString());
                progressBar1.Maximum = iTestRcdCnt;
            }
            else
            {
                var count = "select count(0) from " + comboBox_tablename.Text + " where " + where;
                progressBar1.Maximum = Convert.ToInt32(dataTool.Scaler(count));
            }
            
            
            if (this.dataConnectorUI is UserControls.ucAccessConnector)
            {
                dataTool = new MdbTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucExcelConnector)
            {
                dataTool = new ExcelTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucMySqlConnector)
            {
                dataTool = new MySqlTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucOracleConnector)
            {
                dataTool = new OracleTool(_connstring);
            }
            else if (this.dataConnectorUI is UserControls.ucSqliteConnector)
            {
                dataTool = new SqliteTool(_connstring, true);
            }
            else if (this.dataConnectorUI is UserControls.ucSqlServerConnector)
            {
                dataTool = new SqlTool(_connstring);
            }
            System.Data.IDataReader MyDataReader = dataTool.DoReader(sql);
            //Lucene.Net.Store.FSDirectory luceneDirectory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(textBox_savepath.Text));
            //IndexWriter writer = new IndexWriter(luceneDirectory, analyzerCreate, true,IndexWriter.MaxFieldLength.LIMITED);
            if (!System.IO.Directory.Exists(textBox_savepath.Text))
                System.IO.Directory.CreateDirectory(textBox_savepath.Text);

            System.Configuration.Configuration cfa = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["DefaultIndexRootPath"].Value = textBox_savepath.Text;
            cfa.Save();

            if(!string.IsNullOrEmpty(txtWhere.Text))
            {
                cfa.AppSettings.Settings["LastWhereLogic"].Value = txtWhere.Text;
            }

            //IndexWriter writer = new IndexWriter(textBox_savepath.Text, analyzerCreate, !checkBox_indextype.Checked);//用选中的分词器,存储在savepath下,选用indextype的方式(增量与否)     
            IndexWriter writer = new IndexWriter(textBox_savepath.Text, analyzerSet, !checkBox_indextype.Checked);//用选中的分词器,存储在savepath下,选用indextype的方式(增量与否)     analyzerSet     //20180312   新增自动选择分词器

            writer.SetMaxBufferedDocs(BUFFERSIZE);//
                                                  //writer.SetMergeFactor(10000);
                                                  //writer.SetMaxMergeDocs(10000);
            writer.SetMergeFactor(1000); //数据量大时 出内存溢出 20210225
            writer.SetMaxMergeDocs(1000);//数据量大时 出内存溢出 20210225
            while (MyDataReader.Read())//循环处理SQL的每一个字段
            {
                //stopflag = true;
                try
                {
                    if (stopflag)//中途停止时
                    {
                        writer.Optimize();
                        writer.Close();
                        break;
                    }
                    toluceneprogress++;//计数器
                    lblTip.Text = toluceneprogress.ToString() + " / " + progressBar1.Maximum;
                    progressBar1.Value = toluceneprogress;
                    ExportTolucene.AddDocument(writer, MyDataReader, afieldlist, blnGroup);//自定义静态方法,用于添加一条记录的索引到IndexWriter中
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    return;
                }
                if (toluceneprogress % 300 == 0)//每100条记录更新一次进度条
                {
                    Application.DoEvents();
                    this.Update();
                }
            }
            if (!stopflag)
            {
                MyDataReader.Close();
                //dataTool.Dispose();
                writer.Optimize();
                writer.Close();
            }
            MessageBox.Show(DateDiff(dtStart, DateTime.Now) + "完成");
            #endregion
            #endregion
        }
        /// <summary>
        /// 计算两个日期的时间间隔
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        private string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days.ToString() + "天"
                + ts.Hours.ToString() + "小时"
                + ts.Minutes.ToString() + "分钟"
                + ts.Seconds.ToString() + "秒"
                + ts.Milliseconds.ToString() + "毫秒";

            return dateDiff;
        }

        public void BindDocument(int docNum)
        {
            this.DocView.Items.Clear();
            if (CurrentInfo.isOpen && !CurrentInfo.reader.IsDeleted(docNum))
            {
                Document document = CurrentInfo.GetDocument(docNum);
                this.DocCurrentPtr.Text = docNum.ToString();
                System.Collections.IEnumerator enumerator = document.Fields();
                while (enumerator.MoveNext())
                {
                    Field current = (Field)enumerator.Current;
                    this.DocView.Items.Add(new ListViewItem(new string[] { current.Name(), CurrentInfo.GetNorm(current.Name(), docNum).ToString(), current.StringValue() }));
                }
            }
            if (CurrentInfo.isOpen && CurrentInfo.reader.IsDeleted(docNum))
            {
                this.DocView.Items.Add(new ListViewItem(new string[] { string.Empty, string.Empty, "记录已经被删除！" }));
            }
            this.tbSearch.SelectedIndex = 0;
        }

        private void btnOpenIndex_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = string.IsNullOrEmpty(this.txtIndexPath.Text) ? Application.StartupPath : this.txtIndexPath.Text;
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtIndexPath.Text = this.folderBrowserDialog1.SelectedPath;
                if (System.IO.Directory.Exists(this.txtIndexPath.Text))
                {
                    if (!CurrentInfo.IsDirectory(this.txtIndexPath.Text))
                    {
                        MessageBox.Show("指定的目录不是有效的Lucene索引目录！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        CurrentInfo.CurrentPath = this.txtIndexPath.Text;
                        this.MyBind();
                    }
                }
                else
                {
                    MessageBox.Show("指定的目录不存在！", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(CurrentInfo.CurrentPath))
                {
                    this.txtIndexPath.Text = CurrentInfo.CurrentPath;
                }
            }
        }

        private void MyBind()
        {
            if (CurrentInfo.Open())
            {
                MessageModel message = CurrentInfo.message;
                this.CtlNumOfFields.Text = message.NumberOfFields.ToString();
                this.CtlNumOfDocument.Text = message.NumberOfDocuments.ToString();
                this.CtlNumOfTerms.Text = message.NumberOfTerms.ToString();
                this.CtlLastModify.Text = message.LastModify.ToString();
                this.listbox_FieldsName.Items.Clear();
                this.NumOfDocs.Text = (message.NumberOfDocuments - 1).ToString();

                this.listbox_FieldsName.Items.Clear();
                for (int i = 0; i < message.Fields.Length; i++)
                {
                    this.listbox_FieldsName.Items.Add(message.Fields[i]);
                }

                this.listView1.Items.Clear();
                for (int j = 0; j < message.Terms.Length; j++)
                {
                    string[] items = new string[] { (j + 1).ToString(), message.Terms[j].Count.ToString(), message.Terms[j].Term.Field(), message.Terms[j].Term.Text() };
                    this.listView1.Items.Add(new ListViewItem(items));
                }
                ((SearcherFrom)this.tbSearch.TabPages[1].Controls[0]).Bind();
            }
            else
            {
                //this.DoOpen();
            }
        }

        private void listbox_FieldsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] fields = new string[this.listbox_FieldsName.SelectedItems.Count];
            for (int i = 0; i < this.listbox_FieldsName.SelectedItems.Count; i++)
            {
                fields[i] = this.listbox_FieldsName.SelectedItems[i].ToString();
            }
            if (CurrentInfo.isOpen)
            {
                int result = 50;
                CurrentInfo.MixField(fields, result);
                MessageModel message = CurrentInfo.message;
                this.listView1.Items.Clear();
                for (int j = 0; j < message.Terms.Length; j++)
                {
                    string[] items = new string[] { (j + 1).ToString(), message.Terms[j].Count.ToString(), message.Terms[j].Term.Field(), message.Terms[j].Term.Text() };
                    this.listView1.Items.Add(new ListViewItem(items));
                }
            }
        }

        private void btnPrevDoc_Click(object sender, EventArgs e)
        {
            int result = 0;
            int.TryParse(this.DocCurrentPtr.Text, out result);
            if (result > 0)
            {
                result--;
            }
            this.DocCurrentPtr.Text = result.ToString();
            this.BindDocument(result);
        }

        private void btnNextDoc_Click(object sender, EventArgs e)
        {
            int result = 0;
            int.TryParse(this.DocCurrentPtr.Text, out result);
            if (result < (CurrentInfo.message.NumberOfDocuments - 1))
            {
                result++;
            }
            this.DocCurrentPtr.Text = result.ToString();
            this.BindDocument(result);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            dicUnitCode2Name = MirrorIndexHelper.DataUtils.GetUnitNames();
            cmbUnitNames.Items.Clear();
            foreach (string key in dicUnitCode2Name.Keys)
                cmbUnitNames.Items.Add(dicUnitCode2Name[key]);
            XmlIntoSQLHelper xmlHelper = new MirrorIndexHelper.Xml2DB.XmlIntoSQLHelper();
            Dictionary<string, LiteratureType> dic = xmlHelper.GetAllLiterInfos();
            cmbLiterTypes.Items.Clear();
            foreach (string key in dic.Keys)
                cmbLiterTypes.Items.Add(dic[key].F_Code + "_" + key);

            cmbDataSourceType.SelectedIndex = 0;
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            string start = dtStart.Value.ToShortDateString();
            string end = dtEnd.Value.ToShortDateString();
            string token = start + "|" + end;
            string[] span = token.Split('|');
            txtToken.Text = EncryptHelper.Encode(token);
            txtDirect.Text = EncryptHelper.Encode(txtToken.Text.Substring(0,token.Length));
            txtPartial.Text = EncryptHelper.Encode(txtToken.Text.Reverse().ToString().Substring(0,token.Length));
            DateTime startdate = Convert.ToDateTime(span[0]);//航天五院本地刊有效期起始日期
            DateTime enddate = Convert.ToDateTime(span[1]);//航天五院本地刊有效期结束日期
            MessageBox.Show("本地刊服务时间从【" + startdate + "】开始到【" + enddate + "】结束\r\n\r\n请服务组注意到期前变更合同");
        }
        

        private void btnStartPickPDF_Click(object sender, EventArgs e)
        {
            //单位代码
            string unitcode = dicUnitCode2Name.Single(o => o.Value == cmbUnitNames.Text).Key.ToString();
            //批次号
            string batchNum = txtBatchNumber.Text.Trim();
            //起始年
            string year = txtStartYear.Text.Trim();
            int count = 0;
            lblOpertion.Text = "获取可匹配文档数";
            Update();
            Application.DoEvents();
            //可抽取全文总数
            if (DataUtils.GetLocalPeriodicalDataCount(unitcode,batchNum,year,out count))
            {
                pgsPickPDF.Maximum = count;
            }

            //创建提取的临时表：doc_id,Uniqueid
            lblOpertion.Text = "提取可匹配文档标识";
            Update();
            Application.DoEvents();
            string tablename = DataUtils.PickLocalPeriodicalDataToTable(unitcode, batchNum, year);
            MatchPeriodical.OnProgress += PickProgress;

            //用175接口匹配文摘    
            
            lblOpertion.Text = "匹配文档";
            Update();
            Application.DoEvents();

            int debugCount = 0;
            Exception timeoutError = null;
            do
            {
                timeoutError = null;
                try
                {
                    MatchPeriodical.Match(tablename, count);
                }
                catch (Exception err)
                {
                    debugCount++;
                    Console.WriteLine(debugCount);
                    timeoutError = err;
                    System.Threading.Thread.Sleep(5000);
                    continue;
                }
            } while (null != timeoutError);

            ///20181030  代码注释原因 分成两步，下面PDF下载部分放在了  button4_Click 方法里。
            ///注释代码范围 用 /*  */
            ///

            /*
            lblOpertion.Text = "提取PDF文档";
            Update();
            Application.DoEvents();
            PdfDownloader.OnProgress += PickProgress;
            
            PdfDownloader.DownloadAll(tablename);
            */

            ///20180416  代码注释原因 分成两步，下面PDF加密、加水印部分放在了  button2_Click 方法里。
            ///注释代码范围 用 /*  */
            ///

            /*
            //用175接口匹配文摘
            lblOpertion.Text = "匹配文档";
            Update();
            Application.DoEvents();

            int debugCount = 0;
            Exception timeoutError = null;
            do
            {
                timeoutError = null;
                try
                {
                    MatchPeriodical.Match(tablename, count);
                }
                catch (Exception err)
                {
                    debugCount++;
                    Console.WriteLine(debugCount);
                    timeoutError = err;
                    System.Threading.Thread.Sleep(5000);
                    continue;
                }
            } while (null != timeoutError);

            lblOpertion.Text = "提取PDF文档";
            Update();
            Application.DoEvents();
            PdfDownloader.OnProgress += PickProgress;
            PdfDownloader.DownloadAll(tablename);

            lblOpertion.Text = "PDF加密" + (chkSetMark.Checked ? "、打水印" : string.Empty);
            string userPassword = string.Empty;
            string ownerPassword = System.Configuration.ConfigurationSettings.AppSettings["pdfOwnerPassword"];
            ownerPassword = EncryptHelper.Encode(ownerPassword);
            if (rbtnUPWDinConfig.Checked)
            {
                userPassword = System.Configuration.ConfigurationSettings.AppSettings["pdfUserPassword"];
                userPassword = EncryptHelper.Encode(userPassword);
            }
            string[] sourcePDFs = System.IO.Directory.GetFiles(System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "origin"));
            string targetPath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Result");
            if (!System.IO.Directory.Exists(targetPath))
                System.IO.Directory.CreateDirectory(targetPath);
            pgsPickPDF.Maximum = sourcePDFs.Length;
            int ipgs = 0;
            foreach (string sPDF in sourcePDFs)
            {
                string tPDF = System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(sPDF));
                ipgs++;
                if (ipgs % 20 == 0)
                {
                    lblTipPickProgress.Text = ipgs + " / " + sourcePDFs.Length;
                    pgsPickPDF.Value = ipgs;
                    Update();
                    Application.DoEvents();
                }
                if (rbtnUPWDbyFilename.Checked)
                {
                    userPassword = System.IO.Path.GetFileNameWithoutExtension(sPDF).Reverse().ToString();
                    userPassword = EncryptHelper.Encode(userPassword);
                }
                iTextSharp.text.pdf.PdfReader pdfReader = null;
                try//仓储中的部分PDF是损坏的
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(sPDF);
                    pdfReader.Close();
                }
                catch
                {
                    DataUtils.UpdateDBData("update " + tablename + " set del_flag=1 where f_doc_id='" + System.IO.Path.GetFileNameWithoutExtension(sPDF) + "'");
                    continue;
                }
                finally
                {

                }
                PDFWatermark.EncryptPDF(chkSetMark.Checked, txtWaterMark.Text, sPDF, tPDF, userPassword, ownerPassword, -9);
            }

            lblOpertion.Text = "导出本地刊数据到SQLite文件";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.s3db|*.s3db";
            sfd.FileName = "data5.s3db";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Exception fileInUse = null;
                do
                {
                    fileInUse = null;
                    try
                    {
                        System.IO.File.Copy(System.IO.Path.Combine(Common.GetAssemblyPath(), "data", "sqlite.db3"), sfd.FileName, true);
                    }
                    catch (Exception fileError)
                    {
                        MessageBox.Show("文件【" + sfd.FileName + "】占用中，请重试");
                        fileInUse = fileError;
                        continue;
                    }
                } while (null != fileInUse);
                ExportLocalPeriodicalData.OnProgress += PickProgress;
                ExportLocalPeriodicalData.ExportToSqlite(tablename, sfd.FileName);
            }

            MessageBox.Show("完成");
             */
            MessageBox.Show("完成");
        }

        private void PickProgress(int total ,int pgs)
        {
            lblTipPickProgress.Text = pgs + " / " + total;
            pgsPickPDF.Maximum = total;
            pgsPickPDF.Value = pgs;
            this.Update();
            Application.DoEvents();
        }

        private void 测试加密打水印()
        {
            string sPdf = @"D:\WorkSpace\Projects\2017\内网镜像数据处理工具\MirrorIndexHelper\bin\x86\Debug\tmp_mirror29_201701_2009\JJ0237687795.pdf";
            string tPdf = @"JJ0237687795.pdf";
            PDFWatermark.EncryptPDF(true, "www.istic.ac.cn", sPdf, tPdf, "istic", "owner", -9);
            MessageBox.Show("Encoded");
            PDFWatermark.PdfDeCode(tPdf, "istic", "owner");
            MessageBox.Show("Decoded");
        }

        private void btnDeCodeToken_Click(object sender, EventArgs e)
        {
            string token = txtToken.Text.Trim();
            if(!string.IsNullOrEmpty(token))
            {
                MessageBox.Show(EncryptHelper.Decode(token));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox2.Visible = !groupBox2.Visible;
        }

        private void btnSelectXMLPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            
            if(fb.ShowDialog() ==  DialogResult.OK)
            {
                txtXMLPath.Text = fb.SelectedPath;
                this.Update();
                Application.DoEvents();
            }
        }

        /// <summary>
        /// XML入库
        /// </summary>
        private void btnXMLIntoDB_Click(object sender, EventArgs e)
        {
            string[] xmls = System.IO.Directory.GetFiles(txtXMLPath.Text, "*.xml", chkXmlPathIncludeChild.Checked ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
            pgsXmlIntoDB.Maximum = xmls.Length;
            XmlIntoSQLHelper xmlHelper = new XmlIntoSQLHelper();
            LiteratureType lt = xmlHelper.GetLiterInfoByType(cmbLiterTypes .Text.Trim().Split('_')[1]);
            string batchNO = txtBatchNO.Text;
            //反射自己，根据用户选择和配置设置写库内容和更新方法
            System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Common.GetAssemblyPath(), "MirrorIndexHelper.exe"));
            string config = System.Configuration.ConfigurationSettings.AppSettings[lt.F_Type];
            string[] tmp = config.Split('|');//反射地址和一个垃圾常量字段值（设计表结构的人没过脑子）
            Type asmType = asm.GetType(tmp[0]);
            object instance = System.Activator.CreateInstance(asmType);
            System.Reflection.FieldInfo[] filedsInfo = instance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);
            Dictionary<string, System.Reflection.FieldInfo> XDocFields = new Dictionary<string, System.Reflection.FieldInfo>();
            foreach(System.Reflection.FieldInfo finfo in filedsInfo)
            {
                if (finfo.Name == "F_Batch")
                    finfo.SetValue(instance, batchNO);
                if(finfo.Name =="F_TableName")
                    finfo.SetValue(instance, tmp[1]);
                if (finfo.Name == "InsertSql")
                    finfo.GetValue(instance);
                XDocFields[finfo.Name] = finfo;
            }
            int ipgs = 0;
            List<string> lstInsertSqls = new List<string>();

            string pattern = @"(?<value><paper>[\s\S]*?</paper>)";
            //读XML入库
            foreach(string xml in xmls)
            {

                //ceshi
                formatXml(xml);

                ipgs++;
                pgsXmlIntoDB.Value = ipgs;
                this.Update();
                Application.DoEvents();
                bool getData = false;   ///20190514

                System.IO.StreamReader sr = new System.IO.StreamReader(xml,Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                string fname = System.IO.Path.GetFileName(xml);

                ///20180413新增自动判断起始的标签名
                ///
                string startEndCode = null;
                System.Xml.XmlDocument xdo = new System.Xml.XmlDocument();
                xdo.Load(xml);
                System.Xml.XmlElement xe = xdo.DocumentElement;              
                foreach (System.Xml.XmlNode xn in xe.ChildNodes)
                {
                    if (xn.Name.ToLower().Equals("paper")) { startEndCode = "<paper>|</paper>"; break; }
                    else if (xn.Name.ToLower().Equals("record")) { startEndCode = "<record>|</record>"; break; }
                    else if (xn.Name.ToLower().Equals("gc")) { startEndCode = "<gc>|</gc>"; break; }     //t01
                    else { MessageBox.Show("请核实节点名称是否含有：" + " paper " + " record " + " gc "); }
                }


                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    sb.AppendLine(line);

                    string[] seCode = startEndCode.Split('|');

                    if (line.Trim().ToLower() ==  seCode[0])      //"<record>")
                    {
                        sb = new StringBuilder();
                        sb.AppendLine(line);
                        //getData = true;
                    }
                    else if (line.Trim().ToLower() == seCode[1])     //"</record>")
                    {
                        var value = sb.ToString();
                        string sql = string.Format(XDocFields["InsertSql"].GetValue(instance).ToString(),
                                                            lt.F_TableName,
                                                            /*lt.F_Code.ToLower().StartsWith("p") ? "Sub" : */string.Empty,
                                                            value.Replace("'", "''"),
                                                            DateTime.Now.ToShortDateString(),
                                                            fname,
                                                            batchNO,
                                                            //XDocFields["F_TableName"].GetValue(instance),
                                                            lt.F_Code);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            getData = true;
                        }

                        if (lstInsertSqls.Count > 499)
                        {
                            xmlHelper.TransXmlDataIntoSQL(lstInsertSqls);
                            lstInsertSqls.Clear();
                            lblXmlTips.Text = ipgs.ToString() + " / " + xmls.Length;
                            this.Update();
                            Application.DoEvents();
                        }
                        lstInsertSqls.Add(sql);
                        Application.DoEvents();
                    }
                }
                sr.Close();
                sr.Dispose();
                //string xmlContent = System.IO.File.ReadAllText(xml, Encoding.UTF8);
                //MatchCollection mc = Regex.Matches(xmlContent, pattern, RegexOptions.IgnoreCase);
                //foreach (Match ma in mc)
                //{
                //    string value = ma.Groups["value"].Value.Trim();
                //    string fname = System.IO.Path.GetFileName(xml);
                //    //F_Content,F_InsertDate,F_FileName,F_Batch,F_TableName,F_Type
                //    string sql = string.Format(XDocFields["InsertSql"].GetValue(instance).ToString(),
                //                                            lt.F_TableName,
                //                                            /*lt.F_Code.ToLower().StartsWith("p") ? "Sub" : */string.Empty,
                //                                            value.Replace("'","''"),
                //                                            DateTime.Now.ToShortDateString(),
                //                                            fname,
                //                                            batchNO,
                //                                            //XDocFields["F_TableName"].GetValue(instance),
                //                                            lt.F_Code);
                //    if (lstInsertSqls.Count>499)
                //    {
                //        xmlHelper.TransXmlDataIntoSQL(lstInsertSqls);
                //        lstInsertSqls.Clear();
                //        lblXmlTips.Text = ipgs.ToString() + " / " + xmls.Length;
                //        this.Update();
                //        Application.DoEvents();
                //    }
                //    lstInsertSqls.Add(sql);
                //    Application.DoEvents();
                //}


                /////20190514  因有些数据来源没有格式化，导致数据无法取出来。新增格式化

                if (!getData) {

                   string fileNewPath =  formatXml(xml);
                   System.IO.StreamReader sr2 = new System.IO.StreamReader(fileNewPath, Encoding.UTF8);

                   while (!sr2.EndOfStream)
                   {
                       var line = sr2.ReadLine();       /*<paper xmlns="http://spec.nstl.gov.cn/specification/namespace">*/
                       sb.AppendLine(line);

                       string[] seCode = startEndCode.Split('|');
                       string seCode0 = seCode[0].Substring(0, seCode[0].Length - 1);   ///20190515 新增判断起始位置，来的数据太奇葩，各种样子都有
                       bool scStart = Regex.IsMatch(line, seCode0 + " [\\s\\S]*?>");
                       if (line.Trim().ToLower() == seCode0 + "s>") {
                           scStart = false;
                       }

                       if (line.Trim().ToLower() == seCode[0] || scStart)      //"<record>")
                       {
                           sb = new StringBuilder();
                           sb.AppendLine(line);
                       }
                       else if (line.Trim().ToLower() == seCode[1])     //"</record>")
                       {
                           var value = sb.ToString();
                           string sql = string.Format(XDocFields["InsertSql"].GetValue(instance).ToString(),
                                                               lt.F_TableName,
                               /*lt.F_Code.ToLower().StartsWith("p") ? "Sub" : */string.Empty,
                                                               value.Replace("'", "''"),
                                                               DateTime.Now.ToShortDateString(),
                                                               fname,
                                                               batchNO,
                               //XDocFields["F_TableName"].GetValue(instance),
                                                               lt.F_Code);


                           if (lstInsertSqls.Count > 499)
                           {
                               xmlHelper.TransXmlDataIntoSQL(lstInsertSqls);
                               lstInsertSqls.Clear();
                               lblXmlTips.Text = ipgs.ToString() + " / " + xmls.Length;
                               this.Update();
                               Application.DoEvents();
                           }
                           lstInsertSqls.Add(sql);
                           Application.DoEvents();
                          
                       }
                   }//
                   sr2.Close();
                   sr2.Dispose();
                   File.Delete(fileNewPath);
                }

            }
            if(lstInsertSqls.Count>0)
            {
                xmlHelper.TransXmlDataIntoSQL(lstInsertSqls);
                lstInsertSqls.Clear();
                lblXmlTips.Text = ipgs.ToString() + " / " + xmls.Length;
                this.Update();
                Application.DoEvents();
            }
            //更新条件字段
            lblXmlTips.Text = "正在更新写入的数据……";
            System.Reflection.MethodInfo updateMethod = asmType.GetMethod("UpdateSQL");
            string update = (string)updateMethod.Invoke(instance, null);
            if (!string.IsNullOrEmpty(update))//////20170828：对专利等各别表没有做Update处理，视图能满足建索引需要
                xmlHelper.UpdateInsertData(string.Format(update, lt.F_TableName, batchNO));
            lblXmlTips.Text = "更新写入的数据完成！";
            MessageBox.Show("完成");
        }

        private void gpConfigDataSource_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox_analyzer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkSetMark_CheckedChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// PDF 加密加水印   20180417 拆分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            MatchPeriodical.OnProgress += PickProgress;    //进度条
            //单位代码
            string unitcode = dicUnitCode2Name.Single(o => o.Value == cmbUnitNames.Text).Key.ToString();
            //批次号
            string batchNum = txtBatchNumber.Text.Trim();
            //起始年
            string year = txtStartYear.Text.Trim();
            //当批次PDF临时表
            string tablename = "tmp_" + unitcode + "_" + batchNum + "_" + year;
            //当批次PDF数量
            //int count = countT(batchNum, year, unitcode);


            lblOpertion.Text = "PDF加密" + (chkSetMark.Checked ? "、打水印" : string.Empty);
            string userPassword = string.Empty;
            string ownerPassword = System.Configuration.ConfigurationSettings.AppSettings["pdfOwnerPassword"];
            ownerPassword = EncryptHelper.Encode(ownerPassword);
            if (rbtnUPWDinConfig.Checked)
            {
                userPassword = System.Configuration.ConfigurationSettings.AppSettings["pdfUserPassword"];
                userPassword = EncryptHelper.Encode(userPassword);
            }
            string[] sourcePDFs = System.IO.Directory.GetFiles(System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "origin"));
            string targetPath = System.IO.Path.Combine(Common.GetAssemblyPath(), tablename, "Result");
            if (!System.IO.Directory.Exists(targetPath))
                System.IO.Directory.CreateDirectory(targetPath);
            pgsPickPDF.Maximum = sourcePDFs.Length;
            int ipgs = 0;
            foreach (string sPDF in sourcePDFs)
            {              
                string tPDF = System.IO.Path.Combine(targetPath, System.IO.Path.GetFileName(sPDF));
                ipgs++;
                if (ipgs % 20 == 0)
                {
                    lblTipPickProgress.Text = ipgs + " / " + sourcePDFs.Length;
                    pgsPickPDF.Value = ipgs;
                    Update();
                    Application.DoEvents();
                }
                if (rbtnUPWDbyFilename.Checked)
                {
                    userPassword = System.IO.Path.GetFileNameWithoutExtension(sPDF).Reverse().ToString();
                    userPassword = EncryptHelper.Encode(userPassword);
                }
       

                iTextSharp.text.pdf.PdfReader pdfReader = null;
                try//仓储中的部分PDF是损坏的
                {
                    pdfReader = new iTextSharp.text.pdf.PdfReader(sPDF);
                    pdfReader.Close();
                    if (!pdfReader.IsOpenedWithFullPermissions) {            //20180418 判断PDF是否有密码 有就去掉它
                        string s = "pdfdecrypt -i " + sPDF + " -o " + sPDF;
                        cmdRun(s);                       
                    }
                }
                catch
                {                   
                    DataUtils.UpdateDBData("update " + tablename + " set del_flag=1 where f_doc_id='" + System.IO.Path.GetFileNameWithoutExtension(sPDF) + "'");
                    continue;
                }
                finally
                {

                }
                PDFWatermark.EncryptPDF(chkSetMark.Checked, txtWaterMark.Text, sPDF, tPDF, userPassword, ownerPassword, -9);
            }

            lblOpertion.Text = "导出本地刊数据到SQLite文件";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.s3db|*.s3db";
            sfd.FileName = "data5.s3db";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Exception fileInUse = null;
                do
                {
                    fileInUse = null;
                    try
                    {
                        System.IO.File.Copy(System.IO.Path.Combine(Common.GetAssemblyPath(), "data", "sqlite.db3"), sfd.FileName, true);
                    }
                    catch (Exception fileError)
                    {
                        MessageBox.Show("文件【" + sfd.FileName + "】占用中，请重试");
                        fileInUse = fileError;
                        continue;
                    }
                } while (null != fileInUse);
                ExportLocalPeriodicalData.OnProgress += PickProgress;
                ExportLocalPeriodicalData.ExportToSqlite(tablename, sfd.FileName);
            }

            MessageBox.Show("完成");
        }

        /// <summary>
        /// 返回抽取数据总量
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="year"></param>
        /// <param name="unitcode"></param>
        /// <returns></returns>
        public int countT(string batch, string year, string unitcode)
        {
            SqlTool mssql = new SqlTool(System.Configuration.ConfigurationSettings.AppSettings["dbconnectionstring"]);
            System.Data.SqlClient.SqlCommand cmd = mssql.Connection.CreateCommand();
            int countNo = 0;
            string sqlString = string.Empty;
            sqlString = "select count(0) as cnt from t_periodical_eng where 1=1 ";
            sqlString = sqlString + " and f_batch='"+batch+"' and f_year>='"+year+"'\r\n";
            sqlString = sqlString + " and f_journalcode in(\r\nselect f_journalcode from t_locperlist where f_unitcode='" +unitcode+ "' and f_journalcode is not null and f_journalcode !=''\r\n)";
            try
            {
                if (mssql.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                cmd.CommandText = sqlString;
                cmd.CommandTimeout = 1000;
                cmd.ExecuteNonQuery();
                countNo = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception err)
            {          
                throw err;
            }
            finally
            {
                cmd.Dispose();
                mssql.Dispose();
            }

            return countNo ;
        }



        /// <summary>
        /// 去掉PDF安全性设置  20180416
        /// </summary>
        /// <param name="str"></param>
        public void cmdRun(string str)
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = "cmd.exe";
            //p.StartInfo.Arguments = "/c C:\\Windows\\System32\\cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            //p.StartInfo.Verb = "RunAs";
            p.Start();
            string path = Directory.GetCurrentDirectory();
            string driveLetter = path.Substring(0, 1);
            p.StandardInput.WriteLine(driveLetter);
            p.StandardInput.WriteLine(@"cd " + path);
            p.StandardInput.AutoFlush = true;
            p.StandardInput.WriteLine(str + "&exit");

            p.StandardInput.AutoFlush = true;

            //获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //单位代码
            string unitcode = dicUnitCode2Name.Single(o => o.Value == cmbUnitNames.Text).Key.ToString();
            //批次号
            string batchNum = txtBatchNumber.Text.Trim();
            //起始年
            string year = txtStartYear.Text.Trim();
            string tableName = DataUtils.PickLocalPeriodicalDataToTable(unitcode, batchNum, year);
            PdfDownloader.incrementDownPDF(tableName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //单位代码
            string unitcode = dicUnitCode2Name.Single(o => o.Value == cmbUnitNames.Text).Key.ToString();
            //批次号
            string batchNum = txtBatchNumber.Text.Trim();
            //起始年
            string year = txtStartYear.Text.Trim();
            string tablename = "tmp_" + unitcode + "_" + batchNum + "_" + year;
            lblOpertion.Text = "提取PDF文档";
            Update();
            Application.DoEvents();
            PdfDownloader.OnProgress += PickProgress;

            PdfDownloader.DownloadAll(tablename);
        }




        ///20190514  格式化xml
        ///
        public string formatXml(string path) {

            string strDir = System.Environment.CurrentDirectory;
            string strTarDir = strDir + "\\TempXmlFiles";
            if (!Directory.Exists(strTarDir))
            {
                Directory.CreateDirectory(strTarDir);
            }

            string fileName = path.Substring(path.LastIndexOf("\\") + 1);
            string tar = strTarDir+"\\"+fileName;
            //File.Copy(path, tar);
            if (File.Exists(tar)) {
                File.Delete(tar);
            }
            System.Xml.XmlDocument xdo = new System.Xml.XmlDocument();
            xdo.Load(path);
            xdo.Save(tar);

            return tar;

           

        }



    }


}
