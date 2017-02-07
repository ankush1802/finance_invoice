using AutoMapper;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using KF.Repo.Modules.Common;
using KF.Dto.Modules.Invoice;
using KF.Entity;
using KF.Dto.Modules.Common;
using KF.Utilities.Common;
using KF.Web.Models.Invoice;
using KF.Repo.Modules.Finance;



namespace KF.Repo.Modules.Invoice
{
    public class InvoiceUserRegistrationRepository : IDisposable
    {
        string pdfPath = string.Empty;
        int CustomerJvid = 0;
        AES encrypt = new AES();
        public static string stripesecretkey = System.Configuration.ConfigurationSettings.AppSettings["SecretKey"];
        public static string stripePublickey = System.Configuration.ConfigurationSettings.AppSettings["PublishableKey"];

        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Account Details
        public bool UserEmailChk(string EmailAddress)
        {
            bool ExistChk = false;
            using (var context = new KFentities())
            {
                var chk = context.InvoiceUserRegistrations.Where(i => i.EmailTo == EmailAddress && i.IsDeleted != true).Any();
                if (chk == true)
                    ExistChk = true;
                else
                    ExistChk = false;
            }
            return ExistChk;
        }
        public bool FinanceUserEmailChk(string EmailAddress)
        {
            bool ExistChk = false;
            using (var context = new KFentities())
            {
                var chk = context.UserRegistrations.Where(i => i.Email == EmailAddress && i.IsDeleted != true).Any();
                if (chk == true)

                    ExistChk = true;
                else
                    ExistChk = false;
            }
            return ExistChk;
        }
        public int Register(InvoiceUserRegistrationDto Obj)
        {
            int userId = 0;

            using (var context = new KFentities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Mapper.CreateMap<InvoiceUserRegistrationDto, InvoiceUserRegistration>();
                        var userData = Mapper.Map<InvoiceUserRegistration>(Obj);
                        context.InvoiceUserRegistrations.Add(userData);
                        userData.CreatedOn = DateTime.Now;
                        userData.IsDeleted = false;
                        userData.IsTrial = true;
                        userData.IsPaid = false;
                        userData.IsVerified = false;
                        userData.IsActive = true;
                        userData.IsOnlyInvoice = false;
                        var data = context.UserRegistrations.Where(i => i.Email == Obj.EmailTo).FirstOrDefault();
                        if (data != null)
                        {
                            data.IsOnlyInvoice = false;
                        }
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return userId = userData.Id;
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
        public int RegisterWithFinance(InvoiceUserRegistrationDto Obj, string Isverfied, int IsonlyInvoice)
        {
            int userId = 0;
            string[] isver = Isverfied.Split(',');
            using (var context = new KFentities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Mapper.CreateMap<InvoiceUserRegistrationDto, InvoiceUserRegistration>();
                        var userData = Mapper.Map<InvoiceUserRegistration>(Obj);
                        context.InvoiceUserRegistrations.Add(userData);
                        userData.CreatedOn = DateTime.Now;
                        userData.IsDeleted = false;
                        userData.IsTrial = Convert.ToBoolean(isver[2]);
                        userData.IsPaid = Convert.ToBoolean(isver[1]);
                        userData.IsVerified = Convert.ToBoolean(isver[0]);
                        userData.IsActive = true;
                        userData.IsOnlyInvoice = Convert.ToBoolean(IsonlyInvoice);
                        //UserRegistration obj = new UserRegistration();
                        //obj.IsOnlyInvoice = false;
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return userId = userData.Id;
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
        public string ChangePass(ChangePassword obj)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var olddata = context.InvoiceUserRegistrations.Where(a => a.Id == obj.UserId).FirstOrDefault();
                    if (olddata != null)
                    {
                        // var oldpassword = encrypt.AesDecrypt(olddata.Password); //Gurinder 20.12.16
                        if (olddata.Password == obj.OldPassword)
                        {
                            //var newpass = encrypt.AesEncrypt(obj.NewPassword);

                            olddata.Password = obj.NewPassword;
                            context.SaveChanges();
                            return "Success";
                        }
                        else
                        {
                            return "Incorrect password.";
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return "Failed to update password.";
        }
        public string ForgotPassword(string email, string pasword)
        {
            string username = string.Empty;
            using (var context = new KFentities())
            {
                try
                {
                    if (context.InvoiceUserRegistrations.Where(i => i.EmailTo == email && i.IsVerified == true).Any())
                    {
                        var user = context.InvoiceUserRegistrations.Where(i => i.EmailTo == email).FirstOrDefault();
                        //var newPassword = encrypt.AesEncrypt(pasword);

                        user.Password = pasword;
                        context.SaveChanges();
                        return username = user.EmailTo;
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
        public string IsVerifiedUserEmailChk(string EmailAddress)
        {
            String res = "";
            using (var context = new KFentities())
            {
                var chk = context.UserRegistrations.Where(i => i.Email == EmailAddress && i.IsVerified == true && i.IsDeleted != true).FirstOrDefault();
                if (chk != null)
                {
                    bool isverified = Convert.ToBoolean(chk.IsVerified);
                    bool isPaid = Convert.ToBoolean(chk.IsPaid);
                    bool isTrial = Convert.ToBoolean(chk.IsTrial);

                    res = isverified + "," + isPaid + "," + isTrial;

                }
                else
                {
                    res = "0,0,1";
                }
            }
            return res;
        }
        public string IsOnlyInvoice(String EmailAddress)
        {
            String res = "";
            using (var context = new KFentities())
            {
                var chk = context.UserRegistrations.Where(i => i.Email == EmailAddress && i.IsDeleted != true).FirstOrDefault();
                if (chk != null)
                {
                    chk.IsOnlyInvoice = false;

                    context.SaveChanges();
                    res = "1";
                }
                else
                {
                    res = "0";
                }
            }
            return res;
        }




        #endregion

        #region Invoice
        public bool FinanceInvoiceBit(InvoiceUserRegistrationDto Obj)
        {
            using (var context = new KFentities())
            {
                bool isSave = false;

                try
                {
                    var userData = context.UserRegistrations.Where(i => i.Email == Obj.EmailTo).FirstOrDefault();
                    if (userData != null)
                    {
                        userData.IsOnlyInvoice = false;
                        isSave = true;
                        context.SaveChanges();
                        return isSave;
                    }
                    else
                    {
                        return isSave;
                    }

                }

                catch (Exception)
                {
                    return isSave;
                }

            }
        }
        public InvoiceUserRegistrationDto Login(InvoiceUserRegistrationDto ObjUser)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var UserDetails = context.InvoiceUserRegistrations.Where(i => (i.EmailTo == ObjUser.EmailTo || i.Username == ObjUser.EmailTo) && i.IsVerified == true).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        //var oldpassword = encrypt.AesDecrypt(UserDetails.Password);


                        if (UserDetails.Password == ObjUser.Password)
                        {
                            Mapper.CreateMap<InvoiceUserRegistration, InvoiceUserRegistrationDto>();
                            var usrData = Mapper.Map<InvoiceUserRegistrationDto>(UserDetails);

                            return usrData;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ObjUser;
        }

        public bool AlreadyExistInvoiceEmailwithSameId(AddCustomerSupplierDto Obj)
        {
            using (var context = new KFentities())
            {
                bool UserExist = false;

                try
                {

                    var userData = context.InvoiceUserRegistrations.Where(i => i.EmailTo == Obj.Email && i.Id == Obj.UserId).FirstOrDefault();
                    if (userData != null)
                    {
                        UserExist = true;
                        return UserExist;

                    }
                    else
                    {
                        return UserExist;
                    }

                }

                catch (Exception)
                {
                    return UserExist;
                }

            }
        }
        public bool AlreadyExistInvoiceEmail(AddCustomerSupplierDto Obj)
        {
            using (var context = new KFentities())
            {
                bool isSave = false;

                try
                {

                    var userData = context.tblCustomerOrSuppliers.Where(i => i.Email == Obj.Email && i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).FirstOrDefault();
                    if (userData != null)
                    {
                        isSave = true;
                        return isSave;

                    }
                    else
                    {
                        return isSave;
                    }

                }

                catch (Exception)
                {
                    return isSave;
                }

            }
        }
        #endregion

        #region Customer/Supplier
        public AddCustomerSupplierDto FinanceAlreadyExist(AddCustomerSupplierDto ObjUser)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var UserDetails = context.UserRegistrations.Where(i => i.Email == ObjUser.Email && i.IsVerified == true && i.IsDeleted != true).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        ObjUser.Company_Name = UserDetails.CompanyName;
                        ObjUser.Email = UserDetails.Email;
                        ObjUser.City = UserDetails.City;
                        ObjUser.FirstName = UserDetails.FirstName;
                        ObjUser.Address = UserDetails.CorporationAddress;
                        ObjUser.DateCreated = DateTime.Now;
                        ObjUser.Mobile = UserDetails.MobileNumber;
                        ObjUser.PostalCode = UserDetails.PostalCode;
                        ObjUser.Id = UserDetails.Id;
                        ObjUser.IsFinance = true;
                        return ObjUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public AddCustomerSupplierDto InvoiceAlreadyExist(AddCustomerSupplierDto ObjUser)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var UserDetails = context.InvoiceUserRegistrations.Where(i => i.EmailTo == ObjUser.Email && i.IsVerified == true).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        ObjUser.Email = UserDetails.EmailTo;
                        ObjUser.Company_Name = UserDetails.CompanyName;
                        ObjUser.FirstName = UserDetails.ContactPerson;
                        ObjUser.Address = UserDetails.CorporateAptNo + "" + UserDetails.CorporateHouseNo + "" + UserDetails.CorporateStreet;
                        ObjUser.DateCreated = DateTime.Now;
                        ObjUser.Mobile = UserDetails.MobileNumber;
                        ObjUser.PostalCode = UserDetails.CorporatePostalCode;
                        ObjUser.City = UserDetails.CorporateCity;
                        ObjUser.ShippingAddress = UserDetails.ShippingAptNo + "" + UserDetails.ShippingHouseNo + "" + UserDetails.ShippingStreet;
                        ObjUser.ShippingCity = UserDetails.ShippingCity;
                        ObjUser.ShippingPostalCode = UserDetails.ShippingPostalCode;
                        ObjUser.Website = UserDetails.Website;
                        ObjUser.DateCreated = DateTime.Now;
                        ObjUser.IsFinance = false;
                        return ObjUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public int EmailExist(string EmailAddress)
        {
            // int userid = 0;
            int EmailExist = 0;
            using (var context = new KFentities())
            {
                var chk = context.InvoiceUserRegistrations.Where(i => (i.EmailTo == EmailAddress || i.Username == EmailAddress)).FirstOrDefault();
                if (chk != null)
                {
                    EmailExist = 1;
                }
                else
                    EmailExist = 0;
            }
            return EmailExist;
        }

        #region Customer Id
        public int GetCustomerId(string EmailAddress)
        {
            using (var context = new KFentities())
            {
                int CustomerId = 0;

                var IsEmailAddress = context.InvoiceUserRegistrations.Where(i => (i.EmailTo == EmailAddress)).FirstOrDefault();

                if (IsEmailAddress != null)
                {
                    var CustId = context.tblCustomerOrSuppliers.Where(x => x.Email == EmailAddress && x.RoleId == 2).Select(s => s.Id).FirstOrDefault();
                    if (CustId > 0)
                    {
                        return CustId;
                    }
                    else
                        return CustomerId;
                }
                else
                {
                    var Email = context.InvoiceUserRegistrations.Where(i => (i.Username == EmailAddress)).Select(s => s.EmailTo).FirstOrDefault();

                    var CustId = context.tblCustomerOrSuppliers.Where(x => x.Email == Email && x.RoleId == 2).Select(s => s.Id).FirstOrDefault();
                    if (CustId > 0)
                    {
                        return CustId;
                    }
                    else
                        return CustomerId;
                }


            }
            return 0;
        }
        #endregion

        public bool Isverify(string EmailAddress)
        {
            bool Isverify = false;
            using (var context = new KFentities())
            {
                var chk = context.InvoiceUserRegistrations.Where(i => (i.EmailTo == EmailAddress || i.Username == EmailAddress) && i.IsVerified == true).FirstOrDefault();
                if (chk != null)
                {
                    Isverify = true;
                }
                else
                    Isverify = false;
            }
            return Isverify;
        }
        //public int RegisterInvoiceCustomerSupplier(AddCustomerSupplierDto Obj)
        //{
        //    int userId = 0;

        //    using (var context = new KFentities())
        //    {
        //        using (var dbContextTransaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                //invoiceuser emil <> obj email || this condition is added 5-12-2016 (InvoiceData)
        //                var CustomerExist = context.tblCustomerOrSuppliers.Where(i => i.Email == Obj.Email && i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).FirstOrDefault();
        //                //var InvoiceRegisterUser = context.InvoiceUserRegistrations.Where(i => (i.Id == Obj.UserId)).FirstOrDefault();

        //                if (CustomerExist == null) //InvoiceRegisterUser == null &&
        //                {
        //                    if (Obj.RoleId == 2)
        //                    {
        //                        // userid email != invoice user email  && i.EmailTo != Obj.Email
        //                        var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
        //                        if (data != null)
        //                        {
        //                            int debator = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
        //                            debator = debator + 1;
        //                            Obj.Debtors = "1200-" + debator;
        //                        }
        //                        else
        //                        {
        //                            Obj.Debtors = "1200-1000";
        //                        }

        //                        var InvoiceData = context.InvoiceUserRegistrations.Where(i => i.Id == Obj.UserId).FirstOrDefault();//add emal check

        //                        var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == Obj.Email).Select(s => s.Id).FirstOrDefault();

        //                        if (InvoiceData.Id > 0)
        //                        {
        //                            tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();
        //                            dbinsert.Company_Name = InvoiceData.CompanyName;
        //                            dbinsert.FirstName = InvoiceData.ContactPerson;
        //                            dbinsert.Address = InvoiceData.CorporateAptNo + "|||" + InvoiceData.CorporateHouseNo + "|||" + InvoiceData.CorporateStreet;
        //                            dbinsert.State = InvoiceData.CorporateStreet;
        //                            dbinsert.City = InvoiceData.CorporateCity;
        //                            dbinsert.PostalCode = InvoiceData.CorporatePostalCode;
        //                            dbinsert.ServiceOffered = InvoiceData.GoodsType;
        //                            dbinsert.Mobile = InvoiceData.MobileNumber;
        //                            dbinsert.Email = InvoiceData.EmailTo;
        //                            dbinsert.Website = InvoiceData.Website;
        //                            dbinsert.UserId = InvoiceUserId;
        //                            dbinsert.Isdeleted = false;
        //                            dbinsert.Telephone = InvoiceData.MobileNumber;
        //                            dbinsert.RoleId = 1;


        //                            var InvoiceDataCreater = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
        //                            if (InvoiceDataCreater != null)
        //                            {
        //                                int credetor = Convert.ToInt32(InvoiceDataCreater.Substring(InvoiceDataCreater.LastIndexOf('-') + 1));
        //                                credetor = credetor + 1;
        //                                dbinsert.Credetor = "2250-" + credetor;

        //                            }
        //                            else
        //                            {
        //                                dbinsert.Credetor = "2250-1000";
        //                            }
        //                            //if dbinsert email == userregistration email
        //                            var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
        //                            if (EmailExist != null)
        //                            {
        //                                dbinsert.IsFinance = true;
        //                            }
        //                            else
        //                            {
        //                                dbinsert.IsFinance = false;
        //                            }
        //                            context.tblCustomerOrSuppliers.Add(dbinsert);
        //                            context.SaveChanges();

        //                            Mapper.CreateMap<AddCustomerSupplierDto, tblCustomerOrSupplier>();

        //                            var userData = Mapper.Map<tblCustomerOrSupplier>(Obj);
        //                            var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == Obj.Email);
        //                            if (EmailExistinFinance != null)
        //                            {
        //                                userData.IsFinance = true;
        //                            }
        //                            else
        //                            {
        //                                userData.IsFinance = false;
        //                            }
        //                            userData.DateCreated = DateTime.Now;
        //                            userData.Isdeleted = false;
        //                            context.tblCustomerOrSuppliers.Add(userData);
        //                            context.SaveChanges();
        //                            userId = userData.Id;


        //                        }
        //                        dbContextTransaction.Commit();
        //                        return userId;
        //                    }
        //                    else if (Obj.RoleId == 1)
        //                    {
        //                        //invoiceuser emil <> obj email 

        //                        var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
        //                        if (data != null)
        //                        {
        //                            int credetor = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
        //                            credetor = credetor + 1;
        //                            Obj.Credetor = "2250-" + credetor;
        //                        }
        //                        else
        //                        {
        //                            Obj.Credetor = "2250-1000";
        //                        }

        //                        var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == Obj.Email).Select(s => s.Id).FirstOrDefault();

        //                        var InvoiceUserData = context.InvoiceUserRegistrations.Where(i => i.Id == Obj.UserId).FirstOrDefault();

        //                        tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();

        //                        if (InvoiceUserData.Id > 0 && dbinsert.Email != Obj.Email) //add emal check
        //                        {
        //                            dbinsert.Company_Name = InvoiceUserData.CompanyName;
        //                            dbinsert.FirstName = InvoiceUserData.ContactPerson;
        //                            dbinsert.Address = InvoiceUserData.CorporateAptNo + "|||" + InvoiceUserData.CorporateHouseNo + "|||" + InvoiceUserData.CorporateStreet;
        //                            dbinsert.State = InvoiceUserData.CorporateStreet;
        //                            dbinsert.City = InvoiceUserData.CorporateCity;
        //                            dbinsert.PostalCode = InvoiceUserData.CorporatePostalCode;
        //                            dbinsert.ShippingAddress = InvoiceUserData.ShippingAptNo + "|||" + InvoiceUserData.ShippingHouseNo + "|||" + InvoiceUserData.ShippingStreet;
        //                            dbinsert.ShippingState = InvoiceUserData.ShippingState;
        //                            dbinsert.ShippingCity = InvoiceUserData.ShippingCity;
        //                            dbinsert.ShippingPostalCode = InvoiceUserData.ShippingPostalCode;
        //                            dbinsert.AdditionalEmail = InvoiceUserData.EmailCc;
        //                            dbinsert.Mobile = InvoiceUserData.MobileNumber;
        //                            dbinsert.Email = InvoiceUserData.EmailTo;
        //                            dbinsert.Website = InvoiceUserData.Website;
        //                            dbinsert.UserId = InvoiceUserId;
        //                            dbinsert.Isdeleted = false;
        //                            dbinsert.Telephone = InvoiceUserData.MobileNumber;
        //                            dbinsert.RoleId = 2;


        //                            var CusData = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
        //                            if (CusData != null)
        //                            {
        //                                int debator = Convert.ToInt32(data.Substring(CusData.LastIndexOf('-') + 1));
        //                                debator = debator + 1;
        //                                dbinsert.Debtors = "1200-" + debator;

        //                            }

        //                            else
        //                            {
        //                                dbinsert.Debtors = "1200-1000";
        //                            }
        //                            //if dbinsert email == userregistration email


        //                            var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
        //                            if (EmailExist != null)
        //                            {
        //                                dbinsert.IsFinance = true;
        //                            }
        //                            else
        //                            {
        //                                dbinsert.IsFinance = false;
        //                            }
        //                            context.tblCustomerOrSuppliers.Add(dbinsert);
        //                            context.SaveChanges();

        //                            Mapper.CreateMap<AddCustomerSupplierDto, tblCustomerOrSupplier>();

        //                            var userData = Mapper.Map<tblCustomerOrSupplier>(Obj);
        //                            var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == Obj.Email);
        //                            if (EmailExistinFinance != null)
        //                            {
        //                                userData.IsFinance = true;
        //                            }
        //                            else
        //                            {
        //                                userData.IsFinance = false;
        //                            }
        //                            userData.DateCreated = DateTime.Now;
        //                            userData.Isdeleted = false;
        //                            context.tblCustomerOrSuppliers.Add(userData);
        //                            context.SaveChanges();
        //                            userId = userData.Id;


        //                        }
        //                        dbContextTransaction.Commit();
        //                        return userId;
        //                    }
        //                    return userId;
        //                }
        //                else
        //                {
        //                    return 0;
        //                }

        //            }
        //            catch (Exception)
        //            {
        //                dbContextTransaction.Rollback();
        //                return userId;
        //                throw;
        //            }
        //        }
        //    }
        //}

        public int RegisterInvoiceCustomerSupplier(AddCustomerSupplierDto Obj)
        {
            int userId = 0;

            using (var context = new KFentities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //invoiceuser emil <> obj email || this condition is added 5-12-2016 (InvoiceData)
                        var CustomerExist = context.tblCustomerOrSuppliers.Where(i => i.Email == Obj.Email && i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).FirstOrDefault();
                        //var InvoiceRegisterUser = context.InvoiceUserRegistrations.Where(i => (i.Id == Obj.UserId)).FirstOrDefault();

                        if (CustomerExist == null) //InvoiceRegisterUser == null &&
                        {
                            if (Obj.RoleId == 2)
                            {
                                // userid email != invoice user email  && i.EmailTo != Obj.Email
                                var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
                                if (data != null)
                                {
                                    int debator = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
                                    debator = debator + 1;
                                    Obj.Debtors = "1200-" + debator;
                                }
                                else
                                {
                                    Obj.Debtors = "1200-1000";
                                }

                                var InvoiceData = context.InvoiceUserRegistrations.Where(i => i.Id == Obj.UserId).FirstOrDefault();//add emal check

                                var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == Obj.Email).Select(s => s.Id).FirstOrDefault();

                                if (InvoiceData.Id > 0)
                                {
                                    tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();
                                    dbinsert.Company_Name = InvoiceData.CompanyName;
                                    dbinsert.FirstName = InvoiceData.ContactPerson;
                                    dbinsert.Address = InvoiceData.CorporateAptNo + "|||" + InvoiceData.CorporateHouseNo + "|||" + InvoiceData.CorporateStreet;
                                    dbinsert.State = InvoiceData.CorporateStreet;
                                    dbinsert.City = InvoiceData.CorporateCity;
                                    dbinsert.PostalCode = InvoiceData.CorporatePostalCode;
                                    dbinsert.ServiceOffered = InvoiceData.GoodsType;
                                    dbinsert.Mobile = InvoiceData.MobileNumber;
                                    dbinsert.Email = InvoiceData.EmailTo;
                                    dbinsert.Website = InvoiceData.Website;
                                    dbinsert.UserId = InvoiceUserId;
                                    dbinsert.Isdeleted = false;
                                    dbinsert.Telephone = InvoiceData.MobileNumber;
                                    dbinsert.RoleId = 1;


                                    var InvoiceDataCreater = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
                                    if (InvoiceDataCreater != null)
                                    {
                                        int credetor = Convert.ToInt32(InvoiceDataCreater.Substring(InvoiceDataCreater.LastIndexOf('-') + 1));
                                        credetor = credetor + 1;
                                        dbinsert.Credetor = "2250-" + credetor;

                                    }
                                    else
                                    {
                                        dbinsert.Credetor = "2250-1000";
                                    }
                                    //if dbinsert email == userregistration email
                                    var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
                                    if (EmailExist != null)
                                    {
                                        dbinsert.IsFinance = true;
                                    }
                                    else
                                    {
                                        dbinsert.IsFinance = false;
                                    }
                                    context.tblCustomerOrSuppliers.Add(dbinsert);
                                    context.SaveChanges();

                                    #region Developer Gurinder  16-1-2017
                                    var ClassificationData = new Classification();
                                    ClassificationData.ChartAccountDisplayNumber = Obj.Debtors;
                                    ClassificationData.ChartAccountNumber = Convert.ToInt32(Obj.Debtors.Remove(4, 1));
                                    ClassificationData.ClassificationType = InvoiceData.CompanyName;
                                    ClassificationData.Desc = InvoiceData.CompanyName;
                                    ClassificationData.Name = InvoiceData.CompanyName;
                                    ClassificationData.IsDeleted = false;
                                    ClassificationData.CategoryId = 4;
                                    ClassificationData.Type = "A";
                                    context.Classifications.Add(ClassificationData);
                                    context.SaveChanges();
                                    #endregion
                                    Mapper.CreateMap<AddCustomerSupplierDto, tblCustomerOrSupplier>();

                                    var userData = Mapper.Map<tblCustomerOrSupplier>(Obj);
                                    var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == Obj.Email);
                                    if (EmailExistinFinance != null)
                                    {
                                        userData.IsFinance = true;
                                    }
                                    else
                                    {
                                        userData.IsFinance = false;
                                    }
                                    userData.DateCreated = DateTime.Now;
                                    userData.Isdeleted = false;
                                    context.tblCustomerOrSuppliers.Add(userData);
                                    context.SaveChanges();
                                    userId = userData.Id;

                                    #region Developer Gurinder  16-1-2017
                                    var Classification = new Classification();
                                    Classification.ChartAccountDisplayNumber = dbinsert.Credetor;
                                    Classification.ChartAccountNumber = Convert.ToInt32(dbinsert.Credetor.Remove(4, 1));
                                    Classification.ClassificationType = Obj.Company_Name;
                                    Classification.Desc = Obj.Company_Name;
                                    Classification.Name = Obj.Company_Name;
                                    Classification.IsDeleted = false;
                                    Classification.CategoryId = 4;
                                    Classification.Type = "A";
                                    context.Classifications.Add(Classification);
                                    context.SaveChanges();
                                    #endregion
                                }
                                dbContextTransaction.Commit();
                                return userId;
                            }
                            else if (Obj.RoleId == 1)
                            {
                                //invoiceuser emil <> obj email 

                                var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == Obj.UserId && i.RoleId == Obj.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
                                if (data != null)
                                {
                                    int credetor = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
                                    credetor = credetor + 1;
                                    Obj.Credetor = "2250-" + credetor;
                                }
                                else
                                {
                                    Obj.Credetor = "2250-1000";
                                }

                                var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == Obj.Email).Select(s => s.Id).FirstOrDefault();

                                var InvoiceUserData = context.InvoiceUserRegistrations.Where(i => i.Id == Obj.UserId).FirstOrDefault();

                                tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();

                                if (InvoiceUserData.Id > 0 && dbinsert.Email != Obj.Email) //add emal check
                                {
                                    dbinsert.Company_Name = InvoiceUserData.CompanyName;
                                    dbinsert.FirstName = InvoiceUserData.ContactPerson;
                                    dbinsert.Address = InvoiceUserData.CorporateAptNo + "|||" + InvoiceUserData.CorporateHouseNo + "|||" + InvoiceUserData.CorporateStreet;
                                    dbinsert.State = InvoiceUserData.CorporateStreet;
                                    dbinsert.City = InvoiceUserData.CorporateCity;
                                    dbinsert.PostalCode = InvoiceUserData.CorporatePostalCode;
                                    dbinsert.ShippingAddress = InvoiceUserData.ShippingAptNo + "|||" + InvoiceUserData.ShippingHouseNo + "|||" + InvoiceUserData.ShippingStreet;
                                    dbinsert.ShippingState = InvoiceUserData.ShippingState;
                                    dbinsert.ShippingCity = InvoiceUserData.ShippingCity;
                                    dbinsert.ShippingPostalCode = InvoiceUserData.ShippingPostalCode;
                                    dbinsert.AdditionalEmail = InvoiceUserData.EmailCc;
                                    dbinsert.Mobile = InvoiceUserData.MobileNumber;
                                    dbinsert.Email = InvoiceUserData.EmailTo;
                                    dbinsert.Website = InvoiceUserData.Website;
                                    dbinsert.UserId = InvoiceUserId;
                                    dbinsert.Isdeleted = false;
                                    dbinsert.Telephone = InvoiceUserData.MobileNumber;
                                    dbinsert.RoleId = 2;


                                    var CusData = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
                                    if (CusData != null)
                                    {
                                        int debator = Convert.ToInt32(data.Substring(CusData.LastIndexOf('-') + 1));
                                        debator = debator + 1;
                                        dbinsert.Debtors = "1200-" + debator;

                                    }

                                    else
                                    {
                                        dbinsert.Debtors = "1200-1000";
                                    }
                                    //if dbinsert email == userregistration email


                                    var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
                                    if (EmailExist != null)
                                    {
                                        dbinsert.IsFinance = true;
                                    }
                                    else
                                    {
                                        dbinsert.IsFinance = false;
                                    }
                                    context.tblCustomerOrSuppliers.Add(dbinsert);
                                    context.SaveChanges();

                                    #region Developer Gurinder  16-1-2017
                                    var Classification = new Classification();
                                    Classification.ChartAccountDisplayNumber = Obj.Credetor;
                                    Classification.ChartAccountNumber = Convert.ToInt32(Obj.Credetor.Remove(4, 1));
                                    Classification.ClassificationType = InvoiceUserData.CompanyName;
                                    Classification.Desc = InvoiceUserData.CompanyName;
                                    Classification.Name = InvoiceUserData.CompanyName;
                                    Classification.IsDeleted = false;
                                    Classification.CategoryId = 4;
                                    Classification.Type = "A";
                                    context.Classifications.Add(Classification);
                                    context.SaveChanges();
                                    #endregion

                                    Mapper.CreateMap<AddCustomerSupplierDto, tblCustomerOrSupplier>();

                                    var userData = Mapper.Map<tblCustomerOrSupplier>(Obj);
                                    var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == Obj.Email);
                                    if (EmailExistinFinance != null)
                                    {
                                        userData.IsFinance = true;
                                    }
                                    else
                                    {
                                        userData.IsFinance = false;
                                    }
                                    userData.DateCreated = DateTime.Now;
                                    userData.Isdeleted = false;
                                    context.tblCustomerOrSuppliers.Add(userData);
                                    context.SaveChanges();
                                    userId = userData.Id;

                                    #region Developer Gurinder  16-1-2017
                                    var ClassificationData = new Classification();
                                    ClassificationData.ChartAccountDisplayNumber = dbinsert.Debtors;
                                    ClassificationData.ChartAccountNumber = Convert.ToInt32(dbinsert.Debtors.Remove(4, 1));
                                    ClassificationData.ClassificationType = Obj.Company_Name;
                                    ClassificationData.Desc = Obj.Company_Name;
                                    ClassificationData.Name = Obj.Company_Name;
                                    ClassificationData.IsDeleted = false;
                                    ClassificationData.CategoryId = 4;
                                    ClassificationData.Type = "A";
                                    context.Classifications.Add(ClassificationData);
                                    context.SaveChanges();
                                    #endregion

                                }
                                dbContextTransaction.Commit();
                                return userId;
                            }
                            return userId;
                        }
                        else
                        {
                            return 0;
                        }

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

        public bool ExistingInvoiceUsernameCheck(InvoiceUserRegistrationDto obj)
        {
            var UserExisted = false;
            using (var dbContext = new KFentities())
            {
                try
                {
                    var userExistChk = dbContext.InvoiceUserRegistrations.Where(i => i.Username == obj.Username).Any();
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
        public bool AlreadyCustomer(AddCustomerSupplierDto ObjUser)
        {
            try
            {
                bool IsSave = false;
                using (var context = new KFentities())
                {
                    var UserDetails = context.tblCustomerOrSuppliers.Where(i => i.Email == ObjUser.Email && i.RoleId == ObjUser.RoleId).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        Mapper.CreateMap<AddCustomerSupplierDto, tblCustomerOrSupplier>();
                        if (ObjUser.RoleId == 2)
                        {
                            var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == ObjUser.UserId && i.RoleId == ObjUser.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
                            if (data != null)
                            {
                                int debator = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
                                debator = debator + 1;
                                UserDetails.Debtors = "1200-" + debator;
                            }
                            else
                            {
                                UserDetails.Debtors = "1200-1000";
                            }

                            var InvoiceData = context.InvoiceUserRegistrations.Where(i => i.Id == ObjUser.UserId).FirstOrDefault();
                            var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == ObjUser.Email).Select(s => s.Id).FirstOrDefault();

                            if (InvoiceData.Id > 0)
                            {
                                tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();

                                dbinsert.Company_Name = InvoiceData.CompanyName;
                                dbinsert.FirstName = InvoiceData.ContactPerson;
                                dbinsert.Address = InvoiceData.CorporateAptNo + "|||" + InvoiceData.CorporateHouseNo + "|||" + InvoiceData.CorporateStreet;
                                dbinsert.State = InvoiceData.CorporateStreet;
                                dbinsert.City = InvoiceData.CorporateCity;
                                dbinsert.PostalCode = InvoiceData.CorporatePostalCode;
                                dbinsert.ServiceOffered = InvoiceData.GoodsType;
                                dbinsert.Mobile = InvoiceData.MobileNumber;
                                dbinsert.Email = InvoiceData.EmailTo;
                                dbinsert.Website = InvoiceData.Website;
                                dbinsert.UserId = InvoiceUserId;
                                dbinsert.Isdeleted = false;
                                dbinsert.Telephone = InvoiceData.MobileNumber;
                                dbinsert.RoleId = 1;

                                var InvoiceDataCreater = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
                                if (InvoiceDataCreater != null)
                                {
                                    int credetor = Convert.ToInt32(InvoiceDataCreater.Substring(InvoiceDataCreater.LastIndexOf('-') + 1));
                                    credetor = credetor + 1;
                                    dbinsert.Credetor = "2250-" + credetor;

                                }
                                else
                                {
                                    dbinsert.Credetor = "2250-1000";
                                }
                                var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
                                if (EmailExist != null)
                                {
                                    dbinsert.IsFinance = true;
                                }
                                else
                                {
                                    dbinsert.IsFinance = false;
                                }
                                context.tblCustomerOrSuppliers.Add(dbinsert);
                                context.SaveChanges();

                            }
                            UserDetails.UserId = ObjUser.UserId;
                            var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == ObjUser.Email);
                            if (EmailExistinFinance != null)
                            {
                                UserDetails.IsFinance = true;
                            }
                            else
                            {
                                UserDetails.IsFinance = false;
                            }
                            context.tblCustomerOrSuppliers.Add(UserDetails);
                        }
                        else if (ObjUser.RoleId == 1)
                        {
                            var data = context.tblCustomerOrSuppliers.Where(i => i.UserId == ObjUser.UserId && i.RoleId == ObjUser.RoleId).OrderByDescending(i => i.Id).Select(i => i.Credetor).FirstOrDefault();
                            if (data != null)
                            {
                                int credetor = Convert.ToInt32(data.Substring(data.LastIndexOf('-') + 1));
                                credetor = credetor + 1;
                                UserDetails.Credetor = "2250-" + credetor;
                            }
                            else
                            {
                                UserDetails.Credetor = "2250-1000";
                            }

                            var InvoiceUserId = context.InvoiceUserRegistrations.Where(i => i.EmailTo == ObjUser.Email).Select(s => s.Id).FirstOrDefault();
                            var InvoiceUserData = context.InvoiceUserRegistrations.Where(i => i.Id == ObjUser.UserId).FirstOrDefault();
                            if (InvoiceUserData.Id > 0)
                            {
                                tblCustomerOrSupplier dbinsert = new tblCustomerOrSupplier();

                                dbinsert.Company_Name = InvoiceUserData.CompanyName;
                                dbinsert.FirstName = InvoiceUserData.ContactPerson;
                                dbinsert.Address = InvoiceUserData.CorporateAptNo + "|||" + InvoiceUserData.CorporateHouseNo + "|||" + InvoiceUserData.CorporateStreet;
                                dbinsert.State = InvoiceUserData.CorporateStreet;
                                dbinsert.City = InvoiceUserData.CorporateCity;
                                dbinsert.PostalCode = InvoiceUserData.CorporatePostalCode;
                                dbinsert.ShippingAddress = InvoiceUserData.ShippingAptNo + "|||" + InvoiceUserData.ShippingHouseNo + "|||" + InvoiceUserData.ShippingStreet;
                                dbinsert.ShippingState = InvoiceUserData.ShippingState;
                                dbinsert.ShippingCity = InvoiceUserData.ShippingCity;
                                dbinsert.ShippingPostalCode = InvoiceUserData.ShippingPostalCode;
                                dbinsert.AdditionalEmail = InvoiceUserData.EmailCc;
                                dbinsert.Mobile = InvoiceUserData.MobileNumber;
                                dbinsert.Email = InvoiceUserData.EmailTo;
                                dbinsert.Website = InvoiceUserData.Website;
                                dbinsert.UserId = InvoiceUserId;
                                dbinsert.Isdeleted = false;
                                dbinsert.Telephone = InvoiceUserData.MobileNumber;
                                dbinsert.RoleId = 2;

                                var CusData = context.tblCustomerOrSuppliers.Where(i => i.UserId == dbinsert.UserId && i.RoleId == dbinsert.RoleId).OrderByDescending(i => i.Id).Select(i => i.Debtors).FirstOrDefault();
                                if (CusData != null)
                                {
                                    int debator = Convert.ToInt32(data.Substring(CusData.LastIndexOf('-') + 1));
                                    debator = debator + 1;
                                    dbinsert.Debtors = "1200-" + debator;
                                }

                                else
                                {
                                    dbinsert.Debtors = "1200-1000";
                                }
                                //if dbinsert email == userregistration email
                                var EmailExist = context.UserRegistrations.FirstOrDefault(x => x.Email == dbinsert.Email);
                                if (EmailExist != null)
                                {
                                    dbinsert.IsFinance = true;
                                }
                                else
                                {
                                    dbinsert.IsFinance = false;
                                }
                                context.tblCustomerOrSuppliers.Add(dbinsert);

                                context.SaveChanges();
                            }
                            UserDetails.UserId = ObjUser.UserId;
                            var EmailExistinFinance = context.UserRegistrations.FirstOrDefault(x => x.Email == ObjUser.Email);
                            if (EmailExistinFinance != null)
                            {
                                UserDetails.IsFinance = true;
                            }
                            else
                            {
                                UserDetails.IsFinance = false;
                            }
                            context.tblCustomerOrSuppliers.Add(UserDetails);
                        }
                        //var userData = Mapper.Map<tblCustomerOrSupplier>(ObjUser);
                        context.SaveChanges();
                        IsSave = true;
                        return IsSave;
                    }
                    else
                    {
                        return IsSave;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<AddCustomerSupplierDto> InvoiceCustomerSupplierList(int userid, int roleid)
        {
            List<AddCustomerSupplierDto> ObjList = new List<AddCustomerSupplierDto>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList = new List<tblCustomerOrSupplier>();
            List<tblCustomerOrSupplier> ObjAllEmployeeList1 = new List<tblCustomerOrSupplier>();
            try
            {
                using (var context = new KFentities())
                {
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
                        Mapper.CreateMap<tblCustomerOrSupplier, AddCustomerSupplierDto>();

                        return ObjList = Mapper.Map<List<AddCustomerSupplierDto>>(ObjAllEmployeeList);

                        //ObjAllEmployeeList = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        //Mapper.CreateMap<tblCustomerOrSupplier, AddCustomerSupplierDto>();
                        //return ObjList = Mapper.Map<List<AddCustomerSupplierDto>>(ObjAllEmployeeList);

                    }
                    else
                    {
                        var Invoice_ID = context.tblCustomerOrSuppliers.Where(i => i.RoleId == roleid && i.UserId == userid).ToList();
                        foreach (var InvoiData in Invoice_ID)
                        {
                            int ids = Convert.ToInt32(InvoiData.Id);

                            ObjAllEmployeeList1 = context.tblCustomerOrSuppliers.Where(i => i.Id == ids).ToList();
                            ObjAllEmployeeList.AddRange(ObjAllEmployeeList1);
                            Mapper.CreateMap<tblCustomerOrSupplier, AddCustomerSupplierDto>();
                            //ObjList.Add(ObjAllEmployeeList);  // = Mapper.Map<List<AddCustomerSupplierDto>>(ObjAllEmployeeList);
                        }

                        // Mapper.CreateMap<tblCustomerOrSupplier, AddCustomerSupplierDto>();
                        return ObjList = Mapper.Map<List<AddCustomerSupplierDto>>(ObjAllEmployeeList);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public AddCustomerSupplierDto UpdateInvoiceCustomerSupplier(AddCustomerSupplierDto objUser)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == objUser.Id).FirstOrDefault();
                    if (userChk != null)
                    {
                        userChk.FirstName = objUser.FirstName;
                        userChk.Company_Name = objUser.Company_Name;
                        userChk.Mobile = objUser.Mobile;
                        userChk.PostalCode = objUser.PostalCode;
                        userChk.Address = objUser.Address;
                        userChk.AdditionalEmail = objUser.AdditionalEmail;
                        userChk.City = objUser.City;
                        userChk.ServiceOffered = objUser.ServiceOffered;
                        userChk.ShippingAddress = objUser.ShippingAddress;
                        userChk.ShippingCity = objUser.ShippingCity;
                        userChk.ShippingPostalCode = objUser.ShippingPostalCode;
                        userChk.ShippingState = objUser.ShippingState;
                        userChk.State = objUser.State;
                        userChk.Website = objUser.Website;
                        userChk.Telephone = objUser.Telephone;
                    }
                    context.SaveChanges();
                    Mapper.CreateMap<tblCustomerOrSupplier, AddCustomerSupplierDto>();
                    return Mapper.Map<AddCustomerSupplierDto>(userChk);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }
        #endregion

        # region Invoice Create
        //Developer - Gurinder 
        //    Date - 21-11-2016

        public List<Classification> RevenueList(int UserId, int num)
        {
            using (var db = new KFentities())
            {
                try
                {
                    if (num == 1)
                    {
                        return db.Classifications.Where(i => (i.UserId == null) && i.CategoryId == 3 && (i.Type == "A" || i.Type == "G") && (i.IndustryId == null)).ToList();
                    }
                    else if (num == 2)
                    {
                        var Industrydata = db.KI_getIndustryID(UserId).FirstOrDefault();

                        if (Industrydata != null)
                        {
                            return db.Classifications.Where(i => (i.UserId == null || i.UserId == Industrydata.Id) && i.CategoryId == 3 && (i.Type == "A" || i.Type == "G") && (i.IndustryId == null || (i.IndustryId == Industrydata.IndustryId && i.SubIndustryId == Industrydata.Ind))).ToList();
                        }
                        else
                        {
                            return db.Classifications.Where(i => (i.UserId == null) && i.CategoryId == 3 && (i.Type == "A" || i.Type == "G") && (i.IndustryId == null)).ToList();
                        }

                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null;
        }

        public int CreateInvoiceNumber(int UserId)
        {
            int invoicenumber = 00001;
            using (var context = new KFentities())
            {
                try
                {
                    var lastnumber = context.tblInvoiceDetails.Where(i => i.UserId == UserId).OrderByDescending(s => s.Id).Select(n => n.InvoiceNumber).FirstOrDefault();
                    if (lastnumber != null)
                    {
                        invoicenumber = Convert.ToInt32(lastnumber) + 1;
                    }
                    else
                    {
                        return invoicenumber;
                    }
                    return invoicenumber;

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //public int CreateInvoice(InvoiceDetailDto Obj)
        //{
        //    int userId = 0;

        //    using (var context = new KFentities())
        //    {
        //        using (var dbContextTransaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var userData = new tblInvoiceDetail();

        //                if (userData.InvoiceNumber == null)
        //                {
        //                    if (Obj.ButtonType == "Save")
        //                    {
        //                        userData.In_R_FlowStatus = "Draft";
        //                        userData.In_R_Status = "Inprogress";
        //                        userData.Pro_FlowStatus = "Draft";
        //                        userData.Pro_Status = "Inprogress";
        //                    }
        //                    userData.CreatedDate = DateTime.Now;
        //                    if (Obj.ButtonType == "Send")
        //                    {
        //                        //customerid =tblcustomer = email


        //                        userData.In_R_FlowStatus = "Sent";
        //                        userData.In_R_Status = "Inprogress";
        //                        userData.Pro_FlowStatus = "Pending Approval";
        //                        userData.Pro_Status = "Inprogress";

        //                    }
        //                    userData.BalanceDue = Obj.BalanceDue;
        //                    userData.DocumentRef = Obj.DocumentRef;
        //                    userData.SalesPerson = Obj.SalesPerson;
        //                    //userData.DepositePayment = Obj.DepositePayment;
        //                    userData.UserId = Obj.UserId;
        //                    // userData.SubTotal = Obj.SubTotal;
        //                    userData.Total = Obj.Total;
        //                    if (Obj.ShippingCost == null || Obj.ShippingCost.HasValue == false)
        //                    {
        //                        userData.ShippingCost = 0;
        //                    }
        //                    else
        //                    {
        //                        userData.ShippingCost = Obj.ShippingCost;
        //                    }
        //                    if (Obj.DepositePayment == null || Obj.DepositePayment.HasValue == false)
        //                    {
        //                        userData.DepositePayment = 0;
        //                    }
        //                    else
        //                    {
        //                        userData.DepositePayment = Obj.DepositePayment;
        //                    }
        //                    userData.Terms = Obj.Terms;
        //                    userData.Note = Obj.Note;
        //                    userData.InvoiceDate = Obj.InvoiceDate;
        //                    userData.DueDate = Obj.DueDate;
        //                    userData.PaymentTerms = Obj.PaymentTerms;
        //                    userData.InvoiceNumber = Obj.InvoiceNumber;
        //                    userData.DepositePayment = Obj.DepositePayment;
        //                    userData.CustomerId = Obj.CustomerId;
        //                    userData.Type = Obj.Type;
        //                    userData.IsInvoiceReport = true;
        //                    userData.IsDeleted = false;
        //                    userData.CustomerManualPaidAmount = (decimal)0.0000;
        //                    userData.SupplierManualPaidAmount = (decimal)0.0000;
        //                    userData.IsCustomerManualPaid = false;
        //                    userData.IsSupplierManualPaid = false;
        //                    context.tblInvoiceDetails.Add(userData);
        //                    context.SaveChanges();

        //                    tblItemDetail dbinsert = new tblItemDetail();
        //                    var counter = Obj.ServiceTypeId.Count();

        //                    for (int i = 0; i < counter; i++)
        //                    {
        //                        dbinsert.InvoiceId = userData.Id;
        //                        dbinsert.ItemId = Obj.ItemId[i];
        //                        dbinsert.Item = Obj.Item[i];
        //                        dbinsert.ServiceTypeId = Obj.ServiceTypeId[i];
        //                        dbinsert.Description = Obj.Description[i];
        //                        dbinsert.Quantity = Obj.Quantity[i];
        //                        dbinsert.Rate = Obj.Rate[i];
        //                        dbinsert.Amount = Obj.Amount[i];
        //                        dbinsert.Discount = Obj.Discount[i];
        //                        dbinsert.SubTotal = Obj.SubTotal[i];

        //                        if (Obj.Tax[i] == "null")
        //                        {
        //                            dbinsert.Tax = EmptyIfNull(Obj.Tax[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.Tax = Obj.Tax[i];
        //                        }
        //                        if (Obj.GST_Tax[i] == "null")
        //                        {
        //                            dbinsert.GST_Tax = EmptyIfNull(Obj.GST_Tax[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.GST_Tax = Obj.GST_Tax[i];
        //                        }
        //                        if (Obj.HST_Tax[i] == "null")
        //                        {
        //                            dbinsert.HST_Tax = EmptyIfNull(Obj.HST_Tax[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.HST_Tax = Obj.HST_Tax[i];
        //                        }
        //                        if (Obj.PST_Tax[i] == "null")
        //                        {
        //                            dbinsert.PST_Tax = EmptyIfNull(Obj.PST_Tax[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.PST_Tax = Obj.PST_Tax[i];
        //                        }
        //                        if (Obj.QST_Tax[i] == "null")
        //                        {
        //                            dbinsert.QST_Tax = EmptyIfNull(Obj.QST_Tax[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.QST_Tax = Obj.QST_Tax[i];
        //                        }

        //                        if (Obj.Customer_Service[i] == "null")
        //                        {
        //                            dbinsert.Customer_Service = EmptyIfNull(Obj.Customer_Service[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.Customer_Service = Obj.Customer_Service[i];
        //                        }
        //                        if (Obj.ServiceType[i] == "null")
        //                        {
        //                            dbinsert.ServiceType = EmptyIfNull(Obj.ServiceType[i]);
        //                        }
        //                        else
        //                        {
        //                            dbinsert.ServiceType = Obj.ServiceType[i];
        //                        }
        //                        dbinsert.Customer_ServiceTypeId = Obj.Customer_ServiceTypeId[i];

        //                        context.tblItemDetails.Add(dbinsert);
        //                        context.SaveChanges();
        //                    }

        //                    dbContextTransaction.Commit();
        //                    if (Obj.ButtonType == "Send")
        //                    {
        //                        GeneratingReport genReport = new GeneratingReport();
        //                        pdfPath = genReport.PrintRun(obj.Id, 1);

        //                        InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
        //                        InvoiceEmail.SendEmailToInvoice(obj.Id, pdfPath);

        //                    }
        //                    return userId = userData.Id;
        //                }
        //                else
        //                {
        //                    return userId;
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                dbContextTransaction.Rollback();
        //                return userId;
        //                throw;
        //            }
        //        }
        //    }
        //}

        #endregion

        public string EmptyIfNull(string value)
        {
            if (value == "null")
                value = string.Empty;
            return value.ToString();
        }

        #region Return Invoive/Proforma Listing
        //Developer Gurinder
        #region Send Invoice
        //Developer - Gurinder 
        //    Date - 21-11-2016
        public List<Sp_SendInvoiceListDto> SendInvoiceList(int userid)
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
                                       PaymentDate = Invoice.PaymentDate.ToString()
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

        #region Get ItemsData by InvoiceId
        //Developer - Gurinder 
        //    Date - 22-11-2016
        public List<ItemtableDto> GetItemsDatabyInvoiceId(int InvoiceId)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        var Itemsobj = (from Item in context.tblItemDetails
                                        where Item.InvoiceId == InvoiceId
                                        select new ItemtableDto
                                           {
                                               Id = Item.Id,
                                               InvoiceId = Item.InvoiceId,
                                               Item = Item.Item,
                                               Quantity = Item.Quantity,
                                               Amount = Item.Amount,
                                               Description = Item.Description,
                                               ServiceType = Item.ServiceType,
                                               Discount = Item.Discount,
                                               Tax = Item.Tax,
                                               GST_Tax = Item.GST_Tax,
                                               QST_Tax = Item.QST_Tax,
                                               PST_Tax = Item.PST_Tax,
                                               HST_Tax = Item.HST_Tax,
                                               ServiceTypeId = Item.ServiceTypeId,
                                               ItemId = Item.ItemId,
                                               Rate = Item.Rate,
                                               SubTotal = Item.SubTotal
                                           }).ToList();
                        return Itemsobj;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Send Proforma
        //Developer - Gurinder 
        //    Date - 22-11-2016
        public List<Sp_SendInvoiceListDto> SendProformaList(int userid)
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
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       PaymentDate = Invoice.PaymentDate.ToString()
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

        #region Received Proforma
        //Developer - Gurinder 
        //    Date - 23-11-2016
        public List<Sp_SendInvoiceListDto> ReceivedProformaList(int userid)
        {
            try
            {
                using (var context = new KFentities())
                {

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
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       PaymentDate = Invoice.PaymentDate.ToString()

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

        #region Received Invoice
        //Developer - Gurinder 
        //    Date - 23-11-2016
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
                                       FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                       PaymentDate = Invoice.PaymentDate.ToString()
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

        #region ReportingList
        //Developer - Gurinder 
        //    Date - 23-11-2016
        public List<Sp_SendInvoiceListDto> ReportingList(int userid)
        {
            Nullable<int> aa = userid;
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
                                            DepositePayment = Convert.ToDecimal(Invoice.DepositePayment == null ? 0 : Invoice.DepositePayment),
                                            BalanceDue = Convert.ToDecimal(Invoice.BalanceDue == null ? 0 : Invoice.BalanceDue),// Invoice.BalanceDue,
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
                                            ShippingCost = Convert.ToDecimal(Invoice.ShippingCost == null ? 0 : Invoice.ShippingCost),
                                            //Invoice.ShippingCost,
                                            Terms = Invoice.Terms,
                                            Total = Convert.ToDecimal(Invoice.Total == null ? 0 : Invoice.Total),//Invoice.Total,
                                            Type = Invoice.Type,
                                            UserId = Invoice.UserId,
                                            Username = context.Kl_GetCompanyNameSend(userid, Invoice.Id).FirstOrDefault(),
                                            FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                            IsCustomer = false,
                                            IsSupplierManualPaid = Invoice.IsSupplierManualPaid,
                                            SupplierManualPaidAmount = Convert.ToDecimal(Invoice.SupplierManualPaidAmount == null ? 0 : Invoice.SupplierManualPaidAmount), //Invoice.SupplierManualPaidAmount,
                                            SupplierManualPaidJVID = Invoice.SupplierManualPaidJVID,
                                            IsCustomerManualPaid = Invoice.IsCustomerManualPaid,
                                            CustomerManualPaidAmount = Convert.ToDecimal(Invoice.CustomerManualPaidAmount == null ? 0 : Invoice.CustomerManualPaidAmount),
                                            CustomerManualPaidJVID = Invoice.CustomerManualPaidJVID,
                                            IsStripe = false,
                                            InvoiceJVID = Invoice.InvoiceJVID,
                                            PaymentDate = Invoice.PaymentDate.ToString(),
                                            StripeJVID = Invoice.StripeJVID == null ? "" : Invoice.StripeJVID,
                                            IsPaymentbyStripe = (bool)context.StripePaymnts.Where(r => r.InvoiceID == Invoice.Id).Select(s => s.IsPaymentbyStripe).FirstOrDefault()
                                        }).ToList();


                    MainList.AddRange(listByUserid);


                    // var data = from i in context.kl_ReportingListwithoutDraft(userid) select i;
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
                                       //  ModifyDate = Invoice.ModifyDate,
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
                    return MainList;
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        #endregion

        #endregion

        #region Currency List
        public List<Currency> GetCurrencyList()
        {
            using (var db = new KFentities())
            {
                try
                {
                    return db.Currencies.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<tblCurrency> GetCurrencyListInvoice()
        {
            using (var db = new KFentities())
            {
                try
                {
                    return db.tblCurrencies.ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region ExpenseAssetList
        //Developer Gurinder
        public List<Kl_BlockListDto> ExpenseAssetList(int UserId, int num)
        {
            using (var db = new KFentities())
            {
                try
                {
                    List<Kl_BlockListDto> listuu = new List<Kl_BlockListDto>();
                    if (num == 1)
                    {


                        listuu = (from BlockList in db.Kl_BlockAccNoList(UserId)
                                  select new Kl_BlockListDto
                                   {
                                       Id = BlockList.Id,
                                       CategoryId = BlockList.CategoryId,
                                       CategoryValue = BlockList.CategoryValue,
                                       ChartAccountNumber = BlockList.ChartAccountDisplayNumber,
                                       ClassificationType = BlockList.ClassificationType,
                                       CreatedDate = BlockList.CreatedDate,
                                       Desc = BlockList.Desc,
                                       IndustryId = BlockList.IndustryId,
                                       IsDeleted = BlockList.IsDeleted,
                                       IsIncorporated = BlockList.IsIncorporated,
                                       IsPartnerShip = BlockList.IsPartnerShip,
                                       IsSole = BlockList.IsSole,
                                       ModifiedDate = BlockList.ModifiedDate,
                                       Name = BlockList.Name,
                                       RangeofAct = BlockList.RangeofAct,
                                       SubIndustryId = BlockList.SubIndustryId,
                                       Type = BlockList.Type,
                                       UserId = BlockList.UserId,
                                   }).ToList();
                        return listuu;
                    }
                    else if (num == 2)
                    {

                        listuu = (from BlockList in db.Kl_BlockAccNoList_forUserIdNull(UserId)
                                  select new Kl_BlockListDto
                                  {
                                      Id = BlockList.Id,
                                      CategoryId = BlockList.CategoryId,
                                      CategoryValue = BlockList.CategoryValue,
                                      ChartAccountNumber = BlockList.ChartAccountDisplayNumber,
                                      ClassificationType = BlockList.ClassificationType,
                                      CreatedDate = BlockList.CreatedDate,
                                      Desc = BlockList.Desc,
                                      IndustryId = BlockList.IndustryId,
                                      IsDeleted = BlockList.IsDeleted,
                                      IsIncorporated = BlockList.IsIncorporated,
                                      IsPartnerShip = BlockList.IsPartnerShip,
                                      IsSole = BlockList.IsSole,
                                      ModifiedDate = BlockList.ModifiedDate,
                                      Name = BlockList.Name,
                                      RangeofAct = BlockList.RangeofAct,
                                      SubIndustryId = BlockList.SubIndustryId,
                                      Type = BlockList.Type,
                                      UserId = BlockList.UserId,
                                  }).ToList();
                        return listuu;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Update Invoice

        //public int UpdateInvoice(InvoiceDetailDto obj)
        //{
        //    int InvoiceId = 0;
        //    try
        //    {
        //        using (var context = new KFentities())
        //        {
        //            using (var dbContextTransaction = context.Database.BeginTransaction())
        //            {
        //                var userChk = context.tblInvoiceDetails.Where(i => i.Id == obj.Id).FirstOrDefault();
        //                if (userChk != null)
        //                {
        //                    if (obj.ButtonType == "Save" && obj.Type == 1 && obj.SectionType == "Sent")
        //                    {

        //                        if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                        {
        //                            userChk.DepositePayment = 0;
        //                        }
        //                        else
        //                        {
        //                            userChk.DepositePayment = obj.DepositePayment;
        //                        }
        //                        userChk.BalanceDue = obj.BalanceDue;
        //                        userChk.In_R_FlowStatus = "Draft";
        //                        userChk.In_R_Status = "Inprogress";
        //                        userChk.Pro_FlowStatus = "Draft";
        //                        userChk.Pro_Status = "Inprogress";
        //                        context.SaveChanges();

        //                        var counter = obj.ServiceTypeId.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {
        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.ServiceType[i] == "null")
        //                            {
        //                                item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                            }
        //                            else
        //                            {
        //                                item.ServiceType = obj.ServiceType[i];
        //                            }
        //                            //obj.ServiceType[i];
        //                            item.ServiceTypeId = obj.ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                    else if (obj.ButtonType == "Send" && obj.Type == 1 && obj.SectionType == "Sent")
        //                    {

        //                        // invoiceid = customerid =tblcustomer = email

        //                        var email = context.kl_getCustomerEmail(obj.InvoiceId).FirstOrDefault();
        //                        string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplateInvoice.html"));
        //                        var counter = email.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {
        //                            SendMailModelDto _objModelMail = new SendMailModelDto();
        //                            _objModelMail.To = email;
        //                            _objModelMail.Subject = "Invitation mail from Kippin-Invoice";
        //                            _objModelMail.MessageBody = html;
        //                            var mailSent = Sendmail.SendEmail(_objModelMail);
        //                        }

        //                        if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                        {

        //                            userChk.DepositePayment = 0;

        //                        }
        //                        else
        //                        {
        //                            userChk.DepositePayment = obj.DepositePayment;
        //                        }

        //                        userChk.BalanceDue = obj.BalanceDue;
        //                        userChk.In_R_FlowStatus = "Sent";
        //                        userChk.In_R_Status = "Inprogress";
        //                        userChk.Pro_FlowStatus = "Pending Approval";
        //                        userChk.Pro_Status = "Inprogress";
        //                        context.SaveChanges();
        //                        tblItemDetail Items = new tblItemDetail();

        //                        var counter1 = obj.ServiceTypeId.Count();
        //                        for (int i = 0; i < counter1; i++)
        //                        {
        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.ServiceType[i] == "null")
        //                            {
        //                                item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                            }
        //                            else
        //                            {
        //                                item.ServiceType = obj.ServiceType[i];
        //                            }
        //                            item.ServiceTypeId = obj.ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }

        //                    }
        //                    else if (obj.ButtonType == "Accept" && obj.Type == 1 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Closed";
        //                        userChk.In_R_Status = "Accepted";
        //                        userChk.Pro_FlowStatus = "Closed";
        //                        userChk.Pro_Status = "Accepted";
        //                        context.SaveChanges();
        //                        tblItemDetail Items = new tblItemDetail();

        //                        var counter = obj.Customer_ServiceTypeId.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {

        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.Customer_Service[i] == "null")
        //                            {
        //                                item.Customer_Service = EmptyIfNull(obj.Customer_Service[i]);
        //                            }
        //                            else
        //                            {
        //                                item.Customer_Service = obj.Customer_Service[i];
        //                            }
        //                            item.Customer_ServiceTypeId = obj.Customer_ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }

        //                    }
        //                    else if (obj.ButtonType == "Cancel" && obj.Type == 1 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Cancelled";
        //                        userChk.In_R_Status = "Declined";
        //                        userChk.Pro_FlowStatus = "Cancelled";
        //                        userChk.Pro_Status = "Declined";
        //                        context.SaveChanges();

        //                    }
        //                    else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Closed";
        //                        userChk.In_R_Status = "Deleted";
        //                        userChk.Pro_FlowStatus = "Closed";
        //                        userChk.Pro_Status = "Deleted";
        //                        context.SaveChanges();
        //                    }
        //                    else if (obj.ButtonType == "Delete" && obj.Type == 2 && obj.SectionType == "Sent")
        //                    {
        //                        userChk.In_R_FlowStatus = "Closed";
        //                        userChk.In_R_Status = "Deleted";
        //                        userChk.Pro_FlowStatus = "Closed";
        //                        userChk.Pro_Status = "Deleted";
        //                        context.SaveChanges();
        //                    }
        //                    else if (obj.ButtonType == "Send" && obj.Type == 2 && obj.SectionType == "Sent")
        //                    {
        //                        if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                        {
        //                            userChk.DepositePayment = 0;
        //                        }
        //                        else
        //                        {
        //                            userChk.DepositePayment = obj.DepositePayment;
        //                        }

        //                        userChk.BalanceDue = obj.BalanceDue;
        //                        userChk.In_R_FlowStatus = "Sent";
        //                        userChk.In_R_Status = "Inprogress";
        //                        userChk.Pro_FlowStatus = "Pending Approval";
        //                        userChk.Pro_Status = "Inprogress";
        //                        context.SaveChanges();
        //                        tblItemDetail Items = new tblItemDetail();

        //                        var counter = obj.ServiceTypeId.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {

        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.ServiceType[i] == "null")
        //                            {
        //                                item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                            }
        //                            else
        //                            {
        //                                item.ServiceType = obj.ServiceType[i];
        //                            }
        //                            item.ServiceTypeId = obj.ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                    else if (obj.ButtonType == "Convert" && obj.Type == 2 && obj.SectionType == "Sent")
        //                    {
        //                        userChk.In_R_FlowStatus = "Pending Approval";
        //                        userChk.In_R_Status = "Converted";
        //                        userChk.Pro_FlowStatus = "Pending Approval";
        //                        userChk.Pro_Status = "Converted";
        //                        userChk.Type = 1;
        //                        if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                        {
        //                            userChk.DepositePayment = 0;
        //                        }
        //                        else
        //                        {
        //                            userChk.DepositePayment = obj.DepositePayment;
        //                        }

        //                        userChk.BalanceDue = obj.BalanceDue;
        //                        context.SaveChanges();
        //                        tblItemDetail Items = new tblItemDetail();

        //                        var counter = obj.ServiceTypeId.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {
        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.ServiceType[i] == "null")
        //                            {
        //                                item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                            }
        //                            else
        //                            {
        //                                item.ServiceType = obj.ServiceType[i];
        //                            }

        //                            // obj.ServiceType[i];
        //                            item.ServiceTypeId = obj.ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                    else if (obj.ButtonType == "Save" && obj.Type == 2 && obj.SectionType == "Sent")
        //                    {
        //                        userChk.In_R_FlowStatus = "Draft";
        //                        userChk.In_R_Status = "Inprogress";
        //                        userChk.Pro_FlowStatus = "Draft";
        //                        userChk.Pro_Status = "Inprogress";
        //                        if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                        {
        //                            userChk.DepositePayment = 0;
        //                        }
        //                        else
        //                        {
        //                            userChk.DepositePayment = obj.DepositePayment;
        //                        }

        //                        userChk.BalanceDue = obj.BalanceDue;
        //                        context.SaveChanges();

        //                        var counter = obj.ServiceTypeId.Count();
        //                        for (int i = 0; i < counter; i++)
        //                        {

        //                            int id = obj.ItemId[i];
        //                            var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                            if (obj.ServiceType[i] == null)
        //                            {
        //                                item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                            }
        //                            else
        //                            {
        //                                item.ServiceType = obj.ServiceType[i];
        //                            }
        //                            item.ServiceTypeId = obj.ServiceTypeId[i];
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                    else if (obj.ButtonType == "Approve" && obj.Type == 2 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Approved";
        //                        userChk.In_R_Status = "Pending Conversion";
        //                        userChk.Pro_FlowStatus = "Approved";
        //                        userChk.Pro_Status = "Pending Conversion";
        //                        context.SaveChanges();
        //                    }
        //                    else if (obj.ButtonType == "Reject" && obj.Type == 2 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Declined";
        //                        userChk.In_R_Status = "Rejected";
        //                        userChk.Pro_FlowStatus = "Declined";
        //                        userChk.Pro_Status = "Rejected";
        //                        context.SaveChanges();
        //                    }
        //                    else if (obj.ButtonType == "Decline" && obj.Type == 2 && obj.SectionType == "Received")
        //                    {
        //                        userChk.In_R_FlowStatus = "Cancelled";
        //                        userChk.In_R_Status = "Declined";
        //                        userChk.Pro_FlowStatus = "Cancelled";
        //                        userChk.Pro_Status = "Declined";
        //                        context.SaveChanges();
        //                    }
        //                    else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Sent")
        //                    {
        //                        userChk.In_R_FlowStatus = "Closed";
        //                        userChk.In_R_Status = "Deleted";
        //                        userChk.Pro_FlowStatus = "Closed";
        //                        userChk.Pro_Status = "Deleted";
        //                        context.SaveChanges();
        //                    }
        //                }
        //                dbContextTransaction.Commit();
        //                return InvoiceId = userChk.Id;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return InvoiceId;

        //}

        //public int UpdateInvoice(InvoiceDetailDto obj)
        //{
        //    int InvoiceId = 0;
        //    try
        //    {
        //        if (obj != null)
        //        {
        //            using (var repository = new InvoiceUserRegistrationRepository())
        //            {
        //                // int InvoiceId = repository.UpdateInvoice(obj);
        //                // var pdfPath = string.Empty;

        //                var context = new KFentities();
        //                using (var dbContextTransaction = context.Database.BeginTransaction())
        //                {
        //                    var userChk = context.tblInvoiceDetails.Where(i => i.Id == obj.Id).FirstOrDefault();
        //                    if (userChk != null)
        //                    {
        //                        if (obj.ButtonType == "Save" && obj.Type == 1 && obj.SectionType == "Sent")
        //                        {

        //                            if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                            {
        //                                userChk.DepositePayment = 0;
        //                            }
        //                            else
        //                            {
        //                                userChk.DepositePayment = obj.DepositePayment;
        //                            }
        //                            userChk.BalanceDue = obj.BalanceDue;
        //                            userChk.In_R_FlowStatus = "Draft";
        //                            userChk.In_R_Status = "Inprogress";
        //                            userChk.Pro_FlowStatus = "Draft";
        //                            userChk.Pro_Status = "Inprogress";
        //                            context.SaveChanges();

        //                            var counter = obj.ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {
        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.ServiceType[i] == "null")
        //                                {
        //                                    item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.ServiceType = obj.ServiceType[i];
        //                                }
        //                                //obj.ServiceType[i];
        //                                item.ServiceTypeId = obj.ServiceTypeId[i];
        //                                context.SaveChanges();
        //                            }
        //                            dbContextTransaction.Commit();

        //                            GeneratingReport genReport = new GeneratingReport();
        //                            pdfPath = genReport.PrintRun(obj.Id, 0);

        //                            InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
        //                            InvoiceEmail.SendEmailToInvoice(obj.Id, pdfPath);

        //                        }
        //                        else if (obj.ButtonType == "Send" && obj.Type == 1 && obj.SectionType == "Sent")
        //                        {
        //                            if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                            {
        //                                userChk.DepositePayment = 0;
        //                            }
        //                            else
        //                            {
        //                                userChk.DepositePayment = obj.DepositePayment;
        //                            }

        //                            userChk.BalanceDue = obj.BalanceDue;
        //                            userChk.In_R_FlowStatus = "Sent";
        //                            userChk.In_R_Status = "Inprogress";
        //                            userChk.Pro_FlowStatus = "Pending Approval";
        //                            userChk.Pro_Status = "Inprogress";
        //                            context.SaveChanges();
        //                            tblItemDetail Items = new tblItemDetail();

        //                            var counter = obj.ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {
        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.ServiceType[i] == "null")
        //                                {
        //                                    item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.ServiceType = obj.ServiceType[i];
        //                                }
        //                                item.ServiceTypeId = obj.ServiceTypeId[i];
        //                                context.SaveChanges();

        //                            }
        //                            dbContextTransaction.Commit();

        //                            GeneratingReport genReport = new GeneratingReport();
        //                            pdfPath = genReport.PrintRun(obj.Id, 0);

        //                            InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
        //                            InvoiceEmail.SendEmailToInvoice(obj.Id, pdfPath);
        //                        }
        //                        else if (obj.ButtonType == "Accept" && obj.Type == 1 && obj.SectionType == "Received")
        //                        {
        //                            #region  update finance tables for User

        //                            var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userChk.UserId && i.IsOnlyInvoice == false).Select(s => s.EmailTo).FirstOrDefault();
        //                            var ItemDetails = context.tblItemDetails.Where(x => x.InvoiceId == obj.Id).FirstOrDefault();

        //                            var itemdetails = (from invc in context.tblInvoiceDetails
        //                                               join item in context.tblItemDetails
        //                                              on invc.Id equals item.InvoiceId
        //                                               select new
        //                                               {
        //                                                   item.InvoiceId,
        //                                                   invc.CreatedDate

        //                                               }).ToList();
        //                            //  context.tblitemdetails.where(x => x.invoiceid == obj.id).firstordefault();

        //                            var FinanceUser = context.UserRegistrations.Where(i => i.Email == InvoiceEmail && i.IsUnlink == false).FirstOrDefault();

        //                            if (FinanceUser != null)
        //                            {
        //                                int JVID = 0001;

        //                                //var BankExpres = context.BankExpenses.Where(x => x.UserId == FinanceUser.Id).OrderByDescending(d => d.Id).FirstOrDefault();
        //                                var BankExpres = new BankExpense();
        //                                using (var repo = new ExpenseRepository())
        //                                {
        //                                    JVID = repo.GetJvid(FinanceUser.Id, Convert.ToInt16(itemdetails[0].CreatedDate.Value.Year));
        //                                }

        //                                //var JVID = context.BankExpenses.Where(x => x.UserId == FinanceUser.Id && (Convert.ToDateTime(x.Date).Year == DateTime.Now.Year)).Select(s => s.JVID).FirstOrDefault();
        //                                //JVID = JVID + 1;
        //                                //context.BankExpenses.Where(x => x.UserId == FinanceUserIdByInvoice && Convert.ToDateTime(x.CreatedDate).Year == DateTime.Now.Year).Select(s => s.JVID).FirstOrDefault(); 

        //                                decimal Total = Decimal.Multiply(Convert.ToDecimal(ItemDetails.Quantity), Convert.ToDecimal(ItemDetails.Rate));

        //                                BankExpres.Total = Total;
        //                                BankExpres.Credit = BankExpres.Total;
        //                                BankExpres.Debit = 0;
        //                                if (ItemDetails.GST_Tax.Contains("P_"))
        //                                {
        //                                    BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.GST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2)), 2);
        //                                    BankExpres.GSTtax = Math.Round((Convert.ToDecimal(BankExpres.GSTPercentage * BankExpres.Total) / 100), 2);
        //                                }
        //                                else
        //                                {
        //                                    BankExpres.GSTtax = Math.Round((Convert.ToDecimal(ItemDetails.GST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2))), 2);
        //                                    BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.GSTtax * 100) / BankExpres.Total), 2);
        //                                }

        //                                if (ItemDetails.HST_Tax.Contains("P_"))
        //                                {
        //                                    BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.HST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2)), 2);
        //                                    BankExpres.HSTtax = Math.Round(Convert.ToDecimal((BankExpres.HSTPercentage * BankExpres.Total) / 100), 2);
        //                                }
        //                                else
        //                                {
        //                                    BankExpres.HSTtax = Math.Round(Convert.ToDecimal(ItemDetails.HST_Tax.Substring(2, ItemDetails.HST_Tax.Length - 2)), 2);
        //                                    BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.HSTtax * 100) / BankExpres.Total), 2);
        //                                }

        //                                if (ItemDetails.PST_Tax.Contains("P_"))
        //                                {
        //                                    BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.PST_Tax.Substring(2, ItemDetails.PST_Tax.Length - 2)), 2);
        //                                    BankExpres.PSTtax = Math.Round(Convert.ToDecimal((BankExpres.PSTPercentage * BankExpres.Total) / 100), 2);
        //                                }
        //                                else
        //                                {
        //                                    BankExpres.PSTtax = Math.Round(Convert.ToDecimal(ItemDetails.PST_Tax.Substring(2, ItemDetails.PST_Tax.Length - 2)), 2);
        //                                    BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.PSTtax * 100) / BankExpres.Total), 2);
        //                                }

        //                                if (ItemDetails.QST_Tax.Contains("P_"))
        //                                {
        //                                    BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.QST_Tax.Substring(2, ItemDetails.QST_Tax.Length - 2)), 2);
        //                                    BankExpres.QSTtax = Math.Round(Convert.ToDecimal((BankExpres.QSTPercentage * BankExpres.Total)) / 100);
        //                                }
        //                                else
        //                                {
        //                                    BankExpres.QSTtax = Math.Round(Convert.ToDecimal(ItemDetails.QST_Tax.Substring(2, ItemDetails.QST_Tax.Length - 2)), 2);
        //                                    BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.QSTtax * 100) / BankExpres.Total), 2);
        //                                }


        //                                decimal TotalTaxAmount = Convert.ToDecimal(BankExpres.QSTtax + BankExpres.PSTtax + BankExpres.HSTtax + BankExpres.GSTtax);
        //                                BankExpres.UserId = FinanceUser.Id;
        //                                BankExpres.BankId = 8;
        //                                BankExpres.Description = ItemDetails.ServiceType;
        //                                BankExpres.ClassificationId = context.Classifications.Where(i => i.Name == ItemDetails.ServiceType).Select(s => s.Id).FirstOrDefault();
        //                                BankExpres.AccountType = "INV";
        //                                BankExpres.Total = ItemDetails.Quantity * ItemDetails.Rate;
        //                                BankExpres.CreatedDate = DateTime.Now;
        //                                BankExpres.StatusId = 4;
        //                                BankExpres.AccountClassificationId = 1030;
        //                                BankExpres.Date = DateTime.Now;
        //                                BankExpres.ActualTotal = TotalTaxAmount + BankExpres.Total;
        //                                BankExpres.TotalTax = TotalTaxAmount;
        //                                BankExpres.ActualPercentage = Convert.ToDouble((BankExpres.ActualTotal * 100) / BankExpres.Total);  //TotalTaxAmount;

        //                                BankExpres.JVID = JVID; //Convert.ToInt32(userChk.InvoiceJVID); // get latest jvid based on userid and year      may no need to save while creating invoice
        //                                BankExpres.UploadType = "M";
        //                                context.BankExpenses.Add(BankExpres);
        //                                context.SaveChanges();

        //                                //For Discount
        //                                var InvoiceDetail = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();
        //                                BankExpres.Credit = TotalTaxAmount;
        //                                BankExpres.UserId = FinanceUser.Id;
        //                                BankExpres.BankId = 8;
        //                                BankExpres.Description = "Discount Allowed";
        //                                BankExpres.ClassificationId = 966;
        //                                BankExpres.Debit = 0;
        //                                BankExpres.Total = TotalTaxAmount;
        //                                BankExpres.ActualTotal = TotalTaxAmount;
        //                                BankExpres.JVID = JVID;
        //                                BankExpres.StatusId = 4;
        //                                BankExpres.AccountType = "INV";
        //                                BankExpres.AccountClassificationId = 1030;
        //                                BankExpres.Date = DateTime.Now;
        //                                BankExpres.ActualPercentage = Convert.ToDouble((BankExpres.ActualTotal * 100) / BankExpres.Total);

        //                                //Cash Advanced Payment
        //                                BankExpres.Debit = InvoiceDetail.DepositePayment;
        //                                BankExpres.UserId = FinanceUser.Id;
        //                                BankExpres.BankId = 8;
        //                                BankExpres.Description = "Unbanked Cash";
        //                                BankExpres.ClassificationId = 838;
        //                                BankExpres.Credit = InvoiceDetail.BalanceDue;
        //                                BankExpres.StatusId = 4;
        //                                BankExpres.AccountType = "INV";
        //                                BankExpres.AccountClassificationId = 1030;
        //                                BankExpres.Date = DateTime.Now;
        //                                BankExpres.ActualPercentage = Convert.ToDouble((BankExpres.ActualTotal * 100) / BankExpres.Total);
        //                                context.BankExpenses.Add(BankExpres);
        //                                context.SaveChanges();

        //                                //Description/Creditor           //save data in classificatin table

        //                                var SuppliersEmail = context.tblCustomerOrSuppliers.Where(x => x.UserId == userChk.UserId && x.RoleId == 1).Select(s => s.Email).FirstOrDefault();
        //                                if (SuppliersEmail != null)
        //                                {
        //                                    var SupplierId = context.InvoiceUserRegistrations.Where(x => x.EmailTo == SuppliersEmail).Select(s => s.Id).FirstOrDefault();
        //                                    var SupplierDetails = context.tblCustomerOrSuppliers.Where(x => x.UserId == SupplierId && x.RoleId == 1).FirstOrDefault();

        //                                    var ClassificationData = new Classification();
        //                                    //ClassificationData.CategoryId = 
        //                                    ClassificationData.ChartAccountDisplayNumber = SupplierDetails.Credetor;
        //                                    ClassificationData.ChartAccountNumber = Convert.ToInt32(SupplierDetails.Credetor.Trim('-'));
        //                                    ClassificationData.ClassificationType = SupplierDetails.Company_Name;
        //                                    ClassificationData.Desc = SupplierDetails.Company_Name;
        //                                    ClassificationData.Name = SupplierDetails.Company_Name;
        //                                    //ClassificationData.Type =  
        //                                    ClassificationData.IsDeleted = false;
        //                                    context.Classifications.Add(ClassificationData);
        //                                    context.SaveChanges();


        //                                    BankExpres.Description = SupplierDetails.Company_Name;
        //                                    BankExpres.Debit = 0;
        //                                    BankExpres.UserId = FinanceUser.Id;
        //                                    BankExpres.BankId = 8;
        //                                    BankExpres.Credit = 0;
        //                                    BankExpres.StatusId = 4;
        //                                    BankExpres.ClassificationId = ClassificationData.Id;
        //                                    BankExpres.AccountType = "INV";
        //                                    BankExpres.AccountClassificationId = 1030;
        //                                    BankExpres.Date = DateTime.Now;
        //                                    context.BankExpenses.Add(BankExpres);
        //                                    context.SaveChanges();
        //                                }
        //                                dbContextTransaction.Commit();
        //                            }

        //                            #endregion


        //                            #region  update finance tables for Customer


        //                            var CustomerEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userChk.CustomerId && i.IsOnlyInvoice == false).Select(s => s.EmailTo).FirstOrDefault();
        //                            if (CustomerEmail != null)
        //                            {
        //                                var ItemDetailsForCustomer = context.tblItemDetails.Where(x => x.InvoiceId == obj.Id).FirstOrDefault();

        //                                var itemdetails1 = (from invc in context.tblInvoiceDetails
        //                                                    join item in context.tblItemDetails
        //                                                   on invc.Id equals item.InvoiceId
        //                                                    select new
        //                                                    {
        //                                                        item.InvoiceId,
        //                                                        invc.CreatedDate

        //                                                    }).ToList();

        //                                var Customer_FinanceUser = context.UserRegistrations.Where(i => i.Email == InvoiceEmail && i.IsUnlink == false).FirstOrDefault();
        //                                if (Customer_FinanceUser != null)
        //                                {
        //                                    int JVID = 0001;

        //                                    //var BankExpres = context.BankExpenses.Where(x => x.UserId == FinanceUser.Id).OrderByDescending(d => d.Id).FirstOrDefault();
        //                                    var BankExpres = new BankExpense();
        //                                    using (var repo = new ExpenseRepository())
        //                                    {
        //                                        JVID = repo.GetJvid(FinanceUser.Id, Convert.ToInt16(itemdetails[0].CreatedDate.Value.Year));
        //                                    }
        //                                    //  var BankExpres = context.BankExpenses.Where(x => x.UserId == Customer_FinanceUser.Id).FirstOrDefault();
        //                                    //var JVID = context.BankExpenses.Where(x => x.UserId == Customer_FinanceUser.Id && (Convert.ToDateTime(x.CreatedDate).Year == DateTime.Now.Year)).Select(s => s.JVID).FirstOrDefault();
        //                                    //JVID = JVID + 1;
        //                                    if (BankExpres != null)
        //                                    {
        //                                        BankExpres.Total = ItemDetails.Quantity * ItemDetails.Rate;
        //                                        BankExpres.Credit = 0;
        //                                        BankExpres.Debit = BankExpres.Total;

        //                                        if (ItemDetails.GST_Tax.Contains("P_"))
        //                                        {
        //                                            BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.GST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2)), 2);
        //                                            BankExpres.GSTtax = Math.Round((Convert.ToDecimal(BankExpres.GSTPercentage * BankExpres.Total) / 100), 2);
        //                                        }
        //                                        else
        //                                        {
        //                                            BankExpres.GSTtax = Math.Round((Convert.ToDecimal(ItemDetails.GST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2))), 2);
        //                                            BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.GSTtax * 100) / BankExpres.Total), 2);
        //                                        }

        //                                        if (ItemDetails.HST_Tax.Contains("P_"))
        //                                        {
        //                                            BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.HST_Tax.Substring(2, ItemDetails.GST_Tax.Length - 2)), 2);
        //                                            BankExpres.HSTtax = Math.Round(Convert.ToDecimal((BankExpres.HSTPercentage * BankExpres.Total) / 100), 2);
        //                                        }
        //                                        else
        //                                        {
        //                                            BankExpres.HSTtax = Math.Round(Convert.ToDecimal(ItemDetails.HST_Tax.Substring(2, ItemDetails.HST_Tax.Length - 2)), 2);
        //                                            BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.HSTtax * 100) / BankExpres.Total), 2);
        //                                        }

        //                                        if (ItemDetails.PST_Tax.Contains("P_"))
        //                                        {
        //                                            BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.PST_Tax.Substring(2, ItemDetails.PST_Tax.Length - 2)), 2);
        //                                            BankExpres.PSTtax = Math.Round(Convert.ToDecimal((BankExpres.PSTPercentage * BankExpres.Total) / 100), 2);
        //                                        }
        //                                        else
        //                                        {
        //                                            BankExpres.PSTtax = Math.Round(Convert.ToDecimal(ItemDetails.PST_Tax.Substring(2, ItemDetails.PST_Tax.Length - 2)), 2);
        //                                            BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.PSTtax * 100) / BankExpres.Total), 2);
        //                                        }

        //                                        if (ItemDetails.QST_Tax.Contains("P_"))
        //                                        {
        //                                            BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal(ItemDetails.QST_Tax.Substring(2, ItemDetails.QST_Tax.Length - 2)), 2);
        //                                            BankExpres.QSTtax = Math.Round(Convert.ToDecimal((BankExpres.QSTPercentage * BankExpres.Total)) / 100);
        //                                        }
        //                                        else
        //                                        {
        //                                            BankExpres.QSTtax = Math.Round(Convert.ToDecimal(ItemDetails.QST_Tax.Substring(2, ItemDetails.QST_Tax.Length - 2)), 2);
        //                                            BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.QSTtax * 100) / BankExpres.Total), 2);
        //                                        }


        //                                        decimal TotalTaxAmount = Convert.ToDecimal(BankExpres.QSTtax + BankExpres.PSTtax + BankExpres.HSTtax + BankExpres.GSTtax);
        //                                        BankExpres.UserId = FinanceUser.Id;
        //                                        BankExpres.BankId = 8;
        //                                        BankExpres.Description = ItemDetails.Customer_Service;
        //                                        BankExpres.AccountType = "INV";
        //                                        BankExpres.AccountClassificationId = 1030;
        //                                        BankExpres.ClassificationId = context.Classifications.Where(i => i.Name == ItemDetails.Customer_Service).Select(s => s.Id).FirstOrDefault();
        //                                        BankExpres.Total = ItemDetails.Quantity * ItemDetails.Rate;
        //                                        BankExpres.CreatedDate = DateTime.Now;
        //                                        BankExpres.StatusId = 4;
        //                                        BankExpres.Date = DateTime.Now;
        //                                        BankExpres.ActualTotal = TotalTaxAmount + BankExpres.Total;
        //                                        BankExpres.TotalTax = TotalTaxAmount;
        //                                        BankExpres.JVID = Convert.ToInt32(userChk.InvoiceJVID);
        //                                        BankExpres.UploadType = "M";
        //                                        BankExpres.ActualPercentage = Convert.ToDouble((BankExpres.ActualTotal * 100) / BankExpres.Total);
        //                                        context.BankExpenses.Add(BankExpres);
        //                                        context.SaveChanges();



        //                                        //For Discount
        //                                        var InvoiceDetail = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();
        //                                        BankExpres.Credit = 0;
        //                                        BankExpres.UserId = FinanceUser.Id;
        //                                        BankExpres.BankId = 8;
        //                                        BankExpres.Description = "Discount Allowed";
        //                                        BankExpres.ClassificationId = 966;
        //                                        BankExpres.Debit = TotalTaxAmount;
        //                                        BankExpres.Total = TotalTaxAmount;
        //                                        BankExpres.ActualTotal = TotalTaxAmount;
        //                                        BankExpres.JVID = Convert.ToInt32(userChk.InvoiceJVID);
        //                                        BankExpres.StatusId = 4;
        //                                        BankExpres.AccountType = "INV";
        //                                        BankExpres.ActualPercentage = Convert.ToDouble((BankExpres.ActualTotal * 100) / BankExpres.Total);
        //                                        BankExpres.AccountClassificationId = 1030;
        //                                        BankExpres.Date = DateTime.Now;


        //                                        //Cash Advanced Payment
        //                                        BankExpres.Debit = InvoiceDetail.DepositePayment;
        //                                        BankExpres.UserId = FinanceUser.Id;
        //                                        BankExpres.BankId = 8;
        //                                        BankExpres.ClassificationId = 838;
        //                                        BankExpres.Description = "Unbanked Cash";
        //                                        BankExpres.Credit = InvoiceDetail.BalanceDue;
        //                                        BankExpres.StatusId = 4;
        //                                        BankExpres.AccountClassificationId = 1030;
        //                                        BankExpres.AccountType = "INV";
        //                                        BankExpres.Date = DateTime.Now;


        //                                        //Description/Debitor
        //                                        var CustomerDetails = context.tblCustomerOrSuppliers.Where(x => x.Id == userChk.CustomerId).FirstOrDefault();

        //                                        var ClassificationData = new Classification();
        //                                        //ClassificationData.CategoryId = 
        //                                        ClassificationData.ChartAccountDisplayNumber = CustomerDetails.Debtors;
        //                                        ClassificationData.ChartAccountNumber = Convert.ToInt32(CustomerDetails.Credetor.Trim('-'));
        //                                        ClassificationData.ClassificationType = CustomerDetails.Company_Name;
        //                                        ClassificationData.Desc = CustomerDetails.Company_Name;
        //                                        ClassificationData.Name = CustomerDetails.Company_Name;
        //                                        ClassificationData.Type = "A";
        //                                        ClassificationData.IsDeleted = false;
        //                                        context.Classifications.Add(ClassificationData);
        //                                        context.SaveChanges();

        //                                        BankExpres.ClassificationId = ClassificationData.Id;
        //                                        BankExpres.Description = CustomerDetails.Company_Name;
        //                                        BankExpres.Debit = 0;
        //                                        BankExpres.UserId = FinanceUser.Id;
        //                                        BankExpres.BankId = 8;
        //                                        BankExpres.Credit = 0;
        //                                        BankExpres.StatusId = 4;
        //                                        BankExpres.AccountType = "INV";
        //                                        BankExpres.AccountClassificationId = 1030;
        //                                        BankExpres.Date = DateTime.Now;
        //                                        context.BankExpenses.Add(BankExpres);
        //                                        context.SaveChanges();
        //                                    }

        //                                }
        //                                dbContextTransaction.Commit();
        //                            }

        //                            #endregion

        //                            userChk.In_R_FlowStatus = "Closed";
        //                            userChk.In_R_Status = "Accepted";
        //                            userChk.Pro_FlowStatus = "Closed";
        //                            userChk.Pro_Status = "Accepted";
        //                            context.SaveChanges();
        //                            tblItemDetail Items = new tblItemDetail();

        //                            var counter = obj.Customer_ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {

        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.Customer_Service[i] == "null")
        //                                {
        //                                    item.Customer_Service = EmptyIfNull(obj.Customer_Service[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.Customer_Service = obj.Customer_Service[i];
        //                                }
        //                                item.Customer_ServiceTypeId = obj.Customer_ServiceTypeId[i];
        //                                context.SaveChanges();
        //                            }

        //                        }
        //                        else if (obj.ButtonType == "Cancel" && obj.Type == 1 && obj.SectionType == "Received")
        //                        {
        //                            userChk.In_R_FlowStatus = "Cancelled";
        //                            userChk.In_R_Status = "Declined";
        //                            userChk.Pro_FlowStatus = "Cancelled";
        //                            userChk.Pro_Status = "Declined";
        //                            context.SaveChanges();

        //                        }
        //                        else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Received")
        //                        {
        //                            userChk.In_R_FlowStatus = "Closed";
        //                            userChk.In_R_Status = "Deleted";
        //                            userChk.Pro_FlowStatus = "Closed";
        //                            userChk.Pro_Status = "Deleted";
        //                            context.SaveChanges();
        //                        }
        //                        else if (obj.ButtonType == "Delete" && obj.Type == 2 && obj.SectionType == "Sent")
        //                        {
        //                            userChk.In_R_FlowStatus = "Closed";
        //                            userChk.In_R_Status = "Deleted";
        //                            userChk.Pro_FlowStatus = "Closed";
        //                            userChk.Pro_Status = "Deleted";
        //                            context.SaveChanges();
        //                        }
        //                        else if (obj.ButtonType == "Send" && obj.Type == 2 && obj.SectionType == "Sent")
        //                        {
        //                            if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                            {
        //                                userChk.DepositePayment = 0;
        //                            }
        //                            else
        //                            {
        //                                userChk.DepositePayment = obj.DepositePayment;
        //                            }

        //                            userChk.BalanceDue = obj.BalanceDue;
        //                            userChk.In_R_FlowStatus = "Sent";
        //                            userChk.In_R_Status = "Inprogress";
        //                            userChk.Pro_FlowStatus = "Pending Approval";
        //                            userChk.Pro_Status = "Inprogress";
        //                            context.SaveChanges();
        //                            tblItemDetail Items = new tblItemDetail();

        //                            var counter = obj.ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {

        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.ServiceType[i] == "null")
        //                                {
        //                                    item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.ServiceType = obj.ServiceType[i];
        //                                }
        //                                item.ServiceTypeId = obj.ServiceTypeId[i];
        //                                context.SaveChanges();
        //                            }
        //                        }
        //                        else if (obj.ButtonType == "Convert" && obj.Type == 2 && obj.SectionType == "Sent")
        //                        {
        //                            userChk.In_R_FlowStatus = "Pending Approval";
        //                            userChk.In_R_Status = "Converted";
        //                            userChk.Pro_FlowStatus = "Pending Approval";
        //                            userChk.Pro_Status = "Converted";
        //                            userChk.Type = 1;
        //                            if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                            {
        //                                userChk.DepositePayment = 0;
        //                            }
        //                            else
        //                            {
        //                                userChk.DepositePayment = obj.DepositePayment;
        //                            }

        //                            userChk.BalanceDue = obj.BalanceDue;
        //                            context.SaveChanges();
        //                            tblItemDetail Items = new tblItemDetail();

        //                            var counter = obj.ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {
        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.ServiceType[i] == "null")
        //                                {
        //                                    item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.ServiceType = obj.ServiceType[i];
        //                                }

        //                                // obj.ServiceType[i];
        //                                item.ServiceTypeId = obj.ServiceTypeId[i];
        //                                context.SaveChanges();
        //                            }
        //                        }
        //                        else if (obj.ButtonType == "Save" && obj.Type == 2 && obj.SectionType == "Sent")
        //                        {
        //                            userChk.In_R_FlowStatus = "Draft";
        //                            userChk.In_R_Status = "Inprogress";
        //                            userChk.Pro_FlowStatus = "Draft";
        //                            userChk.Pro_Status = "Inprogress";
        //                            if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
        //                            {
        //                                userChk.DepositePayment = 0;
        //                            }
        //                            else
        //                            {
        //                                userChk.DepositePayment = obj.DepositePayment;
        //                            }

        //                            userChk.BalanceDue = obj.BalanceDue;
        //                            context.SaveChanges();

        //                            var counter = obj.ServiceTypeId.Count();
        //                            for (int i = 0; i < counter; i++)
        //                            {

        //                                int id = obj.ItemId[i];
        //                                var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
        //                                if (obj.ServiceType[i] == null)
        //                                {
        //                                    item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
        //                                }
        //                                else
        //                                {
        //                                    item.ServiceType = obj.ServiceType[i];
        //                                }
        //                                item.ServiceTypeId = obj.ServiceTypeId[i];
        //                                context.SaveChanges();
        //                            }
        //                            dbContextTransaction.Commit();
        //                        }
        //                        else if (obj.ButtonType == "Approve" && obj.Type == 2 && obj.SectionType == "Received")
        //                        {
        //                            userChk.In_R_FlowStatus = "Approved";
        //                            userChk.In_R_Status = "Pending Conversion";
        //                            userChk.Pro_FlowStatus = "Approved";
        //                            userChk.Pro_Status = "Pending Conversion";
        //                            context.SaveChanges();
        //                            dbContextTransaction.Commit();
        //                        }
        //                        else if (obj.ButtonType == "Reject" && obj.Type == 2 && obj.SectionType == "Received")
        //                        {
        //                            userChk.In_R_FlowStatus = "Declined";
        //                            userChk.In_R_Status = "Rejected";
        //                            userChk.Pro_FlowStatus = "Declined";
        //                            userChk.Pro_Status = "Rejected";
        //                            context.SaveChanges();
        //                            dbContextTransaction.Commit();
        //                        }
        //                        else if (obj.ButtonType == "Decline" && obj.Type == 2 && obj.SectionType == "Received")
        //                        {
        //                            userChk.In_R_FlowStatus = "Cancelled";
        //                            userChk.In_R_Status = "Declined";
        //                            userChk.Pro_FlowStatus = "Cancelled";
        //                            userChk.Pro_Status = "Declined";
        //                            context.SaveChanges();
        //                            dbContextTransaction.Commit();
        //                        }
        //                        else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Sent")
        //                        {
        //                            userChk.In_R_FlowStatus = "Closed";
        //                            userChk.In_R_Status = "Deleted";
        //                            userChk.Pro_FlowStatus = "Closed";
        //                            userChk.Pro_Status = "Deleted";
        //                            context.SaveChanges();
        //                            dbContextTransaction.Commit();
        //                        }
        //                    }
        //                    return InvoiceId = userChk.Id;
        //                    // if (userChk.Id > 0)
        //                    // {
        //                    // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = userChk.Id };
        //                    // }
        //                    // else
        //                    // {
        //                    // return new ApiResponseDto { ResponseMessage = "Data is wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
        //                    // }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //return new ApiResponseDto { ResponseMessage = "Please provide data 2.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return InvoiceId;
        //}



        public int UpdateInvoice(InvoiceDetailDto obj)
        {
            int InvoiceId = 0;
            try
            {
                if (obj != null)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        // int InvoiceId = repository.UpdateInvoice(obj);
                        // var pdfPath = string.Empty;

                        var context = new KFentities();
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            var userChk = context.tblInvoiceDetails.Where(i => i.Id == obj.Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                if (obj.ButtonType == "Save" && obj.Type == 1 && obj.SectionType == "Sent")
                                {

                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userChk.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userChk.DepositePayment = obj.DepositePayment;
                                    }
                                    userChk.BalanceDue = obj.BalanceDue;
                                    userChk.In_R_FlowStatus = "Draft";
                                    userChk.In_R_Status = "Inprogress";
                                    userChk.Pro_FlowStatus = "Draft";
                                    userChk.Pro_Status = "Inprogress";
                                    context.SaveChanges();

                                    var counter = obj.ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.ServiceType[i] == "null")
                                        {
                                            item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            item.ServiceType = obj.ServiceType[i];
                                        }
                                        //obj.ServiceType[i];
                                        item.ServiceTypeId = obj.ServiceTypeId[i];
                                        context.SaveChanges();
                                    }
                                    dbContextTransaction.Commit();

                                    GeneratingReport genReport = new GeneratingReport();
                                    pdfPath = genReport.PrintRun(obj.Id, 0);

                                    InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
                                    InvoiceEmail.SendEmailToInvoice(obj.Id, pdfPath);

                                }
                                else if (obj.ButtonType == "Send" && obj.Type == 1 && obj.SectionType == "Sent")
                                {
                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userChk.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userChk.DepositePayment = obj.DepositePayment;
                                    }

                                    userChk.BalanceDue = obj.BalanceDue;
                                    userChk.In_R_FlowStatus = "Sent";
                                    userChk.In_R_Status = "Inprogress";
                                    userChk.Pro_FlowStatus = "Pending Approval";
                                    userChk.Pro_Status = "Inprogress";
                                    context.SaveChanges();
                                    tblItemDetail Items = new tblItemDetail();

                                    var counter = obj.ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.ServiceType[i] == "null")
                                        {
                                            item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            item.ServiceType = obj.ServiceType[i];
                                        }
                                        item.ServiceTypeId = obj.ServiceTypeId[i];
                                        context.SaveChanges();

                                    }
                                    dbContextTransaction.Commit();

                                    GeneratingReport genReport = new GeneratingReport();
                                    pdfPath = genReport.PrintRun(obj.Id, 0);

                                    InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
                                    InvoiceEmail.SendEmailToInvoice(obj.Id, pdfPath);
                                }
                                else if (obj.ButtonType == "Accept" && obj.Type == 1 && obj.SectionType == "Received")
                                {
                                    decimal UsersDiscountTotal = 0;
                                    decimal UserFirstActualTotal = 0;
                                    decimal UserDiscountActualTotal = 0;
                                    decimal CustomerFirstActualTotal = 0;
                                    decimal CustomerDiscountActualTotal = 0;
                                    decimal discount = 0;
                                    int Classifications_Id = 0;

                                    #region  update finance tables for User
                                    var InvoiceEmail = context.InvoiceUserRegistrations.Where(i => i.Id == userChk.UserId && i.IsOnlyInvoice == false).Select(s => s.EmailTo).FirstOrDefault();
                                    var itemdetails = (from invc in context.tblInvoiceDetails
                                                       join item in context.tblItemDetails
                                                      on invc.Id equals item.InvoiceId
                                                       where item.InvoiceId == obj.Id
                                                       select new
                                                       {
                                                           invc.CreatedDate,
                                                           invc.InvoiceNumber,
                                                           invc.InvoiceDate,
                                                           item.InvoiceId,
                                                           item.Quantity,
                                                           item.Rate,
                                                           item.GST_Tax,
                                                           item.HST_Tax,
                                                           item.PST_Tax,
                                                           item.QST_Tax,
                                                           item.ServiceType,
                                                           item.SubTotal,
                                                           item.Description,
                                                           item.Discount,
                                                           item.Customer_Service,
                                                           item.Amount,
                                                           item.Item
                                                       }).ToList();

                                    var FinanceUser = context.UserRegistrations.Where(i => i.Email == InvoiceEmail && i.IsUnlink == false).FirstOrDefault();
                                    var CustomerEmail1 = context.tblCustomerOrSuppliers.Where(i => i.Id == userChk.CustomerId && i.IsFinance == true).Select(s => s.Email).FirstOrDefault();
                                    if (CustomerEmail1 != null)
                                    {
                                        var Customer_FinanceID = context.UserRegistrations.Where(i => i.Email == CustomerEmail1 && i.IsUnlink == false).Select(s => s.Id).FirstOrDefault();
                                        var date = context.tblInvoiceDetails.Where(i => i.Id == obj.Id).Select(s => s.CreatedDate).FirstOrDefault();

                                        using (var repo = new ExpenseRepository())
                                        {
                                            CustomerJvid = repo.GetJvid(Customer_FinanceID, Convert.ToInt16(date.Value.Year));
                                        }
                                    }

                                    if (FinanceUser != null)
                                    {
                                        int JVID = 0001;

                                        using (var repo = new ExpenseRepository())
                                        {
                                            JVID = repo.GetJvid(FinanceUser.Id, Convert.ToInt16(itemdetails[0].CreatedDate.Value.Year));
                                        }

                                        foreach (var itmd in itemdetails)
                                        {
                                            var BankExpres = new BankExpense();
                                            decimal Total = Decimal.Multiply(Convert.ToDecimal(itmd.Quantity), Convert.ToDecimal(itmd.Rate));
                                            BankExpres.Total = Total;
                                            BankExpres.Credit = Total;
                                            BankExpres.Debit = 0;
                                            BankExpres.IsDeleted = false;
                                            BankExpres.IsVirtualEntry = false;

                                            if (!string.IsNullOrEmpty(itmd.Discount))
                                            {
                                                if (itmd.Discount.Contains("P_"))
                                                {
                                                    discount = Math.Round(Convert.ToDecimal(itmd.Discount.Substring(2, itmd.Discount.Length - 2)) / 100 * Total, 2);
                                                }
                                                else
                                                {
                                                    discount = Math.Round(Convert.ToDecimal(itmd.Discount.Substring(2, itmd.Discount.Length - 2)), 2);
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(itmd.GST_Tax) && itmd.GST_Tax != "0")
                                            {
                                                if (itmd.GST_Tax.Contains("P_"))
                                                {
                                                    BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal(itmd.GST_Tax.Substring(2, itmd.GST_Tax.Length - 2)), 2);
                                                    BankExpres.GSTtax = Math.Round((Convert.ToDecimal(BankExpres.GSTPercentage * (BankExpres.Total - discount)) / 100), 2);
                                                }
                                                else
                                                {
                                                    BankExpres.GSTtax = Math.Round((Convert.ToDecimal(itmd.GST_Tax.Substring(2, itmd.GST_Tax.Length - 2))), 2);
                                                    BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.GSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                }
                                            }
                                            else
                                            {
                                                BankExpres.GSTtax = 0;
                                                BankExpres.GSTPercentage = 0;
                                            }


                                            if (!string.IsNullOrEmpty(itmd.HST_Tax) && itmd.HST_Tax != "0")
                                            {
                                                if (itmd.HST_Tax.Contains("P_"))
                                                {
                                                    BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal(itmd.HST_Tax.Substring(2, itmd.HST_Tax.Length - 2)), 2);
                                                    BankExpres.HSTtax = Math.Round(Convert.ToDecimal((BankExpres.HSTPercentage * (BankExpres.Total - discount)) / 100), 2);
                                                }
                                                else
                                                {
                                                    BankExpres.HSTtax = Math.Round(Convert.ToDecimal(itmd.HST_Tax.Substring(2, itmd.HST_Tax.Length - 2)), 2);
                                                    BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.HSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                }
                                            }
                                            else
                                            {
                                                BankExpres.HSTPercentage = 0;
                                                BankExpres.HSTtax = 0;

                                            }


                                            if (!string.IsNullOrEmpty(itmd.PST_Tax) && itmd.PST_Tax != "0")
                                            {

                                                if (itmd.PST_Tax.Contains("P_"))
                                                {
                                                    BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal(itmd.PST_Tax.Substring(2, itmd.PST_Tax.Length - 2)), 2);
                                                    BankExpres.PSTtax = Math.Round(Convert.ToDecimal((BankExpres.PSTPercentage * (BankExpres.Total - discount)) / 100), 2);
                                                }
                                                else
                                                {
                                                    BankExpres.PSTtax = Math.Round(Convert.ToDecimal(itmd.PST_Tax.Substring(2, itmd.PST_Tax.Length - 2)), 2);
                                                    BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.PSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                }
                                            }
                                            else
                                            {
                                                BankExpres.PSTPercentage = 0;
                                                BankExpres.PSTtax = 0;
                                            }



                                            if (!string.IsNullOrEmpty(itmd.QST_Tax) && itmd.QST_Tax != "0")
                                            {
                                                if (itmd.QST_Tax.Contains("P_"))
                                                {
                                                    BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal(itmd.QST_Tax.Substring(2, itmd.QST_Tax.Length - 2)), 2);
                                                    BankExpres.QSTtax = Math.Round(Convert.ToDecimal((BankExpres.QSTPercentage * (BankExpres.Total - discount))) / 100, 2);
                                                }
                                                else
                                                {
                                                    BankExpres.QSTtax = Math.Round(Convert.ToDecimal(itmd.QST_Tax.Substring(2, itmd.QST_Tax.Length - 2)), 2);
                                                    BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.QSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                }
                                            }
                                            else
                                            {
                                                BankExpres.QSTtax = 0;
                                                BankExpres.QSTPercentage = 0;
                                            }

                                            decimal TotalTaxAmount = Convert.ToDecimal(BankExpres.QSTtax + BankExpres.PSTtax + BankExpres.HSTtax + BankExpres.GSTtax);
                                            BankExpres.UserId = FinanceUser.Id;
                                            BankExpres.BankId = 8;
                                            BankExpres.Description = "INV#" + itmd.InvoiceNumber.PadLeft(5, '0') + " - " + itmd.Description;       //INV invoicenumber #  invoicenumber=5 digits

                                            if (!string.IsNullOrEmpty(itmd.ServiceType))
                                            {
                                                BankExpres.ClassificationId = context.Classifications.Where(i => i.Name == itmd.ServiceType).Select(s => s.Id).FirstOrDefault();
                                            }
                                            else
                                                BankExpres.ClassificationId = 1;
                                            BankExpres.AccountType = "INV";
                                            BankExpres.CreatedDate = DateTime.Now;
                                            BankExpres.StatusId = 4;
                                            BankExpres.AccountClassificationId = 1030;
                                            BankExpres.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                            decimal Subtract = TotalTaxAmount + discount;

                                            BankExpres.ActualTotal = Total - Subtract;
                                            UserFirstActualTotal = Convert.ToDecimal(BankExpres.ActualTotal);
                                            BankExpres.TotalTax = TotalTaxAmount;
                                            BankExpres.JVID = JVID;
                                            BankExpres.UploadType = "M";

                                            if (BankExpres.ActualTotal != 0 && Total != 0)
                                            {
                                                context.BankExpenses.Add(BankExpres);
                                                context.SaveChanges();
                                            }
                                            //For Discount  Secound entry

                                            //if Discount != null or 0 than save
                                            decimal Discounttotal = 0;
                                            if (!string.IsNullOrEmpty(itmd.Discount))
                                            {
                                                if (itmd.Discount.Contains("P_"))
                                                {
                                                    Discounttotal = (Convert.ToDecimal(itmd.Discount.Substring(2, itmd.Discount.Length - 2)) / 100) * Total;   //(ItemDetails.Discount / 100) ;
                                                }
                                                else
                                                {
                                                    Discounttotal = Convert.ToDecimal(itmd.Discount.Substring(2, itmd.Discount.Length - 2));
                                                }

                                                if (Discounttotal != 0)
                                                {
                                                    BankExpres.Credit = 0;
                                                    BankExpres.UserId = FinanceUser.Id;
                                                    BankExpres.BankId = 8;
                                                    BankExpres.Total = Discounttotal;
                                                    BankExpres.IsDeleted = false;
                                                    BankExpres.IsVirtualEntry = false;
                                                    BankExpres.Description = "INV#" + itmd.InvoiceNumber.PadLeft(5, '0');
                                                    BankExpres.ClassificationId = 966;
                                                    BankExpres.GSTPercentage = 0;
                                                    BankExpres.HSTPercentage = 0;
                                                    BankExpres.QSTPercentage = 0;
                                                    BankExpres.PSTPercentage = 0;
                                                    BankExpres.PSTtax = 0;
                                                    BankExpres.HSTtax = 0;
                                                    BankExpres.QSTtax = 0;
                                                    BankExpres.GSTtax = 0;
                                                    UsersDiscountTotal = Convert.ToDecimal(BankExpres.Total);
                                                    BankExpres.Debit = BankExpres.Total;
                                                    BankExpres.ActualTotal = BankExpres.Total;
                                                    BankExpres.JVID = JVID;
                                                    BankExpres.StatusId = 4;
                                                    BankExpres.AccountType = "INV";
                                                    BankExpres.AccountClassificationId = 1030;
                                                    BankExpres.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                                    BankExpres.TotalTax = 0;
                                                    BankExpres.UploadType = "M";
                                                    UserDiscountActualTotal = Convert.ToDecimal(BankExpres.ActualTotal); // GET ActualTotal of 2nd entry 

                                                    context.BankExpenses.Add(BankExpres);
                                                    context.SaveChanges();
                                                }
                                            }

                                            //third Entry 
                                            var CustomerDetails = context.tblCustomerOrSuppliers.Where(x => x.Id == userChk.CustomerId).FirstOrDefault();
                                            // var SuppliersEmail = context.Kl_GetSupplierDataforUserID(userChk.UserId, obj.Id).FirstOrDefault(); 
                                            if (CustomerDetails != null)
                                            {
                                                var Company_NameExist1 = context.Classifications.Where(i => i.Name == CustomerDetails.Company_Name && i.ChartAccountDisplayNumber == CustomerDetails.Debtors).FirstOrDefault();
                                                if (Company_NameExist1 != null)
                                                {
                                                    Classifications_Id = Company_NameExist1.Id;
                                                }
                                                else
                                                {
                                                    var ClassificationData = new Classification();
                                                    ClassificationData.ChartAccountDisplayNumber = CustomerDetails.Debtors;
                                                    ClassificationData.ChartAccountNumber = Convert.ToInt32(CustomerDetails.Debtors.Remove(4, 1));
                                                    ClassificationData.ClassificationType = CustomerDetails.Company_Name;
                                                    ClassificationData.Desc = CustomerDetails.Company_Name;
                                                    ClassificationData.Name = CustomerDetails.Company_Name;
                                                    ClassificationData.IsDeleted = false;
                                                    ClassificationData.CategoryId = 4;
                                                    ClassificationData.Type = "A";
                                                    context.Classifications.Add(ClassificationData);
                                                    context.SaveChanges();
                                                    Classifications_Id = ClassificationData.Id;
                                                }
                                            }

                                            if (itmd.SubTotal != 0 && itmd.SubTotal != null && Classifications_Id != 0 && Classifications_Id != null)
                                            {
                                                var BankE3 = new BankExpense();
                                                BankE3.ClassificationId = Classifications_Id;
                                                BankE3.Description = "INV#" + itmd.InvoiceNumber.PadLeft(5, '0');//CustomerDetails.Company_Name;
                                                BankE3.Debit = itmd.SubTotal;
                                                BankE3.UserId = FinanceUser.Id;
                                                BankE3.BankId = 8;
                                                BankE3.Credit = 0;
                                                BankE3.StatusId = 4;
                                                BankE3.AccountType = "INV";
                                                BankE3.TotalTax = 0;
                                                BankE3.GSTPercentage = 0;
                                                BankE3.HSTPercentage = 0;
                                                BankE3.QSTPercentage = 0;
                                                BankE3.PSTPercentage = 0;
                                                BankE3.PSTtax = 0;
                                                BankE3.HSTtax = 0;
                                                BankE3.IsDeleted = false;
                                                BankE3.IsVirtualEntry = false;
                                                BankE3.QSTtax = 0;
                                                BankE3.GSTtax = 0;
                                                BankE3.JVID = JVID;
                                                BankE3.UploadType = "M";
                                                BankE3.CreatedDate = DateTime.Now;
                                                BankE3.ActualTotal = itmd.SubTotal;
                                                BankE3.Total = itmd.SubTotal;
                                                BankE3.AccountClassificationId = 1030;
                                                BankE3.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                                context.BankExpenses.Add(BankE3);
                                                context.SaveChanges();

                                                //userChk.InvoiceJVID = JVID.ToString();
                                                //context.SaveChanges();
                                            }
                                        }
                                        //only one entry for all iyems
                                        #region if InvoiceDetail.DepositePayment==0 not add in db

                                        var InvoiceDetail = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();
                                        if (InvoiceDetail.DepositePayment != 0 && InvoiceDetail.DepositePayment != null)
                                        {
                                            //Cash Advanced Payment 
                                            var BankE4 = new BankExpense();
                                            BankE4.Debit = InvoiceDetail.DepositePayment; //if InvoiceDetail.DepositePayment==0 not add in db
                                            BankE4.UserId = FinanceUser.Id;
                                            BankE4.BankId = 8;
                                            BankE4.Description = "INV#" + itemdetails[0].InvoiceNumber.PadLeft(5, '0');
                                            BankE4.ClassificationId = 838;
                                            BankE4.Credit = 0;
                                            BankE4.StatusId = 4;
                                            BankE4.AccountType = "INV";
                                            BankE4.AccountClassificationId = 1030;
                                            BankE4.GSTPercentage = 0;
                                            BankE4.HSTPercentage = 0;
                                            BankE4.QSTPercentage = 0;
                                            BankE4.TotalTax = 0;
                                            BankE4.PSTPercentage = 0;
                                            BankE4.PSTtax = 0;
                                            BankE4.HSTtax = 0;
                                            BankE4.IsDeleted = false;
                                            BankE4.IsVirtualEntry = false;
                                            BankE4.UploadType = "M";
                                            BankE4.CreatedDate = DateTime.Now;
                                            BankE4.QSTtax = 0;
                                            BankE4.JVID = JVID;
                                            BankE4.GSTtax = 0;
                                            BankE4.Vendor = "1";
                                            BankE4.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                            BankE4.ActualTotal = InvoiceDetail.DepositePayment;
                                            BankE4.Total = InvoiceDetail.DepositePayment;
                                            context.BankExpenses.Add(BankE4);
                                            context.SaveChanges();


                                            //Description/Creditor    Fifthe entry of user 
                                            if (Classifications_Id != 0 && Classifications_Id != null)
                                            {
                                                var BankE5 = new BankExpense();
                                                BankE5.ClassificationId = Classifications_Id;
                                                BankE5.Description = "INV#" + itemdetails[0].InvoiceNumber.PadLeft(5, '0');
                                                BankE5.Debit = 0;
                                                BankE5.UserId = FinanceUser.Id;
                                                BankE5.BankId = 8;
                                                BankE5.Credit = InvoiceDetail.DepositePayment;
                                                BankE5.StatusId = 4;
                                                BankE5.AccountType = "INV";
                                                BankE5.TotalTax = 0;
                                                BankE5.GSTPercentage = 0;
                                                BankE5.HSTPercentage = 0;
                                                BankE5.QSTPercentage = 0;
                                                BankE5.PSTPercentage = 0;
                                                BankE5.PSTtax = 0;
                                                BankE5.HSTtax = 0;
                                                BankE5.IsDeleted = false;
                                                BankE5.IsVirtualEntry = false;
                                                BankE5.QSTtax = 0;
                                                BankE5.GSTtax = 0;
                                                BankE5.JVID = JVID;
                                                BankE5.Vendor = "1";
                                                BankE5.UploadType = "M";
                                                BankE5.CreatedDate = DateTime.Now;
                                                BankE5.ActualTotal = InvoiceDetail.DepositePayment;
                                                BankE5.Total = InvoiceDetail.DepositePayment;
                                                BankE5.AccountClassificationId = 1030;
                                                BankE5.Date = DateTime.Now;
                                                context.BankExpenses.Add(BankE5);
                                                context.SaveChanges();
                                            }
                                        }

                                        #endregion

                                        #region if InvoiceDetail.ShippingCost==0 not add in db

                                        var UserInvoiceShipping = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();

                                        if (UserInvoiceShipping.ShippingCost != 0 && UserInvoiceShipping.ShippingCost != null)
                                        {
                                            //Cash Advanced Payment 
                                            var BankE6 = new BankExpense();
                                            BankE6.Debit = UserInvoiceShipping.ShippingCost;
                                            BankE6.UserId = FinanceUser.Id;
                                            BankE6.BankId = 8;
                                            BankE6.Description = "INV#" + itemdetails[0].InvoiceNumber.PadLeft(5, '0') + " - Shipping/Freight";
                                            BankE6.ClassificationId = 952;
                                            BankE6.Credit = 0;
                                            BankE6.StatusId = 4;
                                            BankE6.AccountType = "INV";
                                            BankE6.AccountClassificationId = 1030;
                                            BankE6.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                            BankE6.GSTPercentage = 0;
                                            BankE6.HSTPercentage = 0;
                                            BankE6.QSTPercentage = 0;
                                            BankE6.TotalTax = 0;
                                            BankE6.PSTPercentage = 0;
                                            BankE6.PSTtax = 0;
                                            BankE6.HSTtax = 0;
                                            BankE6.IsDeleted = false;
                                            BankE6.IsVirtualEntry = false;
                                            BankE6.UploadType = "M";
                                            BankE6.CreatedDate = DateTime.Now;
                                            BankE6.QSTtax = 0;
                                            BankE6.JVID = JVID;
                                            BankE6.GSTtax = 0;
                                            BankE6.ActualTotal = UserInvoiceShipping.ShippingCost;
                                            BankE6.Total = UserInvoiceShipping.ShippingCost;
                                            context.BankExpenses.Add(BankE6);
                                            context.SaveChanges();


                                            //Description/Creditor    Fifthe entry of user 
                                            if (Classifications_Id != 0 && Classifications_Id != null)
                                            {
                                                var BankE7 = new BankExpense();
                                                BankE7.ClassificationId = Classifications_Id;
                                                BankE7.Description = "INV#" + itemdetails[0].InvoiceNumber.PadLeft(5, '0');
                                                BankE7.Debit = 0;
                                                BankE7.UserId = FinanceUser.Id;
                                                BankE7.BankId = 8;
                                                BankE7.Credit = UserInvoiceShipping.ShippingCost;
                                                BankE7.StatusId = 4;
                                                BankE7.AccountType = "INV";
                                                BankE7.TotalTax = 0;
                                                BankE7.GSTPercentage = 0;
                                                BankE7.HSTPercentage = 0;
                                                BankE7.QSTPercentage = 0;
                                                BankE7.PSTPercentage = 0;
                                                BankE7.PSTtax = 0;
                                                BankE7.HSTtax = 0;
                                                BankE7.IsDeleted = false;
                                                BankE7.IsVirtualEntry = false;
                                                BankE7.QSTtax = 0;
                                                BankE7.GSTtax = 0;
                                                BankE7.JVID = JVID;
                                                BankE7.UploadType = "M";
                                                BankE7.CreatedDate = DateTime.Now;
                                                BankE7.ActualTotal = UserInvoiceShipping.ShippingCost;
                                                BankE7.Total = UserInvoiceShipping.ShippingCost;
                                                BankE7.AccountClassificationId = 1030;
                                                BankE7.Date = Convert.ToDateTime(itemdetails[0].InvoiceDate);
                                                context.BankExpenses.Add(BankE7);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                        #endregion


                                    #endregion

                                    #region  update finance tables for Customer

                                    var CustomerEmail = context.tblCustomerOrSuppliers.Where(i => i.Id == userChk.CustomerId && i.IsFinance == true).Select(s => s.Email).FirstOrDefault();
                                    if (CustomerEmail != null)
                                    {
                                        var ItemDetailsForCustomer = (from invc in context.tblInvoiceDetails
                                                                      join item in context.tblItemDetails
                                                                     on invc.Id equals item.InvoiceId
                                                                      where item.InvoiceId == obj.Id
                                                                      select new
                                                                      {
                                                                          invc.CreatedDate,
                                                                          invc.InvoiceNumber,
                                                                          invc.InvoiceDate,
                                                                          item.InvoiceId,
                                                                          item.Quantity,
                                                                          item.Rate,
                                                                          item.GST_Tax,
                                                                          item.HST_Tax,
                                                                          item.PST_Tax,
                                                                          item.QST_Tax,
                                                                          item.ServiceType,
                                                                          item.SubTotal,
                                                                          item.Description,
                                                                          item.Discount,
                                                                          item.Amount,
                                                                          item.Item
                                                                      }).ToList();

                                        var CustomerService = obj.Customer_Service;



                                        var Customer_FinanceUser = context.UserRegistrations.Where(i => i.Email == CustomerEmail && i.IsUnlink == false).FirstOrDefault();
                                        if (Customer_FinanceUser != null)
                                        {
                                            var BankExpres = new BankExpense();

                                            for (int i = 0; i < ItemDetailsForCustomer.Count(); i++)
                                            {
                                                decimal Total = Decimal.Multiply(Convert.ToDecimal(ItemDetailsForCustomer[i].Quantity), Convert.ToDecimal(ItemDetailsForCustomer[i].Rate));
                                                BankExpres.IsDeleted = false;
                                                BankExpres.IsVirtualEntry = false;
                                                BankExpres.Total = Total;
                                                BankExpres.Credit = 0;
                                                BankExpres.Debit = Total;

                                                if (!string.IsNullOrEmpty(ItemDetailsForCustomer[i].Discount))
                                                {
                                                    if (ItemDetailsForCustomer[i].Discount.Contains("P_"))
                                                    {
                                                        discount = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].Discount.Substring(2, ItemDetailsForCustomer[i].Discount.Length - 2)) / 100 * Total, 2); //* Convert.ToDecimal(itmd.Rate * itmd.Quantity);   //(ItemDetails.Discount / 100) ;
                                                    }
                                                    else
                                                    {
                                                        discount = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].Discount.Substring(2, ItemDetailsForCustomer[i].Discount.Length - 2)), 2);
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(ItemDetailsForCustomer[i].GST_Tax) && ItemDetailsForCustomer[i].GST_Tax != "0")
                                                {
                                                    if (ItemDetailsForCustomer[i].GST_Tax.Contains("P_"))
                                                    {
                                                        BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].GST_Tax.Substring(2, ItemDetailsForCustomer[i].GST_Tax.Length - 2)), 2);
                                                        BankExpres.GSTtax = Math.Round((Convert.ToDecimal(BankExpres.GSTPercentage * (BankExpres.Total - discount)) / 100), 2);
                                                    }
                                                    else
                                                    {
                                                        BankExpres.GSTtax = Math.Round((Convert.ToDecimal(ItemDetailsForCustomer[i].GST_Tax.Substring(2, ItemDetailsForCustomer[i].GST_Tax.Length - 2))), 2);
                                                        BankExpres.GSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.GSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                    }
                                                }
                                                else
                                                {
                                                    BankExpres.GSTtax = 0;
                                                    BankExpres.GSTPercentage = 0;
                                                }

                                                if (!string.IsNullOrEmpty(ItemDetailsForCustomer[i].HST_Tax) && ItemDetailsForCustomer[i].HST_Tax != "0")
                                                {
                                                    if (ItemDetailsForCustomer[i].HST_Tax.Contains("P_"))
                                                    {
                                                        BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].HST_Tax.Substring(2, ItemDetailsForCustomer[i].HST_Tax.Length - 2)), 2);
                                                        BankExpres.HSTtax = Math.Round(Convert.ToDecimal((BankExpres.HSTPercentage * (BankExpres.Total - discount)) / 100), 2); //not inserted in db
                                                    }
                                                    else
                                                    {
                                                        BankExpres.HSTtax = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].HST_Tax.Substring(2, ItemDetailsForCustomer[i].HST_Tax.Length - 2)), 2);
                                                        BankExpres.HSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.HSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                    }
                                                }
                                                else
                                                {
                                                    BankExpres.HSTPercentage = 0;
                                                    BankExpres.HSTtax = 0;

                                                }

                                                if (!string.IsNullOrEmpty(ItemDetailsForCustomer[i].PST_Tax) && ItemDetailsForCustomer[i].PST_Tax != "0")
                                                {

                                                    if (ItemDetailsForCustomer[i].PST_Tax.Contains("P_"))
                                                    {
                                                        BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].PST_Tax.Substring(2, ItemDetailsForCustomer[i].PST_Tax.Length - 2)), 2);
                                                        BankExpres.PSTtax = Math.Round(Convert.ToDecimal((BankExpres.PSTPercentage * (BankExpres.Total - discount)) / 100), 2);
                                                    }
                                                    else
                                                    {
                                                        BankExpres.PSTtax = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].PST_Tax.Substring(2, ItemDetailsForCustomer[i].PST_Tax.Length - 2)), 2);
                                                        BankExpres.PSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.PSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                    }
                                                }
                                                else
                                                {
                                                    BankExpres.PSTPercentage = 0;
                                                    BankExpres.PSTtax = 0;
                                                }

                                                if (!string.IsNullOrEmpty(ItemDetailsForCustomer[i].QST_Tax) && ItemDetailsForCustomer[i].QST_Tax != "0")
                                                {
                                                    if (ItemDetailsForCustomer[i].QST_Tax.Contains("P_"))
                                                    {
                                                        BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].QST_Tax.Substring(2, ItemDetailsForCustomer[i].QST_Tax.Length - 2)), 2);
                                                        BankExpres.QSTtax = Math.Round(Convert.ToDecimal((BankExpres.QSTPercentage * (BankExpres.Total - discount))) / 100, 2);
                                                    }
                                                    else
                                                    {
                                                        BankExpres.QSTtax = Math.Round(Convert.ToDecimal(ItemDetailsForCustomer[i].QST_Tax.Substring(2, ItemDetailsForCustomer[i].QST_Tax.Length - 2)), 2);
                                                        BankExpres.QSTPercentage = Math.Round(Convert.ToDecimal((BankExpres.QSTtax * 100) / (BankExpres.Total - discount)), 2);
                                                    }
                                                }
                                                else
                                                {
                                                    BankExpres.QSTtax = 0;
                                                    BankExpres.QSTPercentage = 0;
                                                }



