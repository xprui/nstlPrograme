using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuceneHelper
{
    class Fieldattribute
    {
        #region 字段属性设置
        private string orgfieldname = string.Empty;
        /// <summary>
        /// 源字段名
        /// </summary>
        public string Orgfieldname
        {
            get { return orgfieldname; }
            set { orgfieldname = value; }
        }
        private string tagertfieldname = string.Empty;
        /// <summary>
        /// 索引后使用字段名
        /// </summary>
        public string Tagertfieldname
        {
            get { return tagertfieldname; }
            set { tagertfieldname = value; }
        }
        private string indextype = string.Empty;
        /// <summary>
        /// 索引类型
        /// </summary>
        public string Indextype
        {
            get { return indextype; }
            set { indextype = value; }
        }
        private string storetype = string.Empty;
        /// <summary>
        /// 存储类型
        /// </summary>
        public string Storetype
        {
            get { return storetype; }
            set { storetype = value; }
        }
        #endregion
    }
}
