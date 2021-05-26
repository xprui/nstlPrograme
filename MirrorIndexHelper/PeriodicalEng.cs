using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace MirrorIndexHelper
{
    public class PeriodicalEng 
    {
        string _f_UniqueId = string.Empty;
        /// <summary>
        /// 仓储中的数据标识
        /// </summary>
        public string F_UniqueId
        {
            get { return _f_UniqueId; }
            set { _f_UniqueId = value; }
        }
        string _f_qcode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string F_qcode
        {
            get { return _f_qcode; }
            set { _f_qcode = value; }
        }
        string _f_DOI = string.Empty;
        /// <summary>
        /// 索取号，数据库中有重复？？？？
        /// </summary>
        public string F_DOI
        {
            get { return _f_DOI; }
            set { _f_DOI = value; }
        }
        string _f_title = string.Empty;
        /// <summary>
        /// 标题
        /// </summary>
        public string F_Title
        {
            get { return _f_title; }
            set { _f_title = value; }
        }
        string _f_OtherTitle = string.Empty;
        /// <summary>
        /// 其它题名
        /// </summary>
        public string F_OtherTitle
        {
            get { return _f_OtherTitle; }
            set { _f_OtherTitle = value; }
        }
        string _f_author = string.Empty;
        /// <summary>
        /// 作者
        /// </summary>
        public string F_Author
        {
            get { return _f_author; }
            set { _f_author = value; }
        }
        string _f_StartPage = string.Empty;
        /// <summary>
        /// 起始页
        /// </summary>
        public string F_StartPage
        {
            get { return _f_StartPage; }
            set { _f_StartPage = value; }
        }
        string _f_EndPage = string.Empty;
        /// <summary>
        /// 结束页
        /// </summary>
        public string F_EndPage
        {
            get { return _f_EndPage; }
            set { _f_EndPage = value; }
        }
        string _f_TotalPage = string.Empty;
        /// <summary>
        /// 页数
        /// </summary>
        public string F_TotalPage
        {
            get { return _f_TotalPage; }
            set { _f_TotalPage = value; }
        }
        string _f_year = string.Empty;
        /// <summary>
        /// 出版年、发表年、会议年等
        /// </summary>
        public string F_year
        {
            get { return _f_year; }
            set { _f_year = value; }
        }
        string _f_Volume = string.Empty;
        /// <summary>
        /// 刊的卷
        /// </summary>
        public string F_Volume
        {
            get { return _f_Volume; }
            set { _f_Volume = value; }
        }
        string _f_Issue = string.Empty;
        /// <summary>
        /// 刊的期
        /// </summary>
        public string F_Issue
        {
            get { return _f_Issue; }
            set { _f_Issue = value; }
        }
        string _f_Issn = string.Empty;
        /// <summary>
        /// ISSN
        /// </summary>
        public string F_Issn
        {
            get { return _f_Issn; }
            set { _f_Issn = value; }
        }
        string _f_PeriodicalName = string.Empty;
        /// <summary>
        /// 母体名
        /// </summary>
        public string F_PeriodicalName
        {
            get { return _f_PeriodicalName; }
            set { _f_PeriodicalName = value; }
        }
        string _f_ReqNum = string.Empty;
        /// <summary>
        /// 馆藏号
        /// </summary>
        public string F_ReqNum
        {
            get { return _f_ReqNum; }
            set { _f_ReqNum = value; }
        }
        string _f_总期 = string.Empty;
        /// <summary>
        /// 仓储中没有的字段，但北邮系统中有，选定是否实际应用
        /// </summary>
        public string F_总期
        {
            get { return _f_总期; }
            set { _f_总期 = value; }
        }
        public PeriodicalEng() { }
        /// <summary>
        /// 用序列化后的XML构造类
        /// </summary>
        public PeriodicalEng(string xml)
        {
            try
            {
                XmlSerializer xser = new XmlSerializer(typeof(PeriodicalEng));
                System.IO.StringReader stream = new System.IO.StringReader(xml);
                PeriodicalEng obj = (PeriodicalEng)xser.Deserialize(stream);
                stream.Close();
                stream.Dispose();
                this._f_author = obj._f_author;
                this._f_DOI = obj._f_DOI;
                this._f_EndPage = obj._f_EndPage;
                this._f_Issn = obj._f_Issn;
                this._f_Issue = obj._f_Issue;
                this._f_OtherTitle = obj._f_OtherTitle;
                this._f_PeriodicalName = obj._f_PeriodicalName;
                this._f_qcode = obj._f_qcode;
                this._f_ReqNum = obj._f_ReqNum;
                this._f_StartPage = obj._f_StartPage;
                this._f_title = obj._f_title;
                this._f_TotalPage = obj._f_TotalPage;
                this._f_UniqueId = obj._f_UniqueId;
                this._f_Volume = obj._f_Volume;
                this._f_year = obj._f_year;
                this._f_总期 = obj._f_总期;
            }
            catch(Exception err)
            {
                throw err;
            }
        }
        public override string ToString()
        {
            XmlSerializer xser = new XmlSerializer(typeof(PeriodicalEng));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            xser.Serialize(ms, this);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
