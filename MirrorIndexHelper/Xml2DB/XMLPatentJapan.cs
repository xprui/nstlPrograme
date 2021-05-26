using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    public class XMLPatentJapan : XMLDocument
    {
        public new string InstalSql = "insert into {0}(F_Content,F_InsertDate,F_FileName,F_Batch,F_TableName,F_SubType)values('{1}','{2}','{3}','{4}','{5}','{6}')";
        public override string UpdateSQL()
        {
            return string.Empty;
        }
    }
}
