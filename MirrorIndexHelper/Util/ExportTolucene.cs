using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.KTDictSeg;
using System.Data.SqlClient;
using System.Reflection;

namespace LuceneHelper
{
    class ExportTolucene
    {
        public static void AddDocument(IndexWriter writer, System.Data.IDataReader areader, List<Fieldattribute> afildlist, bool isGroupIndex)
        {
            Document document = new Document();
            try
            {
                for (int i = 0; i < afildlist.Count; i++)
                {
                    Type typeofindex = typeof(Field.Index);
                    Type typeofstore = typeof(Field.Store);

                    string fieldname = afildlist[i].Tagertfieldname;
                    string origfieldname = afildlist[i].Orgfieldname;
                    try
                    {
                        //20190326 byLuyg 新增判断 abstract 里是否含有 cdata 的特殊字符
                        var fn = fieldname;
                        var are = areader[origfieldname].ToString();
                        if (fn == "abstract") {
                            are = System.Text.RegularExpressions.Regex.Replace(are, "<.*?>", string.Empty);
                            are = are.Replace("]]>", "");
                        }
                        var tfstore= (Field.Store)typeofstore.GetField(afildlist[i].Storetype).GetValue(null);
                        var tfindex= (Field.Index)typeofindex.GetField(afildlist[i].Indextype).GetValue(null);
                        //document.Add(new Field(fieldname, areader[origfieldname].ToString(), (Field.Store)typeofstore.GetField(afildlist[i].Storetype).GetValue(null), (Field.Index)typeofindex.GetField(afildlist[i].Indextype).GetValue(null)));
                        document.Add(new Field(fn, are, tfstore, tfindex));
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    #region 废弃代码
                    /*
                    if (fieldname == "TC")
                    {
                        document.Add(new Field(fieldname, NumberTools.LongToString((long)int.Parse(areader[fieldname].ToString())), Field.Store.YES, Field.Index.UN_TOKENIZED));
                    }
                    else
                    {
                        document.Add(new Field(fieldname, areader[fieldname].ToString(), (Field.Store)typeofstore.GetField(afildlist[i].Storetype).GetValue(null), (Field.Index)typeofindex.GetField(afildlist[i].Indextype).GetValue(null)));
                    }
                    */
                    #endregion
                }
                try
                {
                    writer.AddDocument(document);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
