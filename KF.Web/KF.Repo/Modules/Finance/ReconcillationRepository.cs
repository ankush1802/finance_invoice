using AutoMapper;
using KF.Dto.Modules.Finance;
using KF.Dto.Modules.Other;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Finance
{
    public class ReconcillationRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Credit account list
        public List<AccountsDto> GetUserAccounts(int userid, string AccountType)
        {
            List<AccountsDto> accountList = new List<AccountsDto>();
            using (var db = new KFentities())
            {
                var accounts = db.BankExpenses.Where(x => x.UserId == userid).ToList();
                if (accounts.Count > 0)
                {
                    var accList = accounts.Where(x => x.AccountType.Contains(AccountType)).OrderByDescending(p => p.Id).Select(x => new { x.AccountName, x.BankId }).Distinct().ToList();

                    accountList = accList.Select(s => new AccountsDto()
                    {
                        AccountName = s.AccountName,
                        BankId = s.BankId
                    }).ToList();
                }

                return accountList;
            }
        }

        public List<AccountsDto> GetUserAccountsByUserId(int userid, int bankid, string AccountType)
        {
            List<AccountsDto> accountList = new List<AccountsDto>();
            using (var db = new KFentities())
            {
                var accounts = db.BankExpenses.Where(x => x.UserId == userid && x.BankId == bankid).ToList();
                if (bankid != 7)
                {
                    var accountData = accounts.Where(x => x.AccountType.Contains(AccountType)).OrderByDescending(p => p.Id).Select(x => x.AccountName).Distinct().ToList();
                    accountList = accountData.Select(s => new AccountsDto()
                    {
                        AccountName = s
                    }).ToList();
                }
                else
                {
                    var accountData = accounts.OrderByDescending(p => p.Id).Select(x => x.AccountName).Distinct().ToList();
                    accountList = accountData.Select(s => new AccountsDto()
                    {
                        AccountName = s
                    }).ToList();
                }
                return accountList;
            }
        }

        public List<BankDto> GetStatementList(string AccName, int month, int year, int UserId, int BankId, string AccountType)
        {
            using (var db = new KFentities())
            {
                try
                {
                    List<BankDto> Objlist = new List<BankDto>();
                    var data = db.BankExpenses.Where(d => d.Date.Value.Year == year && d.Date.Value.Month == month &&
                        d.UserId == UserId && d.BankId == BankId && d.IsDeleted == false && d.IsVirtualEntry != true
                        && d.AccountName == AccName &&
                        d.AccountType == AccountType).ToList();
                    Objlist = data.Select(t => new BankDto()
                    {
                        Id = t.Id,
                        BankId = Convert.ToInt32(t.BankId),
                        AccountType = t.AccountType,
                        Category = t.Category.CategoryType,
                        CategoryId = Convert.ToInt32(t.CategoryId),
                        Vendor = t.Vendor,
                        Description = t.Description,
                        Purpose = t.Purpose,
                        Status = t.Status.StatusType,
                        StatusId = Convert.ToInt32(t.StatusId),
                        Credit = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.Credit)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.Credit),
                        Debit = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.Debit)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.Debit),
                        Total = Convert.ToDecimal(t.Total),
                        ClassificationId = Convert.ToInt32(t.ClassificationId),
                        ClassificationDescription = t.Classification.Desc,
                        ClassificationType = t.Classification.ClassificationType,
                        Date = Convert.ToDateTime(t.Date),
                        Comments = t.Comments,
                        BillTotal = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.ActualTotal)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.ActualTotal),
                        BillTax = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.TotalTax)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.TotalTax),
                        GSTtax = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.GSTtax)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.GSTtax),
                        QSTtax = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.QSTtax)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.QSTtax),
                        HSTtax = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.HSTtax)), 2, MidpointRounding.AwayFromZero),// Convert.ToDecimal(t.HSTtax),
                        PSTtax = decimal.Round(Convert.ToDecimal(Convert.ToDecimal(t.PSTtax)), 2, MidpointRounding.AwayFromZero),// = Convert.ToDecimal(t.PSTtax),
                        GSTPercentage = Convert.ToDouble(t.GSTPercentage),
                        QSTPercentage = Convert.ToDouble(t.QSTPercentage),
                        HSTPercentage = Convert.ToDouble(t.HSTPercentage),
                        PSTPercentage = Convert.ToDouble(t.PSTPercentage)
                    }).ToList();
                    return Objlist;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// Update Bank Expense
        /// </summary>
        /// <param name="_bankdto"></param>
        /// <returns></returns>
        public bool UpdateBankStatement(BankDto _bankdto)
        {
            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = db.BankExpenses.Where(x => x.Id == _bankdto.Id).FirstOrDefault();
                        if (entity != null)
                        {
                            entity.CategoryId = _bankdto.CategoryId;
                            entity.ClassificationId = _bankdto.ClassificationId;
                            entity.Vendor = _bankdto.Vendor;
                            entity.Description = _bankdto.Description;
                            entity.Purpose = _bankdto.Purpose;
                            entity.Total = _bankdto.Total;
                            entity.StatusId = _bankdto.StatusId;
                            entity.Comments = _bankdto.Comments;
                            //update bill data
                            entity.Total = _bankdto.BillTotal;
                            entity.TotalTax = _bankdto.GSTtax + _bankdto.QSTtax + _bankdto.HSTtax + _bankdto.PSTtax;
                            entity.GSTtax = _bankdto.GSTtax;
                            entity.QSTtax = _bankdto.QSTtax;
                            entity.HSTtax = _bankdto.HSTtax;
                            entity.PSTtax = _bankdto.PSTtax;

                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        return false;

                    }
                }
            }
        }

        public int DeleteBankExpenseById(int BankExpenseId)
        {
            int DeletedRecUserId = 0;
            try
            {
                using (var context = new KFentities())
                {
                    var data = context.BankExpenses.Where(i => i.Id == BankExpenseId).FirstOrDefault();
                    if (data != null)
                    {

                        var fiscalYearPosting = context.FiscalYearPostings.Where(f => f.UserId == data.UserId).OrderByDescending(g => g.Id).ToList();
                        if (fiscalYearPosting.Count > 0)
                        {
                            foreach(var item in fiscalYearPosting)
                            {
                                if (data.Date >= item.TaxStartYear && data.Date <= item.TaxEndYear)
                                {
                                    return 3;
                                }
                            }
                            
                        }
                        var chkShadowEntry = context.ShadowEntryRecords.Where(s => s.BankExpenseId == BankExpenseId && s.IsDeleted == false).OrderByDescending(d => d.Id).FirstOrDefault();
                        if (chkShadowEntry != null)
                        {
                            chkShadowEntry.IsDeleted = true;
                            chkShadowEntry.DateModified = DateTime.Now;
                            var shadowBankExpenseData = context.BankExpenses.Where(i => i.Id == chkShadowEntry.BankExpenseShadowId).FirstOrDefault();
                            if (shadowBankExpenseData != null)
                            {
                                shadowBankExpenseData.IsDeleted = true;
                            }
                        }
                        data.IsDeleted = true;
                        #region reference entries
                        var refList = context.BankExpenses.Where(s => s.StatementReferenceNumber == BankExpenseId).ToList();
                        if (refList.Count > 0)
                        {
                            refList.ForEach(s => s.IsDeleted = true);
                        }
                        #endregion

                        #region Percenrtage account log entry
                        var accdata = context.DirectorAccountLogs.Where(s => s.StatementId == BankExpenseId).FirstOrDefault();
                        if (accdata != null)
                        {
                            context.DirectorAccountLogs.Remove(accdata);
                        }
                        #endregion
                        context.SaveChanges();
                        DeletedRecUserId = Convert.ToInt32(data.UserId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return DeletedRecUserId;
        }
        #endregion

        #region Kippin Store
        #region Add image entry to cloud
        public bool AddImageToKippinStore(KippinStoreImageDto obj)
        {
            bool IsSave = false;
            try
            {
                using (var db = new KFentities())
                {
                    Mapper.CreateMap<KippinStoreImageDto, KippinStoreImage>();
                    var data = Mapper.Map<KippinStoreImage>(obj);
                    db.KippinStoreImages.Add(data);
                    db.SaveChanges();
                    IsSave = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return IsSave;
        }


        #endregion
        #endregion



        #region Bank Statement List Section
        /// <summary>
        /// Get bank statement list
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="selectedUserId"></param>
        /// <param name="StatusId"></param>
        /// <param name="roleId"></param>
        /// <param name="Skip"></param>
        /// <param name="Take"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="BankId"></param>
        /// <param name="JVID"></param>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public StatementListDto GetExpensesByUserId(int userId, int selectedUserId, int? StatusId, int roleId, int Skip, int Take, int? Year, int? Month, int? BankId, int? JVID, int? CategoryId)
        {
            List<BankExpense> ExpenseData = new List<BankExpense>();

            List<StatementList> BankStatementList = new List<StatementList>();
            Int32 TotalCount = 0;
            try
            {
                using (var context = new KFentities())
                {
                    if (userId != selectedUserId)
                    {
                        // ExpenseData = context.BankExpenses.Where(i => i.UserId == selectedUserId && i.IsDeleted == false && i.IsVirtualEntry != true && i.BankId != 6).ToList();
                        ExpenseData = context.BankExpenses.Where(i => i.UserId == selectedUserId && i.IsDeleted == false && i.IsVirtualEntry != true).ToList();
                    }
                    else
                    {
                        //user as an accountant
                        // ExpenseData = context.BankExpenses.Where(i => i.UserId == userId && i.IsDeleted == false && i.IsVirtualEntry != true && i.BankId != 6).ToList();
                        ExpenseData = context.BankExpenses.Where(i => i.UserId == userId && i.IsDeleted == false && i.IsVirtualEntry != true).ToList();
                    }
                    //if (userId != selectedUserId)
                    //{
                    //    if (Year > 0)
                    //    {
                    //        var year = context.tblYears.Where(y => y.Id == Year).Select(t => t.Year).FirstOrDefault();
                    //        var cashData = context.BankExpenses.Where(i => i.UserId == selectedUserId && i.Date.Value.Year == year && i.IsVirtualEntry != true && i.IsDeleted == false && i.BankId == 6).ToList();
                    //        ExpenseData.AddRange(cashData);
                    //    }
                    //    else
                    //    {
                    //        var cashData = context.BankExpenses.Where(i => i.UserId == selectedUserId && i.Date.Value.Year == DateTime.Now.Year && i.IsVirtualEntry != true && i.IsDeleted == false && i.BankId == 6).ToList();
                    //        ExpenseData.AddRange(cashData);
                    //    }

                    //}
                    //else
                    //{
                    //    if (Year > 0)
                    //    {
                    //        var year = context.tblYears.Where(y => y.Id == Year).Select(t => t.Year).FirstOrDefault();
                    //        var cashData = context.BankExpenses.Where(i => i.UserId == userId && i.Date.Value.Year == year && i.IsDeleted == false && i.IsVirtualEntry != true && i.BankId == 6).ToList();
                    //        ExpenseData.AddRange(cashData);
                    //    }
                    //    else
                    //    {
                    //        var cashData = context.BankExpenses.Where(i => i.UserId == userId && i.Date.Value.Year == DateTime.Now.Year && i.IsVirtualEntry != true && i.IsDeleted == false && i.BankId == 6).ToList();
                    //        ExpenseData.AddRange(cashData);
                    //    }

                    //}
                    if (Year > 0)
                    {
                        var year = context.tblYears.Where(y => y.Id == Year).Select(t => t.Year).FirstOrDefault();
                        ExpenseData = ExpenseData.Where(i => i.Date.Value.Year == Convert.ToInt32(year)).ToList();
                    }
                    //if (BankId != "")
                    //{

                    //    // ExpenseData = ExpenseData.Where(i => i.BankId == BankId).ToList();
                    //    String AccountType = "";
                    //    int newbankid = Convert.ToInt32(BankId.Split('_')[1].Trim());
                    //    if (BankId.Contains('-'))
                    //    {
                    //        AccountType = (BankId.Split('_')[0].Split('-')[1].Trim()).ToString();
                    //    }
                    //    else
                    //    {
                    //        AccountType = (BankId.Split('_')[0].Trim()).ToString();
                    //    }

                    //    if (AccountType == "MJV")
                    //    {
                    //        ExpenseData = ExpenseData.Where(i => i.AccountName.Trim().ToLower() == "cash" && i.BankId == newbankid).ToList();
                    //    }
                    //    else
                    //    {
                    //        ExpenseData = ExpenseData.Where(i => i.AccountName.Trim().ToLower() == AccountType.Trim().ToLower() && i.BankId == newbankid).ToList();
                    //    }

                    //}

                    if (BankId > 0)
                    {

                        ExpenseData = ExpenseData.Where(i => i.BankId == BankId).ToList();
                    }

                    if (Month > 0)
                    {
                        ExpenseData = ExpenseData.Where(i => i.Date.Value.Month == Month).ToList();
                    }
                    if (CategoryId > 0)
                    {

                        ExpenseData = ExpenseData.Where(s => s.CategoryId == CategoryId).ToList();

                    }
                    if (StatusId > 0)
                    {
                        ExpenseData = ExpenseData.Where(i => i.StatusId == StatusId).ToList();
                    }

                    if (JVID > 0)
                    {
                        if (Year > 0)
                        { }
                        else
                            ExpenseData = ExpenseData.Where(i => i.JVID == JVID).ToList();
                    }
                    TotalCount = ExpenseData.Count();

                    ExpenseData = ExpenseData.OrderBy(u => u.Id).Skip(Skip).Take(Take).ToList();

                    BankStatementList = ExpenseData.Select(i => new StatementList
                    {
                        Id = i.Id,
                        JVID = i.JVID.ToString().PadLeft(4, '0'),
                        StatementDescription = i.Description == null ? "" : i.Description,
                        StatementDate = i.Date.Value.ToShortDateString(),
                        Credit = i.Credit == null ? 0 : i.Credit,
                        Debit = i.Debit == null ? 0 : i.Debit,
                        StatementClassification = i.Classification.ClassificationType,
                        StatementBank = i.Bank.BankName,
                        StatementUploadType = i.UploadType == null ? "" : i.UploadType,
                        StatementStatus = i.Status.StatusType
                    }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new StatementListDto { TotalCount = TotalCount, StatementList = BankStatementList };
        }

        /// <summary>
        /// Lock bank statemnet for certain period
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="userId"></param>
        /// <param name="StatusId"></param>
        /// <returns></returns>
        public bool LockBankExpense(int month, int year, int userId, int StatusId)
        {
            bool updateExpense = false;
            using (var context = new KFentities())
            {
                try
                {
                    var data = context.BankExpenses.Where(i => i.UserId == userId && i.IsDeleted == false).ToList();
                    var Year = context.tblYears.Where(y => y.Id == year).Select(t => t.Year).FirstOrDefault();
                    data = data.Where(i => i.Date.Value.Year == Convert.ToInt32(Year)).ToList();
                    //var Month = context.tblMonths.Where(y => y.Id == month).Select(t => t.Month).FirstOrDefault();
                    data = data.Where(i => i.Date.Value.Month == Convert.ToInt32(month)).ToList();
                    //  data = data.Where(m => m.StatusId != 2).ToList(); //2 is status id to Submit status
                    data.ForEach(u => u.StatusId = StatusId);
                    context.SaveChanges();
                    updateExpense = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return updateExpense;
        }

        public List<StatementList> GetPrintData(int userId, int? StatusId, int? Year, int? Month, int? BankId, int? JVID, int? CategoryId)
        {
            List<BankExpense> ExpenseData = new List<BankExpense>();
            try
            {
                using (var context = new KFentities())
                {
                    //user as an accountant
                    ExpenseData = context.BankExpenses.Where(i => i.UserId == userId && i.IsDeleted == false && i.IsVirtualEntry != true && i.BankId != 6).ToList();

                    if (Year > 0)
                    {
                        var year = context.tblYears.Where(y => y.Id == Year).Select(t => t.Year).FirstOrDefault();
                        var cashData = context.BankExpenses.Where(i => i.UserId == userId && i.Date.Value.Year == year && i.IsDeleted == false && i.IsVirtualEntry != true && i.BankId == 6).ToList();
                        ExpenseData.AddRange(cashData);
                    }
                    else
                    {
                        var cashData = context.BankExpenses.Where(i => i.UserId == userId && i.Date.Value.Year == DateTime.Now.Year && i.IsVirtualEntry != true && i.IsDeleted == false && i.BankId == 6).ToList();
                        ExpenseData.AddRange(cashData);
                    }


                    if (Year > 0)
                    {
                        var year = context.tblYears.Where(y => y.Id == Year).Select(t => t.Year).FirstOrDefault();
                        ExpenseData = ExpenseData.Where(i => i.Date.Value.Year == Convert.ToInt32(year)).ToList();
                    }
                    if (BankId > 0)
                    {

                        ExpenseData = ExpenseData.Where(i => i.BankId == BankId).ToList();
                    }
                    if (Month > 0)
                    {
                        ExpenseData = ExpenseData.Where(i => i.Date.Value.Month == Month).ToList();
                    }
                    if (CategoryId > 0)
                    {
                        ExpenseData = ExpenseData.Where(i => i.CategoryId == CategoryId).ToList();
                    }
                    if (StatusId > 0)
                    {
                        ExpenseData = ExpenseData.Where(i => i.StatusId == StatusId).ToList();
                    }

                    if (JVID > 0)
                    {

                        ExpenseData = ExpenseData.Where(i => i.Id == JVID).ToList();
                    }

                    List<StatementList> PrintedDataList = new List<StatementList>();
                    PrintedDataList = ExpenseData.Select(i => new StatementList()
                    {
                        //Bind Expense Properties here
                        JVID = Convert.ToString(i.JVID).PadLeft(4, '0'),
                        StatementDescription = i.Description,
                        Credit = i.Credit,
                        Debit = i.Debit,
                        StatementClassification = context.Classifications.Where(u => u.Id == i.ClassificationId).Select(o => o.ClassificationType).FirstOrDefault(),
                        StatementStatus = context.Status.Where(s => s.Id == i.StatusId).Select(d => d.StatusType).FirstOrDefault(),
                        StatementDate = i.Date.Value.Date.ToString("yyyy-MM-dd"),
                        StatementBank = context.Banks.Where(o => o.Id == i.BankId).Select(p => p.BankName).FirstOrDefault(),
                        StatementUploadType = i.UploadType
                    }).ToList();
                    return PrintedDataList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public decimal CalculateTax(decimal BillTotal, decimal TotalTax, decimal taxValue)
        {
            decimal percValue = 0;
            decimal amount = Decimal.Add(BillTotal, TotalTax);
            percValue = taxValue * 100 / amount;
            return percValue;
        }
        #endregion


        public List<ReconcillationBankExpenseDto> GetExpensesForUserWithAnAccountantByUserId(int userId, int selectedUserId, int? StatusId, int roleId, int Skip, int Take, int? Year, int? Month, int? BankId, int? CategoryId)
        {
            List<ReconcillationBankExpenseDto> ObjLstExpenses = new List<ReconcillationBankExpenseDto>();
            List<BankExpense> ExpenseData = new List<BankExpense>();
            try
            {
                using (var context = new KFentities())
                {
                    ExpenseData = context.BankExpenses.Where(i => i.UserId == userId && i.IsDeleted == false).OrderByDescending(a => a.Id).ToList();

                    int TotalCount = ExpenseData.Count();
                    ExpenseData = ExpenseData.OrderByDescending(u => u.Id).Skip(Skip).Take(Take).Distinct().ToList();
                    ObjLstExpenses = ExpenseData.Select(i => new ReconcillationBankExpenseDto()
                    {
                        //Bind Expense Properties here
                        Id = i.Id,
                        UserId = Convert.ToInt32(i.UserId),
                        Description = i.Description,
                        Username = context.UserRegistrations.Where(o => o.Id == userId).Select(p => p.Username).FirstOrDefault(),
                        Credit = i.Credit,
                        Debit = i.Debit,
                        Total = i.Total,
                        ClassificationId = i.ClassificationId,
                        Classification = context.Classifications.Where(u => u.Id == i.ClassificationId).Select(o => o.ClassificationType).FirstOrDefault(),
                        // ExpenseId = Convert.ToInt32(i.ExpenseId),
                        // Status = i.sta.StatusType,
                        TransactionType = i.TransactionType,
                        Purpose = i.Purpose,
                        Category = i.Category.CategoryType,
                        Comment = i.Comments,
                        Date = i.Date.Value.Date,
                        customDate = i.Date.Value.Date.ToString("yyyy-MM-dd"),
                        BankId = Convert.ToInt32(i.BankId),
                        Bank = context.Banks.Where(o => o.Id == i.BankId).Select(p => p.BankName).FirstOrDefault(),
                        TotalCount = TotalCount
                    }).OrderByDescending(w => w.Date).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ObjLstExpenses;
        }

        #region web create classification
        public bool CheckClassificationAccountNumberExistCheck(string ChartAccountNumber, int UserId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ChartAccountDisplayNumber == ChartAccountNumber && a.UserId == UserId).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        public bool CheckClassificationAccountNumberExistCheck(string ChartAccountNumber)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ChartAccountDisplayNumber == ChartAccountNumber).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        public bool CheckClassificationNameExistCheck(string Name)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ClassificationType == Name).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        public bool CheckWebClassificationNameExistCheck(string Name, int userId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ClassificationType == Name && a.UserId == userId).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        #endregion
    }
}
