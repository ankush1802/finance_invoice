using AutoMapper;
using KF.Dto.Modules.Common;
using KF.Dto.Modules.Finance;
using KF.Dto.Modules.FinanceReport;
using KF.Entity;
using KF.Repo.Modules.Finance;
using KF.Utilities.Common;
using KF.Web.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using YodleeBase;
using YodleeBase.model;

namespace KF.Web.Controllers
{
    public class AccountingController : Controller
    {
        // GET: Accounting
        public ActionResult Index()
        {
            return View();
        }

        #region Active Users

        [KippinAuthorize(Roles = "Accountant,Sub accountant")]
        public ActionResult SendEmail(int UserId)
        {
            //here UserId is userid which we have to send mail
            using (var userRepository = new AccountRepository())
            {
                try
                {
                    var userData = UserData.GetUnpaidUserData(UserId);
                    var data = UserData.GetCurrentUserData();
                    //Read Email Message body from html file
                    string html = System.IO.File.ReadAllText(Server.MapPath("~/EmailFormats/PrivateKeyEmailTemplate.html"));
                    //Replace Username
                    html = html.Replace("#Username", userData.FirstName + " " + userData.LastName);

                    var bodymessage = "Your KIPPIN account details: <br/>";
                    bodymessage += "Client name: " + userData.FirstName + " " + userData.LastName + "<br/>";
                    bodymessage += "Email: " + userData.Email + "<br/>";
                    bodymessage += "Private Key: " + userData.PrivateKey + "<br/>";
                    // bodymessage += "Your Accountant name is: " + user.FirstName+" " + user.LastName + "." + "<br/>";
                    bodymessage += "Accountant's Company name is: " + data.CompanyName + "." + "<br/>";
                    bodymessage += "Please download the KIPPIN app from IOS / Android store and login to activate your account";
                    html = html.Replace("#MESSAGE", bodymessage);
                    SendMailModelDto _objModelMail = new SendMailModelDto();
                    _objModelMail.To = userData.Email;
                    _objModelMail.Subject = "Kippin-Finance Account Details";
                    _objModelMail.MessageBody = html;
                    //Sending mail
                    var mailSent = Sendmail.SendEmail(_objModelMail);
                    //update database
                    var sendEmailStatusUpdate = userRepository.SentEmailStatus(userData.Email);
                    TempData["SendEmailStatus"] = "Success";
                }
                catch (Exception)
                {
                    TempData["SendEmailStatus"] = "Failure";
                }

            }
            return RedirectToAction("ActiveUser");
        }

        /// <summary>
        /// Select Active user dashboard for accounatant & Employee
        /// </summary>
        /// <param name="Selectuser"></param>
        /// <returns></returns>
        [KippinAuthorize(Roles = "Accountant,Sub accountant")]
        public ActionResult ActiveUser(bool? Selectuser)
        {
            if (Selectuser == false)
            {
                Response.Cookies.Clear();
                var c = new HttpCookie("SelectedActiveUser");
                c.Value = null;
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
                ViewBag.ten = 1;
            }
            return View();
        }

