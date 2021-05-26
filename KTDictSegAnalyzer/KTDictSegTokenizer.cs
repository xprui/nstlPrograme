/**********************************************
 * ����KTDictSeg��Lucene.Net ���ķִʷ�����
 * �ο� suyuan �Ŀ�Դ�����޸�
 * suyuan �Ŀ�Դ������� http://www.cnblogs.com/suyuan/archive/2008/03/25/1120827.html
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
        private int m_Position = -1; //�ʻ��ڻ����е�λ��.
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

        //DotLucene�ķִ�������˵������ʵ��Tokenizer��Next�������ѷֽ������ÿһ���ʹ���Ϊһ��Token����ΪToken��DotLucene�ִʵĻ�����λ��
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

            int length = 0;    //�ʻ�ĳ���.
            int start = 0;     //��ʼƫ����.

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
