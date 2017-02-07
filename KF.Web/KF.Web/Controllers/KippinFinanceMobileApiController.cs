using AutoMapper;
using KF.Dto.Modules.Common;
using KF.Dto.Modules.Finance;
using KF.Dto.Modules.FinanceReport;
using KF.Entity;
using KF.Repo.Modules.Common;
using KF.Repo.Modules.Finance;
using KF.Utilities.Common;
using KF.Utilities.Finance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using YodleeBase;
using YodleeBase.model;

namespace KF.Web.Controllers
{
    [RoutePrefix("KippinFinanceApi")]
    public class KippinFinanceMobileApiController : ApiController
    {
        #region Account

        #region UserAsAnAccountantRegistration

        [Route("Account/User/UserAsAnAccountantRegistration")]
        [HttpPost]
        public ApiResponseDto UserAsAnAccountantRegistration(UserRegistrationDto ObjUserDetail)
        {
            try
            {
                using (var repo = new AccountRepository())
                {
                    if (ObjUserDetail != null)
                    {
                        //Exist email and username Check
                        var chkUsernameExisted = repo.UsernameExistCheck(ObjUserDetail.Username);
                        if (chkUsernameExisted == true)
                        {
                            return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.IsExist, ResponseMessage = "Username Already existed." };
                        }
                        var chkUserExisted = repo.UserEmailExistCheck(ObjUserDetail.Email);
                        if (chkUserExisted == true)
                        {
                            return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.IsExist, ResponseMessage = "User Already existed." };
                        }
                        else
                        {
                            ObjUserDetail.IsDeleted = false;
                            ObjUserDetail.RoleId = 2; // User as an accountant
                            ObjUserDetail.SectorId = 1;
                            ObjUserDetail.CurrencyId = 1;
                            ObjUserDetail.RoleId = 2;
                            ObjUserDetail.IsTrial = true;
                            ObjUserDetail.IsPaid = false;
                            ObjUserDetail.IsVerified = false;
                            ObjUserDetail.CreatedDate = DateTime.Now;
                            // Mapper.CreateMap<UserRegistrationDto, UserRegistration>();
                            //var userData = Mapper.Map<UserRegistration>(ObjUserDetail);
                            var userDetail = repo.AddUser(ObjUserDetail);

                            var bytes = Encoding.UTF8.GetBytes(userDetail.Id.ToString());
                            var base64 = Convert.ToBase64String(bytes);
                            //success
                            //Read Email Message body from html file
                            string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailFormats/EmailTemplate.html"));
                            //Replace Username
                            html = html.Replace("#FirstName", userDetail.FirstName);
                            html = html.Replace("#LastName", userDetail.LastName);
                            string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/MobileUserPayment/" + base64;
                            html = html.Replace("#URL", Url);

                            string TrialUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantTrialMode/" + base64;
                            html = html.Replace("#TrialURL", TrialUrl);
                            //Send a confirmation Email and send to a page where a message will display plz chk your email
                            SendMailModelDto _objModelMail = new SendMailModelDto();
                            _objModelMail.To = userDetail.Email;
                            _objModelMail.Subject = "Confirmation mail from Kippin-Finance";
                            _objModelMail.MessageBody = html;
                            var mailSent = Sendmail.SendEmail(_objModelMail);
                            if (mailSent == true)
                            {
                                return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Please check your mail for email confirmation.", UserId = userDetail.Id };
                            }
                            else
                            {
                                //var rollBackTransaction = repo.RollBackUserCreation(userDetail.Id);
                                return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Unable to create user due to internal server error", UserId = 0 };
                            }

                        }
                    }
                    else
                    {
                        return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Please provide data." };
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Failure" };
            }
        }

        #endregion

        #region User with an accountant registaration
        [Route("Account/User/RegisterWithAnAccountant/{privateKey}")]
        [HttpPut]
        public ApiResponseDto RegisterWithAnAccountant(UserRegistrationDto userDetails, string privateKey)
        {
            try
            {
                using (var userRepository = new AccountRepository())
                {
                    if (userDetails.IsUnlink == true)
                    {
                        userDetails.ModifiedDate = DateTime.Now;
                        userDetails.IsVerified = true;
                        if (userDetails.IsPaid != true)
                        {
                            userDetails.IsTrial = null;
                            userDetails.IsPaid = false;
                        }

                        userDetails.IsDeleted = false;
                        var addUserWithAnAccountant = userRepository.RegisterUserWithAnAccountant(userDetails);
                        if (addUserWithAnAccountant != null)
                        {
                            #region Send Mail
                            var bytes = Encoding.UTF8.GetBytes(addUserWithAnAccountant.Id.ToString());
                            var base64 = Convert.ToBase64String(bytes);
                            //success
                            //Read Email Message body from html file
                            string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailFormats/UserWithAnAccountantEmailTemplate.html"));
                            //Replace Username
                            html = html.Replace("#FirstName", addUserWithAnAccountant.FirstName);
                            html = html.Replace("#LastName", addUserWithAnAccountant.LastName);
                            string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantPayment/" + base64;
                            string TrialUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantTrialMode/" + base64;
                            html = html.Replace("#URL", Url);
                            html = html.Replace("#TrialURL", TrialUrl);
                            //Send a confirmation Email and send to a page where a message will display plz chk your email
                            SendMailModelDto _objModelMail = new SendMailModelDto();
                            _objModelMail.To = addUserWithAnAccountant.Email;
                            _objModelMail.Subject = "Confirmation mail from Kippin-Finance";
                            _objModelMail.MessageBody = html;

                            if (userDetails.IsPaid != true)
                            {
                                var mailSent = Sendmail.SendEmail(_objModelMail);
                                if (mailSent == true)
                                {
                                    return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Please check your mail.", UserId = addUserWithAnAccountant.Id };
                                }
                                else
                                {
                                    var rollBackTransaction = userRepository.RollBackUserCreation(addUserWithAnAccountant.Id);
                                    return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Unable to create user.", UserId = 0 };
                                }
                            }
                            else
                            {
                                return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Successfully Registered.", UserId = addUserWithAnAccountant.Id };
                            }

                            #endregion
                        }

                        //if (addUserWithAnAccountant > 0)
                        //    return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success, UserId = addUserWithAnAccountant };
                        //else
                        //    return new ApiResponseDto { ResponseMessage = "Unable to add user.", ResponseCode = (int)ApiStatusCode.Failure };
                    }
                    var chkUserExisted = userRepository.UsernameExistCheck(userDetails.Username);
                    if (chkUserExisted == true)
                    {
                        return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Username already exist" };
                    }
                    else
                    {
                        var chkValidPrivateKey = userRepository.ValidUserEmailAndPrivateKeyCheck(userDetails.Email, privateKey);
                        if (chkValidPrivateKey == false)
                        {
                            return new ApiResponseDto { ResponseMessage = "Invalid email and private key.", ResponseCode = (int)ApiStatusCode.Failure };
                        }
                        else
                        {
                            userDetails.ModifiedDate = DateTime.Now;
                            userDetails.IsVerified = true;
                            userDetails.IsTrial = null;
                            userDetails.IsPaid = false;
                            userDetails.IsDeleted = false;
                            var addUserWithAnAccountantData = userRepository.RegisterUserWithAnAccountant(userDetails);
                            if (addUserWithAnAccountantData != null)
                            {
                                #region Send Mail
                                var bytes = Encoding.UTF8.GetBytes(addUserWithAnAccountantData.Id.ToString());
                                var base64 = Convert.ToBase64String(bytes);
                                //Read Email Message body from html file
                                string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailFormats/UserWithAnAccountantEmailTemplate.html"));
                                //Replace Username
                                html = html.Replace("#FirstName", addUserWithAnAccountantData.FirstName);
                                html = html.Replace("#LastName", addUserWithAnAccountantData.LastName);
                                string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantPayment/" + base64;
                                html = html.Replace("#URL", Url);
                                string TrialUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantTrialMode/" + base64;
                                html = html.Replace("#TrialURL", TrialUrl);

                                //Send a confirmation Email and send to a page where a message will display plz chk your email
                                SendMailModelDto _objModelMail = new SendMailModelDto();
                                _objModelMail.To = addUserWithAnAccountantData.Email;
                                _objModelMail.Subject = "Confirmation mail from Kippin-Finance";
                                _objModelMail.MessageBody = html;
                                var mailSent = Sendmail.SendEmail(_objModelMail);
                                if (mailSent == true)
                                {
                                    return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Please check your mail.", UserId = addUserWithAnAccountantData.Id };
                                }
                                else
                                {
                                    //   var rollBackTransaction = userRepository.RollBackUserCreation(addUserWithAnAccountantData.Id);
                                    return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Unable to create user.", UserId = 0 };
                                }
                                #endregion
                            }
                            //if (addUserWithAnAccountant > 0)
                            //    return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success, UserId = addUserWithAnAccountant };
                            //else
                            //    return new ApiResponseDto { ResponseMessage = "Unable to add user.", ResponseCode = (int)ApiStatusCode.Failure };
                        }

                    }
                }
                return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Failure, ResponseMessage = "Unable to create user.", UserId = 0 };
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
        }
        #endregion

