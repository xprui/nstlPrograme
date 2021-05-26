using Lucene.Net.Analysis;
using Lucene.Net.Analysis.KTDictSeg;
using Lucene.Net.Analysis.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper
{
    /// <summary>
    /// 20180313 新增自动选择 分词判断类 Luyg
    /// </summary>
    class AnalyzerChEn
    {
        /// <summary>
        /// 分词判断用选择的视图名，只添加中文的视图，其他默认视为英文对应的分词标准
        /// </summary>
        /// <param name="tableName">视图名</param>
        /// <returns></returns>
        private String judgeChEn(String tableName) {
            switch (tableName) {
                case "View_Conference_zho":                   //中文会议
                    par = "ch" ;
                    break;
                case "View_Standard":                         //中文标准
                    par = "ch" ;
                    break;
                case "View_Patent":                           //中文专利
                    par = "ch" ;
                    break;
                default :
                    par = "en" ;
                    break ;
            }
            return par;
        }

        private Analyzer analyzerChoose(String parameter) {

            if (parameter.Equals("ch"))
            {
                analyzerSet = new KTDictSegAnalyzer();
            }
            else {
                analyzerSet = new StandardAnalyzer();
            }
            return analyzerSet;
        }

        public  String getAnalyzerValue(String tableN) {
            return judgeChEn(tableN);
        }
        public Analyzer getAnalyzerChoose(String tableN) {
            return analyzerChoose(judgeChEn(tableN));
        }

        Lucene.Net.Analysis.Analyzer analyzerSet = null;
        public String par = null;
    }
}
