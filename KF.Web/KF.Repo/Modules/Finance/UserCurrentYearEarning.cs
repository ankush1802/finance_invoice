using KF.Dto.Modules.FinanceReport;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Finance
{
    public static class UserCurrentYearEarning
    {
        #region Current Year Earning
        public static decimal GetCurrentYearEarning(int userId, DateTime? startDate, DateTime? endDate)
        {
            decimal CurrentYearEarning = 0;
            decimal CurrentYearEarningExpenseTotal = 0;
            decimal CurrentYearEarningRevenueTotal = 0;
            using (var db = new KFentities())
            {
                List<BankExpense> objExpenseData = new List<BankExpense>();
                List<BankExpense> objRevenueData = new List<BankExpense>();

                if (startDate != null && endDate != null)
                {
                    objExpenseData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 2 && i.StatusId == 4 && i.ClassificationId != 1 && i.UserId == userId).ToList();
                    objExpenseData = objExpenseData.Where(i => i.Date >= startDate && i.Date < endDate).ToList();
                    objRevenueData = db.BankExpenses.Where(i => i.IsDeleted == false && i.CategoryId == 3 && i.StatusId == 4 && i.ClassificationId != 1 && i.UserId == userId).ToList();
                    objRevenueData = objRevenueData.Where(i => i.Date >= startDate && i.Date < endDate).ToList();
                }

                #region Expense Data binding
                List<IncomeChildModel> objExpenseList = new List<IncomeChildModel>();
                foreach (var expenseData in objExpenseData)
                {
                    var expenseRecord = new IncomeChildModel();
                  //  var OcrData = db.OcrExpenseDetails.Where(a => a.StatementID == expenseData.Id).FirstOrDefault();
                    if (expenseData != null)
                    {
                        if (expenseData.Credit == null)
                        {
                            expenseData.Credit = 0;
                        }
                        if (expenseData.Debit == null)
                        {
                            expenseData.Debit = 0;
                        }
                        decimal Total = 0;

                        if (expenseData.Credit > expenseData.Debit)
                        {
                            Total = Convert.ToDecimal(expenseData.ActualTotal);
                            expenseRecord.GrossTotal = decimal.Round(Convert.ToDecimal("-" + Total), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            Total = Convert.ToDecimal(expenseData.ActualTotal);
                            expenseRecord.GrossTotal = decimal.Round(Total, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        expenseRecord.GrossTotal = 0;
                    }

                    objExpenseList.Add(expenseRecord);
                }
                CurrentYearEarningExpenseTotal = objExpenseList.Sum(s => s.GrossTotal);
                #endregion
                #region revenue
                List<IncomeChildModel> objRevenueList = new List<IncomeChildModel>();
                foreach (var revenueData in objRevenueData)
                {
                    var expenseRecord = new IncomeChildModel();
                  //  var OcrData = db.OcrExpenseDetails.Where(a => a.StatementID == revenueData.Id).FirstOrDefault();
                    if (revenueData != null)
                    {
                        if (revenueData.Credit == null)
                        {
                            revenueData.Credit = 0;
                        }
                        if (revenueData.Debit == null)
                        {
                            revenueData.Debit = 0;
                        }
                        if (revenueData.Credit > revenueData.Debit)
                        {
                            expenseRecord.GrossTotal = decimal.Round(Convert.ToDecimal(revenueData.ActualTotal), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            expenseRecord.GrossTotal = decimal.Round(Convert.ToDecimal("-" + revenueData.ActualTotal), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                    {
                        expenseRecord.GrossTotal = 0;
                    }
                    objRevenueList.Add(expenseRecord);
                }
                CurrentYearEarningRevenueTotal = objRevenueList.Sum(s => s.GrossTotal);
                #endregion
                CurrentYearEarning = Decimal.Subtract(CurrentYearEarningRevenueTotal, CurrentYearEarningExpenseTotal);

            }
            return CurrentYearEarning;
        }
        #endregion
    }
}
