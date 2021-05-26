/**********************************************
 * ����KTDictSeg��Lucene.Net ���ķִʷ�����
 * �ο� suyuan �Ŀ�Դ�����޸�
 * suyuan �Ŀ�Դ������� http://www.cnblogs.com/suyuan/archive/2008/03/25/1120827.html
 *********************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Lucene.Net.Analysis.KTDictSeg
{
    public class KTDictSegAnalyzer : Analyzer
    {
        private static Stopwatch m_Duration = new Stopwatch();

        private bool _OriginalResult = false;

        /// <summary>
        /// ͳ�Ʒִ�ռ��ʱ��
        /// </summary>
        public static long Duration
        {
            get
            {
                return m_Duration.ElapsedMilliseconds;
            }

            set
            {
                m_Duration.Reset();
            }
        }

        public KTDictSegAnalyzer()
        {
        }

        /// <summary>
        /// Return original string.
        /// Does not use only segment
        /// </summary>
        /// <param name="originalResult"></param>
        public KTDictSegAnalyzer(bool originalResult)
        {
            _OriginalResult = originalResult;
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
#if DEBUG
                m_Duration.Start();
#endif
            TokenStream result = new KTDictSegTokenizer(reader, _OriginalResult);
#if DEBUG
                m_Duration.Stop();
#endif
            result = new LowerCaseFilter(result);
            return result;
        }
    }


}