                                                decimal TotalTaxAmount = Convert.ToDecimal(BankExpres.QSTtax + BankExpres.PSTtax + BankExpres.HSTtax + BankExpres.GSTtax);
                                                BankExpres.UserId = Customer_FinanceUser.Id;
                                                BankExpres.BankId = 8;
                                                BankExpres.Description = "INV#" + ItemDetailsForCustomer[i].InvoiceNumber.PadLeft(5, '0') + " - " + ItemDetailsForCustomer[i].Description;
                                                BankExpres.AccountType = "INV";
                                                BankExpres.AccountClassificationId = 1030;
                                                string ObjectCustomerService = CustomerService[i].ToString();
                                                if (!string.IsNullOrEmpty(ObjectCustomerService))
                                                {
                                                    BankExpres.ClassificationId = context.Classifications.Where(e => e.Name == ObjectCustomerService).Select(s => s.Id).FirstOrDefault();
                                                }
                                                else
                                                    BankExpres.ClassificationId = 1;

                                                BankExpres.CreatedDate = DateTime.Now;
                                                BankExpres.StatusId = 4;
                                                BankExpres.Date = Convert.ToDateTime(ItemDetailsForCustomer[i].InvoiceDate);

                                                decimal Subtract = TotalTaxAmount + discount;
                                                BankExpres.ActualTotal = BankExpres.Total - Subtract;
                                                CustomerFirstActualTotal = Convert.ToDecimal(BankExpres.ActualTotal);
                                                BankExpres.TotalTax = TotalTaxAmount;
                                                BankExpres.JVID = CustomerJvid;
                                                BankExpres.UploadType = "M";

