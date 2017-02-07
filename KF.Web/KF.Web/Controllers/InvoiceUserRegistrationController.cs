using AutoMapper;
using KF.Dto.Modules.Invoice;
using KF.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using KF.Dto.Modules.Common;
using KF.Repo.Modules;
using KF.Entity;
using KF.Repo.Modules.Invoice;
using KF.Web.Models.Invoice;
using KF.Utilities.Common;
using KF.Repo.Modules.Finance;



namespace KF.MobileWebApi.Controllers
{
    [RoutePrefix("InvoiceUserApi/Registration")]
    public class InvoiceUserRegistrationController : ApiController
    {
        int CustomerJvid = 0001;
        string pdfPath = string.Empty;
        #region Register Invoice
        [Route("User/InvoiceRegistration")]
        //Invoice=true,Finance=False
        [HttpPost]
        public ApiResponseDto InvoiceUserRegistration(InvoiceUserRegistrationDto ObjUser)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    string ImagePath = string.Empty;
                    if (ObjUser != null)
                    {
                        var existCheck = repository.UserEmailChk(ObjUser.EmailTo);
                        if (existCheck == true)
                        {
                            return new ApiResponseDto { ResponseMessage = "User already exist.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                        }
                        else
                        {
                            var fin_emailexistCheck = repository.FinanceUserEmailChk(ObjUser.EmailTo);
                            if (fin_emailexistCheck == true)
                            {
                                if (ObjUser.FromInvoiceOrFinance == false)
                                {
                                    if (!string.IsNullOrEmpty(ObjUser.CompanyLogo))
                                    {
                                        #region Save Profile Image
                                        Image CompanyLogo;
                                        byte[] array = Convert.FromBase64String(ObjUser.CompanyLogo);
                                        using (var ms = new MemoryStream(array, 0, array.Length))
                                        {
                                            //string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                                            string folder = HttpContext.Current.Server.MapPath("~/InvoiceUserConpanyLogoImage/");
                                            if (!Directory.Exists(folder))
                                            {
                                                Directory.CreateDirectory(folder);
                                            }
                                            ms.Write(array, 0, array.Length);
                                            CompanyLogo = Image.FromStream(ms, true);
                                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                                            var filePath = folder + datetime + ".png";
                                            CompanyLogo.Save(filePath);
                                            ObjUser.CompanyLogo = "InvoiceUserConpanyLogoImage/" + datetime + ".png";
                                        }
                                        #endregion
                                    }

                                    //#region
                                    //var CustomeraddressUpdated = repository.CustomeraddressUpdated(ObjUser);
                                    //#endregion

                                    var IsOnlyInvoicesettozero = repository.IsOnlyInvoice(ObjUser.EmailTo);
                                    var isVerified = repository.IsVerifiedUserEmailChk(ObjUser.EmailTo);
                                    if (isVerified != "")
                                    {
                                        var userid = repository.RegisterWithFinance(ObjUser, isVerified, 0);
                                        if (userid > 0)
                                        {
                                            var FinanceUpdateInvoice = repository.FinanceInvoiceBit(ObjUser);
                                            if (FinanceUpdateInvoice == true)
                                            {
                                                return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                                            }
                                            else
                                            {
                                                return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                            }
                                        }
                                        else
                                        {
                                            return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                        }
                                    }
                                }
                                else
                                {
                                    return new ApiResponseDto { ResponseMessage = "Email already associated with finance account. Please login with finance to access invoice dashboard.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                }
                            }

                            else
                            {
                                if (!string.IsNullOrEmpty(ObjUser.CompanyLogo))
                                {
                                    #region Save Profile Image
                                    Image CompanyLogo;
                                    byte[] array = Convert.FromBase64String(ObjUser.CompanyLogo);
                                    using (var ms = new MemoryStream(array, 0, array.Length))
                                    {
                                        string folder = HttpContext.Current.Server.MapPath("~/InvoiceUserConpanyLogoImage/");
                                        if (!Directory.Exists(folder))
                                        {
                                            Directory.CreateDirectory(folder);
                                        }
                                        ms.Write(array, 0, array.Length);
                                        CompanyLogo = Image.FromStream(ms, true);
                                        var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                                        var filePath = folder + datetime + ".png";
                                        CompanyLogo.Save(filePath);
                                        ObjUser.CompanyLogo = "InvoiceUserConpanyLogoImage/" + datetime + ".png";
                                    }
                                    #endregion
                                }
                                string pass = RandomString.GetUniqueKey();
                                ObjUser.Password = pass;
                                var usernamecheck = repository.ExistingInvoiceUsernameCheck(ObjUser);
                                if (usernamecheck == true)
                                {
                                    return new ApiResponseDto { ResponseMessage = "Username Already Exist.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                                else
                                {
                                    var userid = repository.Register(ObjUser);
                                    if (userid > 0)
                                    {
                                        var bytes = Encoding.UTF8.GetBytes(userid.ToString());
                                        var base64 = Convert.ToBase64String(bytes);
                                        //success
                                        //Read Email Message body from html file
                                        string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplateInvoice.html"));
                                        //Replace Username
                                        html = html.Replace("#CompanyName", ObjUser.CompanyName);
                                        //string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/MobileUserPayment/" + base64;
                                        //html = html.Replace("#URL", Url);
                                        html = html.Replace("#Password", pass);
                                        //string TrialUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantTrialMode/" + base64;
                                        //html = html.Replace("#TrialURL", TrialUrl);
                                        // string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Views/Account/ConfirmEmailForMobileInvoice/" + base64;
                                        string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/ConfirmEmailForMobileInvoice/" + base64;
                                        html = html.Replace("#URL", Url);
                                        //Send a confirmation Email and send to a page where a message will display plz chk your email
                                        SendMailModelDto _objModelMail = new SendMailModelDto();
                                        _objModelMail.To = ObjUser.EmailTo;
                                        _objModelMail.Subject = "Confirmation mail from Kippin-Invoice";
                                        _objModelMail.MessageBody = html;
                                        var mailSent = Sendmail.SendEmail(_objModelMail);
                                        if (mailSent == true)
                                        {
                                            return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Please check your mail for email confirmation.", UserId = userid };
                                        }
                                        return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                                    }
                                    else
                                    {
                                        return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        return new ApiResponseDto { ResponseMessage = "Please provide data for user registration.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
            return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
        }
        #endregion

        #region Login/ForgetPassword/Change Password
        #region Login

        [Route("InvoiceUser/Login")]
        [HttpPost]
        public InvoiceUserRegistrationDto Login(InvoiceUserRegistrationDto ObjUser)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    if (!string.IsNullOrEmpty(ObjUser.EmailTo) && !string.IsNullOrEmpty(ObjUser.Password))
                    {
                        var emailExist = repository.EmailExist(ObjUser.EmailTo);
                        var CustomerId = repository.GetCustomerId(ObjUser.EmailTo);
                        if (emailExist > 0)
                        {
                            var isverify = repository.Isverify(ObjUser.EmailTo);
                            if (isverify == true)
                            {
                                var userData = repository.Login(ObjUser);
                                if (userData.Id == 0 || userData.Id == null)
                                {
                                    return new InvoiceUserRegistrationDto { ResponseMessage = "Invalid Email and Password.", ResponseCode = (int)ApiStatusCode.Unauthorised, CustomerId = CustomerId };
                                }
                                userData.ResponseCode = (int)ApiStatusCode.Success;
                                userData.ResponseMessage = "Success";
                                userData.UserId = emailExist;
                                userData.CustomerId = CustomerId;
                                return userData;
                            }
                            else
                            {
                                return new InvoiceUserRegistrationDto { ResponseMessage = "Please Verify to proceed.", ResponseCode = (int)ApiStatusCode.NullParameter, CustomerId = CustomerId };
                            }
                        }
                        else
                        {
                            return new InvoiceUserRegistrationDto { ResponseMessage = "Invalid login credentials.", ResponseCode = (int)ApiStatusCode.NullParameter };
                        }
                    }
                    else
                    {
                        return new InvoiceUserRegistrationDto { ResponseMessage = "Please provide Email and Password.", ResponseCode = (int)ApiStatusCode.NullParameter };
                    }
                }
            }
            catch (Exception)
            {
                return new InvoiceUserRegistrationDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            }
        }
        #endregion

        #region Forgot Password

        [Route("InvoiceUser/ForgotPassword/{email}")]
        [HttpPut]
        public ApiResponseDto ForgotPassword(string email)
        {
            try
            {
                string password = Sendmail.GenerateRandomString(6);
                var userrepository = new InvoiceUserRegistrationRepository();

                var username = userrepository.ForgotPassword(email, password);
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
            catch (Exception ex)
            {
                return new ApiResponseDto { ResponseMessage = "Failure", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
        }

        #endregion

        #region Change Password
        [Route("InvoiceUser/Changepassword")]
        [HttpPost]
        public ApiResponseDto Changepass(ChangePassword obj)
        {
            try
            {
                if (obj.UserId > 0 && !string.IsNullOrEmpty(obj.OldPassword) && !string.IsNullOrEmpty(obj.NewPassword))
                {
                    using (var repo = new InvoiceUserRegistrationRepository())
                    {
                        var updateUserProfile = repo.ChangePass(obj);
                        if (updateUserProfile == "Success")
                        {
                            return new ApiResponseDto { ResponseMessage = "Password successfully updated.", ResponseCode = (int)ApiStatusCode.Success };
                        }
                        else if (updateUserProfile == "Incorrect password.")
                        {
                            return new ApiResponseDto { ResponseMessage = "Incorrect old Password.", ResponseCode = (int)ApiStatusCode.Failure };
                        }
                        else
                        {
                            return new ApiResponseDto { ResponseMessage = "Failed to update password.", ResponseCode = (int)ApiStatusCode.Failure };
                        }
                    }
                }
                return new ApiResponseDto { ResponseMessage = "Unable to change password. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
            catch (Exception)
            {

                return new ApiResponseDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #endregion

        #region Supplier/Customer
        #region Search

        [Route("CheckInvoiceExistOrNot")]
        [HttpPost]
        public ApiResponseDto AddCustomerSupplier(AddCustomerSupplierDto obj)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    var UserExistInVoice = repository.AlreadyExistInvoiceEmailwithSameId(obj);

                    if (obj != null)
                    {

                        if (UserExistInVoice == false)
                        {

                            var EmailExist = repository.AlreadyExistInvoiceEmail(obj);
                            if (EmailExist != true)
                            {
                                var ExistWithAnotherUser = repository.AlreadyCustomer(obj);
                                if (ExistWithAnotherUser == true)
                                {
                                    return new AddCustomerSupplierDto { ResponseMessage = "Added Successfully from Existing.", ResponseCode = (int)ApiStatusCode.Success };
                                }
                                else
                                {
                                    var Financedata = repository.FinanceAlreadyExist(obj);
                                    if (Financedata != null)
                                    {
                                        Financedata.ResponseCode = (int)ApiStatusCode.Success;
                                        Financedata.ResponseMessage = "Success";
                                        return Financedata;
                                    }
                                    var Invoicedata = repository.InvoiceAlreadyExist(obj);
                                    if (Invoicedata != null)
                                    {
                                        Invoicedata.ResponseCode = (int)ApiStatusCode.Success;
                                        Invoicedata.ResponseMessage = "Success";
                                        return Invoicedata;
                                    }
                                    else
                                    {
                                        return new AddCustomerSupplierDto { ResponseMessage = "Not Register yet.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                    }
                                }

                            }
                            else
                            {
                                return new AddCustomerSupplierDto { ResponseMessage = "Already Registered.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                            }

                        }
                        else
                        {
                            return new AddCustomerSupplierDto { ResponseMessage = "You cannot add yourself as a supplier/customer.", ResponseCode = (int)ApiStatusCode.NullParameter };
                        }
                    }
                    else
                    {
                        return new AddCustomerSupplierDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter };
                    }

                }

            }
            catch (Exception)
            {
                return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            }
        }

        #endregion

        [Route("RegisterInvoiceCustomerSupplier")]
        [HttpPost]
        public ApiResponseDto RegisterInvoiceCustomerSupplier(AddCustomerSupplierDto obj)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    if (obj != null)
                    {
                        var userid = repository.RegisterInvoiceCustomerSupplier(obj);
                        if (userid > 0)
                        {
                            return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                        }
                        else
                        {
                            return new ApiResponseDto { ResponseMessage = "Already Exist.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }
                    }
                    else
                    {
                        return new ApiResponseDto { ResponseMessage = "Please provide data for registration.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }

        [Route("UpdateInvoiceCustomerSupplier")]
        [HttpPost]
        public AddCustomerSupplierDto UpdateInvoiceCustomerSupplier(AddCustomerSupplierDto objUser)
        {
            try
            {
                if (objUser != null && objUser.RoleId > 0)
                {
                    using (var repo = new InvoiceUserRegistrationRepository())
                    {
                        var updateUserProfile = repo.UpdateInvoiceCustomerSupplier(objUser);
                        updateUserProfile.ResponseCode = (int)ApiStatusCode.Success;
                        updateUserProfile.ResponseMessage = "Success";
                        return updateUserProfile;
                    }
                }
                return new AddCustomerSupplierDto { ResponseMessage = "Unable to update the profile. Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter };
            }
            catch (Exception)
            {
                return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #region Get currency list
        [Route("InvoiceCurrency")]
        [HttpGet]
        public List<InvoiceCurrencyDto> GetInvoiceCurrencyList()
        {
            try
            {
                using (var userRepository = new InvoiceUserRegistrationRepository())
                {
                    var industry = userRepository.GetCurrencyListInvoice();
                    if (industry.Count > 0)
                    {
                        Mapper.CreateMap<tblCurrency, InvoiceCurrencyDto>();
                        var currencyDto = Mapper.Map<List<InvoiceCurrencyDto>>(industry);
                        currencyDto.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        currencyDto.ForEach(x => x.ResponseMessage = "Success");
                        return currencyDto;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return new List<InvoiceCurrencyDto> { new InvoiceCurrencyDto { ResponseMessage = ex.Message, ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Customer/Supplier
        [Route("InvoiceCustomerSupplierList/{UserId}/{RoleId}")]
        [HttpGet]
        public List<AddCustomerSupplierDto> InvoiceCustomerSupplierList(int UserId, int RoleId)
        {
            try
            {
                if (UserId > 0 && RoleId > 0)
                {
                    using (var userRepository = new InvoiceUserRegistrationRepository())
                    {
                        var data = userRepository.InvoiceCustomerSupplierList(UserId, RoleId);
                        if (data.Count > 0)
                        {
                            data.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            data.ForEach(x => x.ResponseMessage = "Success");
                            return data;
                        }
                        else
                        {
                            return new List<AddCustomerSupplierDto> { new AddCustomerSupplierDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<AddCustomerSupplierDto> { new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch (Exception)
            {
                return new List<AddCustomerSupplierDto> { new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Create Invoice
        [Route("CreateInvoiceNumber/{UserId}")]
        [HttpPost]
        public ApiResponseDto CreateInvoiceNumber(int UserId)
        {
            try
            {
                if (UserId > 0)
                {
                    using (var repo = new InvoiceUserRegistrationRepository())
                    {
                        int invoiceNumber = repo.CreateInvoiceNumber(UserId);
                        if (invoiceNumber > 0)
                        {
                            return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                        }
                        else
                        {
                            return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }
                    }
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }

        [Route("CreateInvoice")]
        [HttpPost]
        public ApiResponseDto CreateInvoice(InvoiceDetailDto obj)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    if (obj != null)
                    {
                        using (var context = new KFentities())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                var userData = new tblInvoiceDetail();

                                if (userData.InvoiceNumber == null)
                                {

                                    if (obj.ButtonType == "Save")
                                    {
                                        userData.In_R_FlowStatus = "Draft";
                                        userData.In_R_Status = "Inprogress";
                                        userData.Pro_FlowStatus = "Draft";
                                        userData.Pro_Status = "Inprogress";
                                    }
                                    userData.CreatedDate = DateTime.Now;
                                    if (obj.ButtonType == "Send")
                                    {
                                        //customerid =tblcustomer = email
                                        userData.In_R_FlowStatus = "Sent";
                                        userData.In_R_Status = "Inprogress";
                                        userData.Pro_FlowStatus = "Pending Approval";
                                        userData.Pro_Status = "Inprogress";

                                    }
                                    userData.BalanceDue = obj.BalanceDue;
                                    userData.DocumentRef = obj.DocumentRef;
                                    userData.SalesPerson = obj.SalesPerson;
                                    //userData.DepositePayment = Obj.DepositePayment;
                                    userData.UserId = obj.UserId;
                                    // userData.SubTotal = Obj.SubTotal;
                                    userData.Total = obj.Total;
                                    if (obj.ShippingCost == null || obj.ShippingCost.HasValue == false)
                                    {
                                        userData.ShippingCost = 0;
                                    }
                                    else
                                    {
                                        userData.ShippingCost = obj.ShippingCost;
                                    }
                                    if (obj.DepositePayment == null || obj.DepositePayment.HasValue == false)
                                    {
                                        userData.DepositePayment = 0;
                                    }
                                    else
                                    {
                                        userData.DepositePayment = obj.DepositePayment;
                                    }
                                    userData.Terms = obj.Terms;
                                    userData.Note = obj.Note;
                                    userData.InvoiceDate = obj.InvoiceDate;
                                    userData.DueDate = obj.DueDate;
                                    userData.PaymentTerms = obj.PaymentTerms;
                                    userData.InvoiceNumber = obj.InvoiceNumber;
                                    userData.DepositePayment = obj.DepositePayment;
                                    userData.CustomerId = obj.CustomerId;
                                    userData.Type = obj.Type;
                                    userData.IsInvoiceReport = true;
                                    userData.IsDeleted = false;
                                    userData.CustomerManualPaidAmount = (decimal)0.0000;
                                    userData.SupplierManualPaidAmount = (decimal)0.0000;
                                    userData.IsCustomerManualPaid = false;
                                    userData.IsSupplierManualPaid = false;
                                    context.tblInvoiceDetails.Add(userData);
                                    context.SaveChanges();

                                    tblItemDetail dbinsert = new tblItemDetail();
                                    var counter = obj.ServiceTypeId.Count();

                                    for (int i = 0; i < counter; i++)
                                    {
                                        dbinsert.InvoiceId = userData.Id;
                                        dbinsert.ItemId = obj.ItemId[i];
                                        dbinsert.Item = obj.Item[i];
                                        dbinsert.ServiceTypeId = obj.ServiceTypeId[i];
                                        dbinsert.Description = obj.Description[i];
                                        dbinsert.Quantity = obj.Quantity[i];
                                        dbinsert.Rate = obj.Rate[i];
                                        dbinsert.Amount = obj.Amount[i];
                                        dbinsert.Discount = obj.Discount[i];
                                        dbinsert.SubTotal = obj.SubTotal[i];

                                        if (obj.Tax[i] == "null")
                                        {
                                            dbinsert.Tax = EmptyIfNull(obj.Tax[i]);
                                        }
                                        else
                                        {
                                            dbinsert.Tax = obj.Tax[i];
                                        }
                                        if (obj.GST_Tax[i] == "null")
                                        {
                                            dbinsert.GST_Tax = EmptyIfNull(obj.GST_Tax[i]);
                                        }
                                        else
                                        {
                                            dbinsert.GST_Tax = obj.GST_Tax[i];
                                        }
                                        if (obj.HST_Tax[i] == "null")
                                        {
                                            dbinsert.HST_Tax = EmptyIfNull(obj.HST_Tax[i]);
                                        }
                                        else
                                        {
                                            dbinsert.HST_Tax = obj.HST_Tax[i];
                                        }
                                        if (obj.PST_Tax[i] == "null")
                                        {
                                            dbinsert.PST_Tax = EmptyIfNull(obj.PST_Tax[i]);
                                        }
                                        else
                                        {
                                            dbinsert.PST_Tax = obj.PST_Tax[i];
                                        }
                                        if (obj.QST_Tax[i] == "null")
                                        {
                                            dbinsert.QST_Tax = EmptyIfNull(obj.QST_Tax[i]);
                                        }
                                        else
                                        {
                                            dbinsert.QST_Tax = obj.QST_Tax[i];
                                        }

                                        if (obj.Customer_Service[i] == "null")
                                        {
                                            dbinsert.Customer_Service = EmptyIfNull(obj.Customer_Service[i]);
                                        }
                                        else
                                        {
                                            dbinsert.Customer_Service = obj.Customer_Service[i];
                                        }
                                        if (obj.ServiceType[i] == "null")
                                        {
                                            dbinsert.ServiceType = EmptyIfNull(obj.ServiceType[i]);
                                        }
                                        else
                                        {
                                            dbinsert.ServiceType = obj.ServiceType[i];
                                        }
                                        dbinsert.Customer_ServiceTypeId = obj.Customer_ServiceTypeId[i];

                                        context.tblItemDetails.Add(dbinsert);
                                        context.SaveChanges();

                                    }
                                    dbContextTransaction.Commit();
                                    if (obj.ButtonType == "Send")
                                    {

                                        var report = string.Empty;

                                        GeneratingReport genReport = new GeneratingReport();
                                        pdfPath = genReport.PrintRun(userData.Id, 0);

                                        InvoiceEmailwithpdf InvoiceEmail = new InvoiceEmailwithpdf();
                                        InvoiceEmail.SendEmailToInvoice(userData.Id, pdfPath);

                                    }
                                    if (userData.Id > 0)
                                    {
                                        return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = userData.Id };
                                    }
                                    else
                                    {
                                        return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                    }
                                }
                                else
                                {
                                    return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                    }
                    else
                    {
                        return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }




        [Route("RevenueList/{UserId}/{num}")]
        [HttpGet]
        public List<RevenueListDto> RevenueList(int UserId, int num)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    var Revenue = repository.RevenueList(UserId, num);
                    if (Revenue.Count > 0)
                    {
                        Mapper.CreateMap<Classification, RevenueListDto>();
                        var Revenuedata = Mapper.Map<List<RevenueListDto>>(Revenue);
                        Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                        return Revenuedata;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                return new List<RevenueListDto> { new RevenueListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Send Invoice
        [Route("SendInvoiceList/{userid}")]
        [HttpGet]
        public List<Sp_SendInvoiceListDto> SendInvoiceList(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var RecievedList = repository.SendInvoiceList(userid);
                        if (RecievedList.Count() > 0)
                        {
                            RecievedList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            RecievedList.ForEach(x => x.ResponseMessage = "Success");
                            return RecievedList;
                        }
                        else
                        {
                            return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region GetItems
        [Route("GetItemsDatabyInvoiceId/{InvoiceId}")]
        [HttpGet]
        public List<ItemtableDto> GetItemsDatabyInvoiceId(int InvoiceId)
        {
            try
            {
                if (InvoiceId > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var Items = repository.GetItemsDatabyInvoiceId(InvoiceId);
                        if (Items.Count > 0)
                        {
                            Items.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            Items.ForEach(x => x.ResponseMessage = "Success");
                            return Items;
                        }
                        else
                        {
                            return new List<ItemtableDto> { new ItemtableDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId } };
                        }
                    }
                }
                else
                {
                    return new List<ItemtableDto> { new ItemtableDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<ItemtableDto> { new ItemtableDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Send Proforma
        //Developer Gurinder
        //Date 22-11-2016
        [Route("SendProformaList/{userid}")]
        [HttpGet]
        public List<Sp_SendInvoiceListDto> SendProformaList(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var RecievedList = repository.SendProformaList(userid);
                        if (RecievedList.Count() > 0)
                        {
                            RecievedList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            RecievedList.ForEach(x => x.ResponseMessage = "Success");
                            return RecievedList;
                        }
                        else
                        {
                            return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Received Proforma
        //Developer Gurinder
        //Date 23-11-2016
        [Route("ReceivedProformaList/{userid}")]
        [HttpGet]
        public List<Sp_SendInvoiceListDto> ReceivedProformaList(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var RecievedList = repository.ReceivedProformaList(userid);
                        if (RecievedList.Count() > 0)
                        {
                            RecievedList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            RecievedList.ForEach(x => x.ResponseMessage = "Success");
                            return RecievedList;
                        }
                        else
                        {
                            return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Received Invoice
        //Developer Gurinder
        //Date 23-11-2016
        [Route("ReceivedInvoiceList/{userid}")]
        [HttpGet]
        public List<Sp_SendInvoiceListDto> ReceivedInvoiceList(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var RecievedList = repository.ReceivedInvoiceList(userid);
                        if (RecievedList.Count() > 0)
                        {
                            RecievedList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            RecievedList.ForEach(x => x.ResponseMessage = "Success");
                            return RecievedList;
                        }
                        else
                        {
                            return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }

        #endregion

        #region ExpenseAssetList with CustomerId 1,2
        //Developer Gurinder 
        //Date 23-11-2016
        [Route("ExpenseAssetList/{UserId}/{num}")]
        [HttpGet]
        public List<Kl_BlockListDto> ExpenseAssetList(int UserId, int num)
        {
            try
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    var Revenue = repository.ExpenseAssetList(UserId, num);
                    if (Revenue.Count > 0)
                    {
                        Mapper.CreateMap<Classification, Kl_BlockListDto>();
                        var Revenuedata = Mapper.Map<List<Kl_BlockListDto>>(Revenue);
                        Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                        Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                        return Revenuedata;
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                return new List<Kl_BlockListDto> { new Kl_BlockListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region Update Invoice
        //Developer Gurinder
        //Date 23-11-2016
        [Route("UpdateInvoice")]
        [HttpPost]
        public ApiResponseDto UpdateInvoice(InvoiceDetailDto obj)
        {
            try
            {
                if (obj != null)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
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
                                        var item = context.tblItemDetails.Where(s => s.ItemId == id && s.InvoiceId == obj.Id).SingleOrDefault();
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
                            if (userChk.Id > 0)
                            {
                                return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = userChk.Id };
                            }
                            else
                            {
                                return new ApiResponseDto { ResponseMessage = "Data is wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                            }
                        }
                    }
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Please provide data 2.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                }
            }
            catch (Exception ex)
            {
                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                return new ApiResponseDto { ResponseMessage = ex.InnerException.ToString(), ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            }
        }
        #endregion

        #region Reporting List
        [Route("ReportingList/{userid}")]
        [HttpGet]
        public List<Sp_SendInvoiceListDto> ReportingList(int userid)
        {
            try
            {
                if (userid > 0)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {

                        var RecievedList = repository.ReportingList(userid);
                        if (RecievedList.Count() > 0)
                        {
                            RecievedList.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            RecievedList.ForEach(x => x.ResponseMessage = "Success");
                            return RecievedList;
                        }

                        else
                        {
                            return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "No records found", ResponseCode = (int)ApiStatusCode.NoContent } };
                        }
                    }
                }
                else
                {
                    return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
                }
            }
            catch
            {
                return new List<Sp_SendInvoiceListDto> { new Sp_SendInvoiceListDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.NullParameter } };
            }
        }
        #endregion

        #region generate pdf
        //developer gurinder
        //date 30-11-2016

        [Route("generateInvoicepdf/{userid}/{isCustomer}")]
        [HttpGet]
        public string generateInvoicepdf(int userid, int isCustomer)
        {
            try { 
            GeneratingReport genReport = new GeneratingReport();
            pdfPath = genReport.PrintRun(userid, isCustomer);
            return pdfPath;
                }
            catch (Exception ex)
            {
                return ex.ToString();

            }
        }
        #endregion

        #region Manual Payment
        //Developer Gurinder
        //Date 23-11-2016
        [Route("ManualPayment")]
        [HttpPost]
        public ApiResponseDto ManualPayment(ManualPaymentDto obj)
        {
            try
            {
                if (obj != null)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        int InvoiceId = repository.ManualPayment(obj);
                        if (InvoiceId > 0)
                        {
                            return new ApiResponseDto { ResponseMessage = "Manual Payment is Completed.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        }
                        else
                        {
                            return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                        }
                    }
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                }
            }
            catch
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        #region Stripe Payment
        //Developer Gurinder
        //Date 13-12-2016
        [Route("StripePayment")]
        [HttpPost]
        public ApiResponseDto StripePayment(StripePaymentDto obj)
        {
            try
            {
                if (obj != null)
                {
                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        int InvoiceId = repository.StripePayment(obj);
                        if (InvoiceId > 0)
                        {
                            return new ApiResponseDto { ResponseMessage = "Successfully Paid.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        }
                        else
                        {
                            return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                        }
                    }
                }
                else
                {
                    return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                }
            }
            catch
            {
                return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            }
        }
        #endregion

        public string EmptyIfNull(string value)
        {
            if (value == "null")
                value = string.Empty;
            return value.ToString();
        }

    }
}
