using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper
{
    class Common
    {
        static Random rnd = new Random((int)DateTime.Now.Ticks);
        static public string GetRndString(int len)
        {
            string str = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder res = new StringBuilder();
            for (int i = 0; i < len; ++i)
                res.Append(str.Substring(rnd.Next(str.Length), 1));
            return res.ToString();
        }
        static public string GetAssemblyPath()
        {
            const string _PREFIX = @"file:///";
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            codeBase = codeBase.Substring(_PREFIX.Length, codeBase.Length - _PREFIX.Length).Replace("/", "\\");
            return System.IO.Path.GetDirectoryName(codeBase) + @"\";
        }
    }
}
