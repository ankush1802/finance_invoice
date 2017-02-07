using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.FinanceReport
{
    [DataContract]
    public class CategoryReportDto
    {
        [DataMember]
        public int ClassificationChartAccountNumber { get; set; }
        [DataMember]
        public string ClassificationName { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
        [DataMember]
        public decimal GrossTotal { get; set; }
        [DataMember]
        public string DisplayClassificationChartAccountNumber { get; set; }
        [DataMember]
        public int ReportingTotalNumber { get; set; }
        [DataMember]
        public string ReportingTotalDisplayNumber { get; set; }

        [DataMember]
        public string ReportingTotalClassification { get; set; }

        public string ReportingSubTotalClassification { get; set; }
       

    }
}