        public JsonResult ActiveUserList(string firstName, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetActiveUserList(page, limit, sortBy, direction, searchString, out total, firstName);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Serach User by name on active user dashboard
        /// </summary>
        /// <param name="FirstName"></param>
        /// <returns></returns>
        public ActionResult SearchUser(string FirstName)
        {
            ViewBag.Firstname = FirstName;
            return View("ActiveUser");
        }


        /// <summary>
        /// Select user and make dynamic menu based on user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [KippinAuthorize(Roles = "Accountant,Sub accountant")]
        public ActionResult SelectUser(int UserId)
        {
            try
            {
                Response.Cookies.Clear();
                HttpCookie cookie = new HttpCookie("SelectedActiveUser");
                cookie.Value = Convert.ToString(UserData.GetUserData(UserId).Id);
                cookie.Expires = DateTime.Now.AddHours(23);
                if (UserData.GetUserData(UserId).IsPaid == true)
                {
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Report", "Accounting", new { Selectuser = true });
                    //Add username so that we can show him the selected user
                }
                else
                {
                    Response.Cookies.Clear();
                    var c = new HttpCookie("SelectedActiveUser");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }

            }
            catch (Exception)
            { }

            return RedirectToAction("ActiveUser");
        }

        /// <summary>
        /// Download Create User Template
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadTemplate()
        {
            return File("~/CustomResources/ActiveUser/UsersTemplate.xlsx", "application/vnd.ms-excel", "UsersTemplate.xlsx");
        }

        #region User Creation By Accountant
        [KippinAuthorize(Roles = "Accountant")]
        public ActionResult CreateUser()
        {
            UserWithAnAccountantViewModel user = new UserWithAnAccountantViewModel();
            user.MonthList = ClsDropDownList.PopulateMonth();
            user.YearList = ClsDropDownList.PopulateYear();
            user.IndustryList = ClsDropDownList.PopulateIndustry();
            // user.CustomIndustryList = ClsDropDownList.PopulateCustomIndustryList();
            user.ProvinceList = ClsDropDownList.PopulateProvince();
            user.OwnershipList = ClsDropDownList.PopulateOwnership();
            return View(user);

        }

        [HttpPost]
        public ActionResult CreateUser(UserWithAnAccountantViewModel user)
        {
            if (user.SubIndustryId > 0)
            {
                if (ModelState.IsValid)
                {
                    using (var userRepository = new AccountRepository())
                    {
                        try
                        {
                            var userExistedChk = userRepository.UserEmailExistCheck(user.Email);
                            var _user = UserData.GetCurrentUserData();
                            if (!userExistedChk)
                            {
                                user.RoleId = 3; // User with an accountant
                                user.SectorId = 1; // 1 For Finance
                                user.CurrencyId = 1;
                                user.AccountantId = _user.Id; // Accountant id who create user.
                                user.PrivateKey = RandomString.GetUniqueKey();
                                user.TaxEndMonthId = user.TaxStartMonthId;
                                user.CountryId = 1;//by default there is one country so we are set canada id i.e. 1
                                user.TaxEndYear = Convert.ToString(Convert.ToInt32(UserData.GetYear(Convert.ToInt32(user.TaxStartYear))) + 1);
                                user.TaxStartYear = UserData.GetYear(Convert.ToInt32(user.TaxStartYear));
                                user.TaxationEndDay = Convert.ToInt32(user.TaxStartDay);
                                user.TaxationStartDay = Convert.ToInt32(user.TaxStartDay);
                                user.CreatedDate = DateTime.Now;
                                user.MobileNumber = user.AreaCode + "-" + user.MobileNumber;
                                Mapper.CreateMap<UserWithAnAccountantViewModel, UserRegistrationDto>();
                                var userDetails = Mapper.Map<UserRegistrationDto>(user);
                                userRepository.AddUser(userDetails);
                                return RedirectToAction("ActiveUser");
                            }
                            else
                            {
                                var UserDetails = UserData.GetUserDataByEmail(user.Email);
                                if (UserDetails.IsDeleted == true && UserDetails.IsUnlink == true)
                                {
                                    var UserWithAnAccountantData = new UserRegistrationDto();
                                    UserWithAnAccountantData.Id = UserDetails.Id;
                                    UserWithAnAccountantData.AccountantId = _user.Id; // Accountant id who create user.
                                    UserWithAnAccountantData.PrivateKey = RandomString.GetUniqueKey();
                                    UserWithAnAccountantData.TaxEndMonthId = Convert.ToByte(user.TaxStartMonthId);
                                    UserWithAnAccountantData.TaxEndYear = Convert.ToInt32(UserData.GetYear(Convert.ToInt32(user.TaxStartYear))) + 1;
                                    UserWithAnAccountantData.TaxStartYear = Convert.ToInt32(UserData.GetYear(Convert.ToInt32(user.TaxStartYear)));
                                    UserWithAnAccountantData.TaxationEndDay = Convert.ToInt32(user.TaxStartDay);
                                    UserWithAnAccountantData.TaxationStartDay = Convert.ToInt32(user.TaxStartDay);
                                    UserWithAnAccountantData.SubIndustryId = user.SubIndustryId;
                                    UserWithAnAccountantData.MobileNumber = user.AreaCode + "-" + user.MobileNumber;

                                    var updateDetails = userRepository.UpdateUserWithAnAccountantDetails(UserWithAnAccountantData);

                                    return RedirectToAction("ActiveUser");
                                }
                                ModelState.AddModelError("Email", "Email already exist try another.");
                            }

                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    //chk user existed or not

                }
            }
            else
            {
                user.SubIndustryList = ClsDropDownList.GetSubIndustryListById(user.IndustryId);
                user.SubIndustryError = true;
                ModelState.AddModelError("SubIndustryId", "Please select sub industry.");
            }

            user.MonthList = ClsDropDownList.PopulateMonth();
            user.YearList = ClsDropDownList.PopulateYear();
            user.ProvinceList = ClsDropDownList.PopulateProvince();
            user.OwnershipList = ClsDropDownList.PopulateOwnership();
            user.IndustryList = ClsDropDownList.PopulateIndustry();
            return View(user);

        }

        #region Upload Excel sheet
        [KippinAuthorize]
        [HttpPost]
        public ActionResult ActiveUser(FormCollection _file)
        {
            HttpPostedFileBase file = Request.Files["UploadedFile"];
            var result = CreateUserWithAnAccountUsingExcelSheet.ReadExcelFile(file);
            return new FileContentResult(result, "application/vnd.ms-excel") { FileDownloadName = "Response-CreateUser.xlsx" };
        }


        #endregion


        #region Get Sub Industry Dynamic dropdown based on industry id
        [HttpGet]
        public ActionResult GetSubIndustryForAccountant(int industryId)
        {
            UserWithAnAccountantViewModel ObjView = new UserWithAnAccountantViewModel();
            ObjView.SubIndustryList = ClsDropDownList.GetSubIndustryListById(industryId);
            return PartialView(ObjView);
        }
        #endregion

        #endregion

        #endregion

        #region UploadStatement
        public JsonResult AllExpenseListForUserWithAnAccountant(int userId, int selectedUserId, int? StatusId, int roleId, int? Year, int? Month, int? BankId, int? CategoryId, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetUserExpenseListForUserWithAnAccountant(page, limit, sortBy, direction, searchString, out total, userId, selectedUserId, StatusId, 3, Year, Month, BankId, CategoryId);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _GetStatementType(int bankId)
        {
            return PartialView(bankId);
        }
        [KippinAuthorize]
        [HttpGet]
        public ActionResult UploadStatement(string result)
        {
            if (!string.IsNullOrEmpty(result) && result.ToLower() == "true")
            {
                ViewBag.Message = "File Uploaded Successfully";
            }
            if (!string.IsNullOrEmpty(result) && result == "ClosedFiscalYear")
            {
                ViewBag.Message = "Fiscal year is closed";
            }

            if (!string.IsNullOrEmpty(result) && result.ToLower() == "false")
            {
                ViewBag.Message = "Error uploading file. Please try again";
            }
            return View();
        }

        public ActionResult UploadStatementList(string status, int reconcilationType)
        {

            UploadStatementParent ObjView = new UploadStatementParent();
            if (status == "Yes")
            {
                ObjView.IsExisting = true;
                //show friendly name  textbox

            }
            else if (status == "No")
            {
                ///show friendlyname dropdown
                ObjView.IsExisting = false;
            }

            var user = UserData.GetCurrentUserData();

            if (user.RoleId == 1)
            {
                ObjView.UserList = ClsDropDownList.PopulateUser(user.Id, true);
                int SelectedActiveUser = 0;
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        int.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                    }

                }
                ObjView.selectedUserId = Convert.ToInt32(SelectedActiveUser);
            }
            else
            {
                ObjView.UserList = ClsDropDownList.PopulateUser(user.Id, false);
            }
            int userId = 0;
            if (reconcilationType == 2)
            {
                //Automatic banks from yodlee

                if (user.RoleId == 1)
                {
                    userId = ObjView.selectedUserId;
                }
                else
                {
                    userId = user.Id;
                }
                ObjView.BankList = ClsDropDownList.PopulateUserBank(userId, false);
            }
            else
            {
                ObjView.BankList = ClsDropDownList.PopulateUserBank(userId, true);
            }

            ObjView.UserId = user.Id;
            ObjView.RoleId = Convert.ToInt32(user.RoleId);
            return View(ObjView);
        }

        [KippinAuthorize]
        [HttpPost]
        public JsonResult UploadStatementList(HttpPostedFileBase file, int selectedUserId, string FriendlyName, int bankid, string bankname, int selectedStatementType)
        {
            using (var _expenserepo = new ExpenseRepository())
            {
                string result = string.Empty;
                string BankName = string.Empty;
                switch (bankid)
                {
                    case 1:
                        BankName = "RBC";
                        break;
                    case 2:
                        BankName = "Scotia";
                        break;
                    case 3:
                        BankName = "BMO";
                        break;
                    case 4:
                        BankName = "TD";
                        break;
                    case 5:
                        BankName = "CIBC";
                        break;
                    default:
                        BankName = bankname;
                        break;
                }
                try
                {
                    int userId = 0;
                    if (string.IsNullOrEmpty(FriendlyName))
                    {
                        switch (bankid)
                        {
                            case 1:
                                FriendlyName = "RBC";
                                break;
                            case 2:
                                FriendlyName = "Scotia";
                                break;
                            case 3:
                                FriendlyName = "BMO";
                                break;
                            case 4:
                                FriendlyName = "TD";
                                break;
                            case 5:
                                FriendlyName = "CIBC";
                                break;
                            default:
                                FriendlyName = bankname;
                                break;
                        }
                    }
                    var chartNumber = _expenserepo.CheckNumberOfAccountByUserId(selectedUserId, bankid, FriendlyName, selectedStatementType);
                    int chkAccNos = chartNumber.Item2;
                    int accClassificationNumber = 0000;
                    if (chkAccNos >= 4)
                    {
                        result = "false";
                        TempData["chkAccNos"] = "You aready added four bank accounts. Please upload statement in these existing accounts.";
                        return Json("Failure", JsonRequestBehavior.AllowGet);
                    }

                    if (selectedStatementType == 1)
                    {
                        switch (chkAccNos)
                        {
                            case 1:
                                accClassificationNumber = 1061;
                                break;
                            case 2:
                                accClassificationNumber = 1062;
                                break;
                            case 3:
                                accClassificationNumber = 1063;
                                break;
                            case 0:
                                accClassificationNumber = 1060;
                                break;
                        }
                    }
                    else if (selectedStatementType == 2)
                    {
                        switch (chkAccNos)
                        {
                            case 1:
                                accClassificationNumber = 1081;
                                break;
                            case 2:
                                accClassificationNumber = 1082;
                                break;
                            case 3:
                                accClassificationNumber = 1083;
                                break;
                            case 0:
                                accClassificationNumber = 1080;
                                break;
                        }
                    }

                    string UpdateDate = "";
                    userId = selectedUserId;
                    string statementName = file.FileName;
                    if (chartNumber.Item1)
                    {
                        accClassificationNumber = Convert.ToInt32(chartNumber.Item3);
                    }
                    switch (bankid)
                    {
                        case 1:
                            //RBC
                            var _bankexpense = ReadCSV.ReadRBCBankStatementFile(file, bankid, userId);
                            result = _expenserepo.SaveBankExpense(_bankexpense, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                            break;
                        case 2:
                            //Scotia
                            var ScotiaCsvData = ReadCSV.GetScotiaBankData(file, bankid, userId, 1, 6);
                            if (ScotiaCsvData.Count > 0)
                            {
                                result = _expenserepo.SaveScotiaBankExpense(ScotiaCsvData, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                            }
                            break;
                        case 3:
                            //BMO No bank statement available
                            var BmoCsvData = ReadCSV.GetBmoBankData(file, bankid, userId, 1, 6);
                            if (BmoCsvData.Count > 0)
                            {
                                result = _expenserepo.SaveBmoBankExpense(BmoCsvData, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                            }
                            break;
                        case 4:
                            //TD
                            var TdCsvData = ReadCSV.GetTdBankData(file, bankid, userId, 1, 6);
                            if (TdCsvData.Count > 0)
                            {
                                result = _expenserepo.SaveTdBankExpense(TdCsvData, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                            }
                            break;
                        case 5:
                            //CIBC
                            var CibcCsvData = ReadCSV.GetCibcBankData(file, bankid, userId, 1, 6);
                            if (CibcCsvData.Count > 0)
                            {
                                result = _expenserepo.SaveCibcBankExpense(CibcCsvData, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                            }
                            break;
                        //default:
                        //    if (bankname.ToLower().Contains("royal"))
                        //    {
                        //        var _bankexpense1 = ReadCSVFile(file, bankid, userId);
                        //        result = _expenserepo.SaveBankExpense(_bankexpense1, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                        //    }
                        //    else if (bankname.ToLower().Contains("scotia"))
                        //    {
                        //        //Scotia
                        //        var ScotiaCsvData1 = Getdata.GetScotiaBankData(file, bankid, userId, 1, 6);
                        //        if (ScotiaCsvData1.Count > 0)
                        //        {
                        //            result = _expenserepo.SaveScotiaBankExpense(ScotiaCsvData1, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                        //        }
                        //    }
                        //    else if (bankname.ToLower().Contains("bmo"))
                        //    {
                        //        var BmoCsvData1 = Getdata.GetBmoBankData(file, bankid, userId, 1, 6);
                        //        if (BmoCsvData1.Count > 0)
                        //        {
                        //            result = _expenserepo.SaveBmoBankExpense(BmoCsvData1, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                        //        }
                        //    }
                        //    else if (bankname.ToLower().Contains("td"))
                        //    {
                        //        var TdCsvData1 = Getdata.GetTdBankData(file, bankid, userId, 1, 6);
                        //        if (TdCsvData1.Count > 0)
                        //        {
                        //            result = _expenserepo.SaveTdBankExpense(TdCsvData1, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                        //        }
                        //    }
                        //    else if (bankname.ToLower().Contains("cibc"))
                        //    {
                        //        var CibcCsvData1 = Getdata.GetCibcBankData(file, bankid, userId, 1, 6);
                        //        if (CibcCsvData1.Count > 0)
                        //        {
                        //            result = _expenserepo.SaveCibcBankExpense(CibcCsvData1, UpdateDate, bankid, userId, FriendlyName, statementName, "M", selectedStatementType, accClassificationNumber).ToString();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        //Do nothing bcoz bank format is not available
                        //        result = "false";
                        //        return Json("Failure", JsonRequestBehavior.AllowGet);
                        //    }
                        //    break;
                    }
                    if (result == "false")
                    {
                        return Json("Failure", JsonRequestBehavior.AllowGet);
                    }
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    result = "false";
                    return Json("Failure", JsonRequestBehavior.AllowGet);
                }

            }
        }

        [KippinAuthorize]
        [HttpPost]
        public JsonResult CheckExistingRecord(HttpPostedFileBase file, int selectedUserId, string FriendlyName, int bankid, string bankname)
        {
            try
            {
                int userId = 0;
                string result = "false";
                using (var repo = new ExpenseRepository())
                {
                    if (!string.IsNullOrEmpty(FriendlyName))
                    {
                        userId = selectedUserId;
                        string fileName = file.FileName;
                        string friendlyAccName = FriendlyName;
                        string excelDate = string.Empty;
                        switch (bankid)
                        {
                            case 1:
                                //RBC
                                var _bankexpense = ReadCSV.ReadRbcLatestDate(file, 1, bankname).Split('^');
                                string accNo = _bankexpense[1];
                                excelDate = _bankexpense[0];
                                result = repo.RbcBankStatementExistCheck(Convert.ToDateTime(_bankexpense[0]), userId, 1, friendlyAccName, fileName, accNo).ToString() + " " + _bankexpense[0];
                                break;
                            case 2:
                                //Scotia
                                var ScotiaCsvData = ReadCSV.ReadLatestDate(file, 2, bankname);
                                excelDate = ScotiaCsvData.ToShortDateString();
                                result = repo.BankStatementExistCheck(ScotiaCsvData, userId, 2, friendlyAccName, fileName).ToString() + " " + ScotiaCsvData.ToShortDateString();

                                break;
                            case 3:
                                //BMO No bank statement available
                                var BmoCsvData = ReadCSV.ReadLatestDate(file, 3, bankname);
                                excelDate = BmoCsvData.ToShortDateString();
                                result = repo.BankStatementExistCheck(BmoCsvData, userId, 3, friendlyAccName, fileName).ToString() + " " + BmoCsvData.ToShortDateString();

                                break;
                            case 4:
                                //TD
                                var TdCsvData = ReadCSV.ReadLatestDate(file, 4, bankname);
                                excelDate = TdCsvData.ToShortDateString();
                                result = repo.BankStatementExistCheck(TdCsvData, userId, 4, friendlyAccName, fileName).ToString() + " " + TdCsvData.ToShortDateString();

                                break;
                            case 5:
                                //CIBC
                                var CibcCsvData = ReadCSV.ReadLatestDate(file, 5, bankname);
                                excelDate = CibcCsvData.ToShortDateString();
                                result = repo.BankStatementExistCheck(CibcCsvData, userId, 5, friendlyAccName, fileName).ToString() + " " + CibcCsvData.ToShortDateString();
                                break;
                            //default:
                            //    if (bankname.ToLower().Contains("royal"))
                            //    {
                            //        var _bankexpense1 = Getdata.ReadRbcLatestDate(file, bankid, bankname).Split('^');
                            //        string accNo1 = _bankexpense1[1];
                            //        excelDate = _bankexpense1[0];
                            //        result = _expenserepo.RbcBankStatementExistCheck(Convert.ToDateTime(_bankexpense1[0]), userId, bankid, friendlyAccName, fileName, accNo1).ToString() + " " + _bankexpense1[0];
                            //        break;
                            //    }
                            //    else if (bankname.ToLower().Contains("scotia"))
                            //    {
                            //        //Scotia
                            //        var ScotiaCsvData1 = Getdata.ReadLatestDate(file, bankid, bankname);
                            //        excelDate = ScotiaCsvData1.ToShortDateString();
                            //        result = _expenserepo.BankStatementExistCheck(ScotiaCsvData1, userId, bankid, friendlyAccName, fileName).ToString() + " " + ScotiaCsvData1.ToShortDateString();

                            //        break;
                            //    }
                            //    else if (bankname.ToLower().Contains("bmo"))
                            //    {
                            //        //BMO No bank statement available
                            //        var BmoCsvData1 = Getdata.ReadLatestDate(file, bankid, bankname);
                            //        excelDate = BmoCsvData1.ToShortDateString();
                            //        result = _expenserepo.BankStatementExistCheck(BmoCsvData1, userId, bankid, friendlyAccName, fileName).ToString() + " " + BmoCsvData1.ToShortDateString();
                            //        break;
                            //    }
                            //    else if (bankname.ToLower().Contains("td"))
                            //    {
                            //        var TdCsvData1 = Getdata.ReadLatestDate(file, bankid, bankname);
                            //        excelDate = TdCsvData1.ToShortDateString();
                            //        result = _expenserepo.BankStatementExistCheck(TdCsvData1, userId, bankid, friendlyAccName, fileName).ToString() + " " + TdCsvData1.ToShortDateString();

                            //        break;
                            //    }
                            //    else if (bankname.ToLower().Contains("cibc"))
                            //    {
                            //        var CibcCsvData1 = Getdata.ReadLatestDate(file, bankid, bankname);
                            //        excelDate = CibcCsvData1.ToShortDateString();
                            //        result = _expenserepo.BankStatementExistCheck(CibcCsvData1, userId, bankid, friendlyAccName, fileName).ToString() + " " + CibcCsvData1.ToShortDateString();
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        //Do nothing bcoz bank format is not available
                            //        result = "false";
                            //    }
                            //    break;
                        }
                        if (!string.IsNullOrEmpty(excelDate))
                        {
                            if (repo.CheckFiscalYearCloser(userId, excelDate))
                            {
                                result = "close";
                            }
                        }
                    }

                }


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult GetUserListforUploadingStatementForAccountant(int? selectedUserId, int? selectedBankId, int? selectedStatementType)
        {
            UploadStatementParent ObjView = new UploadStatementParent();
            var user = UserData.GetCurrentUserData();
            if (user.RoleId == 1)
            {
                ObjView.UserList = ClsDropDownList.PopulateUser(user.Id, true);
            }
            else
            {
                ObjView.UserList = ClsDropDownList.PopulateUser(user.Id, false);
            }
            ObjView.selectedUserId = Convert.ToInt32(selectedUserId);
            ObjView.selectedBankId = Convert.ToInt32(selectedBankId);
            // ObjView.StatementTypeId = selectedStatementType;
            ObjView.UserId = user.Id;
            ObjView.RoleId = Convert.ToInt32(user.RoleId);
            using (var repo = new AccountRepository())
            {
                ObjView.friendlynameList = repo.GetUserAccountName(Convert.ToInt32(selectedUserId), Convert.ToInt32(selectedBankId), selectedStatementType);
            }
            return PartialView("~/Views/Accounting/Partials/_GetFriendlynameListByUserId.cshtml", ObjView);
        }
        #endregion

        #region Get User Details
        public Tuple<Int32, Int32> GetUserDetails()
        {
            Int32 UserId = 0;
            Int32 SelectedUserId = 0;
            var getCurrentUserData = UserData.GetCurrentUserData();
            UserId = getCurrentUserData.Id;
            if (getCurrentUserData.RoleId != 2)
            {
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        int.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedUserId);
                    }
                }
            }
            else
            {
                SelectedUserId = getCurrentUserData.Id;
            }
            Tuple<Int32, Int32> getUserData = new Tuple<Int32, Int32>(UserId, SelectedUserId);
            return getUserData;
        }
        #endregion

        #region Statement Section
        [KippinAuthorize(Roles = "Accountant,User as an accountant,Sub accountant")]
        [HttpGet]
        public ActionResult Reconciliation(ReconcillationViewModel ObjView, int? uid)
        {
            try
            {
                if(uid == 3)
                {
                    //close fiscal entry
                    ViewBag.DeleteFiscalYearEntry = "True";
                }
                //uid is used when we delete the record
                var objUser = UserData.GetCurrentUserData();
                ObjView.UserId = objUser.Id;
                ObjView.RoleId = Convert.ToInt32(objUser.RoleId);
                ObjView.CategoryList = ClsDropDownList.PopulateCategory();
                ObjView.MonthList = ClsDropDownList.PopulateMonth();
                ObjView.YearList = ClsDropDownList.PopulateYear();
                ObjView.StatusList = ClsDropDownList.PopulateStatus();


                if (objUser.RoleId == 2)
                {
                    ObjView.StatementTypeList = ClsDropDownList.PopulateUserBank(objUser.Id);
                    ObjView.selectedUserId = Convert.ToInt32(objUser.Id);
                }
                else
                {
                    int SelectedActiveUser = 0;
                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            int.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }

                    }
                    if (objUser.RoleId == 1)
                    {
                        ObjView.UserList = ClsDropDownList.PopulateUser(objUser.Id);
                    }
                    else if (objUser.RoleId == 4)
                    {
                        ObjView.UserList = ClsDropDownList.PopulateUser(Convert.ToInt32(objUser.AccountantId));
                    }
                    ObjView.StatementTypeList = ClsDropDownList.PopulateUserBank(SelectedActiveUser);
                    ObjView.selectedUserId = SelectedActiveUser;
                }
            }
            catch (Exception Ex)
            {
                var error = Ex.Message;
            }
            return View(ObjView);
        }

        public JsonResult AllUserExpenseList(int? StatusId, int roleId, int? Year, int? Month, int? BankId, int? JVID, int? CategoryId, bool? IsSecondLoad, int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var userData = GetUserDetails();
            var records = new GridModel().GetUserExpenseList(page, limit, sortBy, direction, searchString, out total, userData.Item1, userData.Item2, StatusId, roleId, Year, Month, BankId, JVID, CategoryId, IsSecondLoad);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(ReconcillationViewModel ObjView)
        {
            try
            {
                using (var expenseRepository = new ReconcillationRepository())
                {
                    int userid = 0;
                    if (ObjView.selectedUserId > 0)
                    {
                        userid = ObjView.selectedUserId;
                    }
                    else
                    {
                        userid = ObjView.UserId;
                    }
                    var itemList = new List<StatementList>();

                    //int selectedtype = Convert.ToInt16(ObjView.selectedStatementTypeId.Split('_')[1].Trim())==null?0:Convert.ToInt16(ObjView.selectedStatementTypeId.Split('_')[1].Trim());
                    int selectedtype = ObjView.selectedStatementTypeId == null ? 0 : Convert.ToInt16(ObjView.selectedStatementTypeId);
                    itemList = expenseRepository.GetPrintData(userid, ObjView.StatusId, ObjView.selectedYearId, ObjView.selectedMonthId, selectedtype, ObjView.JVID, ObjView.selectedCategoryId);


                    DataTable dt = new DataTable();
                    dt.Columns.Add("JVID", typeof(int));
                    dt.Columns.Add("Date", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Debit", typeof(double));
                    dt.Columns.Add("Credit", typeof(double));
                    dt.Columns.Add("Classification", typeof(string));
                    dt.Columns.Add("Bank", typeof(string));
                    dt.Columns.Add("UploadType", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    foreach (var item in itemList)
                    {
                        dt.Rows.Add(item.JVID, item.StatementDate, item.StatementDescription, decimal.Round(Convert.ToDecimal(item.Debit), 2, MidpointRounding.AwayFromZero), decimal.Round(Convert.ToDecimal(item.Credit), 2, MidpointRounding.AwayFromZero), item.StatementClassification, item.StatementBank, item.StatementUploadType, item.StatementStatus);
                    }
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        Response.Clear();
                        Response.ClearHeaders();
                        ExcelWorksheet ws = package.Workbook.Worksheets.Add("Response");
                        ws.Cells["A1"].LoadFromDataTable(dt, true);
                        var stream = new MemoryStream();
                        package.SaveAs(stream);

                        string fileName = "Response-User-Statements.xlsx";
                        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                        stream.Position = 0;
                        return File(stream, contentType, fileName);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ReconciliationFilters(ReconcillationViewModel ObjView, string Command)
        {
            try
            {
                var currentUser = UserData.GetCurrentUserData();
                if (Command == "Save")
                {
                    if (ObjView.StatusId > 0 || ObjView.selectedYearId > 0 || ObjView.selectedMonthId > 0 || ObjView.selectedStatementTypeId != "" || ObjView.selectedCategoryId > 0 || ObjView.JVID > 0)
                    {
                        ObjView.enablePrintBtn = true;
                    }
                }
                if (Command == "Cash")
                {
                    if (currentUser.RoleId == 2)
                    {
                        return RedirectToAction("AddManualEntry");
                    }
                    else if (currentUser.RoleId == 4 || currentUser.RoleId == 1)
                    {
                        if (ObjView.selectedUserId > 0)
                        {
                            return RedirectToAction("AddManualEntry");
                        }
                        else
                        {
                            return RedirectToAction("ActiveUser");
                        }
                    }

                }
                if (Command == "Print")
                {
                    return RedirectToAction("Print", "Accounting", ObjView);
                }
                //AddClassification
                if (Command == "AddClassification")
                { return RedirectToAction("CreateClassification"); }
                if (Command == "Locked")
                {
                    //Update the Bank status for selected month and year
                    if (ObjView.selectedYearId > 0 && ObjView.selectedMonthId > 0)
                    {
                        if (currentUser.RoleId == 1)
                        {
                            if (ObjView.selectedUserId > 0)
                            {
                                using (var repository = new ReconcillationRepository())
                                {
                                    bool updateExpense = repository.LockBankExpense(ObjView.selectedMonthId, ObjView.selectedYearId, ObjView.selectedUserId, 4); //4 is statusID for locking the statement
                                    TempData["LockStatementError"] = "Statement successfully locked for the selected month and year.";
                                }
                            }
                            else
                            {
                                TempData["LockStatementError"] = "Please select user for locking the bank statement for particular period";
                            }
                        }
                        else
                        {
                            using (var repository = new ReconcillationRepository())
                            {
                                ObjView.selectedUserId = currentUser.Id;
                                bool updateExpense = repository.LockBankExpense(ObjView.selectedMonthId, ObjView.selectedYearId, ObjView.selectedUserId, 4);
                                TempData["LockStatementError"] = "Statement successfully locked for the selected month and year.";
                            }
                        }

                    }
                    else
                    {
                        TempData["LockStatementError"] = "Please select both month and year for locking the bank statement for particular period";
                    }
                }

                if (currentUser.RoleId != 4 && currentUser.RoleId != 1)
                {
                    ObjView.selectedUserId = currentUser.Id;
                }
                else
                {
                    if (ObjView.selectedUserId == 0)
                    {
                        if (Command != "Locked")
                        {
                            //  TempData["StatementSearchError"] = "Please select user for see the bank statements.";
                        }

                    }
                }

                //  ObjView.selectedUserId = ObjView.selectedUserId;

            }
            catch (Exception Ex)
            {
                var error = Ex.Message;
            }
            ObjView.IsSecondLoad = true;
            return RedirectToAction("Reconciliation", ObjView);
        }

        #region New Mannual Entry Module
        public ActionResult SaveManualEntries(int userId)
        {
            //get the data from session and save the values and null the session
            List<AddCashExpenseViewModel> objManualEntriesList = new List<AddCashExpenseViewModel>();
            if (Session["ManualEntriesList"] != null)
            {
                objManualEntriesList = (List<AddCashExpenseViewModel>)Session["ManualEntriesList"];

                if (Decimal.Compare(decimal.Round(Convert.ToDecimal(objManualEntriesList.Select(s => s.Debit).Sum()), 2, MidpointRounding.AwayFromZero), decimal.Round(Convert.ToDecimal(objManualEntriesList.Select(s => s.Credit).Sum()), 2, MidpointRounding.AwayFromZero)) == 0)
                {
                    Session["ManualEntriesList"] = null;
                    Session["ManualEntryBills"] = null;
                    if (objManualEntriesList.Count > 0)
                    {

                        bool isCredit = false;
                        bool isDebit = false;
                        int entryCount = 0;
                        int referenceId = 0;
                        foreach (var item in objManualEntriesList)
                        {
                            using (var repo = new ExpenseRepository())
                            {
                                if (referenceId > 0)
                                {
                                    item.ReferenceId = referenceId;
                                }
                                if (entryCount > 0)
                                {
                                    if (isCredit == true)
                                    {
                                        if (item.Debit > 0)
                                        {
                                            item.IsVirtualEntry = true;
                                        }
                                    }
                                    else if (isDebit == true)
                                    {
                                        if (item.Credit > 0)
                                        {
                                            item.IsVirtualEntry = true;
                                        }
                                    }
                                }
                                Mapper.CreateMap<AddCashExpenseViewModel, AddMjvEntryDto>();
                                var MjvData = Mapper.Map<AddMjvEntryDto>(item);
                                var InsertCashEntry = repo.SaveMjvEntry(MjvData);
                                if (InsertCashEntry > 0)
                                {
                                    if (entryCount == 0)
                                    {
                                        if (item.Credit > item.Debit)
                                        {
                                            isCredit = true;
                                        }
                                        else if (item.Debit > item.Credit)
                                        {
                                            isDebit = true;
                                        }
                                        referenceId = InsertCashEntry;

                                    }
                                    entryCount = entryCount + 1;
                                    if (item.objManualEntryBills != null)
                                    {
                                        string StatementId = Convert.ToString(InsertCashEntry);
                                        string sourcePath = Server.MapPath("~/OcrImages/" + item.UserId + "/");
                                        string targetPath = Server.MapPath("~/OcrImages/" + item.UserId + "/") + "/" + StatementId + "/";

                                        if (!Directory.Exists(targetPath))
                                        {
                                            Directory.CreateDirectory(targetPath);
                                        }
                                        //foreach (var srcPath in Directory.GetFiles(src))
                                        foreach (var srcPath in item.objManualEntryBills)
                                        {
                                            string sourceFile = System.IO.Path.Combine(sourcePath, srcPath);
                                            string destFile = System.IO.Path.Combine(targetPath, srcPath);
                                            System.IO.File.Move(sourceFile, destFile);

                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                else
                {
                    return RedirectToAction("AddManualEntry", new { userId = userId, Status = "True", SaveEntry = "False" });
                }

            }

            return RedirectToAction("AddManualEntry", new { userId = userId, SaveEntry = "True" });
        }
        [HttpGet]
        [KippinAuthorize(Roles = "Accountant,Sub accountant,User as an accountant")]
        public ActionResult AddManualEntry(string Status, string SaveEntry, string Date, string Comment)
        {
            if (Status != "True")
            {
                Session["ManualEntriesList"] = null;
                Session["ManualEntryBills"] = null;

            }
            if (SaveEntry == "False")
            {
                ModelState.AddModelError("CustomError", "Credit and Debit total is not equal unable to save entries.");
            }
            if (SaveEntry == "True")
            {
                ModelState.AddModelError("CustomError", "Entries Successfully added.");
            }
            AddCashExpenseViewModel objView = new AddCashExpenseViewModel();
            int userId = 0;
            #region Get Selected User or active user
            var currentLoggedInUserData = UserData.GetCurrentUserData();
            if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
            {
                //User As An Accountant
                userId = currentLoggedInUserData.Id;
            }
            else
            {
                //Accountant / Sub Accountant

                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        int.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out userId);
                    }
                }
                if (userId < 1)
                    return RedirectToAction("ActiveUser", "Kippin");
            }

            #endregion
            objView.UserId = userId;
            objView.ClassificationId = ClsDropDownList.GetDefaultClassificationForMJVentries();
            objView.CategoryList = ClsDropDownList.PopulateCategory();
            var currentUser = UserData.GetCurrentUserData();
            objView.CustomClassificationList = ClsDropDownList.PopulateCustomClassificationForManualEntry(userId);

            if (!string.IsNullOrEmpty(Date))
            {
                objView.Date = Date;
            }

            objView.Comment = Comment;
            return View(objView);
        }

        [HttpPost]
        public ActionResult AddManualEntry(AddCashExpenseViewModel objView)
        {
            if (!string.IsNullOrEmpty(objView.Date))
            {
                if (UserData.CheckUserFiscalYearClosed(objView.UserId, objView.Date))
                {
                    ModelState.AddModelError("FiscalYear", "Fiscal year is closed.");
                }
                else
                {
                    //decimal FirstTax = Decimal.Add(Convert.ToDecimal(objView.GSTtax), Convert.ToDecimal(objView.QSTtax));
                    //decimal SecondTax = Decimal.Add(Convert.ToDecimal(objView.HSTtax), Convert.ToDecimal(objView.PSTtax));
                    //decimal Tax = Decimal.Add(FirstTax, SecondTax);
                    if (objView.Classification == "1")
                    {
                        objView.ClassificationId = Convert.ToInt32(objView.Classification);
                        ModelState.AddModelError("ClassificationId", "Please select classification");
                    }
                    //else if (Decimal.Subtract(Convert.ToDecimal(objView.BillTotal), Tax) <= 0)
                    //{
                    //    ModelState.AddModelError("CustomError", "please complete fields.");
                    //}
                    else if (objView.Credit == null && objView.Debit == null)
                    {
                        ModelState.AddModelError("CustomError", "Please add either credit or debit");
                    }
                    else if (objView.Credit > 0 && objView.Debit > 0)
                    {
                        ModelState.AddModelError("CustomError", "Please add either credit or debit");
                    }
                    else
                    {
                        //  var getRoleId = UserData.GetCurrentUserData();              
                        if (ModelState.IsValid)
                        {
                            using (var repo = new ExpenseRepository())
                            {
                                List<AddCashExpenseViewModel> objManualEntriesList = new List<AddCashExpenseViewModel>();
                                objView.ClassificationId = Convert.ToInt32(objView.Classification);
                                if (Session["ManualEntriesList"] != null)
                                {
                                    objManualEntriesList = (List<AddCashExpenseViewModel>)Session["ManualEntriesList"];
                                    Session["ManualEntriesList"] = null;
                                }
                                if (Session["ManualEntryBills"] != null)
                                {
                                    objView.objManualEntryBills = (List<string>)Session["ManualEntryBills"];
                                    Session["ManualEntryBills"] = null;
                                }
                                objView.Id = objManualEntriesList.Count + 1;
                                objManualEntriesList.Add(objView);
                                Session["ManualEntriesList"] = objManualEntriesList;
                                return RedirectToAction("AddManualEntry", new { Status = "True", Date = objView.Date, Comment = objView.Comment });
                            }
                        }
                    }
                }
            }
            objView.CategoryList = ClsDropDownList.PopulateCategory();
            objView.CustomClassificationList = ClsDropDownList.PopulateCustomClassificationForManualEntry(objView.UserId);
            return View(objView);
        }


        public ActionResult RemoveManualEnrty(int manualEntryId, int userId)
        {
            if (Session["ManualEntriesList"] != null)
            {
                List<AddCashExpenseViewModel> objManualEntriesList = new List<AddCashExpenseViewModel>();
                objManualEntriesList = (List<AddCashExpenseViewModel>)Session["ManualEntriesList"];
                Session["ManualEntriesList"] = null;
                var deleteEntry = objManualEntriesList.Where(a => a.Id == manualEntryId).FirstOrDefault();
                objManualEntriesList.Remove(deleteEntry);
                Session["ManualEntriesList"] = objManualEntriesList;
            }
            return RedirectToAction("AddManualEntry", new { userId = userId, Status = "True" });
        }

        public JsonResult UploadManualEntryBill(FormCollection form)
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
            var file = Request.Files[0];
            int userId = Convert.ToInt32(form["UserId"]);
            string Path = string.Empty;
            try
            {
                string folder = Server.MapPath("~/OcrImages/" + userId + "/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var filePath = Server.MapPath("~" + "/OcrImages/" + userId + "/" + dateTime + file.FileName);
                file.SaveAs(filePath);
                Path = "/OcrImages/" + userId + "/" + dateTime + file.FileName;
                List<string> objManualEntryBills = new List<string>();
                if (Session["ManualEntryBills"] != null)
                {
                    objManualEntryBills = (List<string>)Session["ManualEntryBills"];
                    Session["ManualEntryBills"] = null;
                }
                objManualEntryBills.Add(dateTime + file.FileName);
                Session["ManualEntryBills"] = objManualEntryBills;
            }
            catch (Exception)
            {
                throw;
            }
            return Json(Path, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadManualEntryBillFromCloud(FormCollection form)
        {
            using (var expenseRepository = new ExpenseRepository())
            {
                var dateTime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
                string sourceFile = Convert.ToString(form["FilePath"]);
                sourceFile = sourceFile.Replace(@"/\", @"\");
                sourceFile = sourceFile.Replace(@"/", @"\");
                int userId = Convert.ToInt32(form["UserId"]);
                string Path = string.Empty;
                try
                {
                    string folder = Server.MapPath("~/OcrImages/" + Convert.ToInt32(form["UserId"]) + "/");
                    string destFile = System.IO.Path.Combine(folder);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    var aa = sourceFile.Split(new[] { "CameraUploadImages" }, StringSplitOptions.None);
                    var bb = aa[1].Split(new[] { "\\" }, StringSplitOptions.None);
                    sourceFile = Server.MapPath("~") + sourceFile;
                    sourceFile = sourceFile.Replace(@"\\", @"\");
                    System.IO.File.Copy(sourceFile, destFile + bb[4], true);
                    CloudImagesRecordDto obj = new CloudImagesRecordDto();
                    obj.ImageName = bb[4];
                    obj.DateCreated = DateTime.Now;
                    obj.IsAssociated = true;
                    obj.StatementId = Convert.ToInt32(form["StatementId"]);
                    obj.UserId = Convert.ToInt32(form["UserId"]);
                    obj.Month = Convert.ToInt32(bb[3]);
                    obj.Year = Convert.ToInt32(bb[2]);
                    var addImageToCloudDbEntry = expenseRepository.AddImageToCloud(obj);

                    Path = "/OcrImages/" + userId + "/" + bb[4];
                    List<string> objManualEntryBills = new List<string>();
                    if (Session["ManualEntryBills"] != null)
                    {
                        objManualEntryBills = (List<string>)Session["ManualEntryBills"];
                        Session["ManualEntryBills"] = null;
                    }
                    objManualEntryBills.Add(bb[4]);
                    Session["ManualEntryBills"] = objManualEntryBills;
                }
                catch (Exception)
                {
                    throw;
                }
                return Json(Path, JsonRequestBehavior.AllowGet);

            }
        }
        #endregion

        [HttpPost]
        public JsonResult GetChartAccountNumberByClassificationId(string id)
        {
            int classificationId = Convert.ToInt32(id);
            JsonResult data = new JsonResult();
            data.Data = ClsDropDownList.GetCategoryById(classificationId);
            data.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return data;
        }
        [HttpPost]
        public JsonResult GetCategoryById(string id)
        {
            int classificationId = Convert.ToInt32(id);
            JsonResult data = new JsonResult();
            data.Data = ClsDropDownList.GetCategoryById(classificationId);
            data.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return data;
        }

        #region Classification Section
        public ActionResult CreateClassification(string status, bool? IsFirstLoad)
        {
            long SelectedActiveUser = 0;
            if (Request.Cookies["SelectedActiveUser"] != null)
            {
                if (Request.Cookies["SelectedActiveUser"].Value != null)
                {
                    long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                }

            }
            if (IsFirstLoad == null || IsFirstLoad == true)
            {
                Session["ManualClassificationList"] = null;
            }
            if (!string.IsNullOrEmpty(status))
            {
                ViewBag.Status = "Classification successfully added.";
            }

            AddClassificationViewModel obj = new AddClassificationViewModel()
            {
                CategoryList = ClsDropDownList.SPopulateCategory(),
                ClassificationTypeList = ClsDropDownList.PopulateClassificationType(),

                SName = ClsDropDownList.SPopulateCategory(),
                CategoryList2 = ClsDropDownList.T1PopulateCategory(Convert.ToInt32(SelectedActiveUser)),
                CategoryList3 = ClsDropDownList.T2PopulateCategory(Convert.ToInt32(SelectedActiveUser)),
                CategoryList4 = ClsDropDownList.ZPopulateCategory()

            };
            ViewBag.Isexists = "";
            return View(obj);
        }

        public JsonResult getsname()
        {
            using (var context = new KF.Entity.KFentities())
            {
                var selectedlist = context.Classifications.Where(s => s.Type == "S").Select(s => new
                {
                    SName = s.ClassificationType + "-" + s.ChartAccountDisplayNumber
                    ,
                    SValues = s.ChartAccountDisplayNumber.Substring(0, 4)
                }).ToList();
                return Json(selectedlist, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult checkchartaccountnumberexists(String ChartAccountNumbers)
        {
            String IsExist = "";
            using (var context = new KF.Entity.KFentities())
            {
                IsExist = Convert.ToString(context.Classifications.Where(s => s.ChartAccountDisplayNumber == ChartAccountNumbers).Any());
            }
            return Json(IsExist, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckAccountExists(string AccountNumbers, string type)
        {
            using (var dbcontext = new KF.Entity.KFentities())
            {
                if (type == "A")
                {
                    if (dbcontext.Classifications.Where(s => s.ChartAccountDisplayNumber == AccountNumbers).Any())
                    {
                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }

                }

                else if (type == "S")
                {
                    if (dbcontext.Classifications.Where(s => s.ChartAccountDisplayNumber == AccountNumbers + "-5000").Any())
                    {

                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
                else if (type == "T")
                {
                    if (dbcontext.Classifications.Where(s => s.ChartAccountDisplayNumber == AccountNumbers + "-9999").Any())
                    {

                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (dbcontext.Classifications.Where(s => s.ChartAccountDisplayNumber == AccountNumbers).Any())
                    {

                        return Json("1", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("0", JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }



        [HttpPost]
        public ActionResult CreateClassification(AddClassificationViewModel obj, string Type)
        {
            #region Add classification to list
            // if (obj.CategoryId > 0 && !string.IsNullOrEmpty(obj.ClassificationDesc))
            long SelectedActiveUser1 = 0;
            if (Request.Cookies["SelectedActiveUser"] != null)
            {
                if (Request.Cookies["SelectedActiveUser"].Value != null)
                {
                    long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser1);
                }

            }
            obj.CategoryList = ClsDropDownList.SPopulateCategory();
            obj.CategoryList2 = ClsDropDownList.T1PopulateCategory(Convert.ToInt32(SelectedActiveUser1));
            obj.CategoryList3 = ClsDropDownList.T2PopulateCategory(Convert.ToInt32(SelectedActiveUser1));
            obj.CategoryList4 = ClsDropDownList.ZPopulateCategory();
            obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
            if (!string.IsNullOrEmpty(obj.ClassificationDesc))
            {



                int CategoryId = 0;
                bool ChkClassificationRange = false;
                #region reserver classification

                List<string> objReservedClassification = new List<string>();


                objReservedClassification.Add("1050-1060");
                objReservedClassification.Add("1050-1061");
                objReservedClassification.Add("1050-1062");
                objReservedClassification.Add("1050-1063");
                objReservedClassification.Add("1050-1064");
                objReservedClassification.Add("1050-1073");

                objReservedClassification.Add("1100-1080");
                objReservedClassification.Add("1100-1081");
                objReservedClassification.Add("1100-1082");
                objReservedClassification.Add("1100-1083");
                objReservedClassification.Add("1100-1087");
                objReservedClassification.Add("1100-1088");
                objReservedClassification.Add("1100-1089");


                var userData_new = UserData.GetCurrentUserData();
                #endregion

                String chartaccno = "";
                if (obj.Type == "A")
                {
                    chartaccno = obj.NewChartAccountNumber.Substring(0, 4);
                }
                else if (obj.Type == "G")
                {
                    chartaccno = obj.CategoryValue;
                }
                else
                {
                    chartaccno = obj.ChartAccountNumber;
                }

                if (objReservedClassification.Where(s => s == chartaccno).Any())
                {
                    ModelState.AddModelError("ChartAccountNumber", "Chart Account Number already taken");
                }
                else
                {
                    #region Check Chartaccount number


                    if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 1, 1999))
                    {
                        CategoryId = 1;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 2000, 2999))
                    {
                        CategoryId = 4;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 3000, 3999))
                    {
                        CategoryId = 5;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 4000, 4999))
                    {
                        CategoryId = 3;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 5000, 8999))
                    {
                        CategoryId = 2;
                    }


                    //switch (obj.CategoryId)
                    switch (CategoryId)
                    {
                        case 1:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(chartaccno), 1, 1999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 1000-1999");
                            }
                            break;
                        case 2:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(chartaccno), 5000, 8999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 5000-8999");
                            }
                            break;
                        case 3:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(chartaccno), 4000, 4999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 4000-4999");
                            }
                            break;
                        case 4:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(chartaccno), 2000, 2999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 2000-2999");
                            }
                            break;
                        case 5:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(chartaccno), 3000, 3999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 3000-3999");
                            }
                            break;
                    }
                    #endregion
                }

                if (ChkClassificationRange == true)
                {
                    // Add classification to session object
                    var objList = new List<AddClassificationViewModel>();
                    using (var repo = new ReconcillationRepository())
                    {
                        try
                        {
                            /*Added Check to Insert ChartAccount Number via UserId as well*/
                            if (repo.CheckClassificationAccountNumberExistCheck(chartaccno, userData_new.Id) == false)
                            {
                                var userData = UserData.GetCurrentUserData();
                                if (userData.RoleId == 1)
                                {
                                    long SelectedActiveUser = 0;
                                    if (Request.Cookies["SelectedActiveUser"] != null)
                                    {
                                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                                        {
                                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                                        }

                                    }
                                    if (SelectedActiveUser > 0)
                                    {
                                        obj.UserID = Convert.ToInt32(SelectedActiveUser);
                                    }
                                }
                                else
                                {
                                    obj.UserID = userData.Id;
                                }
                                if (obj.UserID > 0)
                                {
                                    if (repo.CheckWebClassificationNameExistCheck(obj.Type, obj.UserID) == false)
                                    {

                                        if (Type == "A")
                                        {
                                            if (Session["ManualClassificationListA"] != null)
                                            {
                                                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListA"];
                                                Session["ManualClassificationListA"] = null;
                                                obj.Id = objList.OrderByDescending(d => d.Id).Select(s => s.Id).FirstOrDefault() + 1;
                                            }
                                            else
                                            {
                                                obj.Id = 1;
                                            }
                                        }
                                        else if (Type == "S")
                                        {
                                            if (Session["ManualClassificationListS"] != null)
                                            {

                                                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListS"];
                                                Session["ManualClassificationListS"] = null;
                                                obj.Id = objList.OrderByDescending(d => d.Id).Select(s => s.Id).FirstOrDefault() + 1;
                                            }
                                            else
                                            {
                                                obj.Id = 1;
                                            }
                                        }
                                        else if (Type == "T")
                                        {
                                            if (Session["ManualClassificationListT"] != null)
                                            {

                                                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListT"];
                                                Session["ManualClassificationListT"] = null;
                                                obj.Id = objList.OrderByDescending(d => d.Id).Select(s => s.Id).FirstOrDefault() + 1;
                                            }
                                            else
                                            {
                                                obj.Id = 1;
                                            }
                                        }
                                        else if (Type == "G")
                                        {
                                            if (Session["ManualClassificationListG"] != null)
                                            {

                                                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListG"];
                                                Session["ManualClassificationListG"] = null;
                                                obj.Id = objList.OrderByDescending(d => d.Id).Select(s => s.Id).FirstOrDefault() + 1;
                                            }
                                            else
                                            {
                                                obj.Id = 1;
                                            }
                                        }


                                        //}
                                        //else
                                        //{
                                        //    obj.Id = 1;
                                        //}
                                        //obj.CategoryName = UserData.GetCategory(obj.CategoryId);
                                        obj.CategoryName = UserData.GetCategory(CategoryId);
                                        obj.ClassificationTypeName = UserData.GetClassificationType(obj.ClassificationTypeId);

                                        //if (objList.Where(s => s.Type == obj.Type).Any())
                                        //if (objList.Where(s => s.Type == Type).Any())
                                        //{
                                        //    ModelState.AddModelError("Type", "Classification type already added in below list.");
                                        //}
                                        //else 
                                        if (objList.Where(s => s.ChartAccountNumber == chartaccno).Any())
                                        {
                                            ModelState.AddModelError("ChartAccountNumber", "Chart Account Number Already added in below list.");
                                        }
                                        else
                                        {
                                            objList.Add(obj);

                                            if (Type == "A")
                                            {
                                                Session["ManualClassificationListA"] = objList;
                                            }
                                            else if (Type == "S")
                                            {
                                                Session["ManualClassificationListS"] = objList;
                                            }
                                            else if (Type == "T")
                                            {
                                                Session["ManualClassificationListT"] = objList;
                                            }
                                            else if (Type == "G")
                                            {
                                                Session["ManualClassificationListG"] = objList;
                                            }



                                            return RedirectToAction("CreateClassification", new { status = string.Empty, IsFirstLoad = false });
                                        }

                                        Session["ManualClassificationListA"] = objList;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("Type", "Classification Type Already Exist.");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("Type", "Please try again later.");
                                }


                            }
                            else
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Chart Account Number Already Exist.");
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }
                }

            }

            return View(obj);
            #endregion

        }

        public bool CheckNumberRange(int chartAccNo, int startRange, int EndRange)
        {
            //if (Enumerable.Range(startRange, EndRange).Contains(chartAccNo))
            //{
            //    return true;
            //}
            //return false;


            if ((chartAccNo >= startRange) && (chartAccNo <= EndRange))
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        public ActionResult SaveManualClassificationList(String Type)
        {
            var objList = new List<AddClassificationViewModel>();

            //objList = (List<AddClassificationViewModel>)Session["ManualClassificationList"];

            if (Type == "A")
            {
                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListA"];
            }
            else if (Type == "S")
            {
                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListS"];
            }
            else if (Type == "T")
            {
                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListT"];
            }
            else if (Type == "G")
            {
                objList = (List<AddClassificationViewModel>)Session["ManualClassificationListG"];
            }

            try
            {

                using (var db = new KFentities())
                {
                    var classifcationList = new List<Classification>();

                    int CategoryId = 0;

                    foreach (var data in objList)
                    {
                        String chartaccno = "";
                        if (data.Type == "A")
                        {
                            chartaccno = data.NewChartAccountNumber.Substring(0, 4);

                        }
                        else if (data.Type == "G")
                        {
                            chartaccno = data.CategoryValue;
                        }
                        else
                        {
                            chartaccno = data.ChartAccountNumber;
                        }
                        if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 1, 1999))
                        {
                            CategoryId = 1;
                        }
                        else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 5000, 8999))
                        {
                            CategoryId = 2;
                        }
                        else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 4000, 4999))
                        {
                            CategoryId = 3;
                        }
                        else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 2000, 2999))
                        {
                            CategoryId = 4;
                        }
                        else if (this.CheckNumberRange(Convert.ToInt32(chartaccno), 3000, 3999))
                        {
                            CategoryId = 5;
                        }

                        var dbInsert = new Classification();
                        dbInsert.CategoryId = CategoryId;

                        bool IsExistChcek = false;
                        if (Type == "A")
                        {
                            dbInsert.ChartAccountDisplayNumber = data.NewChartAccountNumber;
                            IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == data.NewChartAccountNumber).Any();

                        }


                        else if (Type == "S")
                        {
                            dbInsert.ChartAccountDisplayNumber = data.ChartAccountNumber + "-5000";
                            IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == data.ChartAccountNumber + "-5000").Any();
                        }
                        else if (Type == "T")
                        {
                            dbInsert.ChartAccountDisplayNumber = data.ChartAccountNumber + "-9999";
                            IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == data.ChartAccountNumber + "-9999").Any();
                        }

                        else if (Type == "G")
                        {
                            dbInsert.ChartAccountDisplayNumber = data.CategoryValue + "-" + data.ChartAccountNumber;
                            IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == data.CategoryValue + "-" + data.ChartAccountNumber).Any();
                        }

                        //else
                        //{
                        //    dbInsert.ChartAccountNumber = data.ChartAccountNumber + "-0000";
                        //    IsExistChcek = db.Classification_new.Where(s => s.ChartAccountNumber == data.ChartAccountNumber + "-0000").Any();
                        //}



                        //dbInsert.ClassificationType = data.ClassificationTypeName == null ? "" : data.ClassificationTypeName;
                        dbInsert.ClassificationType = data.Name;
                        dbInsert.CreatedDate = DateTime.Now;
                        dbInsert.ModifiedDate = DateTime.Now;
                        dbInsert.Desc = data.ClassificationDesc;
                        dbInsert.Type = data.Type;
                        dbInsert.IsDeleted = false;
                        dbInsert.IsIncorporated = false;
                        dbInsert.IsPartnerShip = false;
                        dbInsert.IsSole = false;
                        dbInsert.UserId = data.UserID;
                        dbInsert.SubIndustryId = 0;
                        dbInsert.IndustryId = 0;
                        dbInsert.Name = data.Name;
                        dbInsert.CategoryValue = data.CategoryValue;
                        if (data.Type != "A")
                        {
                            dbInsert.RangeofAct = data.NewChartAccountNumber;
                        }


                        if (IsExistChcek == false)
                        {
                            classifcationList.Add(dbInsert);
                        }
                    }
                    db.Classifications.AddRange(classifcationList);
                    db.SaveChanges();



                    if (Type == "A")
                    {
                        Session["ManualClassificationListA"] = null;
                    }
                    else if (Type == "S")
                    {
                        Session["ManualClassificationListS"] = null;
                    }
                    else if (Type == "T")
                    {
                        Session["ManualClassificationListT"] = null;
                    }
                    else if (Type == "G")
                    {
                        Session["ManualClassificationListG"] = null;
                    }

                    // db.Classifications_new.AddRange(classifcationList);
                    //if (classifcationList.Where(s => s.Type.Equals("H")).Any())
                    //{
                    //    if (classifcationList.Where(s => s.Type.Equals("T")).Any())
                    //    {
                    //        var dbInsertRange = new tblReportRange();
                    //        dbInsertRange.StartRange = Convert.ToInt32(classifcationList.Where(s => s.Type.Equals("H")).Select(d => d.ChartAccountNumber).FirstOrDefault());
                    //        dbInsertRange.Endrange = Convert.ToInt32(classifcationList.Where(s => s.Type.Equals("T")).Select(d => d.ChartAccountNumber).FirstOrDefault());
                    //        if (classifcationList.Where(s => s.Type.Equals("S")).Any())
                    //        {
                    //            dbInsertRange.SubHeading = Convert.ToInt32(classifcationList.Where(s => s.Type.Equals("S")).Select(d => d.ChartAccountNumber).FirstOrDefault());
                    //            dbInsertRange.SubHeadingName = classifcationList.Where(s => s.Type.Equals("S")).Select(d => d.ClassificationType).FirstOrDefault();
                    //        }
                    //        dbInsertRange.DateCreated = DateTime.Now;
                    //        dbInsertRange.HeadingName = classifcationList.Where(s => s.Type.Equals("H")).Select(d => d.ClassificationType).FirstOrDefault();

                    //        dbInsertRange.TotalName = classifcationList.Where(s => s.Type.Equals("T")).Select(d => d.ClassificationType).FirstOrDefault();
                    //        dbInsertRange.UserId = Convert.ToInt32(classifcationList.Where(s => s.Type.Equals("H")).Select(d => d.UserId).FirstOrDefault());
                    //        dbInsertRange.CategoryId = Convert.ToInt32(classifcationList.Where(s => s.Type.Equals("H")).Select(d => d.CategoryId).FirstOrDefault());
                    //        db.tblReportRanges.Add(dbInsertRange);
                    //    }
                    //}
                    //db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {


                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            return RedirectToAction("CreateClassification", new { status = "true", IsFirstLoad = true });
        }

        public ActionResult RemoveClassificationEnrty(int id, string Type)
        {
            var objList = new List<AddClassificationViewModel>();

            if (Type == "A")
            {
                if (Session["ManualClassificationListA"] != null)
                {
                    objList = (List<AddClassificationViewModel>)Session["ManualClassificationListA"];
                    Session["ManualClassificationListA"] = null;
                }
            }
            else if (Type == "S")
            {
                if (Session["ManualClassificationListS"] != null)
                {
                    objList = (List<AddClassificationViewModel>)Session["ManualClassificationListS"];
                    Session["ManualClassificationListS"] = null;
                }
            }
            else if (Type == "T")
            {
                if (Session["ManualClassificationListT"] != null)
                {
                    objList = (List<AddClassificationViewModel>)Session["ManualClassificationListT"];
                    Session["ManualClassificationListT"] = null;
                }
            }
            else if (Type == "G")
            {
                if (Session["ManualClassificationListG"] != null)
                {
                    objList = (List<AddClassificationViewModel>)Session["ManualClassificationListG"];
                    Session["ManualClassificationListG"] = null;
                }
            }

            objList.Remove(objList.Where(s => s.Id == id).FirstOrDefault());
            if (objList.Count > 0)
            {


                if (Type == "A")
                {
                    Session["ManualClassificationListA"] = objList;
                }
                else if (Type == "S")
                {
                    Session["ManualClassificationListS"] = objList;
                }
                else if (Type == "T")
                {
                    Session["ManualClassificationListT"] = objList;
                }
                else if (Type == "G")
                {
                    Session["ManualClassificationListG"] = objList;
                }

            }
            else
            {
                if (Type == "A")
                {
                    Session["ManualClassificationListA"] = null;
                }
                else if (Type == "S")
                {
                    Session["ManualClassificationListS"] = null;
                }
                else if (Type == "T")
                {
                    Session["ManualClassificationListT"] = null;
                }
                else if (Type == "G")
                {
                    Session["ManualClassificationListG"] = null;
                }

            }
            return RedirectToAction("CreateClassification", new { status = string.Empty });
        }


        #endregion

        #region Update Classification
        public ActionResult UpdateClassification(string args)
        {
            ViewBag.args = args;
            return View();
        }
        #endregion

        #region Manage Classification
        //  [KippinAuthorize(Roles = "User with an accountant,Accountant,Sub accountant")]
        public ActionResult MannualClassification(string status)
        {
            if (!string.IsNullOrEmpty(status))
                ViewBag.status = status;

            return View();
        }

        public ActionResult DeleteClassification(int classificationId)
        {
            using (var db = new KFentities())
            {
                string status = string.Empty;
                if (db.BankExpenses.Where(f => f.ClassificationId == classificationId && f.IsDeleted == false).Any())
                    status = string.Empty;
                else
                {
                    var deleteClassification = db.Classifications.Where(g => g.Id == classificationId).FirstOrDefault();
                    if (deleteClassification.Type == "S")
                    {
                        if (db.BankExpenses.Where(d => d.Classification.ReportingSubTotalDisplayNumber == deleteClassification.ChartAccountDisplayNumber).Any())
                            status = string.Empty;
                        else
                        {
                            db.MasterClassifications.Remove(db.MasterClassifications.Where(r => r.SubTotalDisplayNumber == deleteClassification.ChartAccountDisplayNumber).FirstOrDefault());
                            var list = db.Classifications.Where(d => d.ReportingSubTotalDisplayNumber == deleteClassification.ChartAccountDisplayNumber).ToList();
                            db.Classifications.RemoveRange(list);
                        }
                    }
                    db.Classifications.Remove(db.Classifications.Where(g => g.Id == classificationId).FirstOrDefault());
                    db.SaveChanges();
                    status = "Success";
                }

                return RedirectToAction("MannualClassification", new { status = status });
            }

        }

        public ActionResult UpdateClassification_TypeA(int id, String Type)
        {
            using (var db = new KFentities())
            {
                long SelectedActiveUser = 0;
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                    }

                }
                var data = db.Classifications.Where(s => s.Id == id).FirstOrDefault();
                var obj = new AddClassificationViewModel();
                obj.ClassificationDesc = data.Desc;
                obj.NewChartAccountNumber = data.ChartAccountDisplayNumber;
                obj.CategoryId = data.CategoryId;
                obj.CategoryValue = data.CategoryValue;
                obj.Type = data.ClassificationType;
                obj.Name = data.Name;
                String newselectvalue = data.ChartAccountDisplayNumber.Substring(0, 4);

                String oldchartaccno;
                if (data.ChartAccountDisplayNumber.Contains('-'))
                {
                    oldchartaccno = data.ChartAccountDisplayNumber.Substring(5, 4);
                }
                else
                {
                    oldchartaccno = newselectvalue;
                }



                obj.ChartAccountNumber = oldchartaccno;
                if (Type == "A")
                {
                    obj.CategoryList = ClsDropDownList.SPopulateCategory();
                    obj.CategoryId = Convert.ToInt32(newselectvalue);
                }
                else if (Type == "S")
                {
                    obj.CategoryList = ClsDropDownList.T1PopulateCategory(Convert.ToInt32(SelectedActiveUser));
                    obj.CategoryId = Convert.ToInt32(newselectvalue);
                }
                else if (Type == "T")
                {
                    obj.CategoryList = ClsDropDownList.T2PopulateCategory(Convert.ToInt32(SelectedActiveUser));
                    obj.CategoryId = Convert.ToInt32(newselectvalue);

                }
                else if (Type == "G")
                {
                    obj.CategoryList = ClsDropDownList.ZPopulateCategory();
                    obj.CategoryId = Convert.ToInt32(newselectvalue);
                }

                obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
                return View(obj);

            }
        }

        public ActionResult DeleteClassification_TypeA(int id)
        {
            using (var db = new KFentities())
            {
                var data = db.Classifications.Where(s => s.Id == id).FirstOrDefault();

                db.Entry(data).State = System.Data.Entity.EntityState.Deleted;

                db.SaveChanges();
                return RedirectToAction("MannualClassification");
                //return View();

            }
        }

        public ActionResult UpdateClassification_TypeS(int id, String Type)
        {
            using (var db = new KFentities())
            {
                long SelectedActiveUser = 0;
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                    }

                }
                var data = db.Classifications.Where(s => s.Id == id).FirstOrDefault();
                var obj = new AddClassificationViewModel();
                obj.ClassificationDesc = data.Desc;
                //obj.ChartAccountNumber = data.ChartAccountNumber;
                obj.ChartAccountNumber = data.ChartAccountDisplayNumber.Substring(0, 4);
                obj.NewChartAccountNumber = data.RangeofAct;
                obj.CategoryId = data.CategoryId;
                obj.CategoryValue = data.CategoryValue;
                obj.CategoryId2 = Convert.ToInt32(data.RangeofAct.Substring(5, 4)) + 1;
                obj.Name = data.Name;
                obj.Type = data.ClassificationType;
                obj.CategoryList2 = ClsDropDownList.T1PopulateCategory(Convert.ToInt32(SelectedActiveUser));
                obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
                return View(obj);

            }
        }

        public ActionResult UpdateClassification_TypeT(int id, String Type)
        {
            using (var db = new KFentities())
            {
                var data = db.Classifications.Where(s => s.Id == id).FirstOrDefault();
                var obj = new AddClassificationViewModel();
                obj.ClassificationDesc = data.Desc;
                obj.ChartAccountNumber = data.ChartAccountDisplayNumber.Substring(0, 4);
                obj.NewChartAccountNumber = data.RangeofAct;
                obj.CategoryId = data.CategoryId;
                obj.CategoryId3 = Convert.ToInt32(data.RangeofAct.Substring(5, 4)) + 1;
                obj.Name = data.Name;
                obj.Type = data.ClassificationType;
                obj.CategoryValue = data.CategoryValue;
                String newselectvalue = data.RangeofAct.Substring(0, 4);



                obj.CategoryList3 = ClsDropDownList.ZPopulateCategory();
                obj.CategoryId = Convert.ToInt32(newselectvalue);
                obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
                return View(obj);

            }
        }

        public ActionResult UpdateClassification_TypeG(int id, String Type)
        {
            using (var db = new KFentities())
            {
                long SelectedActiveUser = 0;
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                    }

                }
                var data = db.Classifications.Where(s => s.Id == id).FirstOrDefault();
                var obj = new AddClassificationViewModel();
                obj.ClassificationDesc = data.Desc;
                // obj.ChartAccountNumber = data.ChartAccountNumber;
                //obj.ChartAccountNumber = data.ChartAccountNumber;
                obj.CategoryValue = data.CategoryValue;
                obj.ChartAccountNumber = data.ChartAccountDisplayNumber.Substring(0, 4);
                obj.NewChartAccountNumber = data.RangeofAct;
                obj.CategoryId = data.CategoryId;
                obj.CategoryId4 = Convert.ToInt32(data.RangeofAct.Substring(5, 4)) + 1;
                obj.Name = data.Name;
                obj.Type = data.ClassificationType;
                obj.CategoryList4 = ClsDropDownList.T2PopulateCategory(Convert.ToInt32(SelectedActiveUser));
                obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
                return View(obj);

            }
        }

        [HttpPost]
        public ActionResult UpdateClassification(AddClassificationViewModel obj, string Type)
        {
            //if (obj.CategoryId > 0 && !string.IsNullOrEmpty(obj.ClassificationDesc) && !string.IsNullOrEmpty(obj.Type) && obj.Id > 0)
            if (!string.IsNullOrEmpty(obj.ClassificationDesc))
            {
                int CategoryId = 0;
                bool ChkClassificationRange = false;
                #region reserver classification
                List<string> objReservedClassification = new List<string>();

                objReservedClassification.Add("1050-1060");
                objReservedClassification.Add("1050-1061");
                objReservedClassification.Add("1050-1062");
                objReservedClassification.Add("1050-1063");
                objReservedClassification.Add("1050-1064");
                objReservedClassification.Add("1050-1073");

                objReservedClassification.Add("1100-1080");
                objReservedClassification.Add("1100-1081");
                objReservedClassification.Add("1100-1082");
                objReservedClassification.Add("1100-1083");
                objReservedClassification.Add("1100-1087");
                objReservedClassification.Add("1100-1088");
                objReservedClassification.Add("1100-1089");


                #endregion
                if (objReservedClassification.Where(s => s == obj.ChartAccountNumber).Any())
                {
                    ModelState.AddModelError("ChartAccountNumber", "Chart Account Number already taken");
                }
                else
                {
                    if (this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 1, 1999))
                    {
                        CategoryId = 1;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 5000, 8999))
                    {
                        CategoryId = 2;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 4000, 4999))
                    {
                        CategoryId = 3;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 2000, 2999))
                    {
                        CategoryId = 4;
                    }
                    else if (this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 3000, 3999))
                    {
                        CategoryId = 5;
                    }



                    switch (CategoryId)
                    {
                        case 1:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 1, 1999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 1000-1999");
                            }
                            break;
                        case 2:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 5000, 8999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 5000-8999");
                            }
                            break;
                        case 3:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 4000, 4999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 4000-4999");
                            }
                            break;
                        case 4:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 2000, 2999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 2000-2999");
                            }
                            break;
                        case 5:
                            ChkClassificationRange = this.CheckNumberRange(Convert.ToInt32(obj.ChartAccountNumber), 3000, 3999);
                            if (ChkClassificationRange == false)
                            {
                                ModelState.AddModelError("ChartAccountNumber", "Selected category chart account number range is 3000-3999");
                            }
                            break;
                    }
                }