        #region User Login on app
        [Route("Account/User/Login")]
        [HttpPost]
        public UserRegistrationDto CheckAuthorizedUser(UserRegistrationDto userDetails)
        {
            try
            {
                using (var repo = new AccountRepository())
                {
                    if (userDetails.Password != null && userDetails.Username != null)
                    {
                        var user = repo.CheckAuthorizedUserForMobile(userDetails.Username, userDetails.Password);
                        if (user.IsUnlinkUser == true)
                        {
                            user.ResponseCode = (int)ApiStatusCode.NotExist;
                            user.ResponseMessage = "Your account is expired.";
                            return user;
                        }
                        else if (user.Email == "No User Existed")
                        {
                            return new UserRegistrationDto { ResponseMessage = "User doesn't existed.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                        }
                        else if (user.Email == "Invalid login credentials.")
                        {
                            return new UserRegistrationDto { ResponseMessage = "Invalid login credentials.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                        }
                        else if (user.Email == "Please verify your email.")
                        {
                            return new UserRegistrationDto { ResponseMessage = "Please verify your mail first.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                        }
                        else
                        {
                            if (user.RoleId == 1)
                            {
                                return new UserRegistrationDto { ResponseMessage = "Please login from web.", ResponseCode = (int)ApiStatusCode.Failure };
                            }
                            else
                            {
                                #region Insert Profile update Log
                                string UpdateDetails = "User last logged in details";
                                repo.UserActivityLog(user.Id, true, false, false, UpdateDetails);
                                #endregion
                                user.ResponseCode = (int)ApiStatusCode.Success;
                                user.ResponseMessage = "Success";
                                return user;
                            }

                        }
                    }
                    else
                        return new UserRegistrationDto { ResponseMessage = "Unable to login into app. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                }
            }
            catch (Exception)
            {
                return new UserRegistrationDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #region Check User Trial Period

        [Route("Account/User/CheckUserStatus/{UserId}")]
        [HttpGet]
        public string CheckUserStatus(int UserId)
        {
            string UserStatus = "Error";
            try
            {
                using (var repo = new AccountRepository())
                {
                    UserStatus = repo.CheckUserStatus(UserId);
                }
                return UserStatus;
            }
            catch (Exception)
            {
                return UserStatus;
            }
        }

        #endregion

        #region User as an accountant payment
        [Route("Account/User/Payment/{userId}/{tokenId}")]
        [HttpPost]
        public ApiResponseDto MakePayment(int userId, string tokenId)
        {
            try
            {
                if (!string.IsNullOrEmpty(tokenId))
                {
                    // int  Cost = 20;//static for user as an accountant
                    using (var userRepository = new AccountRepository())
                    {
                        try
                        {
                            decimal Cost = userRepository.UserPaymentByRoldeId(2);
                            PaymentRepository objPayment = new PaymentRepository();
                            var chargeId = objPayment.ProcessUserActivationPayment(tokenId, Cost);
                            var UserIsActive = userRepository.ActivateUserAccount(Convert.ToInt32(userId));
                            if (UserIsActive == true)
                            {
                                var PaymentDetailsInsert = userRepository.InserUserPaymentDetails(Convert.ToInt32(userId), chargeId, Cost, "Old Mobile payment Api");
                                return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                            }
                            else
                                return new ApiResponseDto { ResponseMessage = "Failure to activate User Account", ResponseCode = (int)ApiStatusCode.Failure, UserId = Convert.ToInt32(userId) };
                        }
                        catch (Exception)
                        {
                            return new ApiResponseDto { ResponseMessage = "Something went wrong", ResponseCode = (int)ApiStatusCode.Failure, UserId = Convert.ToInt32(userId) };
                        }
                    }
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Some Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Something went wrong. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
        }
        #endregion

        #region Autologin for app webview
        [Route("Account/User/Web/{uname}/{password}")]
        [HttpGet]
        public HttpResponseMessage WebPage(string uname, string password)
        {
            string host = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            var response = Request.CreateResponse(HttpStatusCode.Found);
            response.Headers.Location = new Uri(System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/Web?uname=" + uname + "&password=" + password);
            return response;
        }
        #endregion

        #region payment card entry
        [Route("Account/User/AddCardDetails")]
        [HttpPost]
        public ApiResponseDto AddCardDetails(AddCardDetailDto cardData)
        {
            try
            {
                using (var repo = new PaymentRepository())
                {
                    var InsertCardDetails = repo.AddCard(cardData);
                    if (InsertCardDetails == true)
                    {
                        //ok
                        return new ApiResponseDto { ResponseMessage = "Success.", ResponseCode = (int)ApiStatusCode.Success };
                    }
                    else
                    {
                        //error
                        return new ApiResponseDto { ResponseMessage = "Unable to add card details. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                    }
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Something went wrong. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
        }
        #endregion

        #region Forgot Password

        [Route("Account/User/ForgotPassword/{email}")]
        [HttpPut]
        public ApiResponseDto ForgotPassword(string email)
        {
            try
            {
                string password = Sendmail.GenerateRandomString(6);
                using (var repo = new AccountRepository())
                {
                    var username = repo.ForgotPassword(email, password);
                    if (!string.IsNullOrEmpty(username))
                    {
                        SendMailModelDto objMail = new SendMailModelDto();
                        objMail.From = ConfigurationSettings.AppSettings["smtpUserName"];
                        objMail.To = email;
                        objMail.Subject = "Kippin Account Details";
                        objMail.MessageBody = "<p style='FONT-WEIGHT: 700; FONT-SIZE: 12pt' face='Arial' color='#333'>Your New Password: " + password + " and Username: " + username + "</p>";
                        Sendmail.SendEmail(objMail);
                        return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                    }
                    else
                    {
                        return new ApiResponseDto { ResponseMessage = "Email doesn't exist", ResponseCode = (int)ApiStatusCode.Failure };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto { ResponseMessage = "Failure", ResponseCode = (int)ApiStatusCode.NullParameter };
            }

        }

        #endregion

        #region Get Terms and Condition
        [Route("Account/TermsAndConditions")]
        [HttpGet]
        public ApiResponseDto TermsAndConditions()
        {
            try
            {
                var terms = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailFormats/AgreementMobile.html"));

                return new ApiResponseDto { ResponseMessage = terms, ResponseCode = (int)ApiStatusCode.Success };
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable to connect to the server. Please try again after sometime.", ResponseCode = (int)ApiStatusCode.Failure };
            }

        }

        #endregion

        #region Send Terms and Condition
        [Route("Account/SendTermsAndConditions")]
        [HttpPost]
        public ApiResponseDto SendTermsAndConditions(SendMailModelDto _objModelMail)
        {
            try
            {
                string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailFormats/Agreement.html"));

                _objModelMail.Subject = "Kippin-Finance Agreement";
                _objModelMail.MessageBody = html;
                var mailSent = Sendmail.SendEmail(_objModelMail);
                if (mailSent == true)
                {
                    return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Failure", ResponseCode = (int)ApiStatusCode.Failure };
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable to connect to the server. Please try again after sometime.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #endregion

        #region MobileDropdownListing

        #region Get Province list
        [Route("MobileDropdownListing/Province")]
        [HttpGet]
        public List<ProvinceDto> GetProvinceList()
        {
            try
            {
                using (var repo = new DropdownListRepository())
                {
                    var provinceList = repo.GetProvinceList();
                    if (provinceList.Count > 0)
                    {
                        provinceList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        provinceList.ForEach(x => x.ResponseMessage = "Success");
                        return provinceList;
                    }
                    else
                        return new List<ProvinceDto> { new ProvinceDto { ResponseMessage = "No Province available", ResponseCode = (int)ApiStatusCode.Failure } };
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new List<ProvinceDto> { new ProvinceDto { ResponseMessage = "Unable connect to the server.Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }
        #endregion

        #region Get bank list
        [Route("MobileDropdownListing/BankList/{userid}/{bankType}")]
        [HttpGet]
        public List<BankListDto> GetbankList(int userid, int bankType)
        {
            try
            {
                using (var repo = new DropdownListRepository())
                {
                    var bankList = repo.GetBankList(userid, bankType);
                    if (bankList.Count > 0)
                    {
                        bankList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        bankList.ForEach(x => x.ResponseMessage = "Success");
                        return bankList;
                    }
                    else
                        return new List<BankListDto> { new BankListDto { ResponseMessage = "No bank available for you.", ResponseCode = (int)ApiStatusCode.Failure } };
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new List<BankListDto> { new BankListDto { ResponseMessage = "Unable connect to the server.Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }
        #endregion

        #region Get Classification list
        [Route("MobileDropdownListing/ClassificationByUserId/{userId}")]
        [HttpGet]
        public List<ClassificationDto> GetClassificationList(int userId)
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var classificationList = userRepository.GetClassificationListByUserId(userId);
                    if (classificationList.Count > 0)
                    {

                        classificationList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        classificationList.ForEach(x => x.ResponseMessage = "Success");
                        return classificationList;
                    }
                    else
                        return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = "No record found", ResponseCode = (int)ApiStatusCode.NoContent } };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        #endregion

        #region Get ownership list
        [Route("MobileDropdownListing/Ownership")]
        [HttpGet]
        public List<OwnershipDto> GetOwnershipList()
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var ownershipList = userRepository.GetOwnershipList();
                    if (ownershipList.Count > 0)
                    {
                        ownershipList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        ownershipList.ForEach(x => x.ResponseMessage = "Success");
                        return ownershipList;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<OwnershipDto> { new OwnershipDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Category List
        [Route("MobileDropdownListing/GetCategoryById/{id}")]
        [HttpGet]
        public CategoryDto GetCategoryById(int id)
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var data = userRepository.GetCategoryById(id);
                    if (data != null)
                    {
                        Mapper.CreateMap<Category, CategoryDto>();
                        var dataDto = Mapper.Map<CategoryDto>(data);
                        dataDto.ResponseCode = (int)ApiStatusCode.Success;
                        dataDto.ResponseMessage = "Success";
                        return dataDto;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new CategoryDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter };
            }
        }

        [Route("MobileDropdownListing/GetCategoryList")]
        [HttpGet]
        public List<CategoryDto> GetCategoryList()
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var categoryList = userRepository.GetCategoryList();
                    if (categoryList.Count > 0)
                    {

                        categoryList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        categoryList.ForEach(x => x.ResponseMessage = "Success");
                        return categoryList;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<CategoryDto> { new CategoryDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }

        }
        #endregion

        #region Get currency list
        [Route("MobileDropdownListing/Currency")]
        [HttpGet]
        public List<CurrencyDto> GetCurrencyList()
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var currencyList = userRepository.GetCurrencyList();
                    if (currencyList.Count > 0)
                    {

                        currencyList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        currencyList.ForEach(x => x.ResponseMessage = "Success");
                        return currencyList;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<CurrencyDto> { new CurrencyDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Get industry list
        [Route("MobileDropdownListing/Industry")]
        [HttpGet]
        public List<IndustryListDto> GetIndustryList()
        {
            try
            {
                using (var userRepository = new DropdownListRepository())
                {
                    var industry = userRepository.GetIndustryWithSubIndustryList();
                    if (industry.Count > 0)
                    {
                        industry.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        industry.ForEach(x => x.ResponseMessage = "Success");
                        return industry;
                    }
                    else
                        return new List<IndustryListDto> { new IndustryListDto { ResponseMessage = "No Industry Available", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception)
            {
                //  ExceptionLogging.LogException(ex);
                return new List<IndustryListDto> { new IndustryListDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }
        #endregion

        #region SaveClassification
        [Route("Classification/SaveClassification")]
        [HttpPost]
        public List<ClassificationDto> SaveClassification(AddNewClassificationDto ObjClassification)
        {
            using (var repo = new DropdownListRepository())
            {
                try
                {
                    if (ObjClassification != null)
                    {
                        if (repo.CheckClassificationAccountNumberExistCheck(ObjClassification.ChartAccountNumber) == true)
                        {
                            return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = "Chart Account Number already exist.", ResponseCode = (int)ApiStatusCode.Failure } };
                        }
                        var saveClassification = repo.AddNewClassification(ObjClassification);
                        if (saveClassification == false)
                        {
                            return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = "Classification already exist.", ResponseCode = (int)ApiStatusCode.Failure } };
                        }
                        else
                        {
                            using (var userRepository = new DropdownListRepository())
                            {
                                var classificationList = userRepository.GetClassificationListByUserId(Convert.ToInt32(ObjClassification.UserID));
                                if (classificationList.Count > 0)
                                {

                                    classificationList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                                    classificationList.ForEach(x => x.ResponseMessage = "Success");
                                    return classificationList;
                                }
                            }
                        }
                    }
                    return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = "Unable to provide classification.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
                catch (Exception ex)
                {
                    // ExceptionLogging.LogException(ex);
                    return new List<ClassificationDto> { new ClassificationDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
        }

        #endregion

        #region ClassificationTypeList
        [Route("Classification/ClassificationTypeList")]
        [HttpGet]
        public List<ClassificationTypeDto> ClassificationTypeList()
        {
            using (var repo = new DropdownListRepository())
            {
                try
                {
                    var classificationTypelist = repo.GetClassificationType();
                    if (classificationTypelist.Count > 0)
                    {

                        classificationTypelist.ForEach(a => a.ResponseMessage = "Success");
                        classificationTypelist.ForEach(a => a.ResponseCode = (int)ApiStatusCode.Success);
                        return classificationTypelist;
                    }

                    return new List<ClassificationTypeDto> { new ClassificationTypeDto { ResponseMessage = "Unable to provide classification.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
                catch (Exception ex)
                {
                    // ExceptionLogging.LogException(ex);
                    return new List<ClassificationTypeDto> { new ClassificationTypeDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
        }
        #endregion

        #region Get Month list
        [Route("MobileDropdownListing/Month")]
        [HttpGet]
        public List<MonthModelDto> GetMonthList()
        {
            try
            {
                using (var repo = new DropdownListRepository())
                {
                    var List = repo.GetMonthList();
                    if (List.Count > 0)
                    {
                        List.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        List.ForEach(x => x.ResponseMessage = "Success");
                        return List;
                    }
                    else
                        return new List<MonthModelDto> { new MonthModelDto { ResponseMessage = "No Month available", ResponseCode = (int)ApiStatusCode.Failure } };
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new List<MonthModelDto> { new MonthModelDto { ResponseMessage = "Unable connect to the server.Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }
        #endregion

        #region Get Year list
        [Route("MobileDropdownListing/Year")]
        [HttpGet]
        public List<YearModelDto> GetYearList()
        {
            try
            {
                using (var repo = new DropdownListRepository())
                {
                    var List = repo.GetYearList();
                    if (List.Count > 0)
                    {
                        List.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        List.ForEach(x => x.ResponseMessage = "Success");
                        return List;
                    }
                    else
                        return new List<YearModelDto> { new YearModelDto { ResponseMessage = "No Month available", ResponseCode = (int)ApiStatusCode.Failure } };
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new List<YearModelDto> { new YearModelDto { ResponseMessage = "Unable connect to the server.Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }
        #endregion

        #endregion

        #region Yodlee
        [Route("Yodlee/CheckUserYodleeAccount/{UserId}")]
        [HttpGet]
        public bool CheckUserYodleeAccount(int UserId)
        {
            bool UserStatus = false;
            try
            {
                using (var repo = new AccountRepository())
                {
                    UserStatus = repo.CheckUserYodleeAccountExist(UserId);
                }
                return UserStatus;
            }
            catch (Exception)
            {
                return UserStatus;
            }
        }

        [Route("Yodlee/GetAccountList/{UserId}")]
        [HttpGet]
        public List<Account> GetAccountList(int UserId)
        {
            try
            {
                using (var repo = new AccountRepository())
                {
                    var chkAcc = repo.CheckUserYodleeAccount(UserId);
                    if (chkAcc != null)
                    {
                        var AccCredential = new LoginWebsiteDto()
                        {
                            UserName = chkAcc.UName,
                            Password = chkAcc.UPassword
                        };
                        var userTokens = this.UserYodleeTokens(AccCredential);
                        if (string.IsNullOrEmpty(userTokens.Error))
                        {
                            var accountList = new List<Account>();
                            //get list
                            var restClient = new CustomRestClient(ConfigurationManager.AppSettings["restUrl"]);

                            var _parameters = new Dictionary<string, object>();
                            _parameters.Add("cobSessionToken", userTokens.CobrandToken);
                            _parameters.Add("userSessionToken", userTokens.UserToken);

                            var siteAccounts = restClient.Post<IList<SiteAccountInfo>>(YodleeAPI.GET_SITE_ACCOUNTS, _parameters);

                            // 2. For each site account, get item summaries
                            foreach (var site_account in siteAccounts)
                            {
                                var _params = new Dictionary<string, object>() {
						                                                            { "cobSessionToken", userTokens.CobrandToken },
						                                                            { "userSessionToken",  userTokens.UserToken },
						                                                            { "memSiteAccId", site_account.siteAccountId }
					                                                            };

                                // Group item summaries by container name
                                try
                                {
                                    // 2. Fetch the accounts under this site (E.g.: 2 bank accounts, 1 loan)
                                    var itemSummaries = restClient.Post<ICollection<ItemSummary>>(YodleeAPI.GET_ITEM_SUMMARIES_FOR_SITE, _params);

                                    // 3. For each item summary, get the container type
                                    foreach (var item_summary in itemSummaries)
                                    {

                                        var _params2 = new Dictionary<string, object>()
							                {
								                { "cobSessionToken", userTokens.CobrandToken },
								                { "contentServiceId", item_summary.contentServiceId },
								                { "reqSpecifier", 128 },
								                { "notrim", true }
							                };

                                        try
                                        {
                                            item_summary.itemData.accounts.ToList().ForEach(s => s.siteAccountId = site_account.siteAccountId);
                                            item_summary.itemData.accounts.ToList().ForEach(s => s.BankName = site_account.siteInfo.defaultOrgDisplayName);
                                            // Fetch container iformation for the account and update collection
                                            var contentServiceInfo = restClient.Post<ContentServiceInfo>(YodleeAPI.GET_CONTENT_SERVICE_INFO1, _params2);

                                            //if (!accountsGroup.Keys.Contains<string>(contentServiceInfo.containerInfo.containerName))
                                            //    accountsGroup.Add(contentServiceInfo.containerInfo.containerName, new List<Account>(item_summary.itemData.accounts));
                                            //else
                                            //    accountsGroup[contentServiceInfo.containerInfo.containerName].AddRange(item_summary.itemData.accounts);

                                            //  if (contentServiceInfo.containerInfo.containerName == "bank" || contentServiceInfo.containerInfo.containerName == "credit")
                                            //  {
                                            accountList.AddRange((List<Account>)item_summary.itemData.accounts);
                                            // }

                                        }
                                        catch (Exception)
                                        { }
                                    }
                                }
                                catch (Exception)
                                {
                                    // TODO: Handle this exception.  Not implemented in this sample application
                                    // throw e;
                                }
                            }
                            accountList.ForEach(s => s.ResponseMessage = "Success");
                            accountList.ForEach(s => s.ResponseCode = (int)ApiStatusCode.Success);
                            return accountList;
                        }
                    }
                    // else
                    //{
                    return new List<Account> { new Account { ResponseMessage = "No account linked yet.", ResponseCode = (int)ApiStatusCode.NotExist } };
                    // }
                }




            }
            catch (Exception)
            {
                return new List<Account> { new Account { ResponseMessage = "Please try again later.", ResponseCode = (int)ApiStatusCode.Failure } };
            }
        }

        [Route("Yodlee/AddBankAccount")]
        [HttpPost]
        public string AddBankAccount(LoginWebsiteDto Obj)
        {
            try
            {
                using (var repo = new AccountRepository())
                {
                    var accDetails = repo.CheckUserYodleeAccount(Obj.UserId);
                    if (accDetails != null)
                    {
                        Obj.UserName = accDetails.UName;
                        Obj.Password = accDetails.UPassword;
                        var TokenDetails = this.UserYodleeTokens(Obj);
                        if (!string.IsNullOrEmpty(TokenDetails.Error))
                        {
                            return "Invalid credentials";
                        }
                        else
                        {
                            #region Site Id
                            int SiteId = 0;
                            switch (Obj.BankId)
                            {
                                case 1: //RBC
                                    SiteId = 1642;
                                    break;
                                case 2: //Scotia
                                    SiteId = 6866;
                                    break;
                                case 3: //BMO 
                                    SiteId = 10716;
                                    break;
                                case 4: //TD Bank
                                    SiteId = 3521;
                                    break;
                                case 5: //CIBC
                                    SiteId = 10457;
                                    break;
                                default:
                                    SiteId = 16441;
                                    break;
                            }

                            #endregion
                            if (SiteId > 0)
                            {
                                var restUrl = ConfigurationManager.AppSettings["restUrl"];
                                var restClient = new CustomRestClient(restUrl);

                                // Get Site Info
                                var _parameters = new Dictionary<string, object>();

                                _parameters.Add("cobSessionToken", TokenDetails.CobrandToken);
                                _parameters.Add("siteFilter.reqSpecifier", 1);
                                _parameters.Add("siteFilter.siteId", SiteId);
                                var site_info = restClient.Post<SiteInfo>(YodleeAPI.GET_SITE_INFO, _parameters);

                                _parameters.Clear();
                                _parameters.Add("cobSessionToken", TokenDetails.CobrandToken);
                                _parameters.Add("siteId", SiteId);
                                var site_login_form = restClient.Post<FormObject>(YodleeAPI.SITE_LOGIN_FORM, _parameters);

                                var credentialFields = site_login_form as FormObject;
                                int index = 0;

                                _parameters.Clear();
                                _parameters.Add("cobSessionToken", TokenDetails.CobrandToken);
                                _parameters.Add("userSessionToken", TokenDetails.UserToken);
                                _parameters.Add("siteId", SiteId);
                                _parameters.Add("credentialFields.enclosedType", "com.yodlee.common.FieldInfoSingle");

                                foreach (ComponentList componentList in credentialFields.componentList)
                                {
                                    _parameters.Add(String.Format("credentialFields[{0}].displayName", index), componentList.displayName);
                                    _parameters.Add(String.Format("credentialFields[{0}].fieldType.typeName", index), componentList.fieldType.typeName);
                                    _parameters.Add(String.Format("credentialFields[{0}].helpText", index), componentList.helpText);
                                    _parameters.Add(String.Format("credentialFields[{0}].maxlength", index), componentList.maxlength);
                                    _parameters.Add(String.Format("credentialFields[{0}].name", index), componentList.name);
                                    _parameters.Add(String.Format("credentialFields[{0}].size", index), componentList.size);
                                    _parameters.Add(String.Format("credentialFields[{0}].value", index), (index == 0) ? Obj.AddAccountUserName : Obj.AddAccountPassword);
                                    _parameters.Add(String.Format("credentialFields[{0}].valueIdentifier", index), componentList.valueIdentifier);
                                    _parameters.Add(String.Format("credentialFields[{0}].valueMask", index), componentList.valueMask);
                                    _parameters.Add(String.Format("credentialFields[{0}].isEditable", index), componentList.isEditable);
                                    index++;
                                }

                                var add_site_account1 = restClient.Post<SiteAccountInfo>(YodleeAPI.ADD_SITE_ACCOUNT1, _parameters);

                                int memSiteAccId = add_site_account1.siteAccountId;
                                List<string> messages = new List<string>();
                                if (memSiteAccId > 0)
                                {

                                    //   ViewBag.memSiteAccId = memSiteAccId;

                                    _parameters.Clear();
                                    _parameters.Add("cobSessionToken", TokenDetails.CobrandToken);
                                    _parameters.Add("userSessionToken", TokenDetails.UserToken);
                                    _parameters.Add("memSiteAccId", memSiteAccId);

                                chkStatus: var get_site_refresh_info = restClient.Post<SiteRefreshInfo>(YodleeAPI.GET_SITE_REFRESH_INFO, _parameters);

                                    string refreshMode = get_site_refresh_info.siteRefreshMode.refreshMode;
                                    string siteRefreshStatus = get_site_refresh_info.siteRefreshStatus.siteRefreshStatus;

                                    if (refreshMode == "MFA") // It's an MFA refresh
                                    {
                                        // CASE: MFA
                                        if (siteRefreshStatus == "LOGIN_FAILURE")
                                        {
                                            messages.Add(ApiErrors.Errors["402"].description);
                                            return ApiErrors.Errors["402"].description;
                                        }

                                        if (siteRefreshStatus == "REFRESH_TRIGGERED")
                                        {
                                            //ViewBag.memSiteAccId = memSiteAccId;
                                            goto chkStatus;
                                            // return siteRefreshStatus;
                                        }

                                        if (siteRefreshStatus == "REFRESH_COMPLETED" || siteRefreshStatus == "REFRESH_TIMED_OUT")
                                        {
                                            return siteRefreshStatus;
                                        }
                                        else
                                        {
                                            goto chkStatus;
                                            //return siteRefreshStatus;
                                        }
                                    }
                                    else if (refreshMode == "NORMAL")
                                    {
                                        // CASE: NORMAL
                                        if (siteRefreshStatus == "REFRESH_COMPLETED" || siteRefreshStatus == "REFRESH_TIMED_OUT" || siteRefreshStatus == "LOGIN_FAILURE")
                                        {
                                            if (siteRefreshStatus == "LOGIN_FAILURE")
                                            {
                                                messages.Add(ApiErrors.Errors["402"].description);
                                                return ApiErrors.Errors["402"].description;
                                            }
                                            else
                                            {
                                                return siteRefreshStatus;

                                                //return RedirectToAction("get_item_summaries_for_site", "home", new { memSiteAccId = memSiteAccId });
                                            }
                                        }
                                        else
                                        {

                                            goto chkStatus;
                                            // return siteRefreshStatus;
                                        }
                                    }
                                    else
                                    {
                                        return "No result";
                                    }
                                }
                                else
                                {
                                    messages.Add("No result.");
                                    return "No result";

                                }
                            }
                            else
                            {
                                return "Please select a valid bank";
                            }
                        }
                    }
                    else
                    {
                        return "Not able to add account.";
                    }

                }

            }
            catch (Exception)
            {

                return "Unable Connect to Server.";
            }
        }

        [Route("Yodlee/RemoveAccount/{UserId}/{siteAccountId}")]
        [HttpPost]
        public bool RemoveAccount(int UserId, string siteAccountId)
        {
            try
            {
                using (var repo = new AccountRepository())
                {
                    var chkAcc = repo.CheckUserYodleeAccount(UserId);
                    if (chkAcc != null)
                    {
                        var AccCredential = new LoginWebsiteDto()
                        {
                            UserName = chkAcc.UName,
                            Password = chkAcc.UPassword
                        };
                        var userTokens = this.UserYodleeTokens(AccCredential);
                        if (string.IsNullOrEmpty(userTokens.Error))
                        {
                            var _parameters = new Dictionary<string, object>();
                            var restUrl = ConfigurationManager.AppSettings["restUrl"];
                            var restClient = new CustomRestClient(restUrl);
                            _parameters.Add("cobSessionToken", userTokens.CobrandToken);
                            _parameters.Add("userSessionToken", userTokens.UserToken);
                            _parameters.Add("memSiteAccId", siteAccountId);
                            //restClient.Post(YodleeAPI.REMOVE_SITE_ACCOUNT, _parameters);
                            string dataResponse = restClient.RemovePost(YodleeAPI.REMOVE_SITE_ACCOUNT, _parameters);
                            if (!string.IsNullOrEmpty(dataResponse))
                            {
                                return false;
                            }
                            return true;
                        }
                    }

                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        [Route("Yodlee/YodleeLogin")]
        [HttpPost]
        public string YodleeLogin(LoginWebsiteDto Obj)
        {
            try
            {
                var TokenDetails = this.UserYodleeTokens(Obj);
                if (!string.IsNullOrEmpty(TokenDetails.Error))
                {
                    return "Invalid credentials";
                }
                else
                {
                    //Insert account into database
                    #region Add Account To Database
                    using (var repo = new AccountRepository())
                    {
                        return repo.YodleeRegistration(Obj, ConfigurationManager.AppSettings["cobrandLogin"], ConfigurationManager.AppSettings["cobrandPassword"]);
                    }
                    #endregion
                }
            }
            catch (Exception)
            {
                return "Unable Connect to Server.";
            }
        }

        public LoginWebsiteDto UserYodleeTokens(LoginWebsiteDto Obj)
        {
            string CobrandToken = string.Empty;
            string UserToken = string.Empty;
            var cobrandLogin = ConfigurationManager.AppSettings["cobrandLogin"];
            var cobrandPassword = ConfigurationManager.AppSettings["cobrandPassword"];
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var contextPath = ConfigurationManager.AppSettings["contextPath"];
            var _parameters = new Dictionary<string, object>();

            var restClient = new CustomRestClient(restUrl);

            _parameters.Add("cobrandLogin", cobrandLogin);
            _parameters.Add("cobrandPassword", cobrandPassword);

            var cobrandContext = restClient.Post<CobrandContext>(YodleeAPI.COB_LOGIN, _parameters);
            _parameters.Clear();
            if (cobrandContext.cobrandConversationCredentials == null)
            {
                Obj.Error = "Invalid Credentials";
                // throw new Exception("Invalid Credentials");
            }
            CobrandToken = cobrandContext.cobrandConversationCredentials.sessionToken;

            _parameters.Add("cobSessionToken", CobrandToken);
            _parameters.Add("login", Obj.UserName);
            _parameters.Add("password", Obj.Password);

            var panel_login_info = restClient.Post<UserInfo>(YodleeAPI.USER_LOGIN, _parameters);
            UserToken = panel_login_info.userContext.conversationCredentials.sessionToken;
            if (panel_login_info.userContext != null)
            {
                Obj.CobrandToken = CobrandToken;
                Obj.UserToken = UserToken;
            }
            else
            {
                Obj.Error = "Invalid Credentials";
            }
            return Obj;
        }
        #endregion

        #region Bank Statement

        #region Credit account list
        [Route("GeneralLedger/UserCreditAccount/{userid}")]
        [HttpGet]
        public List<AccountsDto> GetCreditAccounts(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var userRepository = new ReconcillationRepository())
                    {
                        string AccountType = "credits";
                        var accounts = userRepository.GetUserAccounts(userid, AccountType);
                        if (accounts.Count > 0)
                        {
                            accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            accounts.ForEach(x => x.ResponseMessage = "Success");
                            return accounts;
                        }
                        else
                        {
                            return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<AccountsDto> { new AccountsDto { ResponseMessage = "Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<AccountsDto> { new AccountsDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        [Route("GeneralLedger/UserCreditCardAccount/{userid}/{bankid}")]
        [HttpGet]
        public List<AccountsDto> GetCreditCardAccounts(int userid, int bankid)
        {
            try
            {
                if (userid > 0 && bankid > 0)
                {
                    using (var userRepository = new ReconcillationRepository())
                    {
                        //if (bankid == 1)
                        //{
                        //    var accounts = userRepository.GetUserAccountsByUserId(userid, bankid);
                        //    if (accounts.Count > 0)
                        //    {
                        //        accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        //        accounts.ForEach(x => x.ResponseMessage = "Success");
                        //        return accounts;
                        //    }
                        //    else
                        //    {
                        //        return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        //    }
                        //}
                        //else
                        //{
                        string AccountType = "credits";
                        var accounts = userRepository.GetUserAccountsByUserId(userid, bankid, AccountType);
                        if (accounts.Count > 0)
                        {
                            accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            accounts.ForEach(x => x.ResponseMessage = "Success");
                            return accounts;
                        }
                        else
                        {
                            return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                        //}
                    }
                }
                else
                {
                    return new List<AccountsDto> { new AccountsDto { ResponseMessage = "Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<AccountsDto> { new AccountsDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        [Route("GeneralLedger/UserAccount/CreditStatementDetails")]
        [HttpPost]
        public List<BankDto> GetCreditStatement(BankAccountDto _bankaccdto)
        {
            try
            {
                using (var userRepository = new ReconcillationRepository())
                {
                    string AccountType = _bankaccdto.BankId == 7 ? "Cash" : "credits";
                    var bankstatement = userRepository.GetStatementList(_bankaccdto.Accountname, _bankaccdto.Month, _bankaccdto.Year, Convert.ToInt32(_bankaccdto.UserId), Convert.ToInt32(_bankaccdto.BankId), AccountType);
                    if (bankstatement.Count > 0)
                    {
                        bankstatement.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        bankstatement.ForEach(x => x.ResponseMessage = "Success");
                        return bankstatement;
                    }
                    else
                    {
                        return new List<BankDto> { new BankDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                    }
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<BankDto> { new BankDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Bank Account
        [Route("GeneralLedger/UserBankAccounts/{userid}")]
        [HttpGet]
        public List<AccountsDto> UserBankAccounts(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var userRepository = new ReconcillationRepository())
                    {
                        string AccountType = "bank";
                        var accounts = userRepository.GetUserAccounts(userid, AccountType);
                        // var accounts = userRepository.GetBankAccounts(userid);
                        if (accounts.Count > 0)
                        {
                            accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            accounts.ForEach(x => x.ResponseMessage = "Success");
                            return accounts;
                        }
                        else
                        {
                            return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<AccountsDto> { new AccountsDto { ResponseMessage = "Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<AccountsDto> { new AccountsDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        [Route("GeneralLedger/UserAccount/{userid}/{bankid}")]
        [HttpGet]
        public List<AccountsDto> GetAccounts(int userid, int bankid)
        {
            try
            {
                if (userid > 0 && bankid > 0)
                {
                    using (var userRepository = new ReconcillationRepository())
                    {
                        //if (bankid == 1)
                        //{
                        //    string AccountType = "bank";
                        //    var accounts = userRepository.GetUserAccountsByUserId(userid, bankid, AccountType);
                        //    if (accounts.Count > 0)
                        //    {
                        //        accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        //        accounts.ForEach(x => x.ResponseMessage = "Success");
                        //        return accounts;
                        //    }
                        //    else
                        //    {
                        //        return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        //    }
                        //}
                        //else
                        //{
                        string AccountType = "bank";
                        var accounts = userRepository.GetUserAccountsByUserId(userid, bankid, AccountType);
                        if (accounts.Count > 0)
                        {
                            accounts.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            accounts.ForEach(x => x.ResponseMessage = "Success");
                            return accounts;
                        }
                        else
                        {
                            return new List<AccountsDto> { new AccountsDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                        //}
                    }
                }
                else
                {
                    return new List<AccountsDto> { new AccountsDto { ResponseMessage = "Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<AccountsDto> { new AccountsDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        [Route("GeneralLedger/UserAccount/BankStatementDetails")]
        [HttpPost]
        public List<BankDto> GetBankStatement(BankAccountDto _bankaccdto)
        {
            try
            {
                using (var userRepository = new ReconcillationRepository())
                {
                    string AccountType = _bankaccdto.BankId == 7 ? "Cash" : "bank";
                    var bankstatement = userRepository.GetStatementList(_bankaccdto.Accountname, _bankaccdto.Month, _bankaccdto.Year, Convert.ToInt32(_bankaccdto.UserId), Convert.ToInt32(_bankaccdto.BankId), AccountType);
                    if (bankstatement.Count > 0)
                    {
                        bankstatement.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        bankstatement.ForEach(x => x.ResponseMessage = "Success");
                        return bankstatement;
                    }
                    else
                    {
                        return new List<BankDto> { new BankDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                    }
                }
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new List<BankDto> { new BankDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        /// <summary>
        /// Update bank statement
        /// </summary>
        /// <param name="_bankdto"></param>
        /// <returns></returns>
        [Route("GeneralLedger/UserAccount/UpdateBankStatement")]
        [HttpPost]
        public ApiResponseDto UpdateBankStatement(BankDto _bankdto)
        {
            //update bank n bill data from mobile
            try
            {
                using (var expenseRepository = new ReconcillationRepository())
                {
                    bool result = expenseRepository.UpdateBankStatement(_bankdto);
                    if (result)
                    {
                        return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                    }
                    else
                    {
                        return new ApiResponseDto { ResponseMessage = "Failure", ResponseCode = (int)ApiStatusCode.Failure };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.Failure };
            }
        }

        /// <summary>
        /// Delete statement
        /// </summary>
        /// <param name="statementId"></param>
        /// <returns></returns>
        [Route("GeneralLedger/UserAccount/DeleteStatement/{statementId}")]
        [HttpPost]
        public bool DeleteStatement(int statementId)
        {
            bool IsDeleted = false;
            try
            {
                using (var repo = new ReconcillationRepository())
                {
                    var rep = repo.DeleteBankExpenseById(statementId);
                    IsDeleted = true;
                }
            }
            catch (Exception)
            {
            }
            return IsDeleted;
        }

        [Route("GeneralLedger/UploadImage")]
        [HttpPost]
        public ApiResponseDto MyFileUpload(CameraImageUploadDto imageDto)
        {
            try
            {
                if (imageDto.UserId != null && !string.IsNullOrEmpty(imageDto.Base64Image) && !string.IsNullOrEmpty(imageDto.ImageName))
                {
                    Image ocrImage;
                    byte[] array = Convert.FromBase64String(imageDto.Base64Image);
                    using (var repo = new ReconcillationRepository())
                    {
                        using (var ms = new MemoryStream(array, 0, array.Length))
                        {
                            string rawfolder = HttpContext.Current.Server.MapPath("~/RawImages/");
                            if (!Directory.Exists(rawfolder))
                            {
                                Directory.CreateDirectory(rawfolder);
                            }
                            ms.Write(array, 0, array.Length);
                            ocrImage = Image.FromStream(ms, true);
                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                            var filePath = rawfolder + datetime + imageDto.ImageName;
                            ocrImage.Save(filePath);
                            var DateMonth = ReadDateFromImage.ReadTextFromImage(filePath);
                            //datetime + imageDto.ImageName;
                            KippinStoreImageDto obj = new KippinStoreImageDto();
                            obj.ImageName = datetime + imageDto.ImageName;
                            obj.DateCreated = DateTime.Now;
                            obj.IsAssociated = false;
                            obj.UserId = imageDto.UserId;
                            obj.Month = DateMonth.Date.Month;
                            obj.Year = DateMonth.Date.Year;
                            obj.IsDeleted = false;
                            var addImageToCloudDbEntry = repo.AddImageToKippinStore(obj);
                            string folder = HttpContext.Current.Server.MapPath("~/CameraUploadImages/" + imageDto.UserId + "/" + DateMonth.Date.Year + "/" + DateMonth.Date.Month);
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }
                            if (DateMonth != null)
                            {
                                File.Move(filePath, folder + "/" + datetime + ".jpg");
                            }

                            ApiResponseDto response = new ApiResponseDto();
                            response.ResponseCode = (int)ApiStatusCode.Success;
                            response.ResponseMessage = "Image Successfully Saved.";
                            return response;
                        }
                    }
                }
                else
                {
                    //  ExceptionLogging.LogException(ex);
                    return new ApiResponseDto { ResponseMessage = "Image Failed to Upload.", ResponseCode = (int)ApiStatusCode.NullParameter };
                }
            }
            catch (ArgumentNullException ex)
            {
                //  ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Image Failed to Upload.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
            catch (Exception ex)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }


        #region Add cash entry
        [Route("GeneralLedger/AddCashEntry")]
        [HttpPost]
        public ApiResponseDto AddCashEntry(AddCashExpenseDto obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj.CashBillDate))
                {
                    obj.Date = DateTime.ParseExact(obj.CashBillDate, "dd/MM/yyyy", null).ToShortDateString();
                }
                string rawfolder = HttpContext.Current.Server.MapPath("~/OcrImages/" + obj.UserId + "/");
                //save bill image
                if (!string.IsNullOrEmpty(obj.BillPath))
                {
                    Image ocrImage;
                    byte[] array = Convert.FromBase64String(obj.BillPath);
                    using (var ms = new MemoryStream(array, 0, array.Length))
                    {
                        //rawfolder = rawfolder.Replace("MobileApi\\", "");
                        if (!Directory.Exists(rawfolder))
                        {
                            Directory.CreateDirectory(rawfolder);
                        }
                        ms.Write(array, 0, array.Length);
                        ocrImage = Image.FromStream(ms, true);
                        string filePath = string.Empty;
                        var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                        if (obj.IsCloud == true)
                        {
                            filePath = rawfolder + obj.CloudImageName;
                        }
                        else
                        {
                            filePath = rawfolder + datetime + obj.ImageName;
                        }

                        ocrImage.Save(filePath);
                    }
                }
                //
                using (var repo = new ExpenseRepository())
                {
                    var InsertCashEntry = repo.SaveCashBankExpenseEntryFromMobile(obj);
                    if (InsertCashEntry > 0)
                    {
                        string StatementId = Convert.ToString(InsertCashEntry);
                        string tar = rawfolder + StatementId + "/";
                        if (!Directory.Exists(tar))
                        {
                            Directory.CreateDirectory(tar);
                        }
                        foreach (var srcPath in Directory.GetFiles(rawfolder))
                        {
                            //Copy the file from sourcepath and place into mentioned target path, 
                            //Overwrite the file if same file is exist in target path
                            System.IO.File.Move(srcPath, srcPath.Replace(rawfolder, tar));
                        }
                        #region Cloud image entry
                        if (obj.IsCloud == true)
                        {
                            var aa = obj.ImagePath.Split(new[] { "CameraUploadImages" }, StringSplitOptions.None);
                            var bb = aa[1].Split('/');
                            CloudImagesRecordDto objCloud = new CloudImagesRecordDto();
                            objCloud.ImageName = Convert.ToString(bb[4]);
                            objCloud.DateCreated = DateTime.Now;
                            objCloud.IsAssociated = true;
                            objCloud.StatementId = Convert.ToInt32(StatementId);
                            objCloud.UserId = Convert.ToInt32(bb[1]);
                            objCloud.Month = Convert.ToInt32(bb[3]);
                            objCloud.Year = Convert.ToInt32(bb[2]);

                            var addImageToCloudDbEntry = repo.AddImageToCloud(objCloud);
                        }
                        #endregion
                        return new ApiResponseDto { ResponseMessage = "Success", ResponseCode = (int)ApiStatusCode.Success };
                    }
                }
                return new ApiResponseDto { ResponseMessage = "Unable to save the cash entry data. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #region upload bill images
        [Route("GeneralLedger/UploadBillImage")]
        [HttpPost]
        public ApiResponseDto UploadBillImage(KippinStoreImageDto imageDto)
        {
            try
            {
                //if (imageDto.Image != null && imageDto.ImageName != null && imageDto.ExpenseId > 0 && imageDto.BankId > 0 && imageDto.UserId > 0)
                if (imageDto.UserId != null && imageDto.StatementId != null && !string.IsNullOrEmpty(imageDto.Image) && !string.IsNullOrEmpty(imageDto.ImageName))
                {
                    Image ocrImage;
                    byte[] array = Convert.FromBase64String(imageDto.Image);
                    using (var expenseRepository = new ExpenseRepository())
                    {
                        using (var ms = new MemoryStream(array, 0, array.Length))
                        {
                            string rawfolder = HttpContext.Current.Server.MapPath("~/OcrImages/" + imageDto.UserId + "/" + imageDto.StatementId + "/");
                            // rawfolder = rawfolder.Replace("MobileApi\\", "");
                            if (!Directory.Exists(rawfolder))
                            {
                                Directory.CreateDirectory(rawfolder);
                            }
                            ms.Write(array, 0, array.Length);
                            ocrImage = Image.FromStream(ms, true);
                            string filePath = string.Empty;
                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                            if (imageDto.IsCloud == true)
                            {
                                filePath = rawfolder + imageDto.CloudImageName;
                                CloudImagesRecordDto obj = new CloudImagesRecordDto();
                                obj.ImageName = imageDto.CloudImageName;
                                obj.DateCreated = DateTime.Now;
                                obj.IsAssociated = true;
                                obj.StatementId = imageDto.StatementId;
                                obj.UserId = imageDto.UserId;
                                obj.Month = imageDto.Month;
                                obj.Year = imageDto.Year;
                                var addImageToCloudDbEntry = expenseRepository.AddImageToCloud(obj);
                            }
                            else
                            {
                                filePath = rawfolder + datetime + ".jpg";
                            }

                            ocrImage.Save(filePath);

                            ApiResponseDto response = new ApiResponseDto();
                            response.ResponseCode = (int)ApiStatusCode.Success;
                            //  response.ResponseMessage = "Image Successfully Saved.";
                            response.ResponseMessage = "Success";
                            return response;
                            // return null;
                        }
                    }
                }
                else
                {
                    //  ExceptionLogging.LogException(ex);
                    return new ApiResponseDto { ResponseMessage = "Image Failed to Upload.", ResponseCode = (int)ApiStatusCode.NullParameter };
                }
            }
            catch (ArgumentNullException ex)
            {
                // ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Image Failed to Upload.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
            catch (Exception ex)
            {
                //  ExceptionLogging.LogException(ex);
                return new ApiResponseDto { ResponseMessage = "Image Failed to Upload.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }


        [Route("GeneralLedger/GetBillUploadedImages/{userId}/{statementId}")]
        [HttpGet]
        public List<KippinStoreImageDto> GetBillUploadedImages(int userId, int statementId)
        {
            List<KippinStoreImageDto> imageList = new List<KippinStoreImageDto>();
            var Chkfolder = HttpContext.Current.Server.MapPath("~/OcrImages/" + userId + "/" + statementId + "/");
            // Chkfolder = Chkfolder.Replace("\\MobileApi", "");
            if (Directory.Exists(Chkfolder))
            {
                var data = Directory.EnumerateFiles(Chkfolder);

                if (data != null)
                {
                    foreach (var image in data)
                    {
                        string baseUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"];
                        KippinStoreImageDto imageDto = new KippinStoreImageDto();
                        imageDto.ImagePath = baseUrl + "OcrImages/" + userId + "/" + statementId + "/" + Path.GetFileName(image);
                        imageDto.ImageName = Path.GetFileName(image);
                        imageDto.UserId = Convert.ToInt32(userId);
                        imageDto.ResponseCode = 1;
                        imageDto.ResponseMessage = "Success";
                        imageList.Add(imageDto);
                    }
                }
                else
                {
                    KippinStoreImageDto imageDto = new KippinStoreImageDto();
                    imageDto.ResponseCode = 2;
                    imageDto.ResponseMessage = "No Image Avaiable";
                    imageList.Add(imageDto);
                }
            }
            else
            {
                KippinStoreImageDto imageDto = new KippinStoreImageDto();
                imageDto.ResponseCode = 2;
                imageDto.ResponseMessage = "No Image Avaiable";
                imageList.Add(imageDto);
            }



            return imageList;
        }
        #endregion

        #region Kippin Store
        [Route("GeneralLedger/GetUploadedImages/{userId}/{month}/{year}/{startRec}/{endRec}")]
        [HttpGet]
        public List<ImageListDto> MyFileUpload(int userId, int month, int year, int startRec, int endRec)
        {

            List<ImageListDto> imageList = new List<ImageListDto>();
            var Chkfolder = HttpContext.Current.Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/" + month);
            // Chkfolder = Chkfolder.Replace("KF.MobileWebApi", "KF.Web\\MobileApi");//Commented as we shift api into same web solution
            if (Directory.Exists(Chkfolder))
            {
                var data = Directory.EnumerateFiles(Chkfolder);

                using (var db = new KFentities())
                {
                    var associatedImagesList = db.CloudImagesRecords.Where(a => a.UserId == userId && a.Year == year && a.Month == month).ToList();

                    if (data != null)
                    {
                        foreach (var image in data)
                        {
                            string baseUrl = ConfigurationManager.AppSettings["WebsiteBaseUrl"];
                            ImageListDto imageDto = new ImageListDto();
                            imageDto.ImagePath = baseUrl + "CameraUploadImages/" + userId + "/" + year + "/" + month + "/" + Path.GetFileName(image);
                            imageDto.ImageName = Path.GetFileName(image);
                            imageDto.UserId = Convert.ToString(userId);
                            imageDto.ResponseCode = 1;
                            imageDto.ResponseMessage = "Success";
                            if (associatedImagesList.Count > 0)
                            {
                                foreach (var item in associatedImagesList)
                                {
                                    if (Path.GetFileName(image) == item.ImageName)
                                    {
                                        imageDto.IsAssociated = true;
                                        break;
                                    }
                                    else
                                    {
                                        imageDto.IsAssociated = false;
                                    }
                                }

                            }
                            imageList.Add(imageDto);
                        }
                    }
                    else
                    {
                        ImageListDto imageDto = new ImageListDto();
                        imageDto.ResponseCode = 2;
                        imageDto.ResponseMessage = "No Image Avaiable";
                        imageList.Add(imageDto);
                    }
                }
            }
            else
            {
                ImageListDto imageDto = new ImageListDto();
                imageDto.ResponseCode = 2;
                imageDto.ResponseMessage = "No Image Avaiable";
                imageList.Add(imageDto);
            }



            return imageList;
        }

        [Route("GeneralLedger/GetUploadedImages/{userId}")]
        [HttpGet]
        public List<FolderListDto> MyFileUpload(int userId)
        {

            List<FolderListDto> imageList = new List<FolderListDto>();
            var Chkfolder = HttpContext.Current.Server.MapPath("~/CameraUploadImages/" + userId);
            // Chkfolder = Chkfolder.Replace("KF.MobileWebApi", "KF.Web\\MobileApi");
            using (var db = new KFentities())
            {
                bool IsAssociatedBit = false;
                var imageAssociated = db.KippinStoreImages.Where(a => a.UserId == userId && a.IsDeleted == false).Any();
                if (imageAssociated == true)
                {
                    IsAssociatedBit = false;
                }
                else
                {
                    IsAssociatedBit = true;
                }
                //KF.MobileWebApi
                if (Directory.Exists(Chkfolder))
                {
                    var data = Directory.GetDirectories(Chkfolder);
                    string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                    string[] folders = Directory.GetDirectories(Chkfolder);
                    foreach (var image in folders)
                    {
                        FolderListDto imageDto = new FolderListDto();
                        FileInfo f = new FileInfo(image);
                        imageDto.IsAssociated = IsAssociatedBit;
                        imageDto.FolderName = f.Name;
                        imageDto.ResponseCode = 1;
                        imageDto.ResponseMessage = "Success";
                        imageList.Add(imageDto);
                    }


                }
                else
                {
                    FolderListDto imageDto = new FolderListDto();
                    imageDto.ResponseCode = 2;
                    imageDto.ResponseMessage = "No Image Avaiable";
                    imageList.Add(imageDto);
                }

            }

            return imageList;
        }

        [Route("GeneralLedger/GetUploadedImages/{userId}/{year}")]
        [HttpGet]
        public List<FolderListDto> MyFileUploadYear(int userId, int year)
        {

            List<FolderListDto> imageList = new List<FolderListDto>();
            var Chkfolder = HttpContext.Current.Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/");
            //  Chkfolder = Chkfolder.Replace("KF.MobileWebApi", "KF.Web\\MobileApi");
            using (var db = new KFentities())
            {

                if (Directory.Exists(Chkfolder))
                {
                    var data = Directory.GetDirectories(Chkfolder);
                    string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                    string[] folders = Directory.GetDirectories(Chkfolder);

                    foreach (var image in folders)
                    {
                        FolderListDto imageDto = new FolderListDto();
                        FileInfo f = new FileInfo(image);
                        int month = Convert.ToInt32(f.Name);
                        var imageAssociated = db.KippinStoreImages.Where(x => x.UserId == userId && x.Year == year && x.Month == month && x.IsDeleted == false).Any();
                        if (imageAssociated == true)
                        {
                            imageDto.IsAssociated = false;
                        }
                        else
                        {
                            imageDto.IsAssociated = true;
                        }
                        imageDto.FolderName = f.Name;

                        imageDto.ResponseCode = 1;
                        imageDto.ResponseMessage = "Success";
                        imageList.Add(imageDto);
                    }
                }
                else
                {
                    FolderListDto imageDto = new FolderListDto();
                    imageDto.ResponseCode = 2;
                    imageDto.ResponseMessage = "No Image Avaiable";
                    imageList.Add(imageDto);
                }
            }
            return imageList;
        }

        #endregion

        #region Reports
        [Route("GeneralLedger/IncomeReport/{userid}/{startDate}/{endDate}")]
        [HttpGet]
        public IncomeSheetDto GetIncomeReport(int userid, string startDate, string endDate)
        {
            try
            {
                if (userid > 0 && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    using (var userRepository = new ReportRepository())
                    {
                        var data = userRepository.GetIncomeSheetData(userid, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddHours(23),null,null);
                        data.ResponseCode = (int)ApiStatusCode.Success;
                        data.ResponseMessage = "Success";
                        return data;
                    }
                  //  return new IncomeSheetDto { ResponseMessage = "No content available", ResponseCode = (int)ApiStatusCode.NoContent };
                }
                else
                {
                    return new IncomeSheetDto { ResponseMessage = "Parameters are null", ResponseCode = (int)ApiStatusCode.NullParameter };
                }
            }
            catch (Exception)
            {
                // ExceptionLogging.LogException(ex);
                return new IncomeSheetDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion
        #endregion
    }
}
