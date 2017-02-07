using AutoMapper;
using KF.Dto.Modules.Finance;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Finance
{
    public class ExpenseRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Check Acc Number
        public Tuple<Boolean, Int32, String> CheckNumberOfAccountByUserId(int userId, int bankid, string FriendlyAccName, int BankType)
        {
            try
            {
                using (var db = new KFentities())
                {
                    //Tuple<Boolean, Int32> AccountNumber = new Tuple<bool, int>();
                    String Type = (BankType == 1 ? "bank" : "credits");
                    var accountNos = db.BankExpenses.Where(a => a.UserId == userId && a.AccountName != FriendlyAccName && a.AccountType == Type).Select(s => s.AccountName).Distinct().ToList();
                    //int accountNos = db.BankExpenses.Where(a => a.UserId == userId && a.AccountName != FriendlyAccName).Select(s => s.AccountName).Distinct().ToList().Count;
                    //return accountNos;
                    return new Tuple<Boolean, Int32, String>(db.Classifications.Where(f => f.ClassificationType.Equals(FriendlyAccName)).Any(), accountNos.Count, db.Classifications.Where(f => f.ClassificationType.Equals(FriendlyAccName)).Select(r=>r.ChartAccountDisplayNumber.Substring(5,4)).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Save CSV Bank Expense Data (Scotia,CIBC,TD,bmo,rbc)

        //Bank STatement Exist check
        public bool BankStatementExistCheck(DateTime dt, int userId, int bankId, string FreindlyAccountName, string filename)
        {
            //String.Format("{0:ddd, MMM d, yyyy}", data.StartDate);
            var date = dt.ToShortDateString();
            var a = String.Format("{0:yyyy/MM/dd}", dt);
            bool IsExisted = false;
            using (var context = new KFentities())
            {
                try
                {
                    var data = context.BankExpenses.Where(i => i.UserId == userId && i.BankId == bankId && i.IsDeleted == false).OrderByDescending(z => z.Id).FirstOrDefault(q => q.Date.Value.Year == dt.Year && q.Date.Value.Month == dt.Month);
                    //var d1ata = context.BankExpenses.Where(i => i.UserId == userId && i.BankId == bankId).FirstOrDefault(r => EntityFunctions.TruncateTime(r.Date) == EntityFunctions.TruncateTime(dt));
                    if (data != null)
                    {
                        if (data.AccountName == FreindlyAccountName)
                        {
                            if (data.UploadedStatementName == filename)
                            {
                                IsExisted = true;
                            }
                        }


                    }
                    //  IsExisted = context.BankExpenses.Where(i => i.Date.Equals(dt.Date) && i.UserId == userId && i.BankId == bankId).Any();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return IsExisted;
        }

        public int GetJvid(int userID, int year)
        {
            try
            {
                int JVID = 0;
                using (var db = new KFentities())
                {
                    var lastStatementDetail = db.BankExpenses.Where(d => d.UserId == userID && d.Date.Value.Year == year).OrderByDescending(f => f.Id).FirstOrDefault();
                    if (lastStatementDetail != null)
                    {
                        JVID = Convert.ToInt32(lastStatementDetail.JVID + 1);
                    }
                    else
                    {
                        JVID = 0001;
                    }
                    return JVID;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool SaveBankExpense(List<BankExpense> _bankexpense, string UpdateDate, int bankid, int userId, string FriendaccName, string filename, string UploadType, int selectedStatementType, int chkAccNos)
        {

            /// return virtual
            using (var db = new KFentities())
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        int Year = _bankexpense[0].Date.Value.Year;
                        int JVID = this.GetJvid(userId, Year);


                        this.AddUserReconcilationType(userId, bankid);
                        string AccountType = string.Empty;
                        // _bankexpense.ForEach(i => i.IsDeleted = false);
                        // _bankexpense.ForEach(i => i.UploadType = UploadType);
                        // _bankexpense.ForEach(i => i.UploadedStatementName = filename);
                        if (selectedStatementType == 1)
                        {
                            AccountType = "bank";
                        }
                        else if (selectedStatementType == 2)
                        {
                            AccountType = "credits";
                        }
                        //  _bankexpense.ForEach(a => a.AccountType = AccountType);
                        //  _bankexpense.ForEach(a => a.AccountClassificationId = chkAccNos);
                        int StatusId = db.Status.Where(d => d.StatusType == "Pending").Select(d => d.Id).FirstOrDefault();
                        foreach (var item in _bankexpense)
                        {
                            item.JVID = JVID;
                            item.IsDeleted = false;
                            item.UploadType = UploadType;
                            item.UploadedStatementName = filename;
                            item.AccountType = AccountType;
                            item.AccountClassificationId = chkAccNos;
                            item.AccountName = FriendaccName;
                            item.StatusId = StatusId;
                            JVID++;
                        }
                        if (string.IsNullOrEmpty(UpdateDate))
                        {

                            //int year = 0;
                            //decimal Bankamount = 0;

                            //foreach (var itm in _bankexpense)
                            //{

                            //    Bankamount = decimal.Add(Bankamount, Convert.ToDecimal(itm.Credit));
                            //    year = itm.Date.Value.Year;


                            //}
                            // _bankexpense.ForEach(i => i.AccountName = FriendaccName);





                            BankExpense _expense = new BankExpense();
                            db.BankExpenses.AddRange(_bankexpense);
                            //var lastbanckblance = db.tblBankOpeningBalances.Where(s => s.UserId == userId && s.Year == year - 1).Select(s => s.TotalBalance).FirstOrDefault();
                            //decimal prevbankbalance = 0;
                            //if (lastbanckblance != null)
                            //{
                            //    prevbankbalance = Convert.ToDecimal(lastbanckblance.Value);
                            //}

                            // db.tblBankOpeningBalances.Add(new tblBankOpeningBalance { UserId = userId, Year = year, ChartAccountNumber = "", AccountName = FriendaccName, TotalBalance = decimal.Add(prevbankbalance, Bankamount), CreatedDate = DateTime.Now, IsDeleted = false });
                            db.SaveChanges();

                        }

                        #region Add FriendlyAccNameClassification
                        string ChartNo = Convert.ToString(chkAccNos);
                        var classificationNumberDetails = db.Classifications.Where(s => s.UserId == userId && s.ClassificationType == FriendaccName).FirstOrDefault();
                        if (classificationNumberDetails != null)
                        {
                            if (classificationNumberDetails.ChartAccountDisplayNumber.Split('-')[1].ToString() == ChartNo)
                            {

                            }
                            else
                            {
                                var dbInsert = new Classification();
                                dbInsert.CategoryId = 1; //Asset Entry


                                if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                                }
                                else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);
                                }
                                else
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                }
                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsDeleted = false;
                                dbInsert.IsIncorporated = false;
                                dbInsert.IsPartnerShip = false;
                                dbInsert.IsSole = false;
                                dbInsert.UserId = userId;
                                dbInsert.Desc = FriendaccName;
                                dbInsert.ClassificationType = FriendaccName;
                                dbInsert.Name = FriendaccName;
                                dbInsert.Type = "A";
                                db.Classifications.Add(dbInsert);
                            }
                        }

                        else
                        {
                            var dbInsert = new Classification();
                            dbInsert.CategoryId = 1; //Asset Entry


                            if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                            }
                            else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);
                            }
                            else
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                            }
                            dbInsert.CreatedDate = DateTime.Now;
                            dbInsert.IsDeleted = false;
                            dbInsert.IsIncorporated = false;
                            dbInsert.IsPartnerShip = false;
                            dbInsert.IsSole = false;
                            dbInsert.UserId = userId;
                            dbInsert.Desc = FriendaccName;
                            dbInsert.ClassificationType = FriendaccName;
                            dbInsert.Name = FriendaccName;
                            dbInsert.Type = "A";
                            db.Classifications.Add(dbInsert);
                        }

                        #endregion

                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool SaveScotiaBankExpense(List<ReadScotiaBankDetails> ObjLst, string UpdateDate, int bankid, int userId, string FriendaccName, string filename, string UploadType, int selectedStatementType, int chkAccNos)
        {
            using (var db = new KFentities())
            {

                List<BankExpense> _bankexpense = new List<BankExpense>();
                _bankexpense = ObjLst.Select(i => new BankExpense()
                {
                    BankId = i.BankId,
                    ClassificationId = i.ClassificationId,
                    CategoryId = i.CategoryId,
                    UserId = Convert.ToInt32(i.UserId),
                    Date = i.Date,
                    CreatedDate = DateTime.Now,
                    Description = i.Description,
                    Credit = Convert.ToDecimal(i.Credit),
                    Debit = Convert.ToDecimal(i.Debit),
                }).ToList();

                int Year = ObjLst[0].Date.Value.Year;
                int JVID = this.GetJvid(userId, Year);
                int StatusId = db.Status.Where(d => d.StatusType == "Pending").Select(d => d.Id).FirstOrDefault();
                foreach (var item in _bankexpense)
                {
                    item.JVID = JVID;
                    item.IsDeleted = false;
                    item.UploadType = UploadType;
                    item.UploadedStatementName = filename;
                    if (selectedStatementType == 1)
                    {
                        item.AccountType = "bank";
                    }
                    else if (selectedStatementType == 2)
                    {
                        item.AccountType = "credits";
                    }
                    item.AccountClassificationId = chkAccNos;
                    item.AccountName = FriendaccName;
                    item.StatusId = StatusId;
                    JVID++;
                }



                //if (selectedStatementType == 1)
                //{
                //    _bankexpense.ForEach(a => a.AccountType = "bank");
                //}
                //else if (selectedStatementType == 2)
                //{
                //    _bankexpense.ForEach(a => a.AccountType = "credits");
                //}
                // _bankexpense.ForEach(a => a.AccountClassificationId = chkAccNos);
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        if (string.IsNullOrEmpty(UpdateDate))
                        {
                            BankExpense _expense = new BankExpense();
                            db.BankExpenses.AddRange(_bankexpense);
                        }

                        #region Add FriendlyAccNameClassification

                        string ChartNo = Convert.ToString(chkAccNos);
                        var classificationNumberDetails = db.Classifications.Where(s => s.UserId == userId && s.ClassificationType == FriendaccName).FirstOrDefault();
                        if (classificationNumberDetails != null)
                        {
                            if (classificationNumberDetails.ChartAccountDisplayNumber.Split('-')[1].ToString() == ChartNo)
                            {

                            }
                            else
                            {
                                var dbInsert = new Classification();
                                dbInsert.CategoryId = 1; //Asset Entry
                                if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                                }
                                else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                                }
                                else
                                {
                                    dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                }
                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsDeleted = false;
                                dbInsert.IsIncorporated = false;
                                dbInsert.IsPartnerShip = false;
                                dbInsert.IsSole = false;
                                dbInsert.UserId = userId;
                                dbInsert.Desc = FriendaccName;
                                dbInsert.ClassificationType = FriendaccName;
                                dbInsert.Name = FriendaccName;
                                dbInsert.Type = "A";
                                db.Classifications.Add(dbInsert);
                            }
                        }

                        else
                        {
                            var dbInsert = new Classification();
                            dbInsert.CategoryId = 1; //Asset Entry


                            if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                            }
                            else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                            }
                            else
                            {
                                dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                            }
                            dbInsert.CreatedDate = DateTime.Now;
                            dbInsert.IsDeleted = false;
                            dbInsert.IsIncorporated = false;
                            dbInsert.IsPartnerShip = false;
                            dbInsert.IsSole = false;
                            dbInsert.UserId = userId;
                            dbInsert.Desc = FriendaccName;
                            dbInsert.ClassificationType = FriendaccName;
                            dbInsert.Name = FriendaccName;
                            dbInsert.Type = "A";
                            db.Classifications.Add(dbInsert);
                        }
                        #endregion
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool SaveBmoBankExpense(List<ReadBmoBankDetails> ObjLst, string UpdateDate, int bankid, int userId, string FriendaccName, string filename, string UploadType, int selectedStatementType, int chkAccNos)
        {
            using (var db = new KFentities())
            {
                List<BankExpense> _bankexpense = new List<BankExpense>();
                _bankexpense = ObjLst.Select(i => new BankExpense()
                {
                    BankId = i.BankId,
                    ClassificationId = i.ClassificationId,
                    CategoryId = i.CategoryId,
                    UserId = Convert.ToInt32(i.UserId),
                    Date = i.Date,
                    CreatedDate = DateTime.Now,
                    Description = i.Description,
                    AccountNumber = i.AccountNo,
                    Credit = Convert.ToDecimal(i.Credit),
                    Debit = Convert.ToDecimal(i.Debit),
                    IsDeleted = false,
                }).ToList();


                int Year = ObjLst[0].Date.Value.Year;
                int JVID = this.GetJvid(userId, Year);
                int StatusId = db.Status.Where(d => d.StatusType == "Pending").Select(d => d.Id).FirstOrDefault();
                foreach (var item in _bankexpense)
                {
                    item.JVID = JVID;
                    item.IsDeleted = false;
                    item.UploadType = UploadType;
                    item.UploadedStatementName = filename;
                    if (selectedStatementType == 1)
                    {
                        item.AccountType = "bank";
                    }
                    else if (selectedStatementType == 2)
                    {
                        item.AccountType = "credits";
                    }
                    item.AccountClassificationId = chkAccNos;
                    item.AccountName = FriendaccName;
                    item.StatusId = StatusId;
                    JVID++;
                }

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(UpdateDate))
                        {
                            BankExpense _expense = new BankExpense();
                            db.BankExpenses.AddRange(_bankexpense);
                        }

                        #region Add FriendlyAccNameClassification
                        string ChartNo = Convert.ToString(chkAccNos);
                        var classificationNumberDetails = db.Classifications.Where(s => s.UserId == userId && s.ClassificationType == FriendaccName).FirstOrDefault();
                        if (classificationNumberDetails != null)
                        {
                            if (classificationNumberDetails.ChartAccountDisplayNumber.Split('-')[1].ToString() == ChartNo)
                            {

                            }
                            else
                            {
                                var dbInsert = new Classification();
                                dbInsert.CategoryId = 1; //Asset Entry


                                if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                                }
                                else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                                }
                                else
                                {
                                    dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                }
                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsDeleted = false;
                                dbInsert.IsIncorporated = false;
                                dbInsert.IsPartnerShip = false;
                                dbInsert.IsSole = false;
                                dbInsert.UserId = userId;
                                dbInsert.Desc = FriendaccName;
                                dbInsert.ClassificationType = FriendaccName;
                                dbInsert.Name = FriendaccName;
                                dbInsert.Type = "A";
                                db.Classifications.Add(dbInsert);
                            }
                        }

                        else
                        {
                            var dbInsert = new Classification();
                            dbInsert.CategoryId = 1; //Asset Entry


                            if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                            }
                            else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                            }
                            else
                            {
                                dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                            }
                            dbInsert.CreatedDate = DateTime.Now;
                            dbInsert.IsDeleted = false;
                            dbInsert.IsIncorporated = false;
                            dbInsert.IsPartnerShip = false;
                            dbInsert.IsSole = false;
                            dbInsert.UserId = userId;
                            dbInsert.Desc = FriendaccName;
                            dbInsert.ClassificationType = FriendaccName;
                            dbInsert.Name = FriendaccName;
                            dbInsert.Type = "A";
                            db.Classifications.Add(dbInsert);
                        }



                        #endregion
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool SaveTdBankExpense(List<ReadTdDetails> ObjLst, string UpdateDate, int bankid, int userId, string FriendaccName, string filename, string UploadType, int selectedStatementType, int chkAccNos)
        {
            using (var db = new KFentities())
            {
                this.AddUserReconcilationType(userId, bankid);
                List<BankExpense> _bankexpense = new List<BankExpense>();
                _bankexpense = ObjLst.Select(i => new BankExpense()
                {
                    BankId = i.BankId,
                    ClassificationId = i.ClassificationId,
                    CategoryId = i.CategoryId,
                    UserId = Convert.ToInt32(i.UserId),
                    Date = i.Date,
                    CreatedDate = DateTime.Now,
                    Description = i.Withdrawal,
                    Credit = Convert.ToDecimal(i.Credit),
                    Debit = Convert.ToDecimal(i.Debit),
                    Total = Convert.ToDecimal(i.Balance),
                    StatusId = 1,
                    IsDeleted = false,
                    UploadType = UploadType,
                    AccountName = FriendaccName,
                    UploadedStatementName = filename
                }).ToList();


                int Year = ObjLst[0].Date.Value.Year;
                int JVID = this.GetJvid(userId, Year);
                int StatusId = db.Status.Where(d => d.StatusType == "Pending").Select(d => d.Id).FirstOrDefault();
                foreach (var item in _bankexpense)
                {
                    item.JVID = JVID;
                    item.IsDeleted = false;
                    item.UploadType = UploadType;
                    item.UploadedStatementName = filename;
                    if (selectedStatementType == 1)
                    {
                        item.AccountType = "bank";
                    }
                    else if (selectedStatementType == 2)
                    {
                        item.AccountType = "credits";
                    }
                    item.AccountClassificationId = chkAccNos;
                    item.AccountName = FriendaccName;
                    item.StatusId = StatusId;
                    JVID++;
                }

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(UpdateDate))
                        {
                            BankExpense _expense = new BankExpense();
                            db.BankExpenses.AddRange(_bankexpense);
                        }

                        #region Add FriendlyAccNameClassification

                        string ChartNo = Convert.ToString(chkAccNos);
                        var classificationNumberDetails = db.Classifications.Where(s => s.UserId == userId && s.ClassificationType == FriendaccName).FirstOrDefault();
                        if (classificationNumberDetails != null)
                        {
                            if (classificationNumberDetails.ChartAccountDisplayNumber.Split('-')[1].ToString() == ChartNo)
                            {

                            }
                            else
                            {
                                var dbInsert = new Classification();
                                dbInsert.CategoryId = 1; //Asset Entry
                                if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                                }
                                else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                                }
                                else
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                }
                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsDeleted = false;
                                dbInsert.IsIncorporated = false;
                                dbInsert.IsPartnerShip = false;
                                dbInsert.IsSole = false;
                                dbInsert.UserId = userId;
                                dbInsert.Desc = FriendaccName;
                                dbInsert.ClassificationType = FriendaccName;
                                dbInsert.Name = FriendaccName;
                                dbInsert.Type = "A";
                                db.Classifications.Add(dbInsert);
                            }
                        }

                        else
                        {
                            var dbInsert = new Classification();
                            dbInsert.CategoryId = 1; //Asset Entry


                            if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                            }
                            else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                            }
                            else
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                            }
                            dbInsert.CreatedDate = DateTime.Now;
                            dbInsert.IsDeleted = false;
                            dbInsert.IsIncorporated = false;
                            dbInsert.IsPartnerShip = false;
                            dbInsert.IsSole = false;
                            dbInsert.UserId = userId;
                            dbInsert.Desc = FriendaccName;
                            dbInsert.ClassificationType = FriendaccName;
                            dbInsert.Name = FriendaccName;
                            dbInsert.Type = "A";
                            db.Classifications.Add(dbInsert);
                        }
                        #endregion
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool SaveCibcBankExpense(List<ReadCisvDetails> ObjLst, string UpdateDate, int bankid, int userId, string FriendaccName, string filename, string UploadType, int selectedStatementType, int chkAccNos)
        {
            using (var db = new KFentities())
            {
                List<BankExpense> _bankexpense = new List<BankExpense>();
                _bankexpense = ObjLst.Select(i => new BankExpense()
                {
                    BankId = i.BankId,
                    ClassificationId = i.ClassificationId,
                    CategoryId = i.CategoryId,
                    UserId = Convert.ToInt32(i.UserId),
                    Date = i.Date,
                    CreatedDate = DateTime.Now,
                    Description = i.Transaction,
                    Credit = Convert.ToDecimal(i.Credit),
                    Debit = Convert.ToDecimal(i.Debit),
                }).ToList();


                int Year = ObjLst[0].Date.Value.Year;
                int JVID = this.GetJvid(userId, Year);
                int StatusId = db.Status.Where(d => d.StatusType == "Pending").Select(d => d.Id).FirstOrDefault();
                foreach (var item in _bankexpense)
                {
                    item.JVID = JVID;
                    item.IsDeleted = false;
                    item.UploadType = UploadType;
                    item.UploadedStatementName = filename;
                    if (selectedStatementType == 1)
                    {
                        item.AccountType = "bank";
                    }
                    else if (selectedStatementType == 2)
                    {
                        item.AccountType = "credits";
                    }
                    item.AccountClassificationId = chkAccNos;
                    item.AccountName = FriendaccName;
                    item.StatusId = StatusId;
                    JVID++;
                }

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(UpdateDate))
                        {
                            BankExpense _expense = new BankExpense();
                            db.BankExpenses.AddRange(_bankexpense);
                        }

                        #region Add FriendlyAccNameClassification
                        string ChartNo = Convert.ToString(chkAccNos);
                        var classificationNumberDetails = db.Classifications.Where(s => s.UserId == userId && s.ClassificationType == FriendaccName).FirstOrDefault();
                        if (classificationNumberDetails != null)
                        {
                            if (classificationNumberDetails.ChartAccountDisplayNumber.Split('-')[1].ToString() == ChartNo)
                            {

                            }
                            else
                            {
                                var dbInsert = new Classification();
                                dbInsert.CategoryId = 1; //Asset Entry

                                if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                                }
                                else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                                }
                                else
                                {
                                    dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                    dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                                }
                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsDeleted = false;
                                dbInsert.IsIncorporated = false;
                                dbInsert.IsPartnerShip = false;
                                dbInsert.IsSole = false;
                                dbInsert.UserId = userId;
                                dbInsert.Desc = FriendaccName;
                                dbInsert.ClassificationType = FriendaccName;
                                dbInsert.Name = FriendaccName;
                                dbInsert.Type = "A";
                                db.Classifications.Add(dbInsert);
                            }
                        }

                        else
                        {
                            var dbInsert = new Classification();
                            dbInsert.CategoryId = 1; //Asset Entry

                            if (chkAccNos == 1060 || chkAccNos == 1061 || chkAccNos == 1062 || chkAccNos == 1063)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1050-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1050" + chkAccNos);
                            }
                            else if (chkAccNos == 1080 || chkAccNos == 1081 || chkAccNos == 1082 || chkAccNos == 1083)
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString("1100-" + chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32("1100" + chkAccNos);

                            }
                            else
                            {
                                dbInsert.ChartAccountDisplayNumber = Convert.ToString(chkAccNos);
                                dbInsert.ChartAccountNumber = Convert.ToInt32(chkAccNos);
                            }
                            dbInsert.CreatedDate = DateTime.Now;
                            dbInsert.IsDeleted = false;
                            dbInsert.IsIncorporated = false;
                            dbInsert.IsPartnerShip = false;
                            dbInsert.IsSole = false;
                            dbInsert.UserId = userId;
                            dbInsert.Desc = FriendaccName;
                            dbInsert.ClassificationType = FriendaccName;
                            dbInsert.Name = FriendaccName;
                            dbInsert.Type = "A";
                            db.Classifications.Add(dbInsert);
                        }

                        #endregion
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Add MJV entries
        /// </summary>
        /// <param name="ObjData"></param>
        /// <returns></returns>
        public int SaveMjvEntry(AddMjvEntryDto ObjData)
        {
            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int Year = Convert.ToDateTime(ObjData.Date).Year;
                        int ? JVID;

                        BankExpense ObjBank = new BankExpense();
                        if (ObjData.ReferenceId > 0)
                        {
                            ObjBank.StatementReferenceNumber = ObjData.ReferenceId;
                            JVID = db.BankExpenses.Where(d=>d.Id==ObjData.ReferenceId).Select(f=>f.JVID).FirstOrDefault();
                        }
                        else
                        {
                            JVID = this.GetJvid(ObjData.UserId, Year);
                        }
                        if (ObjData.IsVirtualEntry == true)
                        {
                            ObjBank.IsVirtualEntry = true;
                        }
                        ObjBank.UserId = ObjData.UserId;
                        ObjBank.JVID = JVID;
                        ObjBank.AccountType = "MJV";
                        ObjBank.StatusId = 1;//1 for pending status
                        ObjBank.BankId = 6; //for MJV only
                        ObjBank.Comments = ObjData.Comment;
                        ObjBank.Purpose = ObjData.Purpose;
                        ObjBank.UploadType = "M";
                        ObjBank.AccountClassificationId = 1030;
                        ObjBank.Vendor = ObjData.Vendor;
                        ObjBank.Total = ObjData.Total;
                        ObjBank.TransactionType = ObjData.TransactionType;
                        ObjBank.CategoryId = ObjData.CategoryId;
                        ObjBank.ClassificationId = ObjData.ClassificationId;//cash classification entry
                        ObjBank.Credit = ObjData.Credit;
                        ObjBank.Debit = ObjData.Debit;
                        ObjBank.Description = ObjData.Description;
                        if (ObjData.Credit > 0)
                        {
                            ObjData.BillTotal = ObjData.Credit;
                        }
                        if (ObjData.Debit > 0)
                        {
                            ObjData.BillTotal = ObjData.Debit;
                        }
                        ObjBank.Total = ObjData.BillTotal;
                        ObjBank.ActualTotal = ObjData.BillTotal;
                        ObjBank.TotalTax = ObjData.GSTtax + ObjData.QSTtax + ObjData.HSTtax + ObjData.PSTtax;
                        ObjBank.GSTtax = 0;
                        ObjBank.QSTtax = 0;
                        ObjBank.HSTtax = 0;
                        ObjBank.PSTtax = 0;
                        ObjBank.GSTPercentage = 0;
                        ObjBank.QSTPercentage = 0;
                        ObjBank.HSTPercentage = 0;
                        ObjBank.PSTPercentage = 0;
                        ObjBank.ActualPercentage = ObjData.ActualPercentage;

                        var chartAccNo = db.Classifications.Where(s => s.Id == ObjData.ClassificationId).FirstOrDefault();
                        if (chartAccNo.ChartAccountDisplayNumber == "1050-1060" || chartAccNo.ChartAccountDisplayNumber == "1050-1061" || chartAccNo.ChartAccountDisplayNumber == "1050-1062" ||
                            chartAccNo.ChartAccountDisplayNumber == "1050-1063" || chartAccNo.ChartAccountDisplayNumber == "1050-1064" || chartAccNo.ChartAccountDisplayNumber == "1050-1073" ||
                            chartAccNo.ChartAccountDisplayNumber == "1100-1080" || chartAccNo.ChartAccountDisplayNumber == "1100-1081" ||
                            chartAccNo.ChartAccountDisplayNumber == "1100-1082" || chartAccNo.ChartAccountDisplayNumber == "1100-1083" || chartAccNo.ChartAccountDisplayNumber == "1100-1087" ||
                            chartAccNo.ChartAccountDisplayNumber == "1100-1088" || chartAccNo.ChartAccountDisplayNumber == "1100-1089")
                        {
                            ObjBank.AccountName = chartAccNo.ClassificationType;
                        }
                        else
                        {
                            ObjBank.AccountName = "Cash";
                        }
                        ObjBank.Date = Convert.ToDateTime(ObjData.Date);
                        ObjBank.CreatedDate = DateTime.Now;
                        ObjBank.IsDeleted = false;
                        ObjBank.StatusId = 4;
                        db.BankExpenses.Add(ObjBank);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return ObjBank.Id;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }

                }
                return 0;
            }
        }

        public void AddUserReconcilationType(int userid, int bankId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var existChk = db.UserRegistrations.Where(s => s.Id == userid).FirstOrDefault();
                    if (existChk != null)
                    {
                        if (existChk.ReconciliationType == null)
                        {
                            if (bankId > 0 && bankId <= 5)
                            {
                                existChk.ReconciliationType = 1;//manual
                            }
                            else
                            {
                                existChk.ReconciliationType = 2;//autonmatic
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region statement exist check
        public bool RbcBankStatementExistCheck(DateTime dt, int userId, int bankId, string FreindlyAccountName, string Filename, string accNo)
        {
            //String.Format("{0:ddd, MMM d, yyyy}", data.StartDate);
            var date = dt.ToShortDateString();
            var a = String.Format("{0:yyyy/MM/dd}", dt);
            bool IsExisted = false;
            using (var context = new KFentities())
            {
                try
                {
                    var data = context.BankExpenses.Where(i => i.UserId == userId && i.BankId == bankId && i.IsDeleted == false).OrderByDescending(z => z.Id).FirstOrDefault(q => q.Date.Value.Year == dt.Year && q.Date.Value.Month == dt.Month);

                    if (data != null)
                    {
                        if (data.AccountNumber == accNo)
                        {
                            if (data.AccountName == FreindlyAccountName)
                            {
                                if (data.UploadedStatementName == Filename)
                                {
                                    IsExisted = true;
                                }
                            }
                        }



                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return IsExisted;
        }

        #endregion

        #region Check Fiscal Year Closer
        public bool CheckFiscalYearCloser(int userId, string excelDate)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.FiscalYearPostings.Where(s => s.UserId == userId).Any())
                    {
                        var seletedUserData = db.UserRegistrations.Where(s => s.Id == userId).FirstOrDefault();
                        string FiscalYearstring = seletedUserData.TaxStartMonthId + "/" + seletedUserData.TaxationStartDay + "/" + seletedUserData.TaxStartYear;
                        DateTime FiscalYearstart = Convert.ToDateTime(FiscalYearstring);
                        DateTime FiscalYearEnd = FiscalYearstart.AddYears(1);
                        DateTime UploadDataDate = Convert.ToDateTime(excelDate);

                        var closingfiscalyear = db.FiscalYearPostings.Where(s => s.UserId == userId).Select(s => s.TaxStartYear).FirstOrDefault();

                        var closingfiscalyr = Convert.ToDateTime(closingfiscalyear).ToString("yyyy");

                        if (closingfiscalyr == UploadDataDate.ToString("yyyy"))
                            return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        public bool AddImageToCloud(CloudImagesRecordDto obj)
        {
            bool IsSave = false;
            try
            {
                using (var db = new KFentities())
                {
                    var kippinStoreImages = db.KippinStoreImages.Where(a => a.ImageName == obj.ImageName && a.Month == obj.Month && a.Year == obj.Year).ToList();
                    if (kippinStoreImages.Count > 0)
                    {
                        foreach (var item in kippinStoreImages)
                        {
                            var raw = db.KippinStoreImages.Where(i => i.Id == item.Id).FirstOrDefault();
                            db.KippinStoreImages.Attach(raw);
                            db.KippinStoreImages.Remove(raw);
                            db.SaveChanges();
                        }
                    }
                    Mapper.CreateMap<CloudImagesRecordDto, CloudImagesRecord>();
                    var data = Mapper.Map<CloudImagesRecord>(obj);
                    db.CloudImagesRecords.Add(data);
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

        public int SaveCashBankExpenseEntryFromMobile(AddCashExpenseDto ObjData)
        {
            using (var db = new KFentities())
            {
                //  bool IsSave = false;
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        #region First original entry
                        BankExpense ObjBank = new BankExpense();
                        int JVID = this.GetJvid(ObjData.UserId, Convert.ToDateTime(ObjData.Date).Year);
                        ObjBank.UserId = ObjData.UserId;
                        ObjBank.JVID = JVID;
                        ObjBank.StatusId = 2;//1 for pending status
                        ObjBank.BankId = 7; //for cash only
                        ObjBank.IsVirtualEntry = false;
                        ObjBank.Comments = ObjData.Comment;
                        ObjBank.Purpose = ObjData.Purpose;
                        ObjBank.UploadType = "M";
                        ObjBank.AccountType = "Cash";
                        ObjBank.AccountClassificationId = 1030;
                        ObjBank.Vendor = ObjData.Vendor;
                        ObjBank.Total = ObjData.Total;
                        ObjBank.TransactionType = ObjData.Credit > ObjData.Debit ? "Cr." : "Dr.";
                        ObjBank.CategoryId = ObjData.CategoryId;
                        ObjBank.ClassificationId = ObjData.ClassificationId;//cash classification entry
                        ObjBank.Credit = ObjData.Credit;
                        ObjBank.Debit = ObjData.Debit;
                        ObjBank.Description = ObjData.Description;
                        var chartAccNo = db.Classifications.Where(s => s.Id == ObjData.ClassificationId).FirstOrDefault();
                        if (chartAccNo.ChartAccountDisplayNumber == "1050-1060" || chartAccNo.ChartAccountDisplayNumber == "1050-1061" || chartAccNo.ChartAccountDisplayNumber == "1050-1062" || chartAccNo.ChartAccountDisplayNumber == "1050-1063")
                        {
                            ObjBank.AccountName = chartAccNo.ClassificationType;
                        }
                        else
                        {
                            ObjBank.AccountName = "Cash";
                        }

                        ObjBank.Date = Convert.ToDateTime(ObjData.Date);
                        ObjBank.CreatedDate = DateTime.Now;
                        ObjBank.IsDeleted = false;

                        ObjBank.ActualTotal = ObjData.BillTotal;
                        ObjBank.TotalTax = ObjData.GSTtax + ObjData.QSTtax + ObjData.HSTtax + ObjData.PSTtax;
                        ObjBank.GSTtax = ObjData.GSTtax;
                        ObjBank.QSTtax = ObjData.QSTtax;
                        ObjBank.HSTtax = ObjData.HSTtax;
                        ObjBank.PSTtax = ObjData.PSTtax;
                        ObjBank.ActualPercentage = ObjData.ActualPercentage;
                        var refId = db.BankExpenses.Add(ObjBank);
                        db.SaveChanges();
                        #endregion

                        #region Shadow entry
                        BankExpense ObjShadowBank = new BankExpense();
                        ObjShadowBank.UserId = ObjData.UserId;
                        ObjShadowBank.JVID = JVID + 1;
                        ObjShadowBank.StatusId = 2;//1 for pending status,2 for submitted
                        ObjShadowBank.BankId = 7; //for cash only
                        ObjShadowBank.IsVirtualEntry = true;
                        ObjShadowBank.Comments = ObjData.Comment;
                        ObjShadowBank.Purpose = ObjData.Purpose;
                        ObjShadowBank.AccountType = "Cash";
                        ObjShadowBank.UploadType = "M";
                        ObjShadowBank.AccountClassificationId = 1030;
                        ObjShadowBank.Vendor = ObjData.Vendor;
                        ObjShadowBank.Total = ObjData.Total;

                        ObjShadowBank.CategoryId = 1;
                        ObjShadowBank.StatementReferenceNumber = refId.Id;
                        ObjShadowBank.ClassificationId = db.Classifications.Where(d => d.ChartAccountDisplayNumber == "1050-1030").Select(g => g.Id).FirstOrDefault();//for shadow entry classification id is 95 i.e. Cash
                        if (ObjData.Credit > ObjData.Debit)
                        {
                            ObjShadowBank.Debit = ObjData.Credit;
                            ObjShadowBank.Credit = 0;
                            ObjShadowBank.TransactionType = "Dr.";
                        }
                        else
                        {
                            ObjShadowBank.Debit = 0;
                            ObjShadowBank.Credit = ObjData.Debit;
                            ObjShadowBank.TransactionType = "Cr.";
                        }
                        ObjShadowBank.Description = ObjData.Description;
                        var shadowChartAccNo = db.Classifications.Where(s => s.Id == ObjData.ClassificationId).FirstOrDefault();
                        if (shadowChartAccNo.ChartAccountDisplayNumber == "1050-1060" || shadowChartAccNo.ChartAccountDisplayNumber == "1050-1061" ||
                            shadowChartAccNo.ChartAccountDisplayNumber == "1050-1062" || shadowChartAccNo.ChartAccountDisplayNumber == "1050-1063")
                        {
                            ObjShadowBank.AccountName = shadowChartAccNo.ClassificationType;
                        }
                        else
                        {
                            ObjShadowBank.AccountName = "Cash";
                        }

                        ObjShadowBank.Date = Convert.ToDateTime(ObjData.Date);
                        ObjShadowBank.CreatedDate = DateTime.Now;
                        ObjShadowBank.IsDeleted = false;
                        ObjShadowBank.ActualTotal = ObjData.BillTotal;
                        ObjShadowBank.TotalTax = ObjData.GSTtax + ObjData.QSTtax + ObjData.HSTtax + ObjData.PSTtax;
                        ObjShadowBank.GSTtax = ObjData.GSTtax;
                        ObjShadowBank.QSTtax = ObjData.QSTtax;
                        ObjShadowBank.HSTtax = ObjData.HSTtax;
                        ObjShadowBank.PSTtax = ObjData.PSTtax;
                        ObjShadowBank.ActualPercentage = ObjData.ActualPercentage;
                        db.BankExpenses.Add(ObjShadowBank);
                        #endregion

                        db.SaveChanges();
                        #region enter a entry of shawdow record into other table
                        int bankExpenseId = ObjBank.Id;
                        var dbInsertShadowEntryRecord = new ShadowEntryRecord();
                        dbInsertShadowEntryRecord.BankExpenseId = ObjBank.Id;
                        dbInsertShadowEntryRecord.DateCreated = DateTime.Now;
                        dbInsertShadowEntryRecord.DateModified = DateTime.Now;
                        dbInsertShadowEntryRecord.IsDeleted = false;
                        dbInsertShadowEntryRecord.BankExpenseShadowId = ObjShadowBank.Id;
                        db.ShadowEntryRecords.Add(dbInsertShadowEntryRecord);
                        #endregion

                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return ObjBank.Id;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }

                }
                return 0;
            }
        }
    }
}
