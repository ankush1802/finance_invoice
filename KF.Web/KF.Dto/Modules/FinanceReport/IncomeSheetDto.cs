using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.FinanceReport
{
    [DataContract]
    public class IncomeSheetDto : ApiResponseDto
    {
        [DataMember]
        public List<CategoryReportDto> objExpenseList { get; set; }
        [DataMember]
        public List<CategoryReportDto> objRevenueList { get; set; }
        [DataMember]
        public decimal NetIncome1 { get; set; }

        public List<CategoryReportDto> objSecondExpenseList { get; set; }

        public List<CategoryReportDto> objSecondRevenueList { get; set; }

        public decimal NetIncome2 { get; set; }
    }

    [DataContract]
    public class IncomeChildModel
    {
        [DataMember]
        public int ClassificationChartAccountNumber { get; set; }
        [DataMember]
        public string ClassificationName { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
        [DataMember]
        public decimal GrossTotal { get; set; }

        public int ModelNumber { get; set; }
    }
}
