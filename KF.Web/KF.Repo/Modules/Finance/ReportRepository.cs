using KF.Dto.Modules.FinanceReport;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Finance
{
    public class ReportRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Trial Balance
        public List<TrialBalanceDto> TrialBalanceList(int userId, DateTime startDate, DateTime endDate, int? JVID, string ChartAccountNumber)
        {
            List<TrialBalanceDto> trialBalanceList = new List<TrialBalanceDto>();

            List<TrialBalanceDto> trialBalanceTemporaryAccountList = new List<TrialBalanceDto>();

            List<TrialBalanceTaxDto> trialBalanceTemporaryTaxList = new List<TrialBalanceTaxDto>();
            try
            {
                using (var db = new KFentities())
                {
                    #region Local Variable
                    List<BankExpense> bankStatementData = new List<BankExpense>();
                    List<BankExpense> bankAccountStatementData = new List<BankExpense>();
                    #endregion

                    #region Get Bank Statement Data From Database
                    // ClassificationID is not 1 means statement is not reconcilled
                    //categoryID 6 means not PLEASE SELECT
                    // Also we dont want to fetch the account based classification record thats why we check in classification table whose user id == null
                    bankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.UserId == userId && i.ClassificationId != 1 && i.StatusId == 4 &&
                      i.Classification.ChartAccountDisplayNumber != "1050-1060" && i.Classification.ChartAccountDisplayNumber != "1050-1061" &&
                            i.Classification.ChartAccountDisplayNumber != "1050-1062" && i.Classification.ChartAccountDisplayNumber != "1050-1063"
                            && i.Classification.ChartAccountDisplayNumber != "1050-1064" && i.Classification.ChartAccountDisplayNumber != "1050-1073"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1080" && i.Classification.ChartAccountDisplayNumber != "1100-1081"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1082" && i.Classification.ChartAccountDisplayNumber != "1100-1083"
                             && i.Classification.ChartAccountDisplayNumber != "1100-1087" && i.Classification.ChartAccountDisplayNumber != "1100-1088"
                              && i.Classification.ChartAccountDisplayNumber != "1100-1089"
                    && i.Classification.ChartAccountDisplayNumber != "3550-3561"
                    && i.CategoryId != 6 && i.Date >= startDate && i.Date <= endDate).ToList();

                    //Get Account Based Entry
                    bankAccountStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.UserId == userId && i.ClassificationId != 1 && i.StatusId == 4 &&
                        i.Classification.UserId == userId && i.Classification.ChartAccountDisplayNumber != "3550-3561"
                        && (i.Classification.ChartAccountDisplayNumber == "1050-1060" || i.Classification.ChartAccountDisplayNumber == "1050-1061"
                         || i.Classification.ChartAccountDisplayNumber == "1050-1062" || i.Classification.ChartAccountDisplayNumber == "1050-1063"
                         || i.Classification.ChartAccountDisplayNumber == "1050-1064" || i.Classification.ChartAccountDisplayNumber == "1050-1073"
                         || i.Classification.ChartAccountDisplayNumber == "1100-1080" || i.Classification.ChartAccountDisplayNumber == "1100-1081"
                         || i.Classification.ChartAccountDisplayNumber == "1100-1082" || i.Classification.ChartAccountDisplayNumber == "1100-1083"
                         || i.Classification.ChartAccountDisplayNumber == "1100-1087" || i.Classification.ChartAccountDisplayNumber == "1100-1088"
                         || i.Classification.ChartAccountDisplayNumber == "1100-1089")
                             && i.CategoryId != 6 && i.Date >= startDate && i.Date <= endDate).ToList();

                    if (JVID > 0)
                    {
                        bankStatementData = bankStatementData.Where(f => f.JVID.Equals(JVID)).ToList();
                        bankAccountStatementData = bankAccountStatementData.Where(f => f.JVID.Equals(JVID)).ToList();
                    }
                    if (!string.IsNullOrEmpty(ChartAccountNumber))
                    {
                        bankStatementData = bankStatementData.Where(f => f.Classification.ChartAccountDisplayNumber == ChartAccountNumber).ToList();
                        bankAccountStatementData = bankAccountStatementData.Where(f => f.Classification.ChartAccountDisplayNumber == ChartAccountNumber).ToList();
                    }
                    #endregion

                    #region Bind Normal Bank data
                    var NormalBankData = bankStatementData.OrderBy(s => s.Date).GroupBy(s => s.ClassificationId).ToList();
                    foreach (var item in NormalBankData)
                    {
                        var classificationDetails = db.Classifications.Where(s => s.Id == item.Key).FirstOrDefault();

                        TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                        trialBalanceRecord.ClassificationID = item.Key;
                        trialBalanceRecord.ClassificationName = classificationDetails.ClassificationType;
                        trialBalanceRecord.ChartAccountNumber = classificationDetails.ChartAccountNumber;
                        trialBalanceRecord.DisplayChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;

                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                        decimal ClassificationBalance = 0;
                        decimal ClassificationCreditTotal = 0;
                        decimal ClassificationDebitTotal = 0;
                        var statementList = item.OrderBy(s => s.Date).ToList();
                        foreach (var rowItem in statementList)
                        {
                            if (rowItem.Credit == null)
                                rowItem.Credit = 0;
                            else if (rowItem.Debit == null)
                                rowItem.Debit = 0;

                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();

                            //if (rowItem.StatementReferenceNumber != null)
                            //  rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementReferenceNumber);
                            //else
                            rowItemRecord.JVID = Convert.ToInt32(rowItem.JVID);

                            if (rowItem.AccountType == "MJV")
                                rowItemRecord.Description = rowItem.Comments;
                            else
                                rowItemRecord.Description = rowItem.Description;

                            rowItemRecord.Source = rowItem.Bank.BankName;
                            rowItemRecord.UploadType = rowItem.UploadType;
                            rowItemRecord.Date = rowItem.Date.Value.Date.ToString("dd/MM/yyyy");
                            // var ocrExpenseData = rowItem.OcrExpenseDetails.FirstOrDefault();
                            if (rowItem.Credit > 0)
                            {
                                if (rowItem.BankId != 8)
                                {
                                    ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.ActualTotal));
                                    ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(rowItem.ActualTotal));
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.ActualTotal), 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    //INV entry
                                    ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.Total));
                                    ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(rowItem.Total));
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Total), 2, MidpointRounding.AwayFromZero);
                                }

                            }
                            else if (rowItem.Debit > 0)
                            {
                                if (rowItem.BankId != 8)
                                {
                                    ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(rowItem.ActualTotal));
                                    ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(rowItem.ActualTotal));
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.ActualTotal), 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(rowItem.Total));
                                    ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(rowItem.Total));
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Total), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                            if (ClassificationBalance > 0)
                                rowItemRecord.TransactionType = "Cr.";
                            else
                                rowItemRecord.TransactionType = "Dr.";

                            rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                            rowItemRecord.StatementDate = rowItem.Date;

                            //Add row item correspond to classificationID
                            trialBalanceRowItems.Add(rowItemRecord);
                        }
                        trialBalanceRecord.objrowItemList = trialBalanceRowItems;
                        trialBalanceRecord.ClassificationDebitTotal = ClassificationDebitTotal;
                        trialBalanceRecord.ClassificationCreditTotal = ClassificationCreditTotal;

                        //Add classification and therie row items to list
                        trialBalanceList.Add(trialBalanceRecord);

                    }
                    #endregion

                    #region Bind Account Bank data
                    #region Bind Data for virtual entries
                    var NormalBankDataForAccount = bankStatementData.Where(a => a.AccountClassificationId != 1030).OrderBy(s => s.Date).GroupBy(s => s.AccountClassificationId).ToList();
                    foreach (var item in NormalBankDataForAccount)
                    {
                        TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                        switch (item.Key)
                        {
                            case 1060:
                                trialBalanceRecord.ClassificationID = 1060;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1060" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501060;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1060";
                                break;
                            case 1061:
                                trialBalanceRecord.ClassificationID = 1061;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1061" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501061;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1061";
                                break;
                            case 1062:
                                trialBalanceRecord.ClassificationID = 1062;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1062" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501062;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1062";
                                break;
                            case 1063:
                                trialBalanceRecord.ClassificationID = 1063;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1063" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501063;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1063";
                                break;
                            case 1064:
                                trialBalanceRecord.ClassificationID = 1064;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1064" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501064;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1064";
                                break;
                            case 1073:
                                trialBalanceRecord.ClassificationID = 1073;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1073" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 10501073;
                                trialBalanceRecord.DisplayChartAccountNumber = "1050-1073";
                                break;
                            case 1080:
                                trialBalanceRecord.ClassificationID = 1080;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1080" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001080;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1080";
                                break;
                            case 1081:
                                trialBalanceRecord.ClassificationID = 1081;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1081" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001081;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1081";
                                break;
                            case 1082:
                                trialBalanceRecord.ClassificationID = 1082;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1082" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001082;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1082";
                                break;
                            case 1083:
                                trialBalanceRecord.ClassificationID = 1083;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1083" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001083;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1083";
                                break;
                            case 1087:
                                trialBalanceRecord.ClassificationID = 1087;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1087" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001087;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1087";
                                break;
                            case 1088:
                                trialBalanceRecord.ClassificationID = 1088;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1088" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001088;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1088";
                                break;
                            case 1089:
                                trialBalanceRecord.ClassificationID = 1089;
                                trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1089" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                trialBalanceRecord.ChartAccountNumber = 11001089;
                                trialBalanceRecord.DisplayChartAccountNumber = "1100-1089";
                                break;
                        }

                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();

                        var statementList = item.OrderBy(s => s.Date).ToList();
                        foreach (var rowItem in statementList)
                        {
                            if (rowItem.Credit == null)
                                rowItem.Credit = 0;
                            else if (rowItem.Debit == null)
                                rowItem.Debit = 0;

                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();

                            //if (rowItem.StatementReferenceNumber != null)
                            //  rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementReferenceNumber);
                            //else
                            rowItemRecord.JVID = Convert.ToInt32(rowItem.JVID);

                            if (rowItem.AccountType == "MJV")
                                rowItemRecord.Description = rowItem.Comments;
                            else
                                rowItemRecord.Description = rowItem.Description;

                            rowItemRecord.Source = rowItem.Bank.BankName;
                            rowItemRecord.UploadType = rowItem.UploadType;
                            rowItemRecord.Date = rowItem.Date.Value.Date.ToString("dd/MM/yyyy");
                            //  var ocrExpenseData = rowItem.OcrExpenseDetails.FirstOrDefault();
                            if (rowItem.Credit > 0)
                            {
                                //reverse the virtual entry
                                rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Credit), 2, MidpointRounding.AwayFromZero);
                                rowItemRecord.Credit = 0;
                            }
                            else if (rowItem.Debit > 0)
                            {
                                rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Debit), 2, MidpointRounding.AwayFromZero);
                                rowItemRecord.Debit = 0;
                            }

                            rowItemRecord.StatementDate = rowItem.Date;

                            //Add row item correspond to classificationID
                            trialBalanceRowItems.Add(rowItemRecord);
                        }
                        trialBalanceRecord.objrowItemList = trialBalanceRowItems;

                        //Add classification and their row items to list
                        trialBalanceTemporaryAccountList.Add(trialBalanceRecord);

                    }
                    #endregion

                    #region Bind Account entry to virtual entry list
                    // var classificationListCheck = bankAccountStatementData.Select(d => d.Classifications.ChartAccountNumber.Substring(5, 4)).ToList();
                    var AccountBankData = bankAccountStatementData.OrderBy(s => s.Date).GroupBy(s => s.Classification.ChartAccountDisplayNumber.Substring(5, 4)).ToList();
                    foreach (var accRow in AccountBankData)
                    {
                        int ClassificationID = Convert.ToInt32(accRow.Key);
                        if (trialBalanceTemporaryAccountList.Where(s => s.ClassificationID == ClassificationID).Any())
                        {
                            //add data to existing list
                            var existingData = trialBalanceTemporaryAccountList.Where(s => s.ClassificationID == ClassificationID).FirstOrDefault();
                            foreach (var accRowItem in accRow.ToList())
                            {
                                if (accRowItem.Credit == null)
                                    accRowItem.Credit = 0;
                                else if (accRowItem.Debit == null)
                                    accRowItem.Debit = 0;

                                TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();

                                //if (rowItem.StatementReferenceNumber != null)
                                //  rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementReferenceNumber);
                                //else
                                rowItemRecord.JVID = Convert.ToInt32(accRowItem.JVID);

                                if (accRowItem.AccountType == "MJV")
                                    rowItemRecord.Description = accRowItem.Comments;
                                else
                                    rowItemRecord.Description = accRowItem.Description;

                                rowItemRecord.Source = accRowItem.Bank.BankName;
                                rowItemRecord.UploadType = accRowItem.UploadType;
                                rowItemRecord.Date = accRowItem.Date.Value.Date.ToString("dd/MM/yyyy");
                                // var ocrExpenseData = accRowItem.OcrExpenseDetails.FirstOrDefault();
                                if (accRowItem.Credit > 0)
                                {
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(accRowItem.Credit), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (accRowItem.Debit > 0)
                                {
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(accRowItem.Debit), 2, MidpointRounding.AwayFromZero);
                                }

                                rowItemRecord.StatementDate = accRowItem.Date;
                                existingData.objrowItemList.Add(rowItemRecord);
                            }
                        }
                        else
                        {
                            //add new record
                            TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                            switch (ClassificationID)
                            {
                                case 1060:
                                    trialBalanceRecord.ClassificationID = 1060;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1060" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501060;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1060";
                                    break;
                                case 1061:
                                    trialBalanceRecord.ClassificationID = 1061;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1061" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501061;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1061";
                                    break;
                                case 1062:
                                    trialBalanceRecord.ClassificationID = 1062;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1062" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501062;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1062";
                                    break;
                                case 1063:
                                    trialBalanceRecord.ClassificationID = 1063;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1063" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501063;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1063";
                                    break;
                                case 1064:
                                    trialBalanceRecord.ClassificationID = 1064;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1064" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501064;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1064";
                                    break;
                                case 1073:
                                    trialBalanceRecord.ClassificationID = 1073;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1073" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 10501073;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1050-1073";
                                    break;
                                case 1080:
                                    trialBalanceRecord.ClassificationID = 1080;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1080" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001080;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1080";
                                    break;
                                case 1081:
                                    trialBalanceRecord.ClassificationID = 1081;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1081" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001081;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1081";
                                    break;
                                case 1082:
                                    trialBalanceRecord.ClassificationID = 1082;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1082" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001082;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1082";
                                    break;
                                case 1083:
                                    trialBalanceRecord.ClassificationID = 1083;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1083" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001083;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1083";
                                    break;
                                case 1087:
                                    trialBalanceRecord.ClassificationID = 1087;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1087" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001087;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1087";
                                    break;
                                case 1088:
                                    trialBalanceRecord.ClassificationID = 1088;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1088" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001088;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1088";
                                    break;
                                case 1089:
                                    trialBalanceRecord.ClassificationID = 1089;
                                    trialBalanceRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1089" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    trialBalanceRecord.ChartAccountNumber = 11001089;
                                    trialBalanceRecord.DisplayChartAccountNumber = "1100-1089";
                                    break;
                            }

                            List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();

                            var statementList = accRow.OrderBy(s => s.Date).ToList();
                            foreach (var rowItem in statementList)
                            {
                                if (rowItem.Credit == null)
                                    rowItem.Credit = 0;
                                else if (rowItem.Debit == null)
                                    rowItem.Debit = 0;

                                TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();

                                //if (rowItem.StatementReferenceNumber != null)
                                //  rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementReferenceNumber);
                                //else
                                rowItemRecord.JVID = Convert.ToInt32(rowItem.JVID);

                                if (rowItem.AccountType == "MJV")
                                    rowItemRecord.Description = rowItem.Comments;
                                else
                                    rowItemRecord.Description = rowItem.Description;

                                rowItemRecord.Source = rowItem.Bank.BankName;
                                rowItemRecord.UploadType = rowItem.UploadType;
                                rowItemRecord.Date = rowItem.Date.Value.Date.ToString("dd/MM/yyyy");
                                //  var ocrExpenseData = rowItem.OcrExpenseDetails.FirstOrDefault();
                                if (rowItem.Credit > 0)
                                {
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Credit), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (rowItem.Debit > 0)
                                {
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Debit), 2, MidpointRounding.AwayFromZero);
                                }

                                rowItemRecord.StatementDate = rowItem.Date;

                                //Add row item correspond to classificationID
                                trialBalanceRowItems.Add(rowItemRecord);
                            }
                            trialBalanceRecord.objrowItemList = trialBalanceRowItems;

                            //Add classification and their row items to list
                            trialBalanceTemporaryAccountList.Add(trialBalanceRecord);

                        }
                    }
                    #endregion

                    #region Balance Calculation and add temporary list to main list

                    foreach (var accDataList in trialBalanceTemporaryAccountList)
                    {
                        TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                        trialBalanceRecord.ClassificationID = accDataList.ClassificationID;
                        trialBalanceRecord.ChartAccountNumber = accDataList.ChartAccountNumber;
                        trialBalanceRecord.ClassificationName = accDataList.ClassificationName;
                        trialBalanceRecord.DisplayChartAccountNumber = accDataList.DisplayChartAccountNumber;
                        decimal ClassificationBalance = 0;
                        decimal CreditTotalbalance = 0;
                        decimal DebitTotalBalance = 0;
                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                        foreach (var rowItem in accDataList.objrowItemList.OrderBy(d => d.Date).ToList())
                        {
                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                            rowItemRecord.JVID = rowItem.JVID;
                            rowItemRecord.Description = rowItem.Description;

                            rowItemRecord.Source = rowItem.Source;
                            rowItemRecord.UploadType = rowItem.UploadType;
                            rowItemRecord.Date = rowItem.Date;
                            if (rowItem.Credit > 0)
                            {
                                ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.Credit));
                                CreditTotalbalance = Decimal.Add(CreditTotalbalance, Convert.ToDecimal(rowItem.Credit));
                                rowItemRecord.Credit = rowItem.Credit;
                            }
                            else if (rowItem.Debit > 0)
                            {
                                ClassificationBalance = Decimal.Subtract(ClassificationBalance, rowItem.Debit);
                                DebitTotalBalance = Decimal.Add(DebitTotalBalance, rowItem.Debit);
                                rowItemRecord.Debit = rowItem.Debit;
                            }
                            if (ClassificationBalance > 0)
                                rowItemRecord.TransactionType = "Cr.";
                            else
                                rowItemRecord.TransactionType = "Dr.";

                            rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                            rowItemRecord.StatementDate = rowItem.StatementDate;

                            //Add row item correspond to classificationID
                            trialBalanceRowItems.Add(rowItemRecord);
                        }
                        trialBalanceRecord.objrowItemList = trialBalanceRowItems;
                        trialBalanceRecord.ClassificationDebitTotal = DebitTotalBalance;
                        trialBalanceRecord.ClassificationCreditTotal = CreditTotalbalance;

                        //Add classification and their row items to list
                        trialBalanceList.Add(trialBalanceRecord);


                    }
                    #endregion

                    #endregion

                    #region Bind Tax Part For Normal Statements

                    #region Make Temp. Tax List
                    var TaxDataForNormalEntries = db.BankExpenses.Where(i => i.IsDeleted == false && i.UserId == userId && i.ClassificationId != 1 && i.StatusId == 4 &&
                         i.Classification.ChartAccountDisplayNumber != "1050-1060" && i.Classification.ChartAccountDisplayNumber != "1050-1061" &&
                            i.Classification.ChartAccountDisplayNumber != "1050-1062" && i.Classification.ChartAccountDisplayNumber != "1050-1063"
                            && i.Classification.ChartAccountDisplayNumber != "1050-1064" && i.Classification.ChartAccountDisplayNumber != "1050-1073"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1080" && i.Classification.ChartAccountDisplayNumber != "1100-1081"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1082" && i.Classification.ChartAccountDisplayNumber != "1100-1083"
                             && i.Classification.ChartAccountDisplayNumber != "1100-1087" && i.Classification.ChartAccountDisplayNumber != "1100-1088"
                              && i.Classification.ChartAccountDisplayNumber != "1100-1089"
                    && i.Classification.ChartAccountDisplayNumber != "3550-3561"
                    && i.CategoryId != 6 && i.Date >= startDate && i.Date <= endDate).ToList();

                    if (JVID > 0)
                    {
                        TaxDataForNormalEntries = TaxDataForNormalEntries.Where(f => f.JVID.Equals(JVID)).ToList();
                    }
                    if (!string.IsNullOrEmpty(ChartAccountNumber))
                    {
                        TaxDataForNormalEntries = TaxDataForNormalEntries.Where(f => f.Classification.ChartAccountDisplayNumber == ChartAccountNumber).ToList();
                    }

                    foreach (var item in TaxDataForNormalEntries)
                    {
                        if (item.Credit == null)
                            item.Credit = 0;
                        else if (item.Debit == null)
                            item.Debit = 0;

                        // var Ocrdata = item.OcrExpenseDetails.FirstOrDefault();
                        switch (item.CategoryId)
                        {
                            case 3:
                                if (item.HSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2370";
                                    objTax.ClassificationType = "HST Charged on Sales";
                                    objTax.Tax = item.HSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);

                                }
                                if (item.PSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2373";
                                    objTax.ClassificationType = "PST Charged on Sales";
                                    objTax.Tax = item.PSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                if (item.GSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2371";
                                    objTax.ClassificationType = "GST Charged on Sales";
                                    objTax.Tax = item.GSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                if (item.QSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2372";
                                    objTax.ClassificationType = "QST Charged on Sales";
                                    objTax.Tax = item.QSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                break;
                            default:
                                if (item.HSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2375";
                                    objTax.ClassificationType = "HST Paid on Purchases";
                                    objTax.Tax = item.HSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                if (item.PSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2378";
                                    objTax.ClassificationType = "PST Paid on Purchases";
                                    objTax.Tax = item.PSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                if (item.GSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2376";
                                    objTax.ClassificationType = "GST Paid on Purchases";
                                    objTax.Tax = item.GSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                if (item.QSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.CategoryId = Convert.ToInt32(item.CategoryId);
                                    objTax.ChartAccountNumber = "2300-2377";
                                    objTax.ClassificationType = "QST Paid on Purchases";
                                    objTax.Tax = item.QSTtax;
                                    objTax.StatementId = item.JVID == null ? 0 : Convert.ToInt32(item.JVID);
                                    objTax.Comments = item.Description;
                                    objTax.Date = item.Date.Value.Date.ToString("dd/MM/yyyy");
                                    objTax.StatementDate = item.Date;
                                    objTax.BankName = item.Bank.BankName;
                                    objTax.UploadType = item.UploadType;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    trialBalanceTemporaryTaxList.Add(objTax);
                                }
                                break;
                        }
                    }
                    #endregion

                    #region Calculate Balance For Tax Part
                    foreach (var rowTaxItem in trialBalanceTemporaryTaxList.OrderBy(s => s.StatementDate).GroupBy(d => d.ChartAccountNumber).ToList())
                    {
                        if (trialBalanceList.Where(d => d.DisplayChartAccountNumber == rowTaxItem.Key).Any())
                        {
                            //Update tax list in existing liability range
                            var existingData = trialBalanceList.Where(d => d.DisplayChartAccountNumber == rowTaxItem.Key).FirstOrDefault();
                            var taxList = rowTaxItem.ToList();
                            foreach (var rowtaxItem in taxList)
                            {
                                TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                                rowItemRecord.Date = rowtaxItem.Date;
                                if (rowtaxItem.TransactionType == "Cr.")
                                {
                                    rowItemRecord.Credit = Convert.ToDecimal(rowtaxItem.Tax);
                                    rowItemRecord.Debit = 0;
                                }
                                if (rowtaxItem.TransactionType == "Dr.")
                                {
                                    rowItemRecord.Debit = Convert.ToDecimal(rowtaxItem.Tax);
                                    rowItemRecord.Credit = 0;
                                }

                                rowItemRecord.Description = rowtaxItem.Comments;
                                rowItemRecord.UploadType = rowtaxItem.UploadType;
                                rowItemRecord.Source = rowtaxItem.BankName;
                                rowItemRecord.JVID = Convert.ToInt32(rowtaxItem.StatementId);
                                rowItemRecord.StatementDate = rowtaxItem.StatementDate;
                                existingData.objrowItemList.Add(rowItemRecord);
                            }
                            #region Recalculate Balance as new entry added in tax list
                            var subRowItems = existingData.objrowItemList.OrderBy(s => s.StatementDate).ToList();
                            List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                            decimal ClassificationBalance = 0;
                            decimal ClassificationCreditTotal = 0;
                            decimal ClassificationDebitTotal = 0;
                            foreach (var calBal in subRowItems)
                            {
                                TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();

                                rowItemRecord.JVID = calBal.JVID;

                                rowItemRecord.Description = calBal.Description;

                                rowItemRecord.Source = calBal.Source;
                                rowItemRecord.UploadType = calBal.UploadType;
                                rowItemRecord.Date = calBal.Date;
                                if (calBal.Credit > 0)
                                {
                                    ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(calBal.Credit));
                                    ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(calBal.Credit));
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(calBal.Credit), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (calBal.Debit > 0)
                                {
                                    ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(calBal.Debit));
                                    ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(calBal.Debit));
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(calBal.Debit), 2, MidpointRounding.AwayFromZero);
                                }
                                if (ClassificationBalance > 0)
                                    rowItemRecord.TransactionType = "Cr.";
                                else
                                    rowItemRecord.TransactionType = "Dr.";

                                rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                                rowItemRecord.StatementDate = calBal.StatementDate;

                                //Add row item correspond to classificationID
                                trialBalanceRowItems.Add(rowItemRecord);
                            }
                            existingData.ClassificationDebitTotal = ClassificationDebitTotal;
                            existingData.ClassificationCreditTotal = ClassificationCreditTotal;
                            existingData.objrowItemList = trialBalanceRowItems;
                            #endregion
                        }
                        else
                        {
                            //add in main list
                            var classificationDetails = db.Classifications.Where(s => s.ChartAccountDisplayNumber == rowTaxItem.Key).FirstOrDefault();

                            TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                            trialBalanceRecord.ClassificationID = classificationDetails.Id;
                            trialBalanceRecord.ClassificationName = classificationDetails.ClassificationType;
                            trialBalanceRecord.ChartAccountNumber = Convert.ToInt32(classificationDetails.ChartAccountNumber);
                            trialBalanceRecord.DisplayChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;

                            List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                            decimal ClassificationBalance = 0;
                            decimal ClassificationCreditTotal = 0;
                            decimal ClassificationDebitTotal = 0;
                            var statementList = rowTaxItem.OrderBy(s => s.StatementDate).ToList();
                            foreach (var rowItem in statementList)
                            {
                                TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                                rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementId);

                                rowItemRecord.Description = rowItem.Comments;

                                rowItemRecord.Source = rowItem.BankName;
                                rowItemRecord.UploadType = rowItem.UploadType;
                                rowItemRecord.Date = rowItem.Date;
                                if (rowItem.TransactionType == "Cr.")
                                {
                                    ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.Tax));
                                    ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(rowItem.Tax));
                                    rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Tax), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (rowItem.TransactionType == "Dr.")
                                {
                                    ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(rowItem.Tax));
                                    ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(rowItem.Tax));
                                    rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Tax), 2, MidpointRounding.AwayFromZero);
                                }
                                if (ClassificationBalance > 0)
                                    rowItemRecord.TransactionType = "Cr.";
                                else
                                    rowItemRecord.TransactionType = "Dr.";

                                rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                                rowItemRecord.StatementDate = rowItem.StatementDate;

                                //Add row item correspond to classificationID
                                trialBalanceRowItems.Add(rowItemRecord);
                            }
                            trialBalanceRecord.objrowItemList = trialBalanceRowItems;
                            trialBalanceRecord.ClassificationDebitTotal = ClassificationDebitTotal;
                            trialBalanceRecord.ClassificationCreditTotal = ClassificationCreditTotal;

                            //Add classification and therie row items to list
                            trialBalanceList.Add(trialBalanceRecord);
                        }
                    }
                    #endregion

                    #endregion

                    #region Bind Tax Part For Account Statements
                    //MJV's Account entry can dont have tax part as discussed with koko & carlos on 7th dec 2016
                    #endregion

                    #region Add Director Loan Entry To List
                    var directorLoanExistChk = trialBalanceList.Where(s => s.DisplayChartAccountNumber == "1200-0001").FirstOrDefault();
                    var directorDataList = (from bnkexpense in db.BankExpenses
                                            // join classificationdata in db.Classification_new on bnkexpense.ClassificationId equals classificationdata.Id
                                            join director in db.DirectorAccountLogs on bnkexpense.Id equals director.StatementId
                                            //  join ocrexpense in db.OcrExpenseDetails on bnkexpense.Id equals ocrexpense.StatementID
                                            //join bank in db.Banks on bnkexpense.BankId equals bank.Id
                                            where bnkexpense.UserId == userId && bnkexpense.Date >= startDate && bnkexpense.Date <= endDate && bnkexpense.StatusId == 4 
                                            && bnkexpense.BankId != 6 //bank id 6 for MJV
                                            select new
                                            {
                                                bnkexpense.Classification.Id,
                                                bnkexpense.Classification.ClassificationType,
                                                bnkexpense.Classification.ChartAccountNumber,
                                                bnkexpense.Classification.ChartAccountDisplayNumber,
                                                StatementID = bnkexpense.JVID,
                                                bnkexpense.TotalTax,
                                                bnkexpense.Credit,
                                                bnkexpense.Debit,
                                                bnkexpense.ActualTotal,
                                                director.PercentageAmount,
                                                bnkexpense.Description,
                                                bnkexpense.Comments,
                                                bnkexpense.Date,
                                                bnkexpense.UploadType,
                                                bnkexpense.Bank.BankName,
                                                bnkexpense.AccountName,
                                                bnkexpense.AccountClassificationId
                                            }).ToList();
                    //  directorDataList = directorDataList.Where(bnkexpense => bnkexpense.Date >= startDate && bnkexpense.Date <= endDate).ToList();

                    if (JVID > 0)
                    {
                        directorDataList = directorDataList.Where(f => f.StatementID.Equals(JVID)).ToList();
                    }
                    if (!string.IsNullOrEmpty(ChartAccountNumber))
                    {
                        directorDataList = directorDataList.Where(f => f.ChartAccountDisplayNumber == ChartAccountNumber).ToList();
                    }

                    if (directorLoanExistChk != null)
                    {
                        foreach (var rawData in directorDataList.OrderBy(s => s.Date).ToList())
                        {
                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                            rowItemRecord.JVID = Convert.ToInt32(rawData.StatementID);
                            rowItemRecord.Description = rawData.Description;
                            rowItemRecord.Source = rawData.BankName;
                            rowItemRecord.UploadType = rawData.UploadType;
                            rowItemRecord.Date = rawData.Date.Value.Date.ToString("dd/MM/yyyy"); ;
                            if (rawData.Credit > 0 && rawData.Credit != null)
                            {
                                rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rawData.PercentageAmount), 2, MidpointRounding.AwayFromZero);
                            }
                            else if (rawData.Debit > 0 && rawData.Debit != null)
                            {
                                rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rawData.PercentageAmount), 2, MidpointRounding.AwayFromZero);
                            }
                            rowItemRecord.StatementDate = rawData.Date;

                            //Add row item correspond to classificationID
                            directorLoanExistChk.objrowItemList.Add(rowItemRecord);

                        }

                        #region Recalculate Balance for director data
                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                        decimal ClassificationBalance = 0;
                        decimal ClassificationCreditTotal = 0;
                        decimal ClassificationDebitTotal = 0;
                        var statementList = directorLoanExistChk.objrowItemList.OrderBy(s => s.StatementDate).ToList();
                        foreach (var rowItem in statementList)
                        {
                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                            rowItemRecord.JVID = Convert.ToInt32(rowItem.JVID);
                            rowItemRecord.Description = rowItem.Description;
                            rowItemRecord.Source = rowItem.Source;
                            rowItemRecord.UploadType = rowItem.UploadType;
                            rowItemRecord.Date = rowItem.Date;
                            if (rowItem.Credit > 0)
                            {
                                ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.Credit));
                                ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(rowItem.Credit));
                                rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Credit), 2, MidpointRounding.AwayFromZero);
                            }
                            else if (rowItem.Debit > 0)
                            {
                                ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(rowItem.Debit));
                                ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(rowItem.Debit));
                                rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Debit), 2, MidpointRounding.AwayFromZero);
                            }
                            if (ClassificationBalance > 0)
                                rowItemRecord.TransactionType = "Cr.";
                            else
                                rowItemRecord.TransactionType = "Dr.";

                            rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                            rowItemRecord.StatementDate = rowItem.StatementDate;

                            //Add row item correspond to classificationID
                            trialBalanceRowItems.Add(rowItemRecord);
                        }
                        directorLoanExistChk.objrowItemList = trialBalanceRowItems;
                        directorLoanExistChk.ClassificationDebitTotal = ClassificationDebitTotal;
                        directorLoanExistChk.ClassificationCreditTotal = ClassificationCreditTotal;
                        #endregion
                    }
                    else
                    {
                        //add new entry
                        var classificationDetails = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1200-0001").FirstOrDefault();
                        TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                        trialBalanceRecord.ClassificationID = classificationDetails.Id;
                        trialBalanceRecord.ClassificationName = classificationDetails.ClassificationType;
                        trialBalanceRecord.ChartAccountNumber = classificationDetails.ChartAccountNumber;
                        trialBalanceRecord.DisplayChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;

                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                        decimal ClassificationBalance = 0;
                        decimal ClassificationCreditTotal = 0;
                        decimal ClassificationDebitTotal = 0;
                        var statementList = directorDataList.OrderBy(s => s.Date).ToList();
                        foreach (var rowItem in statementList)
                        {
                            TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                            rowItemRecord.JVID = Convert.ToInt32(rowItem.StatementID);
                            rowItemRecord.Description = rowItem.Description;
                            rowItemRecord.Source = rowItem.BankName;
                            rowItemRecord.UploadType = rowItem.UploadType;
                            rowItemRecord.Date = rowItem.Date.Value.Date.ToString("dd/MM/yyyy"); ;
                            if (rowItem.Credit > 0 && rowItem.Credit != null)
                            {
                                ClassificationBalance = Decimal.Add(ClassificationBalance, Convert.ToDecimal(rowItem.PercentageAmount));
                                ClassificationCreditTotal = Decimal.Add(ClassificationCreditTotal, Convert.ToDecimal(rowItem.PercentageAmount));
                                rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.PercentageAmount), 2, MidpointRounding.AwayFromZero);
                            }
                            else if (rowItem.Debit > 0 && rowItem.Debit != null)
                            {
                                ClassificationBalance = Decimal.Subtract(ClassificationBalance, Convert.ToDecimal(rowItem.PercentageAmount));
                                ClassificationDebitTotal = Decimal.Add(ClassificationDebitTotal, Convert.ToDecimal(rowItem.PercentageAmount));
                                rowItemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.PercentageAmount), 2, MidpointRounding.AwayFromZero);
                            }
                            if (ClassificationBalance > 0)
                                rowItemRecord.TransactionType = "Cr.";
                            else
                                rowItemRecord.TransactionType = "Dr.";

                            rowItemRecord.Balance = Math.Abs(ClassificationBalance);
                            rowItemRecord.StatementDate = rowItem.Date;

                            //Add row item correspond to classificationID
                            trialBalanceRowItems.Add(rowItemRecord);
                        }
                        trialBalanceRecord.objrowItemList = trialBalanceRowItems;
                        trialBalanceRecord.ClassificationDebitTotal = ClassificationDebitTotal;
                        trialBalanceRecord.ClassificationCreditTotal = ClassificationCreditTotal;

                        if (trialBalanceRowItems.Count > 0)
                        {
                            //Add classification and therie row items to list
                            trialBalanceList.Add(trialBalanceRecord);
                        }

                    }
                    #endregion

                    #region Add Retained Earning Entry To List
                    //var retainedEarning = db.BankExpenses.Where(s => s.Classification.ChartAccountDisplayNumber == "3550-3561" 
                    //    && s.UserId == userId && s.IsDeleted == false && s.CategoryId != 6
                    //    && s.Date >= startDate && s.Date <= endDate).ToList();
                    var retainedEarning = db.BankExpenses.Where(s => s.Classification.ChartAccountDisplayNumber == "3550-3561"
                        && s.UserId == userId && s.IsDeleted == false && s.CategoryId != 6 && s.StatusId == 4 
                        && s.Date.Value.Year <= startDate.Year).ToList();
                    if (JVID > 0)
                    {
                        retainedEarning = retainedEarning.Where(f => f.JVID.Equals(JVID)).ToList();
                    }
                    if (!string.IsNullOrEmpty(ChartAccountNumber))
                    {
                        retainedEarning = retainedEarning.Where(f => f.Classification.ChartAccountDisplayNumber == ChartAccountNumber).ToList();
                    }
                    if (retainedEarning.Count > 0)
                    {
                        int ReTainedJVID = 0;
                        var classificationDetails = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "3550-3561").FirstOrDefault();
                        TrialBalanceDto trialBalanceRecord = new TrialBalanceDto();
                        trialBalanceRecord.ClassificationID = classificationDetails.Id;
                        trialBalanceRecord.ClassificationName = classificationDetails.ClassificationType;
                        trialBalanceRecord.ChartAccountNumber = classificationDetails.ChartAccountNumber;
                        trialBalanceRecord.DisplayChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;

                        List<TrialBalanceRowItems> trialBalanceRowItems = new List<TrialBalanceRowItems>();
                        decimal ClassificationCreditTotal = 0;
                        decimal ClassificationDebitTotal = 0;

                        TrialBalanceRowItems rowItemRecord = new TrialBalanceRowItems();
                        decimal Balance = 0;
                        foreach (var item in retainedEarning)
                        {
                            if (item.Credit > 0)
                                Balance = Decimal.Add(Balance, Convert.ToDecimal(item.Credit));
                            if (item.Debit > 0)
                                Balance = Decimal.Subtract(Balance, Convert.ToDecimal(item.Debit));

                            ReTainedJVID = Convert.ToInt32(item.JVID);
                        }

                        if (Balance > 0)
                        {
                            rowItemRecord.TransactionType = "Cr.";
                            ClassificationCreditTotal = Math.Abs(Balance);
                            ClassificationDebitTotal = 0;
                            rowItemRecord.Credit = Math.Abs(Balance);
                            rowItemRecord.Balance = Math.Abs(Balance);
                        }
                        else
                        {
                            rowItemRecord.TransactionType = "Dr.";
                            ClassificationDebitTotal = Math.Abs(Balance);
                            ClassificationCreditTotal = 0;
                            rowItemRecord.Debit = Math.Abs(Balance);
                            rowItemRecord.Balance = Math.Abs(Balance);
                        }
                        rowItemRecord.Balance = Math.Abs(Balance);
                        rowItemRecord.Source = db.Banks.Where(s => s.Id == 6).Select(d => d.BankName).FirstOrDefault();
                        DateTime dt = new DateTime(startDate.Year, 01, 01);
                        rowItemRecord.StatementDate = dt.Date;
                        rowItemRecord.Date = dt.Date.ToString("d");
                        rowItemRecord.UploadType = "M";
                        rowItemRecord.JVID = ReTainedJVID;
                        rowItemRecord.Description = classificationDetails.ClassificationType;
                        trialBalanceRowItems.Add(rowItemRecord);
                        trialBalanceRecord.ClassificationDebitTotal = ClassificationDebitTotal;
                        trialBalanceRecord.ClassificationCreditTotal = ClassificationCreditTotal;
                        trialBalanceRecord.objrowItemList = trialBalanceRowItems;
                        trialBalanceList.Add(trialBalanceRecord);
                    }
                    #endregion

                }
            }
            catch (Exception)
            {
                //Log Exception Exception Table
            }


            return trialBalanceList.OrderBy(s => s.DisplayChartAccountNumber).ToList();
        }
        #endregion

        #region Retained Year Earning
        public decimal GetRetainedYearEarnings(int userId)
        {
            decimal TotalRetainedEarning = 0;

            using (var db = new KFentities())
            {
                var userdata = db.UserRegistrations.Where(s => s.Id == userId).FirstOrDefault();

                var rst = db.RetainedEarnings.Where(s => s.UserId == userId).Select(s => s.ReatinedYearEarning).ToList().Sum();

                TotalRetainedEarning = Convert.ToDecimal(rst);

            }
            return TotalRetainedEarning;
        }
        #endregion

        #region Income Sheet
        public IncomeSheetDto GetIncomeSheetData(int userId, DateTime? startDate, DateTime? endDate, DateTime? startSecondDate, DateTime? endSecondDate)
        {
            var incomeSheetList = new IncomeSheetDto();
            List<CategoryReportDto> objExpenseList = new List<CategoryReportDto>();
            List<CategoryReportDto> objRevenueList = new List<CategoryReportDto>();
            List<CategoryReportDto> objSecondExpenseList = new List<CategoryReportDto>();
            List<CategoryReportDto> objSecondRevenueList = new List<CategoryReportDto>();
            decimal TotalRevenue1 = 0;
            decimal TotalRevenue2 = 0;
            decimal TotalExpense1 = 0;
            decimal TotalExpense2 = 0;


            #region Revenue
            if (startSecondDate != null && endSecondDate != null)
            {
                objSecondRevenueList = this.GetRevenueData(userId, startSecondDate, endSecondDate);
                TotalRevenue2 = objSecondRevenueList.Select(d => d.GrossTotal).ToList().Sum();
            }
            objRevenueList = this.GetRevenueData(userId, startDate, endDate);
            TotalRevenue1 = objRevenueList.Select(d => d.GrossTotal).ToList().Sum();
            #endregion

            #region Expense
            if (startSecondDate != null && endSecondDate != null)
            {
                objSecondExpenseList = this.GetExpenseData(userId, startSecondDate, endSecondDate);
                TotalExpense2 = objSecondExpenseList.Select(d => d.GrossTotal).ToList().Sum();
            }
            objExpenseList = this.GetExpenseData(userId, startDate, endDate);
            TotalExpense1 = objExpenseList.Select(d => d.GrossTotal).ToList().Sum();
            #endregion

            #region Calculate Total Income
            incomeSheetList.NetIncome1 = Decimal.Subtract(TotalRevenue1, TotalExpense1);
            incomeSheetList.NetIncome2 = Decimal.Subtract(TotalRevenue2, TotalExpense2);
            #endregion

            incomeSheetList.objExpenseList = objExpenseList;
            incomeSheetList.objRevenueList = objRevenueList;
            incomeSheetList.objSecondExpenseList = objSecondExpenseList;
            incomeSheetList.objSecondRevenueList = objSecondRevenueList;
            return incomeSheetList;
        }
        #endregion

        #region Balance Sheet
        public BalanceSheetDto GetBalanceSheetData(int userId, DateTime? startDate, DateTime? endDate, DateTime? startSecondDate, DateTime? endSecondDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    BalanceSheetDto objBalanceSheetData = new BalanceSheetDto();


                    objBalanceSheetData.objAssetList = this.GetAssetData(userId, startDate, endDate);
                    objBalanceSheetData.objLiablityList = this.GetLiabilityData(userId, startDate, endDate);
                    objBalanceSheetData.objEquityList = this.GetEquityData(userId, startDate, endDate);

                    if (startSecondDate != null && endSecondDate != null)
                    {
                        objBalanceSheetData.objSecondAssetList = this.GetAssetData(userId, startSecondDate, endSecondDate);
                        objBalanceSheetData.objSecondLiablityList = this.GetLiabilityData(userId, startSecondDate, endSecondDate);
                        objBalanceSheetData.objSecondEquityList = this.GetEquityData(userId, startSecondDate, endSecondDate);
                    }
                    else
                    {
                        objBalanceSheetData.objSecondAssetList = new List<CategoryReportDto>();
                        objBalanceSheetData.objSecondLiablityList = new List<CategoryReportDto>();
                        objBalanceSheetData.objSecondEquityList = new List<CategoryReportDto>();
                    }

                    return objBalanceSheetData;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region Get Revenue Data
        public List<CategoryReportDto> GetRevenueData(int userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var classificationDetailList = new List<CategoryReportDto>();
                    var objBankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 3 && i.ClassificationId != 1  && i.UserId == userId && i.StatusId == 4).ToList();

                    var objRevenueData = objBankStatementData.Where(i => i.Date >= startDate && i.Date < endDate).OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationId).ToList();

                    foreach (var data in objRevenueData)
                    {
                        var classificationRecord = new CategoryReportDto();
                        int classificationId = Convert.ToInt32(data.Key);
                        var classificationDetails = db.Classifications.Where(d => d.Id == classificationId).FirstOrDefault();
                        classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                        classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                        classificationRecord.ClassificationType = classificationDetails.Type;
                        classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                        classificationRecord.ReportingTotalNumber = Convert.ToInt32(classificationDetails.ReportingTotalNumber);
                        classificationRecord.ReportingTotalDisplayNumber = classificationDetails.ReportingTotalDisplayNumber;
                        classificationRecord.ReportingTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingTotalDisplayNumber && f.UserId == userId).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.GrossTotal = 0;
                        var statementList = data.ToList();
                        foreach (var item in statementList)
                        {
                            if (item.Credit == null)
                            {
                                item.Credit = 0;
                            }
                            if (item.Debit == null)
                            {
                                item.Debit = 0;
                            }
                            if (item.Credit > item.Debit)
                                classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                            else if (item.Debit > item.Credit)
                                classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                        }
                        if (Math.Abs(classificationRecord.GrossTotal) > 0)
                        {
                            classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                            classificationDetailList.Add(classificationRecord);
                        }
                    }

                    return classificationDetailList;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region Get Expense Data
        public List<CategoryReportDto> GetExpenseData(int userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var classificationDetailList = new List<CategoryReportDto>();
                    var objBankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 2 && i.ClassificationId != 1 && i.UserId == userId && i.StatusId == 4).ToList();

                    var objRevenueData = objBankStatementData.Where(i => i.Date >= startDate && i.Date < endDate).OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationId).ToList();

                    foreach (var data in objRevenueData)
                    {
                        var classificationRecord = new CategoryReportDto();
                        int classificationId = Convert.ToInt32(data.Key);
                        var classificationDetails = db.Classifications.Where(d => d.Id == classificationId).FirstOrDefault();
                        classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                        classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                        classificationRecord.ClassificationType = classificationDetails.Type;
                        classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                        classificationRecord.ReportingTotalNumber = Convert.ToInt32(classificationDetails.ReportingTotalNumber);
                        classificationRecord.ReportingTotalDisplayNumber = classificationDetails.ReportingTotalDisplayNumber;
                        classificationRecord.ReportingTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingTotalDisplayNumber && f.UserId == userId).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.GrossTotal = 0;
                        var statementList = data.ToList();
                        foreach (var item in statementList)
                        {
                            if (item.Credit == null)
                            {
                                item.Credit = 0;
                            }
                            if (item.Debit == null)
                            {
                                item.Debit = 0;
                            }
                            if (item.Credit > item.Debit)
                                classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                            else if (item.Debit > item.Credit)
                                classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                        }
                        if (Math.Abs(classificationRecord.GrossTotal) > 0)
                        {
                            classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                            classificationDetailList.Add(classificationRecord);
                        }
                    }

                    return classificationDetailList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Asset Data
        public List<CategoryReportDto> GetAssetData(int userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var classificationDetailList = new List<CategoryReportDto>();
                    #region Normal Asset Entry
                    var objBankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 1 && i.ClassificationId != 1 && i.UserId == userId && i.CategoryId != 6 && i.StatusId == 4 
                            && i.Classification.ChartAccountDisplayNumber != "1050-1060" && i.Classification.ChartAccountDisplayNumber != "1050-1061" &&
                            i.Classification.ChartAccountDisplayNumber != "1050-1062" && i.Classification.ChartAccountDisplayNumber != "1050-1063"
                            && i.Classification.ChartAccountDisplayNumber != "1050-1064" && i.Classification.ChartAccountDisplayNumber != "1050-1073"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1080" && i.Classification.ChartAccountDisplayNumber != "1100-1081"
                            && i.Classification.ChartAccountDisplayNumber != "1100-1082" && i.Classification.ChartAccountDisplayNumber != "1100-1083"
                             && i.Classification.ChartAccountDisplayNumber != "1100-1087" && i.Classification.ChartAccountDisplayNumber != "1100-1088"
                              && i.Classification.ChartAccountDisplayNumber != "1100-1089"
                            && i.Date >= startDate && i.Date <= endDate).ToList();
                    var capAssets = objBankStatementData.Where(f => f.Classification.ChartAccountNumber > 15000000 && f.Classification.ChartAccountNumber < 19009999).ToList();

                    var objAssetData = objBankStatementData.OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationId).ToList();

                    foreach (var data in objAssetData)
                    {
                        var classificationRecord = new CategoryReportDto();
                        int classificationId = Convert.ToInt32(data.Key);
                        var classificationDetails = db.Classifications.Where(d => d.Id == classificationId).FirstOrDefault();
                        classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                        classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                        classificationRecord.ClassificationType = classificationDetails.Type;
                        classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                        classificationRecord.ReportingTotalNumber = Convert.ToInt32(classificationDetails.ReportingSubTotalNumber);
                        classificationRecord.ReportingTotalDisplayNumber = classificationDetails.ReportingSubTotalDisplayNumber;
                        //classificationRecord.ReportingTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingTotalDisplayNumber).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.ReportingSubTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingSubTotalDisplayNumber && f.UserId == userId).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.GrossTotal = 0;
                        var statementList = data.ToList();
                        foreach (var item in statementList)
                        {
                            if (item.Credit == null)
                            {
                                item.Credit = 0;
                            }
                            if (item.Debit == null)
                            {
                                item.Debit = 0;
                            }
                            if (item.Credit > item.Debit)
                                classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                            else if (item.Debit > item.Credit)
                                classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                        }
                        if (Math.Abs(classificationRecord.GrossTotal) > 0)
                        {
                            classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                            classificationDetailList.Add(classificationRecord);
                        }
                    }
                    #endregion

                    #region Director Loan
                    var directorLoanExistChk = classificationDetailList.Where(s => s.DisplayClassificationChartAccountNumber == "1200-0001").FirstOrDefault();
                    var directorDataList = (from bnkexpense in db.BankExpenses
                                            join classificationdata in db.Classifications on bnkexpense.ClassificationId equals classificationdata.Id
                                            join director in db.DirectorAccountLogs on bnkexpense.Id equals director.StatementId
                                            //  join ocrexpense in db.OcrExpenseDetails on bnkexpense.Id equals ocrexpense.StatementID
                                            join bank in db.Banks on bnkexpense.BankId equals bank.Id
                                            where bnkexpense.UserId == userId && bnkexpense.Date >= startDate && bnkexpense.Date <= endDate && bnkexpense.StatusId == 4 
                                            && bnkexpense.BankId != 6 //Bank ID 6 for MJV
                                            select new
                                            {
                                                classificationdata.Id,
                                                classificationdata.ClassificationType,
                                                classificationdata.ChartAccountNumber,
                                                StatementID = bnkexpense.Id,
                                                Tax = bnkexpense.TotalTax,
                                                bnkexpense.Credit,
                                                bnkexpense.Debit,
                                                Total = bnkexpense.ActualTotal,
                                                director.PercentageAmount,
                                                bnkexpense.Description,
                                                bnkexpense.Comments,
                                                bnkexpense.Date,
                                                bnkexpense.UploadType,
                                                bank.BankName,
                                                bnkexpense.AccountName,
                                                bnkexpense.AccountClassificationId
                                            }).ToList();
                    if (directorDataList.Count > 0)
                    {
                        if (directorLoanExistChk != null)
                        {
                            //update existing entry
                            foreach (var data in directorDataList)
                            {
                                if (data.Credit > data.Debit)
                                {
                                    directorLoanExistChk.GrossTotal = Decimal.Subtract(directorLoanExistChk.GrossTotal, Convert.ToDecimal(data.PercentageAmount));
                                }
                                else if (data.Debit > data.Credit)
                                {
                                    directorLoanExistChk.GrossTotal = Decimal.Add(directorLoanExistChk.GrossTotal, Convert.ToDecimal(data.PercentageAmount));
                                }
                            }
                        }
                        else
                        {
                            //add new
                            var classificationRecord = new CategoryReportDto();
                            var classificationDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == "1200-0001").FirstOrDefault();
                            classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                            classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                            classificationRecord.ClassificationType = classificationDetails.Type;
                            classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                            classificationRecord.GrossTotal = 0;
                            foreach (var item in directorDataList)
                            {
                                if (item.Credit > item.Debit)
                                    classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.PercentageAmount));
                                else if (item.Debit > item.Credit)
                                    classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.PercentageAmount));
                            }
                            if (Math.Abs(classificationRecord.GrossTotal) > 0)
                            {
                                classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                                classificationDetailList.Add(classificationRecord);
                            }
                        }
                    }

                    #endregion

                    #region  Account Based Asset Entry

                    #region Normal Entries
                    var query = db.BankExpenses.Where(i => i.IsDeleted == false && i.AccountClassificationId != 1030 && i.ClassificationId != 1 && i.UserId == userId && i.StatusId == 4).OrderBy(s => s.Date).ToList();
                    var NormalBankDataForAccount = db.BankExpenses.Where(i => i.IsDeleted == false && i.AccountClassificationId != 1030 && i.ClassificationId != 1 && i.UserId == userId && i.StatusId == 4 
                        && i.Date >= startDate && i.Date <= endDate).OrderBy(s => s.Date).GroupBy(s => s.AccountClassificationId).ToList();
                    if (NormalBankDataForAccount.Count > 0)
                    {
                        foreach (var data in NormalBankDataForAccount)
                        {
                            var classificationRecord = new CategoryReportDto();
                            switch (data.Key)
                            {
                                case 1060:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1060" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501060;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1060";
                                    break;
                                case 1061:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1061" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501061;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1061";
                                    break;
                                case 1062:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1062" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501062;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1062";
                                    break;
                                case 1063:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1063" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501063;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1063";
                                    break;
                                case 1064:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1064" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501064;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1064";
                                    break;
                                case 1073:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1050-1073" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 10501073;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1050-1073";
                                    break;
                                case 1080:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1080" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001080;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1080";
                                    break;
                                case 1081:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1081" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001081;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1081";
                                    break;
                                case 1082:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1082" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001082;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1082";
                                    break;
                                case 1083:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1083" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001083;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1083";
                                    break;
                                case 1087:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1087" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001087;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1087";
                                    break;
                                case 1088:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1088" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001088;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1088";
                                    break;
                                case 1089:
                                    classificationRecord.ClassificationType = "A";
                                    classificationRecord.ClassificationName = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "1100-1089" && s.UserId == userId).Select(d => d.ClassificationType).FirstOrDefault();
                                    classificationRecord.ClassificationChartAccountNumber = 11001089;
                                    classificationRecord.DisplayClassificationChartAccountNumber = "1100-1089";
                                    break;
                            }
                            classificationRecord.GrossTotal = 0;
                            var statementList = data.ToList();
                            foreach (var item in statementList)
                            {
                                if (item.Credit == null)
                                    item.Credit = 0;
                                else if (item.Debit == null)
                                    item.Debit = 0;

                                if (item.Credit > 0)
                                {
                                    //reverse the virtual entry
                                    classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.Credit));
                                    //itemRecord.Debit = decimal.Round(Convert.ToDecimal(rowItem.Credit), 2, MidpointRounding.AwayFromZero);
                                }
                                else if (item.Debit > 0)
                                {
                                    classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.Debit));
                                    // rowItemRecord.Credit = decimal.Round(Convert.ToDecimal(rowItem.Debit), 2, MidpointRounding.AwayFromZero);
                                }
                            }
                            if (Math.Abs(classificationRecord.GrossTotal) > 0)
                            {
                                classificationDetailList.Add(classificationRecord);
                            }
                        }
                    }
                    #endregion

                    #region MJV Entries
                    var MJVEntries = db.BankExpenses.Where(i => i.IsDeleted == false && i.UserId == userId && i.ClassificationId != 1 && i.Classification.UserId == userId && i.StatusId == 4 &&
                       (i.Classification.ChartAccountDisplayNumber == "1050-1060" || i.Classification.ChartAccountDisplayNumber == "1050-1061"
                      || i.Classification.ChartAccountDisplayNumber == "1050-1062" || i.Classification.ChartAccountDisplayNumber == "1050-1063"
                      || i.Classification.ChartAccountDisplayNumber == "1050-1064" || i.Classification.ChartAccountDisplayNumber == "1050-1073"
                      || i.Classification.ChartAccountDisplayNumber == "1100-1080" || i.Classification.ChartAccountDisplayNumber == "1100-1081"
                      || i.Classification.ChartAccountDisplayNumber == "1100-1082" || i.Classification.ChartAccountDisplayNumber == "1100-1083"
                      || i.Classification.ChartAccountDisplayNumber == "1100-1087" || i.Classification.ChartAccountDisplayNumber == "1100-1088"
                      || i.Classification.ChartAccountDisplayNumber == "1100-1089")
                    && i.CategoryId != 6 && i.Date >= startDate && i.Date <= endDate).ToList();
                    var AccountBankData = MJVEntries.OrderBy(s => s.Date).GroupBy(s => s.Classification.ChartAccountDisplayNumber).ToList();
                    if (AccountBankData.Count > 0)
                    {
                        foreach (var MjvItem in AccountBankData)
                        {
                            if (classificationDetailList.Where(d => d.DisplayClassificationChartAccountNumber == MjvItem.Key).Any())
                            {
                                //update
                                var oldEntryDetails = classificationDetailList.Where(d => d.DisplayClassificationChartAccountNumber == MjvItem.Key).FirstOrDefault();
                                foreach (var item in MjvItem.ToList())
                                {
                                    if (item.Credit == null)
                                        item.Credit = 0;
                                    if (item.Debit == null)
                                        item.Debit = 0;

                                    if (item.Credit > item.Debit)
                                        oldEntryDetails.GrossTotal = Decimal.Subtract(oldEntryDetails.GrossTotal, Convert.ToDecimal(item.Credit));
                                    else if (item.Debit > item.Credit)
                                        oldEntryDetails.GrossTotal = Decimal.Add(oldEntryDetails.GrossTotal, Convert.ToDecimal(item.Debit));
                                }
                            }
                            else
                            {
                                //insert
                                var classificationRecord = new CategoryReportDto();
                                var classificationDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == MjvItem.Key && d.UserId == userId).FirstOrDefault();
                                classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                                classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                                classificationRecord.ClassificationType = classificationDetails.Type;
                                classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                                classificationRecord.GrossTotal = 0;
                                foreach (var item in MjvItem.ToList())
                                {
                                    if (item.Credit == null)
                                        item.Credit = 0;
                                    if (item.Debit == null)
                                        item.Debit = 0;

                                    if (item.Credit > item.Debit)
                                        classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.Credit));
                                    else if (item.Debit > item.Credit)
                                        classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.Debit));
                                }
                                if (Math.Abs(classificationRecord.GrossTotal) > 0)
                                {
                                    classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                                    classificationDetailList.Add(classificationRecord);
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion

                    return classificationDetailList.OrderBy(s => s.ClassificationChartAccountNumber).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Liability Data
        public List<CategoryReportDto> GetLiabilityData(int userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var classificationDetailList = new List<CategoryReportDto>();
                    var objBankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 4 && i.ClassificationId != 1 && i.UserId == userId && i.StatusId == 4).ToList();

                    var objLiabilityData = objBankStatementData.Where(i => i.Date >= startDate && i.Date < endDate).OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationId).ToList();

                    foreach (var data in objLiabilityData)
                    {
                        var classificationRecord = new CategoryReportDto();
                        int classificationId = Convert.ToInt32(data.Key);
                        var classificationDetails = db.Classifications.Where(d => d.Id == classificationId).FirstOrDefault();
                        classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                        classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                        classificationRecord.ClassificationType = classificationDetails.Type;
                        classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                        classificationRecord.ReportingTotalNumber = Convert.ToInt32(classificationDetails.ReportingSubTotalNumber);
                        classificationRecord.ReportingTotalDisplayNumber = classificationDetails.ReportingSubTotalDisplayNumber;
                        // classificationRecord.ReportingTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingTotalDisplayNumber).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.ReportingSubTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingSubTotalDisplayNumber && f.UserId == userId).Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.GrossTotal = 0;
                        var statementList = data.ToList();
                        foreach (var item in statementList)
                        {
                            if (item.Credit == null)
                            {
                                item.Credit = 0;
                            }
                            if (item.Debit == null)
                            {
                                item.Debit = 0;
                            }
                            if (item.Credit > item.Debit)
                                classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                            else if (item.Debit > item.Credit)
                                classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                        }
                        if (Math.Abs(classificationRecord.GrossTotal) > 0)
                        {
                            classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                            classificationDetailList.Add(classificationRecord);
                        }
                    }

                    #region Tax Part
                    var TaxDataForNormalEntries = db.BankExpenses.Where(i => i.IsDeleted == false && i.UserId == userId && i.StatusId == 4 && i.ClassificationId != 1 // && i.Classification.UserId == null
                   && i.CategoryId != 6 && i.Date >= startDate && i.Date <= endDate).ToList();
                    List<TrialBalanceTaxDto> liabilityTemporaryTaxList = new List<TrialBalanceTaxDto>();
                    foreach (var item in TaxDataForNormalEntries)
                    {
                        if (item.Credit == null)
                            item.Credit = 0;
                        else if (item.Debit == null)
                            item.Debit = 0;

                        //   var Ocrdata = item.OcrExpenseDetails.FirstOrDefault();

                        switch (item.CategoryId)
                        {
                            case 3:
                                if (item.HSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2370";
                                    objTax.ClassificationType = "HST Charged on Sales";
                                    objTax.Tax = item.HSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.PSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2373";
                                    objTax.ClassificationType = "PST Charged on Sales";
                                    objTax.Tax = item.PSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.GSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2371";
                                    objTax.ClassificationType = "GST Charged on Sales";
                                    objTax.Tax = item.GSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.QSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2372";
                                    objTax.ClassificationType = "QST Charged on Sales";
                                    objTax.Tax = item.QSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                break;
                            default:
                                if (item.HSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2375";
                                    objTax.ClassificationType = "HST Paid on Purchases";
                                    objTax.Tax = item.HSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.PSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2378";
                                    objTax.ClassificationType = "PST Paid on Purchases";
                                    objTax.Tax = item.PSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.GSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2376";
                                    objTax.ClassificationType = "GST Paid on Purchases";
                                    objTax.Tax = item.GSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                if (item.QSTtax > 0)
                                {
                                    TrialBalanceTaxDto objTax = new TrialBalanceTaxDto();
                                    objTax.ChartAccountNumber = "2300-2377";
                                    objTax.ClassificationType = "QST Paid on Purchases";
                                    objTax.Tax = item.QSTtax;
                                    objTax.TransactionType = item.Credit > 0 ? "Cr." : "Dr.";
                                    liabilityTemporaryTaxList.Add(objTax);
                                }
                                break;
                        }
                    }
                    #endregion

                    #region Add Tax part to List
                    if (liabilityTemporaryTaxList.Count > 0)
                    {
                        var taxList = liabilityTemporaryTaxList.OrderBy(s => s.ChartAccountNumber).GroupBy(d => d.ChartAccountNumber).ToList();
                        foreach (var row in taxList)
                        {
                            if (classificationDetailList.Where(d => d.DisplayClassificationChartAccountNumber == row.Key).Any())
                            {
                                var existChk = classificationDetailList.Where(d => d.DisplayClassificationChartAccountNumber == row.Key).FirstOrDefault();
                                foreach (var item in row.ToList())
                                {
                                    if (item.TransactionType == "Cr.")
                                        existChk.GrossTotal = Decimal.Add(existChk.GrossTotal, Convert.ToDecimal(item.Tax));
                                    else if (item.TransactionType == "Dr.")
                                        existChk.GrossTotal = Decimal.Subtract(existChk.GrossTotal, Convert.ToDecimal(item.Tax));
                                }
                            }
                            else
                            {
                                var classificationRecord = new CategoryReportDto();
                                var classificationDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == row.Key).FirstOrDefault();
                                classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                                classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                                classificationRecord.ClassificationType = classificationDetails.Type;
                                classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                                classificationRecord.GrossTotal = 0;
                                var statementList = row.ToList();
                                foreach (var item in statementList)
                                {
                                    if (item.TransactionType == "Cr.")
                                        classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.Tax));
                                    else if (item.TransactionType == "Dr.")
                                        classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.Tax));
                                }
                                if (Math.Abs(classificationRecord.GrossTotal) > 0)
                                {
                                    classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                                    classificationDetailList.Add(classificationRecord);
                                }
                            }
                        }
                    }
                    #endregion

                    return classificationDetailList.OrderBy(s => s.ClassificationChartAccountNumber).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Equity Data
        public List<CategoryReportDto> GetEquityData(int userId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var classificationDetailList = new List<CategoryReportDto>();
                    var objBankStatementData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 5 && i.StatusId == 4 && i.ClassificationId != 1 && i.UserId == userId && i.Classification.ChartAccountDisplayNumber != "3550-3561").ToList();

                    var objEquityData = objBankStatementData.Where(i => i.Date >= startDate && i.Date < endDate).OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationId).ToList();

                    foreach (var data in objEquityData)
                    {
                        var classificationRecord = new CategoryReportDto();
                        int classificationId = Convert.ToInt32(data.Key);
                        var classificationDetails = db.Classifications.Where(d => d.Id == classificationId).FirstOrDefault();
                        classificationRecord.ClassificationChartAccountNumber = classificationDetails.ChartAccountNumber;
                        classificationRecord.DisplayClassificationChartAccountNumber = classificationDetails.ChartAccountDisplayNumber;
                        classificationRecord.ClassificationType = classificationDetails.Type;
                        classificationRecord.ClassificationName = classificationDetails.ClassificationType;
                        classificationRecord.ReportingTotalNumber = Convert.ToInt32(classificationDetails.ReportingSubTotalNumber);
                        classificationRecord.ReportingTotalDisplayNumber = classificationDetails.ReportingSubTotalDisplayNumber;
                        //classificationRecord.ReportingTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingTotalDisplayNumber)
                        //    .Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.ReportingSubTotalClassification = db.Classifications.Where(f => f.ChartAccountDisplayNumber == classificationDetails.ReportingSubTotalDisplayNumber && f.UserId == userId)
                            .Select(q => q.ClassificationType).FirstOrDefault();
                        classificationRecord.GrossTotal = 0;
                        var statementList = data.ToList();
                        foreach (var item in statementList)
                        {
                            if (item.Credit == null)
                            {
                                item.Credit = 0;
                            }
                            if (item.Debit == null)
                            {
                                item.Debit = 0;
                            }
                            if (item.Credit > item.Debit)
                                classificationRecord.GrossTotal = Decimal.Add(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                            else if (item.Debit > item.Credit)
                                classificationRecord.GrossTotal = Decimal.Subtract(classificationRecord.GrossTotal, Convert.ToDecimal(item.ActualTotal));
                        }
                        if (Math.Abs(classificationRecord.GrossTotal) > 0)
                        {
                            classificationRecord.GrossTotal = decimal.Round(classificationRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                            classificationDetailList.Add(classificationRecord);
                        }
                    }

                    #region Retained Earning
                    //var classificationRetainedEarningRecord = new CategoryReportDto();
                    //var retainedEarningData = db.BankExpenses.Where(i => i.Date >= startDate && i.Date < endDate && i.IsDeleted == false && i.CategoryId == 5 && i.ClassificationId != 1 && i.UserId == userId && i.Classification.ChartAccountDisplayNumber == "3550-3561").ToList();
                    //var cfReDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == "3550-3561").FirstOrDefault();
                    //classificationRetainedEarningRecord.ClassificationChartAccountNumber = cfReDetails.ChartAccountNumber;
                    //classificationRetainedEarningRecord.DisplayClassificationChartAccountNumber = cfReDetails.ChartAccountDisplayNumber;
                    //classificationRetainedEarningRecord.ClassificationType = cfReDetails.Type;
                    //classificationRetainedEarningRecord.ClassificationName = cfReDetails.ClassificationType;
                    //decimal CreditSum = 0;
                    //decimal DebitSum = 0;
                    //DebitSum = Convert.ToDecimal(retainedEarningData.Select(d => d.Debit).ToList().Sum());
                    //CreditSum = Convert.ToDecimal(retainedEarningData.Select(d => d.Credit).ToList().Sum());
                    //classificationRetainedEarningRecord.GrossTotal = Decimal.Subtract(CreditSum, DebitSum);
                    //classificationRetainedEarningRecord.GrossTotal = decimal.Round(classificationRetainedEarningRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);

                    classificationDetailList.Add(GetReatinedEarning(startDate, endDate, userId));

                    #endregion

                    #region Current Year Earning
                    var classificationCurrentEarningRecord = new CategoryReportDto();
                    var cfDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == "3550-3600").FirstOrDefault();
                    classificationCurrentEarningRecord.ClassificationChartAccountNumber = Convert.ToInt32(cfDetails.ChartAccountNumber);
                    classificationCurrentEarningRecord.DisplayClassificationChartAccountNumber = cfDetails.ChartAccountDisplayNumber;
                    classificationCurrentEarningRecord.ClassificationType = cfDetails.Type;
                    classificationCurrentEarningRecord.ClassificationName = cfDetails.ClassificationType;
                    classificationCurrentEarningRecord.GrossTotal = UserCurrentYearEarning.GetCurrentYearEarning(userId, startDate, endDate);
                    classificationCurrentEarningRecord.GrossTotal = decimal.Round(classificationCurrentEarningRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                    classificationDetailList.Add(classificationCurrentEarningRecord);
                    #endregion
                    classificationDetailList.RemoveAll(item => item == null);
                    return classificationDetailList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Retained Earning 19Jan 2017
        public CategoryReportDto GetReatinedEarning(DateTime? startDate, DateTime? endDate, Int32 userId)
        {
            using (var db = new KFentities())
            {
                var classificationRetainedEarningRecord = new CategoryReportDto();
                var retainedEarningData = db.BankExpenses.Where(i => i.Date.Value.Year <= startDate.Value.Year && i.StatusId == 4 && i.IsDeleted == false && i.CategoryId == 5 && i.ClassificationId != 1 && i.UserId == userId && i.Classification.ChartAccountDisplayNumber == "3550-3561").ToList();
                var cfReDetails = db.Classifications.Where(d => d.ChartAccountDisplayNumber == "3550-3561").FirstOrDefault();
                classificationRetainedEarningRecord.ClassificationChartAccountNumber = cfReDetails.ChartAccountNumber;
                classificationRetainedEarningRecord.DisplayClassificationChartAccountNumber = cfReDetails.ChartAccountDisplayNumber;
                classificationRetainedEarningRecord.ClassificationType = cfReDetails.Type;
                classificationRetainedEarningRecord.ClassificationName = cfReDetails.ClassificationType;
                decimal CreditSum = 0;
                decimal DebitSum = 0;
                DebitSum = Convert.ToDecimal(retainedEarningData.Select(d => d.Debit).ToList().Sum());
                CreditSum = Convert.ToDecimal(retainedEarningData.Select(d => d.Credit).ToList().Sum());
                classificationRetainedEarningRecord.GrossTotal = Decimal.Subtract(CreditSum, DebitSum);
                if (Math.Abs(classificationRetainedEarningRecord.GrossTotal) > 0)
                {
                    classificationRetainedEarningRecord.GrossTotal = decimal.Round(classificationRetainedEarningRecord.GrossTotal, 2, MidpointRounding.AwayFromZero);
                    return classificationRetainedEarningRecord;
                }
                else
                    return null;
            }
        }
        #endregion
    }
}
