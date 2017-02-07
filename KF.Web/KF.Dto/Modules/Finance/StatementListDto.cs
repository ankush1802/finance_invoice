using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
   public class StatementListDto
    {
       public List<StatementList> StatementList { get; set; }
       public Int32 TotalCount { get; set; }
    }
   public class StatementList
   {
       public Int32 Id { get; set; }
       public String JVID { get; set; }
       public String StatementDate { get; set; }
       public String StatementDescription { get; set; }
       public Nullable<decimal> Credit { get; set; }
       public Nullable<decimal> Debit { get; set; }
       public String StatementClassification { get; set; }
       public String StatementBank { get; set; }
       public String StatementUploadType { get; set; }
       public String StatementStatus { get; set; }
   }
}