                if (ChkClassificationRange == true)
                {

                    using (var db = new KFentities())
                    {
                        try
                        {
                            var data = db.Classifications.Where(s => s.Id == obj.Id).FirstOrDefault();
                            if (data != null)
                            {
                                if (!db.Classifications.Where(s => s.ChartAccountDisplayNumber == obj.ChartAccountNumber && s.Id != obj.Id).Any())
                                {
                                    data.Desc = obj.ClassificationDesc;

                                    data.ClassificationType = obj.ClassificationDesc;
                                    data.Type = data.Type;
                                    //data.CategoryId = obj.CategoryId;
                                    data.CategoryId = CategoryId;
                                    if (obj.Type == "A")
                                    {
                                        data.ChartAccountDisplayNumber = obj.NewChartAccountNumber;
                                    }

                                    else if (obj.Type == "S")
                                    {
                                        data.ChartAccountDisplayNumber = obj.ChartAccountNumber + "-5000";
                                        data.RangeofAct = obj.NewChartAccountNumber;
                                    }
                                    else if (obj.Type == "T")
                                    {
                                        data.ChartAccountDisplayNumber = obj.ChartAccountNumber + "-9999";
                                        data.RangeofAct = obj.NewChartAccountNumber;
                                    }

                                    else
                                    {
                                        data.ChartAccountDisplayNumber = obj.ChartAccountNumber + "-0000";
                                        data.RangeofAct = obj.NewChartAccountNumber;
                                    }

                                    data.Name = obj.Name;

                                    bool IsExistChcek = false;


                                    if (Type == "A")
                                    {
                                        IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == obj.NewChartAccountNumber && s.Id != obj.Id).Any();
                                    }
                                    else if (Type == "S")
                                    {
                                        IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == obj.ChartAccountNumber + "-5000" && s.Id != obj.Id).Any();
                                    }
                                    else if (Type == "T")
                                    {
                                        IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == obj.ChartAccountNumber + "-9999" && s.Id != obj.Id).Any();
                                    }
                                    else
                                    {
                                        IsExistChcek = db.Classifications.Where(s => s.ChartAccountDisplayNumber == obj.ChartAccountNumber + "-0000" && s.Id != obj.Id).Any();
                                    }


                                    // IsExistChcek = db.Classification_new.Where(s => s.ChartAccountNumber == obj.NewChartAccountNumber).Any();
                                    if (IsExistChcek == false)
                                    {
                                        db.SaveChanges();
                                    }
                                    else
                                    { }

                                    return RedirectToAction("MannualClassification");
                                }
                                else
                                {
                                    ModelState.AddModelError("ChartAccountNumber", "Chart Account Number Already Exist.");
                                }

                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

            }
            obj.CategoryList = ClsDropDownList.PopulateCategory();
            obj.ClassificationTypeList = ClsDropDownList.PopulateClassificationType();
            return View(obj);


        }

        #endregion

        #endregion

        #region Edit Statement Section
        public ActionResult EncodeQueryString(int statementId, int? year)
        {
            Dictionary<String, String> encryptedQueryString = new Dictionary<String, String>();
            encryptedQueryString = new Dictionary<String, String> { { "StatementID", statementId.ToString() }, { "Year", year > 0 ? year.ToString() : string.Empty } };
            String encryptQueryString = Security.ToEncryptedQueryString(encryptedQueryString);
            return RedirectToAction("StatementReconcilation", new { args = encryptQueryString });
            // return encryptQueryString;
        }

        public ActionResult StatementReconcilation(string args)
        {
            ViewBag.args = args;
            return View();
        }
        #endregion

