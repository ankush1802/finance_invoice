using AutoMapper;
using KF.Dto.Modules.Common;
using KF.Dto.Modules.Finance;
using KF.Entity;
using KF.Repo.Modules.Common;
using KF.Repo.Modules.Finance;
using KF.Repo.Modules.Invoice;
using KF.Utilities.Common;
using KF.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace KF.Web.Controllers
{
    public class AccountController : Controller
    {

        #region Finance
        // GET: Account

        public ActionResult Web(string uname, string password)
        {
            using (var userRepository = new AccountRepository())
            {
                var user = userRepository.CheckAuthorizedUser(uname, password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    if (user.RoleId == 1)
                        return RedirectToAction("UploadStatement", "Accounting");
                    else
                        return RedirectToAction("UploadStatement", "Accounting");

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
        }

        #region Web Finance Login
        public ActionResult Login()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                Session["panel_login_info"] = null;
                Response.Cookies.Clear();
                var c = new HttpCookie("SelectedActiveUser");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
                using (var repo = new AccountRepository())
                {
                    var user = repo.CheckAuthorizedUser(login.Username, login.Password);

                    if (user.Email == "NoCompanyExists")
                    {
                        ModelState.AddModelError("ErrorMsg", "No Company Exist For This User !!");
                        return View(login);
                    }
                    if (user.Email == "User Unlink the Accountant")
                    {
                        //ModelState.AddModelError("ErrorMsg", "Currently your account is expired.");
                        //return View(login);
                    }
                    else if (user.Email == "Invalid login credentials.")
                    {
                        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        return View(login);

                    }
                    else if (user != null)
                    {
                        //if (user.IsVerified == true && user.IsUnlink == true && user.RoleId == 3)
                        //{
                        //    ModelState.AddModelError("ErrorMsg", "Your have unlink your account!!");
                        //    return View(login);
                        //}
                        if (user.IsEmployeeActivated == false && user.RoleId == 4)
                        {
                            ModelState.AddModelError("ErrorMsg", "Your account has been disabled!!");
                            return View(login);
                        }
                        else if (user.IsVerified == true && user.RoleId != 1)
                        {
                            DateTime UserModifiedDate = Convert.ToDateTime(user.ModifiedDate);
                            DateTime CurrentDate = DateTime.Now;
                            var days = (CurrentDate - UserModifiedDate).TotalDays;
                            if (user.IsPaid == null || user.IsPaid == false)
                            {
                                if (days > 30)
                                {
                                    ModelState.AddModelError("ErrorMsg", "Your Trial Period is over!!");
                                    return View(login);
                                }
                            }
                            FormsAuthentication.SetAuthCookie(user.Username, login.RememberMe);
                            #region Insert Profile update Log
                            string UpdateDetails = "User last logged in details";
                            repo.UserActivityLog(user.Id, true, false, false, UpdateDetails);
                            #endregion
                            if (user.RoleId == 4)
                            {
                                return RedirectToAction("ActiveUser", "Accounting");
                            }
                            else if (user.IsUnlink == true)
                            {
                                return RedirectToAction("Home", "Kippinstore");
                            }
                            return RedirectToAction("UploadStatement", "Accounting");


                        }
                        //else if (user.IsVerified == true && user.IsPaid == true && (user.RoleId == 1 || user.RoleId == 3))
                        else if (user.IsVerified == true && user.IsPaid == true && user.RoleId == 1)
                        {
                            FormsAuthentication.SetAuthCookie(user.Username, login.RememberMe);
                            #region Insert Profile update Log
                            string UpdateDetails = "User last logged in details";
                            repo.UserActivityLog(user.Id, true, false, false, UpdateDetails);
                            #endregion
                            if (user.RoleId == 1)
                            {
                                return RedirectToAction("ActiveUser", "Accounting", new { Selectuser = false });
                            }

                            else
                                return RedirectToAction("Dashboard", "Accounting");
                        }
                        else if (user.IsVerified == false || user.IsVerified == null)
                        {
                            ModelState.AddModelError("ErrorMsg", "Please Verify your mail first.  !!");
                            return View(login);
                        }
                        else if (user.IsVerified == true && user.IsPaid == false)
                        {
                            var bytes = Encoding.UTF8.GetBytes(user.Id.ToString());
                            var base64 = Convert.ToBase64String(bytes);
                            return RedirectToAction("Payment", "Account", new { id = base64 });
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMsg", "Login credentials are wrong !!");
                        return View(login);
                    }
                }
            }
            else
                ModelState.AddModelError("ErrorMsg", "Please fill both Username and Password !!");
            return View(login);
        }

        public ActionResult Userlogin()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return View();
        }
        [HttpPost]
        public ActionResult Userlogin(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                Session["panel_login_info"] = null;
                Response.Cookies.Clear();
                var c = new HttpCookie("SelectedActiveUser");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
                using (var userRepository = new AccountRepository())
                {
                    var user = userRepository.CheckAuthorizedUser(login.Username, login.Password);
                    if (user.Email == "User Unlink the Accountant")
                    {
                        ModelState.AddModelError("ErrorMsg", "Currently your account is expired.");
                        return View(login);
                    }
                    else if (user.Email == "Invalid credential")
                    {
                        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        return View(login);

                    }
                    else if (user != null)
                    {
                        if (user.IsVerified == true && user.IsUnlink == true && user.RoleId == 3)
                        {
                            //ModelState.AddModelError("ErrorMsg", "Your have unlink your account!!");
                            //return View(login);
                            return RedirectToAction("Home", "Kippinstore");
                        }
                        else if (user.IsVerified == true && user.RoleId != 1)
                        {
                            DateTime UserModifiedDate = Convert.ToDateTime(user.ModifiedDate);
                            DateTime CurrentDate = DateTime.Now;
                            var days = (CurrentDate - UserModifiedDate).TotalDays;
                            if (user.IsPaid == null || user.IsPaid == false)
                            {
                                if (days > 30)
                                {
                                    ModelState.AddModelError("ErrorMsg", "Your Trial Period is over!!");
                                    return View(login);
                                }
                            }
                            FormsAuthentication.SetAuthCookie(user.Username, login.RememberMe);
                            return RedirectToAction("UploadStatement", "Accounting");


                        }
                        //else if (user.IsVerified == true && user.IsPaid == true && (user.RoleId == 1 || user.RoleId == 3))
                        else if (user.IsVerified == true && user.IsPaid == true && user.RoleId == 1)
                        {
                            FormsAuthentication.SetAuthCookie(user.Username, login.RememberMe);
                            if (user.RoleId == 1)
                            {
                                return RedirectToAction("ActiveUser", "Accounting", new { Selectuser = false });
                            }

                            else
                            {
                                #region Insert Profile update Log
                                string UpdateDetails = "User last logged in details";
                                userRepository.UserActivityLog(user.Id, true, false, false, UpdateDetails);
                                // UserUpdateProfileLog.InsertLog(user.Id, true, false, false, UpdateDetails);
                                #endregion
                                return RedirectToAction("Dashboard", "Kippin");
                            }

                        }
                        else if (user.IsVerified == false || user.IsVerified == null)
                        {
                            ModelState.AddModelError("ErrorMsg", "Please Verify your mail first.  !!");
                            return View(login);
                        }
                        else if (user.IsVerified == true && user.IsPaid == false)
                        {
                            var bytes = Encoding.UTF8.GetBytes(user.Id.ToString());
                            var base64 = Convert.ToBase64String(bytes);

                            if (user.RoleId == 2)
                                return RedirectToAction("MobileUserPayment", "Account", new { id = base64 });
                            else if (user.RoleId == 3)
                                return RedirectToAction("UserwithanaccountantPayment", "Account", new { id = base64 });
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("ErrorMsg", "Login credentials are wrong !!");
                        return View(login);
                    }
                }
            }
            else
                ModelState.AddModelError("ErrorMsg", "Please fill both Username and Password !!");
            return View(login);
        }
        #endregion

        #region Web Finance Registration
        public ActionResult Register()
        {
            RegisterViewModel accountant = new RegisterViewModel();
            accountant.ProvinceList = ClsDropDownList.PopulateProvince();
            //accountant.CountryCodeList = ClsDropDownList.PopulateCountryCode();
            return View(accountant);
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel accountant)
        {
            //if (!string.IsNullOrEmpty(accountant.Country1))
            //{
            //    if (accountant.Country1.ToLower() != "canada")
            //    {
            //        ModelState.AddModelError("CommanError", "Kippin-Finance registration not available for your country.");
            //    }

            //}
            //else
            //{
            //    ModelState.AddModelError("CommanError", "Please share your location.");
            //}
            accountant.CountryId = 1;//For canada
            accountant.LicenseId = 1;
            accountant.CurrencyId = 1;
            accountant.SectorId = 1;
            accountant.RoleId = 1; //For accountant
            accountant.IsPaid = false;

            if (ModelState.IsValid)
            {
                try
                {
                    using (var repo = new AccountRepository())
                    {
                        if (repo.UserEmailExistCheck(accountant.Email))
                        {
                            ModelState.AddModelError("Email", "Email already exists please try another");
                        }
                        else if (repo.UsernameExistCheck(accountant.Username))
                        {
                            ModelState.AddModelError("Username", "Username already exists please try another");
                        }
                        else
                        {
                            Mapper.CreateMap<RegisterViewModel, UserRegistrationDto>();
                            var userDetail = Mapper.Map<UserRegistrationDto>(accountant);
                            userDetail.MobileNumber = accountant.AreaCode + "-" + userDetail.MobileNumber;
                            var addUser = repo.AddUser(userDetail);
                            if (addUser.Id > 0)
                            {
                                var bytes = Encoding.UTF8.GetBytes(addUser.Id.ToString());
                                var base64 = Convert.ToBase64String(bytes);
                                //Read Email Message body from html file
                                string html = System.IO.File.ReadAllText(Server.MapPath("~/EmailFormats/EmailTemplate.html"));
                                //Replace Username
                                html = html.Replace("#Firstname", accountant.FirstName);
                                html = html.Replace("#LastName", accountant.LastName);
                                //Building confirmation URL
                                string websiteUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/ConfirmEmail/" + base64;
                                html = html.Replace("#URL", websiteUrl);
                                //Send a confirmation Email and send to a page where a message will display plz chk your email
                                SendMailModelDto _objModelMail = new SendMailModelDto();
                                _objModelMail.To = accountant.Email;
                                _objModelMail.Subject = "Confirmation mail from Kippin-Finance.";
                                _objModelMail.MessageBody = html;
                                var mailSent = Sendmail.SendEmail(_objModelMail);
                                if (mailSent == true)
                                {
                                    return RedirectToAction("Checkmail");
                                }
                                else
                                {
                                    var rollBackTransaction = repo.RollBackUserCreation(userDetail.Id);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("CommanError", "Unable connect to the server. Please try again later.");
                            }

                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("CommanError", "Unable connect to the server. Please try again later.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Unable connect to the server. Please try again later.");
            }
            accountant.ProvinceList = ClsDropDownList.PopulateProvince();
            // accountant.CountryCodeList = ClsDropDownList.PopulateCountryCode();
            return View(accountant);

        }

        public ActionResult Checkmail()
        {
            return View();
        }
        #endregion

        #region Confirm Email
        public ActionResult ConfirmEmail(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            ViewBag.UserId = id;
            using (var repo = new AccountRepository())
            {
                if (!repo.ConfirmEmail(userId))
                {
                    return RedirectToAction("Register");
                }
            }
            return View();
        }

        public ActionResult ConfirmEmailForMobileUser(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            ViewBag.UserId = id;
            using (var repo = new AccountRepository())
            {
                if (!repo.ConfirmEmail(userId))
                {
                    return RedirectToAction("Register");
                }
            }
            return View();
        }
        #endregion

        #region Payment

        #region Web payment / Accounatant Payment Process
        public ActionResult Payment(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            PaymentDetailViewModel _paymentdetail = new PaymentDetailViewModel();
            _paymentdetail.UserId = userId;
            _paymentdetail.LicenseId = 1;
            return View(_paymentdetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(PaymentDetailViewModel _detail)
        {
            if (!ModelState.IsValid)
            {
                return View(_detail);
            }
            var bytes = Encoding.UTF8.GetBytes(_detail.UserId.ToString());
            var base64UserId = Convert.ToBase64String(bytes);
            using (var repo = new AccountRepository())
            {

                if (repo.CheckUserPaidStatus(_detail.UserId))
                {
                    ViewBag.PaymentAlready = "Done";
                    return View(_detail);
                }
                else
                {
                    _detail.Price = repo.UserPaymentByRoldeId(1);
                    PaymentRepository objPayment = new PaymentRepository();
                    var chargeId = objPayment.ProcessUserActivationPayment(_detail.Token, Convert.ToInt32(_detail.Price));
                    if (chargeId != null)
                    {
                        try
                        {
                            if (repo.ActivateUserAccount(Convert.ToInt32(_detail.UserId)))
                            {
                                if (!repo.InserUserPaymentDetails(Convert.ToInt32(_detail.UserId), chargeId, _detail.Price, _detail.FirstName))
                                    return View(_detail);
                                else
                                {
                                    Mapper.CreateMap<PaymentDetailViewModel, AddCardDetailDto>();
                                    var cardData = Mapper.Map<AddCardDetailDto>(_detail);
                                    var InsertCardDetails = repo.AddUserCardDetals(cardData);
                                    return RedirectToAction("PaymentSucess", "Account", new { ReferenceNumber = base64UserId });
                                }
                            }

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            return View(_detail);
        }

        [HttpGet]
        public ActionResult PaymentSucess(string ReferenceNumber)
        {
            TermsAndCondition obj = new TermsAndCondition();
            obj.UserId = ReferenceNumber;
            return View(obj);
        }

        [HttpPost]
        public ActionResult PaymentSucess(TermsAndCondition obj)
        {
            if (obj.TermAndCondition == true)
            {
                using (var repo = new AccountRepository())
                {
                    var data = Convert.FromBase64String(obj.UserId);
                    int UserId = Convert.ToInt32(Encoding.UTF8.GetString(data));
                    repo.UpdateUserTermsAndConditionDetails(UserId);
                }
            }
            return RedirectToAction("Login");
        }

        public ActionResult AccountantAgreement(string Status)
        {
            if (!string.IsNullOrEmpty(Status))
            {
                ViewBag.EmailStatus = Status;
            }
            return View();
        }

        public ActionResult SendAccountantAgreement(string Email)
        {
            string html = System.IO.File.ReadAllText(Server.MapPath("~/EmailFormats/AccountantAgreement.html"));

            SendMailModelDto _objModelMail = new SendMailModelDto();
            _objModelMail.To = Email;
            _objModelMail.Subject = "Kippin-Finance Accountant Agreement";
            _objModelMail.MessageBody = html;
            var mailSent = Sendmail.SendEmail(_objModelMail);
            if (mailSent == true)
            {
                return RedirectToAction("AccountantAgreement", new { Status = "Sent" });
            }

            return RedirectToAction("AccountantAgreement", new { Status = "Fail" });
        }
        #endregion

        #region Mobile payment User As an Accountant
        //Payment
        public ActionResult MobileUserPayment(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            PaymentDetailViewModel _paymentdetail = new PaymentDetailViewModel();
            _paymentdetail.UserId = userId;
            _paymentdetail.LicenseId = 2;
            return View(_paymentdetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MobileUserPayment(PaymentDetailViewModel _detail)
        {
            //_detail.Price =Convert.ToDecimal(27.12);
            if (!ModelState.IsValid)
            {
                return View(_detail);
            }
            var bytes = Encoding.UTF8.GetBytes(_detail.UserId.ToString());
            var base64UserId = Convert.ToBase64String(bytes);

            using (var repo = new AccountRepository())
            {
                if (repo.CheckUserPaidStatus(_detail.UserId))
                {
                    ViewBag.PaymentAlready = "Done";
                    return View(_detail);
                }
                else
                {
                    //create stripe subscrition
                    using (var paymentRepo = new PaymentRepository())
                    {
                        try
                        {
                            var userData = UserData.GetUserData(_detail.UserId);
                            if (paymentRepo.CreateCustomerSubscription(userData.Email, userData.Username, _detail.CardNumber, _detail.ExpiryMonth.ToString(), _detail.ExpiryYear.ToString(), _detail.CVV.ToString(), DateTime.Now.AddSeconds(5)) == true)
                            {
                                return RedirectToAction("MobileUserPaymentSucess", "Account", new { ReferenceNumber = base64UserId });
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrorMesssage = ex.Message;
                        }
                    }

                    #region Old Method
                    // _detail.Price = repo.UserPaymentByRoldeId(2);
                    // PaymentRepository objPayment = new PaymentRepository();

                    //   var chargeId = objPayment.ProcessUserActivationPayment(_detail.Token, Convert.ToInt32(_detail.Price));
                    // if (chargeId != null)
                    // {
                    //try
                    //{
                    //    if (repo.ActivateUserAccount(Convert.ToInt32(_detail.UserId)))
                    //    {
                    //        if (!repo.InserUserPaymentDetails(Convert.ToInt32(_detail.UserId), chargeId, _detail.Price, _detail.FirstName))
                    //            return View(_detail);
                    //        else
                    //        {
                    //            Mapper.CreateMap<PaymentDetailViewModel, AddCardDetailDto>();
                    //            var cardData = Mapper.Map<AddCardDetailDto>(_detail);
                    //            var InsertCardDetails = repo.AddUserCardDetals(cardData);
                    //            return RedirectToAction("MobileUserPaymentSucess", "Account", new { ReferenceNumber = base64UserId });
                    //        }
                    //    }
                    //}
                    //catch (Exception)
                    //{
                    //    throw;
                    //}
                    // }
                    #endregion
                
                }
            }
            return View(_detail);
        }
        public ActionResult MobileUserPaymentSucess(string ReferenceNumber)
        {
            TermsAndCondition obj = new TermsAndCondition();
            obj.UserId = ReferenceNumber;
            return View(obj);
        }
        #endregion

        #region Mobile payment User with an Accountant
        //Payment
        public ActionResult UserwithanaccountantPayment(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            PaymentDetailViewModel _paymentdetail = new PaymentDetailViewModel();
            _paymentdetail.UserId = userId;
            _paymentdetail.LicenseId = 2;
            return View(_paymentdetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserwithanaccountantPayment(PaymentDetailViewModel _detail)
        {
            //_detail.Price =Convert.ToDecimal(27.12);
            if (!ModelState.IsValid)
            {
                return View(_detail);
            }
            var bytes = Encoding.UTF8.GetBytes(_detail.UserId.ToString());
            var base64UserId = Convert.ToBase64String(bytes);
            using (var repo = new AccountRepository())
            {
                if (repo.CheckUserPaidStatus(_detail.UserId))
                {
                    ViewBag.PaymentAlready = "Done";
                    return View(_detail);
                }
                else
                {
                    //create stripe subscrition
                    using (var paymentRepo = new PaymentRepository())
                    {
                        try
                        {
                            var userData = UserData.GetUserData(_detail.UserId);
                            if (paymentRepo.CreateCustomerSubscription(userData.Email, userData.Username, _detail.CardNumber, _detail.ExpiryMonth.ToString(), _detail.ExpiryYear.ToString(), _detail.CVV.ToString(), DateTime.Now.AddSeconds(5)) == true)
                            {
                                return RedirectToAction("UserwithanaccountantPaymentSuccess", "Account", new { ReferenceNumber = base64UserId });
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrorMesssage = ex.Message;
                        }
                    }

                    #region Old Method
                    //_detail.Price = repo.UserPaymentByRoldeId(3);
                    //PaymentRepository objPayment = new PaymentRepository();
                    //var chargeId = objPayment.ProcessUserActivationPayment(_detail.Token, Convert.ToInt32(_detail.Price));
                    //if (chargeId != null)
                    //{
                    //    try
                    //    {
                    //        if (repo.ActivateUserAccount(Convert.ToInt32(_detail.UserId)))
                    //        {
                    //            if (!repo.InserUserPaymentDetails(Convert.ToInt32(_detail.UserId), chargeId, _detail.Price, _detail.FirstName))
                    //                return View(_detail);
                    //            else
                    //            {
                    //                Mapper.CreateMap<PaymentDetailViewModel, AddCardDetailDto>();
                    //                var cardData = Mapper.Map<AddCardDetailDto>(_detail);
                    //                var InsertCardDetails = repo.AddUserCardDetals(cardData);
                    //                return RedirectToAction("UserwithanaccountantPaymentSuccess", "Account", new { ReferenceNumber = base64UserId });
                    //            }
                    //        }
                    //    }
                    //    catch (Exception)
                    //    {
                    //        throw;
                    //    }
                    //}
                    #endregion
                  
                }
            }
            return View(_detail);
        }
        public ActionResult UserwithanaccountantPaymentSuccess(string ReferenceNumber)
        {
            TermsAndCondition obj = new TermsAndCondition();
            obj.UserId = ReferenceNumber;
            return View(obj);
        }
        #endregion

        #endregion

        #region Activate TrialMode
        [HttpGet]
        public ActionResult UserwithanaccountantTrialMode(string id)
        {
            PaymentDetailViewModel obj = new PaymentDetailViewModel();
            try
            {

                using (var db = new KFentities())
                {
                    var data = Convert.FromBase64String(id);
                    int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
                    var userData = db.UserRegistrations.Where(s => s.Id == userId).FirstOrDefault();
                    if (userData != null)
                    {
                        obj.UserId = userData.Id;
                        if (userData.IsVerified == true && userData.IsTrial == true)
                        {
                            return RedirectToAction("Userlogin");
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult UserwithanaccountantTrialMode(PaymentDetailViewModel obj)
        {
            using (var db = new KFentities())
            {
                var userData = db.UserRegistrations.Where(s => s.Id == obj.UserId).FirstOrDefault();
                if (userData != null)
                {
                    userData.IsVerified = true;
                    userData.IsTrial = true;
                    if (userData.ModifiedDate == null)
                    {
                        userData.ModifiedDate = DateTime.Now;
                    }
                    //create stripe subscrition
                    using (var repo = new PaymentRepository())
                    {
                        try
                        {
                            if (repo.CreateCustomerSubscription(userData.Email, userData.Username, obj.CardNumber, obj.ExpiryMonth.ToString(), obj.ExpiryYear.ToString(), obj.CVV.ToString(), DateTime.Now.AddMonths(1)) == true)
                            {
                                db.SaveChanges();
                                return RedirectToAction("TrialMode");
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrorMesssage = ex.Message;
                        }

                    }

                }
            }
            return View();
        }

        public ActionResult TrialMode()
        {
            return View();
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            var userdata = UserData.GetCurrentUserData();
            Response.Cookies.Clear();
            var c = new HttpCookie("SelectedActiveUser");
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            if (userdata.RoleId == 1 || userdata.RoleId == 4)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Userlogin", "Account");
            }
        }
        #endregion

        #region Profile Update
        [HttpGet]
        public ActionResult EditProfile()
        {
            using (var userRepository = new WebUserRepository())
            {
                EditProfile accountant = new EditProfile();
                var userData = UserData.GetCurrentUserData();
                Mapper.CreateMap<UserRegistration, EditProfile>();
                Mapper.Map(userData, accountant);
                var mobile = accountant.MobileNumber.Split('-');
                accountant.AreaCode = mobile[0].ToString();
                accountant.MobileNumber = mobile[1].ToString();
                accountant.OwnershipList = userRepository.GetOwnershipList();
                accountant.IndustryList = userRepository.GetIndustryList();
                accountant.ProvinceList = ClsDropDownList.PopulateProvince();
                return View(accountant);
            }

        }
        //Accountant
        [HttpPost]
        public ActionResult UpdateProfile(EditProfile accountant, HttpPostedFileBase ProfileImage)
        {
            if (accountant.RoleId == 1)
            {
                accountant.OwnershipId = 0;
            }
            using (var userRepository = new AccountRepository())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (ProfileImage != null)
                        {
                            #region Save Profile Image
                            string folder = Server.MapPath("~/ProfileImage/" + accountant.Id + "/");

                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                            var filePath = folder + datetime + ".jpg";
                            ProfileImage.SaveAs(filePath);
                            accountant.ProfileImage = "/ProfileImage/" + accountant.Id + "/" + datetime + ".jpg";

                            #endregion
                        }

                        UserRegistrationDto obj = new UserRegistrationDto();
                        Mapper.CreateMap<EditProfile, UserRegistrationDto>();
                        Mapper.Map(accountant, obj);
                        obj.MobileNumber = accountant.AreaCode + "-" + accountant.MobileNumber;
                        var UpdateUser = userRepository.UpdateUserProfile(obj);
                        if (UpdateUser == true)
                        {
                            TempData["SuccessUpdate"] = "Success";
                            return RedirectToAction("EditProfile", new { id = accountant.Id });
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message;
                    }
                }

                var userData = UserData.GetUserData(accountant.Id);
                Mapper.CreateMap<UserRegistration, EditProfile>();
                Mapper.Map(userData, accountant);
                accountant.OwnershipList = userRepository.GetOwnershipList();
                accountant.ProvinceList = ClsDropDownList.PopulateProvince();
                accountant.IndustryList = userRepository.GetIndustryList();
                return RedirectToAction("EditProfile");
            }


        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel Obj = new ChangePasswordViewModel();
            return View(Obj);

        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel Obj)
        {
            using (var context = new KFentities())
            {
                if (ModelState.IsValid)
                {
                    var userData = UserData.GetCurrentUserData();

                    var user = context.UserRegistrations.Where(i => i.Id == userData.Id).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Password == Obj.OldPassword)
                        {
                            user.Password = Obj.NewPassword.Trim();
                            #region Insert Profile update Log
                            string UpdateDetails = "Old Password = " + Obj.OldPassword + " - New Password = " + Obj.NewPassword.Trim() + " by user itself.";
                            using (var repo = new AccountRepository())
                            {
                                repo.UserActivityLog(userData.Id, false, true, false, UpdateDetails);
                            }

                            #endregion
                            context.SaveChanges();
                            return RedirectToAction("PasswordUpdated", new { password = Obj.NewPassword });
                        }
                    }

                    ModelState.AddModelError("DefaultMsg", "Invalid Email & Password.");
                }
                return View(Obj);
            }
        }
        [Authorize]
        public ActionResult PasswordUpdated(string password)
        {
            FormsAuthentication.SignOut();
            ViewBag.Success = "Password Updated Successfully. Now new password is " + password;
            return View();
        }

        #endregion


        public ActionResult Agreement(string Status)
        {
            if (!string.IsNullOrEmpty(Status))
            {
                ViewBag.EmailStatus = Status;
            }
            return View();
        }
        //SendAgreement
        public ActionResult SendAgreement(string Email)
        {
            string html = System.IO.File.ReadAllText(Server.MapPath("~/Agreement.html"));

            SendMailModelDto _objModelMail = new SendMailModelDto();
            _objModelMail.To = Email;
            _objModelMail.Subject = "Kippin-Finance Agreement";
            _objModelMail.MessageBody = html;
            var mailSent = Sendmail.SendEmail(_objModelMail);
            if (mailSent == true)
            {
                return RedirectToAction("Agreement", new { Status = "Sent" });
            }

            return RedirectToAction("Agreement", new { Status = "Fail" });
        }
        #endregion

        #region Invoice

        #region Logout
        public ActionResult InvoiceLogout()
        {
            var userdata = UserData.GetCurrentUserData();
            Response.Cookies.Clear();
            var c = new HttpCookie("SelectedActiveUser");
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            //return RedirectToAction("InvoiceLogin", "Account");
            return RedirectToAction("", "");

        }
        #endregion



        /*Invoice Login*/
        public ActionResult InvoiceLogin()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return View();
        }

        [HttpPost]
        public ActionResult InvoiceLogin(InvoiceLoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                Session["panel_login_info"] = null;
                Response.Cookies.Clear();
                var c = new HttpCookie("SelectedActiveUser");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

                try
                {

                    string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(login);
                    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    string Url = DomailApiUrl + "InvoiceUserApi/Registration/InvoiceUser/Login";


                    var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                    using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        dynamic data = JObject.Parse(result);
                        var res = data.ResponseCode;
                        String un = data.Username;
                        if (res == 1)
                        {
                            using (var userRepository = new WebUserRepository())
                            {
                                var user = userRepository.CheckAuthorizedInvoiceUser(login.EmailTo, login.Password);

                                FormsAuthentication.SetAuthCookie(user.Username, login.RememberMe);
                                Session["invoiceuaer"] = user.Username;
                            }
                            // return RedirectToAction("InvoiceActiveUser", "Invoice", new { Selectuser = false });
                            return RedirectToAction("InvoiceCustomerListInvoiceNew", "Invoice");

                        }
                        else
                        {
                            ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        }
                    }

                }
                catch
                {
                    //MessageBox.Show("Unable to Connect to Server.");
                }

            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Please fill both Username and Password !!");
            }
            return View(login);
        }

        public ActionResult InvoiceRegister(int Id)
        {
            RegisterInvoiceViewModel invoice = new RegisterInvoiceViewModel();
            invoice.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            invoice.TradingCurrencyList = ClsDropDownList.PopulateCurrency_Guri();

            if (Id > 0)
            {
                using (var userRepository = new WebUserRepository())
                {
                    var user = userRepository.GetUserById(Id);

                    invoice.UserName = user.Username;
                    invoice.CompanyName = user.CompanyName;
                    invoice.ContactPerson = user.FirstName + " " + user.LastName;
                    invoice.BusinessNumber = user.BusinessNumber;

                    string[] cell = user.MobileNumber.Split('-');
                    invoice.AreaCode = cell[0];
                    invoice.MobileNumber = cell[1];


                    invoice.CorporateStreet = user.CorporationAddress;
                    invoice.CorporateCity = user.City;
                    invoice.CorporatePostalCode = user.PostalCode;

                    invoice.ShippingStreet = user.CorporationAddress;
                    invoice.ShippingCity = user.City;
                    invoice.ShippingPostalCode = user.PostalCode;

                    invoice.EmailTo = user.Email;
                    invoice.TradingCurrencyId = 1;
                    invoice.Password = user.Password;

                    foreach (var itms in ClsDropDownList.PopulateIndustry())
                    {
                        var GoodsServices_Name = userRepository.GetGoodsById(Convert.ToInt32(user.IndustryId));

                        if (itms.Text == GoodsServices_Name)
                        {
                            invoice.GoodsservicesId = Convert.ToByte(itms.Value);
                            //obj.GoodsservicesId = Convert.ToInt32(itms.Value);
                        }
                    }
                }

                TempData["res"] = Id;
                Session["forinv"] = Id;

            }


            return View(invoice);
        }

        [HttpPost]
        public ActionResult InvoiceRegister(RegisterInvoiceViewModel invoice, HttpPostedFileBase ProfileImage)
        {
            // if (ModelState.IsValid)
            // {
            try
            {
                var filePath = "";
                string base64String = null;
                using (var context = new KFentities())
                {

                    try
                    {

                        if (ProfileImage == null)
                        {
                            //ProfileImage = "/Logos/invoicelogo.png";
                            #region Save Profile Image
                            string folder = Server.MapPath("~/EmailImages/");

                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                            filePath = folder + datetime + ".jpg";
                            //ProfileImage.SaveAs(filePath);

                            // string filepath= "/Logos/invoicelogo.png"images\image1.jpg";

                            //bitmapImage.Save(filepath,System.Drawing.Imaging.ImageFormat.Jpeg);



                            Bitmap b = new Bitmap(Server.MapPath("~/Logos/invoicelogo.png"));

                            b.Save(filePath);

                            invoice.ProfileImage = "/ProfileImage/" + invoice.UserId + "/" + datetime + ".jpg";

                            using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
                            {
                                using (MemoryStream m = new MemoryStream())
                                {
                                    image.Save(m, image.RawFormat);
                                    byte[] imageBytes = m.ToArray();
                                    base64String = Convert.ToBase64String(imageBytes);
                                }
                            }
                            #endregion
                        }


                        if (ProfileImage != null)
                        {
                            #region Save Profile Image
                            string folder = Server.MapPath("~/EmailImages/");

                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                            filePath = folder + datetime + ".jpg";
                            ProfileImage.SaveAs(filePath);
                            invoice.ProfileImage = "/ProfileImage/" + invoice.UserId + "/" + datetime + ".jpg";

                            using (System.Drawing.Image image = System.Drawing.Image.FromFile(filePath))
                            {
                                using (MemoryStream m = new MemoryStream())
                                {
                                    image.Save(m, image.RawFormat);
                                    byte[] imageBytes = m.ToArray();
                                    base64String = Convert.ToBase64String(imageBytes);
                                }
                            }
                            #endregion
                        }

                        invoice.ProfileImage = base64String;

                        RegisterInvoiceViewModel1 obj = new RegisterInvoiceViewModel1();

                        obj.CompanyLogo = invoice.ProfileImage == null ? "" : invoice.ProfileImage;
                        obj.CompanyName = invoice.CompanyName == null ? "" : invoice.CompanyName;
                        obj.ContactPerson = invoice.ContactPerson == null ? "" : invoice.ContactPerson;
                        obj.CorporateAptNo = invoice.hdncorporateaptno == null ? "" : invoice.hdncorporateaptno;
                        obj.CorporateHouseNo = invoice.hdncorporatehouseno == null ? "" : invoice.hdncorporatehouseno;
                        obj.CorporateStreet = invoice.hdncorporatestreet == null ? "" : invoice.hdncorporatestreet;
                        obj.CorporateCity = invoice.hdncorporatecity == null ? "" : invoice.hdncorporatecity;
                        obj.CorporateState = invoice.hdncorporatestate == null ? "" : invoice.hdncorporatestate;
                        obj.CorporatePostalCode = invoice.hdncorporatepostalcode == null ? "" : invoice.hdncorporatepostalcode;
                        obj.ShippingAptNo = invoice.hdnshippingaptno == null ? "" : invoice.hdnshippingaptno;
                        obj.ShippingHouseNo = invoice.hdnshippinghouseno == null ? "" : invoice.hdnshippinghouseno;
                        obj.ShippingStreet = invoice.hdnshippingstreet == null ? "" : invoice.hdnshippingstreet;
                        obj.ShippingCity = invoice.hdnshippingcity == null ? "" : invoice.hdnshippingcity;
                        obj.ShippingState = invoice.hdnshippingstate == null ? "" : invoice.hdnshippingstate;
                        obj.ShippingPostalCode = invoice.hdnshippingpostalcode == null ? "" : invoice.hdnshippingpostalcode;
                        obj.GoodsType = Convert.ToString(invoice.GoodsservicesId);
                        obj.TradingCurrency = Convert.ToString(invoice.TradingCurrencyId);
                        obj.BusinessNumber = invoice.BusinessNumber == null ? "" : invoice.BusinessNumber;
                        obj.EmailTo = invoice.EmailTo == null ? "" : invoice.EmailTo;
                        obj.EmailCc = invoice.hdnemailCC == null ? "" : invoice.hdnemailCC;
                        obj.AreaCode = invoice.AreaCode == null ? "" : invoice.AreaCode;
                        obj.MobileNumber = invoice.AreaCode + '-' + invoice.MobileNumber;
                        obj.Website = invoice.Website == null ? "" : invoice.Website;
                        obj.UserName = invoice.UserName == null ? "" : invoice.UserName;
                        obj.Password = invoice.Password == null ? "" : invoice.Password;
                        obj.TradingCurrency = invoice.ddl == null ? "" : invoice.ddl;




                        string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);


                        string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        string Url = DomailApiUrl + "InvoiceUserApi/Registration/User/InvoiceRegistration";
                        var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            dynamic data = JObject.Parse(result);
                            var res = data.ResponseCode;
                            //var userdata = new JavaScriptSerializer().Deserialize<ApiResult>(result);

                            if (res == 1)
                            {
                                if ((Session["forinv"]) != null)
                                {
                                    TempData["InvoiceMailConBox"] = "Success";
                                }
                                TempData["InvoiceRegistersuccessfullycreated"] = "Success";
                                // return RedirectToAction("InvoiceRegister", "Account");
                            }
                            else if (res == 2)
                            {
                                TempData["InvoiceRegisterAlready"] = "Success";
                                // return RedirectToAction("InvoiceRegister", "Account");
                            }
                        }



                    }
                    catch
                    {
                        //MessageBox.Show("Unable to Connect to Server.");
                    }

                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("CommanError", "Unable connect to the server. Please try again later.");
            }
            // }
            invoice.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            invoice.TradingCurrencyList = ClsDropDownList.PopulateCurrency_Guri();
            return View(invoice);
        }

        public ActionResult InvoiceForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InvoiceForgotPassword(ChangePasswordViewModel invoice)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string Email = invoice.Email;

                    string password = Sendmail.GenerateRandomString(6);
                    var userrepository = new InvoiceUserRegistrationRepository();

                    var username = userrepository.ForgotPassword(Email, password);
                    if (!string.IsNullOrEmpty(username))
                    {
                        SendMailModelDto objMail = new SendMailModelDto();
                        objMail.From = ConfigurationSettings.AppSettings["smtpUserName"];
                        objMail.To = Email;
                        objMail.Subject = "Kippin Invoice Account Details";
                        objMail.MessageBody = "<p style='FONT-WEIGHT: 700; FONT-SIZE: 12pt' face='Arial' color='#333'>Your New Password: " + password + " and Username: " + username + "</p>";
                        Sendmail.SendEmail(objMail);
                        TempData["invoiceforgotpassword"] = "Success";



                        //return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                    }
                    else
                    {
                        TempData["invoiceforgotpassword"] = "EmailNotExist";
                        //return new ApiResponseDto { ResponseMessage = "Email doesn't exist", ResponseCode = (int)ApiStatusCode.Failure };
                    }

                }

            }
            catch (Exception)
            {
                ModelState.AddModelError("CommanError", "Unable connect to the server. Please try again later.");
            }
            return View();
        }

        public ActionResult ConfirmEmailForMobileInvoice(string id)
        {
            var data = Convert.FromBase64String(id);
            int userId = Convert.ToInt32(Encoding.UTF8.GetString(data));
            ViewBag.UserId = id;
            using (var repo = new AccountRepository())
            {
                if (!repo.ConfirmEmailForMobileInvoices(userId))
                {
                    return RedirectToAction("Register");
                }
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult InvoiceChangePassword()
        {
            ChangePasswordViewModel Obj = new ChangePasswordViewModel();
            return View(Obj);

        }
        [HttpPost]
        public ActionResult InvoiceChangePassword(ChangePasswordViewModel Obj)
        {
            using (var context = new KFentities())
            {
                if (ModelState.IsValid)
                {
                    var userData = UserData.GetCurrentInvoiceUserData();

                    var user = context.InvoiceUserRegistrations.Where(i => i.Id == userData.Id).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Password == Obj.OldPassword)
                        {
                            user.Password = Obj.NewPassword.Trim();
                            #region Insert Profile update Log
                            string UpdateDetails = "Old Password = " + Obj.OldPassword + " - New Password = " + Obj.NewPassword.Trim() + " by user itself.";
                            using (var repo = new AccountRepository())
                            {
                                repo.UserActivityLog(userData.Id, false, true, false, UpdateDetails);
                            }

                            #endregion
                            context.SaveChanges();
                            return RedirectToAction("InvoicePasswordUpdated", new { password = Obj.NewPassword });
                        }
                    }

                    ModelState.AddModelError("DefaultMsg", "Invalid Email & Password.");
                }
                return View(Obj);
            }
        }
        [Authorize]
        public ActionResult InvoicePasswordUpdated(string password)
        {
            FormsAuthentication.SignOut();
            ViewBag.Success = "Password Updated Successfully. Now new password is " + password;
            return View();
        }
        #endregion
    }
}