using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorIndexHelper.Xml2DB
{
    public class XMLPeriodicalEng : XMLDocument
    {
        public override string UpdateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("update {0} set ");
            //sb.AppendLine(" F_Doc_Id=[F_Content].value('(//doc_id)[1]','nvarchar(50)'), ");//20201130  4楼说统一使用新的nstl_doc_id（doc_id 旧数据更新时还是使用旧的，只有新数据才使用新的）。
            sb.AppendLine(" F_Doc_Id=[F_Content].value('(//nstl_doc_id)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_Classification=[F_Content].value('(//classification)[1]','nvarchar(50)'), ");
            //sb.AppendLine(" F_library_code=[F_Content].value('(//proceeding/holdinglist/holding/library_code)[1]','nvarchar(50)'), ");  //20201214  没有节点 proceeding 
            //sb.AppendLine(" F_library_code=[F_Content].value('(//holdinglist/holding/library_code)[1]','nvarchar(50)'), ");  //
            sb.AppendLine("F_library_code=replace(stuff(convert(nvarchar(400),[F_Content].query('for $id in //holdinglist/holding/library_code return concat(\"###@@@;\",$id)'), 0), 1, 7, ''), ' ###@@@;', ';'),");
            sb.AppendLine(" F_JournalCode=[F_Content].value('(//paper/journal/catalog_code)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_Year=[F_Content].value('(//paper/issue/year)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_ISSN=[F_Content].value('(//paper/journal/issn)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_Holdnumber=[F_Content].value('(//paper/issue/holdinglist/holding/holding_number)[1]','nvarchar(50)'), ");
            sb.AppendLine(" F_Title=[F_Content].value('(//title)[1]','nvarchar(450)') ");
            sb.AppendLine(" where F_Batch='{1}'");
            return sb.ToString();
        }
    }
}
