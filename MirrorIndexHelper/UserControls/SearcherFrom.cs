using Lucene.Net.Analysis.KTDictSeg;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Index;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LuceneHelper
{
    
    public class SearcherFrom : UserControl
    {
        private Analyzer analyzer;
        private ComboBox Analyzies;
        private Button button1;
        private Button button10;
        private Button button9;
        private IContainer components;
        private ComboBox Fields;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private Label label1;
        private Label label18;
        private Label label19;
        private Label label2;
        private Query query;
        private string field;
        private bool isprepare;
        private ListView ResultList;
        private TextBox SearcherExpain;
        private TextBox SearcherKey;

        public SearcherFrom()
        {
            this.InitializeComponent();
        }

        public void Bind()
        {
            if (CurrentInfo.isOpen)
            {
                this.Fields.Items.Clear();
                string[] fields = CurrentInfo.message.Fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    this.Fields.Items.Add(fields[i]);
                }
                if (this.Fields.Items.Count > 0)
                {
                    this.Fields.SelectedIndex = 0;
                }
            }
        }

        private void BuildExpainQuery()
        {
            string text = this.SearcherExpain.Text;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
            {
                this.SetAnalyzer();
                if (this.isprepare)
                {
                    QueryParser parser = new QueryParser(this.field, this.analyzer);
                    this.query = parser.Parse(text);
                    Term term = new Term(this.field, text);
                    this.SearcherExpain.Text = this.query.ToString();
                }
            }
        }

        private void BuildQuery()
        {
            string text = this.SearcherKey.Text;
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
            {
                this.SetAnalyzer();
                if (this.isprepare)
                {
                    QueryParser parser = new QueryParser(this.field, this.analyzer);
                    this.query = parser.Parse(text);
                    Term term = new Term(this.field, text);
                    this.SearcherExpain.Text = this.query.ToString();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ResultList.Items.Clear();
            this.BuildExpainQuery();
            if (this.query != null)
            {
                this.DoSearch();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.ResultList.Items.Clear();
            this.BuildQuery();
            if (this.query != null)
            {
                this.DoSearch();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.BuildQuery();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoSearch()
        {
            IndexSearcher searcher = new IndexSearcher(CurrentInfo.reader);
            
            Hits hits = searcher.Search(this.query);
            int num = hits.Length();
            this.label2.Text = string.Format("共搜索到{0}条记录，最多显示前100条。（双击记录查看详细）", num, 100);
            this.ResultList.Items.Clear();
            this.ResultList.Columns.Clear();
            this.ResultList.Columns.Add("#", 80);
            this.ResultList.Columns.Add("Score", 80);
            string[] fields = CurrentInfo.message.Fields;
            for (int i = 0; i < fields.Length; i++)
            {
                this.ResultList.Columns.Add(fields[i], 120);
            }
            num = (num > 100) ? 100 : num;
            for (int j = 0; j < num; j++)
            {
                Document document = hits.Doc(j);
                float num4 = hits.Score(j);
                int num5 = hits.Id(j);
                List<string> list = new List<string>();
                list.Add(num5.ToString());
                list.Add(num4.ToString("f3"));
                for (int k = 0; k < fields.Length; k++)
                {
                    list.Add(document.Get(fields[k]));
                }
                this.ResultList.Items.Add(new ListViewItem(list.ToArray()));
            }
            searcher.Close();
        }

        private void InitializeComponent()
        {
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.Analyzies = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.Fields = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SearcherExpain = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.SearcherKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ResultList = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.button10);
            this.groupBox5.Controls.Add(this.Analyzies);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.Fields);
            this.groupBox5.Location = new System.Drawing.Point(265, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(503, 201);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "其它选项";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 172);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "表达式搜索";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(12, 172);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 2;
            this.button10.Text = "搜索";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // Analyzies
            // 
            this.Analyzies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Analyzies.FormattingEnabled = true;
            this.Analyzies.Location = new System.Drawing.Point(64, 20);
            this.Analyzies.Name = "Analyzies";
            this.Analyzies.Size = new System.Drawing.Size(147, 20);
            this.Analyzies.TabIndex = 3;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 25);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(53, 12);
            this.label19.TabIndex = 2;
            this.label19.Text = "分词器：";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(22, 51);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(41, 12);
            this.label18.TabIndex = 1;
            this.label18.Text = "字段：";
            this.label18.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Fields
            // 
            this.Fields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Fields.FormattingEnabled = true;
            this.Fields.Location = new System.Drawing.Point(64, 46);
            this.Fields.Name = "Fields";
            this.Fields.Size = new System.Drawing.Size(147, 20);
            this.Fields.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SearcherExpain);
            this.groupBox4.Controls.Add(this.button9);
            this.groupBox4.Controls.Add(this.SearcherKey);
            this.groupBox4.Location = new System.Drawing.Point(6, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(253, 201);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "搜索词条";
            // 
            // SearcherExpain
            // 
            this.SearcherExpain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SearcherExpain.Location = new System.Drawing.Point(6, 116);
            this.SearcherExpain.Multiline = true;
            this.SearcherExpain.Name = "SearcherExpain";
            this.SearcherExpain.Size = new System.Drawing.Size(241, 79);
            this.SearcherExpain.TabIndex = 2;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(70, 88);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(112, 23);
            this.button9.TabIndex = 1;
            this.button9.Text = "解析为表达式";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // SearcherKey
            // 
            this.SearcherKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SearcherKey.Location = new System.Drawing.Point(6, 18);
            this.SearcherKey.Multiline = true;
            this.SearcherKey.Name = "SearcherKey";
            this.SearcherKey.Size = new System.Drawing.Size(241, 64);
            this.SearcherKey.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "结果：";
            // 
            // ResultList
            // 
            this.ResultList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultList.FullRowSelect = true;
            this.ResultList.GridLines = true;
            this.ResultList.Location = new System.Drawing.Point(12, 231);
            this.ResultList.MultiSelect = false;
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(750, 173);
            this.ResultList.TabIndex = 5;
            this.ResultList.UseCompatibleStateImageBehavior = false;
            this.ResultList.View = System.Windows.Forms.View.Details;
            this.ResultList.DoubleClick += new System.EventHandler(this.ResultList_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 6;
            // 
            // SearcherFrom
            // 
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ResultList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Name = "SearcherFrom";
            this.Size = new System.Drawing.Size(780, 419);
            this.Load += new System.EventHandler(this.SearcherFrom_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ResultList_DoubleClick(object sender, EventArgs e)
        {
            if (this.ResultList.SelectedIndices.Count == 1)
            {
                int num = this.ResultList.SelectedIndices[0];
                int docNum = int.Parse(this.ResultList.Items[num].SubItems[0].Text);
                TabControl parent = (TabControl) base.Parent.Parent;
                parent.SelectTab(1);
                ((frmMain) parent.FindForm()).BindDocument(docNum);
            }
        }

        private void SearcherFrom_Load(object sender, EventArgs e)
        {
            this.Analyzies.Items.Add("StandardAnalyzer");
            
            this.Analyzies.Items.Add("KeywordAnalyzer");
            this.Analyzies.Items.Add("SimpleAnalyzer");
            this.Analyzies.Items.Add("StopAnalyzer");
            this.Analyzies.Items.Add("WhitespaceAnalyzer");
            this.Analyzies.Items.Add("KTDictSegAnalyzer");
            //this.Analyzies.Items.Add("NstlSegAnalyzer");
           //this.Analyzies.Items.Add("PanGuAnalyzer");
            this.Analyzies.SelectedIndex = 0;
            this.Bind();
        }

        private void SetAnalyzer()
        {
            this.isprepare = false;
            int selectedIndex = this.Analyzies.SelectedIndex;
            if (selectedIndex >= 0)
            {
                switch (selectedIndex)
                {
                    case 1:
                        this.analyzer = new KeywordAnalyzer();
                        break;

                    case 2:
                        this.analyzer = new SimpleAnalyzer();
                        break;

                    case 3:
                        this.analyzer = new StopAnalyzer();
                        break;

                    case 4:
                        this.analyzer = new WhitespaceAnalyzer();
                        break;

                    case 5:
                        this.analyzer = new KTDictSegAnalyzer();
                        break;
                    //case 6:
                    //    this.analyzer = new NstlAnalyzer();
                    //    break;
                    //case 7:
                    //    this.analyzer = new Lucene.Net.Analysis.PanGu.PanGuAnalyzer();
                    //    break;
                    default:
                        this.analyzer = new KTDictSegAnalyzer();
                        break;
                }
                if (this.Fields.Items.Count > 0)
                {
                    this.field = this.Fields.SelectedItem.ToString();
                }
                if (!string.IsNullOrEmpty(this.field))
                {
                    this.isprepare = true;
                }
            }
        }
    }
}