                                                if (Total != 0 && BankExpres.ActualTotal != 0)
                                                {
                                                    context.BankExpenses.Add(BankExpres);
                                                    context.SaveChanges();
                                                }

                                                //For Discount

                                                #region if Discounttotal not equal to null
                                                decimal Discounttotal = 0;
                                                if (UsersDiscountTotal != 0)
                                                {
                                                    var InvoiceDetail = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();
                                                    BankExpres.Credit = UsersDiscountTotal;//TotalTaxAmount;
                                                    BankExpres.IsVirtualEntry = false;
                                                    BankExpres.IsDeleted = false;
                                                    BankExpres.UserId = Customer_FinanceUser.Id;
                                                    BankExpres.BankId = 8;
                                                    BankExpres.Total = UsersDiscountTotal;
                                                    BankExpres.Description = "INV#" + ItemDetailsForCustomer[i].InvoiceNumber.PadLeft(5, '0');
                                                    BankExpres.ClassificationId = 966;
                                                    BankExpres.Debit = 0;
                                                    BankExpres.GSTPercentage = 0;
                                                    BankExpres.TotalTax = 0;
                                                    BankExpres.HSTPercentage = 0;
                                                    BankExpres.QSTPercentage = 0;
                                                    BankExpres.PSTPercentage = 0;
                                                    BankExpres.PSTtax = 0;
                                                    BankExpres.HSTtax = 0;
                                                    BankExpres.QSTtax = 0;
                                                    BankExpres.GSTtax = 0;
                                                    BankExpres.UploadType = "M";

                                                    BankExpres.ActualTotal = UsersDiscountTotal;//TotalTaxAmount;
                                                    BankExpres.JVID = CustomerJvid;
                                                    BankExpres.StatusId = 4;
                                                    BankExpres.AccountType = "INV";
                                                    BankExpres.AccountClassificationId = 1030;
                                                    BankExpres.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                    CustomerDiscountActualTotal = Convert.ToDecimal(BankExpres.ActualTotal);
                                                    context.BankExpenses.Add(BankExpres);
                                                    context.SaveChanges();


                                                #endregion

                                                    //Third Entry 
                                                    var SuppliersEmail = context.Kl_GetSupplierDataforUserID(userChk.UserId, obj.Id).FirstOrDefault(); //context.tblCustomerOrSuppliers.Where(x => x.UserId == userChk.UserId && x.RoleId == 1).Select(s => s.Email).FirstOrDefault();
                                                    //   var CustomerDetails = context.tblCustomerOrSuppliers.Where(x => x.Id == userChk.CustomerId).FirstOrDefault();
                                                    if (SuppliersEmail != null)
                                                    {
                                                        var Company_Name_Supplier = context.Classifications.Where(e => e.Name == SuppliersEmail.Company_Name && e.ChartAccountDisplayNumber == SuppliersEmail.Credetor).FirstOrDefault();
                                                        if (Company_Name_Supplier != null)
                                                        {
                                                            Classifications_Id = Company_Name_Supplier.Id;
                                                        }
                                                        else
                                                        {
                                                            var ClassificationData = new Classification();
                                                            ClassificationData.ChartAccountDisplayNumber = SuppliersEmail.Credetor;
                                                            ClassificationData.ChartAccountNumber = Convert.ToInt32(SuppliersEmail.Credetor.Remove(4, 1));
                                                            ClassificationData.ClassificationType = SuppliersEmail.Company_Name;
                                                            ClassificationData.Desc = SuppliersEmail.Company_Name;
                                                            ClassificationData.Name = SuppliersEmail.Company_Name;
                                                            ClassificationData.Type = "A";
                                                            ClassificationData.CategoryId = 1;
                                                            ClassificationData.IsDeleted = false;
                                                            context.Classifications.Add(ClassificationData);
                                                            context.SaveChanges();

                                                            Classifications_Id = ClassificationData.Id;
                                                        }
                                                    }

                                                    //New Entry Customer
                                                    //decimal k = (Convert.ToDecimal(p) - Convert.ToDecimal(q));
                                                    if (Classifications_Id != 0 && Classifications_Id != null)
                                                    {
                                                        BankExpres.ClassificationId = Classifications_Id;//ClassificationData.Id;
                                                        BankExpres.Description = "INV#" + ItemDetailsForCustomer[i].InvoiceNumber.PadLeft(5, '0');//SuppliersEmail.Company_Name;
                                                        BankExpres.Debit = 0;
                                                        BankExpres.UserId = Customer_FinanceUser.Id;
                                                        BankExpres.BankId = 8;
                                                        BankExpres.GSTPercentage = 0;
                                                        BankExpres.HSTPercentage = 0;
                                                        BankExpres.QSTPercentage = 0;
                                                        BankExpres.IsDeleted = false;
                                                        BankExpres.IsVirtualEntry = false;
                                                        BankExpres.PSTPercentage = 0;
                                                        BankExpres.PSTtax = 0;
                                                        BankExpres.HSTtax = 0;
                                                        BankExpres.QSTtax = 0;
                                                        BankExpres.TotalTax = 0;
                                                        BankExpres.GSTtax = 0;
                                                        BankExpres.Credit = ItemDetailsForCustomer[i].SubTotal; // (Convert.ToDecimal(CustomerFirstActualTotal));
                                                        BankExpres.StatusId = 4;
                                                        BankExpres.ActualTotal = ItemDetailsForCustomer[i].SubTotal;
                                                        BankExpres.AccountType = "INV";
                                                        BankExpres.Total = ItemDetailsForCustomer[i].SubTotal;
                                                        BankExpres.AccountClassificationId = 1030;
                                                        BankExpres.JVID = CustomerJvid;
                                                        BankExpres.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                        BankExpres.UploadType = "M";
                                                        BankExpres.CreatedDate = DateTime.Now;
                                                        context.BankExpenses.Add(BankExpres);
                                                        context.SaveChanges();
                                                    }
                                                }
                                                #region if InvoiceDetail.DepositePayment != 0
                                                var InvoiceDetail1 = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();

