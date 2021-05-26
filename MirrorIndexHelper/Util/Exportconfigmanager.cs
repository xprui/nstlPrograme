using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneHelper
{
    class Exportconfigmanager
    {
        public static string Getsqlcmdstr(List<Fieldattribute> fieldlist)
        {//拼SQL语句用,把传入的字段list拼写成"[field1],[field2],...[fieldn]"
            string cmdstr = "";
            foreach (Fieldattribute afield in fieldlist)
            {
                cmdstr += "[" + afield.Orgfieldname + "]" + " ,";
            }
            return cmdstr.TrimEnd(',');

        }
    }
}