        #region Delete Statement
        [KippinAuthorize]
        public ActionResult DeleteBankExpenseById(int BankExpenseId)
        {
            ReconcillationBankExpenseDto objView = new ReconcillationBankExpenseDto();
            using (var repository = new ReconcillationRepository())
            {
                try
                {
                    var data = repository.DeleteBankExpenseById(BankExpenseId);
                    if (data > 0)
                    {
                        return RedirectToAction("Reconciliation", new { uid = data });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Reconciliation");
        }
        #endregion

        #region Cloud
        [HttpGet]
        public ActionResult KippinCloudHome()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult KippinCloudYearListing(int id)
        {
            List<FolderViewModel> ObjList = new List<FolderViewModel>();
            try
            {
                var Chkfolder = Server.MapPath("~/CameraUploadImages/" + id);
                ViewBag.UserId = id;
                if (Directory.Exists(Chkfolder))
                {
                    var data = Directory.GetDirectories(Chkfolder);
                    string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                    string[] folders = Directory.GetDirectories(Chkfolder);
                    foreach (var image in folders)
                    {
                        FolderViewModel imageDto = new FolderViewModel();
                        FileInfo f = new FileInfo(image);
                        imageDto.Name = f.Name;
                        imageDto.UserId = id;
                        imageDto.Year = f.Name;
                        ObjList.Add(imageDto);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return PartialView(ObjList);
        }

        [HttpGet]
        public ActionResult KippinCloudMonthListing(int userId, string year)
        {
            List<FolderViewModel> ObjList = new List<FolderViewModel>();
            try
            {
                ViewBag.UserId = userId;
                //here id is year id
                var Chkfolder = Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/");
                if (Directory.Exists(Chkfolder))
                {
                    var data = Directory.GetDirectories(Chkfolder);
                    string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                    string[] folders = Directory.GetDirectories(Chkfolder);
                    foreach (var image in folders)
                    {
                        FolderViewModel imageDto = new FolderViewModel();
                        FileInfo f = new FileInfo(image);
                        imageDto.Name = f.Name;
                        imageDto.Year = year;
                        imageDto.UserId = userId;
                        imageDto.Month = f.Name;
                        int month = Convert.ToInt32(f.Name);
                        int Year = Convert.ToInt32(year);
                        ObjList.Add(imageDto);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return PartialView(ObjList);
        }

        [HttpGet]
        public ActionResult KippinCloudImageListing(int userId, string year, string month)
        {

            Boolean IsLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocalhost"].ToString());
            string FolderPath = string.Empty;
            if (!IsLocal)
            {
                FolderPath = "/Finance/CameraUploadImages/";
            }
            else
            {
                FolderPath = "/CameraUploadImages/";
            }

            ViewBag.UserId = userId;
            ViewBag.year = year;
            List<FolderViewModel> ObjList = new List<FolderViewModel>();
            var Chkfolder = Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/" + month);
            if (Directory.Exists(Chkfolder))
            {
                ViewBag.userid = userId;
                int Year = Convert.ToInt32(year);
                int Month = Convert.ToInt32(month);
                var data = Directory.EnumerateFiles(Chkfolder);

                if (data != null)
                {
                    foreach (var image in data)
                    {
                        FolderViewModel imageDto = new FolderViewModel();
                        imageDto.Path = FolderPath + userId + "/" + year + "/" + month + "/" + Path.GetFileName(image);
                        imageDto.Name = Path.GetFileName(image);
                        imageDto.Year = year;
                        imageDto.UserId = userId;
                        imageDto.Month = month;
                        ObjList.Add(imageDto);
                    }
                }
            }
            return PartialView(ObjList);
        }

        public ActionResult ViewStatementUploadedBills(int userId, int statementId)
        {
            ViewBag.statementId = statementId;

            ViewBag.userId = userId;
            Boolean IsLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocalhost"].ToString());
            string FolderPath = string.Empty;
            if (!IsLocal)
            {
                FolderPath = "/Finance/OcrImages/";
            }
            else
            {
                FolderPath = "~/OcrImages/";
            }
            var Chkfolder = Server.MapPath("~/OcrImages/" + userId + "/" + statementId);
            List<string> BillImageList = new List<string>();
            if (Directory.Exists(Chkfolder))
            {
                //ViewBag.Images = Directory.EnumerateFiles(Server.MapPath("~/OcrImages/" + data.UserId + "/" + id + "/"))
                //         .Select(fn => "~/OcrImages/" + data.UserId + "/" + id + "/" + Path.GetFileName(fn));
                var imgList = Directory.EnumerateFiles(Server.MapPath("~/OcrImages/" + userId + "/" + statementId + "/"))
                        .Select(fn => FolderPath + userId + "/" + statementId + "/" + Path.GetFileName(fn));
                foreach (var item in imgList)
                {
                    BillImageList.Add(item);
                }
            }
            return PartialView(BillImageList);
        }
        #endregion

        #region Upload bill from device
        public JsonResult UploadBill(FormCollection form)
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-d--HH-mm-ss-");
            var file = Request.Files[0];
            try
            {
                //Boolean IsLocal = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLocalhost"].ToString());
                //string FolderPath = string.Empty;
                //if (!IsLocal)
                //{
                //    FolderPath = "/Finance/OcrImages/";
                //}
                //else
                //{
                //    FolderPath = "~/OcrImages/";
                //}
                //SaveAs(HttpContext.Current.Server.MapPath("~" + /User_data/ArtistImage/+ "\\" + datetime+SongName));
                //if (imageDto.Image != null && imageDto.ImageName != null && imageDto.ExpenseId > 0 && imageDto.BankId > 0 && imageDto.UserId > 0)
                if (Convert.ToInt32(form["StatementId"]) > 0 && Convert.ToInt32(form["BankId"]) > 0 && Convert.ToInt32(form["UserId"]) > 0)
                {
                    using (var expenseRepository = new ExpenseRepository())
                    {
                        string folder = Server.MapPath("~/OcrImages/" + Convert.ToInt32(form["UserId"]) + "/" + Convert.ToInt32(form["StatementId"]) + "/");
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        var filePath = Server.MapPath("~" + "/OcrImages/" + Convert.ToInt32(form["UserId"]) + "/" + Convert.ToInt32(form["StatementId"]) + "/" + dateTime + file.FileName.Trim().Replace(" ", ""));
                        file.SaveAs(filePath);
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region delete image from the cloud
        [HttpPost]
        public JsonResult DeleteImageFromCloud(FormCollection form)
        {

            //int userId = Convert.ToInt32(form["FolderUserId"]);
            //int StatementId = Convert.ToInt32(form["StatementId"]);
            //int YearId = Convert.ToInt32(form["FolderYearId"]);
            string ImagePath = Convert.ToString(form["FilePath"]);
            //   ImagePath = ImagePath.Replace("/Finance/", "/");
            var path = Server.MapPath("~") + ImagePath;
            path = path.Replace("\\/", "\\");
            path = path.Replace("/", "\\");
            System.IO.File.Delete(path);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Upload Image From Cloud
        public JsonResult UploadImageFromCloud(FormCollection form)
        {
            using (var expenseRepository = new ExpenseRepository())
            {
                if (Convert.ToInt32(form["StatementId"]) > 0 && Convert.ToInt32(form["BankId"]) > 0 && Convert.ToInt32(form["UserId"]) > 0)
                {
                    string folder = Server.MapPath("~/OcrImages/" + Convert.ToInt32(form["UserId"]) + "/" + Convert.ToInt32(form["StatementId"]) + "/");
                    string sourceFile = Convert.ToString(form["FilePath"]);
                    sourceFile = sourceFile.Replace(@"/\", @"\");
                    sourceFile = sourceFile.Replace(@"/", @"\");
                    string destFile = System.IO.Path.Combine(folder, Path.GetFileName(sourceFile));
                    // sourceFile = Server.MapPath(sourceFile);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    //   var rawData = sourceFile.Split(new[] { "\\" }, StringSplitOptions.None);

                    var aa = sourceFile.Split(new[] { "CameraUploadImages" }, StringSplitOptions.None);
                    var bb = aa[1].Split(new[] { "\\" }, StringSplitOptions.None);

                    sourceFile = Server.MapPath("~") + sourceFile;
                    sourceFile = sourceFile.Replace(@"\\", @"\");
                    System.IO.File.Copy(sourceFile, destFile, true);
                    CloudImagesRecordDto obj = new CloudImagesRecordDto();
                    obj.ImageName = Path.GetFileName(sourceFile);
                    obj.DateCreated = DateTime.Now;
                    obj.IsAssociated = true;
                    obj.StatementId = Convert.ToInt32(form["StatementId"]);
                    obj.UserId = Convert.ToInt32(form["UserId"]);
                    obj.Month = Convert.ToInt32(bb[3]);
                    obj.Year = Convert.ToInt32(bb[2]);
                    var addImageToCloudDbEntry = expenseRepository.AddImageToCloud(obj);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Reports

        #region Closing Fiscal Year

        public ActionResult SubmitFiscalYearData()
        {
            try
            {
                using (var db = new KFentities())
                {
                    var userData = UserData.GetCurrentUserData();
                    if (userData.RoleId == 4 || userData.RoleId == 1)
                    {
                        Int32 SelectedActiveUser = 0;
                        if (Request.Cookies["SelectedActiveUser"] != null)
                        {
                            if (Request.Cookies["SelectedActiveUser"].Value != null)
                            {
                                Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                            }
                        }
                        if (SelectedActiveUser > 0)
                        {
                            Int32 userID = SelectedActiveUser;
                            var seletedUserData = UserData.GetUserData(userID);
                            string srtDate = seletedUserData.TaxStartMonthId + "/" + seletedUserData.TaxationStartDay + "/" + seletedUserData.TaxStartYear;
                            DateTime startDate = Convert.ToDateTime(srtDate);
                            DateTime EndDate = startDate.AddYears(1);

                            var pendingStatementCheck = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false && s.StatusId != 4 && s.IsVirtualEntry != true && s.Date >= startDate && s.Date <= EndDate).ToList();
                            if (pendingStatementCheck.Count > 0)
                            {
                                TempData["FiscalYear"] = "Please reconcile all items within statement reconciliation section prior to closing fiscal year.";
                            }
                            else
                            {
                                var bankExpenseData = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false).ToList();

                                bankExpenseData = bankExpenseData.Where(d => d.Date >= startDate && d.Date <= EndDate).ToList();
                                bankExpenseData.ForEach(q => q.StatusId = 4);//4 statusid for locked
                                #region Insert entry into fiscal year table
                                var dbInsert = new FiscalYearPosting();
                                if (userData.RoleId == 4)
                                {
                                    dbInsert.AccountantId = userData.AccountantId;
                                    dbInsert.SubAccountantId = userData.Id;
                                }
                                else
                                {
                                    dbInsert.AccountantId = userData.Id;
                                }

                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsPosted = true;

                                dbInsert.TaxStartYear = startDate.AddHours(12);
                                dbInsert.TaxEndYear = EndDate.AddHours(12);
                                dbInsert.UserId = seletedUserData.Id;
                                db.FiscalYearPostings.Add(dbInsert);
                                #endregion

                                #region Update fiscal year
                                var updatefiscalYears = db.UserRegistrations.Where(s => s.Id == seletedUserData.Id).FirstOrDefault();
                                updatefiscalYears.TaxEndYear = seletedUserData.TaxEndYear + 1;
                                updatefiscalYears.TaxStartYear = seletedUserData.TaxStartYear + 1;
                                updatefiscalYears.ReconciliationType = null;
                                #endregion

                                #region Update retained earning and insert log in to retained earning log
                                decimal totalCurrentYearEarning = 0;
                                using (var repo = new ReportRepository())
                                {
                                    totalCurrentYearEarning = UserCurrentYearEarning.GetCurrentYearEarning(seletedUserData.Id, startDate, EndDate);// repo.GetCurrentYearEarning(seletedUserData.Id, null, null);
                                    #region As Retained earning is calculated from past entries in bank expense so no need for maintaing log in separate table as per the understanding on 19 jan 2017
                                    //repo.GetReatinedEarning(startDate,EndDate, seletedUserData.Id);
                                    //var reatinedEarningData = db.RetainedEarnings.Where(s => s.UserId == seletedUserData.Id).FirstOrDefault();
                                    //if (reatinedEarningData != null)
                                    //{
                                    //    reatinedEarningData.ReatinedYearEarning = Decimal.Add(Convert.ToDecimal(reatinedEarningData.ReatinedYearEarning), totalCurrentYearEarning);
                                    //}
                                    //else
                                    //{
                                    //    var dbinsert = new RetainedEarning();
                                    //    dbinsert.UserId = seletedUserData.Id;
                                    //    if (userData.RoleId == 4)
                                    //    {
                                    //        dbInsert.AccountantId = userData.AccountantId;
                                    //        dbInsert.SubAccountantId = userData.Id;
                                    //    }
                                    //    else
                                    //    {
                                    //        dbInsert.AccountantId = userData.Id;
                                    //    }

                                    //    dbinsert.DateCreated = DateTime.Now;
                                    //    dbinsert.ReatinedYearEarning = Convert.ToDecimal(totalCurrentYearEarning);
                                    //    db.RetainedEarnings.Add(dbinsert);
                                    //}
                                    #endregion

                                }
                                #region Insert reatined earning log
                                var dbinsertLog = new RetainedEarningLog();
                                dbinsertLog.UserId = seletedUserData.Id;
                                if (userData.RoleId == 4)
                                {
                                    dbinsertLog.AccountantId = userData.AccountantId;
                                    dbinsertLog.SubAccountantId = userData.Id;
                                }
                                else
                                {
                                    dbinsertLog.AccountantId = userData.Id;
                                }
                                dbinsertLog.DateCreated = DateTime.Now;
                                dbinsertLog.ReatinedYearEarning = Convert.ToDecimal(totalCurrentYearEarning);
                                db.RetainedEarningLogs.Add(dbinsertLog);
                                #endregion

                                #endregion

                                // #region Opening Balance
                                //Step 1: Get All classification Id's
                                List<OpeningBalanceCalculationDto> openingBalanceRecordList = new List<OpeningBalanceCalculationDto>();

                                //Step 2 : Get Data from general ledger
                                using (var repo = new ReportRepository())
                                {
                                    var reportData = repo.TrialBalanceList(seletedUserData.Id, startDate, EndDate, null, string.Empty).Where(d => d.DisplayChartAccountNumber != "3550-3561" && d.ChartAccountNumber < 39990000).ToList();
                                    if (reportData.Count > 0)
                                    {
                                        foreach (var item in reportData)
                                        {
                                            OpeningBalanceCalculationDto objData = new OpeningBalanceCalculationDto();
                                            if (item.ClassificationID == 1060)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1060" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1061)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1061" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1062)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1062" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1063)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1063" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1064)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1064" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1073)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1050-1073" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1080)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1080" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1081)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1081" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1082)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1082" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1083)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1083" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1087)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1087" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1088)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1088" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else if (item.ClassificationID == 1089)
                                            { objData.ClassificationId = db.Classifications.Where(f => f.ChartAccountDisplayNumber == "1100-1089" && f.UserId == seletedUserData.Id).Select(g => g.Id).FirstOrDefault(); }
                                            else
                                                objData.ClassificationId = item.ClassificationID;

                                            decimal OpeningBalance = Decimal.Subtract(item.ClassificationCreditTotal, item.ClassificationDebitTotal);
                                            //if (item.ClassificationID == db.Classifications.Where(j => j.ChartAccountDisplayNumber == "3550-3561").Select(j => j.Id).FirstOrDefault())
                                            //{
                                            //    OpeningBalance = Decimal.Add(OpeningBalance, totalCurrentYearEarning);
                                            //}
                                            if (OpeningBalance > 0)
                                            {
                                                objData.isDebit = false;
                                            }
                                            else
                                            {
                                                objData.isDebit = true;
                                            }
                                            objData.OpeningBalance = Math.Abs(OpeningBalance);
                                            if (objData.OpeningBalance > 0)
                                            {
                                                openingBalanceRecordList.Add(objData);
                                            }
                                        }
                                    }
                                }

                                #region Retained earning
                                var classificationDetails = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "3550-3561").FirstOrDefault();
                                OpeningBalanceCalculationDto objRetainedEarningData = new OpeningBalanceCalculationDto();
                                objRetainedEarningData.ClassificationId = classificationDetails.Id;
                                if (totalCurrentYearEarning > 0)
                                {
                                    objRetainedEarningData.isDebit = false;
                                }
                                else
                                {
                                    objRetainedEarningData.isDebit = true;
                                }
                                objRetainedEarningData.OpeningBalance = Math.Abs(totalCurrentYearEarning);
                                if (objRetainedEarningData.OpeningBalance > 0)
                                {
                                    openingBalanceRecordList.Add(objRetainedEarningData);
                                }
                                #endregion

                                var classificationListids = openingBalanceRecordList.Select(f => f.ClassificationId).ToList();
                                string aa = string.Join(",", classificationListids);
                                //Step 3 Insert opening balance into Bank expense with new entry into next fiscal year with opening balance bit
                                var dbInsertBankExpenseList = new List<BankExpense>();
                                if (openingBalanceRecordList.Count > 0)
                                {
                                    int JVID = 1;
                                    foreach (var row in openingBalanceRecordList)
                                    {
                                        var dbInsertBankExpense = new BankExpense();
                                        if (row.isDebit)
                                        {
                                            dbInsertBankExpense.Credit = 0;
                                            dbInsertBankExpense.Debit = row.OpeningBalance;
                                        }
                                        if (!row.isDebit)
                                        {
                                            dbInsertBankExpense.Credit = row.OpeningBalance;
                                            dbInsertBankExpense.Debit = 0;
                                        }
                                        dbInsertBankExpense.ClassificationId = row.ClassificationId;
                                        dbInsertBankExpense.CategoryId = db.Classifications.Where(d => d.Id == row.ClassificationId).Select(g => g.CategoryId).FirstOrDefault();
                                        int year = Convert.ToInt32(seletedUserData.TaxStartYear + 1);
                                        DateTime firstDay = new DateTime(year, 1, 1).AddHours(1);
                                        dbInsertBankExpense.Date = firstDay;
                                        dbInsertBankExpense.Comments = "Opening Balance";
                                        dbInsertBankExpense.UploadType = "M";
                                        dbInsertBankExpense.BankId = 6;
                                        dbInsertBankExpense.StatusId = 4;
                                        dbInsertBankExpense.IsDeleted = false;
                                        dbInsertBankExpense.UserId = seletedUserData.Id;
                                        dbInsertBankExpense.Total = row.OpeningBalance;
                                        dbInsertBankExpense.AccountType = "MJV";
                                        dbInsertBankExpense.Description = "Opening Balance";
                                        dbInsertBankExpense.IsYodlee = false;
                                        dbInsertBankExpense.IsVirtualEntry = false;
                                        dbInsertBankExpense.AccountName = "MJV";//!string.IsNullOrEmpty(row.AccountName) ? row.AccountName : string.Empty;
                                        dbInsertBankExpense.AccountClassificationId = 1030;
                                        dbInsertBankExpense.ActualTotal = row.OpeningBalance;
                                        dbInsertBankExpense.TotalTax = 0;
                                        dbInsertBankExpense.StatusId = 4;
                                        dbInsertBankExpense.GSTtax = 0;
                                        dbInsertBankExpense.QSTtax = 0;
                                        dbInsertBankExpense.HSTtax = 0;
                                        dbInsertBankExpense.PSTtax = 0;
                                        dbInsertBankExpense.GSTPercentage = 0;
                                        dbInsertBankExpense.QSTPercentage = 0;
                                        dbInsertBankExpense.HSTPercentage = 0;
                                        dbInsertBankExpense.PSTPercentage = 0;
                                        dbInsertBankExpense.ActualPercentage = 0;
                                        dbInsertBankExpense.JVID = JVID;
                                        db.BankExpenses.Add(dbInsertBankExpense);
                                        //JVID = JVID + 1;
                                    }
                                }
                                db.SaveChanges();
                                TempData["FiscalYear"] = "Fiscal Year " + startDate.Date.ToShortDateString() + " successfully closed.";
                            }
                        }
                    }
                    else if (userData.RoleId == 2)
                    {
                        //User as an accountant
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Report");
        }

        public ActionResult SubmitFiscalYearData1()
        {
            try
            {
                using (var db = new KFentities())
                {
                    var userData = UserData.GetCurrentUserData();
                    if (userData.RoleId == 4 || userData.RoleId == 1)
                    {
                        Int32 SelectedActiveUser = 0;
                        if (Request.Cookies["SelectedActiveUser"] != null)
                        {
                            if (Request.Cookies["SelectedActiveUser"].Value != null)
                            {
                                Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                            }
                        }
                        if (SelectedActiveUser > 0)
                        {
                            Int32 userID = SelectedActiveUser;
                            var seletedUserData = UserData.GetUserData(userID);
                            string srtDate = seletedUserData.TaxStartMonthId + "/" + seletedUserData.TaxationStartDay + "/" + seletedUserData.TaxStartYear;
                            DateTime startDate = Convert.ToDateTime(srtDate);
                            DateTime EndDate = startDate.AddYears(1);

                            var pendingStatementCheck = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false && s.StatusId != 4 && s.IsVirtualEntry != true && s.Date >= startDate && s.Date <= EndDate).ToList();
                            if (pendingStatementCheck.Count > 0)
                            {
                                TempData["FiscalYear"] = "Please reconcile all items within statement reconciliation section prior to closing fiscal year.";
                            }
                            else
                            {
                                var bankExpenseData = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false).ToList();

                                bankExpenseData = bankExpenseData.Where(d => d.Date >= startDate && d.Date <= EndDate).ToList();
                                bankExpenseData.ForEach(q => q.StatusId = 4);//4 statusid for locked
                                #region Insert entry into fiscal year table
                                var dbInsert = new FiscalYearPosting();
                                if (userData.RoleId == 4)
                                {
                                    dbInsert.AccountantId = userData.AccountantId;
                                    dbInsert.SubAccountantId = userData.Id;
                                }
                                else
                                {
                                    dbInsert.AccountantId = userData.Id;
                                }

                                dbInsert.CreatedDate = DateTime.Now;
                                dbInsert.IsPosted = true;

                                dbInsert.TaxStartYear = startDate.AddHours(12);
                                dbInsert.TaxEndYear = EndDate.AddHours(12);
                                dbInsert.UserId = seletedUserData.Id;
                                db.FiscalYearPostings.Add(dbInsert);
                                #endregion

                                #region Update fiscal year
                                var updatefiscalYears = db.UserRegistrations.Where(s => s.Id == seletedUserData.Id).FirstOrDefault();
                                updatefiscalYears.TaxEndYear = seletedUserData.TaxEndYear + 1;
                                updatefiscalYears.TaxStartYear = seletedUserData.TaxStartYear + 1;
                                updatefiscalYears.ReconciliationType = null;
                                #endregion

                                #region Update retained earning and insert log in to retained earning log
                                decimal totalCurrentYearEarning = 0;
                                using (var repo = new ReportRepository())
                                {
                                    totalCurrentYearEarning = UserCurrentYearEarning.GetCurrentYearEarning(seletedUserData.Id, startDate, EndDate);// repo.GetCurrentYearEarning(seletedUserData.Id, null, null);
                                    #region As Retained earning is calculated from past entries in bank expense so no need for maintaing log in separate table as per the understanding on 19 jan 2017
                                    //repo.GetReatinedEarning(startDate,EndDate, seletedUserData.Id);
                                    //var reatinedEarningData = db.RetainedEarnings.Where(s => s.UserId == seletedUserData.Id).FirstOrDefault();
                                    //if (reatinedEarningData != null)
                                    //{
                                    //    reatinedEarningData.ReatinedYearEarning = Decimal.Add(Convert.ToDecimal(reatinedEarningData.ReatinedYearEarning), totalCurrentYearEarning);
                                    //}
                                    //else
                                    //{
                                    //    var dbinsert = new RetainedEarning();
                                    //    dbinsert.UserId = seletedUserData.Id;
                                    //    if (userData.RoleId == 4)
                                    //    {
                                    //        dbInsert.AccountantId = userData.AccountantId;
                                    //        dbInsert.SubAccountantId = userData.Id;
                                    //    }
                                    //    else
                                    //    {
                                    //        dbInsert.AccountantId = userData.Id;
                                    //    }

                                    //    dbinsert.DateCreated = DateTime.Now;
                                    //    dbinsert.ReatinedYearEarning = Convert.ToDecimal(totalCurrentYearEarning);
                                    //    db.RetainedEarnings.Add(dbinsert);
                                    //}
                                    #endregion

                                }
                                #region Insert reatined earning log
                                var dbinsertLog = new RetainedEarningLog();
                                dbinsertLog.UserId = seletedUserData.Id;
                                if (userData.RoleId == 4)
                                {
                                    dbinsertLog.AccountantId = userData.AccountantId;
                                    dbinsertLog.SubAccountantId = userData.Id;
                                }
                                else
                                {
                                    dbinsertLog.AccountantId = userData.Id;
                                }
                                dbinsertLog.DateCreated = DateTime.Now;
                                dbinsertLog.ReatinedYearEarning = Convert.ToDecimal(totalCurrentYearEarning);
                                db.RetainedEarningLogs.Add(dbinsertLog);
                                #endregion

                                #endregion

                                // #region Opening Balance
                                //Step 1: Get All classification Id's
                                List<OpeningBalanceCalculationDto> openingBalanceRecordList = new List<OpeningBalanceCalculationDto>();
                                #region Tax part
                                #region GST Paid
                                var openingBalanceRecordGST = new OpeningBalanceCalculationDto();
                                openingBalanceRecordGST.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2376").Select(f => f.Id).FirstOrDefault();
                                openingBalanceRecordGST.OpeningBalance = 0;
                                #endregion
                                #region QST Paid
                                var openingBalanceRecordQST = new OpeningBalanceCalculationDto();
                                openingBalanceRecordQST.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2377").Select(f => f.Id).FirstOrDefault(); ;
                                openingBalanceRecordQST.OpeningBalance = 0;
                                #endregion
                                #region HST Paid
                                var openingBalanceRecordHST = new OpeningBalanceCalculationDto();
                                openingBalanceRecordHST.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2375").Select(f => f.Id).FirstOrDefault();
                                openingBalanceRecordHST.OpeningBalance = 0;
                                #endregion
                                #region PST Paid
                                var openingBalanceRecordPST = new OpeningBalanceCalculationDto();
                                openingBalanceRecordPST.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2378").Select(f => f.Id).FirstOrDefault(); ;
                                openingBalanceRecordPST.OpeningBalance = 0;
                                #endregion

                                #region GST Charge
                                var openingBalanceRecordGSTCharge = new OpeningBalanceCalculationDto();
                                openingBalanceRecordGSTCharge.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2371").Select(f => f.Id).FirstOrDefault();
                                openingBalanceRecordGSTCharge.OpeningBalance = 0;
                                #endregion
                                #region QST Charge
                                var openingBalanceRecordQSTCharge = new OpeningBalanceCalculationDto();
                                openingBalanceRecordQSTCharge.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2372").Select(f => f.Id).FirstOrDefault(); ;
                                openingBalanceRecordQSTCharge.OpeningBalance = 0;
                                #endregion
                                #region HST Charge
                                var openingBalanceRecordHSTCharge = new OpeningBalanceCalculationDto();
                                openingBalanceRecordHSTCharge.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2370").Select(f => f.Id).FirstOrDefault();
                                openingBalanceRecordHSTCharge.OpeningBalance = 0;
                                #endregion
                                #region PST Change
                                var openingBalanceRecordPSTCharge = new OpeningBalanceCalculationDto();
                                openingBalanceRecordPSTCharge.ClassificationId = db.Classifications.Where(s => s.ChartAccountDisplayNumber == "2300-2373").Select(f => f.Id).FirstOrDefault(); ;
                                openingBalanceRecordPSTCharge.OpeningBalance = 0;
                                #endregion
                                #endregion


                                #region Account Name Binding
                                var objAssetAccNameData = db.BankExpenses.Where(i => i.IsDeleted == false && i.AccountClassificationId != 1030 && i.ClassificationId != 1 && i.UserId == SelectedActiveUser && i.CategoryId != 6).ToList();
                                objAssetAccNameData = objAssetAccNameData.Where(i => i.Date >= startDate && i.Date < EndDate).ToList();
                                #region User own classification data
                                var userClassificationList = db.Classifications.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false).ToList();
                                foreach (var items in userClassificationList)
                                {
                                    if (items.ChartAccountDisplayNumber == "1050-1060" || items.ChartAccountDisplayNumber == "1050-1061" || items.ChartAccountDisplayNumber == "1050-1062" || items.ChartAccountDisplayNumber == "1050-1063")
                                    {
                                        var bankData = db.BankExpenses.Where(s => s.ClassificationId == items.Id && s.IsDeleted == false && s.Date >= startDate && s.Date <= EndDate).ToList();
                                        int accClassId = Convert.ToInt32(items.ChartAccountDisplayNumber.Substring(5, 4));
                                        bankData.ForEach(s => s.AccountClassificationId = accClassId);
                                        foreach (var rowItem in bankData)
                                        {
                                            if (!objAssetAccNameData.Where(d => d.Id == rowItem.Id).Any())
                                            {
                                                objAssetAccNameData.Add(rowItem);
                                            }
                                        }
                                    }
                                }
                                var AssetAccNameData = objAssetAccNameData.OrderBy(s => s.AccountClassificationId).GroupBy(a => a.AccountClassificationId).ToList();
                                List<BalanceChildModel> objAssetList = new List<BalanceChildModel>();
                                foreach (var raw in AssetAccNameData)
                                {
                                    foreach (var rawData in raw.ToList())
                                    {
                                        var assetAccNameRecord = new BalanceChildModel();
                                        assetAccNameRecord.ClassificationChartAccountNumber = Convert.ToInt32(raw.Key);
                                        string bankname = db.Banks.Where(a => a.Id == rawData.BankId).Select(s => s.BankName).FirstOrDefault();
                                        assetAccNameRecord.accountClassificationId = rawData.AccountClassificationId;
                                        switch (rawData.AccountClassificationId)
                                        {
                                            case 1060:
                                                assetAccNameRecord.AccountName = "Bank Account 1 - " + bankname;
                                                assetAccNameRecord.ClassificationID = db.Classifications.Where(s => s.UserId == SelectedActiveUser).Select(d => d.Id).FirstOrDefault();
                                                break;
                                            case 1061:
                                                assetAccNameRecord.AccountName = "Bank Account 2 - " + bankname;
                                                assetAccNameRecord.ClassificationID = db.Classifications.Where(s => s.Id == rawData.ClassificationId).Select(d => d.Id).Skip(1).FirstOrDefault();
                                                break;
                                            case 1062:
                                                assetAccNameRecord.AccountName = "Bank Account 3 - " + bankname;
                                                assetAccNameRecord.ClassificationID = db.Classifications.Where(s => s.Id == rawData.ClassificationId).Select(d => d.Id).Skip(2).FirstOrDefault();
                                                break;
                                            case 1063:
                                                assetAccNameRecord.AccountName = "Bank Account 4 - " + bankname;
                                                assetAccNameRecord.ClassificationID = db.Classifications.Where(s => s.Id == rawData.ClassificationId).Select(d => d.Id).Skip(3).FirstOrDefault();
                                                break;
                                            default:
                                                assetAccNameRecord.AccountName = "Credit Card Account";
                                                assetAccNameRecord.ClassificationID = db.Classifications.Where(s => s.Id == rawData.ClassificationId).Select(d => d.Id).Skip(0).FirstOrDefault();
                                                break;

                                        }
                                        assetAccNameRecord.AccountName = rawData.AccountName;
                                        var classificationDetail = db.Classifications.Where(s => s.Id == rawData.ClassificationId).FirstOrDefault();
                                        assetAccNameRecord.ClassificationName = classificationDetail.ClassificationType;
                                        assetAccNameRecord.ClassificationType = classificationDetail.Type;

                                        if (rawData.Credit == null)
                                            rawData.Credit = 0;
                                        else if (rawData.Debit == null)
                                            rawData.Debit = 0;
                                        if (rawData.AccountType == "MJV")
                                        {


                                            if (rawData.Credit > rawData.Debit)
                                            {
                                                assetAccNameRecord.Debit = 0;
                                                assetAccNameRecord.Credit = decimal.Round(Convert.ToDecimal(rawData.Credit), 2, MidpointRounding.AwayFromZero);
                                            }
                                            else if (rawData.Debit > rawData.Credit)
                                            {
                                                assetAccNameRecord.Credit = 0;
                                                assetAccNameRecord.Debit = decimal.Round(Convert.ToDecimal(rawData.Debit), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        else
                                        {
                                            if (rawData.Credit > rawData.Debit)
                                            {
                                                assetAccNameRecord.Credit = 0;
                                                assetAccNameRecord.Debit = decimal.Round(Convert.ToDecimal(rawData.Credit), 2, MidpointRounding.AwayFromZero);
                                            }
                                            else if (rawData.Debit > rawData.Credit)
                                            {
                                                assetAccNameRecord.Debit = 0;
                                                assetAccNameRecord.Credit = decimal.Round(Convert.ToDecimal(rawData.Debit), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        objAssetList.Add(assetAccNameRecord);
                                    }


                                }
                                if (objAssetList.Count > 0)
                                {
                                    var accNameList = objAssetList.OrderBy(s => s.ClassificationId).GroupBy(d => d.ClassificationChartAccountNumber).ToList();
                                    foreach (var accountData in accNameList)
                                    {
                                        var a = accountData.ToList();
                                        decimal CreditSum = Convert.ToDecimal(a.Select(w => w.Credit).Sum());
                                        decimal DebitSum = Convert.ToDecimal(a.Select(w => w.Debit).Sum());
                                        var openingAccountBalanceRecord = new OpeningBalanceCalculationDto();
                                        openingAccountBalanceRecord.AccountName = a[0].AccountName;
                                        openingAccountBalanceRecord.ClassificationId = a[0].ClassificationID;
                                        openingAccountBalanceRecord.AccountClassificationId = a[0].accountClassificationId;
                                        openingAccountBalanceRecord.OpeningBalance = Decimal.Subtract(CreditSum, DebitSum);
                                        if (openingAccountBalanceRecord.OpeningBalance > 0)
                                            openingAccountBalanceRecord.isDebit = false;
                                        else
                                            openingAccountBalanceRecord.isDebit = true;

                                        openingAccountBalanceRecord.OpeningBalance = Math.Abs(Decimal.Subtract(CreditSum, DebitSum));
                                        if (openingAccountBalanceRecord.OpeningBalance > 0)
                                        {
                                            openingBalanceRecordList.Add(openingAccountBalanceRecord);
                                        }
                                    }
                                }
                                // var openingAccountBalanceRecord = new OpeningBalanceCalculationDto();
                                #endregion
                                #endregion


                                #region Calculate Tax Charge Part
                                var bankStatementTaxChargeData = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false && s.ClassificationId != 1 && s.IsVirtualEntry != true && s.Date >= startDate && s.Date <= EndDate && s.CategoryId != 6).OrderBy(d => d.Id).ToList();
                                foreach (var item in bankStatementTaxChargeData)
                                {
                                    if (item.Credit > 0)
                                    {

                                        #region Tax Entry

                                        if (item.GSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordGSTCharge.OpeningBalance = Decimal.Add(openingBalanceRecordGSTCharge.OpeningBalance, Convert.ToDecimal(item.GSTtax));
                                            else
                                                openingBalanceRecordGST.OpeningBalance = Decimal.Add(openingBalanceRecordGST.OpeningBalance, Convert.ToDecimal(item.GSTtax));
                                        }
                                        if (item.QSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordQSTCharge.OpeningBalance = Decimal.Add(openingBalanceRecordQSTCharge.OpeningBalance, Convert.ToDecimal(item.QSTtax));
                                            else
                                                openingBalanceRecordQST.OpeningBalance = Decimal.Add(openingBalanceRecordQST.OpeningBalance, Convert.ToDecimal(item.QSTtax));
                                        }
                                        if (item.HSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordHSTCharge.OpeningBalance = Decimal.Add(openingBalanceRecordHSTCharge.OpeningBalance, Convert.ToDecimal(item.HSTtax));
                                            else
                                                openingBalanceRecordHST.OpeningBalance = Decimal.Add(openingBalanceRecordHST.OpeningBalance, Convert.ToDecimal(item.HSTtax));
                                        }
                                        if (item.PSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordPSTCharge.OpeningBalance = Decimal.Add(openingBalanceRecordPSTCharge.OpeningBalance, Convert.ToDecimal(item.PSTtax));
                                            else
                                                openingBalanceRecordPST.OpeningBalance = Decimal.Add(openingBalanceRecordPST.OpeningBalance, Convert.ToDecimal(item.PSTtax));
                                        }
                                        #endregion
                                    }
                                    else if (item.Debit > 0)
                                    {
                                        #region Tax Entry

                                        if (item.GSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordGSTCharge.OpeningBalance = Decimal.Subtract(openingBalanceRecordGSTCharge.OpeningBalance, Convert.ToDecimal(item.GSTtax));
                                            else
                                                openingBalanceRecordGST.OpeningBalance = Decimal.Subtract(openingBalanceRecordGST.OpeningBalance, Convert.ToDecimal(item.GSTtax));
                                        }
                                        if (item.QSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordQSTCharge.OpeningBalance = Decimal.Subtract(openingBalanceRecordQSTCharge.OpeningBalance, Convert.ToDecimal(item.QSTtax));
                                            else
                                                openingBalanceRecordQST.OpeningBalance = Decimal.Subtract(openingBalanceRecordQST.OpeningBalance, Convert.ToDecimal(item.QSTtax));
                                        }
                                        if (item.HSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordHSTCharge.OpeningBalance = Decimal.Subtract(openingBalanceRecordHSTCharge.OpeningBalance, Convert.ToDecimal(item.HSTtax));
                                            else
                                                openingBalanceRecordHST.OpeningBalance = Decimal.Subtract(openingBalanceRecordHST.OpeningBalance, Convert.ToDecimal(item.HSTtax));
                                        }
                                        if (item.PSTtax > 0)
                                        {
                                            if (item.CategoryId == 3)
                                                openingBalanceRecordPSTCharge.OpeningBalance = Decimal.Subtract(openingBalanceRecordPSTCharge.OpeningBalance, Convert.ToDecimal(item.PSTtax));
                                            else
                                                openingBalanceRecordPST.OpeningBalance = Decimal.Subtract(openingBalanceRecordPST.OpeningBalance, Convert.ToDecimal(item.PSTtax));
                                        }
                                        #endregion
                                    }

                                }
                                #endregion
                                //Step 2: Calculate their opening balance
                                var bankStatementData = db.BankExpenses.Where(s => s.UserId == SelectedActiveUser && s.IsDeleted == false && s.ClassificationId != 1 && s.IsVirtualEntry != true && s.Date >= startDate && s.Date <= EndDate && s.CategoryId != 2 && s.CategoryId != 3 && s.CategoryId != 6).OrderBy(d => d.Id).GroupBy(r => r.ClassificationId).ToList();
                                foreach (var item in bankStatementData)
                                {
                                    var openingBalanceRecord = new OpeningBalanceCalculationDto();
                                    openingBalanceRecord.ClassificationId = item.Key;
                                    var bankStatementLists = item.ToList();
                                    decimal CreditSum = 0;
                                    decimal DebitSum = 0;
                                    foreach (var data in bankStatementLists)
                                    {
                                        if (data.Credit > 0)
                                        {
                                            CreditSum = Decimal.Add(Convert.ToDecimal(data.ActualTotal), CreditSum);
                                        }
                                        else if (data.Debit > 0)
                                        {
                                            DebitSum = Decimal.Add(Convert.ToDecimal(data.ActualTotal), DebitSum);
                                        }
                                    }
                                    openingBalanceRecord.OpeningBalance = Decimal.Subtract(CreditSum, DebitSum);

                                    if (openingBalanceRecord.OpeningBalance > 0)
                                        openingBalanceRecord.isDebit = false;
                                    else
                                        openingBalanceRecord.isDebit = true;

                                    openingBalanceRecord.OpeningBalance = Math.Abs(Decimal.Subtract(CreditSum, DebitSum));
                                    if (openingBalanceRecord.OpeningBalance > 0)
                                    {
                                        openingBalanceRecordList.Add(openingBalanceRecord);
                                    }



                                }
                                #region Tax Part Paid
                                #region GST Paid
                                if (openingBalanceRecordGST.OpeningBalance > 0)
                                    openingBalanceRecordGST.isDebit = false;
                                else
                                    openingBalanceRecordGST.isDebit = true;

                                if (Math.Abs(openingBalanceRecordGST.OpeningBalance) > 0)
                                {
                                    var existChk_GST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordGST.ClassificationId).FirstOrDefault();
                                    if (existChk_GST != null)
                                    {
                                        existChk_GST.OpeningBalance = Decimal.Add(existChk_GST.OpeningBalance, openingBalanceRecordGST.OpeningBalance);
                                        if (existChk_GST.OpeningBalance > 0)
                                        {
                                            existChk_GST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_GST.isDebit = true;
                                        }
                                        existChk_GST.OpeningBalance = Math.Abs(existChk_GST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGST.OpeningBalance = Math.Abs(openingBalanceRecordGST.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordGST);
                                    }
                                }
                                #endregion

                                #region QST Paid

                                if (openingBalanceRecordQST.OpeningBalance > 0)
                                    openingBalanceRecordQST.isDebit = false;
                                else
                                    openingBalanceRecordQST.isDebit = true;

                                if (Math.Abs(openingBalanceRecordQST.OpeningBalance) > 0)
                                {
                                    var existChk_QST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordQST.ClassificationId).FirstOrDefault();
                                    if (existChk_QST != null)
                                    {
                                        existChk_QST.OpeningBalance = Decimal.Add(existChk_QST.OpeningBalance, openingBalanceRecordQST.OpeningBalance);
                                        if (existChk_QST.OpeningBalance > 0)
                                        {
                                            existChk_QST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_QST.isDebit = true;
                                        }
                                        existChk_QST.OpeningBalance = Math.Abs(existChk_QST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordQST.OpeningBalance = Math.Abs(openingBalanceRecordQST.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordQST);
                                    }
                                }

                                #endregion

                                #region HST Paid

                                if (openingBalanceRecordHST.OpeningBalance > 0)
                                    openingBalanceRecordHST.isDebit = false;
                                else
                                    openingBalanceRecordHST.isDebit = true;

                                if (Math.Abs(openingBalanceRecordHST.OpeningBalance) > 0)
                                {
                                    var existChk_HST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordHST.ClassificationId).FirstOrDefault();
                                    if (existChk_HST != null)
                                    {
                                        existChk_HST.OpeningBalance = Decimal.Add(existChk_HST.OpeningBalance, openingBalanceRecordHST.OpeningBalance);
                                        if (existChk_HST.OpeningBalance > 0)
                                        {
                                            existChk_HST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_HST.isDebit = true;
                                        }
                                        existChk_HST.OpeningBalance = Math.Abs(existChk_HST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGST.OpeningBalance = Math.Abs(openingBalanceRecordHST.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordHST);
                                    }
                                }

                                #endregion

                                #region PST Paid
                                if (openingBalanceRecordPST.OpeningBalance > 0)
                                    openingBalanceRecordPST.isDebit = false;
                                else
                                    openingBalanceRecordPST.isDebit = true;

                                if (Math.Abs(openingBalanceRecordPST.OpeningBalance) > 0)
                                {
                                    var existChk_PST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordPST.ClassificationId).FirstOrDefault();
                                    if (existChk_PST != null)
                                    {
                                        existChk_PST.OpeningBalance = Decimal.Add(existChk_PST.OpeningBalance, openingBalanceRecordPST.OpeningBalance);
                                        if (existChk_PST.OpeningBalance > 0)
                                        {
                                            existChk_PST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_PST.isDebit = true;
                                        }
                                        existChk_PST.OpeningBalance = Math.Abs(existChk_PST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGST.OpeningBalance = Math.Abs(openingBalanceRecordPST.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordPST);
                                    }
                                }
                                #endregion

                                #endregion

                                #region Tax Part Charge
                                #region GST Charge
                                if (openingBalanceRecordGSTCharge.OpeningBalance > 0)
                                    openingBalanceRecordGSTCharge.isDebit = false;
                                else
                                    openingBalanceRecordGSTCharge.isDebit = true;

                                if (Math.Abs(openingBalanceRecordGSTCharge.OpeningBalance) > 0)
                                {
                                    var existChk_GST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordGSTCharge.ClassificationId).FirstOrDefault();
                                    if (existChk_GST != null)
                                    {
                                        existChk_GST.OpeningBalance = Decimal.Add(existChk_GST.OpeningBalance, openingBalanceRecordGSTCharge.OpeningBalance);
                                        if (existChk_GST.OpeningBalance > 0)
                                        {
                                            existChk_GST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_GST.isDebit = true;
                                        }
                                        existChk_GST.OpeningBalance = Math.Abs(existChk_GST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGSTCharge.OpeningBalance = Math.Abs(openingBalanceRecordGSTCharge.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordGSTCharge);
                                    }

                                }
                                #endregion

                                #region QST Charge

                                if (openingBalanceRecordQSTCharge.OpeningBalance > 0)
                                    openingBalanceRecordQSTCharge.isDebit = false;
                                else
                                    openingBalanceRecordQSTCharge.isDebit = true;

                                if (Math.Abs(openingBalanceRecordQSTCharge.OpeningBalance) > 0)
                                {
                                    var existChk_QST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordQSTCharge.ClassificationId).FirstOrDefault();
                                    if (existChk_QST != null)
                                    {
                                        existChk_QST.OpeningBalance = Decimal.Add(existChk_QST.OpeningBalance, openingBalanceRecordQSTCharge.OpeningBalance);
                                        if (existChk_QST.OpeningBalance > 0)
                                        {
                                            existChk_QST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_QST.isDebit = true;
                                        }
                                        existChk_QST.OpeningBalance = Math.Abs(existChk_QST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordQSTCharge.OpeningBalance = Math.Abs(openingBalanceRecordQSTCharge.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordQSTCharge);
                                    }
                                }

                                #endregion

                                #region HST Charge

                                if (openingBalanceRecordHSTCharge.OpeningBalance > 0)
                                    openingBalanceRecordHSTCharge.isDebit = false;
                                else
                                    openingBalanceRecordHSTCharge.isDebit = true;

                                if (Math.Abs(openingBalanceRecordHSTCharge.OpeningBalance) > 0)
                                {
                                    var existChk_HST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordHSTCharge.ClassificationId).FirstOrDefault();
                                    if (existChk_HST != null)
                                    {
                                        existChk_HST.OpeningBalance = Decimal.Add(existChk_HST.OpeningBalance, openingBalanceRecordHSTCharge.OpeningBalance);
                                        if (existChk_HST.OpeningBalance > 0)
                                        {
                                            existChk_HST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_HST.isDebit = true;
                                        }
                                        existChk_HST.OpeningBalance = Math.Abs(existChk_HST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGSTCharge.OpeningBalance = Math.Abs(openingBalanceRecordHSTCharge.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordHSTCharge);
                                    }
                                }

                                #endregion

                                #region PST Charge
                                if (openingBalanceRecordPSTCharge.OpeningBalance > 0)
                                    openingBalanceRecordPSTCharge.isDebit = false;
                                else
                                    openingBalanceRecordPSTCharge.isDebit = true;

                                if (Math.Abs(openingBalanceRecordPSTCharge.OpeningBalance) > 0)
                                {
                                    var existChk_PST = openingBalanceRecordList.Where(d => d.ClassificationId == openingBalanceRecordPSTCharge.ClassificationId).FirstOrDefault();
                                    if (existChk_PST != null)
                                    {
                                        existChk_PST.OpeningBalance = Decimal.Add(existChk_PST.OpeningBalance, openingBalanceRecordPSTCharge.OpeningBalance);
                                        if (existChk_PST.OpeningBalance > 0)
                                        {
                                            existChk_PST.isDebit = false;
                                        }
                                        else
                                        {
                                            existChk_PST.isDebit = true;
                                        }
                                        existChk_PST.OpeningBalance = Math.Abs(existChk_PST.OpeningBalance);
                                    }
                                    else
                                    {
                                        openingBalanceRecordGSTCharge.OpeningBalance = Math.Abs(openingBalanceRecordPSTCharge.OpeningBalance);
                                        openingBalanceRecordList.Add(openingBalanceRecordPSTCharge);
                                    }
                                }
                                #endregion

                                #endregion
                                //Step 3 Insert opening balance into Bank expense with new entry into next fiscal year with opening balance bit
                                var dbInsertBankExpenseList = new List<BankExpense>();
                                if (openingBalanceRecordList.Count > 0)
                                {
                                    int JVID = 1;
                                    foreach (var row in openingBalanceRecordList)
                                    {
                                        var dbInsertBankExpense = new BankExpense();
                                        dbInsertBankExpense.Credit = row.isDebit == true ? 0 : row.OpeningBalance;
                                        dbInsertBankExpense.Debit = row.isDebit == true ? row.OpeningBalance : 0;
                                        dbInsertBankExpense.ClassificationId = row.ClassificationId;
                                        dbInsertBankExpense.CategoryId = db.Classifications.Where(d => d.Id == row.ClassificationId).Select(g => g.CategoryId).FirstOrDefault();
                                        int year = Convert.ToInt32(seletedUserData.TaxStartYear + 1);
                                        DateTime firstDay = new DateTime(year, 1, 1);
                                        dbInsertBankExpense.Date = firstDay;
                                        dbInsertBankExpense.Comments = "Opening Balance";
                                        dbInsertBankExpense.UploadType = "M";
                                        dbInsertBankExpense.BankId = 6;
                                        dbInsertBankExpense.StatusId = 4;
                                        dbInsertBankExpense.IsDeleted = false;
                                        dbInsertBankExpense.UserId = seletedUserData.Id;
                                        dbInsertBankExpense.Total = row.OpeningBalance;
                                        dbInsertBankExpense.AccountType = "MJV";
                                        dbInsertBankExpense.Description = "Opening Balance";
                                        dbInsertBankExpense.IsYodlee = false;
                                        dbInsertBankExpense.IsVirtualEntry = false;
                                        dbInsertBankExpense.AccountName = !string.IsNullOrEmpty(row.AccountName) ? row.AccountName : string.Empty;
                                        dbInsertBankExpense.AccountClassificationId = 1030;
                                        dbInsertBankExpense.ActualTotal = row.OpeningBalance;
                                        dbInsertBankExpense.TotalTax = 0;
                                        dbInsertBankExpense.StatusId = 4;
                                        dbInsertBankExpense.GSTtax = 0;
                                        dbInsertBankExpense.QSTtax = 0;
                                        dbInsertBankExpense.HSTtax = 0;
                                        dbInsertBankExpense.PSTtax = 0;
                                        dbInsertBankExpense.GSTPercentage = 0;
                                        dbInsertBankExpense.QSTPercentage = 0;
                                        dbInsertBankExpense.HSTPercentage = 0;
                                        dbInsertBankExpense.PSTPercentage = 0;
                                        dbInsertBankExpense.ActualPercentage = 0;
                                        dbInsertBankExpense.JVID = JVID;
                                        db.BankExpenses.Add(dbInsertBankExpense);
                                        JVID = JVID + 1;
                                    }
                                }
                                db.SaveChanges();
                                TempData["FiscalYear"] = "Fiscal Year " + startDate.Date.ToShortDateString() + " successfully closed.";
                            }
                        }
                    }
                    else if (userData.RoleId == 2)
                    {
                        //User as an accountant
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Report");
        }

        #endregion

        #region Dashboard
        public ActionResult Report(string error)
        {
            var currentUserData = UserData.GetCurrentUserData();
            ReportUserForAccountant obj = new ReportUserForAccountant();
            if (currentUserData.RoleId == 1 || currentUserData.RoleId == 4)
            {
                if (currentUserData.RoleId == 4)
                {
                    obj.UserList = ClsDropDownList.PopulateUser(Convert.ToInt32(currentUserData.AccountantId), true);
                }
                else
                {
                    obj.UserList = ClsDropDownList.PopulateUser(currentUserData.Id, true);
                }

                obj.RoleId = Convert.ToInt32(currentUserData.RoleId);
                int SelectedActiveUser = 0;
                if (Request.Cookies["SelectedActiveUser"] != null)
                {
                    if (Request.Cookies["SelectedActiveUser"].Value != null)
                    {
                        int.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                    }

                }
                obj.UserId = Convert.ToInt32(SelectedActiveUser);
            }
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Error = "Please Select User.";
            }
            return View(obj);
        }
        public ActionResult ReportFilter(string Command, ReportUserForAccountant obj)
        {
            if (Command == "GeneralLedger")
            {
                return RedirectToAction("TrialBalance");
            }
            if (Command == "IncomeStatement")
            {
                return RedirectToAction("IncomeStatement");
            }
            if (Command == "BalanceSheet")
            {
                return RedirectToAction("BalanceSheet");
            }
            if (Command == "AssetReport")
            {
                return RedirectToAction("Assets", new { id = obj.UserId });
            }
            if (Command == "LiablityReport")
            {
                return RedirectToAction("Liability", new { id = obj.UserId });
            }
            if (Command == "ExpenseReport")
            {
                return RedirectToAction("Expense", new { id = obj.UserId });
            }
            if (Command == "RevenueReport")
            {
                return RedirectToAction("Revenue", new { id = obj.UserId });
            }
            if (Command == "DividentReport")
            {
                return RedirectToAction("Equity", new { id = obj.UserId });
            }
            return RedirectToAction("Report");

        }
        #endregion

        #region Trial Balance
        public ActionResult TrialBalance(string startDate, string endDate, int? JVID, string ChartAccountNumber)
        {

            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<TrialBalanceDto> trialBalanceList = new List<TrialBalanceDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                if (JVID > 0)
                {
                    ViewBag.JVID = JVID;
                }
                if (!string.IsNullOrEmpty(ChartAccountNumber))
                {
                    ViewBag.AccountNumber = ChartAccountNumber;
                }
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion

                try
                {
                    #region Get Trial balance Data From repo
                    trialBalanceList = repo.TrialBalanceList(UserId, startrange, endrange, JVID, ChartAccountNumber);
                    #endregion
                }
                catch (Exception)
                {
                    //Show Error Message
                }

                TempData["UserId"] = UserId;
                return View(trialBalanceList);
            }
        }

        [KippinAuthorize]
        [HttpPost]
        public ActionResult TrialBalanceFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");
            int JVID = 0;
            if (!string.IsNullOrEmpty(collection.Get("txtjvid")))
            {
                JVID = Convert.ToInt32(collection.Get("txtjvid"));
            }
            string ChartAccountNumber = collection.Get("txtAccNo");

            return RedirectToAction("TrialBalance", new { id = Convert.ToInt32(userId), startDate = String.Format(startdate, "MM/dd/yyyy"), endDate = String.Format(enddate, "MM/dd/yyyy"), JVID = JVID, ChartAccountNumber = ChartAccountNumber });
        }
        #endregion

        #region Income Sheet
        public ActionResult IncomeStatement(string startDate, string endDate, string startSecondDate, string endSecondDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                long SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                int UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = Convert.ToInt32(SelectedActiveUser);
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                IncomeSheetDto objData = new IncomeSheetDto();
                Nullable<System.DateTime> startrange = null;
                Nullable<System.DateTime> endrange = null;
                Nullable<System.DateTime> startSecondrange = null;
                Nullable<System.DateTime> endSecondrange = null;
                #endregion

                try
                {
                    #region Search Filter
                    if (!string.IsNullOrEmpty(startDate))
                    {
                        ViewBag.Startdate = startDate;
                        startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(endDate))
                        {
                            ViewBag.Enddate = endDate;
                            endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            endrange = DateTime.Now;
                        }

                    }
                    if (!string.IsNullOrEmpty(startSecondDate))
                    {
                        startSecondrange = DateTime.ParseExact(startSecondDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);// Convert.ToDateTime(startSecondDate);
                        ViewBag.StartSeconddate = startSecondDate;
                        if (!string.IsNullOrEmpty(endSecondDate))
                        {
                            ViewBag.EndSeconddate = endSecondDate;
                            endSecondrange = DateTime.ParseExact(endSecondDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            endSecondrange = DateTime.Now;
                        }
                    }
                    if (string.IsNullOrEmpty(startSecondDate) && string.IsNullOrEmpty(startDate))
                    {
                        // ViewBag.ReportHeader = "success";
                        if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                        {
                            //User Fiscal year Data
                            string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                            startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                            endrange = startrange.Value.AddYears(1).AddMilliseconds(-1);
                        }
                        else
                        {
                            var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                            string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                            startrange = Convert.ToDateTime(datetime);
                            endrange = startrange.Value.AddYears(1).AddMilliseconds(-1);
                        }
                    }
                    #endregion

                    #region Get Report Data
                    objData = repo.GetIncomeSheetData(UserId, startrange, endrange, startSecondrange, endSecondrange);
                    #endregion

                }
                catch (Exception)
                {
                    objData.objRevenueList = new List<CategoryReportDto>();
                    objData.objExpenseList = new List<CategoryReportDto>();
                    objData.objSecondRevenueList = new List<CategoryReportDto>();
                    objData.objSecondExpenseList = new List<CategoryReportDto>();
                }
                TempData["UserId"] = UserId;
                return View(objData);
            }

        }


        [KippinAuthorize]
        [HttpPost]
        public ActionResult IncomeStatementFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string startSeconddate = collection.Get("txtSecondStartDate");
            string endSeconddate = collection.Get("txtSecondEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("IncomeStatement", new { startDate = String.Format(startdate, "MM/dd/yyyy"), endDate = String.Format(enddate, "MM/dd/yyyy"), startSecondDate = String.Format(startSeconddate, "MM/dd/yyyy"), endSecondDate = String.Format(endSeconddate, "MM/dd/yyyy") });
        }
        #endregion

        #region Balance Sheet
        [KippinAuthorize]
        public ActionResult BalanceSheet(int? id, string startDate, string endDate, string startSecondDate, string endSecondDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                long SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                int UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = Convert.ToInt32(SelectedActiveUser);
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local Variable
                BalanceSheetDto objData = new BalanceSheetDto();
                Nullable<System.DateTime> startrange = null;
                Nullable<System.DateTime> endrange = null;

                Nullable<System.DateTime> startSecondrange = null;
                Nullable<System.DateTime> endSecondrange = null;
                #endregion

                try
                {
                    if (!string.IsNullOrEmpty(startDate))
                    {
                        startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);// Convert.ToDateTime(startDate);
                        ViewBag.Startdate = startDate;
                        if (!string.IsNullOrEmpty(endDate))
                        {
                            ViewBag.Enddate = endDate;
                            endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            endrange = DateTime.Now;
                        }
                    }
                    if (!string.IsNullOrEmpty(startSecondDate))
                    {
                        startSecondrange = DateTime.ParseExact(startSecondDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);// Convert.ToDateTime(startSecondDate);
                        ViewBag.StartSeconddate = startSecondDate;
                        if (!string.IsNullOrEmpty(endSecondDate))
                        {
                            ViewBag.EndSeconddate = endSecondDate;
                            endSecondrange = DateTime.ParseExact(endSecondDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);// Convert.ToDateTime(endSecondDate);
                        }
                        else
                        {
                            endSecondrange = DateTime.Now;
                        }
                    }
                    if (string.IsNullOrEmpty(startSecondDate) && string.IsNullOrEmpty(startDate))
                    {
                        if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                        {
                            //User Fiscal year Data
                            string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                            startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                            endrange = startrange.Value.AddYears(1).AddMilliseconds(-1);
                        }
                        else
                        {
                            var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                            string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                            startrange = Convert.ToDateTime(datetime);
                            endrange = startrange.Value.AddYears(1).AddMilliseconds(-1);
                        }
                    }
                    var userData = UserData.GetCurrentUserData();
                    objData = repo.GetBalanceSheetData(UserId, startrange, endrange, startSecondrange, endSecondrange);
                }
                catch (Exception)
                {
                    objData.objAssetList = new List<CategoryReportDto>();
                    objData.objEquityList = new List<CategoryReportDto>();
                    objData.objLiablityList = new List<CategoryReportDto>();
                    objData.objSecondAssetList = new List<CategoryReportDto>();
                    objData.objSecondEquityList = new List<CategoryReportDto>();
                    objData.objSecondLiablityList = new List<CategoryReportDto>();
                }
                TempData["UserId"] = UserId;
                return View(objData);
            }

        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult BalanceSheetFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string startSeconddate = collection.Get("txtSecondStartDate");
            string endSeconddate = collection.Get("txtSecondEndDate");
            //.ToString("dd/MM/yyyy")
            return RedirectToAction("BalanceSheet", new { startDate = String.Format(startdate, "MM/dd/yyyy"), endDate = String.Format(enddate, "MM/dd/yyyy"), startSecondDate = String.Format(startSeconddate, "MM/dd/yyyy"), endSecondDate = String.Format(endSeconddate, "MM/dd/yyyy") });
        }
        #endregion

        #region Asset report
        public ActionResult Assets(string startDate, string endDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<CategoryReportDto> objData = new List<CategoryReportDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                    ViewBag.ReportHeader = "Daterange";
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion

                try
                {
                    TempData["UserId"] = UserId;
                    objData = repo.GetAssetData(UserId, startrange, endrange);
                }
                catch (Exception)
                { }
                return View(objData);
            }
        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult AssetFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("Assets", new { id = Convert.ToInt32(userId), startDate = startdate, endDate = enddate });
        }
        #endregion

        #region Liablity report
        public ActionResult Liability(string startDate, string endDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<CategoryReportDto> objData = new List<CategoryReportDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                    ViewBag.ReportHeader = "Daterange";
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion

                try
                {
                    TempData["UserId"] = UserId;
                    objData = repo.GetLiabilityData(UserId, startrange, endrange);
                }
                catch (Exception)
                { }
                return View(objData);
            }
        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult LiabilityFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("Liability", new { id = Convert.ToInt32(userId), startDate = startdate, endDate = enddate });
        }
        #endregion

        #region Equity report
        public ActionResult Equity(string startDate, string endDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<CategoryReportDto> objData = new List<CategoryReportDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                    ViewBag.ReportHeader = "Daterange";
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion
                try
                {
                    TempData["UserId"] = UserId;
                    objData = repo.GetEquityData(UserId, startrange, endrange);
                }
                catch (Exception)
                { }
                return View(objData);
            }
        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult DividendFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("Equity", new { id = Convert.ToInt32(userId), startDate = startdate, endDate = enddate });
        }
        #endregion

        #region Revenue
        public ActionResult Revenue(string startDate, string endDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<CategoryReportDto> objData = new List<CategoryReportDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                    ViewBag.ReportHeader = "Daterange";
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion
                try
                {
                    TempData["UserId"] = UserId;
                    objData = repo.GetRevenueData(UserId, startrange, endrange);
                }
                catch (Exception)
                { }
                return View(objData);
            }
        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult RevenueFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("Revenue", new { id = Convert.ToInt32(userId), startDate = startdate, endDate = enddate });
        }
        #endregion

        #region Expense
        public ActionResult Expense(string startDate, string endDate)
        {
            using (var repo = new ReportRepository())
            {
                #region Get User Data
                Int32 SelectedActiveUser = 0;
                var currentLoggedInUserData = UserData.GetCurrentUserData();
                Int32 UserId = 0;
                if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                {
                    //User As An Accountant
                    UserId = currentLoggedInUserData.Id;
                }
                else
                {
                    //Accountant / Sub Accountant

                    if (Request.Cookies["SelectedActiveUser"] != null)
                    {
                        if (Request.Cookies["SelectedActiveUser"].Value != null)
                        {
                            Int32.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                        }
                    }
                    if (SelectedActiveUser > 0)
                        UserId = SelectedActiveUser;
                    else
                        return RedirectToAction("ActiveUser", "Kippin");
                }

                #endregion

                #region Local variable
                List<CategoryReportDto> objData = new List<CategoryReportDto>();
                System.DateTime startrange;
                System.DateTime endrange;
                #endregion

                #region Filter Parameter
                //If user Select the start date
                if (!string.IsNullOrEmpty(startDate))
                {
                    ViewBag.Startdate = startDate;
                    startrange = DateTime.ParseExact(startDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); //Convert.ToDateTime(startDate);
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        ViewBag.Enddate = endDate;
                        endrange = DateTime.ParseExact(endDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);//;
                    }
                    else
                    {
                        endrange = DateTime.Now;
                    }
                    ViewBag.ReportHeader = "Daterange";
                }
                else
                {
                    if (currentLoggedInUserData.RoleId != 1 && currentLoggedInUserData.RoleId != 4)
                    {
                        //User Fiscal year Data
                        string datetime = currentLoggedInUserData.TaxStartMonthId + "/" + currentLoggedInUserData.TaxationStartDay + "/" + currentLoggedInUserData.TaxStartYear;
                        startrange = datetime != "//" ? Convert.ToDateTime(datetime) : DateTime.Now;
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        var selectedUserData = UserData.GetUserData(Convert.ToInt32(SelectedActiveUser));
                        string datetime = selectedUserData.TaxStartMonthId + "/" + selectedUserData.TaxationStartDay + "/" + selectedUserData.TaxStartYear;
                        startrange = Convert.ToDateTime(datetime);
                        endrange = startrange.AddYears(1).AddMilliseconds(-1);
                    }

                }
                #endregion
                try
                {
                    TempData["UserId"] = UserId;
                    objData = repo.GetExpenseData(UserId, startrange, endrange);
                }
                catch (Exception)
                { }
                return View(objData);
            }
        }
        [KippinAuthorize]
        [HttpPost]
        public ActionResult ExpenseFilter(FormCollection collection)
        {
            string startdate = collection.Get("txtStartDate");
            string enddate = collection.Get("txtEndDate");
            string userId = collection.Get("selectedUserId");

            return RedirectToAction("Expense", new { id = Convert.ToInt32(userId), startDate = startdate, endDate = enddate });
        }
        #endregion
        #endregion

        #region KIPPIN CLOUD
        public ActionResult KippinCloud(string Username)
        {
            using (var context = new KFentities())
            {

                KippinStoreViewModel Obj = new KippinStoreViewModel();
                UserRegistration objUser = UserData.GetCurrentUserData();
                ViewBag.UserListError = string.Empty;
                try
                {
                    if (objUser.RoleId == 4 || objUser.RoleId == 1)
                    {
                        long SelectedActiveUser = 0;
                        if (Request.Cookies["SelectedActiveUser"] != null)
                        {
                            if (Request.Cookies["SelectedActiveUser"].Value != null)
                            {
                                long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                            }

                        }
                        if (SelectedActiveUser > 0)
                        {
                            //if (!string.IsNullOrEmpty(Username))
                            //{

                            //    var GetUserList = context.UserRegistrations.Where(i => (i.AccountantId == objUser.Id && i.IsVerified == true && i.IsDeleted == false) && (i.FirstName == Username || i.LastName == Username)).ToList();

                            //    if (GetUserList.Count > 0)
                            //    {
                            //        Obj.ObjUserList = GetUserList.Select(o => new FolderListViewModel()
                            //        {
                            //            UserId = o.Id,
                            //            UserName = o.FirstName + " " + o.LastName + " / " + o.Email,
                            //            IsAssociated = context.tblKippinStoreImages.Where(a => a.UserId == o.Id && a.IsDeleted == false).Any() ? false : true
                            //        }).ToList();
                            //    }
                            //    else
                            //    {
                            //        ViewBag.UserListError = "No records available for you.";
                            //    }
                            //}
                            // else
                            // {
                            var GetUserList = context.UserRegistrations.Where(b => b.Id == SelectedActiveUser).FirstOrDefault();
                            FolderListViewModel ObjFolder = new FolderListViewModel()
                            {
                                UserId = GetUserList.Id,
                                UserName = GetUserList.FirstName + " " + GetUserList.LastName,
                                IsAssociated = context.KippinStoreImages.Where(a => a.UserId == GetUserList.Id && a.IsDeleted == false).Any() ? false : true
                            };

                            List<FolderListViewModel> list = new List<FolderListViewModel>();
                            list.Add(ObjFolder);
                            Obj.ObjUserList = list;
                            //   var GetUserList = context.UserRegistrations.Where(i => i.AccountantId == objUser.Id && i.IsVerified == true && i.IsDeleted == false).ToList();

                            //if (GetUserList.Count > 0)
                            //{
                            //    Obj.ObjUserList = GetUserList.Select(o => new FolderListViewModel()
                            //    {
                            //        UserId = o.Id,
                            //        UserName = o.FirstName + " " + o.LastName + " / " + o.Email,
                            //        IsAssociated = context.tblKippinStoreImages.Where(a => a.UserId == o.Id && a.IsDeleted == false).Any() ? false : true
                            //    }).ToList();
                            //}
                            //else
                            //{
                            //    ViewBag.UserListError = "No records available for you.";
                            //}
                            // }
                        }
                        else
                        {
                            return RedirectToAction("ActiveUser", "Accounting");
                        }

                    }
                    else
                    {
                        //User as an accountant
                        var GetUserList = context.UserRegistrations.Where(b => b.Id == objUser.Id).FirstOrDefault();
                        FolderListViewModel ObjFolder = new FolderListViewModel()
                        {
                            UserId = GetUserList.Id,
                            UserName = GetUserList.FirstName + " " + GetUserList.LastName,
                            IsAssociated = context.KippinStoreImages.Where(a => a.UserId == GetUserList.Id && a.IsDeleted == false).Any() ? false : true
                        };

                        List<FolderListViewModel> list = new List<FolderListViewModel>();
                        list.Add(ObjFolder);
                        Obj.ObjUserList = list;
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                }

                return View(Obj);
            }
        }

        [HttpGet]
        public ActionResult GetUserImages(int userId, string month, string year)
        {
            if (Request.IsAuthenticated == true)
            {

                List<FolderViewModel> ObjList = new List<FolderViewModel>();
                var Chkfolder = Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/" + month);
                if (Directory.Exists(Chkfolder))
                {
                    using (var db = new KFentities())
                    {
                        ViewBag.userid = userId;
                        int Year = Convert.ToInt32(year);
                        int Month = Convert.ToInt32(month);
                        var associatedImagesList = db.CloudImagesRecords.Where(a => a.UserId == userId && a.Year == Year && a.Month == Month).ToList();
                        var data = Directory.EnumerateFiles(Chkfolder);

                        if (data != null)
                        {
                            foreach (var image in data)
                            {
                                FolderViewModel imageDto = new FolderViewModel();
                                imageDto.Path = "/CameraUploadImages/" + userId + "/" + year + "/" + month + "/" + Path.GetFileName(image);
                                imageDto.Name = Path.GetFileName(image);
                                imageDto.Year = year;
                                imageDto.UserId = userId;
                                imageDto.Month = month;
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
                                ObjList.Add(imageDto);
                            }
                        }
                    }


                }

                return View(ObjList);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public ActionResult GetYearList(int id)
        {
            List<FolderViewModel> ObjList = new List<FolderViewModel>();
            using (var db = new KFentities())
            {
                bool IsAssociatedBit = false;
                var imageAssociated = db.KippinStoreImages.Where(a => a.UserId == id && a.IsDeleted == false).Any();
                if (imageAssociated == true)
                {
                    IsAssociatedBit = false;
                }
                else
                {
                    IsAssociatedBit = true;
                }
                try
                {
                    //here id is userid
                    var Chkfolder = Server.MapPath("~/CameraUploadImages/" + id);
                    // var Chkfolder =ServerPath + "CameraUploadImages/" + id;
                    //New code start //

                    //New code end //
                    if (Directory.Exists(Chkfolder))
                    {
                        var data = Directory.GetDirectories(Chkfolder);
                        string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                        string[] folders = Directory.GetDirectories(Chkfolder);
                        foreach (var image in folders)
                        {
                            FolderViewModel imageDto = new FolderViewModel();
                            FileInfo f = new FileInfo(image);
                            imageDto.Name = f.Name;
                            imageDto.UserId = id;
                            imageDto.Year = f.Name;
                            imageDto.IsAssociated = IsAssociatedBit;
                            ObjList.Add(imageDto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.Message;
                }
            }
            return View(ObjList);
        }

        [HttpGet]
        public ActionResult GetMonthList(int userId, string year)
        {
            List<FolderViewModel> ObjList = new List<FolderViewModel>();
            try
            {
                //here id is year id
                var Chkfolder = Server.MapPath("~/CameraUploadImages/" + userId + "/" + year + "/");
                if (Directory.Exists(Chkfolder))
                {
                    using (var db = new KFentities())
                    {
                        var data = Directory.GetDirectories(Chkfolder);
                        string a = Path.GetFileName(Path.GetDirectoryName(Chkfolder));
                        string[] folders = Directory.GetDirectories(Chkfolder);
                        foreach (var image in folders)
                        {
                            FolderViewModel imageDto = new FolderViewModel();
                            FileInfo f = new FileInfo(image);
                            imageDto.Name = f.Name;
                            imageDto.Year = year;
                            imageDto.UserId = userId;
                            imageDto.Month = f.Name;
                            int month = Convert.ToInt32(f.Name);
                            int Year = Convert.ToInt32(year);
                            var imageAssociated = db.KippinStoreImages.Where(x => x.UserId == userId && x.Year == Year && x.Month == month && x.IsDeleted == false).Any();
                            if (imageAssociated == true)
                            {
                                imageDto.IsAssociated = false;
                            }
                            else
                            {
                                imageDto.IsAssociated = true;
                            }
                            ObjList.Add(imageDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return View(ObjList);
        }

        [HttpPost]
        public JsonResult UploadImagesToCloud(FormCollection form)
        {
            using (var expenseRepository = new ReconcillationRepository())
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    int userId = Convert.ToInt32(form["FolderUserId"]);
                    int MonthId = Convert.ToInt32(form["FolderMonthId"]);
                    int YearId = Convert.ToInt32(form["FolderYearId"]);

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Server.MapPath("../CameraUploadImages") + "/" + userId + "/" + YearId + "/" + MonthId + "/";
                    var Folderpath = Path.Combine(path, fileName);
                    if (Path.GetExtension(file.FileName) == ".png" || Path.GetExtension(file.FileName) == ".jpg" || Path.GetExtension(file.FileName) == ".jpeg" || Path.GetExtension(file.FileName) == ".pdf")
                    {
                        file.SaveAs(Folderpath);
                    }
                    if (Path.GetExtension(file.FileName) == ".png" || Path.GetExtension(file.FileName) == ".jpg" || Path.GetExtension(file.FileName) == ".jpeg")
                    {

                        KippinStoreImageDto obj = new KippinStoreImageDto();
                        obj.ImageName = fileName;
                        obj.DateCreated = DateTime.Now;
                        obj.IsAssociated = false;
                        obj.UserId = userId;
                        obj.Month = MonthId;
                        obj.Year = YearId;
                        obj.IsDeleted = false;
                        var addImageToCloudDbEntry = expenseRepository.AddImageToKippinStore(obj);
                    }



                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //DeleteImageFromCloud
        //[HttpPost]
        //public JsonResult DeleteImageFromCloud(FormCollection form)
        //{

        //    int userId = Convert.ToInt32(form["FolderUserId"]);
        //    int MonthId = Convert.ToInt32(form["FolderMonthId"]);
        //    int YearId = Convert.ToInt32(form["FolderYearId"]);
        //    string ImageName = Convert.ToString(form["ImageName"]);

        //    var path = Server.MapPath("~/CameraUploadImages") + "/" + userId + "/" + YearId + "/" + MonthId + "/" + ImageName;

        //    System.IO.File.Delete(path);
        //    using (var db = new KFentities())
        //    {
        //        var oldData = db.KippinStoreImages.Where(a => a.ImageName == ImageName && a.UserId == userId && a.Year == YearId && a.Month == MonthId).ToList();
        //        oldData.ForEach(q => q.IsDeleted = true);
        //        db.SaveChanges();
        //    }


        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}


        public FileResult ImageDownload(string fileName, string filePath)
        {
            if (Request.IsAuthenticated == true)
            {
                if (fileName.Contains(".pdf"))
                {
                    return File(filePath, "pdf", fileName);
                }
                else
                {
                    return File(filePath, "image/jpeg", fileName);
                }

            }
            else
            {
                return null;
            }

        }
        #endregion

        #region UnlinkAccountant
        [KippinAuthorize(Roles = "User with an accountant")]
        [HttpGet]
        public ActionResult UnlinkAccountant(string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                ViewBag.Error = "Error";
            }
            return View();
        }

        [KippinAuthorize(Roles = "User with an accountant")]
        public ActionResult UnlinkWithAccountant()
        {
            var currentUserData = UserData.GetCurrentUserData();
            using (var dbContext = new KFentities())
            {
                using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var userDetails = dbContext.UserRegistrations.Where(i => i.Id == currentUserData.Id).FirstOrDefault();
                        if (userDetails != null)
                        {
                            int OldAccountantId = Convert.ToInt32(userDetails.AccountantId);
                            var OldAccountantData = dbContext.UserRegistrations.Where(i => i.Id == OldAccountantId).FirstOrDefault();
                            userDetails.IsDeleted = true;
                            userDetails.IsUnlink = true;
                            var bankData = dbContext.BankExpenses.Where(a => a.UserId == currentUserData.Id && a.IsDeleted == false).ToList();

                            #region enter data into archieve table
                            var archieveData = new List<ArchiveBankExpense>();
                            foreach (var item in bankData)
                            {
                                ArchiveBankExpense obj = new ArchiveBankExpense();
                                obj.AccountantEmail = OldAccountantData.Email;
                                obj.AccountantId = OldAccountantData.Id;
                                obj.AccountName = item.AccountName;
                                obj.StatementId = item.StatementReferenceNumber == null ? item.Id : item.StatementReferenceNumber;
                                obj.AccountNumber = item.AccountNumber;
                                obj.AccountType = item.AccountType;
                                obj.BillTax = item.TotalTax;
                                obj.BillTotal = item.ActualTotal;
                                obj.GSTtax = item.GSTtax;
                                obj.QSTtax = item.QSTtax;
                                obj.HSTtax = item.HSTtax;
                                obj.PSTtax = item.PSTtax;
                                obj.Category = dbContext.Categories.Where(s => s.Id == item.CategoryId).Select(d => d.CategoryType).FirstOrDefault();
                                obj.CategoryId = item.CategoryId;
                                obj.ClassificationId = item.ClassificationId;
                                obj.BankId = item.BankId;
                                obj.Classification = dbContext.Classifications.Where(s => s.Id == item.ClassificationId).Select(d => d.ClassificationType).FirstOrDefault();
                                obj.Comments = item.Comments;
                                obj.CompanyName = item.UserRegistration.CompanyName;
                                obj.Credit = item.Credit;
                                obj.Date = item.Date;
                                obj.Debit = item.Debit;
                                obj.Description = item.Description;
                                obj.AccountClassificationId = item.AccountClassificationId;
                                obj.FriendlyName = item.AccountName;
                                obj.Email = currentUserData.Email;
                                obj.Purpose = item.Purpose;
                                obj.Status = dbContext.Status.Where(s => s.Id == item.StatusId).Select(d => d.StatusType).FirstOrDefault();
                                obj.Total = item.Total;
                                obj.TransactionType = item.UploadType;
                                obj.Vendor = item.Vendor;
                                obj.Username = currentUserData.Username;
                                obj.UserId = currentUserData.Id;
                                archieveData.Add(obj);
                            }
                            dbContext.ArchiveBankExpenses.AddRange(archieveData);
                            //   dbContext.SaveChanges();
                            #endregion

                            #region Unlink log
                            UnlinkRecordLog dbInsert = new UnlinkRecordLog();
                            dbInsert.AccountantId = OldAccountantId;
                            dbInsert.UserId = currentUserData.Id;
                            dbInsert.IsDeleted = false;
                            dbInsert.CreatedDate = DateTime.Now;
                            dbContext.UnlinkRecordLogs.Add(dbInsert);
                            #endregion

                            #region Delete OldData
                            //   var deleteOcrdata = dbContext.OcrExpenseDetails.RemoveRange(dbContext.OcrExpenseDetails.Where(a => a.UserId == currentUserData.Id).ToList());
                            var deletedata = dbContext.BankExpenses.RemoveRange(dbContext.BankExpenses.Where(a => a.UserId == currentUserData.Id).ToList());
                            #endregion

                            #region Director Loan Data
                            var directorData = dbContext.DirectorAccountLogs.Where(s => s.UserId == currentUserData.Id).ToList();
                            Mapper.CreateMap<DirectorAccountLog, ArchiveDirectorLog>();
                            var directorArchiveLogList = Mapper.Map<List<ArchiveDirectorLog>>(directorData);
                            var directorArchiveLogData = new List<ArchiveDirectorLog>();
                            directorArchiveLogData.AddRange(directorArchiveLogList);
                            dbContext.ArchiveDirectorLogs.AddRange(directorArchiveLogData);
                            dbContext.DirectorAccountLogs.RemoveRange(directorData);
                            #endregion

                            userDetails.AccountantId = null;
                            dbContext.SaveChanges();
                            dbContextTransaction.Commit();

                            SendMailModelDto objMail = new SendMailModelDto();
                            objMail.From = ConfigurationSettings.AppSettings["smtpUserName"];
                            objMail.To = OldAccountantData.Email;
                            objMail.Subject = "Unlink User Status Email";
                            string Message = "<p>Dear " + OldAccountantData.FirstName + " " + OldAccountantData.LastName + ",</p>";
                            Message += "<br/>";
                            Message += "<p>The user " + currentUserData.FirstName + " " + currentUserData.LastName + " has unlinked their account with you. Your client is now unable to see his bookkeeping data.</p>";
                            Message += "<p>You still maintain the ability to view and print your client’s data</p>";
                            Message += "<br/>";
                            Message += "<p>Regards,</p>";
                            Message += "<br/>";
                            Message += "<p>The KIPPIN Team</p>";
                            objMail.MessageBody = Message;
                            Sendmail.SendEmail(objMail);
                            return RedirectToAction("SuccessfullyUnlink");
                        }
                    }
                    catch (Exception)
                    { dbContextTransaction.Rollback(); }
                }
            }

            return RedirectToAction("UnlinkAccountant", new { status = "Error" });
        }


        public ActionResult SuccessfullyUnlink()
        { FormsAuthentication.SignOut(); return View(); }

        [KippinAuthorize(Roles = "Accountant,Sub accountant")]
        public ActionResult NewUserData()
        {
            var currentUserData = UserData.GetCurrentUserData();
            List<ActiveUserListViewModel> ObjList = new List<ActiveUserListViewModel>();
            using (var db = new AccountRepository())
            {
                var userList = db.GetUserListByAccountantId(currentUserData.Id, currentUserData.Email);
                if (userList.Count > 0)
                {
                    ObjList = userList.Select(a => new ActiveUserListViewModel()
                    {
                        Fullname = a.FirstName + " " + a.LastName,
                        Id = a.Id,
                        Email = a.Email,
                        CorporationAddress = a.CorporationAddress,
                        TaxEndYear = Convert.ToString(a.TaxEndYear),
                        TaxStartYear = Convert.ToString(a.TaxStartYear),
                    }).ToList();
                }
            }
            return View(ObjList);
        }

        [KippinAuthorize(Roles = "Accountant,Sub accountant")]
        public ActionResult DownloadData(int id)
        {
            //var IUser = UserData.GetCurrentUserData();
            using (var dbContext = new KFentities())
            {
                var CurrentUser = dbContext.ArchiveBankExpenses.Where(i => i.UserId == id).ToList();
                if (CurrentUser != null)
                {
                    CurrentUser.ForEach(i => i.IsDownload = true);
                    dbContext.SaveChanges();
                }

                var userDetails = dbContext.ArchiveBankExpenses.Where(i => i.UserId == id).ToList();
                if (userDetails.Count > 0)
                {
                    List<OldBankData> lst = new List<OldBankData>();
                    Mapper.CreateMap<ArchiveBankExpense, OldBankData>();
                    lst = Mapper.Map<List<OldBankData>>(userDetails);
                    //lst.ForEach(a => a.Date = a.Date.Value.Date);
                    //lst.ForEach(a => a.Bank = dbContext.Banks.Where(q => q.Id == a.BankId).Select(z => z.BankName).FirstOrDefault());
                    //lst.ForEach(a => a.Username = dbContext.UserRegistrations.Where(q => q.Id == a.UserId).Select(z => z.FirstName + " " + z.LastName).FirstOrDefault());
                    GridView gv = new GridView();
                    gv.DataSource = lst;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=" + userDetails[0].Username + "_" + userDetails[0].Email + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    return File("~/CustomResources/UnlinkAccountant/EmptyData.xlsx", "application/vnd.ms-excel", userDetails[0].Username + "_" + userDetails[0].Email + ".xls");
                }

            }

            return View();
        }
        #endregion

        #region AutomaticUploadProcess
        #region Yodlee Login
        public ActionResult AutomaticLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AutomaticLogin(YodleeAccountLoginViewModel Obj)
        {
            if (ModelState.IsValid)
            {
                var _parameters = new Dictionary<string, object>();

                var cobrandLogin = ConfigurationManager.AppSettings["cobrandLogin"];
                var cobrandPassword = ConfigurationManager.AppSettings["cobrandPassword"];
                var restUrl = ConfigurationManager.AppSettings["restUrl"];
                var contextPath = ConfigurationManager.AppSettings["contextPath"];

                var restClient = new CustomRestClient(restUrl);

                _parameters.Add("cobrandLogin", cobrandLogin);
                _parameters.Add("cobrandPassword", cobrandPassword);

                try
                {
                    var cobrandContext = restClient.Post<CobrandContext>(YodleeAPI.COB_LOGIN, _parameters);
                    _parameters.Clear();
                    if (cobrandContext.cobrandConversationCredentials == null)
                    {
                        ModelState.AddModelError("CustomError", "Invalid Credentials");
                        return View(Obj);
                        // throw new Exception("Invalid Credentials");
                    }
                    _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                    // _parameters.Add("cobSessionToken", "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad");
                    //08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad
                    _parameters.Add("login", Obj.Username);
                    _parameters.Add("password", Obj.Password);

                    var userInfo = restClient.Post<UserInfo>(YodleeAPI.USER_LOGIN, _parameters);
                    Session["panel_login_info"] = userInfo;
                    Session["cobrandToken"] = cobrandContext.cobrandConversationCredentials.sessionToken;
                    //  Session["cobrandToken"] = "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad";
                    Session["userToken"] = userInfo.userContext.conversationCredentials.sessionToken;
                    Session["restUrl"] = restUrl;
                    Session["Logger"] = new APILogger();
                    //Getting fastlink token
                    _parameters.Clear();
                    _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                    _parameters.Add("rsession", userInfo.userContext.conversationCredentials.sessionToken);
                    _parameters.Add("finAppId", "10003600");
                    var userToken = restClient.Post<FastLinkAppToken>(YodleeAPI.USER_FastLink, _parameters);
                    //FastLink Token
                    Session["Token"] = userToken.finappAuthenticationInfos[0].token;
                    if (Session["Token"] == null)
                    {
                        //AutomaticLogin
                        return RedirectToAction("AutomaticLogin", "Accounting");
                    }
                    #region Add Account To Database
                    using (var db = new KFentities())
                    {
                        var UserInformation = UserData.GetCurrentUserData();
                        var dbExist = db.YodleeUserRegistrations.Where(s => s.UName == Obj.Username && s.KippinUserId == UserInformation.Id).FirstOrDefault();
                        if (dbExist == null)
                        {
                            //insert
                            var dbinsert = new YodleeUserRegistration();
                            dbinsert.UName = Obj.Username;
                            dbinsert.UPassword = Obj.Password;
                            dbinsert.CreatedDate = DateTime.Now;
                            dbinsert.CobrandName = cobrandLogin;
                            dbinsert.CobrandPassword = cobrandPassword;
                            dbinsert.IsDeleted = false;
                            dbinsert.KippinUserId = UserInformation.Id;
                            db.YodleeUserRegistrations.Add(dbinsert);
                        }
                        else
                        {
                            dbExist.UPassword = Obj.Password;
                        }
                        db.SaveChanges();
                    }
                    #endregion
                    return RedirectToAction("AccountList", "Accounting", new { newUser = true });
                }
                catch (Exception e)
                {
                    //return Json(e.Message);
                }
            }
            return View(Obj);

        }
        #endregion

        #region Account Listing
        public ActionResult AccountList(bool? newUser)
        {
            #region Check Automatic login
            this.AutoLogin();
            //if (newUser!=true)
            //{
            //    this.AutoLogin();
            //}

            #endregion
            if (Session["panel_login_info"] == null)
            {
                return View();
            }
            var accountsGroup = new Dictionary<string, List<Account>>();

            // 1. Get site accounts
            var apiLogger = Session["Logger"] as ILogger;
            var restClient = new CustomRestClient(ConfigurationManager.AppSettings["restUrl"], ref apiLogger);

            var _parameters = new Dictionary<string, object>();
            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);

            try
            {
                var siteAccounts = restClient.Post<IList<SiteAccountInfo>>(YodleeAPI.GET_SITE_ACCOUNTS, _parameters);

                // 2. For each site account, get item summaries
                foreach (var site_account in siteAccounts)
                {
                    var _params = new Dictionary<string, object>() {
						{ "cobSessionToken", Session["cobrandToken"] },
						{ "userSessionToken", Session["userToken"] },
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
								{ "cobSessionToken", Session["cobrandToken"] },
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

                                if (!accountsGroup.Keys.Contains<string>(contentServiceInfo.containerInfo.containerName))
                                    accountsGroup.Add(contentServiceInfo.containerInfo.containerName, new List<Account>(item_summary.itemData.accounts));
                                else
                                    accountsGroup[contentServiceInfo.containerInfo.containerName].AddRange(item_summary.itemData.accounts);

                                //if (contentServiceInfo.containerInfo.containerName == "bank")
                                //{
                                //    Act = (List<Account>)item_summary.itemData.accounts;
                                //}
                                ViewBag.YodleeAccountCreated = "True";
                            }
                            catch (Exception e)
                            {
                                // Creating an 'unknown' group
                                if (accountsGroup["unknown"] == null)
                                    accountsGroup["unknown"] = new List<Account>();

                                accountsGroup["unknown"].Add(new Account());
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: Handle this exception.  Not implemented in this sample application
                        // throw e;

                    }
                }
            }
            catch (Exception e)
            {
                // TODO: Handle this exception.  Not implemented in this sample application
                // throw e;
                ViewBag.YodleeAccountCreated = "True";
            }
            //   if (Session["Token"] == null)
            //   {
            //AutomaticLogin
            //   return RedirectToAction("AutomaticLogin", "AutomaticUploadProcess");
            //  }
            //  else
            //  {
            ViewBag.cobrandToken = Convert.ToString(Session["cobrandToken"]);
            ViewBag.userToken = Convert.ToString(Session["userToken"]);
            ViewBag.Token = Convert.ToString(Session["Token"]);
            return View(accountsGroup);
            // }

        }
        #endregion

        #region Add Account
        public ActionResult AddAccount()
        {
            if (Session["panel_login_info"] == null)
            {
                return Redirect("AutomaticLogin");
            }

            var user_info = Session["panel_login_info"] as UserInfo;
            var base_url = ConfigurationManager.AppSettings["restUrl"];
            ViewBag.panel_login_info = user_info;
            ViewBag.base_url = base_url;
            return View();
        }

        // 1. Search a site
        public ActionResult Search_Site(String filter_site)
        {
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var _parameters = new Dictionary<string, object>();
            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);
            _parameters.Add("siteSearchString", filter_site);


            if (String.IsNullOrEmpty(filter_site) || String.IsNullOrWhiteSpace(filter_site))
            {
                return Content("No Result");
            }
            else
            {
                var logger = Session["Logger"] as ILogger;
                var restClient = new CustomRestClient(restUrl, ref logger);

                var siteInfos = restClient.Post<ICollection<SiteInfo>>(YodleeAPI.SEARCH_SITE, _parameters);

                return View(siteInfos);
            }
        }

        // 2. Get site login form
        public ActionResult get_site_login_form(int filter_siteId)
        {
            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);

            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("siteFilter.reqSpecifier", 1);
            _parameters.Add("siteFilter.siteId", filter_siteId);

            var site_info = restClient.Post<SiteInfo>(YodleeAPI.GET_SITE_INFO, _parameters);

            Session["site_info"] = site_info;

            _parameters.Clear();

            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("siteId", filter_siteId);

            var site_login_form = restClient.Post<FormObject>(YodleeAPI.SITE_LOGIN_FORM, _parameters);

            Session["site_login_form"] = site_login_form;
            ViewBag.siteId = filter_siteId;

            return View(site_info);
        }

        // 3. Add the site account providing the login form
        public ActionResult add_site_account1(int siteId, string login, string password, string confirm_password)
        {
            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);
            List<string> messages = new List<string>();

            var site_info = Session["site_info"] as SiteInfo;
            bool isValid = true;

            if (login == "" || password == "" || confirm_password == "")
            {
                messages.Add("All field are required.");
                Session["error_msg"] = messages;
                isValid = false;
            }
            else if (password != confirm_password)
            {
                messages.Add("Passwords must be the same");
                Session["error_msg"] = messages;
                isValid = false;
            }

            if (isValid)
            {
                var credentialFields = Session["site_login_form"] as FormObject;
                int index = 0;

                _parameters.Add("cobSessionToken", Session["cobrandToken"]);
                _parameters.Add("userSessionToken", Session["userToken"]);
                _parameters.Add("siteId", siteId);
                _parameters.Add("credentialFields.enclosedType", "com.yodlee.common.FieldInfoSingle");

                foreach (ComponentList componentList in credentialFields.componentList)
                {
                    _parameters.Add(String.Format("credentialFields[{0}].displayName", index), componentList.displayName);
                    _parameters.Add(String.Format("credentialFields[{0}].fieldType.typeName", index), componentList.fieldType.typeName);
                    _parameters.Add(String.Format("credentialFields[{0}].helpText", index), componentList.helpText);
                    _parameters.Add(String.Format("credentialFields[{0}].maxlength", index), componentList.maxlength);
                    _parameters.Add(String.Format("credentialFields[{0}].name", index), componentList.name);
                    _parameters.Add(String.Format("credentialFields[{0}].size", index), componentList.size);
                    _parameters.Add(String.Format("credentialFields[{0}].value", index), (index == 0) ? login : password);
                    _parameters.Add(String.Format("credentialFields[{0}].valueIdentifier", index), componentList.valueIdentifier);
                    _parameters.Add(String.Format("credentialFields[{0}].valueMask", index), componentList.valueMask);
                    _parameters.Add(String.Format("credentialFields[{0}].isEditable", index), componentList.isEditable);
                    index++;
                }

                var add_site_account1 = restClient.Post<SiteAccountInfo>(YodleeAPI.ADD_SITE_ACCOUNT1, _parameters);

                int memSiteAccId = add_site_account1.siteAccountId;
                if (memSiteAccId > 0)
                {
                    ViewBag.memSiteAccId = memSiteAccId;
                    return View("poll_refresh_site_account", site_info);
                }
                else
                {
                    messages.Add("No result.");
                    return View("msg_errors", messages);
                }
            }
            else
            {
                ViewBag.siteId = siteId;
                return View("get_site_login_form", site_info);
            }
        }

        // 4. Check MFA flow.  
        //	4.1 Get site refresh information.
        public ActionResult get_site_refresh_info(int memSiteAccId)
        {
            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);
            List<string> messages = new List<string>();
            var site_info = Session["site_info"] as SiteInfo;

            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);
            _parameters.Add("memSiteAccId", memSiteAccId);

            var get_site_refresh_info = restClient.Post<SiteRefreshInfo>(YodleeAPI.GET_SITE_REFRESH_INFO, _parameters);

            string refreshMode = get_site_refresh_info.siteRefreshMode.refreshMode;
            string siteRefreshStatus = get_site_refresh_info.siteRefreshStatus.siteRefreshStatus;

            if (refreshMode == "MFA") // It's an MFA refresh
            {
                // CASE: MFA
                if (siteRefreshStatus == "LOGIN_FAILURE")
                {
                    messages.Add(Convert.ToString(ApiErrors.Errors["402"].description));
                    return View("msg_errors", messages);
                }

                if (siteRefreshStatus == "REFRESH_TRIGGERED")
                {
                    ViewBag.memSiteAccId = memSiteAccId;
                    return View("get_mfa_response_for_site", site_info);
                }

                if (siteRefreshStatus == "REFRESH_COMPLETED" || siteRefreshStatus == "REFRESH_TIMED_OUT")
                {
                    return RedirectToAction("get_item_summaries_for_site", "home", new { memSiteAccId = memSiteAccId });
                }
                else
                {
                    return Content("");
                }
            }
            else if (refreshMode == "NORMAL")
            {
                // CASE: NORMAL
                if (siteRefreshStatus == "REFRESH_COMPLETED" || siteRefreshStatus == "REFRESH_TIMED_OUT" || siteRefreshStatus == "LOGIN_FAILURE")
                {
                    if (siteRefreshStatus == "LOGIN_FAILURE")
                    {
                        messages.Add(Convert.ToString(ApiErrors.Errors["402"].description));
                        return View("msg_errors", messages);
                    }
                    else
                    {
                        return RedirectToAction("get_item_summaries_for_site", "home", new { memSiteAccId = memSiteAccId });
                    }
                }
                else
                {
                    return Content("");
                }
            }

            return View();
        }

        //	4.2 Get an MFA request if the refresh is neither with error or completed
        public ActionResult get_mfa_response_for_site(int memSiteAccId)
        {
            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);
            List<string> messages = new List<string>();
            var site_info = Session["site_info"] as SiteInfo;

            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);
            _parameters.Add("memSiteAccId", memSiteAccId);

            var mfa_refresh_info = restClient.Post<MFARefreshInfo>(YodleeAPI.GET_MFA_RESPONSE_FOR_SITE, _parameters);

            Session["mfa_refresh_info"] = mfa_refresh_info;

            if (mfa_refresh_info.retry) // The gatherer is still trying to gather information.  Wait interval.
            {
                return Content("");
            }
            else if (mfa_refresh_info.errorCode == null) // The gatherer has information, if no error, then it's a valid MFA challenge
            {
                if (mfa_refresh_info.isMessageAvailable) // A this point, the message can be available.  Let's present the challenge to the user.
                {
                    var fieldInfo = mfa_refresh_info.fieldInfo;
                    ViewBag.fieldInfo = fieldInfo;
                    ViewBag.memSiteAccId = memSiteAccId;
                    ViewBag.timeOutTime = mfa_refresh_info.timeOutTime;

                    if (fieldInfo.responseFieldType != null)
                    {
                        return View("form_mfa_token", site_info);
                    }

                    if (fieldInfo.questionAndAnswerValues.Count() > 0)
                    {
                        return View("form_mfa_question", site_info);
                    }

                }
                else
                {
                    messages.Add("Error while trying to get information of " + site_info.defaultDisplayName);
                    return View("msg_errors", messages);
                }
            }
            else if (Convert.ToInt32(mfa_refresh_info.errorCode) == 0) // Successfully refresh
            {
                ViewBag.memSiteAccId = memSiteAccId;
                return View("poll_refresh_site_account", site_info);
            }
            else if (Convert.ToInt32(mfa_refresh_info.errorCode) > 0) // Error code is neither null nor 0.  We print out the error
            {
                messages.Add(ApiErrors.Errors[mfa_refresh_info.errorCode].description);
                return View("msg_errors", messages);
            }
            else
            {
                messages.Add("Unexpected error.");
                return View("msg_errors", messages);
            }

            return Content("");
        }
        #endregion

        #region Mobile Fastlink
        public ActionResult AddAccountwithKippin(int userId)
        {
            try
            {
                Session["panel_login_info"] = null;
                Session["userToken"] = null;
                using (var db = new KFentities())
                {
                    var yodleeAccDetails = db.YodleeUserRegistrations.Where(s => s.KippinUserId == userId).FirstOrDefault();
                    if (yodleeAccDetails != null)
                    {
                        var _parameters = new Dictionary<string, object>();

                        var cobrandLogin = ConfigurationManager.AppSettings["cobrandLogin"];
                        var cobrandPassword = ConfigurationManager.AppSettings["cobrandPassword"];
                        var restUrl = ConfigurationManager.AppSettings["restUrl"];
                        var contextPath = ConfigurationManager.AppSettings["contextPath"];

                        var restClient = new CustomRestClient(restUrl);

                        _parameters.Add("cobrandLogin", cobrandLogin);
                        _parameters.Add("cobrandPassword", cobrandPassword);

                        var cobrandContext = restClient.Post<CobrandContext>(YodleeAPI.COB_LOGIN, _parameters);
                        _parameters.Clear();
                        if (cobrandContext.cobrandConversationCredentials == null)
                        {
                            ViewBag.Error = "Co-brand Token Null";
                            // throw new Exception("Invalid Credentials");
                        }
                        _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                        // _parameters.Add("cobSessionToken", "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad");
                        //08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad
                        _parameters.Add("login", yodleeAccDetails.UName);
                        _parameters.Add("password", yodleeAccDetails.UPassword);

                        var userInfo = restClient.Post<UserInfo>(YodleeAPI.USER_LOGIN, _parameters);
                        Session["panel_login_info"] = userInfo;
                        Session["cobrandToken"] = cobrandContext.cobrandConversationCredentials.sessionToken;
                        //  Session["cobrandToken"] = "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad";
                        Session["userToken"] = userInfo.userContext.conversationCredentials.sessionToken;
                        Session["restUrl"] = restUrl;
                        Session["Logger"] = new APILogger();
                        //Getting fastlink token
                        _parameters.Clear();

                        _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                        _parameters.Add("rsession", userInfo.userContext.conversationCredentials.sessionToken);
                        _parameters.Add("finAppId", "10003600");
                        var userToken = restClient.Post<FastLinkAppToken>(YodleeAPI.USER_FastLink, _parameters);

                        ViewBag.userToken = userInfo.userContext.conversationCredentials.sessionToken;
                        ViewBag.Token = userToken.finappAuthenticationInfos[0].token;   //FastLink Token

                    }
                    else
                    {
                        ViewBag.comingSoon = "true";
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.comingSoon = "false";
            }
            return View();
        }
        #endregion

        public void AutoLogin()
        {
            try
            {
                Session["panel_login_info"] = null;
                Session["userToken"] = null;
                var UserInformation = UserData.GetCurrentUserData();
                using (var db = new KFentities())
                {
                    var loginInfo = db.YodleeUserRegistrations.Where(s => s.KippinUserId == UserInformation.Id).FirstOrDefault();

                    if (loginInfo != null)
                    {
                        var _parameters = new Dictionary<string, object>();

                        var cobrandLogin = ConfigurationManager.AppSettings["cobrandLogin"];
                        var cobrandPassword = ConfigurationManager.AppSettings["cobrandPassword"];
                        var restUrl = ConfigurationManager.AppSettings["restUrl"];
                        var contextPath = ConfigurationManager.AppSettings["contextPath"];

                        var restClient = new CustomRestClient(restUrl);

                        _parameters.Add("cobrandLogin", cobrandLogin);
                        _parameters.Add("cobrandPassword", cobrandPassword);

                        try
                        {
                            var cobrandContext = restClient.Post<CobrandContext>(YodleeAPI.COB_LOGIN, _parameters);
                            _parameters.Clear();
                            if (cobrandContext.cobrandConversationCredentials == null)
                            {

                                throw new Exception("Invalid Credentials");
                            }
                            _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                            // _parameters.Add("cobSessionToken", "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad");
                            //08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad
                            _parameters.Add("login", loginInfo.UName);
                            _parameters.Add("password", loginInfo.UPassword);

                            var userInfo = restClient.Post<UserInfo>(YodleeAPI.USER_LOGIN, _parameters);
                            Session["panel_login_info"] = userInfo;
                            Session["cobrandToken"] = cobrandContext.cobrandConversationCredentials.sessionToken;
                            //  Session["cobrandToken"] = "08062013_0:54ec50ddeb741bc6d2caf5b71d2a95e22a4d03ca638e26ec8ee3e0594e1c1b376a99097cf2d100f4d85fd2361ada952df7040da02f022e3f520c921bef5d19ad";
                            Session["userToken"] = userInfo.userContext.conversationCredentials.sessionToken;
                            Session["restUrl"] = restUrl;
                            Session["Logger"] = new APILogger();
                            //Getting fastlink token
                            _parameters.Clear();
                            _parameters.Add("cobSessionToken", cobrandContext.cobrandConversationCredentials.sessionToken);
                            _parameters.Add("rsession", userInfo.userContext.conversationCredentials.sessionToken);
                            _parameters.Add("finAppId", "10003600");
                            var userToken = restClient.Post<FastLinkAppToken>(YodleeAPI.USER_FastLink, _parameters);
                            //FastLink Token
                            Session["Token"] = userToken.finappAuthenticationInfos[0].token;
                        }
                        catch (Exception e)
                        {
                            //return Json(e.Message);
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
        }

        #region FastLink Yodlee Add account

        #endregion

        #region Remove/Unlink Account
        public ActionResult UnLinkAccount(String siteAccountId)
        {

            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);

            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);
            _parameters.Add("memSiteAccId", siteAccountId);

            var response = restClient.RemovePost(YodleeAPI.REMOVE_SITE_ACCOUNT, _parameters);
            if (!string.IsNullOrEmpty(response))
            {
                TempData["RemoveError"] = "Unable to remove account please try again later";
            }
            return RedirectToAction("AccountList");
        }
        #endregion

        //ViewTransaction
        [HttpGet]
        public ActionResult ViewTransaction(string itemAccountId, string BankName, string containerType)
        {

            ViewBag.itemAccountId = itemAccountId;
            ViewBag.BankName = BankName;
            bool past_seven_day = true;
            string filter = "";
            int limitPage = 100,
                endNumber = 100,
                startNumber = 1,
                countRows,
                countPages,
                page;
            var _parameters = new Dictionary<string, object>();
            var logger = Session["Logger"] as ILogger;
            var restUrl = ConfigurationManager.AppSettings["restUrl"];
            var restClient = new CustomRestClient(restUrl, ref logger);
            List<string> messages = new List<string>();

            // Building transaction search request
            _parameters.Add("cobSessionToken", Session["cobrandToken"]);
            _parameters.Add("userSessionToken", Session["userToken"]);
            _parameters.Add("transactionSearchRequest.containerType", containerType);
            _parameters.Add("transactionSearchRequest.higherFetchLimit", "500");
            _parameters.Add("transactionSearchRequest.lowerFetchLimit", "1");
            _parameters.Add("transactionSearchRequest.resultRange.startNumber", startNumber.ToString());
            _parameters.Add("transactionSearchRequest.resultRange.endNumber", endNumber.ToString());
            _parameters.Add("transactionSearchRequest.searchFilter.currencyCode", "USD");

            if (itemAccountId.Trim() != String.Empty)
            {
                _parameters.Add("transactionSearchRequest.searchFilter.itemAccountId.identifier", itemAccountId);
            }

            if (past_seven_day == true || String.IsNullOrEmpty(filter))
            {
                _parameters.Add("transactionSearchRequest.ignoreUserInput", "true");
            }
            else
            {
                _parameters.Add("transactionSearchRequest.ignoreUserInput", "false");
                _parameters.Add("transactionSearchRequest.userInput", filter.ToString());
            }

            if (past_seven_day == true)
            {
                DateTime fromDate = DateTime.Today.AddMonths(-20);
                DateTime toDate = DateTime.Today;

                _parameters.Add("transactionSearchRequest.searchFilter.postDateRange.fromDate", fromDate.ToString("MM-dd-yyyy"));
                _parameters.Add("transactionSearchRequest.searchFilter.postDateRange.toDate", toDate.ToString("MM-dd-yyyy"));
            }

            try
            {

                var transaction_search_exec_info = restClient.Post<TransactionSearchExecInfo>(YodleeAPI.EXECUTE_USER_SEARCH_REQUEST, _parameters);

                if (transaction_search_exec_info.searchResult == null)
                {
                    throw new Exception();
                }

                var transactions = transaction_search_exec_info.searchResult.transactions;

                /*To Yoodlee Data For Testing*/
                //foreach (var itm in transactions)
                //{
                //    //string itmm = itm.status.description.ToString() + "," + itm.account.ToString() + "," + itm.amount.ToString() + "," + itm.postDate.ToString() + "," + itm.status.ToString() + "," + itm.transactionBaseType.ToString() + "," + itm.transactionDate.ToString() + "," + itm.viewKey.ToString() + "," + Session["cobrandToken"].ToString(); ;
                //    string itmm = itm.status.description.ToString() + "," + Session["cobrandToken"].ToString() + "," + Session["userToken"].ToString() + "," + containerType + "," + endNumber + "," + itemAccountId + ",";


                //    using (var db = new KFentities())
                //    {
                //        TestYodleeData ord = new TestYodleeData
                //       {
                //           Data = itmm,

                //       };
                //        // Add the new object to the Orders collection.
                //        db.TestYodleeDatas.Add(ord);

                //        // Submit the change to the database.
                //        try
                //        {
                //            db.SaveChanges();
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine(e);

                //        }
                //    }

                //}


                if (transactions.Count() > 0)
                {
                    Session["TransactionSessionList"] = null;
                    Session["TransactionSessionList"] = transactions;
                    return View(transactions);
                }
                else
                {
                    Session["TransactionSessionList"] = null;
                    Session["TransactionSessionList"] = new List<Transactions>();
                    return View(new List<Transactions>());
                }
            }
            catch (Exception e)
            {
                ViewBag.countRows = 0;
                ViewBag.countPages = 0;
                ViewBag.first_page = 0;
                ViewBag.previous_page = 0;
                ViewBag.next_page = 0;
                ViewBag.last_page = 0;
                ViewBag.searchIdentifier = "";
                ViewBag.current_page = 0;
                return View(new List<Transactions>());
            }


            //    return RedirectToAction("AccountList", "AutomaticUploadProcess");
        }

        #region UploadTrasactions
        public ActionResult UploadTrasactions(string itemAccountId, string BankName)
        {
            ViewBag.BankName = BankName;
            var list = Session["TransactionSessionList"] as List<Transactions>;
            var userData = UserData.GetCurrentUserData();
            using (var db = new KFentities())
            {
                var TransactionId = db.BankExpenses.Where(s => s.UserId == userData.Id && s.TransactionId != null).OrderByDescending(d => d.Id).FirstOrDefault();
                if (TransactionId != null)
                {
                    list = list.Where(w => w.viewKey.transactionId > TransactionId.TransactionId).ToList();
                }

                if (list.Count > 0)
                {
                    if (userData.RoleId == 1)
                    {
                        //Accountant
                        long SelectedActiveUser = 0;
                        if (Request.Cookies["SelectedActiveUser"] != null)
                        {
                            if (Request.Cookies["SelectedActiveUser"].Value != null)
                            {
                                long.TryParse(Convert.ToString(Request.Cookies["SelectedActiveUser"].Value), out SelectedActiveUser);
                            }

                        }
                        if (SelectedActiveUser > 0)
                        {
                            //Do insert operation
                        }
                    }
                    else
                    {
                        //Normal User with roleId=2,3


                        int BankId = this.GetBankId(BankName);
                        if (BankId > 0)
                        {
                            int userAccCount = db.BankExpenses.Where(s => s.UserId == userData.Id && s.BankId == BankId).Select(s => s.AccountClassificationId).Distinct().Count();
                            if (userAccCount < 4)
                            {
                                List<BankExpense> objList = new List<BankExpense>();
                                int accClassificationNumber = 0000;
                                switch (userAccCount)
                                {
                                    case 1:
                                        accClassificationNumber = 1061;
                                        break;
                                    case 2:
                                        accClassificationNumber = 1062;
                                        break;
                                    case 3:
                                        accClassificationNumber = 1063;
                                        break;
                                    case 0:
                                        accClassificationNumber = 1060;
                                        break;
                                }
                                foreach (var data in list)
                                {
                                    var bankEntry = new BankExpense();

                                    bankEntry.BankId = BankId;
                                    bankEntry.UserId = userData.Id;
                                    bankEntry.AccountClassificationId = accClassificationNumber;
                                    bankEntry.AccountName = data.account.accountName;
                                    bankEntry.AccountNumber = data.account.accountNumber;
                                    bankEntry.Description = data.description.description;
                                    bankEntry.CategoryId = 6;
                                    bankEntry.IsDeleted = false;
                                    bankEntry.UploadType = "A";
                                    bankEntry.StatusId = 1;
                                    bankEntry.ClassificationId = 1;
                                    if (data.transactionBaseType == "credit")
                                    {
                                        bankEntry.Credit = data.amount.amount;
                                        bankEntry.Debit = 0;
                                        bankEntry.TransactionType = "C";
                                    }
                                    else
                                    {
                                        bankEntry.Credit = 0;
                                        bankEntry.Debit = data.amount.amount;
                                        bankEntry.TransactionType = "D";
                                    }
                                    bankEntry.IsYodlee = true;
                                    bankEntry.Date = Convert.ToDateTime(data.transactionDate);
                                    bankEntry.CreatedDate = DateTime.Now;
                                    //  bankEntry.Total = data.account.availableBalance.amount;
                                    objList.Add(bankEntry);
                                }
                                if (objList.Count > 0)
                                {
                                    #region Add FriendlyAccNameClassification
                                    string ClassificationType = objList[0].AccountName;
                                    string ChartNo = Convert.ToString(accClassificationNumber);
                                    if (db.Classifications.Where(s => s.UserId == userData.Id && s.ClassificationType == ClassificationType && s.ChartAccountDisplayNumber == ChartNo).Any())
                                    {

                                    }
                                    else
                                    {
                                        var dbInsert = new Classification();
                                        dbInsert.CategoryId = 1; //Asset Entry
                                        dbInsert.ChartAccountDisplayNumber = Convert.ToString(accClassificationNumber);
                                        dbInsert.CreatedDate = DateTime.Now;
                                        dbInsert.IsDeleted = false;
                                        dbInsert.IsIncorporated = false;
                                        dbInsert.IsPartnerShip = false;
                                        dbInsert.IsSole = false;
                                        dbInsert.UserId = userData.Id;
                                        dbInsert.Desc = list[0].account.accountName;
                                        dbInsert.ClassificationType = list[0].account.accountName;
                                        dbInsert.Type = "A";
                                        db.Classifications.Add(dbInsert);
                                    }

                                    #endregion
                                    db.BankExpenses.AddRange(objList);
                                    db.SaveChanges();
                                }
                            }
                        }

                    }
                    TempData["TransactionUploadMsg"] = "Transaction uploaded successfully.";
                }
                else
                {
                    TempData["TransactionUploadMsg"] = "No new transaction found for upload.";
                }
            }

            return RedirectToAction("ViewTransaction", new { itemAccountId = itemAccountId, BankName = BankName });
        }
        #endregion

        public int GetBankId(string siteName)
        {
            if (siteName.Contains("rbc"))
                return 1;
            else if (siteName.Contains("scotia"))
                return 2;
            else if (siteName.Contains("bmo"))
                return 3;
            else if (siteName.Contains("td"))
                return 4;
            else if (siteName.Contains("cibc"))
                return 5;
            else
                return 0;
        }
        #endregion

        #region Add Sub Accountant /Employee Section
        #region DeleteEmployee
        public ActionResult DeleteEmployee(int id)
        {
            using (var db = new KFentities())
            {
                db.UserRegistrations.Remove(db.UserRegistrations.Where(s => s.Id == id).FirstOrDefault());
                db.SaveChanges();
            }
            return RedirectToAction("GetEmployees", new { isDeleted = true });
        }
        #endregion
        #region DeactivateEmployee
        public ActionResult DeactivateEmployee(int id)
        {
            using (var db = new KFentities())
            {
                var userData = db.UserRegistrations.Where(s => s.Id == id).FirstOrDefault();
                if (userData != null)
                {
                    userData.IsDeleted = false;
                    userData.IsPaid = false;
                    userData.IsEmailSent = false;
                    userData.IsTrial = false;
                    userData.IsEmployeeActivated = false;
                }
                db.SaveChanges();
            }
            return RedirectToAction("GetEmployees", new { isDeactivated = true });
        }
        #endregion
        #region ActivateEmployee
        public ActionResult ActivateEmployee(int id)
        {
            using (var db = new KFentities())
            {
                var userData = db.UserRegistrations.Where(s => s.Id == id).FirstOrDefault();
                if (userData != null)
                {
                    userData.IsDeleted = false;
                    userData.IsPaid = true;
                    userData.IsTrial = true;
                    userData.IsEmployeeActivated = true;
                }
                db.SaveChanges();
            }
            return RedirectToAction("GetEmployees", new { isActivated = true });
        }
        #endregion

        //Encrypt SubAccountantId
        public ActionResult EncryptSubAccountantId(int id, bool? status)
        {
            //here id is sub accountantID
            Dictionary<String, String> encryptedQueryString = new Dictionary<String, String>();
            encryptedQueryString = new Dictionary<String, String> { { "SubAccountantId", id.ToString() } };
            String encryptQueryString = Security.ToEncryptedQueryString(encryptedQueryString);
            if (status != null)
                return RedirectToAction("SaveSubAccountant", new { args = encryptQueryString, status });
            else
                return RedirectToAction("SaveSubAccountant", new { args = encryptQueryString });
        }

        [KippinAuthorize(Roles = "Accountant")]
        public ActionResult SaveSubAccountant(bool? status, string args)
        {

            #region Decrypt Sub AccountantID from query string
            int SubAccountantId = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["args"]))
            {
                Dictionary<String, String> decryptedQueryString = new Dictionary<String, String>();
                decryptedQueryString.ToDecryptedQueryString(Request.QueryString["args"].ToString());

                if (decryptedQueryString.Where(s => s.Key == "SubAccountantId").Any())
                {
                    var statementData = decryptedQueryString.Where(s => s.Key == "SubAccountantId").FirstOrDefault();
                    SubAccountantId = Convert.ToInt32(statementData.Value);
                }
            }
            #endregion

            SubAccountantViewModel obj = new SubAccountantViewModel();
            if (status == true)
            {
                obj.Status = true;
            }
            if (SubAccountantId > 0)
            {
                var userData = UserData.GetUserData(SubAccountantId);
                if (userData != null)
                {
                    obj.Id = SubAccountantId;
                    obj.Username = userData.Username;
                    obj.Password = userData.Password;
                    obj.Email = userData.Email;
                    var mob = userData.MobileNumber.Split('-');
                    obj.AreaCode = mob[0];
                    obj.MobileNumber = mob[1];
                }

                obj.CustomerList = ClsDropDownList.GetAccountantCustomerList(true, SubAccountantId);

            }
            else
                obj.CustomerList = ClsDropDownList.GetAccountantCustomerList(false, null);//false means simple create sub accounatnt

            return View(obj);
        }
        [HttpPost]
        public ActionResult SaveSubAccountant(SubAccountantViewModel obj, string hdnUser)
        {
            var selectedIds = hdnUser.Split(',');

            using (var db = new KFentities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.Id > 0)
                        {
                            if (db.UserRegistrations.Where(s => s.Email == obj.Email && s.Id != obj.Id).Any())
                            {
                                ModelState.AddModelError("Email", "Email already in use.Please try another.");
                            }
                            else if (db.UserRegistrations.Where(s => s.Username == obj.Username && s.Id != obj.Id).Any())
                            {
                                ModelState.AddModelError("Username", "Username already in use.Please try another.");
                            }
                        }
                        else
                        {
                            if (db.UserRegistrations.Where(s => s.Email == obj.Email).Any())
                            {
                                ModelState.AddModelError("Email", "Email already in use.Please try another.");
                            }
                            else if (db.UserRegistrations.Where(s => s.Username == obj.Username).Any())
                            {
                                ModelState.AddModelError("Username", "Username already in use.Please try another.");
                            }
                        }
                        if (obj.Password.Trim().Length < 6)
                        {
                            ModelState.AddModelError("Password", "Password must be 6 character long.");
                        }
                        if (ModelState.IsValid)
                        {
                            var userdata = UserData.GetCurrentUserData();
                            if (userdata != null)
                            {
                                if (obj.Id > 0)
                                {
                                    //update
                                    var oldPermissions = db.SubAccountantUserPermissions.Where(d => d.SubAccountantId == obj.Id).ToList();
                                    oldPermissions.ForEach(f => f.IsDeleted = true);

                                    #region Update Sub Accountant
                                    var dbUpdate = db.UserRegistrations.Where(s => s.Id == obj.Id).FirstOrDefault();
                                    if (dbUpdate != null)
                                    {
                                        dbUpdate.Password = obj.Password;
                                        dbUpdate.MobileNumber = obj.AreaCode + "-" + obj.MobileNumber;

                                        #region Employee Customer Permission
                                        var customerListPermission = new List<SubAccountantUserPermission>();
                                        foreach (var checkedItem in selectedIds)
                                        {
                                            var objPermission = new SubAccountantUserPermission();
                                            objPermission.IsDeleted = false;
                                            objPermission.AccountantId = userdata.Id;
                                            objPermission.CreatedDate = DateTime.Now;
                                            objPermission.SubAccountantId = dbUpdate.Id;
                                            objPermission.UserWithAnAccountantId = Convert.ToInt32(checkedItem);
                                            customerListPermission.Add(objPermission);
                                        }

                                        db.SubAccountantUserPermissions.AddRange(customerListPermission);
                                        #endregion
                                        db.SaveChanges();
                                        dbContextTransaction.Commit();
                                        return RedirectToAction("EncryptSubAccountantId", new { id = obj.Id, status = true });
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Create New Sub Accountant
                                    var dbInsert = new UserRegistration();
                                    dbInsert.AccountantId = userdata.Id;
                                    dbInsert.RoleId = 4;
                                    dbInsert.CreatedDate = DateTime.Now;
                                    dbInsert.Email = obj.Email;
                                    dbInsert.Username = obj.Username;
                                    dbInsert.Password = obj.Password;
                                    dbInsert.FirstName = userdata.Username;
                                    dbInsert.LastName = userdata.Username;
                                    dbInsert.MobileNumber = obj.AreaCode + "-" + obj.MobileNumber;
                                    dbInsert.CountryId = userdata.CountryId;
                                    dbInsert.ProvinceId = userdata.ProvinceId;
                                    dbInsert.City = userdata.City;
                                    dbInsert.PostalCode = userdata.PostalCode;
                                    dbInsert.SectorId = userdata.SectorId;
                                    dbInsert.CompanyName = userdata.CompanyName;
                                    dbInsert.CorporationAddress = userdata.CorporationAddress;
                                    dbInsert.GSTNumber = userdata.GSTNumber;
                                    dbInsert.BusinessNumber = userdata.BusinessNumber;
                                    dbInsert.OwnershipId = userdata.OwnershipId;
                                    dbInsert.CurrencyId = userdata.CurrencyId;
                                    dbInsert.LicenseId = 1;
                                    dbInsert.IsTrial = false;
                                    dbInsert.IsVerified = true;
                                    dbInsert.IsPaid = true;
                                    dbInsert.IsEmployeeActivated = true;
                                    dbInsert.IsTermsAndConditionAccepted = userdata.IsTermsAndConditionAccepted;
                                    dbInsert.IsDeleted = false;

                                    db.UserRegistrations.Add(dbInsert);
                                    db.SaveChanges();
                                    var customerListPermission = new List<SubAccountantUserPermission>();
                                    foreach (var checkedItem in selectedIds)
                                    {
                                        var objPermission = new SubAccountantUserPermission();
                                        objPermission.IsDeleted = false;
                                        objPermission.AccountantId = userdata.Id;
                                        objPermission.CreatedDate = DateTime.Now;
                                        objPermission.SubAccountantId = dbInsert.Id;
                                        objPermission.UserWithAnAccountantId = Convert.ToInt32(checkedItem);
                                        db.SubAccountantUserPermissions.Add(objPermission);
                                        db.SaveChanges();
                                        customerListPermission.Add(objPermission);
                                    }



                                    #endregion

                                }

                                dbContextTransaction.Commit();
                                return RedirectToAction("SaveSubAccountant", new { status = true });
                            }
                        }



                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }



            }
            if (obj.Id > 0)
            {
                obj.CustomerList = ClsDropDownList.GetAccountantCustomerList(true, null);
            }
            else
            {
                obj.CustomerList = ClsDropDownList.GetAccountantCustomerList(false, null);
            }
            if (!string.IsNullOrEmpty(hdnUser))
            {
                foreach (var checkedItem in selectedIds)
                {
                    int customerId = Convert.ToInt32(checkedItem);
                    var data = obj.CustomerList.Where(d => d.CustomerId == customerId).FirstOrDefault();
                    data.IsChecked = true;
                }
            }
            return View(obj);
        }

        public ActionResult GetEmployees(bool? isActivated, bool? isDeactivated, bool? isDeleted)
        {
            if (isActivated == true)
            {
                ViewBag.isActivated = "true";
            }
            if (isDeactivated == true)
            {
                ViewBag.isDeactivated = "true";
            }
            if (isDeleted == true)
            {
                ViewBag.isDeleted = "true";
            }
            return View();
        }
        public JsonResult ActiveEmployeesList(int? page, int? limit, string sortBy, string direction, string searchString = null)
        {
            int total;
            var records = new GridModel().GetActiveEmployeeList(page, limit, sortBy, direction, searchString, out total);
            return Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }
        [KippinAuthorize(Roles = "Accountant")]
        public ActionResult SendEmployeeEmail(int UserId)
        {
            //here UserId is userid which we have to send mail
            using (var userRepository = new AccountRepository())
            {
                try
                {
                    var userData = UserData.GetUnpaidUserData(UserId);
                    var data = UserData.GetCurrentUserData();
                    //Read Email Message body from html file
                    string html = System.IO.File.ReadAllText(Server.MapPath("~/EmailFormats/AddEmployee.html"));
                    //Replace Username
                    html = html.Replace("#Username", userData.Username);

                    var bodymessage = "Your KIPPIN account details: <br/>";
                    bodymessage += "Username: " + userData.Username + "<br/>";
                    bodymessage += "Email: " + userData.Email + "<br/>";
                    bodymessage += "Private Key: " + userData.Password + "<br/>";
                    // bodymessage += "Your Accountant name is: " + user.FirstName+" " + user.LastName + "." + "<br/>";
                    bodymessage += "Accountant's Company name is: " + data.CompanyName + "." + "<br/>";
                    //  bodymessage += "Please download the KIPPIN app from IOS / Android store and login to activate your account";
                    html = html.Replace("#MESSAGE", bodymessage);
                    SendMailModelDto _objModelMail = new SendMailModelDto();
                    _objModelMail.To = userData.Email;
                    _objModelMail.Subject = "Kippin-Finance Account Details";
                    _objModelMail.MessageBody = html;
                    //Sending mail
                    var mailSent = Sendmail.SendEmail(_objModelMail);
                    //update database
                    var sendEmailStatusUpdate = userRepository.SentEmailStatus(userData.Email);
                    TempData["SendEmailStatus"] = "Success";
                }
                catch (Exception)
                {
                    TempData["SendEmailStatus"] = "Failure";
                }

            }
            return RedirectToAction("GetEmployees");
        }

        //[KippinAuthorize(Roles = "Accountant")]
        //public ActionResult UpdateSubAccountant(int id, bool? status)
        //{
        //    using (var db = new KFentities())
        //    {
        //        // ViewBag.records = new GridModel().GetEmployeeUserList().ToList();
        //        var userData = db.UserRegistrations.Where(s => s.Id == id).FirstOrDefault();

        //        var obj = new SubAccountantViewModel();
        //        if (userData != null)
        //        {
        //            obj.Id = userData.Id;
        //            obj.Username = userData.Username;
        //            obj.Password = userData.Password;
        //            obj.Email = userData.Email;
        //            var mob = userData.MobileNumber.Split('-');
        //            obj.AreaCode = mob[0];
        //            obj.MobileNumber = mob[1];
        //            // obj.AccountantUnderEmployees = userData.AccountantUnderEmployees;
        //        }
        //        if (status == true)
        //        {
        //            obj.Status = true;
        //        }
        //        return View(obj);
        //    }
        //}
        //[HttpPost]
        //public ActionResult UpdateSubAccountant(SubAccountantViewModel obj, string hdnUser_updateValue)
        //{
        //    using (var db = new KFentities())
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var data = db.UserRegistrations.Where(s => s.Id == obj.Id).FirstOrDefault();
        //            if (data != null)
        //            {
        //                data.Password = obj.Password;
        //                data.MobileNumber = obj.AreaCode + "-" + obj.MobileNumber;
        //                //  data.AccountantUnderEmployees = hdnUser_updateValue;
        //                db.SaveChanges();
        //                return RedirectToAction("UpdateSubAccountant", new { id = obj.Id, status = true });
        //            }
        //        }
        //    }
        //    return View(obj);
        //}
        #endregion

    }
}