                                                if (InvoiceDetail1.DepositePayment != 0 && InvoiceDetail1.DepositePayment != null)
                                                {
                                                    #region Cash Advanced Payment
                                                    BankExpres.IsDeleted = false;
                                                    BankExpres.IsVirtualEntry = false;
                                                    BankExpres.Debit = 0;
                                                    BankExpres.UserId = Customer_FinanceUser.Id;
                                                    BankExpres.BankId = 8;
                                                    BankExpres.ClassificationId = 838;
                                                    BankExpres.Description = "INV#" + ItemDetailsForCustomer[0].InvoiceNumber.PadLeft(5, '0');
                                                    BankExpres.Credit = InvoiceDetail1.DepositePayment;
                                                    BankExpres.StatusId = 4;
                                                    BankExpres.AccountClassificationId = 1030;
                                                    BankExpres.AccountType = "INV";
                                                    BankExpres.TotalTax = 0;
                                                    BankExpres.ActualTotal = InvoiceDetail1.DepositePayment;
                                                    BankExpres.GSTPercentage = 0;
                                                    BankExpres.HSTPercentage = 0;
                                                    BankExpres.QSTPercentage = 0;
                                                    BankExpres.PSTPercentage = 0;
                                                    BankExpres.PSTtax = 0;
                                                    BankExpres.HSTtax = 0;
                                                    BankExpres.QSTtax = 0;
                                                    BankExpres.GSTtax = 0;
                                                    BankExpres.UploadType = "M";
                                                    BankExpres.CreatedDate = DateTime.Now;
                                                    BankExpres.Total = InvoiceDetail1.DepositePayment;
                                                    BankExpres.JVID = CustomerJvid;
                                                    BankExpres.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                    BankExpres.Vendor = "1";
                                                    context.BankExpenses.Add(BankExpres);
                                                    context.SaveChanges();
                                                    #endregion

                                                    //Description/Debitor
                                                    if (Classifications_Id != 0 && Classifications_Id != null)
                                                    {
                                                        BankExpres.ClassificationId = Classifications_Id;   //ClassificationData.Id;
                                                        BankExpres.Description = "INV#" + ItemDetailsForCustomer[0].InvoiceNumber.PadLeft(5, '0');//SuppliersEmail.Company_Name;
                                                        BankExpres.Debit = InvoiceDetail1.DepositePayment;
                                                        BankExpres.UserId = Customer_FinanceUser.Id;
                                                        BankExpres.BankId = 8;
                                                        BankExpres.GSTPercentage = 0;
                                                        BankExpres.HSTPercentage = 0;
                                                        BankExpres.QSTPercentage = 0;
                                                        BankExpres.IsDeleted = false;
                                                        BankExpres.IsVirtualEntry = false;
                                                        BankExpres.PSTPercentage = 0;
                                                        BankExpres.PSTtax = 0;
                                                        BankExpres.HSTtax = 0;
                                                        BankExpres.QSTtax = 0;
                                                        BankExpres.TotalTax = 0;
                                                        BankExpres.GSTtax = 0;
                                                        BankExpres.Credit = 0;
                                                        BankExpres.StatusId = 4;
                                                        BankExpres.ActualTotal = InvoiceDetail1.DepositePayment;
                                                        BankExpres.AccountType = "INV";
                                                        BankExpres.Total = InvoiceDetail1.DepositePayment;
                                                        BankExpres.AccountClassificationId = 1030;
                                                        BankExpres.JVID = CustomerJvid;
                                                        BankExpres.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                        BankExpres.UploadType = "M";
                                                        BankExpres.CreatedDate = DateTime.Now;
                                                        BankExpres.Vendor = "1";
                                                        context.BankExpenses.Add(BankExpres);
                                                        context.SaveChanges();
                                                    }
                                                }

