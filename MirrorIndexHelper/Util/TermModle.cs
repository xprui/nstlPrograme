using Lucene.Net.Index;
using System;
using System.Runtime.CompilerServices;

namespace LuceneHelper
{
    public class TermModle
    {
        [CompilerGenerated]
        private int Count__BackingField;
        [CompilerGenerated]
        private Lucene.Net.Index.Term Term__BackingField;

        public TermModle(Lucene.Net.Index.Term term, int count)
        {
            this.Term = term;
            this.Count = count;
        }

        public int Count
        {
            [CompilerGenerated]
            get
            {
                return this.Count__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.Count__BackingField = value;
            }
        }

        public Lucene.Net.Index.Term Term
        {
            [CompilerGenerated]
            get
            {
                return this.Term__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.Term__BackingField = value;
            }
        }
    }
}
