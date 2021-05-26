using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Lucene.Net;
using Lucene.Net.Analysis;

using Lucene.Net.Index;
using Lucene.Net.Documents;

using Lucene.Net.Search;
using Lucene.Net.QueryParsers;


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LuceneHelper
{
    public class CurrentInfo
    {
        public static string CurrentPath;
        public static bool isOpen = false;
        public static MessageModel message = new MessageModel();
        public static IndexReader reader;

        private static void Bind()
        {
            message.LastModify = DateTime.FromBinary(IndexReader.LastModified(reader.Directory()));
            message.NumberOfDocuments = reader.NumDocs();
            //2.9 ==> ICollection<string> fieldNames = reader.GetFieldNames(IndexReader.FieldOption.ALL);
            //2.3 ==> ICollection fieldNames = reader.GetFieldNames(IndexReader.FieldOption.ALL);
            ICollection fieldNames = reader.GetFieldNames(IndexReader.FieldOption.ALL);
            message.NumberOfFields = fieldNames.Count;
            IEnumerator enumerator = fieldNames.GetEnumerator();
            string[] strArray = new string[fieldNames.Count];
            int num = 0;
            while (enumerator.MoveNext())
            {
                //DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                strArray[num++] = enumerator.Current.ToString();
            }
            message.Fields = strArray;
            MixField(null, 50);
        }

        public static void Close()
        {
            if (isOpen)
            {
                isOpen = false;
                reader.Close();
            }
        }

        public static TermModle CurrentTerm(string field, string text)
        {
            TermEnum enum2 = reader.Terms();
            if (enum2.SkipTo(new Term(field, text)))
            {
                TermModle modle = new TermModle(enum2.Term(), enum2.DocFreq());
                enum2.Close();
                return modle;
            }
            return null;
        }

        public static void Delete(int docNum)
        {
            if (isOpen)
            {
                reader.DeleteDocument(docNum);
                reader.Flush();
            }
        }

        public static Document GetDocument(int docNum)
        {
            return reader.Document(docNum);
        }

        public static float GetNorm(string field, int docId)
        {
            return SmallFloat.Byte315ToFloat(reader.Norms(field)[docId]);
        }

        public static bool IsDirectory(string path)
        {
            string[] files = System.IO.Directory.GetFiles(path);
            if (files.Length >= 3)
            {
                if (files.Length > 200)
                {
                    return false;
                }
                foreach (string str in files)
                {
                    if (str.EndsWith("segments.gen"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void MixField(string[] fields, int num)
        {
            num++;
            TermInfoQueue queue = new TermInfoQueue(num);
            TermEnum enum2 = reader.Terms();
            int count = 0;
            while (enum2.Next())
            {
                string str = enum2.Term().Field();
                if ((fields != null) && (fields.Length > 0))
                {
                    bool flag = true;
                    for (int j = 0; j < fields.Length; j++)
                    {
                        if (str.Equals(fields[j]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                }
                if (enum2.DocFreq() > count)
                {
                    queue.Put(new TermModle(enum2.Term(), enum2.DocFreq()));
                    if (queue.Size() < num)
                    {
                        continue;
                    }
                    queue.Pop();
                    count = ((TermModle)queue.Top()).Count;
                }
            }
            enum2.Close();
            TermModle[] modleArray = new TermModle[queue.Size()];
            for (int i = 0; i < modleArray.Length; i++)
            {
                modleArray[(modleArray.Length - i) - 1] = (TermModle)queue.Pop();
            }
            message.Terms = modleArray;
        }

        public static TermModle NextTerm(string field, string text)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(text))
            {
                flag = true;
            }
            TermEnum enum2 = reader.Terms();
            while (flag && enum2.Next())
            {
                if (field.Equals(enum2.Term().Field()))
                {
                    TermModle modle = new TermModle(enum2.Term(), enum2.DocFreq());
                    enum2.Close();
                    return modle;
                }
            }
            if (enum2.SkipTo(new Term(field, text)))
            {
                while (enum2.Next())
                {
                    if (field.Equals(enum2.Term().Field()))
                    {
                        TermModle modle2 = new TermModle(enum2.Term(), enum2.DocFreq());
                        enum2.Close();
                        return modle2;
                    }
                }
            }
            return null;
        }

        public static bool Open()
        {
            isOpen = true;
            Lucene.Net.Store.FSDirectory.GetDirectory(CurrentPath);
            try
            {
                reader = Lucene.Net.Index.IndexReader.Open(CurrentPath);
            }
            catch (Exception exception)
            {
                isOpen = false;
                MessageBox.Show(exception.Message, "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            message.IndexName = CurrentPath;
            Bind();
            return true;
        }

        public static void ReOpen()
        {
            if (isOpen)
            {
                isOpen = false;
                reader = reader.Reopen();
                Bind();
                isOpen = true;
            }
        }

        public static List<TermDoc> TermDocs(Term term)
        {
            Lucene.Net.Index.TermDocs docs = reader.TermDocs(term);
            List<TermDoc> list = new List<TermDoc>();
            while (docs.Next())
            {
                TermDoc doc2 = new TermDoc();
                doc2.Freq = docs.Freq();
                doc2.Doc = docs.Doc();
                doc2.Term = term;
                TermDoc item = doc2;
                list.Add(item);
            }
            docs.Close();
            return list;
        }

        public class TermDoc
        {
            [CompilerGenerated]
            private int Doc__BackingField;
            [CompilerGenerated]
            private int Freq__BackingField;
            [CompilerGenerated]
            private Lucene.Net.Index.Term Term__BackingField;

            public int Doc
            {
                [CompilerGenerated]
                get
                {
                    return this.Doc__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.Doc__BackingField = value;
                }
            }

            public int Freq
            {
                [CompilerGenerated]
                get
                {
                    return this.Freq__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.Freq__BackingField = value;
                }
            }

            public Lucene.Net.Index.Term Term
            {
                [CompilerGenerated]
                get
                {
                    return this.Term__BackingField;
                }
                [CompilerGenerated]
                set
                {
                    this.Term__BackingField = value;
                }
            }
        }
    }
}