                                                #endregion

                                                #region if InvoiceDetail.ShippingCost != 0
                                                var CustomerInvoiceShipping = context.tblInvoiceDetails.Where(x => x.Id == obj.Id).FirstOrDefault();

                                                if (CustomerInvoiceShipping.ShippingCost != 0 && CustomerInvoiceShipping.ShippingCost != null)
                                                {
                                                    var CusBank6 = new BankExpense();
                                                    #region Cash Advanced Payment
                                                    CusBank6.IsDeleted = false;
                                                    CusBank6.IsVirtualEntry = false;
                                                    CusBank6.Debit = 0;
                                                    CusBank6.UserId = Customer_FinanceUser.Id;
                                                    CusBank6.BankId = 8;
                                                    CusBank6.ClassificationId = 969;
                                                    CusBank6.Description = "INV#" + ItemDetailsForCustomer[0].InvoiceNumber.PadLeft(5, '0') + " - Shipping/Freight";
                                                    CusBank6.Credit = CustomerInvoiceShipping.ShippingCost;
                                                    CusBank6.StatusId = 4;
                                                    CusBank6.AccountClassificationId = 1030;
                                                    CusBank6.AccountType = "INV";
                                                    CusBank6.TotalTax = 0;
                                                    CusBank6.ActualTotal = CustomerInvoiceShipping.ShippingCost;
                                                    CusBank6.GSTPercentage = 0;
                                                    CusBank6.HSTPercentage = 0;
                                                    CusBank6.QSTPercentage = 0;
                                                    CusBank6.PSTPercentage = 0;
                                                    CusBank6.PSTtax = 0;
                                                    CusBank6.HSTtax = 0;
                                                    CusBank6.QSTtax = 0;
                                                    CusBank6.GSTtax = 0;
                                                    CusBank6.UploadType = "M";
                                                    CusBank6.CreatedDate = DateTime.Now;
                                                    CusBank6.Total = CustomerInvoiceShipping.ShippingCost;
                                                    CusBank6.JVID = CustomerJvid;
                                                    CusBank6.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                    context.BankExpenses.Add(CusBank6);
                                                    context.SaveChanges();
                                                    #endregion

                                                    //Description/Debitor
                                                    if (Classifications_Id != 0 && Classifications_Id != null)
                                                    {
                                                        var CusBank7 = new BankExpense();
                                                        CusBank7.ClassificationId = Classifications_Id;//ClassificationData.Id;
                                                        CusBank7.Description = "INV#" + ItemDetailsForCustomer[0].InvoiceNumber.PadLeft(5, '0');//SuppliersEmail.Company_Name;
                                                        CusBank7.Debit = CustomerInvoiceShipping.ShippingCost;
                                                        CusBank7.UserId = Customer_FinanceUser.Id;
                                                        CusBank7.BankId = 8;
                                                        CusBank7.GSTPercentage = 0;
                                                        CusBank7.HSTPercentage = 0;
                                                        CusBank7.QSTPercentage = 0;
                                                        CusBank7.IsDeleted = false;
                                                        CusBank7.IsVirtualEntry = false;
                                                        CusBank7.PSTPercentage = 0;
                                                        CusBank7.PSTtax = 0;
                                                        CusBank7.HSTtax = 0;
                                                        CusBank7.QSTtax = 0;
                                                        CusBank7.TotalTax = 0;
                                                        CusBank7.GSTtax = 0;
                                                        CusBank7.Credit = 0;
                                                        CusBank7.StatusId = 4;
                                                        CusBank7.ActualTotal = CustomerInvoiceShipping.ShippingCost;
                                                        CusBank7.AccountType = "INV";
                                                        CusBank7.Total = CustomerInvoiceShipping.ShippingCost;
                                                        CusBank7.AccountClassificationId = 1030;
                                                        CusBank7.UploadType = "M";
                                                        CusBank7.JVID = CustomerJvid;
                                                        CusBank7.Date = Convert.ToDateTime(ItemDetailsForCustomer[0].InvoiceDate);
                                                        CusBank7.CreatedDate = DateTime.Now;
                                                        context.BankExpenses.Add(CusBank7);
                                                        context.SaveChanges();
                                                    }
                                                }

