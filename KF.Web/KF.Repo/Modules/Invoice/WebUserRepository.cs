using AutoMapper;
using KF.Dto.Modules.Invoice;
using KF.Entity;
using KF.ModelDto.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KF.Repo.Modules.Invoice
{
    public class WebUserRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Industry List
        public List<Industry> GetIndustryList()
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Industries.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Ownership List
        public List<Ownership> GetOwnershipList()
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Ownerships.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        

        

        #region Sub Industry List By Id
        public List<SelectListItem> GetSubIndustryListById(int industryId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var ObjYear = db.SubIndustries.Where(a => a.IndustryId == industryId && a.IsDeleted == false).ToList();
                    var Year = new List<SelectListItem>();
                    Year = ObjYear.Select(x => new SelectListItem { Text = x.SubIndustryName, Value = Convert.ToString(x.Id) }).ToList();
                    return Year;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region User Account Name
        public List<string> GetUserAccountName(int userId, int bankId, int? selectedStatementType)
        {
            List<string> objFriendlyAccName = new List<string>();
            try
            {

                using (var db = new KFentities())
                {
                    string accountType = selectedStatementType == 1 ? "bank" : "credits";
                    var friendlyAccountName = db.BankExpenses.Where(a => a.UserId == userId && a.BankId == bankId && a.AccountType == accountType).Select(s => s.AccountName).Distinct().ToList();
                    if (friendlyAccountName.Count > 0)
                    {
                        foreach (var item in friendlyAccountName)
                        {
                            if (string.IsNullOrEmpty(item))
                            {
                                switch (bankId)
                                {
                                    case 1:
                                        objFriendlyAccName.Add("RBC");
                                        break;
                                    case 2:
                                        objFriendlyAccName.Add("Scotia");
                                        break;
                                    case 3:
                                        objFriendlyAccName.Add("BMO");
                                        break;
                                    case 4:
                                        objFriendlyAccName.Add("TD");
                                        break;
                                    case 5:
                                        objFriendlyAccName.Add("CIBC");
                                        break;
                                }
                            }
                            else
                            {
                                objFriendlyAccName.Add(item);
                            }
                        }
                    }


                }
            }
            catch (Exception)
            {

                throw;
            }
            return objFriendlyAccName;
        }
        #endregion

        #region User Payment
        public decimal UserPaymentByRoldeId(int RoleId)
        {
            decimal IsPaid = 0;
            using (var dbContext = new KFentities())
            {
                var userDetails = dbContext.UserPrices.Where(i => i.RoldeId == RoleId).FirstOrDefault();
                if (userDetails != null)
                {

                    IsPaid = Convert.ToDecimal(userDetails.Price);
                }
            }
            return IsPaid;
        }
        #endregion

        #region User By Id
        public UserRegistration GetUserById(int Userid)
        {

            try
            {
                using (var db = new KFentities())
                {
                    return db.UserRegistrations.Where(x => x.Id == Userid).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Add Accountant
        public int AddAccountant(UserRegistrationDto registrationDetails)
        {
            int userId = 0;
            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        registrationDetails.IsDeleted = false;

                        Mapper.CreateMap<UserRegistrationDto, UserRegistration>();
                        var user = Mapper.Map<UserRegistration>(registrationDetails);
                        user.IsUnlink = false;
                        user.CreatedDate = DateTime.Now;
                        db.UserRegistrations.Add(user);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        return userId = user.Id;
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        return userId;
                        throw;
                    }
                }
            }
        }
        #endregion

        #region Activate User Account
        public bool ActivateUserAccount(int id)
        {
            bool IsPaid = false;
            using (var dbContext = new KFentities())
            {
                var userDetails = dbContext.UserRegistrations.Where(i => i.Id == id).FirstOrDefault();
                if (userDetails != null)
                {
                    userDetails.IsVerified = true;
                    userDetails.IsPaid = true;
                    userDetails.IsTrial = false;
                    dbContext.SaveChanges();
                    IsPaid = true;
                }
            }
            return IsPaid;
        }
        #endregion

        #region Forgot Password
        public string ForgotPassword(string email, string pasword)
        {
            string username = string.Empty;
            using (var context = new KFentities())
            {
                try
                {
                    if (context.UserRegistrations.Where(i => i.Email == email && i.IsVerified == true).Any())
                    {
                        var user = context.UserRegistrations.Where(i => i.Email == email).FirstOrDefault();
                        user.Password = pasword;
                        context.SaveChanges();
                        return username = user.Username;
                    }
                    else
                    {
                        return username;
                    }
                }
                catch (Exception ex)
                {
                    return username;
                }
            }
        }
        #endregion
        
        #region Check Invoice user authentication with username & password
        public InvoiceUserRegistrationDto CheckAuthorizedInvoiceUser(string email, string password)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var unlinkChk = db.InvoiceUserRegistrations.Where(x => (x.Username == email || x.EmailTo == email) && x.IsActive == true && x.Id == 3).FirstOrDefault();
                    if (unlinkChk != null)
                    {
                        if (unlinkChk.Password == password)
                        {
                            Mapper.CreateMap<InvoiceUserRegistration, InvoiceUserRegistrationDto>();
                            return Mapper.Map<InvoiceUserRegistrationDto>(unlinkChk);
                        }
                    }
                    // var user = (from i in db.UserRegistrations where i.Username == email && i.Password == password && i.IsActive==true select i).FirstOrDefault();


                    var user = db.InvoiceUserRegistrations.Where(x => (x.Username == email || x.EmailTo == email) && x.IsDeleted == false).FirstOrDefault();
                    if (user != null)
                    {

                        if (user.Password == password)
                        {
                            Mapper.CreateMap<InvoiceUserRegistration, InvoiceUserRegistrationDto>();
                            return Mapper.Map<InvoiceUserRegistrationDto>(user);
                        }
                        else
                        {
                            return new InvoiceUserRegistrationDto { EmailTo = "Invalid credential" };
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return new InvoiceUserRegistrationDto { EmailTo = "Invalid credential" };
        }
        #endregion
        
        #region Insert User Payment details
        public bool InserUserPaymentDetails(int userId, string token, decimal PaymentAmount, string Name)
        {
            bool PaymentInsertStatus = false;
            using (var dbContext = new KFentities())
            {
                var data = dbContext.UserLincensePayments.Where(i => i.UserId == userId).Any();
                if (data == true)
                {
                    return PaymentInsertStatus;
                }
                UserLincensePayment dbInsert = new UserLincensePayment();
                dbInsert.UserId = userId;
                dbInsert.DateCreated = DateTime.Now;
                dbInsert.PaymentAmount = PaymentAmount;
                dbInsert.Name = Name;
                dbInsert.ChargeId = token;
                dbContext.UserLincensePayments.Add(dbInsert);
                dbContext.SaveChanges();
                PaymentInsertStatus = true;
            }
            return PaymentInsertStatus;

        }
        #endregion

        #region Insert Card Details
        public bool AddCard(AddCardDetailDto Obj)
        {
            bool IsSaved = false;
            using (var db = new KFentities())
            {
                var existChk = db.CardDetails.Where(p => p.CardNumber == Obj.CardNumber && p.UserId == Obj.UserId).FirstOrDefault();
                if (existChk != null)
                {
                    //update
                    IsSaved = true;
                }
                else
                {
                    //insert
                    CardDetail dbInsert = new CardDetail();
                    dbInsert.CardNumber = Obj.CardNumber;
                    dbInsert.CardHolderName = Obj.FirstName + Obj.LastName;
                    dbInsert.CVV = Convert.ToInt32(Obj.CVV);
                    dbInsert.DateCreated = DateTime.Now;
                    dbInsert.ExpiryMonth = Obj.ExpiryMonth;
                    dbInsert.ExpiryYear = Obj.ExpiryYear;
                    dbInsert.IsDeleted = false;
                    dbInsert.UserId = Obj.UserId;
                    db.CardDetails.Add(dbInsert);
                    db.SaveChanges();
                    IsSaved = true;

                }
            }
            return IsSaved;
        }
        #endregion

        #region Active User List
        public bool SentEmailStatus(string Email)
        {
            bool IsMailSend = false;
            using (var dbContext = new KFentities())
            {
                var userDetails = dbContext.UserRegistrations.Where(i => i.Email == Email).FirstOrDefault();
                if (userDetails != null)
                {
                    userDetails.IsEmailSent = true;
                    dbContext.SaveChanges();
                    IsMailSend = true;
                }
            }
            return IsMailSend;
        }
        //public List<UserRegistration> GetUserListByAccountantId(string firstName, int userId, int Skip, int Take, int RoleId, String AccountantUnderEmployees, int EmployeeLoginId)
        //{
        //    List<UserRegistration> objList = new List<UserRegistration>();
        //    using (var db = new KFentities())
        //    {
        //        var userdetails = db.UserRegistrations.Where(i => i.Id == userId).FirstOrDefault();
        //        if (userdetails != null)
        //        {

        //            if (EmployeeLoginId > 0)
        //            {
        //                var objList1 = db.sp_AccountantUnderEmployees(AccountantUnderEmployees, false).Select(s => new UserRegistration()
        //                {
        //                    Id = s.Id,
        //                    ReconciliationType = s.ReconciliationType,
        //                    AccountantId = s.AccountantId,
        //                    FirstName = s.FirstName,
        //                    LastName = s.LastName,
        //                    Username = s.Username,
        //                    Email = s.Email,
        //                    Password = s.Password,
        //                    MobileNumber = s.MobileNumber,
        //                    CountryId = s.CountryId,
        //                    ProvinceId = s.ProvinceId,
        //                    City = s.City,
        //                    PostalCode = s.PostalCode,
        //                    SectorId = s.SectorId,
        //                    IndustryId = s.IndustryId,
        //                    SubIndustryId = s.SubIndustryId,
        //                    CompanyName = s.CompanyName,
        //                    CorporationAddress = s.CorporationAddress,
        //                    GSTNumber = s.GSTNumber,
        //                    BusinessNumber = s.BusinessNumber,
        //                    OwnershipId = s.OwnershipId,
        //                    CurrencyId = s.CurrencyId,
        //                    LicenseId = s.LicenseId,
        //                    RoleId = s.RoleId,
        //                    TaxStartYear = s.TaxStartYear,
        //                    TaxEndYear = s.TaxEndYear,
        //                    PrivateKey = s.PrivateKey,
        //                    CreatedDate = s.CreatedDate,
        //                    ModifiedDate = s.ModifiedDate,
        //                    IsCompleted = s.IsCompleted,
        //                    TaxStartMonthId = s.TaxStartMonthId,
        //                    TaxEndMonthId = s.TaxEndMonthId,
        //                    IsTrial = s.IsTrial,
        //                    IsEmailSent = s.IsEmailSent,
        //                    Status = s.Status,
        //                    IsVerified = s.IsVerified,
        //                    IsPaid = s.IsPaid,
        //                    TaxationStartDay = s.TaxationStartDay,
        //                    TaxationEndDay = s.TaxationEndDay,
        //                    IsDownload = s.IsDownload,
        //                    IsNewUser = s.IsNewUser,
        //                    IsDeleted = s.IsDeleted,
        //                    IsUnlink = s.IsUnlink,
        //                    IsTermsAndConditionAccepted = s.IsTermsAndConditionAccepted,
        //                    ProfileImage = s.ProfileImage,
        //                    IsYodleeAccountCreated = s.IsYodleeAccountCreated,
        //                    Country = s.Country,
        //                    IsEmployeeActivated = s.IsEmployeeActivated,
        //                    AccountantUnderEmployees = s.AccountantUnderEmployees

        //                });



        //                if (string.IsNullOrEmpty(firstName))
        //                {


        //                    objList = objList1.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
        //                }
        //                else
        //                {
        //                    objList = objList1.ToList().Where(x => x.FirstName.Contains(firstName) || x.LastName.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
        //                    //objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.FirstName.Contains(firstName) && x.LastName.Contains(firstName) && x.IsDeleted == false).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
        //                }
        //            }
        //            else
        //            {


        //                if (string.IsNullOrEmpty(firstName))
        //                {
        //                    objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
        //                }
        //                else
        //                {
        //                    objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && (x.FirstName.Contains(firstName) || x.LastName.Contains(firstName) || x.CompanyName.Contains(firstName)) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
        //                }

        //            }



        //        }
        //    }
        //    return objList;

        //}

        public List<UserRegistration> GetUnlinkedUserListByAccountantId(string firstName, int userId, int Skip, int Take)
        {
            List<UserRegistration> objList = new List<UserRegistration>();
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == userId).FirstOrDefault();
                if (userdetails != null)
                {
                    objList = (from i in db.UnlinkRecordLogs join j in db.UserRegistrations on i.UserId equals j.Id where i.AccountantId == userId select j).ToList();
                    if (string.IsNullOrEmpty(firstName))
                    {
                        objList = objList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                    }
                    else
                    {
                        objList = objList.Where(x => x.FirstName.Contains(firstName) || x.LastName.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                    }
                }
            }
            return objList.Distinct().ToList();

        }

        public int GetUnlinkUserCount(int AccountantId, string firstName)
        {
            using (var db = new KFentities())
            {
                var userdetails = (from i in db.UnlinkRecordLogs join j in db.UserRegistrations on i.UserId equals j.Id where i.AccountantId == AccountantId select j).ToList();
                if (userdetails != null)
                {

                    if (string.IsNullOrEmpty(firstName))
                    {
                        return userdetails.OrderBy(s => s.Id).Distinct().ToList().Count;
                    }
                    else
                    {
                        return userdetails.Where(x => x.FirstName.Contains(firstName)).OrderBy(s => s.Id).Distinct().ToList().Count;
                    }
                }
                return 0;
            }
        }
        public int GetActiveUserCount(int AccountantId, string firstName)
        {
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == AccountantId).FirstOrDefault();
                if (userdetails != null)
                {
                    if (string.IsNullOrEmpty(firstName))
                    {
                        return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                    }
                    else
                    {
                        return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.FirstName.Contains(firstName) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                    }
                }
                return 0;
            }
        }
        public string GetUniqueKey()
        {
            var maxSize = 8;
            //int minSize = 5;
            var chars = new char[62];
            var a = "abcdefghjkmnopqrstuvwxyzABCDEFGHJKMNOPQRSTUVWXYZ234567890";
            chars = a.ToCharArray();
            var size = maxSize;
            var data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
        public UserRegistration AddUser(UserRegistration registrationDetails)
        {
            //int userId = 0;
            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existChk = db.UserRegistrations.Where(o => o.Email == registrationDetails.Email && o.Username == registrationDetails.Username && o.IsVerified == false).Take(1).FirstOrDefault();
                        if (existChk != null)
                        {
                            return existChk;
                        }
                        else
                        {
                            //insert
                            registrationDetails.IsDeleted = false;
                            registrationDetails.IsUnlink = false;
                            db.UserRegistrations.Add(registrationDetails);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return registrationDetails;
                        }

                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }

                    //return userId;
                }
            }
        }


        public bool RollBackUserCreation(int id)
        {
            using (var Context = new KFentities())
            {
                try
                {
                    var itemToRemove = Context.UserRegistrations.SingleOrDefault(x => x.Id == id); //returns a single item.

                    if (itemToRemove != null)
                    {
                        Context.UserRegistrations.Remove(itemToRemove);
                        Context.SaveChanges();
                    }
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
        public Country GetCountryByName(string countryname)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Countries.Where(x => x.CountryName == countryname).FirstOrDefault();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Industry GetIndustryByName(string Industryname)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Industries.Where(x => x.IndustryType.Contains(Industryname)).Take(1).FirstOrDefault();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Ownership GetOwnershipByName(string ownershipName)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Ownerships.Where(x => x.OwnershipType.Contains(ownershipName)).Take(1).FirstOrDefault();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public SubIndustry GetSubIndustryByName(int IndustryId, string SubIndustryName)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.SubIndustries.Where(x => x.SubIndustryName.Contains(SubIndustryName) && x.IndustryId == IndustryId).Take(1).FirstOrDefault();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        //public tblSubIndustry GetSubIndustryByName(int IndustryId, string SubIndustryName)
        //{
        //    try
        //    {
        //        using (var db = new KFentities())
        //        {
        //            return db.tblSubIndustries.Where(x => x.SubIndustryName.Contains(SubIndustryName) && x.IndustryId == IndustryId).Take(1).FirstOrDefault();
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public tblProvince GetProvinceByName(string provincename)
        //{
        //    try
        //    {
        //        using (var db = new KFentities())
        //        {
        //            return db.tblProvinces.Where(x => x.ProvinceName == provincename).FirstOrDefault();
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public Province GetProvinceByName(string provincename)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.Provinces.Where(x => x.ProvinceName == provincename).FirstOrDefault();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public UserRegistrationDto GetUserByEmail(string email)
        {

            try
            {
                using (var db = new KFentities())
                {
                    //if (db.UserRegistrations.Where(x => x.Email == email && x.IsDeleted == true && x.IsUnlink==true && x.IsDownload==true).Any())
                    //{
                    //    var itemToRemove = db.UserRegistrations.SingleOrDefault(x => x.Email == email && x.Username != null  && x.IsDeleted == true); //returns a single item.
                    //    int userId = itemToRemove.Id;
                    //    string userName = itemToRemove.Username;
                    //    Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                    //    var olduserData= Mapper.Map<UserRegistrationDto>(itemToRemove);
                    //    if (itemToRemove != null)
                    //    {
                    //        //newUser.IsDeleted = false;
                    //        //db.UserRegistrations.Add(newUser);
                    //        //db.SaveChanges();
                    //        if (isExcelUser == false)
                    //        {
                    //            var old = db.UserRegistrations.Where(a => a.Id == userId).FirstOrDefault();
                    //            old.Username = null;
                    //            db.SaveChanges();
                    //            olduserData.IsUnlinkUser = true;
                    //        }
                    //        return olduserData;
                    //    }

                    //}

                    var data = db.UserRegistrations.Where(x => x.Email == email).FirstOrDefault();
                    Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                    return Mapper.Map<UserRegistrationDto>(data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public UserRegistration GetUserByname(string name)
        {

            try
            {
                using (var db = new KFentities())
                {
                    return db.UserRegistrations.Where(x => x.Username == name).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Mobile Api
        public bool ExistingMobileUsernameCheck(UserRegistrationDto accDetails)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.UserRegistrations.Where(i => i.Username == accDetails.Username).Any();
                    if (userExistChk == true)
                    {
                        UserExisted = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return UserExisted;
            }
        }
        public bool ExistingMobileEmailUserCheck(UserRegistrationDto accDetails)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.UserRegistrations.Where(i => (i.Username == accDetails.Username || i.Email == accDetails.Email) && i.IsDeleted == false).Any();
                    if (userExistChk == true)
                    {
                        UserExisted = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return UserExisted;
            }
        }
        public bool ExistingUserCheck(UserRegistrationDto accDetails)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.UserRegistrations.Where(i => i.Username == accDetails.Username).Any();
                    if (userExistChk == true)
                    {
                        return UserExisted = true;
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                return UserExisted;
            }
        }
        public bool ValidUserEmailAndPrivateKeyCheck(string Email, string PrivateKey)
        {
            var validUser = false;
            using (var context = new KFentities())
            {
                try
                {
                    var chkUser = context.UserRegistrations.Where(i => i.Email == Email && i.IsDeleted == false).FirstOrDefault();
                    if (chkUser != null)
                    {
                        if (chkUser.PrivateKey == PrivateKey)
                        {
                            validUser = true;
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return validUser;
        }

        public UserRegistration RegisterUserWithAnAccountant(UserRegistration accDetails)
        {
            //  int IsSaved = 0;
            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existingAccountantDetails = db.UserRegistrations.Where(x => x.Email == accDetails.Email && x.PrivateKey == accDetails.PrivateKey).FirstOrDefault();
                        if (existingAccountantDetails != null) //&& (existingAccountantDetails.IsPaid == null || existingAccountantDetails.IsPaid == false)
                        {
                            if (accDetails.IsUnlink == true)
                            {
                                existingAccountantDetails.IsUnlink = false;
                            }
                            existingAccountantDetails.Username = accDetails.Username != null ? accDetails.Username : existingAccountantDetails.Username;
                            existingAccountantDetails.Password = accDetails.Password != null ? accDetails.Password : existingAccountantDetails.Password;
                            existingAccountantDetails.ModifiedDate = DateTime.Now;
                            existingAccountantDetails.ModifiedDate = null;
                            existingAccountantDetails.IsVerified = accDetails.IsVerified;
                            existingAccountantDetails.IsTrial = accDetails.IsTrial;
                            existingAccountantDetails.IsPaid = accDetails.IsPaid;
                            db.Entry(existingAccountantDetails).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return existingAccountantDetails;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                    // return IsSaved;
                }
            }
        }

        public UserRegistrationDto CheckAuthorizedUserForMobile(string email, string password)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var unlinkChk = db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsUnlink == true && x.RoleId == 3).FirstOrDefault();
                    if (unlinkChk != null)
                    {
                        if (unlinkChk.Password == password)
                        {
                            Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                            var user = Mapper.Map<UserRegistrationDto>(unlinkChk);
                            user.IsUnlinkUser = true;
                            return user;
                            //var newUser = db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsDeleted == false && x.RoleId==3).FirstOrDefault();
                            //if(newUser != null)
                            //{
                            //    Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                            //    var user = Mapper.Map<UserRegistrationDto>(newUser);
                            //    user.IsUnlinkUser = true;
                            //    return user;
                            //}
                            //else
                            //{
                            //    return new UserRegistrationDto { IsUnlinkUser = true };
                            //}

                        }
                        else
                        {
                            return new UserRegistrationDto { Email = "Invalid login credentials." };
                        }
                    }
                    else if (db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsDeleted == false).Any())
                    {
                        var user = db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsVerified == true && x.IsDeleted == false).FirstOrDefault();
                        if (user != null)
                        {
                            if (user.Password == password)
                            {
                                Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                                return Mapper.Map<UserRegistrationDto>(user);
                            }
                            else
                            {
                                return new UserRegistrationDto { Email = "Invalid login credentials." };
                            }
                        }
                        else
                        {
                            return new UserRegistrationDto { Email = "Please verify your email." };
                        }
                    }
                    else
                    {
                        return new UserRegistrationDto { Email = "Invalid login credentials." };
                    }


                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        public string CheckUserStatus(int UserId)
        {
            using (var db = new KFentities())
            {
                try
                {
                    var getdata = db.UserRegistrations.Where(i => i.Id == UserId).FirstOrDefault();
                    if (getdata.RoleId == 2 || getdata.RoleId == 3)
                    {
                        if (getdata.IsPaid == true && (getdata.IsTrial == false || getdata.IsTrial == null))
                            return "Paid";
                        else if (getdata.IsTrial == null && (getdata.IsPaid == false || getdata.IsPaid == null))
                        {
                            return "Unpaid";
                        }
                        else
                        {
                            DateTime UserModifiedDate = Convert.ToDateTime(getdata.ModifiedDate);
                            DateTime CurrentDate = DateTime.Now;
                            var days = (CurrentDate - UserModifiedDate).TotalDays;
                            if (days > 30)
                            {
                                //return "Unpaid";

                                if (getdata.IsPaid == true)
                                {
                                    return "Paid";
                                }
                                else
                                {
                                    return "UnPaid";
                                }


                            }
                            else
                                return "Trial";

                        }
                    }
                    else
                        return "Paid";
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Unlink Data
        public List<UserRegistration> GetUserListByAccountantId(int AccountantId, string AccountantMail)
        {
            List<UserRegistration> objList = new List<UserRegistration>();
            using (var db = new KFentities())
            {
                if (db.ArchiveBankExpenses.Where(i => i.AccountantEmail == AccountantMail && (i.IsDownload == false || i.IsDownload == null)).Any())
                {
                    int[] userIds = db.ArchiveBankExpenses.Where(i => i.AccountantEmail == AccountantMail && (i.IsDownload == false || i.IsDownload == null)).Select(l => l.UserId).ToArray();
                    objList = db.UserRegistrations.Where(x => userIds.Contains(x.Id)).ToList();
                }
            }
            return objList;

        }
        #endregion

       

        #region Supplier List By Id
        public List<tblCustomerOrSupplier> GetSupplierListById(int userid, int userId, int Skip, int Take, int RoleId, String AccountantUnderEmployees, int EmployeeLoginId, string firstName)
        {
            //List<tblCustomerOrSupplier> objList = new List<tblCustomerOrSupplier>();
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null && EmployeeLoginId > 0)
            //    {

            //        //var objList1 = db.sp_GetSupplierLists(userid, 2).Select(s => new tblCustomerOrSupplier()
            //        //{
            //        //    Id = s.Id,
            //        //    Company_Name = s.Company_Name

            //        //});

            //        List<AddCustomerSupplierDto> ObjList = new List<AddCustomerSupplierDto>();
            //        List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();

            //        using (var context = new KFentities())
            //        {
            //            var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

            //            var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == 1 && i.Email == InvoiceEmail).ToList();
            //            foreach (var InvoiData in Invoice_ID)
            //            {
            //                int ids = Convert.ToInt32(InvoiData.Id) - 1;
            //                ObjAllEmployeeList = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
            //            }

            //        }

            //        if (string.IsNullOrEmpty(firstName))
            //        {
            //            objList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
            //        }
            //        else
            //        {
            //            objList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

            //        }
            //    }

            //    return objList;
            //}


            //List<tblCustomerOrSupplier> objList = new List<tblCustomerOrSupplier>();
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {

            //        if (EmployeeLoginId > 0)
            //        {
            //            var objList1 = db.sp_GetSupplierLists(userid, 1).Select(s => new tblCustomerOrSupplier()
            //            {
            //                Id = s.Id,
            //                Company_Name = s.Company_Name

            //            });
            //            if (string.IsNullOrEmpty(firstName))
            //            {
            //                objList = objList1.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
            //            }
            //            else
            //            {
            //                objList = objList1.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

            //            }
            //        }
            //        else
            //        {
            //        }



            //    }
            //}
            //return objList;
            List<tblCustomerOrSupplier> ObjList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList1 = new List<tblCustomerOrSupplier>();
            try
            {
                using (var context = new KFentities())
                {
                    int roleid = 1;
                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

                    if (roleid == 1)
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == 2 && i.Email == InvoiceEmail).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id) - 1;
                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                        }


                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

                        }



                        return ObjList;
                    }
                    else
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id);

                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);

                        }
                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

                        }



                        return ObjList;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion       

        #region Count Customer List
        public int CountCustomerList(int userid, string firstName)
        {
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.tblCustomerOrSuppliers.Where(i => i.UserId == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {
            //        if (string.IsNullOrEmpty(firstName))
            //        {
            //            return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.RoleId == 2).OrderBy(s => s.Id).ToList().Count;
            //        }
            //        else
            //        {
            //            return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.Company_Name.Contains(firstName) && i.RoleId == 2).OrderBy(s => s.Id).ToList().Count;
            //        }
            //    }
            //    return 0;
            //}
            List<tblCustomerOrSupplier> ObjList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList1 = new List<tblCustomerOrSupplier>();
            try
            {
                using (var context = new KFentities())
                {
                    int roleid = 2;
                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

                    if (roleid == 1)
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.Email == InvoiceEmail).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id) - 1;
                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                        }


                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList();

                        }

                        return ObjList.Count();
                    }
                    else
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id);

                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);

                        }
                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList();

                        }



                        return ObjList.Count();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


























            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {

            //        List<AddCustomerSupplierDto> ObjList = new List<AddCustomerSupplierDto>();
            //        List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();

            //        using (var context = new KFentities())
            //        {
            //            var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

            //            var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == 2 && i.Email == InvoiceEmail).ToList();
            //            foreach (var InvoiData in Invoice_ID)
            //            {
            //                int ids = Convert.ToInt32(InvoiData.Id) - 1;
            //                ObjAllEmployeeList = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
            //            }

            //        }

            //        if (string.IsNullOrEmpty(firstName))
            //        {
            //            return ObjAllEmployeeList.OrderBy(s => s.Id).ToList().Count; 
            //        }
            //        else
            //        {
            //            return ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList().Count; 

            //        }
            //    }
            //    //var userdetails = db.tblCustomerOrSuppliers.Where(i => i.UserId == userid).FirstOrDefault();
            //    //if (userdetails != null)
            //    //{
            //    //    if (string.IsNullOrEmpty(firstName))
            //    //    {
            //    //        return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.RoleId == 2).OrderBy(s => s.Id).ToList().Count;
            //    //    }
            //    //    else
            //    //    {
            //    //        return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.Company_Name.Contains(firstName) && i.RoleId == 2).OrderBy(s => s.Id).ToList().Count;
            //    //    }
            //    //}
            //    return 0;
            //}
        }
        #endregion

        #region Customer List By Id
        public List<tblCustomerOrSupplier> GetCustomerListById(int userid, int userId, int Skip, int Take, int RoleId, String AccountantUnderEmployees, int EmployeeLoginId, string firstName)
        {
            //List<tblCustomerOrSupplier> objList = new List<tblCustomerOrSupplier>();
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {

            //        if (EmployeeLoginId > 0)
            //        {
            //            var objList1 = db.sp_GetSupplierLists(userid, 2).Select(s => new tblCustomerOrSupplier()
            //            {
            //                Id = s.Id,
            //                Company_Name = s.Company_Name

            //            });
            //            if (string.IsNullOrEmpty(firstName))
            //            {
            //                objList = objList1.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
            //            }
            //            else
            //            {
            //                objList = objList1.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

            //            }
            //        }
            //        else
            //        {
            //        }



            //    }
            //}
            //return objList;

            //List<tblCustomerOrSupplier> objList = new List<tblCustomerOrSupplier>();
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null && EmployeeLoginId > 0)
            //    {

            //            //var objList1 = db.sp_GetSupplierLists(userid, 2).Select(s => new tblCustomerOrSupplier()
            //            //{
            //            //    Id = s.Id,
            //            //    Company_Name = s.Company_Name

            //            //});

            //            List<AddCustomerSupplierDto> ObjList = new List<AddCustomerSupplierDto>();
            //            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
                        
            //                using (var context = new KFentities())
            //                {
            //                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

            //                    var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == 2 && i.Email == InvoiceEmail).ToList();
            //                    foreach (var InvoiData in Invoice_ID)
            //                    {
            //                        int ids = Convert.ToInt32(InvoiData.Id) - 1;
            //                        ObjAllEmployeeList = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
            //                    }
                                
            //                }

            //                if (string.IsNullOrEmpty(firstName))
            //                {
            //                    objList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
            //                }
            //                else
            //                {
            //                    objList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

            //                }
            //            }
                
            //    return objList;
            //        }

            List<tblCustomerOrSupplier> ObjList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList1 = new List<tblCustomerOrSupplier>();
            try
            {
                using (var context = new KFentities())
                {
                    int roleid = 2;
                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

                    if (roleid == 1)
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.Email == InvoiceEmail).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id) - 1;
                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                        }


                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

                        }

                        return ObjList;
                    }
                    else
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id);

                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                           
                        }
                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

                        }



                        return ObjList;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
           

        }
        #endregion

        #region Count Supplier List
        public int CountSupplierList(int userid, string firstName)
        {
            //using (var db = new KFentities())
            //{
            //    var userdetails = db.tblCustomerOrSuppliers.Where(i => i.UserId == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {
            //        if (string.IsNullOrEmpty(firstName))
            //        {
            //            return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.RoleId == 1).OrderBy(s => s.Id).ToList().Count;
            //        }
            //        else
            //        {
            //            return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.Company_Name.Contains(firstName) && i.RoleId == 1).OrderBy(s => s.Id).ToList().Count;
            //        }
            //    }
            //    return 0;
            //}


            List<tblCustomerOrSupplier> ObjList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList1 = new List<tblCustomerOrSupplier>();
            try
            {
                using (var context = new KFentities())
                {
                    int roleid = 1;
                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

                    if (roleid == 1)
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.Email == InvoiceEmail).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id) - 1;
                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                        }


                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList();

                        }

                        return ObjList.Count();
                    }
                    else
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id);

                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);

                        }
                        if (string.IsNullOrEmpty(firstName))
                        {
                            ObjList = ObjAllEmployeeList.OrderBy(s => s.Id).ToList();
                        }
                        else
                        {
                            ObjList = ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList();

                        }



                        return ObjList.Count();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }



            //using (var db = new KFentities())
            //{
            //    var userdetails = db.InvoiceUserRegistrations.Where(i => i.Id == userid).FirstOrDefault();
            //    if (userdetails != null)
            //    {

            //        List<AddCustomerSupplierDto> ObjList = new List<AddCustomerSupplierDto>();
            //        List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();

            //        using (var context = new KFentities())
            //        {
            //            var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userid).Select(s => s.EmailTo).FirstOrDefault();

            //            var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == 1 && i.Email == InvoiceEmail).ToList();
            //            foreach (var InvoiData in Invoice_ID)
            //            {
            //                int ids = Convert.ToInt32(InvoiData.Id) - 1;
            //                ObjAllEmployeeList = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
            //            }

            //        }

            //        if (string.IsNullOrEmpty(firstName))
            //        {
            //            return ObjAllEmployeeList.OrderBy(s => s.Id).ToList().Count;
            //        }
            //        else
            //        {
            //            return ObjAllEmployeeList.ToList().Where(x => x.Company_Name.Contains(firstName)).OrderBy(s => s.Id).ToList().Count;

            //        }
            //    }
            //    //var userdetails = db.tblCustomerOrSuppliers.Where(i => i.UserId == userid).FirstOrDefault();
            //    //if (userdetails != null)
            //    //{
            //    //    if (string.IsNullOrEmpty(firstName))
            //    //    {
            //    //        return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.RoleId == 1).OrderBy(s => s.Id).ToList().Count;
            //    //    }
            //    //    else
            //    //    {
            //    //        return db.tblCustomerOrSuppliers.Where(i => i.UserId == userid && i.Company_Name.Contains(firstName) && i.RoleId == 1).OrderBy(s => s.Id).ToList().Count;
            //    //    }
            //    //}
            //    return 0;
            //}
        }
        #endregion     
       
        #region Count Received Invoice List
        public int CountReceivedInvoiceList(int userid)
        {
            using (var db = new KFentities())
            {
                var objList1 = db.Kl_ReceivedInvoiceList(userid);

                return objList1.Count();
            }
        }
        #endregion

        #region Received Invoice List
        public List<Sp_SendInvoiceListDto> ReceivedInvoiceList(int userid)
        {
            try
            {
                using (var context = new KFentities())
                {

                    var objlist = (from Invoice in context.Kl_ReceivedInvoiceList(userid)
                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Invoice.DepositePayment,
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault()

                                   }).ToList();



                    return objlist;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Reporting List
        public List<Sp_SendInvoiceListDto> ReportingList(int userid, int userId, int Skip, int Take, int RoleId, String AccountantUnderEmployees, int EmployeeLoginId, string typeofinvoice, string flow)
        {
            try
            {
                using (var context = new KFentities())
                {
                    List<Sp_SendInvoiceListDto> MainList = new List<Sp_SendInvoiceListDto>();
                    var listByUserid = (from Invoice in context.Kl_ReportingList(userid)
                                        select new Sp_SendInvoiceListDto
                                        {
                                            Id = Invoice.Id,
                                            In_R_FlowStatus = context.KI_GetFlowStatus(Invoice.Pro_FlowStatus, Invoice.DepositePayment, Invoice.BalanceDue, Invoice.Pro_Status).FirstOrDefault(),
                                            In_R_Status = Invoice.In_R_Status,
                                            InvoiceDate = Invoice.InvoiceDate,
                                            InvoiceNumber = Invoice.InvoiceNumber,
                                            DueDate = Invoice.DueDate,
                                            DocumentRef = Invoice.DocumentRef,
                                            DepositePayment = Invoice.DepositePayment,
                                            BalanceDue = Invoice.BalanceDue,
                                            CustomerId = Invoice.CustomerId,
                                            CreatedDate = Invoice.CreatedDate,
                                            IsDeleted = Invoice.IsDeleted,
                                            IsInvoiceReport = Invoice.IsInvoiceReport,
                                            ModifyDate = Invoice.ModifyDate,
                                            Note = Invoice.Note,
                                            PaymentTerms = Invoice.PaymentTerms,
                                            Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                            //Pro_FlowStatus =Convert.ToString(context.KI_GetFlowStatus(Invoice.Pro_Status, Invoice.DepositePayment, Invoice.BalanceDue)),
                                            Pro_Status = Invoice.Pro_Status,
                                            RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                            SalesPerson = Invoice.SalesPerson,
                                            ShippingCost = Invoice.ShippingCost,
                                            Terms = Invoice.Terms,
                                            Total = Invoice.Total,
                                            Type = Invoice.Type,
                                            UserId = Invoice.UserId,
                                            Username = context.Kl_GetCompanyNameSend(userid, Invoice.Id).FirstOrDefault(),
                                            FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                            IsCustomer = false,
                                            IsSupplierManualPaid = Invoice.IsSupplierManualPaid,
                                            SupplierManualPaidAmount = Invoice.SupplierManualPaidAmount,
                                            SupplierManualPaidJVID = Invoice.SupplierManualPaidJVID,
                                            IsCustomerManualPaid = Invoice.IsCustomerManualPaid,
                                            CustomerManualPaidAmount = Invoice.CustomerManualPaidAmount,
                                            CustomerManualPaidJVID = Invoice.CustomerManualPaidJVID,
                                            IsStripe = false,
                                            PaymentDate = Invoice.PaymentDate.ToString(),
                                            InvoiceJVID = Invoice.InvoiceJVID,
                                            StripeJVID = Invoice.StripeJVID == null ? "" : Invoice.StripeJVID,
                                            IsPaymentbyStripe = (bool)context.StripePaymnts.Where(r => r.InvoiceID == Invoice.Id).Select(s => s.IsPaymentbyStripe).FirstOrDefault()
                                        }).ToList();


                    MainList.AddRange(listByUserid);



                    var objlist = (from Invoice in context.kl_ReportingListwithoutDraft(userid)
                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = context.KI_GetFlowStatus(Invoice.Pro_FlowStatus, Invoice.DepositePayment, Invoice.BalanceDue, Invoice.Pro_Status).FirstOrDefault(),
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Convert.ToDecimal(Invoice.DepositePayment == null ? 0 : Invoice.DepositePayment),
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       // ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       //Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault() == null ? "" : context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       //FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault() == null ? "" : context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       IsCustomer = true,
                                       IsSupplierManualPaid = Invoice.IsSupplierManualPaid,
                                       SupplierManualPaidAmount = Invoice.SupplierManualPaidAmount,
                                       SupplierManualPaidJVID = Invoice.SupplierManualPaidJVID,
                                       IsCustomerManualPaid = Invoice.IsCustomerManualPaid,
                                       CustomerManualPaidAmount = Invoice.CustomerManualPaidAmount,
                                       CustomerManualPaidJVID = Invoice.CustomerManualPaidJVID,
                                       IsStripe = (bool)context.Kl_ExitinStripe(Invoice.UserId).Single(),
                                       PaymentDate = Invoice.PaymentDate.ToString(),
                                       StripeJVID = Invoice.StripeJVID == null ? "" : Invoice.StripeJVID,
                                       InvoiceJVID = Invoice.CustomerInvoiceJVID == null ? "" : Invoice.CustomerInvoiceJVID,
                                       IsPaymentbyStripe = (bool)context.StripePaymnts.Where(r => r.InvoiceID == Invoice.Id).Select(s => s.IsPaymentbyStripe).FirstOrDefault()
                                   }).ToList();

                    MainList.AddRange(objlist);

                    if (typeofinvoice != null || flow != null)
                    {
                        int res = 0;
                        if (typeofinvoice == "Invoice")
                        {
                            res = 1;
                        }
                        else if (typeofinvoice == "Proforma")
                        {
                            res = 2;
                        }

                        if (typeofinvoice == null && flow != null)
                        {
                            return MainList.Where(s => s.In_R_FlowStatus == flow).OrderBy(s => s.InvoiceDate).Skip(Skip).Take(Take).ToList();
                        }
                        else if (flow == null && typeofinvoice != null)
                        {
                            return MainList.Where(s => s.Type == res).OrderBy(s => s.InvoiceDate).Skip(Skip).Take(Take).ToList();
                        }
                        else
                        {

                            return MainList.Where(s => s.Type == res && s.In_R_FlowStatus == flow).OrderBy(s => s.InvoiceDate).Skip(Skip).Take(Take).ToList();
                        }

                    }
                    else
                    {
                        return MainList.OrderBy(s => s.InvoiceDate).Skip(Skip).Take(Take).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

        #region Count Reporting List
        public int CountReportingList(int userid, string typeofinvoice, string flow)
        {
            try
            {
                using (var context = new KFentities())
                {
                    List<Sp_SendInvoiceListDto> MainList = new List<Sp_SendInvoiceListDto>();
                    var listByUserid = (from Invoice in context.Kl_ReportingList(userid)
                                        select new Sp_SendInvoiceListDto
                                        {
                                            Id = Invoice.Id,
                                            In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                            In_R_Status = Invoice.In_R_Status,
                                            InvoiceDate = Invoice.InvoiceDate,
                                            InvoiceNumber = Invoice.InvoiceNumber,
                                            DueDate = Invoice.DueDate,
                                            DocumentRef = Invoice.DocumentRef,
                                            DepositePayment = Invoice.DepositePayment,
                                            BalanceDue = Invoice.BalanceDue,
                                            CustomerId = Invoice.CustomerId,
                                            CreatedDate = Invoice.CreatedDate,
                                            IsDeleted = Invoice.IsDeleted,
                                            IsInvoiceReport = Invoice.IsInvoiceReport,
                                            ModifyDate = Invoice.ModifyDate,
                                            Note = Invoice.Note,
                                            PaymentTerms = Invoice.PaymentTerms,
                                            Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                            Pro_Status = Invoice.Pro_Status,
                                            RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                            SalesPerson = Invoice.SalesPerson,
                                            ShippingCost = Invoice.ShippingCost,
                                            Terms = Invoice.Terms,
                                            Total = Invoice.Total,
                                            Type = Invoice.Type,
                                            UserId = Invoice.UserId,
                                            Username = context.Kl_GetCompanyNameSend(userid, Invoice.Id).FirstOrDefault(),
                                            FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                            IsCustomer = false,
                                            IsSupplierManualPaid = Invoice.IsSupplierManualPaid,
                                            SupplierManualPaidAmount = Invoice.SupplierManualPaidAmount,
                                            SupplierManualPaidJVID = Invoice.SupplierManualPaidJVID,
                                            IsCustomerManualPaid = Invoice.IsCustomerManualPaid,
                                            CustomerManualPaidAmount = Invoice.CustomerManualPaidAmount,
                                            CustomerManualPaidJVID = Invoice.CustomerManualPaidJVID,
                                            IsStripe = false,
                                            PaymentDate = Invoice.PaymentDate.ToString(),
                                            InvoiceJVID = Invoice.InvoiceJVID,
                                            StripeJVID = Invoice.StripeJVID == null ? "" : Invoice.StripeJVID,
                                            IsPaymentbyStripe = (bool)context.StripePaymnts.Where(r => r.InvoiceID == Invoice.Id).Select(s => s.IsPaymentbyStripe).FirstOrDefault()
                                        }).ToList();


                    MainList.AddRange(listByUserid);



                    var objlist = (from Invoice in context.kl_ReportingListwithoutDraft(userid)
                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Convert.ToDecimal(Invoice.DepositePayment == null ? 0 : Invoice.DepositePayment),
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       //ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       //Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault() == null ? "" : context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       //FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault() == null ? "" : context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       IsCustomer = true,
                                       IsSupplierManualPaid = Invoice.IsSupplierManualPaid,
                                       SupplierManualPaidAmount = Invoice.SupplierManualPaidAmount,
                                       SupplierManualPaidJVID = Invoice.SupplierManualPaidJVID,
                                       IsCustomerManualPaid = Invoice.IsCustomerManualPaid,
                                       CustomerManualPaidAmount = Invoice.CustomerManualPaidAmount,
                                       CustomerManualPaidJVID = Invoice.CustomerManualPaidJVID,
                                       IsStripe = (bool)context.Kl_ExitinStripe(Invoice.UserId).Single(),
                                       PaymentDate = Invoice.PaymentDate.ToString(),
                                       StripeJVID = Invoice.StripeJVID == null ? "" : Invoice.StripeJVID,
                                       InvoiceJVID = Invoice.CustomerInvoiceJVID == null ? "" : Invoice.CustomerInvoiceJVID,
                                       IsPaymentbyStripe = (bool)context.StripePaymnts.Where(r => r.InvoiceID == Invoice.Id).Select(s => s.IsPaymentbyStripe).FirstOrDefault()
                                   }).ToList();

                    MainList.AddRange(objlist);



                    if (typeofinvoice != null || flow != null)
                    {
                        int res = 0;
                        if (typeofinvoice == "Invoice")
                        {
                            res = 1;
                        }
                        else if (typeofinvoice == "Proforma")
                        {
                            res = 2;
                        }

                        if (typeofinvoice == null && flow != null)
                        {
                            return MainList.Where(s => s.In_R_FlowStatus == flow).ToList().Count();
                        }
                        else if (flow == null && typeofinvoice != null)
                        {
                            return MainList.Where(s => s.Type == res).ToList().Count();
                        }
                        else
                        {

                            return MainList.Where(s => s.Type == res && s.In_R_FlowStatus == flow).ToList().Count();
                        }

                    }
                    else
                    {
                        return MainList.OrderBy(s => s.InvoiceDate).ToList().Count();
                    }






                    return MainList.Count();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        #endregion

        #region Count Received Proforma List
        public int CountReceivedProformaList(int userid)
        {
            using (var db = new KFentities())
            {
                var objList1 = db.kl_ReceivedProformaList(userid);

                return objList1.Count();
            }
        }
        #endregion

        #region Received Proforma List
        public List<Sp_SendInvoiceListDto> ReceivedProformaList(int userid)
        {
            try
            {
                using (var context = new KFentities())
                {
                    //var Customerid = context.Kl_ReceivedInvoiceList(userid).Select(s => s.CustomerId).FirstOrDefault();

                    //var CompanyName = context.Kl_GetCompanyNamr(userid).FirstOrDefault();

                    //var FirstName1 = context.tblCustomerOrSuppliers.Where(i => i.Id == Customerid).Select(s => s.Company_Name).FirstOrDefault();

                    var objlist = (from Invoice in context.kl_ReceivedProformaList(userid)
                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Invoice.DepositePayment,
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault()

                                   }).ToList();
                    return objlist;
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

        #region Count Sent Invoice List
        public int CountSentInvoiceList(int userid)
        {
            using (var db = new KFentities())
            {
                var objList1 = db.Kl_SendInvoiceList(userid);

                return objList1.Count();
            }
        }
        #endregion

        #region Sent Invoice List
        public List<Sp_SendInvoiceListDto> SentInvoiceList(int userid)
        {
            try
            {
                using (var context = new KFentities())
                {

                    var objlist = (from Invoice in context.Kl_SendInvoiceList(userid)

                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Invoice.DepositePayment,
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       Username = context.Kl_GetCompanyNameSend(userid, Invoice.Id).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),

                                       //  Tax = Invoice.Tax
                                   }).ToList();

                    return objlist;


                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

        #region Count Sent Proforma List
        public int CountSentProformaList(int userid)
        {
            using (var db = new KFentities())
            {
                var objList1 = db.Kl_SendProformaList(userid);

                return objList1.Count();
            }
        }
        #endregion

        #region Sent Proforma List
        public List<Sp_SendInvoiceListDto> SentProformaList(int userid)
        {
            try
            {
                using (var context = new KFentities())
                {

                    var objlist = (from Invoice in context.Kl_SendProformaList(userid)
                                   select new Sp_SendInvoiceListDto
                                   {
                                       Id = Invoice.Id,
                                       In_R_FlowStatus = Invoice.In_R_FlowStatus,
                                       In_R_Status = Invoice.In_R_Status,
                                       InvoiceDate = Invoice.InvoiceDate,
                                       InvoiceNumber = Invoice.InvoiceNumber,
                                       DueDate = Invoice.DueDate,
                                       DocumentRef = Invoice.DocumentRef,
                                       DepositePayment = Invoice.DepositePayment,
                                       BalanceDue = Invoice.BalanceDue,
                                       CustomerId = Invoice.CustomerId,
                                       CreatedDate = Invoice.CreatedDate,
                                       IsDeleted = Invoice.IsDeleted,
                                       IsInvoiceReport = Invoice.IsInvoiceReport,
                                       ModifyDate = Invoice.ModifyDate,
                                       Note = Invoice.Note,
                                       PaymentTerms = Invoice.PaymentTerms,
                                       Pro_FlowStatus = Invoice.Pro_FlowStatus,
                                       Pro_Status = Invoice.Pro_Status,
                                       RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                       SalesPerson = Invoice.SalesPerson,
                                       ShippingCost = Invoice.ShippingCost,
                                       Terms = Invoice.Terms,
                                       Total = Invoice.Total,
                                       Type = Invoice.Type,
                                       UserId = Invoice.UserId,
                                       Username = context.Kl_GetCompanyNameSend(userid, Invoice.Id).FirstOrDefault(),
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault()
                                   }).ToList();
                    return objlist;
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

        #region Goods By Id
        public string GetGoodsById(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var industry = context.Industries.Where(i => i.Id == Id).FirstOrDefault();
                    return industry.IndustryType;
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

    }
}
