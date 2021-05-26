using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    /// <summary>
    /// 文献类型对应数据表T_LiteruatureType
    /// </summary>
    class LiteratureType
    {
        public string F_Code = string.Empty;
        public string F_Type = string.Empty;
        public string F_TableName = string.Empty;
        public LiteratureType() { }
        public LiteratureType(string code,string type,string tbname)
        {
            F_Code = code;
            F_Type = type;
            F_TableName = tbname;
        }
    }
}