                                                #endregion
                                                userChk.CustomerInvoiceJVID = CustomerJvid.ToString();
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    //}
                                    #endregion

                                    userChk.In_R_FlowStatus = "Closed";
                                    userChk.In_R_Status = "Accepted";
                                    userChk.Pro_FlowStatus = "Closed";
                                    userChk.Pro_Status = "Accepted";
                                    context.SaveChanges();
                                    tblItemDetail Items = new tblItemDetail();

                                    var counter = obj.Customer_ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.Customer_Service[i] == "null")
                                        {
                                            item.Customer_Service = EmptyIfNull(obj.Customer_Service[i]);
                                        }
                                        else
                                        {
                                            item.Customer_Service = obj.Customer_Service[i];
                                        }
                                        item.Customer_ServiceTypeId = obj.Customer_ServiceTypeId[i];
                                        context.SaveChanges();
                                    }
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Cancel" && obj.Type == 1 && obj.SectionType == "Received")
                                {
                                    if (!string.IsNullOrEmpty(userChk.InvoiceJVID) && !string.IsNullOrEmpty(userChk.CustomerInvoiceJVID))
                                    {
                                        int InvoiceJVID = Convert.ToInt16(userChk.InvoiceJVID);
                                        var BankEInvoice = context.BankExpenses.Where(x => x.JVID == InvoiceJVID && x.AccountType == "INV" && x.Vendor != "1").ToList();

                                        int CustomerInvoiceJVID = Convert.ToInt16(userChk.CustomerInvoiceJVID);
                                        var BankECustomer = context.BankExpenses.Where(x => x.JVID == CustomerInvoiceJVID && x.AccountType == "INV" && x.Vendor != "1").ToList();

                                        if (!string.IsNullOrEmpty(userChk.InvoiceJVID))
                                        {
                                            foreach (var Bank in BankEInvoice)
                                            {
                                                decimal credit = (decimal)Bank.Debit;
                                                decimal debit = (decimal)Bank.Credit;

                                                Bank.Credit = credit;
                                                Bank.Debit = debit;
                                                context.BankExpenses.Add(Bank);
                                                context.SaveChanges();
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(userChk.CustomerInvoiceJVID))
                                        {
                                            foreach (var Bank in BankECustomer)
                                            {
                                                decimal credit = (decimal)Bank.Debit;
                                                decimal debit = (decimal)Bank.Credit;

                                                Bank.Credit = credit;
                                                Bank.Debit = debit;
                                                context.BankExpenses.Add(Bank);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    userChk.In_R_FlowStatus = "Cancelled";
                                    userChk.In_R_Status = "Declined";
                                    userChk.Pro_FlowStatus = "Cancelled";
                                    userChk.Pro_Status = "Declined";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();

                                }
                                else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Received")
                                {
                                    userChk.In_R_FlowStatus = "Closed";
                                    userChk.In_R_Status = "Deleted";
                                    userChk.Pro_FlowStatus = "Closed";
                                    userChk.Pro_Status = "Deleted";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Delete" && obj.Type == 2 && obj.SectionType == "Sent")
                                {
                                    userChk.In_R_FlowStatus = "Closed";
                                    userChk.In_R_Status = "Deleted";
                                    userChk.Pro_FlowStatus = "Closed";
                                    userChk.Pro_Status = "Deleted";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Send" && obj.Type == 2 && obj.SectionType == "Sent")
                                {
                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userChk.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userChk.DepositePayment = obj.DepositePayment;
                                    }

                                    userChk.BalanceDue = obj.BalanceDue;
                                    userChk.In_R_FlowStatus = "Sent";
                                    userChk.In_R_Status = "Inprogress";
                                    userChk.Pro_FlowStatus = "Pending Approval";
                                    userChk.Pro_Status = "Inprogress";
                                    context.SaveChanges();
                                    tblItemDetail Items = new tblItemDetail();

                                    var counter = obj.ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {

                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.ServiceType[i] == "null")
                                        {
                                            item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            item.ServiceType = obj.ServiceType[i];
                                        }
                                        item.ServiceTypeId = obj.ServiceTypeId[i];
                                        context.SaveChanges();
                                    }
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Convert" && obj.Type == 2 && obj.SectionType == "Sent")
                                {
                                    userChk.In_R_FlowStatus = "Pending Approval";
                                    userChk.In_R_Status = "Converted";
                                    userChk.Pro_FlowStatus = "Pending Approval";
                                    userChk.Pro_Status = "Converted";
                                    userChk.Type = 1;
                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userChk.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userChk.DepositePayment = obj.DepositePayment;
                                    }

                                    userChk.BalanceDue = obj.BalanceDue;
                                    context.SaveChanges();
                                    tblItemDetail Items = new tblItemDetail();

                                    var counter = obj.ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.ServiceType[i] == "null")
                                        {
                                            item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            item.ServiceType = obj.ServiceType[i];
                                        }

                                        // obj.ServiceType[i];
                                        item.ServiceTypeId = obj.ServiceTypeId[i];
                                        context.SaveChanges();
                                    }
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Save" && obj.Type == 2 && obj.SectionType == "Sent")
                                {
                                    userChk.In_R_FlowStatus = "Draft";
                                    userChk.In_R_Status = "Inprogress";
                                    userChk.Pro_FlowStatus = "Draft";
                                    userChk.Pro_Status = "Inprogress";
                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userChk.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userChk.DepositePayment = obj.DepositePayment;
                                    }

                                    userChk.BalanceDue = obj.BalanceDue;
                                    context.SaveChanges();

                                    var counter = obj.ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {

                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.ServiceType[i] == null)
                                        {
                                            item.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            item.ServiceType = obj.ServiceType[i];
                                        }
                                        item.ServiceTypeId = obj.ServiceTypeId[i];
                                        context.SaveChanges();
                                    }
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Approve" && obj.Type == 2 && obj.SectionType == "Received")
                                {
                                    tblItemDetail Items = new tblItemDetail();

                                    var counter = obj.Customer_ServiceTypeId.Count();
                                    for (int i = 0; i < counter; i++)
                                    {
                                        int id = obj.ItemId[i];
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).FirstOrDefault();
                                        if (obj.Customer_Service[i] == "null")
                                        {
                                            item.Customer_Service = EmptyIfNull(obj.Customer_Service[i]);
                                        }
                                        else
                                        {
                                            item.Customer_Service = obj.Customer_Service[i];
                                        }

                                        item.Customer_ServiceTypeId = obj.Customer_ServiceTypeId[i];
                                    }
                                    userChk.In_R_FlowStatus = "Approved";
                                    userChk.In_R_Status = "Pending Conversion";
                                    userChk.Pro_FlowStatus = "Approved";
                                    userChk.Pro_Status = "Pending Conversion";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Reject" && obj.Type == 2 && obj.SectionType == "Received")
                                {
                                    userChk.In_R_FlowStatus = "Declined";
                                    userChk.In_R_Status = "Rejected";
                                    userChk.Pro_FlowStatus = "Declined";
                                    userChk.Pro_Status = "Rejected";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Decline" && obj.Type == 2 && obj.SectionType == "Received")
                                {
                                    userChk.In_R_FlowStatus = "Cancelled";
                                    userChk.In_R_Status = "Declined";
                                    userChk.Pro_FlowStatus = "Cancelled";
                                    userChk.Pro_Status = "Declined";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                                else if (obj.ButtonType == "Delete" && obj.Type == 1 && obj.SectionType == "Sent")
                                {
                                    userChk.In_R_FlowStatus = "Closed";
                                    userChk.In_R_Status = "Deleted";
                                    userChk.Pro_FlowStatus = "Closed";
                                    userChk.Pro_Status = "Deleted";
                                    context.SaveChanges();
                                    dbContextTransaction.Commit();
                                }
                            }
                            return InvoiceId = userChk.Id;

                        }
                    }
                }
                else
                {
                    //return new ApiResponseDto { ResponseMessage = "Please provide data 2.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                }
            }
            catch (Exception)
            {
                throw;
            }
            return InvoiceId;
        }

        #endregion

        #region Manual Payment
        public int ManualPayment(ManualPaymentDto obj)
        {
            try
            {
                using (var context = new KFentities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var userChk = context.tblInvoiceDetails.Where(i => i.Id == obj.InvoiceId).FirstOrDefault();

                        if (userChk != null)
                        {
                            if (obj.IsCustomer == false)
                            {
                                if (userChk.SupplierManualPaidAmount == null)
                                {
                                    userChk.SupplierManualPaidAmount = obj.SupplierManualPaidAmount;
                                }
                                else
                                    userChk.SupplierManualPaidAmount += obj.SupplierManualPaidAmount;

                                userChk.SupplierManualPaidJVID = obj.SupplierManualPaidJVID;
                                userChk.IsSupplierManualPaid = true;
                                userChk.PaymentDate = DateTime.Now;
                                context.SaveChanges();
                            }
                            else if (obj.IsCustomer == true)
                            {
                                if (userChk.CustomerManualPaidAmount == null)
                                {
                                    userChk.CustomerManualPaidAmount += obj.CustomerManualPaidAmount;
                                }
                                userChk.CustomerManualPaidAmount += obj.CustomerManualPaidAmount;
                                userChk.CustomerManualPaidJVID = obj.CustomerManualPaidJVID;
                                userChk.IsCustomerManualPaid = true;
                                userChk.PaymentDate = DateTime.Now;
                                context.SaveChanges();
                            }
                            dbContextTransaction.Commit();
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return 0;
        }
        #endregion

        #region Stripe Payment
        public int StripePayment(StripePaymentDto obj)
        {
            try   //encryption
            {
                using (var context = new KFentities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var CardData = new StripePayCardDetail();
                        var userChk = context.InvoiceUserRegistrations.Where(i => i.Id == obj.SupplierID).Select(s => s.EmailTo).FirstOrDefault();

                        if (userChk != null)
                        {
                            var StripeData = context.StripeKeyDetails.Where(i => i.Email == userChk).FirstOrDefault();
                            if (StripeData != null)
                            {
                             //   obj.CardNumber = encrypt.AesEncrypt(obj.CardNumber);
                                obj.CardNumber = encrypt.AesDecrypt(obj.CardNumber);

                                string Token = "";
                                var myToken = new StripeTokenCreateOptions();
                                myToken.Card = new StripeCreditCardOptions()
                                {
                                    Number = obj.CardNumber,
                                    ExpirationYear = Convert.ToString(obj.ExpiryYear),
                                    ExpirationMonth = Convert.ToString(obj.ExpiryMonth)
                                };
                                var tokenService = new StripeTokenService();
                                //tokenService.ApiKey = StripeData.StripePublishKey;
                                tokenService.ApiKey = stripePublickey;
                                StripeToken stripeToken = tokenService.Create(myToken);
                                var stripeCharge = tokenService.Create(myToken);
                                Token = stripeCharge.Id;

                                if (Token != null)
                                {
                                    var currency = "";
                                    var Invoicecurrency = context.InvoiceUserRegistrations.Where(i => i.EmailTo == userChk).Select(s => s.TradingCurrency).FirstOrDefault();

                                    if (Invoicecurrency != null)
                                    {
                                        currency = Invoicecurrency.ToLower();
                                    }
                                    else
                                    {
                                        var Financecurrency = context.Currencies.Select(d => d.CurrencyType).FirstOrDefault(); // context.UserRegistrations.Where(i => i.Email == userChk).Select(s => s.Currency).FirstOrDefault();
                                        currency = Financecurrency.ToString().ToLower();
                                    }
                                    var myCharge = new StripeChargeCreateOptions
                                    {
                                        //convert the amount of £12.50 to pennies i.e. 1250
                                        Amount = Convert.ToInt32(obj.PaidAmount) * 100,
                                        Currency = currency,                                //trancation depends on Currency 
                                        Description = "Description for test charge",
                                        Source = new StripeSourceOptions
                                        {
                                            TokenId = Token,
                                        }
                                    };
                                    //  var stripesecretkey = "sk_test_P0XjY4QBs5eYKF2qEF0DbQfz";
                                    var chargeService = new StripeChargeService(stripesecretkey);
                                    var InvoicestripeCharge = chargeService.Create(myCharge);

                                    if (InvoicestripeCharge.Status == "succeeded")
                                    {
                                        var pamnt = new StripePaymnt();
                                        pamnt.InvoiceID = obj.InvoiceId;
                                        pamnt.IsPaymentbyStripe = true;
                                        context.StripePaymnts.Add(pamnt);
                                        context.SaveChanges();

                                        var InvoiceDetail = context.tblInvoiceDetails.Where(i => i.Id == obj.InvoiceId).FirstOrDefault();
                                        InvoiceDetail.BalanceDue -= obj.PaidAmount;
                                        InvoiceDetail.DepositePayment += obj.PaidAmount;
                                        InvoiceDetail.PaymentDate = DateTime.Now;
                                        InvoiceDetail.StripeJVID = obj.PaidJVID;
                                        context.SaveChanges();
                                        dbContextTransaction.Commit();
                                        return 1;
                                    }
                                    else
                                    {
                                        dbContextTransaction.Rollback();
                                        return 0;
                                    }
                                }
                            }
                            else
                            {
                                return 0; //data not exist in stripekey table
                            }
                        }
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        //#region Stripe Payment
        //public int StripePayment(StripePaymentDto obj)
        //{

        //    try   //encryption
        //    {
        //        using (var context = new KFentities())
        //        {
        //            using (var dbContextTransaction = context.Database.BeginTransaction())
        //            {
        //                var CardData = new StripePayCardDetail();
        //                var userChk = context.InvoiceUserRegistrations.Where(i => i.Id == obj.SupplierID).Select(s => s.EmailTo).FirstOrDefault();

        //                if (userChk != null)
        //                {
        //                    var StripeData = context.StripeKeyDetails.Where(i => i.Email == userChk).FirstOrDefault();
        //                    if (StripeData != null)
        //                    {
        //                        obj.CardNumber = encrypt.AesDecrypt(obj.CardNumber);

        //                        string Token = "";
        //                        var myToken = new StripeTokenCreateOptions();
        //                        myToken.Card = new StripeCreditCardOptions()
        //                        {
        //                            Number = obj.CardNumber,
        //                            ExpirationYear = Convert.ToString(obj.ExpiryYear),
        //                            ExpirationMonth = Convert.ToString(obj.ExpiryMonth)
        //                        };
        //                        var tokenService = new StripeTokenService();
        //                        //tokenService.ApiKey = StripeData.StripePublishKey;
        //                        tokenService.ApiKey = stripePublickey;
        //                        StripeToken stripeToken = tokenService.Create(myToken);
        //                        var stripeCharge = tokenService.Create(myToken);
        //                        Token = stripeCharge.Id;

        //                        if (Token != null)
        //                        {
        //                            var currency = "";
        //                            var Invoicecurrency = context.InvoiceUserRegistrations.Where(i => i.EmailTo == userChk).Select(s => s.TradingCurrency).FirstOrDefault();

        //                            if (Invoicecurrency != null)
        //                            {
        //                                currency = Invoicecurrency.ToLower();
        //                            }
        //                            else
        //                            {
        //                                var Financecurrency = context.Currencies.Select(d => d.CurrencyType).FirstOrDefault(); // context.UserRegistrations.Where(i => i.Email == userChk).Select(s => s.Currency).FirstOrDefault();
        //                                currency = Financecurrency.ToString().ToLower();
        //                            }
        //                            var myCharge = new StripeChargeCreateOptions
        //                            {
        //                                //convert the amount of £12.50 to pennies i.e. 1250
        //                                Amount = Convert.ToInt32(obj.PaidAmount) * 100,
        //                                Currency = currency,                                //trancation depends on Currency 
        //                                Description = "Description for test charge",
        //                                Source = new StripeSourceOptions
        //                                {
        //                                    TokenId = Token,
        //                                }
        //                            };
        //                            //  var stripesecretkey = "sk_test_P0XjY4QBs5eYKF2qEF0DbQfz";
        //                            var chargeService = new StripeChargeService(stripesecretkey);
        //                            var InvoicestripeCharge = chargeService.Create(myCharge);

        //                            if (InvoicestripeCharge.Status == "succeeded")
        //                            {

        //                                var InvoiceDetail = context.tblInvoiceDetails.Where(i => i.Id == obj.InvoiceId).FirstOrDefault();
        //                                InvoiceDetail.BalanceDue -= obj.PaidAmount;
        //                                InvoiceDetail.DepositePayment += obj.PaidAmount;
        //                                InvoiceDetail.PaymentDate = DateTime.Now;
        //                                InvoiceDetail.StripeJVID = obj.PaidJVID;
        //                                context.SaveChanges();
        //                                dbContextTransaction.Commit();
        //                                return 1;
        //                            }
        //                            else
        //                            {
        //                                dbContextTransaction.Rollback();
        //                                return 0;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return 0; //data not exist in stripekey table
        //                    }
        //                }
        //            }
        //        }
        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //#endregion




    }
}


