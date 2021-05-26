/**********************************************
 * 采用KTDictSeg的Lucene.Net 中文分词分析器
 * 参考 suyuan 的开源代码修改
 * suyuan 的开源代码出处 http://www.cnblogs.com/suyuan/archive/2008/03/25/1120827.html
 *********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using KTDictSeg;
using FTAlgorithm;
using Lucene.Net.Analysis;

namespace Lucene.Net.Analysis.KTDictSeg
{
    public class KTDictSegTokenizer : Tokenizer
    {
        private static object m_LockObj = new object();

        private static CSimpleDictSeg m_SimpleDictSeg;

        private List<T_WordInfo> m_WordList = new List<T_WordInfo>();
        private int m_Position = -1; //词汇在缓冲中的位置.
        private bool _OriginalResult = false;
        string _InputText;

        private string GetAssemblyPath()
        {
            const string _PREFIX = @"file:///";
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            codeBase = codeBase.Substring(_PREFIX.Length, codeBase.Length - _PREFIX.Length).Replace("/", "\\");
            return System.IO.Path.GetDirectoryName(codeBase) + @"\";
        }

        private void InitSimpleDictSeg()
        {
            //Init SimpleDictSeg.
            if (m_SimpleDictSeg == null)
            {
                try
                {
                    m_SimpleDictSeg = new CSimpleDictSeg();
                    m_SimpleDictSeg.LoadConfig(GetAssemblyPath() + "KTDictSeg.xml");

                    m_SimpleDictSeg.LoadDict();
                }
                catch (Exception e)
                {
                    m_SimpleDictSeg = null;
                    throw e;
                }
            }
        }

        public KTDictSegTokenizer(System.IO.TextReader input, bool originalResult):this(input)
        {
            _OriginalResult = originalResult;
        }

        public KTDictSegTokenizer()
        {
            lock (m_LockObj)
            {
                InitSimpleDictSeg();
            }
        }

        public KTDictSegTokenizer(System.IO.TextReader input)
            : base(input) 
        {
            lock (m_LockObj)
            {
                InitSimpleDictSeg();
            }

            _InputText = input.ReadToEnd();

            if (string.IsNullOrEmpty(_InputText))
            {
                char[] readBuf = new char[1024];

                int relCount = input.Read(readBuf, 0, readBuf.Length);

                StringBuilder inputStr = new StringBuilder(readBuf.Length);


                while (relCount > 0)
                {
                    inputStr.Append(readBuf, 0, relCount);

                    relCount = input.Read(readBuf, 0, readBuf.Length);
                }

                if (inputStr.Length > 0)
                {
                    _InputText = inputStr.ToString();
                }
            }



            lock (m_LockObj)
            {
                m_WordList = m_SimpleDictSeg.SegmentToWordInfos(_InputText);
            }
        }

        //DotLucene的分词器简单来说，就是实现Tokenizer的Next方法，把分解出来的每一个词构造为一个Token，因为Token是DotLucene分词的基本单位。
        public override Token Next()
        {
            if (_OriginalResult)
            {
                string retStr = _InputText;
                
                _InputText = null;

                if (retStr == null)
                {
                    return null;
                }

                return new Token(retStr, 0, retStr.Length);
            }

            int length = 0;    //词汇的长度.
            int start = 0;     //开始偏移量.

            while (true)
            {
                m_Position++;
                if (m_Position < m_WordList.Count)
                {
                    if (m_WordList[m_Position] != null)
                    {
                        length = m_WordList[m_Position].Word.Length;
                        start = m_WordList[m_Position].Position;
                        return new Token(m_WordList[m_Position].Word, start, start + length);
                    }
                }
                else
                {
                    break;
                }
            }

            _InputText = null;
            return null;
        }

        public List<T_WordInfo> SegmentToWordInfos(String str)
        {
            lock (m_LockObj)
            {
                return m_SimpleDictSeg.SegmentToWordInfos(str);
            }
        }

        public List<String> Segment(String str)
        {
            lock (m_LockObj)
            {
                return m_SimpleDictSeg.Segment(str);
            }
        }
    }

}
