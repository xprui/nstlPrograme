using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Util;
using Lucene.Net.Analysis;
namespace Istic.NSTL
{
    public class NstlAnalyzer : Analyzer
    {
        private static readonly string[] stopWords = 
        {
            "$", "£",
            "about", "after", "all", "also", "an", "and",
            "another", "any", "are", "as", "at", "be",
            "because", "been", "before", "being", "between",
            "both", "but", "by", "came", "can", "come",
            "could", "did", "do", "does", "each", "else",
            "for", "from", "get", "got", "has", "had",
            "he", "have", "her", "here", "him", "himself",
            "his", "how","if", "in", "into", "is", "it",
            "its", "just", "like", "make", "many", "me",
            "might", "more", "most", "much", "must", "my",
            "never", "now", "of", "on", "only", "or",
            "other", "our", "out", "over", "re", "said",
            "same", "see", "should", "since", "so", "some",
            "still", "such", "take", "than", "that", "the",
            "their", "them", "then", "there", "these",
            "they", "this", "those", "through", "to", "too",
            "under", "up", "use", "very", "want", "was",
            "way", "we", "well", "were", "what", "when",
            "where", "which", "while", "who", "will",
            "with", "would", "you", "your",
            "a", "b", "c", "d", "e", "f", "g", "h", "i",
            "j", "k", "l", "m", "n", "o", "p", "q", "r",
            "s", "t", "u", "v", "w", "x", "y", "z"
        };

        //private Hashtable stopTable;

        //public NstlAnalyzer():this(stopWords){}

        //public NstlAnalyzer(string[] stopWords)
        //{
        //    stopTable = StopFilter.MakeStopSet(stopWords);
        //}
        /*
         StandardAnalyzer analyzer = new StandardAnalyzer();
            TokenStream ts = analyzer.TokenStream("test", new StringReader("Agriculture Agricultural Agriculturalization played happiness play 2000"));//Agriculture Agricultural Agriculturalization
            ts = new PorterStemFilter(ts);
            while (ts.IncrementToken())
            {

                TermAttribute ta = (TermAttribute)(ts.GetAttribute(typeof(TermAttribute)));

                Console.WriteLine(ta.Term());
            }
            ts.End();
            ts.Close();
         */

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            //return new PorterStemFilter
            //(
            //    //new ASCIIFoldingFilter
            //    //(
            //        //new StopFilter
            //        //(
            //            new LowerCaseTokenizer
            //            (
            //                reader
            //            )
            //        //,stopWords
            //        //)
            //    //)
            //);

            Lucene.Net.Analysis.Standard.StandardAnalyzer analyzer;
            //if (null != _indexVersion)
            analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer();
            //else
            //    analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);

            return new PorterStemFilter(analyzer.TokenStream(fieldName,reader));
        }

    }
}
