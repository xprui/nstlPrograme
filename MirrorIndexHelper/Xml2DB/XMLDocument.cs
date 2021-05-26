using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    /// <summary>
    /// 文献类型描述，除共有字段和写入语句外，实体文献需要重写更新语句
    /// </summary>
    public abstract class XMLDocument
    {
        public string F_Batch = string.Empty;
        public string F_TableName = string.Empty;
        public string InsertSql = "insert into {0}(F_Content,F_InsertDate,F_FileName,F_Batch,F_{1}Type)values('{2}','{3}','{4}','{5}','{6}')";
        public abstract string UpdateSQL();
    }
}
