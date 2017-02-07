using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.FinanceReport
{
    public class BalanceSheetDto
    {
        public List<CategoryReportDto> objAssetList { get; set; }

        public List<CategoryReportDto> objLiablityList { get; set; }

        public List<CategoryReportDto> objEquityList { get; set; }

        public List<CategoryReportDto> objSecondAssetList { get; set; }

        public List<CategoryReportDto> objSecondLiablityList { get; set; }

        public List<CategoryReportDto> objSecondEquityList { get; set; }

    }
    public class BalanceChildModel
    {
        public Nullable<int> accountClassificationId { get; set; }
        public int ClassificationID { get; set; }
        public int ClassificationChartAccountNumber { get; set; }
        public string ClassificationName { get; set; }
        public string ClassificationType { get; set; }
        public decimal GrossTotal { get; set; }
        public string TransactionType { get; set; }
        public string AccountName { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public bool IsBankEntry { get; set; }
        public int ModelNumber { get; set; }
        public decimal TotalBankBalance { get; set; }
        public int ClassificationId { get; set; }
        public int ClassificationHeading { get; set; }
        public int ClassificationSubHeading { get; set; }
        public int Year { get; set; }
    }
}
