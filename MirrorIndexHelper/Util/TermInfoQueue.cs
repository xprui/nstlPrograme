using Lucene.Net.Util;
using System;

namespace LuceneHelper
{
    public class TermInfoQueue : Lucene.Net.Util.PriorityQueue
    {
        public TermInfoQueue(int size)
        {
            base.Initialize(size);
        }

        public override bool LessThan(object a, object b)
        {
            TermModle modle = (TermModle)a;
            TermModle modle2 = (TermModle)b;
            return (modle.Count < modle2.Count);
        }
    }
}
