using AutoMapper;
using KF.Dto.Modules.Common;
using KF.Dto.Modules.Finance;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Finance
{
    public class AccountRepository : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

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

        #region Update Profile
        public bool UpdateUserProfile(UserRegistrationDto accDetails)
        {
            bool updateProfile = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userData = dbContext.UserRegistrations.Where(i => i.Id == accDetails.Id).FirstOrDefault();
                    #region Insert Profile update Log
                    string UpdateDetails = string.Empty;
                    if (userData.FirstName != accDetails.FirstName.Trim())
                    {
                        UpdateDetails += " Firstname is updated " + userData.FirstName + " - " + accDetails.FirstName.Trim() + ",";
                    }
                    if (userData.LastName != accDetails.LastName.Trim())
                    {
                        UpdateDetails += " Lastname is updated " + userData.LastName + " - " + accDetails.LastName.Trim() + ",";
                    }
                    if (userData.BusinessNumber != accDetails.BusinessNumber.Trim())
                    {
                        UpdateDetails += " BusinessNumber is updated " + userData.BusinessNumber + " - " + accDetails.BusinessNumber.Trim() + ",";
                    }
                    if (userData.City != accDetails.City.Trim())
                    {
                        UpdateDetails += " City is updated " + userData.City + " - " + accDetails.City.Trim() + ",";
                    }
                    if (userData.CompanyName != accDetails.CompanyName.Trim())
                    {
                        UpdateDetails += " CompanyName is updated " + userData.CompanyName + " - " + accDetails.CompanyName.Trim() + ",";
                    }
                    if (userData.CorporationAddress != accDetails.CorporationAddress.Trim())
                    {
                        UpdateDetails += " CorporationAddress is updated " + userData.CorporationAddress + " - " + accDetails.CorporationAddress.Trim() + ",";
                    }
                    if (!string.IsNullOrEmpty(accDetails.ProfileImage))
                    {
                        UpdateDetails += " ProfileImage is updated ,";
                    }
                    if (accDetails.RoleId != 1)
                    {
                        if (userData.OwnershipId != accDetails.OwnershipId)
                        {
                            UpdateDetails += " Ownership is updated " + dbContext.Ownerships.Where(s => s.Id == userData.OwnershipId).Select(f => f.OwnershipType).FirstOrDefault() + " - " + dbContext.Ownerships.Where(s => s.Id == accDetails.OwnershipId).Select(f => f.OwnershipType).FirstOrDefault() + ",";
                        }
                    }
                    if (userData.MobileNumber != accDetails.MobileNumber.Trim())
                    {
                        UpdateDetails += " MobileNumber is updated " + userData.MobileNumber + " - " + accDetails.MobileNumber.Trim() + ",";
                    }
                    if (userData.GSTNumber != accDetails.GSTNumber.Trim())
                    {
                        UpdateDetails += " GSTNumber is updated " + userData.GSTNumber + " - " + accDetails.GSTNumber.Trim() + ",";
                    }
                    if (userData.PostalCode != accDetails.PostalCode.Trim())
                    {
                        UpdateDetails += " PostalCode is updated " + userData.PostalCode + " - " + accDetails.PostalCode.Trim() + ",";
                    }
                    if (userData.ProvinceId != accDetails.ProvinceId)
                    {
                        UpdateDetails += " Province is updated " + dbContext.Provinces.Where(s => s.Id == userData.ProvinceId).Select(f => f.ProvinceName).FirstOrDefault() + " - " + dbContext.Provinces.Where(s => s.Id == accDetails.ProvinceId).Select(f => f.ProvinceName).FirstOrDefault() + ",";
                    }
                    using(var repo= new AccountRepository())
                    { repo.UserActivityLog(userData.Id, false, false, false, UpdateDetails); }
                    
                    #endregion
                    userData.FirstName = accDetails.FirstName.Trim();
                    userData.LastName = accDetails.LastName.Trim();
                    userData.BusinessNumber = accDetails.BusinessNumber.Trim();
                    userData.City = accDetails.City.Trim();
                    userData.CompanyName = accDetails.CompanyName.Trim();
                    userData.CorporationAddress = accDetails.CorporationAddress.Trim();
                    if (!string.IsNullOrEmpty(accDetails.ProfileImage))
                    {
                        userData.ProfileImage = accDetails.ProfileImage;
                    }
                    if (accDetails.RoleId != 1)
                    {
                        userData.OwnershipId = accDetails.OwnershipId;
                    }
                    userData.MobileNumber = accDetails.MobileNumber;
                    //  userData.CurrencyId = accDetails.CurrencyId;
                    userData.GSTNumber = accDetails.GSTNumber.Trim();
                    userData.PostalCode = accDetails.PostalCode.Trim();
                    userData.ProvinceId = Convert.ToByte(accDetails.ProvinceId);
                    if (accDetails.IndustryId > 0)
                    { userData.IndustryId = Convert.ToByte(accDetails.IndustryId); }



                    dbContext.SaveChanges();
                    updateProfile = true;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return updateProfile;
        }
        #endregion

        #region Active Employee Count
        public int GetActiveEmployeeCount(int AccountantId)
        {
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == AccountantId).FirstOrDefault();
                if (userdetails != null)
                {
                    return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.RoleId == 4 && x.IsDeleted == false).OrderBy(s => s.Id).ToList().Count;
                }
                return 0;
            }
        }
        #endregion

        #region Employee List By Accountant Id
        public List<UserRegistration> GetEmployeeListByAccountantId(int userId, int Skip, int Take)
        {
            List<UserRegistration> objList = new List<UserRegistration>();
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == userId).FirstOrDefault();
                if (userdetails != null)
                {
                    objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.RoleId == 4 && x.IsDeleted == false).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                }
            }
            return objList;

        }
        #endregion

        /// <summary>
        /// Update user terms and condition after payment
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UpdateUserWithAnAccountantDetails(UserRegistrationDto obj)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var oldUserData = db.UserRegistrations.Where(a => a.Id == obj.Id).FirstOrDefault();
                    oldUserData.AccountantId = obj.AccountantId; // Accountant id who create user.
                    oldUserData.PrivateKey = obj.PrivateKey;
                    oldUserData.TaxEndMonthId = Convert.ToByte(obj.TaxStartMonthId);
                    oldUserData.TaxEndYear = obj.TaxEndYear;
                    oldUserData.TaxStartYear = obj.TaxStartYear;
                    oldUserData.TaxationEndDay = obj.TaxationEndDay;
                    oldUserData.TaxationStartDay = obj.TaxationStartDay;
                    oldUserData.CreatedDate = DateTime.Now;
                    oldUserData.IsDeleted = false;
                    oldUserData.IsUnlink = true;
                    oldUserData.IsVerified = true;
                    oldUserData.IsEmailSent = false;
                    oldUserData.SubIndustryId = obj.SubIndustryId;
                    oldUserData.MobileNumber = obj.MobileNumber;
                    db.SaveChanges();
                    return true;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Send Private key Email to user with an accountant
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Account name for bank statement upload
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bankId"></param>
        /// <param name="selectedStatementType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update user terms and condition after payment
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UpdateUserTermsAndConditionDetails(int userID)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var userDetails = db.UserRegistrations.Where(s => s.Id == userID).FirstOrDefault();
                    if (userDetails != null)
                    {
                        userDetails.IsTermsAndConditionAccepted = true;
                        db.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check User payment status
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool CheckUserPaidStatus(int userID)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.UserLincensePayments.Where(i => i.UserId == userID && i.IsActiveSubscription == true).Any())
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Check user Email
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ConfirmEmail(int userID)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var userdetail = db.UserRegistrations.Where(p => p.Id == userID).FirstOrDefault();
                    if (userdetail != null)
                    {
                        userdetail.ModifiedDate = DateTime.Now;
                        userdetail.IsVerified = true;
                        db.SaveChanges();
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

        /// <summary>
        /// Get User with an accountant list for accountant
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="userId"></param>
        /// <param name="Skip"></param>
        /// <param name="Take"></param>
        /// <param name="RoleId"></param>
        /// <param name="AccountantUnderEmployees"></param>
        /// <param name="EmployeeLoginId"></param>
        /// <returns></returns>
        public List<UserRegistration> GetUserListByAccountantId(string firstName, int userId, int Skip, int Take, int EmployeeLoginId)
        {
            List<UserRegistration> objList = new List<UserRegistration>();
            using (var db = new KFentities())
            {

                if (string.IsNullOrEmpty(firstName))
                {
                    if (EmployeeLoginId > 0)
                    {
                        int[] customerlist = db.SubAccountantUserPermissions.Where(f => f.SubAccountantId == EmployeeLoginId && f.AccountantId == userId).Select(g => g.UserWithAnAccountantId).ToArray();
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userId && customerlist.Contains(x.Id) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                    }

                    else
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userId && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();

                }
                else
                {
                    if (EmployeeLoginId > 0)
                    {
                        int[] customerlist = db.SubAccountantUserPermissions.Where(f => f.SubAccountantId == EmployeeLoginId && f.AccountantId == userId).Select(g => g.UserWithAnAccountantId).ToArray();
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userId && customerlist.Contains(x.Id) && (x.FirstName.Contains(firstName) || x.LastName.Contains(firstName)) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                    }
                    else
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userId && (x.FirstName.Contains(firstName) || x.LastName.Contains(firstName)) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                }



            }
            return objList;

        }
        /// <summary>
        /// Get User with an accountant list for accountant total count
        /// </summary>
        /// <param name="AccountantId"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public int GetActiveUserCount(int AccountantId, string firstName, int EmployeeLoginId)
        {
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == AccountantId).FirstOrDefault();
                if (userdetails != null)
                {
                    if (string.IsNullOrEmpty(firstName))
                    {
                        if (EmployeeLoginId > 0)
                        {
                            int[] customerlist = db.SubAccountantUserPermissions.Where(f => f.SubAccountantId == EmployeeLoginId && f.AccountantId == AccountantId).Select(g => g.UserWithAnAccountantId).ToArray();
                            return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && customerlist.Contains(x.Id) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                        }
                        else
                            return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                    }
                    else
                    {
                        if (EmployeeLoginId > 0)
                        {
                            int[] customerlist = db.SubAccountantUserPermissions.Where(f => f.SubAccountantId == EmployeeLoginId && f.AccountantId == AccountantId).Select(g => g.UserWithAnAccountantId).ToArray();
                            return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && customerlist.Contains(x.Id) && x.FirstName.Contains(firstName) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                            // objList = db.UserRegistrations.Where(x => x.AccountantId == userId && customerlist.Contains(x.Id) && (x.FirstName.Contains(firstName) || x.LastName.Contains(firstName)) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).Skip(Skip).Take(Take).ToList();
                        }
                        else
                            return db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.FirstName.Contains(firstName) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList().Count;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// User Activity Log
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="IsLogin"></param>
        /// <param name="IsPwdUpdated"></param>
        /// <param name="IsSuperAdminUpdated"></param>
        /// <param name="UpdateDetails"></param>
        public void UserActivityLog(int userid, bool IsLogin, bool IsPwdUpdated, bool IsSuperAdminUpdated, string UpdateDetails)
        {
            using (var context = new KFentities())
            {
                var dbInsert = new UserUpdateProfileLog();
                dbInsert.Date = DateTime.Now;
                dbInsert.FieldUpdatedDetails = UpdateDetails;
                dbInsert.IsLogin = IsLogin;
                dbInsert.IsPasswordUpdated = IsPwdUpdated;
                dbInsert.IsSuperAdminUpdated = IsSuperAdminUpdated;
                dbInsert.UserId = userid;
                context.UserUpdateProfileLogs.Add(dbInsert);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Username exist check
        /// </summary>
        /// <param name="accDetails"></param>
        /// <returns></returns>
        public bool UsernameExistCheck(string username)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.UserRegistrations.Where(i => i.Username == username).Any();
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

        /// <summary>
        /// Useremail exist check
        /// </summary>
        /// <param name="accDetails"></param>
        /// <returns></returns>
        public bool UserEmailExistCheck(string email)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.UserRegistrations.Where(i => i.Email == email && i.IsDeleted == false).Any();
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

        public UserRegistration AddUser(UserRegistrationDto ObjUserDetail)
        {
            Mapper.CreateMap<UserRegistrationDto, UserRegistration>();
            var registrationDetails = Mapper.Map<UserRegistration>(ObjUserDetail);
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

        /// <summary>
        /// Delete User if email not sent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// User with an accountant registration
        /// </summary>
        /// <param name="accDetails"></param>
        /// <returns></returns>
        public UserRegistration RegisterUserWithAnAccountant(UserRegistrationDto ObjUserDetail)
        {
            Mapper.CreateMap<UserRegistrationDto, UserRegistration>();
            var accDetails = Mapper.Map<UserRegistration>(ObjUserDetail);
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

        /// <summary>
        /// Check the private key when user with an accounatant register on mobile platform
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Login Api
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserRegistrationDto CheckAuthorizedUser(string email, string password)
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

        /// <summary>
        /// Check user status
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// User Payment by role id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Activate after payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        public bool AddUserCardDetals(AddCardDetailDto Obj)
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


        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pasword"></param>
        /// <returns></returns>
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


        #region Yodlee
        public YodleeUserRegistration CheckUserYodleeAccount(int userId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.YodleeUserRegistrations.Where(a => a.KippinUserId == userId).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckUserYodleeAccountExist(int userId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    return db.YodleeUserRegistrations.Where(a => a.KippinUserId == userId).Any();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string YodleeRegistration(LoginWebsiteDto objDetails, string cobrandLogin, string cobrandPassword)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var dbExist = db.YodleeUserRegistrations.Where(s => s.UName == objDetails.UserName && s.KippinUserId == objDetails.UserId).FirstOrDefault();
                    if (dbExist == null)
                    {
                        //insert
                        var dbinsert = new YodleeUserRegistration();
                        dbinsert.UName = objDetails.UserName;
                        dbinsert.UPassword = objDetails.Password;
                        dbinsert.CreatedDate = DateTime.Now;
                        dbinsert.CobrandName = cobrandLogin;
                        dbinsert.CobrandPassword = cobrandPassword;
                        dbinsert.IsDeleted = false;
                        dbinsert.KippinUserId = objDetails.UserId;
                        db.YodleeUserRegistrations.Add(dbinsert);
                    }
                    else
                    {
                        dbExist.UPassword = objDetails.Password;
                    }
                    db.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region MyRegion
        public UserRegistrationDto CheckAuthorizedUserForMobile(string email, string password)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var unlinkChk = db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsUnlink == true && x.RoleId == 3).FirstOrDefault();
                    if (unlinkChk != null)
                    {
                        int CustomerId = db.tblCustomerOrSuppliers.Where(x => x.Email == unlinkChk.Email).Select(s => s.Id).FirstOrDefault();
                        if (unlinkChk.Password == password)
                        {
                            Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                            var user = Mapper.Map<UserRegistrationDto>(unlinkChk);
                            if (unlinkChk.IsOnlyInvoice == true)
                            {
                                var data = db.InvoiceUserRegistrations.Where(i => i.EmailTo == email).FirstOrDefault();
                                if (data != null)
                                {
                                    user.InvoiceId = data.Id;
                                    user.ContactPerson = data.ContactPerson;
                                    user.ShippingAptNo = data.ShippingAptNo;
                                    user.ShippingCity = data.ShippingCity;
                                    user.ShippingHouseNo = data.ShippingHouseNo;
                                    user.ShippingPostalCode = data.ShippingPostalCode;
                                    user.ShippingState = data.ShippingState;
                                    user.ShippingStreet = data.ShippingStreet;
                                    user.CorporateAptNo = data.CorporateAptNo;
                                    user.CorporateCity = data.CorporateCity;
                                    user.CorporateHouseNo = data.CorporateHouseNo;
                                    user.CorporatePostalCode = data.CorporatePostalCode;
                                    user.CorporateState = data.CorporateState;
                                    user.CorporateStreet = data.CorporateStreet;

                                }
                            }
                            user.CustomerId = CustomerId;
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
                        var user1 = db.UserRegistrations.Where(x => (x.Username == email || x.Email == email) && x.IsVerified == true && x.IsDeleted == false).FirstOrDefault();
                        if (user1 != null)
                        {
                            if (user1.Password == password)
                            {

                                Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                                var user = Mapper.Map<UserRegistrationDto>(user1);
                                if (user1.IsOnlyInvoice == false)
                                {
                                    var data = db.InvoiceUserRegistrations.Where(i => i.EmailTo == email || i.Username == email).FirstOrDefault();
                                    if (data != null)
                                    {
                                        user.InvoiceId = data.Id;
                                        user.ContactPerson = data.ContactPerson;
                                        user.ShippingAptNo = data.ShippingAptNo;
                                        user.ShippingCity = data.ShippingCity;
                                        user.ShippingHouseNo = data.ShippingHouseNo;
                                        user.ShippingPostalCode = data.ShippingPostalCode;
                                        user.ShippingState = data.ShippingState;
                                        user.ShippingStreet = data.ShippingStreet;
                                        user.CorporateAptNo = data.CorporateAptNo;
                                        user.CorporateCity = data.CorporateCity;
                                        user.CorporateHouseNo = data.CorporateHouseNo;
                                        user.CorporatePostalCode = data.CorporatePostalCode;
                                        user.CorporateState = data.CorporateState;
                                        user.CorporateStreet = data.CorporateStreet;


                                        var CustId = db.tblCustomerOrSuppliers.Where(x => x.Email == data.EmailTo && x.RoleId == 2).Select(s => s.Id).FirstOrDefault();
                                        if (CustId > 0)
                                        {
                                            user.CustomerId = CustId;
                                        }
                                        else
                                            user.CustomerId = 0;
                                    }
                                }

                                return user;
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


        public bool ConfirmEmailForMobileInvoices(int userID)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var userdetail = db.InvoiceUserRegistrations.Where(p => p.Id == userID).FirstOrDefault();
                    if (userdetail != null)
                    {
                        userdetail.IsVerified = true;
                        db.SaveChanges();
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


        #region Employee User List By Accountant Id
        public List<UserRegistration> GetEmployeeUserListByAccountantId(string firstName, int userId)
        {
            List<UserRegistration> objList = new List<UserRegistration>();
            using (var db = new KFentities())
            {
                var userdetails = db.UserRegistrations.Where(i => i.Id == userId).FirstOrDefault();
                if (userdetails != null)
                {
                    if (string.IsNullOrEmpty(firstName))
                    {
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList();
                    }
                    else
                    {
                        objList = db.UserRegistrations.Where(x => x.AccountantId == userdetails.Id && x.FirstName.Contains(firstName) && x.LastName.Contains(firstName) && x.IsDeleted == false && x.RoleId != 4).OrderBy(s => s.Id).ToList();
                    }
                }
            }
            return objList;

        }
        #endregion
    }
}
