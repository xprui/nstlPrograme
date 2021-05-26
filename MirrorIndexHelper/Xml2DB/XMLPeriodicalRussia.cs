using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    public class XMLPeriodicalRussia : XMLDocument
    {
        public override string UpdateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("update {0} set ");
            sb.AppendLine(" F_Doc_Id=[F_Content].value('(//doc_id)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_Classification=[F_Content].value('(//classification)[1]','nvarchar(50)'), ");
            //sb.AppendLine(" F_library_code=[F_Content].value('(//proceeding/holdinglist/holding/library_code)[1]','nvarchar(50)'), ");//20201229 入库为空，后续需要update还是改代码吧，多条件判断选取
            sb.AppendLine(" F_library_code=  isnull( REPLACE(STUFF(CONVERT(nvarchar(50), [F_Content].query('for $id in //holdinglist/holding/library_code return concat(\"###@@@;\",$id)'), 0), 1, 7, ''), ' ###@@@;', ';'),REPLACE(STUFF(CONVERT(nvarchar(50), F_Content.query('for $id in //hasHoldings/hasHolding/libraryCode return concat(\"###@@@;\",$id)'), 0), 1, 7, ''), ' ###@@@;', ';') ), ");
            sb.AppendLine(" F_JournalCode=[F_Content].value('(//paper/journal/catalog_code)[1]','nvarchar(50)'), ");//20201229 新增
            sb.AppendLine(" F_Title=[F_Content].value('(//title)[1]','nvarchar(450)') ");
            sb.AppendLine(" where F_Batch='{1}'");
            return sb.ToString();
        }
    }
}
