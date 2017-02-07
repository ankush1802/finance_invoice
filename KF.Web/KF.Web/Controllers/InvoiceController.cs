using AutoMapper;
using KF.Dto.Modules.Common;
//using KF.Context;
using KF.Dto.Modules.Invoice;
using KF.Entity;
using KF.ModelDto.DataTransferObjects;
using KF.Repo.Modules.Common;
using KF.Repo.Modules.Invoice;
//using KF.MobileWebApi.Models;
//using KF.ModelDto.DataTransferObjects;
using KF.Utilities;
//using KF.Utilities.DropdownList;
using KF.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KF.Web.Controllers
{
    public class InvoiceController : Controller
    {
        AES encrypt = new AES();
        #region Invoice Active User
        public ActionResult InvoiceActiveUser()
        {
            //if (Selectuser == false || Selectuser == null)
            //{
            //    Response.Cookies.Clear();
            //    var c = new HttpCookie("SelectedActiveUser");
            //    c.Value = "0";
            //    c.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(c);
            //}
            return View();
        }
        #endregion

        #region Invoice Supplier Customer Dashboard
        public ActionResult InvoiceSupplierCustomerDashboard(string Type)
        {

            return View();
        }
        #endregion

        #region Invoice Register

        public ActionResult InvoiceRegister()
        {
            RegisterInvoiceViewModel invoice = new RegisterInvoiceViewModel();
            invoice.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            invoice.TradingCurrencyList = ClsDropDownList.PopulateCurrency_Guri();
            return View(invoice);
        }

        [HttpPost]
        public ActionResult InvoiceRegister(RegisterInvoiceViewModel invoice)
        {
            //try
            //{
            //    using (var repository = new InvoiceUserRegistrationRepository())
            //    {
            //        InvoiceUserRegistrationDto ObjUser = new InvoiceUserRegistrationDto();
            //        ObjUser.CompanyLogo = invoice.CompanyLogo == null ? "" : "";
            //        ObjUser.CompanyName = invoice.CompanyName == null ? "" : invoice.CompanyName;
            //        ObjUser.ContactPerson = invoice.ContactPerson == null ? "" : invoice.ContactPerson;
            //        ObjUser.CorporateAptNo = invoice.CorporateAptNo == null ? "" : invoice.CorporateAptNo;
            //        ObjUser.CorporateHouseNo = invoice.CorporateHouseNo == null ? "" : invoice.CorporateHouseNo;
            //        ObjUser.CorporateStreet = invoice.CorporateStreet == null ? "" : invoice.CorporateStreet;
            //        ObjUser.CorporateCity = invoice.CorporateCity == null ? "" : invoice.CorporateCity;
            //        ObjUser.CorporateState = invoice.CorporateState == null ? "" : invoice.CorporateState;
            //        ObjUser.CorporatePostalCode = invoice.CorporatePostalCode == null ? "" : invoice.CorporatePostalCode;
            //        ObjUser.ShippingAptNo = invoice.ShippingAptNo == null ? "" : invoice.ShippingAptNo;
            //        ObjUser.ShippingHouseNo = invoice.ShippingHouseNo == null ? "" : invoice.ShippingHouseNo;
            //        ObjUser.ShippingStreet = invoice.ShippingStreet == null ? "" : invoice.ShippingStreet;
            //        ObjUser.ShippingCity = invoice.ShippingCity == null ? "" : invoice.ShippingCity;
            //        ObjUser.ShippingState = invoice.ShippingState == null ? "" : invoice.ShippingState;
            //        ObjUser.ShippingPostalCode = invoice.ShippingPostalCode == null ? "" : invoice.ShippingPostalCode;
            //        ObjUser.GoodsType = invoice.GoodsType == null ? "" : invoice.GoodsType;
            //        ObjUser.TradingCurrency = Convert.ToString(invoice.TradingCurrencyId);
            //        ObjUser.BusinessNumber = invoice.BusinessNumber == null ? "" : invoice.BusinessNumber;
            //        ObjUser.EmailTo = invoice.EmailTo == null ? "" : invoice.EmailTo;
            //        ObjUser.EmailCc = invoice.EmailCc == null ? "" : invoice.EmailCc;
            //        ObjUser.AreaCode = invoice.AreaCode == null ? "" : invoice.AreaCode;
            //        ObjUser.MobileNumber = invoice.AreaCode + '-' + invoice.MobileNumber;
            //        ObjUser.Website = invoice.Website == null ? "" : invoice.Website;
            //        ObjUser.Username = invoice.UserName == null ? "" : invoice.UserName;
            //        string ImagePath = string.Empty;
            //        if (ObjUser != null)
            //        {
            //            var existCheck = repository.UserEmailChk(ObjUser.EmailTo);
            //            if (existCheck == true)
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "User already exist.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            //            }
            //            else
            //            {
            //                var fin_emailexistCheck = repository.FinanceUserEmailChk(ObjUser.EmailTo);
            //                if (fin_emailexistCheck == true)
            //                {
            //                    if (ObjUser.FromInvoiceOrFinance == false)
            //                    {
            //                        if (!string.IsNullOrEmpty(ObjUser.CompanyLogo))
            //                        {
            //                            #region Save Profile Image
            //                            Image CompanyLogo;
            //                            byte[] array = Convert.FromBase64String(ObjUser.CompanyLogo);
            //                            using (var ms = new MemoryStream(array, 0, array.Length))
            //                            {
            //                                //string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            //                                string folder = Server.MapPath("~/InvoiceUserConpanyLogoImage/");
            //                                if (!Directory.Exists(folder))
            //                                {
            //                                    Directory.CreateDirectory(folder);
            //                                }
            //                                ms.Write(array, 0, array.Length);
            //                                CompanyLogo = Image.FromStream(ms, true);
            //                                var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
            //                                var filePath = folder + datetime + ".png";
            //                                CompanyLogo.Save(filePath);
            //                                ObjUser.CompanyLogo = "InvoiceUserConpanyLogoImage/" + datetime + ".png";
            //                            }
            //                            #endregion
            //                        }

            //                        //#region
            //                        //var CustomeraddressUpdated = repository.CustomeraddressUpdated(ObjUser);
            //                        //#endregion

            //                        var IsOnlyInvoicesettozero = repository.IsOnlyInvoice(ObjUser.EmailTo);
            //                        var isVerified = repository.IsVerifiedUserEmailChk(ObjUser.EmailTo);
            //                        if (isVerified != "")
            //                        {
            //                            var userid = repository.RegisterWithFinance(ObjUser,isVerified,0);


            //                            if (userid > 0)
            //                            {
            //                                var FinanceUpdateInvoice = repository.FinanceInvoiceBit(ObjUser);
            //                                if (FinanceUpdateInvoice == true)
            //                                {
            //                                    // return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
            //                                }
            //                                else
            //                                {
            //                                    // return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            //                                }
            //                            }
            //                            else
            //                            {
            //                                //return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //return new ApiResponseDto { ResponseMessage = "Email already associated with finance account. Please login with finance to access invoice dashboard.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            //                    }
            //                }
            //                else
            //                {
            //                    if (!string.IsNullOrEmpty(ObjUser.CompanyLogo))
            //                    {
            //                        #region Save Profile Image
            //                        Image CompanyLogo;
            //                        byte[] array = Convert.FromBase64String(ObjUser.CompanyLogo);
            //                        using (var ms = new MemoryStream(array, 0, array.Length))
            //                        {
            //                            //string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            //                            string folder =  Server.MapPath("~/InvoiceUserConpanyLogoImage/");
            //                            if (!Directory.Exists(folder))
            //                            {
            //                                Directory.CreateDirectory(folder);
            //                            }
            //                            ms.Write(array, 0, array.Length);
            //                            CompanyLogo = Image.FromStream(ms, true);
            //                            var datetime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
            //                            var filePath = folder + datetime + ".png";
            //                            CompanyLogo.Save(filePath);
            //                            ObjUser.CompanyLogo = "InvoiceUserConpanyLogoImage/" + datetime + ".png";
            //                        }
            //                        #endregion
            //                    }
            //                    GeneratePassword objpass = new GeneratePassword();
            //                    string pass = objpass.AutoPassword();
            //                    ObjUser.Password = pass;
            //                    var usernamecheck = repository.ExistingInvoiceUsernameCheck(ObjUser);
            //                    if (usernamecheck == true)
            //                    {
            //                        //return new ApiResponseDto { ResponseMessage = "Username Already Exist.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            //                    }
            //                    else
            //                    {
            //                        var userid = repository.Register(ObjUser);
            //                        if (userid > 0)
            //                        {
            //                            var bytes = Encoding.UTF8.GetBytes(userid.ToString());
            //                            var base64 = Convert.ToBase64String(bytes);
            //                            //success
            //                            //Read Email Message body from html file
            //                            string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplateInvoice.html"));
            //                            //Replace Username
            //                            html = html.Replace("#CompanyName", ObjUser.CompanyName);
            //                            //string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/MobileUserPayment/" + base64;
            //                            //html = html.Replace("#URL", Url);
            //                            html = html.Replace("#Password", pass);
            //                            //string TrialUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/UserwithanaccountantTrialMode/" + base64;
            //                            //html = html.Replace("#TrialURL", TrialUrl);
            //                            string Url = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString() + "Account/ConfirmEmailForMobileInvoice/" + base64;
            //                            html = html.Replace("#URL", Url);
            //                            //Send a confirmation Email and send to a page where a message will display plz chk your email
            //                            SendMailModelDto _objModelMail = new SendMailModelDto();
            //                            _objModelMail.To = ObjUser.EmailTo;
            //                            _objModelMail.Subject = "Confirmation mail from Kippin-Invoice";
            //                            _objModelMail.MessageBody = html;
            //                            var mailSent = Sendmail.SendEmail(_objModelMail);
            //                            if (mailSent == true)
            //                            {
            //                                // return new ApiResponseDto { ResponseCode = (int)ApiStatusCode.Success, ResponseMessage = "Please check your mail for email confirmation.", UserId = userid };
            //                            }
            //                            // return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
            //                        }
            //                        else
            //                        {
            //                            //   return new ApiResponseDto { ResponseMessage = "Unable to create the user. Please try later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
            //                        }
            //                    }

            //                }
            //            }

            //        }
            //        else
            //        {
            //            //  return new ApiResponseDto { ResponseMessage = "Please provide data for user registration.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            //}



            // if (ModelState.IsValid)
            // {
            try
            {
                using (var context = new KFentities())
                {

                    try
                    {
                        RegisterInvoiceViewModel1 obj = new RegisterInvoiceViewModel1();
                        obj.CompanyLogo = invoice.CompanyLogo == null ? "" : "";
                        obj.CompanyName = invoice.CompanyName == null ? "" : invoice.CompanyName;
                        obj.ContactPerson = invoice.ContactPerson == null ? "" : invoice.ContactPerson;
                        obj.CorporateAptNo = invoice.CorporateAptNo == null ? "" : invoice.CorporateAptNo;
                        obj.CorporateHouseNo = invoice.CorporateHouseNo == null ? "" : invoice.CorporateHouseNo;
                        obj.CorporateStreet = invoice.CorporateStreet == null ? "" : invoice.CorporateStreet;
                        obj.CorporateCity = invoice.CorporateCity == null ? "" : invoice.CorporateCity;
                        obj.CorporateState = invoice.CorporateState == null ? "" : invoice.CorporateState;
                        obj.CorporatePostalCode = invoice.CorporatePostalCode == null ? "" : invoice.CorporatePostalCode;
                        obj.ShippingAptNo = invoice.ShippingAptNo == null ? "" : invoice.ShippingAptNo;
                        obj.ShippingHouseNo = invoice.ShippingHouseNo == null ? "" : invoice.ShippingHouseNo;
                        obj.ShippingStreet = invoice.ShippingStreet == null ? "" : invoice.ShippingStreet;
                        obj.ShippingCity = invoice.ShippingCity == null ? "" : invoice.ShippingCity;
                        obj.ShippingState = invoice.ShippingState == null ? "" : invoice.ShippingState;
                        obj.ShippingPostalCode = invoice.ShippingPostalCode == null ? "" : invoice.ShippingPostalCode;
                        obj.GoodsType = invoice.GoodsType == null ? "" : invoice.GoodsType;
                        obj.TradingCurrency = Convert.ToString(invoice.TradingCurrencyId);
                        obj.BusinessNumber = invoice.BusinessNumber == null ? "" : invoice.BusinessNumber;
                        obj.EmailTo = invoice.EmailTo == null ? "" : invoice.EmailTo;
                        obj.EmailCc = invoice.EmailCc == null ? "" : invoice.EmailCc;
                        obj.AreaCode = invoice.AreaCode == null ? "" : invoice.AreaCode;
                        obj.MobileNumber = invoice.AreaCode + '-' + invoice.MobileNumber;
                        obj.Website = invoice.Website == null ? "" : invoice.Website;
                        obj.UserName = invoice.UserName == null ? "" : invoice.UserName;

                        string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        string Url = DomailApiUrl + "InvoiceUserApi/Registration/User/InvoiceRegistration";
                        var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            //var userdata = new JavaScriptSerializer().Deserialize<ApiResult>(result);

                            //if (userdata.response_code == 1)
                            //{

                            //}
                            //else
                            //{

                            //}
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

        #endregion

        #region Api Result
        public class ApiResult
        {

            public int response_code { get; set; }
            public string response_msg { get; set; }
        }
        #endregion

        #region Report Filter
        public ActionResult ReportFilter(string Command, ReportUserForAccountant obj)
        {
            if (obj.RoleId == 1)
            {
                if (obj.UserId == 0)
                {
                    return RedirectToAction("Report", new { error = "select user." });
                }
            }

            if (Command == "CUSTOMER")
            {
                return RedirectToAction("CustomerList", new { id = obj.UserId });
            }
            if (Command == "SUPPLIER")
            {
                return RedirectToAction("SupplierList");
            }
            if (Command == "BalanceSheet")
            {
                return RedirectToAction("BalanceSheet", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });

        }
        #endregion

        #region Supplier List
        public ActionResult SupplierList(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("SupplierList");
        }

        [HttpGet]
        public JsonResult SupplierListGrid(int Id, int? page, int? limit, string sortBy, string direction, string searchString)
        {
            int total;
            var records = new GridModel().GetSupplierList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Customer List
        public ActionResult CustomerList(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("CustomerList");
        }

        [HttpGet]
        public JsonResult CustomerListGrid(int Id, int? page, int? limit, string sortBy, string direction, string searchString)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Finance Supplier Customer Dashboard
        public ActionResult FinanceSupplierCustomerDashboard(string Type)
        {
            return View();
        }
        #endregion

        #region Update Customer
        public ActionResult UpdateCustomer(int id, bool? status)
        {
            using (var db = new KFentities())
            {

                var userData = db.tblCustomerOrSuppliers.Where(s => s.Id == id).FirstOrDefault();

                var obj = new UpdateCustomer();
                if (userData != null)
                {
                    obj.Id = userData.Id;
                    obj.CompanyName = userData.Company_Name;
                    obj.ContactPerson = userData.FirstName;
                    obj.BusinessNumber = userData.Telephone;
                    string CorporateAddress = userData.Address.Replace("|||", "|");
                    string[] CorporateArray = CorporateAddress.Split('|');
                    obj.CorporateAptNo = CorporateArray[0];
                    obj.CorporateHouseNo = CorporateArray[1];
                    obj.CorporateStreet = CorporateArray[2];
                    obj.CorporateCity = userData.City;
                    obj.CorporateState = userData.State;
                    obj.CorporatePostalCode = userData.PostalCode;

                    if (CorporateArray[0] == "" && CorporateArray[1] == "")
                    {
                        obj.CustomerAddress = CorporateArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;
                    }
                    else
                    {
                        obj.CustomerAddress = CorporateArray[0] + '-' + CorporateArray[1] + "\r\n" + CorporateArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;
                    }


                    string ShippingAddress = userData.ShippingAddress.Replace("|||", "|");
                    string[] ShippingArray = ShippingAddress.Split('|');
                    obj.ShippingAptNo = ShippingArray[0];
                    obj.ShippingHouseNo = ShippingArray[1];
                    obj.ShippingStreet = ShippingArray[2];
                    obj.ShippingCity = userData.ShippingCity;
                    obj.ShippingState = userData.ShippingState;
                    obj.ShippingPostalCode = userData.ShippingPostalCode;

                    if (ShippingArray[0] == "" && ShippingArray[1] == "")
                    {
                        obj.ShippingAddress = ShippingArray[2] + '-' + userData.ShippingCity + "\r\n" + userData.ShippingState + "\r\n" + userData.ShippingPostalCode;
                    }
                    else
                    {
                        obj.ShippingAddress = ShippingArray[0] + '-' + ShippingArray[1] + "\r\n" + ShippingArray[2] + '-' + userData.ShippingCity + "\r\n" + userData.ShippingState + "\r\n" + userData.ShippingPostalCode;
                    }
                    var mob = userData.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                    obj.EmailTo = userData.Email;

                    obj.Website = userData.Website;
                    TempData["AddEmails"] = userData.AdditionalEmail;
                }

                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult UpdateCustomer(UpdateCustomer objUser, string hdnUser_updateValue)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == objUser.Id).FirstOrDefault();
                    if (userChk != null)
                    {
                        userChk.FirstName = objUser.ContactPerson;
                        userChk.Company_Name = objUser.CompanyName;
                        userChk.Mobile = objUser.AreaCode + "-" + objUser.MobileNumber;
                        userChk.PostalCode = objUser.hdncorporatepostalcode;
                        userChk.Address = objUser.hdncorporateaptno + "|||" + objUser.hdncorporatehouseno + "|||" + objUser.hdncorporatestreet;
                        userChk.AdditionalEmail = objUser.hdnemailCC;
                        userChk.City = objUser.hdncorporatecity;
                        //userChk.ServiceOffered = objUser.ServiceOffered;
                        userChk.ShippingAddress = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.ShippingCity = objUser.hdnshippingcity;
                        userChk.ShippingPostalCode = objUser.hdnshippingpostalcode;
                        userChk.ShippingState = objUser.hdnshippingstate;
                        userChk.State = objUser.hdncorporatestate;
                        userChk.Website = objUser.Website;
                        userChk.Telephone = objUser.BusinessNumber;
                    }

                    context.SaveChanges();

                    TempData["customerupdatedetails"] = "Success";
                    return RedirectToAction("UpdateCustomer", new { id = objUser.Id, status = true });
                }

            }
            catch (Exception ex)
            {

            }

            return View(objUser);
        }
        #endregion

        #region Update Supplier
        public ActionResult UpdateSupplier(int id, bool? status)
        {
            using (var db = new KFentities())
            {

                var userData = db.tblCustomerOrSuppliers.Where(s => s.Id == id).FirstOrDefault();

                var obj = new UpdateSupplier();
                if (userData != null)
                {
                    obj.Id = userData.Id;
                    obj.CompanyName = userData.Company_Name;
                    obj.ContactPerson = userData.FirstName;
                    obj.BusinessNumber = userData.Telephone;

                    string ShippingAddress = userData.Address.Replace("|||", "|");
                    string[] ShippingArray = ShippingAddress.Split('|');
                    obj.ShippingAptNo = ShippingArray[0];
                    obj.ShippingHouseNo = ShippingArray[1];
                    obj.ShippingStreet = ShippingArray[2];
                    obj.ShippingCity = userData.City;
                    obj.ShippingState = userData.State;
                    obj.ShippingPostalCode = userData.PostalCode;
                    obj.SupplierAddress = ShippingArray[0] + '-' + ShippingArray[1] + "\r\n" + ShippingArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;

                    var mob = userData.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                    obj.EmailTo = userData.Email;

                    obj.Website = userData.Website;
                    obj.GoodsservicesList = ClsDropDownList.PopulateIndustry();


                    foreach (var itms in ClsDropDownList.PopulateIndustry())
                    {
                        if (itms.Text == userData.ServiceOffered)
                        {
                            obj.GoodsservicesId = Convert.ToInt32(itms.Value);
                        }
                    }




                }

                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult UpdateSupplier(UpdateSupplier objUser, string hdnUser_updateValue)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == objUser.Id).FirstOrDefault();
                    if (userChk != null)
                    {
                        userChk.FirstName = objUser.ContactPerson;
                        userChk.Company_Name = objUser.CompanyName;
                        userChk.Mobile = objUser.AreaCode + "-" + objUser.MobileNumber;
                        userChk.PostalCode = objUser.hdnshippingpostalcode;
                        userChk.Address = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.AdditionalEmail = objUser.EmailCc;
                        userChk.City = objUser.hdnshippingcity;
                        userChk.ServiceOffered = objUser.ServiceOffered;
                        userChk.ShippingAddress = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.ShippingCity = objUser.hdnshippingcity;
                        userChk.ShippingPostalCode = objUser.hdnshippingpostalcode;
                        userChk.ShippingState = objUser.hdnshippingstate;
                        userChk.State = objUser.hdnshippingstate;
                        userChk.Website = objUser.Website;
                        userChk.Telephone = objUser.BusinessNumber;




                        foreach (var itms in ClsDropDownList.PopulateIndustry())
                        {
                            if (Convert.ToInt32(itms.Value) == Convert.ToInt32(objUser.GoodsservicesId))
                            {
                                userChk.ServiceOffered = itms.Text;
                            }
                        }



                    }

                    context.SaveChanges();

                    TempData["supplierupdatedetails"] = "Success";
                    return RedirectToAction("UpdateSupplier", new { id = objUser.Id, status = true });
                }

            }
            catch (Exception ex)
            {

            }

            return View(objUser);
        }
        #endregion

        #region Customer Register
        public ActionResult CustomerRegister()
        {
            var obj = new CreateCustomer();
            if (TempData["Finance"] != null)
            {
                dynamic lst_finance = TempData["Finance"];

                obj.Company_Name = lst_finance.Company_Name;
                obj.FirstName = lst_finance.FirstName;
                if (lst_finance.Mobile.Split('-') != null)
                {
                    var mob = lst_finance.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                }

                obj.Email = lst_finance.Email;
                obj.Telephone = lst_finance.Telephone;
                obj.City = lst_finance.City;
                obj.Address = lst_finance.Address;
                obj.DateCreated = lst_finance.DateCreated;
                obj.PostalCode = lst_finance.PostalCode;
                obj.UserId = lst_finance.Id;
                obj.IsFinance = true;
            }
            if (TempData["Invoice"] != null)
            {


                dynamic lst_invoice = TempData["Invoice"];

                obj.Company_Name = lst_invoice.Company_Name;
                obj.FirstName = lst_invoice.FirstName;
                if (lst_invoice.Mobile.Split('-') != null)
                {
                    var mob = lst_invoice.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                }

                obj.Email = lst_invoice.Email;

                obj.Address = lst_invoice.Address;
                obj.DateCreated = lst_invoice.DateCreated;

                obj.PostalCode = lst_invoice.PostalCode;
                obj.City = lst_invoice.City;
                obj.ShippingAddress = lst_invoice.ShippingAddress;
                obj.ShippingCity = lst_invoice.ShippingCity;
                obj.ShippingPostalCode = lst_invoice.ShippingPostalCode;
                obj.Website = lst_invoice.Website;
                obj.IsFinance = false;

            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult CustomerRegister(CreateCustomer objCustomer)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        // CreateCustomer obj = new CreateCustomer();
                        obj.Company_Name = objCustomer.Company_Name;
                        obj.FirstName = objCustomer.FirstName;
                        obj.Address = objCustomer.hdncorporateaptno + "|||" + objCustomer.hdncorporatehouseno + "|||" + objCustomer.hdncorporatestreet;
                        obj.City = objCustomer.hdncorporatecity;
                        obj.State = objCustomer.hdncorporatestate;
                        obj.PostalCode = objCustomer.hdncorporatepostalcode;
                        obj.ShippingAddress = objCustomer.hdnshippingaptno + "|||" + objCustomer.hdnshippinghouseno + "|||" + objCustomer.hdnshippingstreet;
                        obj.ShippingCity = objCustomer.hdnshippingcity;
                        obj.ShippingState = objCustomer.hdnshippingstate;
                        obj.ShippingPostalCode = objCustomer.hdnshippingpostalcode;
                        obj.Mobile = objCustomer.AreaCode + "-" + objCustomer.MobileNumber;
                        obj.Email = objCustomer.Email;
                        obj.AdditionalEmail = objCustomer.hdnemailCC;
                        obj.Website = objCustomer.Website;
                        obj.Telephone = objCustomer.Telephone;
                        obj.RoleId = 2;
                        obj.UserId = objCustomer.UserId;


                        using (var repository = new InvoiceUserRegistrationRepository())
                        {

                            if (obj != null)
                            {
                                var userid = repository.RegisterInvoiceCustomerSupplier(obj);
                                if (userid > 0)
                                {
                                    TempData["customercreatedetails"] = "Success";
                                    return RedirectToAction("CustomerRegister", "Invoice", new { Selectuser = false });
                                }
                                else
                                {
                                    objCustomer.CorporateAptNo = objCustomer.hdncorporateaptno;
                                    objCustomer.CorporateHouseNo = objCustomer.hdncorporatehouseno;
                                    objCustomer.CorporateStreet = objCustomer.hdncorporatestreet;
                                    objCustomer.CorporateCity = objCustomer.hdncorporatecity;
                                    objCustomer.CorporateState = objCustomer.hdncorporatestate;
                                    objCustomer.CorporatePostalCode = objCustomer.hdncorporatepostalcode;

                                    objCustomer.ShippingAptNo = objCustomer.hdnshippingaptno;
                                    objCustomer.ShippingHouseNo = objCustomer.hdnshippinghouseno;
                                    objCustomer.ShippingStreet = objCustomer.hdnshippingstreet;
                                    objCustomer.ShippingCity = objCustomer.hdnshippingcity;
                                    objCustomer.ShippingState = objCustomer.hdnshippingstate;
                                    objCustomer.ShippingPostalCode = objCustomer.hdnshippingpostalcode;

                                    TempData["customeralready"] = "Success";
                                    return View(objCustomer);


                                }
                            }
                            else
                            {
                                //return new ApiResponseDto { ResponseMessage = "Please provide data for registration.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
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

            return View();
        }
        #endregion

        #region Search Customer List Email
        [HttpPost]
        public ActionResult SearchCustomerListEmail(CheckInvoice chkInvoice)
        {
            try
            {

                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.RoleId = 2;
                        obj.UserId = chkInvoice.UserId;
                        obj.Email = chkInvoice.EmailCc;
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
                                                // return new AddCustomerSupplierDto { ResponseMessage = "Added Successfully from Existing.", ResponseCode = (int)ApiStatusCode.Success };
                                            }
                                            else
                                            {
                                                var Financedata = repository.FinanceAlreadyExist(obj);
                                                if (Financedata != null)
                                                {
                                                    Financedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Financedata.ResponseMessage = "Success";
                                                    //return Financedata;
                                                    TempData["Finance"] = Financedata;
                                                    return RedirectToAction("CustomerRegister");
                                                    // TempData["customerdetails6"] = "Success";
                                                    //return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 11 });
                                                }

                                                var Invoicedata = repository.InvoiceAlreadyExist(obj);
                                                if (Invoicedata != null)
                                                {
                                                    Invoicedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Invoicedata.ResponseMessage = "Success";
                                                    TempData["Invoice"] = Financedata;
                                                    return RedirectToAction("CustomerRegister");
                                                    //return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 12 });
                                                }
                                                else
                                                {
                                                    // return new AddCustomerSupplierDto { ResponseMessage = "Not Register yet.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                                    TempData["customerdetails6"] = "Success";
                                                    return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 6 });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            // return new AddCustomerSupplierDto { ResponseMessage = "Already Registered.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                                            TempData["customerdetails3"] = "Success";
                                            return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 3 });
                                        }

                                    }
                                    else
                                    {
                                        // return new AddCustomerSupplierDto { ResponseMessage = "You cannot add yourself as a supplier/customer.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                        TempData["customerdetails4"] = "Success";
                                        return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 4 });
                                    }
                                }
                                else
                                {
                                    //return new AddCustomerSupplierDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter };

                                    TempData["customerdetails5"] = "Success";
                                    return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 5 });
                                }

                            }

                        }
                        catch (Exception)
                        {
                            // return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                            TempData["customerdetails2"] = "Success";
                            return RedirectToAction("CustomerListNew", "Invoice", new { Selectuser = 2 });
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/CheckInvoiceExistOrNot";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    var desc = data.ResponseMessage;


                        //    if (desc == "Not Register yet.")
                        //    {
                        //        TempData["customerdetails6"] = "Success";
                        //        return RedirectToAction("CustomerList", "Invoice", new { Selectuser = 6 });
                        //    }
                        //    else if (desc == "Already Registered.")
                        //    {
                        //        TempData["customerdetails3"] = "Success";
                        //        return RedirectToAction("CustomerList", "Invoice", new { Selectuser = 3 });
                        //    }
                        //    else if (desc == "You cannot add yourself as a supplier/customer.")
                        //    {
                        //        TempData["customerdetails4"] = "Success";
                        //        return RedirectToAction("CustomerList", "Invoice", new { Selectuser = 4 });
                        //    }
                        //    else if (desc == "Please provide data.")
                        //    {
                        //        TempData["customerdetails5"] = "Success";
                        //        return RedirectToAction("CustomerList", "Invoice", new { Selectuser = 5 });
                        //    }
                        //    else if (desc == "Unable to connect to server, Please try again later.")
                        //    {
                        //        TempData["customerdetails2"] = "Success";
                        //        return RedirectToAction("CustomerList", "Invoice", new { Selectuser = 2 });
                        //    }
                        //    else
                        //    {
                        //        TempData["customerdetails"] = "Success";
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }

                        //}
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
            return RedirectToAction("CustomerList", "Invoice", new { Selectuser = false });
            //return View();
        }
        #endregion

        #region Supplier Register
        public ActionResult SupplierRegister()
        {
            CreateSupplier supplier = new CreateSupplier();
            supplier.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            return View(supplier);

        }

        [HttpPost]
        public ActionResult SupplierRegister(CreateSupplier objSupplier)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.Company_Name = objSupplier.Company_Name;
                        obj.FirstName = objSupplier.FirstName;
                        obj.Address = objSupplier.hdnshippingaptno + "|||" + objSupplier.hdnshippinghouseno + "|||" + objSupplier.hdnshippingstreet;
                        obj.City = objSupplier.hdnshippingcity;
                        obj.State = objSupplier.hdnshippingstate;
                        obj.PostalCode = objSupplier.hdnshippingpostalcode;
                        obj.Address = objSupplier.hdnshippingaptno + "|||" + objSupplier.hdnshippinghouseno + "|||" + objSupplier.hdnshippingstreet;

                        obj.Mobile = objSupplier.AreaCode + "-" + objSupplier.MobileNumber;
                        obj.Email = objSupplier.Email;
                        //obj.AdditionalEmail = objSupplier.hdnemailCC;
                        obj.Website = objSupplier.Website;
                        obj.Telephone = objSupplier.Telephone;
                        obj.RoleId = 1;
                        obj.UserId = objSupplier.UserId;

                        foreach (var itms in ClsDropDownList.PopulateIndustry())
                        {
                            if (Convert.ToInt32(itms.Value) == Convert.ToInt32(objSupplier.GoodsservicesId))
                            {
                                obj.ServiceOffered = itms.Text;
                            }
                        }
                        try
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                if (obj != null)
                                {
                                    var userid = repository.RegisterInvoiceCustomerSupplier(obj);
                                    if (userid > 0)
                                    {
                                        // return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                                        TempData["suppliercreatedetails"] = "Success";
                                        return RedirectToAction("SupplierRegister", "Invoice", new { Selectuser = false });
                                    }
                                    else
                                    {
                                        objSupplier.ShippingAptNo = objSupplier.hdnshippingaptno;
                                        objSupplier.ShippingHouseNo = objSupplier.hdnshippinghouseno;
                                        objSupplier.ShippingStreet = objSupplier.hdnshippingstreet;
                                        objSupplier.ShippingCity = objSupplier.hdnshippingcity;
                                        objSupplier.ShippingState = objSupplier.hdnshippingstate;
                                        objSupplier.ShippingPostalCode = objSupplier.hdnshippingpostalcode;
                                        objSupplier.GoodsservicesList = ClsDropDownList.PopulateIndustry();
                                        TempData["supplieralready"] = "Success";
                                        return View(objSupplier);

                                    }
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Please provide data for registration.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/RegisterInvoiceCustomerSupplier";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        TempData["suppliercreatedetails"] = "Success";
                        //        return RedirectToAction("SupplierRegister", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Invoice Supplier Register
        public ActionResult InvoiceSupplierRegister()
        {
            CreateSupplier supplier = new CreateSupplier();
            supplier.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            return View(supplier);

        }

        [HttpPost]
        public ActionResult InvoiceSupplierRegister(CreateSupplier objSupplier)
        {
            try
            {
                using (var context = new KFentities())
                {

                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.Company_Name = objSupplier.Company_Name;
                        obj.FirstName = objSupplier.FirstName;
                        obj.Address = objSupplier.hdnshippingaptno + "|||" + objSupplier.hdnshippinghouseno + "|||" + objSupplier.hdnshippingstreet;
                        obj.City = objSupplier.hdnshippingcity;
                        obj.State = objSupplier.hdnshippingstate;
                        obj.PostalCode = objSupplier.hdnshippingpostalcode;
                        obj.Address = objSupplier.hdnshippingaptno + "|||" + objSupplier.hdnshippinghouseno + "|||" + objSupplier.hdnshippingstreet;

                        obj.Mobile = objSupplier.AreaCode + "-" + objSupplier.MobileNumber;
                        obj.Email = objSupplier.Email;
                        obj.AdditionalEmail = objSupplier.hdnemailCC;
                        obj.Website = objSupplier.Website;
                        obj.Telephone = objSupplier.Telephone;
                        obj.RoleId = 1;
                        obj.UserId = objSupplier.UserId;

                        foreach (var itms in ClsDropDownList.PopulateIndustry())
                        {
                            if (Convert.ToInt32(itms.Value) == Convert.ToInt32(objSupplier.GoodsservicesId))
                            {
                                obj.ServiceOffered = itms.Text;
                            }
                        }
                        try
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                if (obj != null)
                                {
                                    var userid = repository.RegisterInvoiceCustomerSupplier(obj);
                                    if (userid > 0)
                                    {
                                        // return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                                        TempData["suppliercreatedetails"] = "Success";
                                        //return RedirectToAction("InvocieSupplierRegister", "Invoice", new { Selectuser = false });
                                    }
                                    else
                                    {
                                        objSupplier.ShippingAptNo = objSupplier.hdnshippingaptno;
                                        objSupplier.ShippingHouseNo = objSupplier.hdnshippinghouseno;
                                        objSupplier.ShippingStreet = objSupplier.hdnshippingstreet;
                                        objSupplier.ShippingCity = objSupplier.hdnshippingcity;
                                        objSupplier.ShippingState = objSupplier.hdnshippingstate;
                                        objSupplier.ShippingPostalCode = objSupplier.hdnshippingpostalcode;
                                        objSupplier.GoodsservicesList = ClsDropDownList.PopulateIndustry();

                                        TempData["supplieralready"] = "Success";
                                        return View(objSupplier);
                                    }
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Please provide data for registration.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
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
            CreateSupplier supplier = new CreateSupplier();
            supplier.GoodsservicesList = ClsDropDownList.PopulateIndustry();
            return View(supplier);
        }
        #endregion

        #region Search Supplier List Email
        [HttpPost]
        public ActionResult SearchSupplierListEmail(CheckSupplier chkSupplier)
        {
            try
            {

                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.RoleId = 1;
                        obj.UserId = chkSupplier.UserId;
                        obj.Email = chkSupplier.EmailTo;
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
                                                // return new AddCustomerSupplierDto { ResponseMessage = "Added Successfully from Existing.", ResponseCode = (int)ApiStatusCode.Success };
                                            }
                                            else
                                            {
                                                var Financedata = repository.FinanceAlreadyExist(obj);
                                                if (Financedata != null)
                                                {
                                                    Financedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Financedata.ResponseMessage = "Success";
                                                    // return Financedata;
                                                }
                                                var Invoicedata = repository.InvoiceAlreadyExist(obj);
                                                if (Invoicedata != null)
                                                {
                                                    Invoicedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Invoicedata.ResponseMessage = "Success";
                                                    // return Invoicedata;
                                                }
                                                else
                                                {
                                                    // return new AddCustomerSupplierDto { ResponseMessage = "Not Register yet.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                                    TempData["customerdetails6"] = "Success";
                                                    return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 6 });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            // return new AddCustomerSupplierDto { ResponseMessage = "Already Registered.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                                            TempData["customerdetails3"] = "Success";
                                            return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 3 });
                                        }

                                    }
                                    else
                                    {
                                        // return new AddCustomerSupplierDto { ResponseMessage = "You cannot add yourself as a supplier/customer.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                        TempData["customerdetails4"] = "Success";
                                        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 4 });
                                    }
                                }
                                else
                                {
                                    //return new AddCustomerSupplierDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter };

                                    TempData["customerdetails5"] = "Success";
                                    return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 5 });
                                }

                            }

                        }
                        catch (Exception)
                        {
                            // return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                            TempData["customerdetails2"] = "Success";
                            return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 2 });
                        }











                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/CheckInvoiceExistOrNot";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    var desc = data.ResponseMessage;


                        //    if (desc == "Not Register yet.")
                        //    {
                        //        TempData["customerdetails6"] = "Success";
                        //        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 6 });
                        //    }
                        //    else if (desc == "Already Registered.")
                        //    {
                        //        TempData["customerdetails3"] = "Success";
                        //        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 3 });
                        //    }
                        //    else if (desc == "You cannot add yourself as a supplier/customer.")
                        //    {
                        //        TempData["customerdetails4"] = "Success";
                        //        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 4 });
                        //    }
                        //    else if (desc == "Please provide data.")
                        //    {
                        //        TempData["customerdetails5"] = "Success";
                        //        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 5 });
                        //    }
                        //    else if (desc == "Unable to connect to server, Please try again later.")
                        //    {
                        //        TempData["customerdetails2"] = "Success";
                        //        return RedirectToAction("SupplierList", "Invoice", new { Selectuser = 2 });
                        //    }
                        //    else
                        //    {
                        //        TempData["customerdetails"] = "Success";
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }

                        //}
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
            return RedirectToAction("CustomerList", "Invoice", new { Id = 0 });
            //return View();
        }
        #endregion

        #region Create Invoice
        public ActionResult CreateInvoice(int Id)
        {

            var obj = new CreateInvoice();
            using (var context = new KFentities())
            {
                if (Id != 0)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            int UserIds = 0;
                            int RoleId = 0;
                            var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                UserIds = Convert.ToInt32(userChk.UserId);
                                RoleId = Convert.ToInt32(userChk.RoleId);
                            }

                            try
                            {
                                if (UserIds > 0)
                                {
                                    using (var repo = new InvoiceUserRegistrationRepository())
                                    {
                                        int invoiceNumber = repo.CreateInvoiceNumber(UserIds);
                                        if (invoiceNumber > 0)
                                        {
                                            //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                                            if (invoiceNumber.ToString().Length == 1)
                                            {
                                                obj.InvoiceNumber = "0000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 2)
                                            {
                                                obj.InvoiceNumber = "000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 3)
                                            {
                                                obj.InvoiceNumber = "00" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 4)
                                            {
                                                obj.InvoiceNumber = "0" + invoiceNumber;
                                            }
                                            else
                                            {
                                                obj.InvoiceNumber = invoiceNumber.ToString();
                                            }
                                            obj.hdnselectuser = Id;
                                            obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                                        }
                                        else
                                        {
                                            //  return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                        }
                                    }
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    //var values = new NameValueCollection();
                    //values["UserId"] = UserIds.ToString();


                    //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();


                    //var response = client.UploadValues(DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoiceNumber/" + UserIds, values);

                    //var responseString = Encoding.Default.GetString(response);

                    //dynamic data = JObject.Parse(responseString);
                    //var res = data.ResponseCode;
                    //String UserId = data.UserId;
                    //if (res == 1)
                    //{

                    //    if (UserId.Length == 1)
                    //    {
                    //        obj.InvoiceNumber = "0000" + UserId;
                    //    }
                    //    else if (UserId.Length == 2)
                    //    {
                    //        obj.InvoiceNumber = "000" + UserId;
                    //    }
                    //    else if (UserId.Length == 3)
                    //    {
                    //        obj.InvoiceNumber = "00" + UserId;
                    //    }
                    //    else if (UserId.Length == 4)
                    //    {
                    //        obj.InvoiceNumber = "0" + UserId;
                    //    }
                    //    else
                    //    {
                    //        obj.InvoiceNumber = UserId;
                    //    }
                    //    obj.hdnselectuser = Id;

                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                    // }
                    //obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                    //}
                    //}

                }
                else
                {
                    return View();
                }
            }



            return View(obj);

        }

        [HttpPost]
        public ActionResult CreateInvoice(InvoiceDetailDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<InvoiceDetailDto> model = new List<InvoiceDetailDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);
            Console.WriteLine(data1.UserId);
            int Ids = Convert.ToInt32(data1.UserId);

            //InvoiceDetailDto obj = new InvoiceDetailDto();
            //obj.DueDate = data1.DueDate;
            //obj.UserId = data1.UserId;
            //obj.DepositePayment = data1.DepositePayment;
            //obj.InvoiceDate = data1.InvoiceDate;
            //obj.PaymentTerms = data1.PaymentTerms;
            //obj.SalesPerson = data1.SalesPerson;
            //obj.Item = data1.Item;
            //obj.QST_Tax = data1.QST_Tax;
            //obj.Total = data1.Total;
            //obj.SubTotal = data1.SubTotal;
            //obj.HST_Tax = data1.HST_Tax;
            //obj.CustomerId = data1.CustomerId;
            //obj.Rate = data1.Rate;
            //obj.Discount = data1.Discount;
            //obj.Customer_ServiceTypeId = data1.Customer_ServiceTypeId;
            //obj.ShippingCost = data1.ShippingCost;
            //obj.ServiceTypeId = data1.ServiceTypeId;
            //obj.ButtonType = data1.ButtonType;
            //obj.Type = data1.Type;
            //obj.Tax = data1.Tax;
            //obj.GST_Tax = data1.GST_Tax;
            //obj.Note = data1.Note;
            //obj.ItemId = data1.ItemId;
            //obj.Terms = data1.Terms;
            //obj.Quantity = data1.Quantity;
            //obj.Description = data1.Description;
            //obj.InvoiceNumber = data1.InvoiceNumber;
            //obj.Customer_Service = data1.Customer_Service;
            //obj.PST_Tax = data1.PST_Tax;
            //obj.DocumentRef = data1.DocumentRef;
            //obj.BalanceDue = data1.BalanceDue;

            //try
            //{
            //    using (var repository = new InvoiceUserRegistrationRepository())
            //    {
            //        if (requestBody != null)
            //        {
            //            var userid = repository.CreateInvoice(obj);
            //            if (userid > 0)
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
            //                TempData["Invoicesuccessfullycreated"] = "Success";
            //                return RedirectToAction("CreateInvoice", "Invoice");
            //            }
            //            else
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };

            //            }
            //        }
            //        else
            //        {
            //            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            //}

            //TempData["Invoicesuccessfullycreated"] = "Success";
            //return RedirectToAction("CreateInvoice", "Invoice", new { Id = 0 });

            //string Url = "http://kippinfinance.web1.anzleads.com/MobileApi/InvoiceUserApi/Registration/CreateInvoice";
            string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
            string Url = DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoice";

            var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            var res = 0;
            using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(result);
                res = data.ResponseCode;

            }
            if (res == 1)
            {
                TempData["Invoicesuccessfullycreated"] = "Success";
                //return RedirectToAction("CreateInvoice", "Invoice", new { Id = Ids });
                return RedirectToAction("CreateInvoice", "Invoice");
            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                return View();
            }

            TempData["Invoicesuccessfullycreated"] = "Success";
            return RedirectToAction("CreateInvoice", "Invoice", new { Id = 0 });
            //return View();
        }
        #endregion

        #region Create Proforma
        public ActionResult CreateProforma(int Id)
        {

            var obj = new CreateInvoice();
            using (var context = new KFentities())
            {
                if (Id != 0)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            int UserIds = 0;
                            int RoleId = 0;
                            var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                UserIds = Convert.ToInt32(userChk.UserId);
                                RoleId = Convert.ToInt32(userChk.RoleId);
                            }

                            try
                            {
                                if (UserIds > 0)
                                {
                                    using (var repo = new InvoiceUserRegistrationRepository())
                                    {
                                        int invoiceNumber = repo.CreateInvoiceNumber(UserIds);
                                        if (invoiceNumber > 0)
                                        {
                                            //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                                            if (invoiceNumber.ToString().Length == 1)
                                            {
                                                obj.InvoiceNumber = "0000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 2)
                                            {
                                                obj.InvoiceNumber = "000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 3)
                                            {
                                                obj.InvoiceNumber = "00" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 4)
                                            {
                                                obj.InvoiceNumber = "0" + invoiceNumber;
                                            }
                                            else
                                            {
                                                obj.InvoiceNumber = invoiceNumber.ToString();
                                            }
                                            obj.hdnselectuser = Id;
                                            obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                                        }
                                        else
                                        {
                                            //  return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                        }
                                    }
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            //var values = new NameValueCollection();
                            //values["UserId"] = UserIds.ToString();
                            //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                            //var response = client.UploadValues(DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoiceNumber/" + UserIds, values);

                            //var responseString = Encoding.Default.GetString(response);

                            //dynamic data = JObject.Parse(responseString);
                            //var res = data.ResponseCode;
                            //String UserId = data.UserId;
                            //if (res == 1)
                            //{

                            //    if (UserId.Length == 1)
                            //    {
                            //        obj.InvoiceNumber = "0000" + UserId;
                            //    }
                            //    else if (UserId.Length == 2)
                            //    {
                            //        obj.InvoiceNumber = "000" + UserId;
                            //    }
                            //    else if (UserId.Length == 3)
                            //    {
                            //        obj.InvoiceNumber = "00" + UserId;
                            //    }
                            //    else if (UserId.Length == 4)
                            //    {
                            //        obj.InvoiceNumber = "0" + UserId;
                            //    }
                            //    else
                            //    {
                            //        obj.InvoiceNumber = UserId;
                            //    }
                            //    obj.hdnselectuser = Id;

                            //}
                            //else
                            //{
                            //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                            //}
                            //obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {

                }
            }
            return View(obj);

        }

        [HttpPost]
        public ActionResult CreateProforma(CreateInvoice g)
        {
            var resolveRequest = HttpContext.Request;
            List<CreateInvoice> model = new List<CreateInvoice>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);
            Console.WriteLine(data1.UserId);
            int Ids = Convert.ToInt32(data1.UserId);

            //try
            //{
            //    using (var repository = new InvoiceUserRegistrationRepository())
            //    {
            //        if (requestBody != null)
            //        {
            //            var userid = repository.CreateInvoice(data1);
            //            if (userid > 0)
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
            //                TempData["Proformasuccessfullycreated"] = "Success";
            //                return RedirectToAction("CreateProforma", "Invoice");
            //            }
            //            else
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };

            //            }
            //        }
            //        else
            //        {
            //            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            //}

            string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
            string Url = DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoice";
            var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            var res = 0;
            using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(result);
                res = data.ResponseCode;

            }
            if (res == 1)
            {
                TempData["Proformasuccessfullycreated"] = "Success";
                return RedirectToAction("CreateProforma", "Invoice");
            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                return View();
            }

            TempData["Proformasuccessfullycreated"] = "Success";
            return RedirectToAction("CreateProforma", "Invoice", new { Id = 0 });
        }
        #endregion

        #region Customer List Proforma New
        public ActionResult CustomerListProformaNew()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CustomerListGridProformaNew(int Id, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Report Filter
        public ActionResult InvoiceReportFilter(string Command, ReportUserForAccountant obj)
        {
            if (obj.RoleId == 1)
            {
                if (obj.UserId == 0)
                {
                    return RedirectToAction("Report", new { error = "select user." });
                }
            }
            //var userdata = UserData.GetCurrentUserData();
            //if(userdata.RoleId != 1)
            //{
            //    obj.UserId = userdata.Id;
            //}
            if (Command == "CUSTOMER")
            {
                return RedirectToAction("InvoiceCustomerList", new { id = obj.UserId });
            }
            if (Command == "SUPPLIER")
            {
                return RedirectToAction("InvoiceSupplierList");
            }
            if (Command == "BalanceSheet")
            {
                return RedirectToAction("BalanceSheet", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });

        }
        #endregion

        #region Invoice Customer Register
        public ActionResult InvoiceCustomerRegister()
        {
            var obj = new CreateCustomer();
            if (TempData["Finance"] != null)
            {
                dynamic lst_finance = TempData["Finance"];

                obj.Company_Name = lst_finance.Company_Name;
                obj.FirstName = lst_finance.FirstName;
                if (lst_finance.Mobile.Split('-') != null)
                {
                    var mob = lst_finance.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                }

                obj.Email = lst_finance.Email;
                obj.Telephone = lst_finance.Telephone;
                obj.City = lst_finance.City;
                obj.Address = lst_finance.Address;
                obj.DateCreated = lst_finance.DateCreated;
                obj.PostalCode = lst_finance.PostalCode;
                obj.UserId = lst_finance.Id;
                obj.IsFinance = true;
            }
            if (TempData["Invoice"] != null)
            {


                dynamic lst_invoice = TempData["Invoice"];

                obj.Company_Name = lst_invoice.Company_Name;
                obj.FirstName = lst_invoice.FirstName;
                if (lst_invoice.Mobile.Split('-') != null)
                {
                    var mob = lst_invoice.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                }

                obj.Email = lst_invoice.Email;

                obj.Address = lst_invoice.Address;
                obj.DateCreated = lst_invoice.DateCreated;

                obj.PostalCode = lst_invoice.PostalCode;
                obj.City = lst_invoice.City;
                obj.ShippingAddress = lst_invoice.ShippingAddress;
                obj.ShippingCity = lst_invoice.ShippingCity;
                obj.ShippingPostalCode = lst_invoice.ShippingPostalCode;
                obj.Website = lst_invoice.Website;
                obj.IsFinance = false;

            }

            return View(obj);
        }

        [HttpPost]
        public ActionResult InvoiceCustomerRegister(CreateCustomer objCustomer)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.Company_Name = objCustomer.Company_Name;
                        obj.FirstName = objCustomer.FirstName;
                        obj.Address = objCustomer.hdncorporateaptno + "|||" + objCustomer.hdncorporatehouseno + "|||" + objCustomer.hdncorporatestreet;
                        obj.City = objCustomer.hdncorporatecity;
                        obj.State = objCustomer.hdncorporatestate;
                        obj.PostalCode = objCustomer.hdncorporatepostalcode;
                        obj.ShippingAddress = objCustomer.hdnshippingaptno + "|||" + objCustomer.hdnshippinghouseno + "|||" + objCustomer.hdnshippingstreet;
                        obj.ShippingCity = objCustomer.hdnshippingcity;
                        obj.ShippingState = objCustomer.hdnshippingstate;
                        obj.ShippingPostalCode = objCustomer.hdnshippingpostalcode;
                        obj.Mobile = objCustomer.AreaCode + "-" + objCustomer.MobileNumber;
                        obj.Email = objCustomer.Email;
                        obj.AdditionalEmail = objCustomer.hdnemailCC;
                        obj.Website = objCustomer.Website;
                        obj.Telephone = objCustomer.Telephone;
                        obj.RoleId = 2;
                        obj.UserId = objCustomer.UserId;


                        try
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                if (obj != null)
                                {
                                    var userid = repository.RegisterInvoiceCustomerSupplier(obj);
                                    if (userid > 0)
                                    {
                                        // return new ApiResponseDto { ResponseMessage = "User successfully registered.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
                                        TempData["customercreatedetails"] = "Success";
                                        return RedirectToAction("InvoiceCustomerRegister", "Invoice", new { Selectuser = false });
                                    }
                                    else
                                    {
                                        objCustomer.CorporateAptNo = objCustomer.hdncorporateaptno;
                                        objCustomer.CorporateHouseNo = objCustomer.hdncorporatehouseno;
                                        objCustomer.CorporateStreet = objCustomer.hdncorporatestreet;
                                        objCustomer.CorporateCity = objCustomer.hdncorporatecity;
                                        objCustomer.CorporateState = objCustomer.hdncorporatestate;
                                        objCustomer.CorporatePostalCode = objCustomer.hdncorporatepostalcode;

                                        objCustomer.ShippingAptNo = objCustomer.hdnshippingaptno;
                                        objCustomer.ShippingHouseNo = objCustomer.hdnshippinghouseno;
                                        objCustomer.ShippingStreet = objCustomer.hdnshippingstreet;
                                        objCustomer.ShippingCity = objCustomer.hdnshippingcity;
                                        objCustomer.ShippingState = objCustomer.hdnshippingstate;
                                        objCustomer.ShippingPostalCode = objCustomer.hdnshippingpostalcode;

                                        TempData["customeralready"] = "Success";
                                        return View(objCustomer);
                                    }
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data for registration.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                        }










                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/RegisterInvoiceCustomerSupplier";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        TempData["customercreatedetails"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerRegister", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Search Invoice Customer List Email
        [HttpPost]
        public ActionResult SearchInvoiceCustomerListEmail(CheckInvoice chkInvoice)
        {
            try
            {

                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.RoleId = 2;
                        obj.UserId = chkInvoice.UserId;
                        obj.Email = chkInvoice.EmailCc;




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
                                                //return new AddCustomerSupplierDto { ResponseMessage = "Added Successfully from Existing.", ResponseCode = (int)ApiStatusCode.Success };
                                            }
                                            else
                                            {
                                                var Financedata = repository.FinanceAlreadyExist(obj);
                                                if (Financedata != null)
                                                {
                                                    Financedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Financedata.ResponseMessage = "Success";
                                                    // return Financedata;
                                                    TempData["Finance"] = Financedata;
                                                    return RedirectToAction("InvoiceCustomerRegister");
                                                }
                                                var Invoicedata = repository.InvoiceAlreadyExist(obj);
                                                if (Invoicedata != null)
                                                {
                                                    Invoicedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Invoicedata.ResponseMessage = "Success";
                                                    //  return Invoicedata;
                                                    TempData["Invoice"] = Invoicedata;
                                                    return RedirectToAction("InvoiceCustomerRegister");
                                                }
                                                else
                                                {
                                                    // return new AddCustomerSupplierDto { ResponseMessage = "Not Register yet.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                                    TempData["customerdetails6"] = "Success";
                                                    return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 6 });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            //return new AddCustomerSupplierDto { ResponseMessage = "Already Registered.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                                            TempData["customerdetails3"] = "Success";
                                            return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 3 });
                                        }

                                    }
                                    else
                                    {
                                        //return new AddCustomerSupplierDto { ResponseMessage = "You cannot add yourself as a supplier/customer.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                        TempData["customerdetails4"] = "Success";
                                        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 4 });
                                    }
                                }
                                else
                                {
                                    //return new AddCustomerSupplierDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                    TempData["customerdetails5"] = "Success";
                                    return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 5 });
                                }

                            }

                        }
                        catch (Exception)
                        {
                            //return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                            TempData["customerdetails2"] = "Success";
                            return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 2 });
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/CheckInvoiceExistOrNot";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    var desc = data.ResponseMessage;


                        //    if (desc == "Not Register yet.")
                        //    {
                        //        TempData["customerdetails6"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 6 });
                        //    }
                        //    else if (desc == "Already Registered.")
                        //    {
                        //        TempData["customerdetails3"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 3 });
                        //    }
                        //    else if (desc == "You cannot add yourself as a supplier/customer.")
                        //    {
                        //        TempData["customerdetails4"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 4 });
                        //    }
                        //    else if (desc == "Please provide data.")
                        //    {
                        //        TempData["customerdetails5"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 5 });
                        //    }
                        //    else if (desc == "Unable to connect to server, Please try again later.")
                        //    {
                        //        TempData["customerdetails2"] = "Success";
                        //        return RedirectToAction("InvoiceCustomerList", "Invoice", new { Selectuser = 2 });
                        //    }
                        //    else
                        //    {
                        //        TempData["customerdetails"] = "Success";
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }

                        //}
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
            return RedirectToAction("CustomerList", "Invoice", new { Selectuser = false });
            //return View();
        }
        #endregion

        #region Customer List New
        public ActionResult CustomerListNew(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("CustomerListNew");
        }

        [HttpGet]
        public JsonResult CustomerListGridNew(int Id, int? page, int? limit, string sortBy, string direction, string searchString)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Item Details
        public ActionResult ItemDetails()
        {
            return View();
        }
        #endregion

        #region Received Invoice Proforma
        public ActionResult ReceivedInvoicePerforma()
        {
            return View();
        }
        #endregion

        #region Received Invoice
        public ActionResult ReceivedInvoice()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ReceivedInvoiceList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetReceivedInvoiceList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //public FileResult ReceivedInvoicePdf(int id, string IsCustomer)
        //{


        //    string result_2 = "";
        //    using (var client = new WebClient())
        //    {
        //        var iscustomer = 0;
        //        string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
        //        if (IsCustomer.ToLower() == "true")
        //        {
        //            iscustomer = 1;
        //        }
        //        else
        //        {
        //            iscustomer = 0;
        //        }
        //        string result_1 = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + id + "/" + iscustomer).ToString();
        //        result_2 = (DomailApiUrl + "InvoicePdf/" + result_1.Substring(53)).Replace(@"\", "").ToString();

        //    }

        //    String res = result_2.TrimEnd('"');

        //    string filepath = Server.MapPath(res);
        //   // byte[] pdfByte = GetBytesFromFile(filepath);
        //    return File("", "application/pdf");

        //}

        #region Received Invoice PDF
        public String ReceivedInvoicePdf(int id, string IsCustomer)
        {
            try
            {
                string result_2 = "";
                using (var client = new WebClient())
                {
                    var iscustomer = 0;
                    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    if (IsCustomer.ToLower() == "true")
                    {
                        iscustomer = 1;
                    }
                    else
                    {
                        iscustomer = 0;
                    }
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                    //string result_1 = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + id + "/" + iscustomer).ToString();
                    string result_1 = "";
                    using (var webpage = new WebClient())
                    {
                        webpage.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                        result_1 = webpage.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + id + "/" + iscustomer).ToString();
                    }





                    var result = result_1.Substring(result_1.LastIndexOf(@"\") + 1);
                    result_2 = (DomailApiUrl + "InvoicePdf/" + result).Replace(@"\", "").ToString();

                }

                String res = result_2.TrimEnd('"');
                TempData["PdfResult"] = res;
                return res;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }


        }
        public String ReceivedInvoicePdfw(int id, string IsCustomer)
        {
            string result_2 = "";
            using (var client = new WebClient())
            {
                var iscustomer = 0;
                string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                if (IsCustomer.ToLower() == "true")
                {
                    iscustomer = 1;
                }
                else
                {
                    iscustomer = 0;
                }
                string result_1 = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + id + "/" + iscustomer).ToString();

                var result = result_1.Substring(result_1.LastIndexOf(@"\") + 1);
                result_2 = (DomailApiUrl + "InvoicePdf/" + result).Replace(@"\", "").ToString();

            }

            String res = result_2.TrimEnd('"');
            TempData["PdfResult"] = res;
            return res;
        }
        #endregion

        //public static byte[] GetBytesFromFile(string fullFilePath)
        //{
        //    // this method is limited to 2^32 byte files (4.2 GB)

        //    FileStream fs = null;
        //    try
        //    {
        //        //fs = File.OpenRead(fullFilePath);
        //        //byte[] bytes = new byte[fs.Length];
        //        //fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
        //        //return bytes;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //    }

        //}



        // [HttpGet]
        //public void ReceivedInvoicePdf(int id,string IsCustomer)
        //{


        //    string result_2 = "";
        //    using (var client = new WebClient())
        //    {
        //        var iscustomer = 0;
        //        string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
        //        if (IsCustomer.ToLower() == "true")
        //        {
        //            iscustomer = 1;
        //        }
        //        else
        //        {
        //            iscustomer = 0;
        //        }
        //        string result_1 = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + id + "/" + iscustomer).ToString();
        //        result_2 = (DomailApiUrl + "InvoicePdf/" + result_1.Substring(53)).Replace(@"\", "").ToString();

        //    }
        //   String res= result_2.TrimEnd('"');
        //   string FilePath =res;

        //   WebClient User = new WebClient();

        //   Byte[] FileBuffer = User.DownloadData(FilePath);

        //   if (FileBuffer != null)
        //   {

        //       Response.ContentType = "application/pdf";

        //       Response.AddHeader("content-length", FileBuffer.Length.ToString());

        //       Response.BinaryWrite(FileBuffer);

        //   }
        //   Response.Redirect(res);
        //    //return View();
        //}










        //string result_2 = "";
        //using (var client = new WebClient())
        //{
        //    var iscustomer = 0;
        //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
        //    if (data.IsCustomer == true)
        //    {
        //        iscustomer = 1;
        //    }
        //    string result_1 = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/generateInvoicepdf/" + data.Id + "/" + iscustomer).ToString();
        //    result_2 = (DomailApiUrl+"InvoicePdf//" + result_1.Substring(53)).Replace(@"\", "").ToString();

        //}
        //obj.PdfViewPath = result_2.TrimEnd('"');

        #region Received Performa
        public ActionResult ReceivedPerforma()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ReceivedPerformaList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetReceivedPerformaList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Received Invoice Performa
        public ActionResult InvoiceReceivedInvoicePerforma(string Command, ReportUserForAccountant obj)
        {

            if (Command == "RECEIVEDINVOICE")
            {
                return RedirectToAction("ReceivedInvoice", new { id = obj.UserId });
            }
            if (Command == "RECEIVEDPROFORMA")
            {
                return RedirectToAction("ReceivedPerforma", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });
        }

        public ActionResult InvoiceReceivedInvoicePerforma(string Command)
        {

            if (Command == "RECEIVEDINVOICE")
            {
                return RedirectToAction("ReceivedInvoice", new { id = 0 });
            }
            if (Command == "RECEIVEDPROFORMA")
            {
                return RedirectToAction("ReceivedPerforma", new { id = 0 });
            }

            return RedirectToAction("Report", new { error = "select user." });
        }
        #endregion

        #region Sent Invoice Performa
        public ActionResult SentInvoicePerforma()
        {
            return View();
        }
        #endregion

        #region Sent Invoice
        public ActionResult SentInvoice()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SentInvoiceList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetSentInvoiceList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sent Performa
        public ActionResult SentPerforma()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SentPerformaList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetSentPerformaList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Sent Invoice Received Invoice Performa
        public ActionResult SentInvoiceReceivedInvoicePerforma(string Command, ReportUserForAccountant obj)
        {

            if (Command == "SENTINVOICE")
            {
                return RedirectToAction("SentInvoice", new { id = obj.UserId });
            }
            if (Command == "SENTPROFORMA")
            {
                return RedirectToAction("SentPerforma", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });
        }
        #endregion

        #region Edit Received Invoice
        /*Update Received Invoice*/

        public ActionResult EditReceivedInvoice(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {

                    //using (var client = new WebClient())
                    //{

                    //    //ExpenseAssetList(0/2)
                    //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    //    string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/ExpenseAssetList/0/2");

                    //    dynamic data = JArray.Parse(result);
                    //    var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                    //    var mo = model.Where(s => s.CategoryId == "1" || s.CategoryId == "2");
                    //    ViewBag.ddl = mo;
                    //}


                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        //var Revenue = repository.ExpenseAssetList(UserId, num);
                        var Revenue = repository.ExpenseAssetList(0, 2);
                        if (Revenue.Count > 0)
                        {
                            Mapper.CreateMap<Classification, Kl_BlockListDto>();
                            var Revenuedata = Mapper.Map<List<Kl_BlockListDto>>(Revenue);
                            Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                            var mo = Revenuedata.Where(s => s.CategoryId == 1 || s.CategoryId == 2);
                            //  ViewBag.ddl = mo;
                            foreach (var itm in mo)
                            {
                                if (itm.CategoryId == 1)
                                {
                                    itm.CategoryValue = "Asset";
                                }
                                else if (itm.CategoryId == 2)
                                {
                                    itm.CategoryValue = "Expense";
                                }
                            }
                            ViewBag.ddl = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);

                        }
                        else
                            throw new Exception();
                    }




                    long SelectedActiveUser = 0;
                    int userid = 0;

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        if (selectedUser1 == null)
                        {
                            var selectedUser2 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                            userid = Convert.ToInt32(selectedUser2.Id);
                        }
                        else
                        {
                            using (var db = new KFentities())
                            {
                                userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.Email).Select(s => s.Id).FirstOrDefault());
                            }
                        }
                    }
                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_ReceivedInvoiceList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    // ViewBag.ddl = ClsDropDownList.newPopulateOwnership();

                    //if (objlist1.InvoiceNumber.Length == 1)
                    //{
                    //    objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                    //}
                    //else if (objlist1.InvoiceNumber.Length == 2)
                    //{
                    //    objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                    //}
                    //else if (objlist1.InvoiceNumber.Length == 2)
                    //{
                    //    objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                    //}
                    //else if (objlist1.InvoiceNumber.Length == 2)
                    //{
                    //    objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                    //}
                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }

                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditReceivedInvoice(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        obj.ButtonType = ButtonType;
                        obj.Type = Type;
                        obj.SectionType = SectionType;
                        obj.InvoiceId = obj.Id;


                        try
                        {
                            if (obj != null)
                            {
                                using (var repository = new InvoiceUserRegistrationRepository())
                                {
                                    int InvoiceId = repository.UpdateInvoice(obj);
                                    if (InvoiceId > 0)
                                    {
                                        //return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                        TempData["updatereceivedinvoicesuccesfuly"] = ButtonType;
                                        return RedirectToAction("ReceivedInvoice", "Invoice");
                                    }
                                    else
                                    {
                                        //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };

                                    }
                                }
                            }
                            else
                            {
                                // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                            }
                        }
                        catch
                        {
                            //return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
                        }




                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        TempData["updatereceivedinvoicesuccesfuly"] = ButtonType;
                        //        return RedirectToAction("ReceivedInvoice", "Invoice");


                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Edit Received Performa
        public ActionResult EditReceivedProforma(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    //using (var client = new WebClient())
                    //{


                    //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    //    string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/ExpenseAssetList/0/2");

                    //    dynamic data = JArray.Parse(result);
                    //    var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                    //    var mo = model.Where(s => s.CategoryId == "1" || s.CategoryId == "2");
                    //    ViewBag.ddl = mo;
                    //}


                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        //var Revenue = repository.ExpenseAssetList(UserId, num);
                        var Revenue = repository.ExpenseAssetList(0, 2);
                        if (Revenue.Count > 0)
                        {
                            Mapper.CreateMap<Classification, Kl_BlockListDto>();
                            var Revenuedata = Mapper.Map<List<Kl_BlockListDto>>(Revenue);
                            Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                            var mo = Revenuedata.Where(s => s.CategoryId == 1 || s.CategoryId == 2);
                            foreach (var itm in mo)
                            {
                                if (itm.CategoryId == 1)
                                {
                                    itm.CategoryValue = "Asset";
                                }
                                else if (itm.CategoryId == 2)
                                {
                                    itm.CategoryValue = "Expense";
                                }
                            }
                            ViewBag.ddl = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            // ViewBag.ddl = mo;
                        }
                        else
                            throw new Exception();
                    }



                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        if (selectedUser1 == null)
                        {
                            var selectedUser2 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                            userid = Convert.ToInt32(selectedUser2.Id);
                        }
                        else
                        {
                            using (var db = new KFentities())
                            {
                                userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.Email).Select(s => s.Id).FirstOrDefault());
                            }
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.kl_ReceivedProformaList(userid)
                                    select new ReceivedInvoiceListViewModel1
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditReceivedProforma(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        obj.ButtonType = ButtonType;
                        obj.Type = Type;
                        obj.SectionType = SectionType;
                        obj.InvoiceId = obj.Id;


                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    TempData["updatereceivedproformasuccesfuly"] = ButtonType;
                                    return RedirectToAction("ReceivedPerforma", "Invoice");
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }







                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        TempData["updatereceivedproformasuccesfuly"] = ButtonType;
                        //        return RedirectToAction("ReceivedPerforma", "Invoice");
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Edit Sent Invoice
        public ActionResult EditSentInvoice(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    using (var client = new WebClient())
                    {
                        using (var repository = new InvoiceUserRegistrationRepository())
                        {

                            var UserIds = context.tblInvoiceDetails.Where(s => s.Id == Id).Select(s => s.UserId).FirstOrDefault();

                            var num = context.InvoiceUserRegistrations.Where(s => s.Id == UserIds).Select(s => s.IsOnlyInvoice).FirstOrDefault().ToString();
                            var num2 = "";
                            if (num == "1")
                            {
                                num2 = "1";
                            }
                            else
                            {
                                num2 = "2";
                            }
                            //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                            //string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/RevenueList/" + UserIds + "/" + num2);

                            //dynamic data = JArray.Parse(result);
                            //var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                            //var mo = model.Where(s => s.CategoryId == "3");
                            //ViewBag.ddlSentInvoice = mo;



                            var Revenue = repository.RevenueList(Convert.ToInt32(UserIds), Convert.ToInt32(num2));
                            if (Revenue.Count > 0)
                            {
                                Mapper.CreateMap<Classification, RevenueListDto>();
                                var Revenuedata = Mapper.Map<List<RevenueListDto>>(Revenue);
                                Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                                Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                                // return Revenuedata;
                                var mo = Revenuedata.Where(s => s.CategoryId == 3);
                                //ViewBag.ddlSentInvoice = mo;
                                foreach (var itm in mo)
                                {
                                    itm.CategoryValue = "Revenue";
                                }
                                ViewBag.ddlSentInvoice = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            }
                            else
                                throw new Exception();
                        }
                    }
                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        if (selectedUser1 == null)
                        {
                            var selectedUser2 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                            userid = Convert.ToInt32(selectedUser2.Id);
                        }
                        else
                        {
                            using (var db = new KFentities())
                            {
                                userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.Email).Select(s => s.Id).FirstOrDefault());
                            }
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_SendInvoiceList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditSentInvoice(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        obj.ButtonType = ButtonType;
                        obj.Type = Type;
                        obj.SectionType = SectionType;
                        obj.InvoiceId = obj.Id;

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    TempData["updatesentinvoicesuccesfuly"] = ButtonType;
                                    return RedirectToAction("SentInvoice", "Invoice");
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        TempData["updatesentinvoicesuccesfuly"] = ButtonType;
                        //        return RedirectToAction("SentInvoice", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }


        [HttpPost]
        public ActionResult EditInvoiceSentInvoice(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        obj.ButtonType = ButtonType;
                        obj.Type = Type;
                        obj.SectionType = SectionType;
                        obj.InvoiceId = obj.Id;

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    TempData["updatesentinvoicesuccesfuly"] = ButtonType;
                                    return RedirectToAction("InvoiceSentInvoice", "Invoice");
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
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

            return View();
        }



        #endregion

        #region Edit Sent Performa
        public ActionResult EditSentProforma(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    using (var client = new WebClient())
                    {

                        var UserIds = context.tblInvoiceDetails.Where(s => s.Id == Id).Select(s => s.UserId).FirstOrDefault();

                        var num = context.InvoiceUserRegistrations.Where(s => s.Id == UserIds).Select(s => s.IsOnlyInvoice).FirstOrDefault().ToString();
                        var num2 = "";
                        if (num == "1")
                        {
                            num2 = "1";
                        }
                        else
                        {
                            num2 = "2";
                        }
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/RevenueList/" + UserIds + "/" + num2);

                        //dynamic data = JArray.Parse(result);
                        //var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                        //var mo = model.Where(s => s.CategoryId == "3");
                        //ViewBag.ddlSentProforma = mo;


                        using (var repository = new InvoiceUserRegistrationRepository())
                        {
                            var Revenue = repository.RevenueList(Convert.ToInt32(UserIds), Convert.ToInt32(num2));
                            if (Revenue.Count > 0)
                            {
                                Mapper.CreateMap<Classification, RevenueListDto>();
                                var Revenuedata = Mapper.Map<List<RevenueListDto>>(Revenue);
                                Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                                Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                                var mo = Revenue.Where(s => s.CategoryId == 3);


                                foreach (var itm in mo)
                                {
                                    itm.CategoryValue = "Revenue";
                                }
                                ViewBag.ddlSentProforma = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);

                                /// var CustomClassificationList = new List<GroupedSelectListItem>();
                                // CustomClassificationList = mo.Select(x => new GroupedSelectListItem() { Value = x.Id.ToString(), Text = x.ClassificationType = x.ClassificationType, GroupName = x.CategoryId.ToString(), GroupKey = x.CategoryId.ToString() }).ToList();


                                //ViewBag.ddlSentProforma = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            }
                            else
                                throw new Exception();
                        }
                    }

                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        if (selectedUser1 == null)
                        {
                            var selectedUser2 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                            userid = Convert.ToInt32(selectedUser2.Id);
                        }
                        else
                        {
                            using (var db = new KFentities())
                            {
                                userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.Email).Select(s => s.Id).FirstOrDefault());
                            }
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_SendProformaList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditSentProforma(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        obj.ButtonType = ButtonType;
                        obj.Type = Type;
                        obj.SectionType = SectionType;
                        obj.InvoiceId = obj.Id;

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    TempData["updatesentperformasuccesfuly"] = ButtonType;
                                    return RedirectToAction("SentPerforma", "Invoice");
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
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

            return View();
        }



        public ActionResult EditInvoiceSentProforma(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    using (var client = new WebClient())
                    {

                        var UserIds = context.tblInvoiceDetails.Where(s => s.Id == Id).Select(s => s.UserId).FirstOrDefault();

                        var num = context.InvoiceUserRegistrations.Where(s => s.Id == UserIds).Select(s => s.IsOnlyInvoice).FirstOrDefault().ToString();
                        var num2 = "";
                        if (num == "1")
                        {
                            num2 = "1";
                        }
                        else
                        {
                            num2 = "2";
                        }
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/RevenueList/" + UserIds + "/" + num2);

                        //dynamic data = JArray.Parse(result);
                        //var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                        //var mo = model.Where(s => s.CategoryId == "3");
                        //ViewBag.ddlSentProforma = mo;


                        using (var repository = new InvoiceUserRegistrationRepository())
                        {
                            var Revenue = repository.RevenueList(Convert.ToInt32(UserIds), Convert.ToInt32(num2));
                            if (Revenue.Count > 0)
                            {
                                Mapper.CreateMap<Classification, RevenueListDto>();
                                var Revenuedata = Mapper.Map<List<RevenueListDto>>(Revenue);
                                Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                                Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                                var mo = Revenue.Where(s => s.CategoryId == 3);
                                //ViewBag.ddlSentProforma = mo;
                                foreach (var itm in mo)
                                {
                                    itm.CategoryValue = "Revenue";
                                }
                                ViewBag.ddlSentProforma = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            }
                            else
                                throw new Exception();
                        }
                    }

                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        if (selectedUser1 == null)
                        {
                            var selectedUser2 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                            userid = Convert.ToInt32(selectedUser2.Id);
                        }
                        else
                        {
                            using (var db = new KFentities())
                            {
                                userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.Email).Select(s => s.Id).FirstOrDefault());
                            }
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_SendProformaList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        //[HttpPost]
        //public ActionResult EditInvoiceSentProforma(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        //{
        //    try
        //    {
        //        using (var context = new KFentities())
        //        {
        //            try
        //            {
        //                obj.ButtonType = ButtonType;
        //                obj.Type = Type;
        //                obj.SectionType = SectionType;
        //                obj.InvoiceId = obj.Id;

        //                if (obj != null)
        //                {
        //                    using (var repository = new InvoiceUserRegistrationRepository())
        //                    {
        //                        int InvoiceId = repository.UpdateInvoice(obj);
        //                        if (InvoiceId > 0)
        //                        {
        //                            // return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
        //                            TempData["updatesentperformasuccesfuly"] = ButtonType;
        //                            return RedirectToAction("InvoiceSentPerforma", "Invoice");
        //                        }
        //                        else
        //                        {
        //                            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
        //                }



        //            }
        //            catch
        //            {
        //                //MessageBox.Show("Unable to Connect to Server.");
        //            }

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ModelState.AddModelError("CommanError", "Unable connect to the server. Please try again later.");
        //    }

        //    return View();
        //}


        #endregion

        #region Invoice Customer List
        public ActionResult InvoiceCustomerList(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("InvoiceCustomerList");
        }
        [HttpGet]
        public JsonResult InvoiceCustomerListGrid(int Id, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Update Customer
        public ActionResult InvoiceUpdateCustomer(int id, bool? status)
        {
            using (var db = new KFentities())
            {

                var userData = db.tblCustomerOrSuppliers.Where(s => s.Id == id).FirstOrDefault();

                var obj = new UpdateCustomer();
                if (userData != null)
                {
                    obj.Id = userData.Id;
                    obj.CompanyName = userData.Company_Name;
                    obj.ContactPerson = userData.FirstName;
                    obj.BusinessNumber = userData.Telephone;
                    string CorporateAddress = userData.Address.Replace("|||", "|");
                    string[] CorporateArray = CorporateAddress.Split('|');
                    obj.CorporateAptNo = CorporateArray[0];
                    obj.CorporateHouseNo = CorporateArray[1];
                    obj.CorporateStreet = CorporateArray[2];
                    obj.CorporateCity = userData.City;
                    obj.CorporateState = userData.State;
                    obj.CorporatePostalCode = userData.PostalCode;

                    if (CorporateArray[0] == "" && CorporateArray[1] == "")
                    {
                        obj.CustomerAddress = CorporateArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;
                    }
                    else
                    {
                        obj.CustomerAddress = CorporateArray[0] + '-' + CorporateArray[1] + "\r\n" + CorporateArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;
                    }
                    
                   

                    if (userData.ShippingAddress != null)
                    {
                        string ShippingAddress = userData.ShippingAddress.Replace("|||", "|");
                        string[] ShippingArray = ShippingAddress.Split('|');
                        obj.ShippingAptNo = ShippingArray[0];
                        obj.ShippingHouseNo = ShippingArray[1];
                        obj.ShippingStreet = ShippingArray[2];
                        obj.ShippingCity = userData.ShippingCity;
                        obj.ShippingState = userData.ShippingState;
                        obj.ShippingPostalCode = userData.ShippingPostalCode;
                      //  obj.ShippingAddress = ShippingArray[0] + '-' + ShippingArray[1] + "\r\n" + ShippingArray[2] + '-' + userData.ShippingCity + "\r\n" + userData.ShippingState + "\r\n" + userData.ShippingPostalCode;
                        if (ShippingArray[0] == "" && ShippingArray[1] == "")
                        {
                            obj.ShippingAddress = ShippingArray[2] + '-' + userData.ShippingCity + "\r\n" + userData.ShippingState + "\r\n" + userData.ShippingPostalCode;
                        }
                        else
                        {
                            obj.ShippingAddress = ShippingArray[0] + '-' + ShippingArray[1] + "\r\n" + ShippingArray[2] + '-' + userData.ShippingCity + "\r\n" + userData.ShippingState + "\r\n" + userData.ShippingPostalCode;
                        }

                    }

                    var mob = userData.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                    obj.EmailTo = userData.Email;

                    obj.Website = userData.Website;
                    TempData["AddEmails"] = userData.AdditionalEmail;
                }

                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult InvoiceUpdateCustomer(UpdateCustomer objUser, string hdnUser_updateValue)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == objUser.Id).FirstOrDefault();
                    if (userChk != null)
                    {
                        userChk.FirstName = objUser.ContactPerson;
                        userChk.Company_Name = objUser.CompanyName;
                        userChk.Mobile = objUser.AreaCode + "-" + objUser.MobileNumber;
                        userChk.PostalCode = objUser.hdncorporatepostalcode;
                        userChk.Address = objUser.hdncorporateaptno + "|||" + objUser.hdncorporatehouseno + "|||" + objUser.hdncorporatestreet;
                        userChk.AdditionalEmail = objUser.hdnemailCC;
                        userChk.City = objUser.hdncorporatecity;
                        //userChk.ServiceOffered = objUser.ServiceOffered;
                        userChk.ShippingAddress = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.ShippingCity = objUser.hdnshippingcity;
                        userChk.ShippingPostalCode = objUser.hdnshippingpostalcode;
                        userChk.ShippingState = objUser.hdnshippingstate;
                        userChk.State = objUser.hdncorporatestate;
                        userChk.Website = objUser.Website;
                        userChk.Telephone = objUser.BusinessNumber;
                    }

                    context.SaveChanges();

                    TempData["customerupdatedetails"] = "Success";
                    return RedirectToAction("InvoiceUpdateCustomer", new { id = objUser.Id, status = true });
                }

            }
            catch (Exception ex)
            {

            }

            return View(objUser);
        }
        #endregion

        #region Invoice Supplier List
        public ActionResult InvoiceSupplierList(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("InvoiceSupplierList");
        }

        [HttpGet]
        public JsonResult InvoiceSupplierListGrid(int Id, int? page, int? limit, string sortBy, string direction, string searchString)
        {
            int total;
            var records = new GridModel().GetSupplierList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Search Invoice Supplier List Email
        [HttpPost]
        public ActionResult SearchInvoiceSupplierListEmail(CheckSupplier chkSupplier)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        AddCustomerSupplierDto obj = new AddCustomerSupplierDto();
                        obj.RoleId = 1;
                        obj.UserId = chkSupplier.UserId;
                        obj.Email = chkSupplier.EmailTo;

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
                                                // return new AddCustomerSupplierDto { ResponseMessage = "Added Successfully from Existing.", ResponseCode = (int)ApiStatusCode.Success };
                                            }
                                            else
                                            {
                                                var Financedata = repository.FinanceAlreadyExist(obj);
                                                if (Financedata != null)
                                                {
                                                    Financedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Financedata.ResponseMessage = "Success";
                                                    // return Financedata;
                                                }
                                                var Invoicedata = repository.InvoiceAlreadyExist(obj);
                                                if (Invoicedata != null)
                                                {
                                                    Invoicedata.ResponseCode = (int)ApiStatusCode.Success;
                                                    Invoicedata.ResponseMessage = "Success";
                                                    // return Invoicedata;
                                                }
                                                else
                                                {
                                                    // return new AddCustomerSupplierDto { ResponseMessage = "Not Register yet.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                                    TempData["customerdetails6"] = "Success";
                                                    return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 6 });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            // return new AddCustomerSupplierDto { ResponseMessage = "Already Registered.", ResponseCode = (int)ApiStatusCode.Unauthorised };
                                            TempData["customerdetails3"] = "Success";
                                            return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 3 });
                                        }

                                    }
                                    else
                                    {
                                        // return new AddCustomerSupplierDto { ResponseMessage = "You cannot add yourself as a supplier/customer.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                        TempData["customerdetails4"] = "Success";
                                        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 4 });
                                    }
                                }
                                else
                                {
                                    // return new AddCustomerSupplierDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter };
                                    TempData["customerdetails5"] = "Success";
                                    return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 5 });
                                }

                            }

                        }
                        catch (Exception)
                        {
                            // return new AddCustomerSupplierDto { ResponseMessage = "Unable to connect to server, Please try again later.", ResponseCode = (int)ApiStatusCode.Failure, UserId = 0 };
                            TempData["customerdetails2"] = "Success";
                            return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 2 });
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/CheckInvoiceExistOrNot";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    var desc = data.ResponseMessage;


                        //    if (desc == "Not Register yet.")
                        //    {
                        //        TempData["customerdetails6"] = "Success";
                        //        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 6 });
                        //    }
                        //    else if (desc == "Already Registered.")
                        //    {
                        //        TempData["customerdetails3"] = "Success";
                        //        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 3 });
                        //    }
                        //    else if (desc == "You cannot add yourself as a supplier/customer.")
                        //    {
                        //        TempData["customerdetails4"] = "Success";
                        //        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 4 });
                        //    }
                        //    else if (desc == "Please provide data.")
                        //    {
                        //        TempData["customerdetails5"] = "Success";
                        //        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 5 });
                        //    }
                        //    else if (desc == "Unable to connect to server, Please try again later.")
                        //    {
                        //        TempData["customerdetails2"] = "Success";
                        //        return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = 2 });
                        //    }
                        //    else
                        //    {
                        //        TempData["customerdetails"] = "Success";
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }

                        //}
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
            return RedirectToAction("InvoiceSupplierList", "Invoice", new { Selectuser = false });
            //return View();
        }
        #endregion

        #region Invoice Update Supplier
        public ActionResult InvoiceUpdateSupplier(int id, bool? status)
        {
            using (var db = new KFentities())
            {

                var userData = db.tblCustomerOrSuppliers.Where(s => s.Id == id).FirstOrDefault();

                var obj = new UpdateSupplier();
                if (userData != null)
                {
                    obj.Id = userData.Id;
                    obj.CompanyName = userData.Company_Name;
                    obj.ContactPerson = userData.FirstName;
                    obj.BusinessNumber = userData.Telephone;

                    string ShippingAddress = userData.Address.Replace("|||", "|");
                    string[] ShippingArray = ShippingAddress.Split('|');
                    obj.ShippingAptNo = ShippingArray[0];
                    obj.ShippingHouseNo = ShippingArray[1];
                    obj.ShippingStreet = ShippingArray[2];
                    obj.ShippingCity = userData.City;
                    obj.ShippingState = userData.State;
                    obj.ShippingPostalCode = userData.PostalCode;
                    obj.SupplierAddress = ShippingArray[0] + '-' + ShippingArray[1] + "\r\n" + ShippingArray[2] + '-' + userData.City + "\r\n" + userData.State + "\r\n" + userData.PostalCode;

                    var mob = userData.Mobile.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                    obj.EmailTo = userData.Email;

                    obj.Website = userData.Website;
                    obj.GoodsservicesList = ClsDropDownList.PopulateIndustry();


                    foreach (var itms in ClsDropDownList.PopulateIndustry())
                    {
                        if (itms.Text == userData.ServiceOffered)
                        {
                            obj.GoodsservicesId = Convert.ToInt32(itms.Value);
                        }
                    }

                }

                return View(obj);
            }
        }

        [HttpPost]
        public ActionResult InvoiceUpdateSupplier(UpdateSupplier objUser, string hdnUser_updateValue)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == objUser.Id).FirstOrDefault();
                    if (userChk != null)
                    {
                        userChk.FirstName = objUser.ContactPerson;
                        userChk.Company_Name = objUser.CompanyName;
                        userChk.Mobile = objUser.AreaCode + "-" + objUser.MobileNumber;
                        userChk.PostalCode = objUser.hdnshippingpostalcode;
                        userChk.Address = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.AdditionalEmail = objUser.EmailCc;
                        userChk.City = objUser.hdnshippingcity;
                        userChk.ServiceOffered = objUser.ServiceOffered;
                        userChk.ShippingAddress = objUser.hdnshippingaptno + "|||" + objUser.hdnshippinghouseno + "|||" + objUser.hdnshippingstreet;
                        userChk.ShippingCity = objUser.hdnshippingcity;
                        userChk.ShippingPostalCode = objUser.hdnshippingpostalcode;
                        userChk.ShippingState = objUser.hdnshippingstate;
                        userChk.State = objUser.hdnshippingstate;
                        userChk.Website = objUser.Website;
                        userChk.Telephone = objUser.BusinessNumber;


                        foreach (var itms in ClsDropDownList.PopulateIndustry())
                        {
                            if (Convert.ToInt32(itms.Value) == Convert.ToInt32(objUser.GoodsservicesId))
                            {
                                userChk.ServiceOffered = itms.Text;
                            }
                        }
                    }

                    context.SaveChanges();

                    TempData["supplierupdatedetails"] = "Success";
                    return RedirectToAction("InvoiceUpdateSupplier", new { id = objUser.Id, status = true });
                }

            }
            catch (Exception ex)
            {

            }

            return View(objUser);
        }
        #endregion

        #region Invoice Create Invoice
        public ActionResult InvoiceCreateInvoice(int Id)
        {

            var obj = new CreateInvoice();
            using (var context = new KFentities())
            {
                if (Id != 0)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            int UserIds = 0;
                            int RoleId = 0;
                            var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                UserIds = Convert.ToInt32(userChk.UserId);
                                RoleId = Convert.ToInt32(userChk.RoleId);
                            }

                            if (UserIds > 0)
                            {
                                using (var repo = new InvoiceUserRegistrationRepository())
                                {
                                    int invoiceNumber = repo.CreateInvoiceNumber(UserIds);
                                    if (invoiceNumber > 0)
                                    {
                                        //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                                        if (invoiceNumber.ToString().Length == 1)
                                        {
                                            obj.InvoiceNumber = "0000" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 2)
                                        {
                                            obj.InvoiceNumber = "000" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 3)
                                        {
                                            obj.InvoiceNumber = "00" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 4)
                                        {
                                            obj.InvoiceNumber = "0" + invoiceNumber;
                                        }
                                        else
                                        {
                                            obj.InvoiceNumber = invoiceNumber.ToString();
                                        }
                                        obj.hdnselectuser = Id;
                                        obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                                    }
                                    else
                                    {
                                        //  return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                    }
                                }
                            }

                            //var values = new NameValueCollection();
                            //values["UserId"] = UserIds.ToString();
                            //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                            //var response = client.UploadValues(DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoiceNumber/" + UserIds, values);

                            //var responseString = Encoding.Default.GetString(response);

                            //dynamic data = JObject.Parse(responseString);
                            //var res = data.ResponseCode;
                            //String UserId = data.UserId;
                            //if (res == 1)
                            //{

                            //    if (UserId.Length == 1)
                            //    {
                            //        obj.InvoiceNumber = "0000" + UserId;
                            //    }
                            //    else if (UserId.Length == 2)
                            //    {
                            //        obj.InvoiceNumber = "000" + UserId;
                            //    }
                            //    else if (UserId.Length == 3)
                            //    {
                            //        obj.InvoiceNumber = "00" + UserId;
                            //    }
                            //    else if (UserId.Length == 4)
                            //    {
                            //        obj.InvoiceNumber = "0" + UserId;
                            //    }
                            //    else
                            //    {
                            //        obj.InvoiceNumber = UserId;
                            //    }
                            //    obj.hdnselectuser = Id;

                            //}
                            //else
                            //{
                            //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                            //}
                            // obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {

                }
            }
            return View(obj);

        }

        [HttpPost]
        public ActionResult InvoiceCreateInvoice(CreateInvoice g)
        {
            var resolveRequest = HttpContext.Request;
            List<CreateInvoice> model = new List<CreateInvoice>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);
            Console.WriteLine(data1.UserId);
            int Ids = Convert.ToInt32(data1.UserId);

            string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
            string Url = DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoice";
            var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            var res = 0;
            using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(result);
                res = data.ResponseCode;

            }
            if (res == 1)
            {
                TempData["Invoicesuccessfullycreated"] = "Success";
                return RedirectToAction("InvoiceCreateInvoice", "Invoice", new { Id = Ids });
            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                return View();
            }

            //return View();
        }
        #endregion

        #region Invoice Customer List New
        public ActionResult InvoiceCustomerListNew(string EmailTo)
        {
            ViewBag.EmailTo = EmailTo;

            return View("InvoiceCustomerListProformaNew");
        }

        [HttpGet]
        public JsonResult InvoiceCustomerListGridNew(int Id, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Create Performa
        public ActionResult InvoiceCreateProforma(int Id)
        {

            var obj = new CreateInvoice();
            using (var context = new KFentities())
            {
                if (Id != 0)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            int UserIds = 0;
                            int RoleId = 0;
                            var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                UserIds = Convert.ToInt32(userChk.UserId);
                                RoleId = Convert.ToInt32(userChk.RoleId);
                            }

                            if (UserIds > 0)
                            {
                                using (var repo = new InvoiceUserRegistrationRepository())
                                {
                                    int invoiceNumber = repo.CreateInvoiceNumber(UserIds);
                                    if (invoiceNumber > 0)
                                    {
                                        //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                                        if (invoiceNumber.ToString().Length == 1)
                                        {
                                            obj.InvoiceNumber = "0000" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 2)
                                        {
                                            obj.InvoiceNumber = "000" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 3)
                                        {
                                            obj.InvoiceNumber = "00" + invoiceNumber;
                                        }
                                        else if (invoiceNumber.ToString().Length == 4)
                                        {
                                            obj.InvoiceNumber = "0" + invoiceNumber;
                                        }
                                        else
                                        {
                                            obj.InvoiceNumber = invoiceNumber.ToString();
                                        }
                                        obj.hdnselectuser = Id;
                                        obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                                    }
                                    else
                                    {
                                        //  return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                    }
                                }
                            }



                            //var values = new NameValueCollection();
                            //values["UserId"] = UserIds.ToString();
                            //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                            //var response = client.UploadValues(DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoiceNumber/" + UserIds, values);

                            //var responseString = Encoding.Default.GetString(response);

                            //dynamic data = JObject.Parse(responseString);
                            //var res = data.ResponseCode;
                            //String UserId = data.UserId;
                            //if (res == 1)
                            //{

                            //    if (UserId.Length == 1)
                            //    {
                            //        obj.InvoiceNumber = "0000" + UserId;
                            //    }
                            //    else if (UserId.Length == 2)
                            //    {
                            //        obj.InvoiceNumber = "000" + UserId;
                            //    }
                            //    else if (UserId.Length == 3)
                            //    {
                            //        obj.InvoiceNumber = "00" + UserId;
                            //    }
                            //    else if (UserId.Length == 4)
                            //    {
                            //        obj.InvoiceNumber = "0" + UserId;
                            //    }
                            //    else
                            //    {
                            //        obj.InvoiceNumber = UserId;
                            //    }
                            //    obj.hdnselectuser = Id;

                            //}
                            //else
                            //{
                            //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                            //}
                            //obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {

                }
            }



            return View(obj);

        }

        [HttpPost]
        public ActionResult InvoiceCreateProforma(CreateInvoice g)
        {
            var resolveRequest = HttpContext.Request;
            List<CreateInvoice> model = new List<CreateInvoice>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);
            Console.WriteLine(data1.UserId);
            int Ids = Convert.ToInt32(data1.UserId);

            //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
            string Url = DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoice";
            var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            var res = 0;
            using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(result);
                res = data.ResponseCode;

            }
            if (res == 1)
            {
                TempData["Proformasuccessfullycreated"] = "Success";
                return RedirectToAction("InvoiceCreateProforma", "Invoice", new { Id = Ids });
            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                return View();
            }

            //return View();
        }
        #endregion

        #region Invoice Customer List Proforma New
        public ActionResult InvoiceCustomerListProformaNew()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceCustomerListGridProformaNew(int Id, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Received Invoice
        public ActionResult InvoiceReceivedInvoice()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceReceivedInvoiceList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetReceivedInvoiceList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Received Invoice Performa 2
        public ActionResult InvoiceReceivedInvoicePerforma2(string Command, ReportUserForAccountant obj)
        {

            if (Command == "RECEIVEDINVOICE")
            {
                return RedirectToAction("InvoiceReceivedInvoice", new { id = obj.UserId });
            }
            if (Command == "RECEIVEDPROFORMA")
            {
                return RedirectToAction("InvoiceReceivedPerforma", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });
        }
        #endregion

        #region Invoice 1 Received Invoice Performa
        public ActionResult Invoice1ReceivedInvoicePerforma()
        {
            return View();
        }
        #endregion

        #region Edit Invoice Received Invoice
        public ActionResult EditInvoiceReceivedInvoice(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {

                    //using (var client = new WebClient())
                    //{


                    //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    //    string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/ExpenseAssetList/0/2");

                    //    dynamic data = JArray.Parse(result);
                    //    var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                    //    var mo = model.Where(s => s.CategoryId == "1" || s.CategoryId == "2");
                    //    ViewBag.ddl = mo;
                    //}

                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        var Revenue = repository.ExpenseAssetList(Convert.ToInt32(0), Convert.ToInt32(2));
                        if (Revenue.Count > 0)
                        {
                            Mapper.CreateMap<Classification, Kl_BlockListDto>();
                            var Revenuedata = Mapper.Map<List<Kl_BlockListDto>>(Revenue);
                            Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                            var mo = Revenuedata.Where(s => s.CategoryId == 1 || s.CategoryId == 2);
                            // ViewBag.ddl = mo;
                            foreach (var itm in mo)
                            {
                                if (itm.CategoryId == 1)
                                {
                                    itm.CategoryValue = "Asset";
                                }
                                else if (itm.CategoryId == 2)
                                {
                                    itm.CategoryValue = "Expense";
                                }
                            }
                            ViewBag.ddl = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                        }
                        else
                            throw new Exception();
                    }

                    long SelectedActiveUser = 0;
                    int userid = 0;

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                        using (var db = new KFentities())
                        {
                            userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.EmailTo).Select(s => s.Id).FirstOrDefault());
                        }

                    }
                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_ReceivedInvoiceList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }

                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditInvoiceReceivedInvoice(InvoiceDetailDto obj)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    return RedirectToAction("EditInvoiceReceivedInvoice", "Invoice", new { Selectuser = false });
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }
                    }
                    //    string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                    //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    //    string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                    //    var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                    //    using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                    //    {
                    //        var result = streamReader.ReadToEnd();
                    //        dynamic data = JObject.Parse(result);
                    //        var res = data.ResponseCode;
                    //        if (res == 1)
                    //        {
                    //            //TempData["customercreatedetails"] = "Success";
                    //            return RedirectToAction("EditInvoiceReceivedInvoice", "Invoice", new { Selectuser = false });
                    //        }
                    //        else
                    //        {
                    //            ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                    //        }
                    //    }

                    //}
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

            return View();
        }
        #endregion

        #region Invoice Received Performa
        public ActionResult InvoiceReceivedPerforma()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceReceivedPerformaList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetReceivedPerformaList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit Invoice Received Performa
        public ActionResult EditInvoiceReceivedProforma(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    //using (var client = new WebClient())
                    //{


                    //    string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                    //    string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/ExpenseAssetList/0/2");

                    //    dynamic data = JArray.Parse(result);
                    //    var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                    //    var mo = model.Where(s => s.CategoryId == "1" || s.CategoryId == "2");
                    //    ViewBag.ddl = mo;
                    //}

                    using (var repository = new InvoiceUserRegistrationRepository())
                    {
                        // var Revenue = repository.ExpenseAssetList(UserId, num);
                        var Revenue = repository.ExpenseAssetList(0, 2);
                        if (Revenue.Count > 0)
                        {
                            Mapper.CreateMap<Classification, Kl_BlockListDto>();
                            var Revenuedata = Mapper.Map<List<Kl_BlockListDto>>(Revenue);
                            Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                            Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                            var mo = Revenuedata.Where(s => s.CategoryId == 1 || s.CategoryId == 2);
                            foreach (var itm in mo)
                            {
                                if (itm.CategoryId == 1)
                                {
                                    itm.CategoryValue = "Asset";
                                }
                                else if (itm.CategoryId == 2)
                                {
                                    itm.CategoryValue = "Expense";
                                }
                            }
                            ViewBag.ddl = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            // ViewBag.ddl = mo;
                        }
                        else
                            throw new Exception();
                    }




                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                        using (var db = new KFentities())
                        {
                            userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.EmailTo).Select(s => s.Id).FirstOrDefault());
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.kl_ReceivedProformaList(userid)
                                    select new ReceivedInvoiceListViewModel1
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();

                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditInvoiceReceivedProforma(InvoiceDetailDto obj, String ButtonType, Int32 Type, String SectionType)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                obj.ButtonType = ButtonType;
                                obj.Type = Type;
                                obj.SectionType = SectionType;
                                obj.InvoiceId = obj.Id;

                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    return RedirectToAction("InvoiceReceivedPerforma", "Invoice", new { Selectuser = false });
                                }
                                else
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }



                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        //TempData["customercreatedetails"] = "Success";
                        //        return RedirectToAction("EditInvoiceReceivedProforma", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Invoice Sent Invoice Performa
        public ActionResult InvoiceSentInvoicePerforma()
        {
            return View();
        }
        #endregion

        #region Invoice sent Invoice
        public ActionResult InvoiceSentInvoice()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceSentInvoiceList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetSentInvoiceList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Sent Invoice Received Invoice Performa
        public ActionResult InvoiceSentInvoiceReceivedInvoicePerforma(string Command, ReportUserForAccountant obj)
        {

            if (Command == "SENTINVOICE")
            {
                return RedirectToAction("InvoiceSentInvoice", new { id = obj.UserId });
            }
            if (Command == "SENTPROFORMA")
            {
                return RedirectToAction("InvoiceSentPerforma", new { id = obj.UserId });
            }

            return RedirectToAction("Report", new { error = "select user." });
        }
        #endregion

        #region Edit Invoice Sent Invoice
        public ActionResult EditInvoiceSentInvoice(int Id)
        {
            try
            {
                using (var context = new KFentities())
                {
                    using (var client = new WebClient())
                    {

                        var UserIds = context.tblInvoiceDetails.Where(s => s.Id == Id).Select(s => s.UserId).FirstOrDefault();

                        var num = context.InvoiceUserRegistrations.Where(s => s.Id == UserIds).Select(s => s.IsOnlyInvoice).FirstOrDefault().ToString();
                        var num2 = "";
                        if (num == "1")
                        {
                            num2 = "1";
                        }
                        else
                        {
                            num2 = "2";
                        }

                        using (var repository = new InvoiceUserRegistrationRepository())
                        {
                            var Revenue = repository.RevenueList(Convert.ToInt32(UserIds), Convert.ToInt32(num2));
                            if (Revenue.Count > 0)
                            {
                                Mapper.CreateMap<Classification, RevenueListDto>();
                                var Revenuedata = Mapper.Map<List<RevenueListDto>>(Revenue);
                                Revenuedata.ForEach(x => x.ResponseCode = (int)ApiStatusCode.Success);
                                Revenuedata.ForEach(x => x.ResponseMessage = "Success");
                                var mo = Revenuedata.Where(s => s.CategoryId == 3);
                                // ViewBag.ddlSentInvoice = mo;
                                foreach (var itm in mo)
                                {
                                    itm.CategoryValue = "Revenue";
                                }
                                ViewBag.ddlSentInvoice = new SelectList(mo.ToList(), "Id", "Name", "CategoryValue", 0);
                            }
                            else
                                throw new Exception();
                        }




                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string result = client.DownloadString(DomailApiUrl + "InvoiceUserApi/Registration/RevenueList/" + UserIds + "/" + num2);

                        //dynamic data = JArray.Parse(result);
                        //var model = JsonConvert.DeserializeObject<List<RootObject>>(result);

                        //var mo = model.Where(s => s.CategoryId == "3");
                        //ViewBag.ddlSentInvoice = mo;
                    }
                    long SelectedActiveUser = 0;
                    int userid = 0;
                    int IsFinance = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser == null)
                    {
                        var userData = KF.Web.Models.UserData.GetCurrentUserData();
                        userid = userData.Id;
                    }
                    else
                    {
                        var selectedUser1 = KF.Web.Models.UserData.GetCurrentInvoiceUserData();
                        using (var db = new KFentities())
                        {
                            userid = Convert.ToInt32(db.InvoiceUserRegistrations.Where(s => s.EmailTo == selectedUser1.EmailTo).Select(s => s.Id).FirstOrDefault());
                        }
                    }

                    var TotalItemCount = context.tblItemDetails.Where(s => s.InvoiceId == Id).Count();

                    var objlist1 = (from Invoice in context.Kl_SendInvoiceList(userid)
                                    select new ReceivedInvoiceListViewModel
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
                                        Pro_Status = Invoice.Pro_Status == "Inprogress" ? "In Progress" : Invoice.Pro_Status,
                                        RoleId = Invoice.RoleId == null ? 2 : Invoice.RoleId,
                                        SalesPerson = Invoice.SalesPerson,
                                        ShippingCost = Invoice.ShippingCost,
                                        Terms = Invoice.Terms,
                                        Total = String.Format("{0:0.00}", Invoice.Total),
                                        Type = Invoice.Type,
                                        UserId = Invoice.UserId,
                                        Username = context.Kl_GetCompanyNamr(Invoice.UserId, Invoice.CustomerId).FirstOrDefault(),
                                        FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == Invoice.CustomerId).Select(s => s.Company_Name).FirstOrDefault(),
                                        ItemsCount = TotalItemCount
                                    }).Where(s => s.Id == Id).FirstOrDefault();
                    if (objlist1 != null)
                    {
                        if (objlist1.InvoiceNumber.Length == 1)
                        {
                            objlist1.InvoiceNumber = "0000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "000" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "00" + objlist1.InvoiceNumber;
                        }
                        else if (objlist1.InvoiceNumber.Length == 2)
                        {
                            objlist1.InvoiceNumber = "0" + objlist1.InvoiceNumber;
                        }
                    }
                    ViewBag.ButtonShow = objlist1.Pro_FlowStatus;
                    return View(objlist1);
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult EditInvoiceSentInvoices(InvoiceDetailDto obj)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {
                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    return RedirectToAction("EditInvoiceSentInvoice", "Invoice", new { Selectuser = false });
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }
                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        //TempData["customercreatedetails"] = "Success";
                        //        return RedirectToAction("EditInvoiceSentInvoice", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Invoice Sent Performa
        public ActionResult InvoiceSentPerforma()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InvoiceSentPerformaList(int? page, int? limit, string sortBy, string direction, int Id, string searchString = null)
        {
            int total;
            var records = new GridModel().GetSentPerformaList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit Invoice Sent Performa


        [HttpPost]
        public ActionResult EditInvoiceSentProforma(InvoiceDetailDto obj)
        {
            try
            {
                using (var context = new KFentities())
                {
                    try
                    {

                        if (obj != null)
                        {
                            using (var repository = new InvoiceUserRegistrationRepository())
                            {
                                int InvoiceId = repository.UpdateInvoice(obj);
                                if (InvoiceId > 0)
                                {
                                    //return new ApiResponseDto { ResponseMessage = "Invoice successfully Updated.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                                    return RedirectToAction("EditInvoiceSentProforma", "Invoice", new { Selectuser = false });
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                        }
                        else
                        {
                            //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                        }


                        //string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
                        //string Url = DomailApiUrl + "InvoiceUserApi/Registration/UpdateInvoice";
                        //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
                        //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
                        //{
                        //    var result = streamReader.ReadToEnd();
                        //    dynamic data = JObject.Parse(result);
                        //    var res = data.ResponseCode;
                        //    if (res == 1)
                        //    {
                        //        //TempData["customercreatedetails"] = "Success";
                        //        return RedirectToAction("EditInvoiceSentProforma", "Invoice", new { Selectuser = false });
                        //    }
                        //    else
                        //    {
                        //        ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                        //    }
                        //}

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

            return View();
        }
        #endregion

        #region Invoice Customer List Invoice New
        public ActionResult InvoiceCustomerListInvoiceNew(string EmailTo)
        {

            ViewBag.EmailTo = EmailTo;

            return View("InvoiceCustomerListInvoiceNew");
        }

        [HttpGet]
        public JsonResult InvoiceCustomerListGridInvoiceNew(int Id, int? page, int? limit, string sortBy, string direction, string searchString)
        {
            int total;
            var records = new GridModel().GetCustomerList(page, limit, sortBy, direction, searchString, out total, Id);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Stripe Details

        public ActionResult AddstripeDetail(String Email)
        {
            using (var db = new KFentities())
            {
                AddStripeDetailsDto obj = new AddStripeDetailsDto();
                try
                {

                    if (Email != "")
                    {
                        var updateDetails = db.StripeKeyDetails.Where(s => s.Email == Email).FirstOrDefault();
                        obj.PublishableKey = updateDetails.StripePublishKey;
                        obj.SecretKey = updateDetails.StripePrivateKey;
                        obj.Id = updateDetails.Id;
                    }
                }
                catch (Exception)
                {

                }

                return View(obj);
            }

        }

        [HttpPost]
        public ActionResult AddstripeDetail(AddStripeDetailsDto obj)
        {
            if (ModelState.IsValid)
            {
                using (var db = new KFentities())
                {
                    var currentUserData = UserData.GetCurrentInvoiceUserData();
                    if (obj.Id > 0)
                    {
                        //update
                        var updateDetails = db.StripeKeyDetails.Where(s => s.Id == obj.Id).FirstOrDefault();
                        updateDetails.StripePrivateKey = obj.SecretKey.Trim();
                        updateDetails.StripePublishKey = obj.PublishableKey.Trim();
                        updateDetails.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["mydata"] = "Successfully Updated";
                    }
                    else
                    {
                        //insert

                        var dbInsert = new StripeKeyDetail();
                        dbInsert.StripePrivateKey = obj.SecretKey.Trim();
                        dbInsert.StripePublishKey = obj.PublishableKey.Trim();
                        dbInsert.CreatedDate = DateTime.Now;
                        dbInsert.IsDeleted = false;
                        dbInsert.Email = obj.Email;
                        db.StripeKeyDetails.Add(dbInsert);
                        db.SaveChanges();
                        obj.Id = dbInsert.Id;
                        TempData["Status"] = "Success";
                        return RedirectToAction("AddstripeDetail", new { id = dbInsert.Id });
                    }


                }
            }
            return View(obj);
        }

        //public ActionResult AddpaymentDetail(int? id)
        //{
        //    AddPaymentDetailsDto obj = new AddPaymentDetailsDto();
        //    if (id > 0)
        //    {
        //        using (var repo = new InvoiceUserRegistrationRepository())
        //        {
        //            var getCarddata = repo.PaymentEdit(Convert.ToInt32(id));

        //            obj.Id = getCarddata.Id;
        //            obj.CardHolderName = getCarddata.CardHolderName;
        //            obj.CardNumber = getCarddata.CardNumber;
        //            obj.CVV = getCarddata.CVV;
        //            obj.ExpiryMonth = getCarddata.ExpiryMonth;
        //            obj.ExpiryYear = getCarddata.ExpiryYear;
        //        }
        //    }
        //    return View(obj);
        //}

        //[HttpPost]
        //public ActionResult AddpaymentDetail(AddPaymentDetailsDto obj, FormCollection form)
        //{
        //    int expiryMonth = Convert.ToInt32(Request.Form["txtExpiryMonth"].ToString());
        //    int expiryYear = Convert.ToInt32(Request.Form["txtExpiryYear"].ToString());

        //    if (expiryMonth < 0)
        //    {
        //        ModelState.AddModelError("ExpiryMonth", "Please Provide Valid Expiry Month");
        //    }
        //    if (expiryYear < 0)
        //    {
        //        ModelState.AddModelError("expiryYear", "Please Provide Valid Expiry Year");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        using (var db = new KFentities())
        //        {
        //            var currentUserData = UserData.GetCurrentUserData();
        //            if (obj.Id > 0)
        //            {
        //                using (var repository = new InvoiceUserRegistrationRepository())
        //                {
        //                    obj.ExpiryMonth = expiryMonth;
        //                    obj.ExpiryYear = expiryYear;
        //                    var UpdateDetails = repository.Updatecarddetail(obj);
        //                    TempData["mydata"] = "Successfully Updated";

        //                }
        //            }
        //            else
        //            {
        //                var dbInsert = new tblCardDetail();
        //                dbInsert.CardHolderName = obj.CardHolderName.Trim();
        //                dbInsert.CardNumber = obj.CardNumber;
        //                dbInsert.CVV = obj.CVV;
        //                dbInsert.DateCreated = DateTime.Now;
        //                dbInsert.ExpiryMonth = expiryMonth;
        //                dbInsert.ExpiryYear = expiryYear;
        //                dbInsert.IsDeleted = false;
        //                dbInsert.UserId = currentUserData.Id;
        //                db.tblCardDetails.Add(dbInsert);
        //                db.SaveChanges();
        //                obj.Id = dbInsert.Id;
        //                ViewBag.Status = "Success";
        //            }

        //        }
        //    }
        //    return View("AddpaymentDetail", obj);
        //}
        #endregion

        #region Stripe Details

        public ActionResult InvoicestripeDetail(String Email)
        {
            using (var db = new KFentities())
            {
                AddStripeDetailsDto obj = new AddStripeDetailsDto();
                try
                {

                    if (Email != "")
                    {
                        var updateDetails = db.StripeKeyDetails.Where(s => s.Email == Email).FirstOrDefault();
                        obj.PublishableKey = updateDetails.StripePublishKey;
                        obj.SecretKey = updateDetails.StripePrivateKey;
                        obj.Id = updateDetails.Id;
                    }
                }
                catch (Exception)
                {

                }

                return View(obj);
            }

        }

        [HttpPost]
        public ActionResult InvoicestripeDetail(AddStripeDetailsDto obj)
        {
            if (ModelState.IsValid)
            {
                using (var db = new KFentities())
                {
                    //var currentUserData = UserData.GetCurrentUserData();
                    var currentUserData = UserData.GetCurrentInvoiceUserData();
                    if (obj.Id > 0)
                    {
                        //update
                        var updateDetails = db.StripeKeyDetails.Where(s => s.Id == obj.Id).FirstOrDefault();
                        updateDetails.StripePrivateKey = obj.SecretKey.Trim();
                        updateDetails.StripePublishKey = obj.PublishableKey.Trim();
                        updateDetails.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                        TempData["mydata"] = "Successfully Updated";
                    }
                    else
                    {
                        //insert
                        var dbInsert = new StripeKeyDetail();
                        dbInsert.StripePrivateKey = obj.SecretKey.Trim();
                        dbInsert.StripePublishKey = obj.PublishableKey.Trim();
                        dbInsert.CreatedDate = DateTime.Now;
                        dbInsert.IsDeleted = false;
                        dbInsert.Email = currentUserData.EmailTo;
                        db.StripeKeyDetails.Add(dbInsert);
                        db.SaveChanges();
                        obj.Id = dbInsert.Id;
                        TempData["Status"] = "Success";
                        return RedirectToAction("InvoicestripeDetail", new { id = dbInsert.Id });
                    }


                }
            }
            return View(obj);
        }

        //public ActionResult AddpaymentDetail(int? id)
        //{
        //    AddPaymentDetailsDto obj = new AddPaymentDetailsDto();
        //    if (id > 0)
        //    {
        //        using (var repo = new InvoiceUserRegistrationRepository())
        //        {
        //            var getCarddata = repo.PaymentEdit(Convert.ToInt32(id));

        //            obj.Id = getCarddata.Id;
        //            obj.CardHolderName = getCarddata.CardHolderName;
        //            obj.CardNumber = getCarddata.CardNumber;
        //            obj.CVV = getCarddata.CVV;
        //            obj.ExpiryMonth = getCarddata.ExpiryMonth;
        //            obj.ExpiryYear = getCarddata.ExpiryYear;
        //        }
        //    }
        //    return View(obj);
        //}

        //[HttpPost]
        //public ActionResult AddpaymentDetail(AddPaymentDetailsDto obj, FormCollection form)
        //{
        //    int expiryMonth = Convert.ToInt32(Request.Form["txtExpiryMonth"].ToString());
        //    int expiryYear = Convert.ToInt32(Request.Form["txtExpiryYear"].ToString());

        //    if (expiryMonth < 0)
        //    {
        //        ModelState.AddModelError("ExpiryMonth", "Please Provide Valid Expiry Month");
        //    }
        //    if (expiryYear < 0)
        //    {
        //        ModelState.AddModelError("expiryYear", "Please Provide Valid Expiry Year");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        using (var db = new KFentities())
        //        {
        //            var currentUserData = UserData.GetCurrentUserData();
        //            if (obj.Id > 0)
        //            {
        //                using (var repository = new InvoiceUserRegistrationRepository())
        //                {
        //                    obj.ExpiryMonth = expiryMonth;
        //                    obj.ExpiryYear = expiryYear;
        //                    var UpdateDetails = repository.Updatecarddetail(obj);
        //                    TempData["mydata"] = "Successfully Updated";

        //                }
        //            }
        //            else
        //            {
        //                var dbInsert = new tblCardDetail();
        //                dbInsert.CardHolderName = obj.CardHolderName.Trim();
        //                dbInsert.CardNumber = obj.CardNumber;
        //                dbInsert.CVV = obj.CVV;
        //                dbInsert.DateCreated = DateTime.Now;
        //                dbInsert.ExpiryMonth = expiryMonth;
        //                dbInsert.ExpiryYear = expiryYear;
        //                dbInsert.IsDeleted = false;
        //                dbInsert.UserId = currentUserData.Id;
        //                db.tblCardDetails.Add(dbInsert);
        //                db.SaveChanges();
        //                obj.Id = dbInsert.Id;
        //                ViewBag.Status = "Success";
        //            }

        //        }
        //    }
        //    return View("AddpaymentDetail", obj);
        //}
        #endregion

        #region Test Create Invoice
        public ActionResult TestCreateInvoice(int Id)
        {

            var obj = new CreateInvoice();
            using (var context = new KFentities())
            {
                if (Id != 0)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            int UserIds = 0;
                            int RoleId = 0;
                            var userChk = context.tblCustomerOrSuppliers.Where(i => i.Id == Id).FirstOrDefault();
                            if (userChk != null)
                            {
                                UserIds = Convert.ToInt32(userChk.UserId);
                                RoleId = Convert.ToInt32(userChk.RoleId);
                            }

                            try
                            {
                                if (UserIds > 0)
                                {
                                    using (var repo = new InvoiceUserRegistrationRepository())
                                    {
                                        int invoiceNumber = repo.CreateInvoiceNumber(UserIds);
                                        if (invoiceNumber > 0)
                                        {
                                            //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = invoiceNumber };
                                            if (invoiceNumber.ToString().Length == 1)
                                            {
                                                obj.InvoiceNumber = "0000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 2)
                                            {
                                                obj.InvoiceNumber = "000" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 3)
                                            {
                                                obj.InvoiceNumber = "00" + invoiceNumber;
                                            }
                                            else if (invoiceNumber.ToString().Length == 4)
                                            {
                                                obj.InvoiceNumber = "0" + invoiceNumber;
                                            }
                                            else
                                            {
                                                obj.InvoiceNumber = invoiceNumber.ToString();
                                            }
                                            obj.hdnselectuser = Id;
                                            obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                                        }
                                        else
                                        {
                                            //  return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                        }
                                    }
                                }
                                else
                                {
                                    // return new ApiResponseDto { ResponseMessage = "Something went wrong.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    //var values = new NameValueCollection();
                    //values["UserId"] = UserIds.ToString();


                    //string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();


                    //var response = client.UploadValues(DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoiceNumber/" + UserIds, values);

                    //var responseString = Encoding.Default.GetString(response);

                    //dynamic data = JObject.Parse(responseString);
                    //var res = data.ResponseCode;
                    //String UserId = data.UserId;
                    //if (res == 1)
                    //{

                    //    if (UserId.Length == 1)
                    //    {
                    //        obj.InvoiceNumber = "0000" + UserId;
                    //    }
                    //    else if (UserId.Length == 2)
                    //    {
                    //        obj.InvoiceNumber = "000" + UserId;
                    //    }
                    //    else if (UserId.Length == 3)
                    //    {
                    //        obj.InvoiceNumber = "00" + UserId;
                    //    }
                    //    else if (UserId.Length == 4)
                    //    {
                    //        obj.InvoiceNumber = "0" + UserId;
                    //    }
                    //    else
                    //    {
                    //        obj.InvoiceNumber = UserId;
                    //    }
                    //    obj.hdnselectuser = Id;

                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                    // }
                    //obj.ServiceType = ClsDropDownList.PopulateServiceProductType(UserIds, RoleId);
                    //}
                    //}

                }
                else
                {
                    return View();
                }
            }



            return View(obj);

        }

        [HttpPost]
        public ActionResult TestCreateInvoice(InvoiceDetailDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<InvoiceDetailDto> model = new List<InvoiceDetailDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);
            Console.WriteLine(data1.UserId);
            int Ids = Convert.ToInt32(data1.UserId);

            //InvoiceDetailDto obj = new InvoiceDetailDto();
            //obj.DueDate = data1.DueDate;
            //obj.UserId = data1.UserId;
            //obj.DepositePayment = data1.DepositePayment;
            //obj.InvoiceDate = data1.InvoiceDate;
            //obj.PaymentTerms = data1.PaymentTerms;
            //obj.SalesPerson = data1.SalesPerson;
            //obj.Item = data1.Item;
            //obj.QST_Tax = data1.QST_Tax;
            //obj.Total = data1.Total;
            //obj.SubTotal = data1.SubTotal;
            //obj.HST_Tax = data1.HST_Tax;
            //obj.CustomerId = data1.CustomerId;
            //obj.Rate = data1.Rate;
            //obj.Discount = data1.Discount;
            //obj.Customer_ServiceTypeId = data1.Customer_ServiceTypeId;
            //obj.ShippingCost = data1.ShippingCost;
            //obj.ServiceTypeId = data1.ServiceTypeId;
            //obj.ButtonType = data1.ButtonType;
            //obj.Type = data1.Type;
            //obj.Tax = data1.Tax;
            //obj.GST_Tax = data1.GST_Tax;
            //obj.Note = data1.Note;
            //obj.ItemId = data1.ItemId;
            //obj.Terms = data1.Terms;
            //obj.Quantity = data1.Quantity;
            //obj.Description = data1.Description;
            //obj.InvoiceNumber = data1.InvoiceNumber;
            //obj.Customer_Service = data1.Customer_Service;
            //obj.PST_Tax = data1.PST_Tax;
            //obj.DocumentRef = data1.DocumentRef;
            //obj.BalanceDue = data1.BalanceDue;

            //try
            //{
            //    using (var repository = new InvoiceUserRegistrationRepository())
            //    {
            //        if (requestBody != null)
            //        {
            //            var userid = repository.CreateInvoice(obj);
            //            if (userid > 0)
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Invoice successfully created.", ResponseCode = (int)ApiStatusCode.Success, UserId = userid };
            //                TempData["Invoicesuccessfullycreated"] = "Success";
            //                return RedirectToAction("CreateInvoice", "Invoice");
            //            }
            //            else
            //            {
            //                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };

            //            }
            //        }
            //        else
            //        {
            //            // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    // return new ApiResponseDto { ResponseMessage = "Unable connect to the server. Please try again later.", ResponseCode = (int)ApiStatusCode.Failure };
            //}

            //TempData["Invoicesuccessfullycreated"] = "Success";
            //return RedirectToAction("CreateInvoice", "Invoice", new { Id = 0 });

            //string Url = "http://kippinfinance.web1.anzleads.com/MobileApi/InvoiceUserApi/Registration/CreateInvoice";

            string DomailApiUrl = System.Configuration.ConfigurationManager.AppSettings["WebsiteBaseUrl"].ToString();
            string Url = DomailApiUrl + "InvoiceUserApi/Registration/CreateInvoice";

            var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            var res = 0;
            using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dynamic data = JObject.Parse(result);
                res = data.ResponseCode;

            }
            if (res == 1)
            {
                TempData["Invoicesuccessfullycreated"] = "Success";
                //return RedirectToAction("CreateInvoice", "Invoice", new { Id = Ids });
                return RedirectToAction("CreateInvoice", "Invoice");
            }
            else
            {
                ModelState.AddModelError("ErrorMsg", "Invalid credential.");
                return View();
            }

            TempData["Invoicesuccessfullycreated"] = "Success";
            return RedirectToAction("CreateInvoice", "Invoice", new { Id = 0 });
            //return View();
        }
        #endregion

        #region Invoice Report
        [HttpPost]
        public ActionResult InvoiceReport(ReceivedInvoiceListViewModel ObjView)
        {
            ObjView.StatementTypeList = ClsDropDownList.PopulateTypeOfInvoices();
            ObjView.StatementTypeList2 = ClsDropDownList.PopulateFlowStatus();
            return View(ObjView);
        }

        public ActionResult InvoiceReport()
        {
            ReceivedInvoiceListViewModel ObjView = new ReceivedInvoiceListViewModel();
            ObjView.StatementTypeList = ClsDropDownList.PopulateTypeOfInvoices();
            ObjView.StatementTypeList2 = ClsDropDownList.PopulateFlowStatus();
            return View(ObjView);
        }

        public JsonResult InvoiceReportList(int? page, int? limit, string sortBy, string direction, int Id, string typeofinvoice, string flow, string searchString = null)
        {
            int total;
            var records = new GridModel().GetInvoiceReportList(page, limit, sortBy, direction, searchString, out total, Id, typeofinvoice, flow);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Manual Report
        [HttpPost]
        public ActionResult InvoiceManualReport(ManualPaymentDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<ManualPaymentDto> model = new List<ManualPaymentDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);

            ManualPaymentDto obj = new ManualPaymentDto();


            //obj.IsSupplierManualPaid = data1.
            obj.SupplierManualPaidAmount = Convert.ToDecimal(data1.SupplierManualPaidAmount);
            obj.SupplierManualPaidJVID = data1.SupplierManualPaidJVID;
            //obj.IsCustomerManualPaid = data1.
            obj.CustomerManualPaidAmount = Convert.ToDecimal(data1.CustomerManualPaidAmount);
            obj.CustomerManualPaidJVID = data1.CustomerManualPaidJVID;
            obj.InvoiceId = Convert.ToInt32(data1.InvoiceId);
            obj.IsCustomer = data1.IsCustomer;


            if (obj != null)
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    int InvoiceId = repository.ManualPayment(obj);
                    if (InvoiceId > 0)
                    {
                        // return new ApiResponseDto { ResponseMessage = "Manual Payment is Completed.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        TempData["Invoicereportmanualpayment"] = "Success";
                        //    //return RedirectToAction("CreateInvoice", "Kippin", new { Id = Ids });
                        return RedirectToAction("InvoiceReport", "Invoice");
                    }
                    else
                    {
                        //return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                    }
                }
            }
            else
            {
                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            }



            ////Console.WriteLine(data1.UserId);
            ////int Ids = Convert.ToInt32(data1.UserId);

            //string Url = "http://kippinfinance.web1.anzleads.com/MobileApi/InvoiceUserApi/Registration/ManualPayment";
            //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            //var res = 0;
            //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    dynamic data = JObject.Parse(result);
            //    res = data.ResponseCode;

            //}
            //if (res == 1)
            //{
            //    TempData["Invoicesuccessfullycreated"] = "Success";
            //    //return RedirectToAction("CreateInvoice", "Kippin", new { Id = Ids });
            //    return RedirectToAction("CreateInvoice", "Kippin");
            //}
            //else
            //{
            //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
            //    return View();
            //}

            //TempData["Invoicesuccessfullycreated"] = "Success";
            //return RedirectToAction("CreateInvoice", "Kippin", new { Id = 0 });
            return View();
        }
        #endregion

        #region Invoice Stripe Report
        public ActionResult InvoiceStripReport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InvoiceStripReport(StripePaymentDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<StripePaymentDto> model = new List<StripePaymentDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);

            StripePaymentDto obj = new StripePaymentDto();

            obj.InvoiceId = Convert.ToInt32(data1.InvoiceId);

            obj.PaidAmount = Convert.ToDecimal(data1.PaidAmount);

            obj.PaidJVID = data1.CustomerManualPaidJVID;

            obj.CardNumber = data1.CreditCardNumber;

            obj.ExpiryMonth = Convert.ToInt32(data1.Month);

            obj.Cvcs = Convert.ToInt32(data1.Cvv);

            obj.ExpiryYear = Convert.ToInt32(data1.Year);

            obj.SupplierID = Convert.ToInt32(data1.SupplierId);





            //Console.WriteLine(data1.UserId);
            //int Ids = Convert.ToInt32(data1.UserId);


            if (obj != null)
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    int InvoiceId = repository.StripePayment(obj);
                    if (InvoiceId > 0)
                    {
                        //return new ApiResponseDto { ResponseMessage = "Successfully Paid.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        TempData["Invoicereportstripepayment"] = "Success";
                        return RedirectToAction("InvoiceReport", "Invoice");
                    }
                    else
                    {
                        //return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                    }
                }
            }
            else
            {
                // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            }

            //string Url = "http://kippinfinance.web1.anzleads.com/MobileApi/InvoiceUserApi/Registration/StripePayment";
            //var apiResponse = ApiCall.GetResponseFromApi(Url, "POST", requestBody);
            //var res = 0;
            //using (var streamReader = new StreamReader(apiResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    dynamic data = JObject.Parse(result);
            //    res = data.ResponseCode;

            //}
            //if (res == 1)
            //{
            //    TempData["Invoicesuccessfullycreated"] = "Success";
            //    //return RedirectToAction("CreateInvoice", "Kippin", new { Id = Ids });
            //    return RedirectToAction("CreateInvoice", "Kippin");
            //}
            //else
            //{
            //    ModelState.AddModelError("ErrorMsg", "Invalid credential.");
            //    return View();
            //}

            //TempData["Invoicesuccessfullycreated"] = "Success";
            // return RedirectToAction("CreateInvoice", "Invoice", new { Id = 0 });
            return View();
        }
        #endregion

        #region Invoice Report Invoice
        [HttpPost]
        public ActionResult InvoiceReportInvoice(ReceivedInvoiceListViewModel ObjView)
        {
            ObjView.StatementTypeList = ClsDropDownList.PopulateTypeOfInvoices();
            ObjView.StatementTypeList2 = ClsDropDownList.PopulateFlowStatus();
            return View(ObjView);
        }

        public ActionResult InvoiceReportInvoice()
        {
            ReceivedInvoiceListViewModel ObjView = new ReceivedInvoiceListViewModel();
            ObjView.StatementTypeList = ClsDropDownList.PopulateTypeOfInvoices();
            ObjView.StatementTypeList2 = ClsDropDownList.PopulateFlowStatus();
            return View(ObjView);
        }

        public JsonResult InvoiceReportListInvoice(int? page, int? limit, string sortBy, string direction, int Id, string typeofinvoice, string flow, string searchString = null)
        {
            int total;
            var records = new GridModel().GetInvoiceReportList(page, limit, sortBy, direction, searchString, out total, Id, typeofinvoice, flow);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Invoice Manual Report Invoice
        [HttpPost]
        public ActionResult InvoiceManualReportInvoice(ManualPaymentDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<ManualPaymentDto> model = new List<ManualPaymentDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);

            ManualPaymentDto obj = new ManualPaymentDto();


            //obj.IsSupplierManualPaid = data1.
            obj.SupplierManualPaidAmount = Convert.ToDecimal(data1.SupplierManualPaidAmount);
            obj.SupplierManualPaidJVID = data1.SupplierManualPaidJVID;
            //obj.IsCustomerManualPaid = data1.
            obj.CustomerManualPaidAmount = Convert.ToDecimal(data1.CustomerManualPaidAmount);
            obj.CustomerManualPaidJVID = data1.CustomerManualPaidJVID;
            obj.InvoiceId = Convert.ToInt32(data1.InvoiceId);
            obj.IsCustomer = data1.IsCustomer;


            if (obj != null)
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    int InvoiceId = repository.ManualPayment(obj);
                    if (InvoiceId > 0)
                    {
                        // return new ApiResponseDto { ResponseMessage = "Manual Payment is Completed.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        TempData["Invoicereportmanualpayment"] = "Success";
                        //    //return RedirectToAction("CreateInvoice", "Kippin", new { Id = Ids });
                        return RedirectToAction("InvoiceReport", "Invoice");
                    }
                    else
                    {
                        return RedirectToAction("InvoiceReport", "Invoice");
                        //return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                    }
                }
            }
            else
            {
                //return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            }

            return View();
        }
        #endregion

        #region Invoice Stripe Report Invoice
        //public ActionResult InvoiceStripReportInvoice()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult InvoiceStripReportInvoice(StripePaymentDto g)
        {
            var resolveRequest = HttpContext.Request;
            List<StripePaymentDto> model = new List<StripePaymentDto>();
            resolveRequest.InputStream.Seek(0, SeekOrigin.Begin);
            string requestBody = new StreamReader(resolveRequest.InputStream).ReadToEnd();

            dynamic data1 = JObject.Parse(requestBody);

            StripePaymentDto obj = new StripePaymentDto();

            obj.InvoiceId = Convert.ToInt32(data1.InvoiceId);

            obj.PaidAmount = Convert.ToDecimal(data1.PaidAmount);

            obj.PaidJVID = data1.CustomerManualPaidJVID;

            obj.CardNumber = data1.CreditCardNumber;

            obj.ExpiryMonth = Convert.ToInt32(data1.Month);

            obj.Cvcs = Convert.ToInt32(data1.Cvv);

            obj.ExpiryYear = Convert.ToInt32(data1.Year);

            obj.SupplierID = Convert.ToInt32(data1.SupplierId);

            if (obj != null)
            {
                using (var repository = new InvoiceUserRegistrationRepository())
                {
                    obj.CardNumber = encrypt.AesEncrypt(obj.CardNumber);
                    int InvoiceId = repository.StripePayment(obj);
                    if (InvoiceId > 0)
                    {
                        //return new ApiResponseDto { ResponseMessage = "Successfully Paid.", ResponseCode = (int)ApiStatusCode.Success, UserId = InvoiceId };
                        TempData["Invoicereportstripepayment"] = "Success";
                        return RedirectToAction("InvoiceReport", "Invoice");
                    }
                    else
                    {
                        return RedirectToAction("InvoiceReport", "Invoice");
                        //return new ApiResponseDto { ResponseMessage = "Please provide relevant data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = InvoiceId };
                    }
                }
            }
            else
            {
                // return new ApiResponseDto { ResponseMessage = "Please provide data.", ResponseCode = (int)ApiStatusCode.NullParameter, UserId = 0 };
            }

            return View();
        }
        #endregion

    }
}