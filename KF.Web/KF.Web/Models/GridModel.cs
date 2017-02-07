using KF.Dto.Modules.Finance;
using KF.Entity;
using KF.Repo.Modules.Finance;
using KF.Repo.Modules.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KF.Web.Models
{
    public class GridModel
    {
        #region Finance
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <param name="searchString"></param>
        /// <param name="total"></param>
        /// <param name="userId"></param>
        /// <param name="selectedUserId"></param>
        /// <param name="StatusId"></param>
        /// <param name="roleId"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="BankId"></param>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public List<KF.Dto.Modules.Other.ReconcillationBankExpenseDto> GetUserExpenseListForUserWithAnAccountant(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userId, int selectedUserId, int? StatusId, int roleId, int? Year, int? Month, int? BankId, int? CategoryId)
        {
            using (var expenseRepository = new ReconcillationRepository())
            {
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties

                var dataList = expenseRepository.GetExpensesForUserWithAnAccountantByUserId(userId, selectedUserId, StatusId, roleId, Skip, Take, Year, Month, BankId, CategoryId);

                //   ObjView.ObjBankExpensesList = expenseRepository.GetExpensesByUserId(id);

                //var dataEventList = dbClass.GetAll().ToList().Skip(start).Take(limit.Value);
                List<KF.Dto.Modules.Other.ReconcillationBankExpenseDto> objViewModel = new List<KF.Dto.Modules.Other.ReconcillationBankExpenseDto>();
                // dataList.ForEach(i=>i.Date=i.Date.Value.)
                objViewModel = dataList;
                if (dataList.Count > 0)
                {
                    total = dataList[0].TotalCount;
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();

                //search on basis of title
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    records = records.Where(p => p.Status.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }

        /// <summary>
        /// Get Bank Statement List for statement reconcillation
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <param name="searchString"></param>
        /// <param name="total"></param>
        /// <param name="userId"></param>
        /// <param name="selectedUserId"></param>
        /// <param name="StatusId"></param>
        /// <param name="roleId"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="BankId"></param>
        /// <param name="JVID"></param>
        /// <param name="CategoryId"></param>
        /// <param name="IsSecondLoad"></param>
        /// <returns></returns>
        public List<StatementList> GetUserExpenseList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userId, int selectedUserId, int? StatusId, int roleId, int? Year, int? Month, int? BankId, int? JVID, int? CategoryId, bool? IsSecondLoad)
        {
            using (var expenseRepository = new ReconcillationRepository())
            {
                List<StatementList> objViewModel = new List<StatementList>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                //step 2 : Get data from database and map the properties
                //Second Load: When first time page load grid is empty when we select any filter then data should be bind to grid this bit is set when reconcilationfilter action method is invoked.
                if (IsSecondLoad == true)
                {
                    if (StatusId > 0 || Year > 0 || Month > 0 || BankId > 0 || CategoryId > 0 || JVID > 0)
                    {
                        var dataList = expenseRepository.GetExpensesByUserId(userId, selectedUserId, StatusId, roleId, Skip, Take, Year, Month, BankId, JVID, CategoryId);
                        objViewModel = dataList.StatementList;
                        if (dataList.StatementList.Count > 0)
                        {
                            total = dataList.TotalCount;
                        }
                        else
                        {
                            total = 0;
                        }
                    }
                    else
                    {
                        total = 0;
                    }
                }
                else
                {
                    total = 0;
                }
                var records = objViewModel.AsQueryable();

                //search on basis of title
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    records = records.Where(p => p.StatementStatus.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.OrderBy(s => s.Id).ToList();
            }
        }

        /// <summary>
        /// Get Active user list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <param name="searchString"></param>
        /// <param name="total"></param>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public List<ActiveUserListViewModel> GetActiveUserList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, string firstName)
        {
            using (var repo = new AccountRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<ActiveUserListViewModel> objViewModel = new List<ActiveUserListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                int EmployeeLoginId = 0;
                if (userData.RoleId == 4)
                {
                    accountantId = Convert.ToInt32(userData.AccountantId);
                    EmployeeLoginId = Convert.ToInt32(userData.Id);
                }
                else
                {
                    accountantId = userData.Id;
                }
                var dataList = repo.GetUserListByAccountantId(firstName, accountantId, Skip, Take, EmployeeLoginId);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ActiveUserListViewModel obj = new ActiveUserListViewModel();
                        obj.SendMail = "Send Mail";
                        obj.Id = data.Id;
                        obj.Fullname = data.FirstName + " " + data.LastName;
                        obj.Email = data.Email;
                        obj.Company = data.CompanyName;
                        obj.TaxStartYear = Convert.ToString(data.TaxStartYear);
                        obj.TaxEndYear = Convert.ToString(data.TaxEndYear);
                        if (data.IsPaid == true)
                            obj.UserStatus = "Active";
                        else if (data.IsVerified == true)
                            obj.UserStatus = "Trial";
                        else
                            obj.UserStatus = "In-Active";
                        if (data.IsEmailSent == true)
                            obj.EmailSendStatus = "Yes";
                        else
                            obj.EmailSendStatus = "No";
                        obj.CorporationAddress = data.CorporationAddress;

                        objViewModel.Add(obj);
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = repo.GetActiveUserCount(accountantId, firstName, EmployeeLoginId);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();

                //search on basis of title
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    records = records.Where(p => p.Fullname.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public List<ActiveUserListViewModel> GetEmployeeUserList()
        //{
        //    using (var expenseRepository = new AccountRepository())
        //    {
        //        var userData = UserData.GetCurrentUserData();
        //        List<AccountantCustomers> objViewModel = new List<AccountantCustomers>();

        //        // int start = (page.Value - 1) * limit.Value;
        //        //step 2 : Get data from database and map the properties
        //        int accountantId = 0;
        //        if (userData.RoleId == 4)
        //        {
        //            accountantId = Convert.ToInt32(userData.AccountantId);
        //        }
        //        else
        //        {
        //            accountantId = userData.Id;
        //        }
        //        var dataList = expenseRepository.GetEmployeeUserListByAccountantId(string.Empty, accountantId);
        //        if (dataList.Count() > 0)
        //        {
        //            foreach (var data in dataList)
        //            {
        //                ActiveUserListViewModel obj = new ActiveUserListViewModel();
        //                obj.SendMail = "Send Mail";
        //                obj.Id = data.Id;
        //                obj.Fullname = data.FirstName + " " + data.LastName;
        //                obj.Email = data.Email;
        //                obj.Company = data.CompanyName;
        //                obj.TaxStartYear = Convert.ToString(data.TaxStartYear);
        //                obj.TaxEndYear = Convert.ToString(data.TaxEndYear);
        //                if (data.IsPaid == true)
        //                    obj.UserStatus = "Active";
        //                else if (data.IsVerified == true)
        //                    obj.UserStatus = "Trial";
        //                else
        //                    obj.UserStatus = "In-Active";
        //                if (data.IsEmailSent == true)
        //                    obj.EmailSendStatus = "Yes";
        //                else
        //                    obj.EmailSendStatus = "No";
        //                obj.CorporationAddress = data.CorporationAddress;

        //                objViewModel.Add(obj);
        //            }
        //        }

        //        var records = objViewModel.ToList();

        //        return records.ToList();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="sortBy"></param>
        /// <param name="direction"></param>
        /// <param name="searchString"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<SubAccountantViewModel> GetActiveEmployeeList(int? page, int? limit, string sortBy, string direction, string searchString, out int total)
        {
            using (var expenseRepository = new AccountRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<SubAccountantViewModel> objViewModel = new List<SubAccountantViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties

                var dataList = expenseRepository.GetEmployeeListByAccountantId(userData.Id, Skip, Take);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        SubAccountantViewModel obj = new SubAccountantViewModel();
                        obj.SendMail = "Send Mail";
                        obj.Id = data.Id;
                        obj.Username = data.Username;
                        obj.Email = data.Email;
                        obj.Password = data.Password;
                        if (data.IsPaid == true)
                            obj.EmployeeStatus = "Active";
                        else
                            obj.EmployeeStatus = "In-Active";
                        if (data.IsEmailSent == true)
                            obj.EmailSendStatus = "Yes";
                        else
                            obj.EmailSendStatus = "No";

                        objViewModel.Add(obj);
                    }
                }

                // objViewModel = dataList;
                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.GetActiveEmployeeCount(userData.Id);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();

                //search on basis of title
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    records = records.Where(p => p.Username.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
       
        #endregion
       
        #region Invoice
        /*Invoice Grid Bind*/
        public List<SupplierListViewModel> GetSupplierList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<SupplierListViewModel> objViewModel = new List<SupplierListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 37;

                var dataList = expenseRepository.GetSupplierListById(userid, accountantId, Skip, Take, Convert.ToInt32(userData.RoleId), AccountantUnderEmployees, EmployeeLoginId, searchString);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        SupplierListViewModel obj = new SupplierListViewModel();

                        obj.Id = data.Id;
                        obj.CompanyName = data.Company_Name;

                        objViewModel.Add(obj);
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountSupplierList(userid, searchString);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
        public List<SupplierListViewModel> GetCustomerList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<SupplierListViewModel> objViewModel = new List<SupplierListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 37;
                var dataList = expenseRepository.GetCustomerListById(userid, accountantId, Skip, Take, Convert.ToInt32(0), AccountantUnderEmployees, EmployeeLoginId, searchString);

                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        SupplierListViewModel obj = new SupplierListViewModel();

                        obj.Id = data.Id;
                        obj.CompanyName = data.Company_Name;

                        objViewModel.Add(obj);
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountCustomerList(userid, searchString);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
        public List<ReceivedInvoiceListViewModel> GetReceivedInvoiceList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<ReceivedInvoiceListViewModel> objViewModel = new List<ReceivedInvoiceListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 0;

                var dataList = expenseRepository.ReceivedInvoiceList(userid);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ReceivedInvoiceListViewModel obj = new ReceivedInvoiceListViewModel();

                        using (var context = new KFentities())
                        {
                            obj.Id = data.Id;
                            obj.In_R_FlowStatus = data.In_R_FlowStatus;
                            obj.In_R_Status = data.In_R_Status;
                            obj.InvoiceDate = data.InvoiceDate;
                            if (data.InvoiceNumber.Length == 1)
                            {
                                data.InvoiceNumber = "0000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "00" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "0" + data.InvoiceNumber;
                            }

                            // No it's not.
                            obj.InvoiceNumber = data.InvoiceNumber;
                            obj.DueDate = data.DueDate;
                            obj.DocumentRef = data.DocumentRef;
                            obj.DepositePayment = data.DepositePayment;
                            obj.BalanceDue = data.BalanceDue;
                            obj.CustomerId = data.CustomerId;
                            obj.CreatedDate = data.CreatedDate;
                            obj.IsDeleted = data.IsDeleted;
                            obj.IsInvoiceReport = data.IsInvoiceReport;
                            obj.ModifyDate = data.ModifyDate;
                            obj.Note = data.Note;
                            obj.PaymentTerms = data.PaymentTerms;
                            obj.Pro_FlowStatus = data.Pro_FlowStatus;
                            obj.Pro_Status = data.Pro_Status;
                            obj.RoleId = data.RoleId;
                            obj.SalesPerson = data.SalesPerson;
                            obj.ShippingCost = data.ShippingCost;
                            obj.Terms = data.Terms;
                            obj.Total = String.Format("{0:0.00}", data.Total);
                            obj.Type = data.Type;
                            obj.UserId = data.UserId;
                            obj.Username = context.Kl_GetCompanyNamr(data.UserId, data.CustomerId).FirstOrDefault();
                            obj.FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == data.CustomerId).Select(s => s.Company_Name).FirstOrDefault();

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
                            objViewModel.Add(obj);
                        }

                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountReceivedInvoiceList(userid);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
        public List<ReceivedInvoiceListViewModel> GetInvoiceReportList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid, string typeofinvoice, string flow)
        {
            using (var expenseRepository = new WebUserRepository())
            {


                var userData = UserData.GetCurrentUserData();
                List<ReceivedInvoiceListViewModel> objViewModel = new List<ReceivedInvoiceListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 0;

                var dataList = expenseRepository.ReportingList(userid, accountantId, Skip, Take, Convert.ToInt32(1), AccountantUnderEmployees, EmployeeLoginId, typeofinvoice, flow);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ReceivedInvoiceListViewModel obj = new ReceivedInvoiceListViewModel();


                        using (var context = new KFentities())
                        {
                            if (data.InvoiceNumber.Length == 1)
                            {
                                data.InvoiceNumber = "0000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "00" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "0" + data.InvoiceNumber;
                            }
                            obj.Id = data.Id;
                            obj.In_R_FlowStatus = data.In_R_FlowStatus;
                            obj.In_R_Status = data.In_R_Status;
                            obj.InvoiceDate = data.InvoiceDate;
                            obj.InvoiceNumber = data.InvoiceNumber;


                            if (data.Pro_Status != "Paid")
                            {
                                String dueDate = data.DueDate.ToString();
                                String todayDate = DateTime.Now.ToString();
                                DateTime date1 = Convert.ToDateTime(dueDate);

                                DateTime date2 = Convert.ToDateTime(todayDate);

                                int res = Convert.ToInt32((date1.Date - date2.Date).TotalDays);

                                obj.DueDate = res.ToString();



                            }
                            else
                            {
                                String dueDate = data.DueDate;
                                String paymentDate = data.PaymentDate;
                                DateTime date1 = Convert.ToDateTime(dueDate);
                                DateTime date2 = Convert.ToDateTime(paymentDate);
                                int res = Convert.ToInt32((date1.Date - date2.Date).TotalDays);

                                obj.DueDate = res.ToString();
                            }

                            // obj.DueDate = data.DueDate;


                            obj.DocumentRef = data.DocumentRef;
                            if (data.IsCustomer == false)
                            {
                                if (data.IsSupplierManualPaid == true)
                                {
                                    obj.BalanceDue = (data.Total == null ? 0 : data.Total) - ((data.SupplierManualPaidAmount == null ? 0 : data.SupplierManualPaidAmount)) - ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                    if (obj.BalanceDue == 0)
                                    {
                                        obj.In_R_FlowStatus = "Paid";
                                    }
                                    obj.DepositePayment = data.SupplierManualPaidAmount + ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                }
                                else
                                {
                                    //obj.BalanceDue = data.BalanceDue;
                                    obj.BalanceDue = (data.Total == null ? 0 : data.Total) - ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                    obj.DepositePayment = data.DepositePayment;
                                }

                                if (data.IsSupplierManualPaid == true)
                                {
                                    obj.MethodOfPayment = "Manual";
                                    if (data.IsPaymentbyStripe == true)
                                    {
                                        obj.MethodOfPayment = "Both";
                                    }
                                }


                            }

                            if (data.IsCustomer == true)
                            {
                                if (data.IsCustomerManualPaid == true)
                                {
                                    obj.BalanceDue = (data.Total == null ? 0 : data.Total) - ((data.CustomerManualPaidAmount == null ? 0 : data.CustomerManualPaidAmount)) - ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                    if (obj.BalanceDue == 0)
                                    {
                                        obj.In_R_FlowStatus = "Paid";
                                    }
                                    obj.DepositePayment = data.CustomerManualPaidAmount + ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                }
                                else
                                {
                                    //obj.BalanceDue = data.BalanceDue;
                                    obj.BalanceDue = (data.Total == null ? 0 : data.Total) - ((data.DepositePayment == null ? 0 : data.DepositePayment));
                                    obj.DepositePayment = data.DepositePayment;
                                }


                                if (data.IsCustomerManualPaid == true)
                                {
                                    obj.MethodOfPayment = "Manual";
                                    if (data.IsPaymentbyStripe == true)
                                    {
                                        obj.MethodOfPayment = "Both";
                                    }
                                }
                            }

                            obj.CustomerId = data.CustomerId;
                            obj.CreatedDate = data.CreatedDate;
                            obj.IsDeleted = data.IsDeleted;
                            obj.IsInvoiceReport = data.IsInvoiceReport;
                            obj.ModifyDate = data.ModifyDate;
                            obj.Note = data.Note;
                            obj.PaymentTerms = data.PaymentTerms;
                            obj.Pro_FlowStatus = data.Pro_FlowStatus;
                            obj.Pro_Status = data.Pro_Status;
                            obj.RoleId = data.RoleId;
                            obj.SalesPerson = data.SalesPerson;
                            obj.ShippingCost = data.ShippingCost;
                            obj.Terms = data.Terms;
                            obj.Total = String.Format("{0:0.00}", data.Total);
                            //obj.Type = data.Type;

                            if (data.Type == 1)
                            {
                                obj.StrType = "Invoice";
                            }
                            else
                            {
                                obj.StrType = "Proforma";
                            }
                            obj.UserId = data.UserId;
                            obj.Username = context.Kl_GetCompanyNamr(data.UserId, data.CustomerId).FirstOrDefault();
                            obj.FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == data.CustomerId).Select(s => s.Company_Name).FirstOrDefault();
                            obj.IsCustomer = data.IsCustomer;
                            obj.IsSupplierManualPaid = data.IsSupplierManualPaid;
                            obj.SupplierManualPaidAmount = data.SupplierManualPaidAmount;
                            obj.SupplierManualPaidJVID = data.SupplierManualPaidJVID;
                            obj.IsCustomerManualPaid = data.IsCustomerManualPaid;
                            obj.CustomerManualPaidAmount = data.CustomerManualPaidAmount;
                            obj.CustomerManualPaidJVID = data.CustomerManualPaidJVID;
                            obj.IsStripe = data.IsStripe;
                           
                            obj.StripeJVID = data.StripeJVID;
                            obj.InvoiceJVID = data.InvoiceJVID;
                            objViewModel.Add(obj);
                        }
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountReportingList(userid, typeofinvoice, flow);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.OrderByDescending(s => s.InvoiceDate).ToList();
            }
        }
        public List<ReceivedInvoiceListViewModel> GetReceivedPerformaList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<ReceivedInvoiceListViewModel> objViewModel = new List<ReceivedInvoiceListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 0;

                var dataList = expenseRepository.ReceivedProformaList(userid);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ReceivedInvoiceListViewModel obj = new ReceivedInvoiceListViewModel();

                        //obj.Id = data.Id;
                        //obj.InvoiceDate = data.InvoiceDate;
                        //obj.InvoiceNumber = data.InvoiceNumber;
                        //obj.Username = data.Username;
                        //obj.Total = data.Total;
                        //obj.Pro_Status = data.Pro_Status;
                        //objViewModel.Add(obj);
                        using (var context = new KFentities())
                        {
                            if (data.InvoiceNumber.Length == 1)
                            {
                                data.InvoiceNumber = "0000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "00" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "0" + data.InvoiceNumber;
                            }
                            obj.Id = data.Id;
                            obj.In_R_FlowStatus = data.In_R_FlowStatus;
                            obj.In_R_Status = data.In_R_Status;
                            obj.InvoiceDate = data.InvoiceDate;
                            obj.InvoiceNumber = data.InvoiceNumber;
                            obj.DueDate = data.DueDate;
                            obj.DocumentRef = data.DocumentRef;
                            obj.DepositePayment = data.DepositePayment;
                            obj.BalanceDue = data.BalanceDue;
                            obj.CustomerId = data.CustomerId;
                            obj.CreatedDate = data.CreatedDate;
                            obj.IsDeleted = data.IsDeleted;
                            obj.IsInvoiceReport = data.IsInvoiceReport;
                            obj.ModifyDate = data.ModifyDate;
                            obj.Note = data.Note;
                            obj.PaymentTerms = data.PaymentTerms;
                            obj.Pro_FlowStatus = data.Pro_FlowStatus;
                            obj.Pro_Status = data.Pro_Status;
                            obj.RoleId = data.RoleId;
                            obj.SalesPerson = data.SalesPerson;
                            obj.ShippingCost = data.ShippingCost;
                            obj.Terms = data.Terms;
                            obj.Total = String.Format("{0:0.00}", data.Total);
                            obj.Type = data.Type;
                            obj.UserId = data.UserId;
                            obj.Username = context.Kl_GetCompanyNamr(data.UserId, data.CustomerId).FirstOrDefault();
                            obj.FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == data.CustomerId).Select(s => s.Company_Name).FirstOrDefault();
                            obj.IsCustomer = data.IsCustomer;


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
                            //    result_2 = (DomailApiUrl + "InvoicePdf/" + result_1.Substring(53)).Replace(@"\", "").ToString();

                            //}
                            //obj.PdfViewPath = result_2.TrimEnd('"');



                            objViewModel.Add(obj);

                        }
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountReceivedProformaList(userid);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
        public List<ReceivedInvoiceListViewModel> GetSentInvoiceList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<ReceivedInvoiceListViewModel> objViewModel = new List<ReceivedInvoiceListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 0;

                var dataList = expenseRepository.SentInvoiceList(userid);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ReceivedInvoiceListViewModel obj = new ReceivedInvoiceListViewModel();

                        //obj.Id = data.Id;
                        //obj.InvoiceDate = data.InvoiceDate;
                        //obj.InvoiceNumber = data.InvoiceNumber;
                        //obj.FirstName = data.FirstName;
                        //obj.Total = data.Total;
                        //obj.In_R_FlowStatus = data.In_R_FlowStatus;
                        //objViewModel.Add(obj);
                        using (var context = new KFentities())
                        {
                            if (data.InvoiceNumber.Length == 1)
                            {
                                data.InvoiceNumber = "0000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "00" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "0" + data.InvoiceNumber;
                            }
                            obj.Id = data.Id;
                            obj.In_R_FlowStatus = data.In_R_FlowStatus;
                            obj.In_R_Status = data.In_R_Status;
                            obj.InvoiceDate = data.InvoiceDate;
                            obj.InvoiceNumber = data.InvoiceNumber;
                            obj.DueDate = data.DueDate;
                            obj.DocumentRef = data.DocumentRef;
                            obj.DepositePayment = data.DepositePayment;
                            obj.BalanceDue = data.BalanceDue;
                            obj.CustomerId = data.CustomerId;
                            obj.CreatedDate = data.CreatedDate;
                            obj.IsDeleted = data.IsDeleted;
                            obj.IsInvoiceReport = data.IsInvoiceReport;
                            obj.ModifyDate = data.ModifyDate;
                            obj.Note = data.Note;
                            obj.PaymentTerms = data.PaymentTerms;
                            obj.Pro_FlowStatus = data.Pro_FlowStatus;
                            obj.Pro_Status = data.Pro_Status;
                            obj.RoleId = data.RoleId;
                            obj.SalesPerson = data.SalesPerson;
                            obj.ShippingCost = data.ShippingCost;
                            obj.Terms = data.Terms;
                            obj.Total = String.Format("{0:0.00}", data.Total);
                            obj.Type = data.Type;
                            obj.UserId = data.UserId;
                            obj.Username = context.Kl_GetCompanyNamr(data.UserId, data.CustomerId).FirstOrDefault();
                            obj.FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == data.CustomerId).Select(s => s.Company_Name).FirstOrDefault();
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
                            //    result_2 = (DomailApiUrl + "InvoicePdf/" + result_1.Substring(53)).Replace(@"\", "").ToString();

                            //}
                            //obj.PdfViewPath = result_2.TrimEnd('"');
                            objViewModel.Add(obj);
                        }
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountSentInvoiceList(userid);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }
        public List<ReceivedInvoiceListViewModel> GetSentPerformaList(int? page, int? limit, string sortBy, string direction, string searchString, out int total, int userid)
        {
            using (var expenseRepository = new WebUserRepository())
            {
                var userData = UserData.GetCurrentUserData();
                List<ReceivedInvoiceListViewModel> objViewModel = new List<ReceivedInvoiceListViewModel>();
                //step 1 : Calculate Start Value of page
                int Skip = page.Value * limit.Value - limit.Value;
                int Take = limit.Value;

                // int start = (page.Value - 1) * limit.Value;
                //step 2 : Get data from database and map the properties
                int accountantId = 0;
                String AccountantUnderEmployees = "";
                int EmployeeLoginId = 0;

                var dataList = expenseRepository.SentProformaList(userid);
                if (dataList.Count() > 0)
                {
                    foreach (var data in dataList)
                    {
                        ReceivedInvoiceListViewModel obj = new ReceivedInvoiceListViewModel();

                        //obj.Id = data.Id;
                        //obj.InvoiceDate = data.InvoiceDate;
                        //obj.InvoiceNumber = data.InvoiceNumber;
                        //obj.FirstName = data.FirstName;
                        //obj.Total = data.Total;
                        //obj.In_R_FlowStatus = data.In_R_FlowStatus;
                        //objViewModel.Add(obj);
                        using (var context = new KFentities())
                        {
                            if (data.InvoiceNumber.Length == 1)
                            {
                                data.InvoiceNumber = "0000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "000" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "00" + data.InvoiceNumber;
                            }
                            else if (data.InvoiceNumber.Length == 2)
                            {
                                data.InvoiceNumber = "0" + data.InvoiceNumber;
                            }
                            obj.Id = data.Id;
                            obj.In_R_FlowStatus = data.In_R_FlowStatus;
                            obj.In_R_Status = data.In_R_Status;
                            obj.InvoiceDate = data.InvoiceDate;
                            obj.InvoiceNumber = data.InvoiceNumber;
                            obj.DueDate = data.DueDate;
                            obj.DocumentRef = data.DocumentRef;
                            obj.DepositePayment = data.DepositePayment;
                            obj.BalanceDue = data.BalanceDue;
                            obj.CustomerId = data.CustomerId;
                            obj.CreatedDate = data.CreatedDate;
                            obj.IsDeleted = data.IsDeleted;
                            obj.IsInvoiceReport = data.IsInvoiceReport;
                            obj.ModifyDate = data.ModifyDate;
                            obj.Note = data.Note;
                            obj.PaymentTerms = data.PaymentTerms;
                            obj.Pro_FlowStatus = data.Pro_FlowStatus;
                            obj.Pro_Status = data.Pro_Status;
                            obj.RoleId = data.RoleId;
                            obj.SalesPerson = data.SalesPerson;
                            obj.ShippingCost = data.ShippingCost;
                            obj.Terms = data.Terms;
                            obj.Total = String.Format("{0:0.00}", data.Total);
                            obj.Type = data.Type;
                            obj.UserId = data.UserId;
                            obj.Username = context.Kl_GetCompanyNamr(data.UserId, data.CustomerId).FirstOrDefault();
                            obj.FirstName = context.tblCustomerOrSuppliers.Where(i => i.Id == data.CustomerId).Select(s => s.Company_Name).FirstOrDefault();
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
                            //    result_2 = (DomailApiUrl + "InvoicePdf/" + result_1.Substring(53)).Replace(@"\", "").ToString();

                            //}
                            //obj.PdfViewPath = result_2.TrimEnd('"');
                            objViewModel.Add(obj);
                        }
                    }
                }

                if (objViewModel.Count > 0)
                {
                    total = expenseRepository.CountSentProformaList(userid);
                    //total = expenseRepository.GetActiveUserCount(userData.Id, firstName);
                }
                else
                {
                    total = 0;
                }
                // total = expenseRepository.TotalExpenseCount(UserId);

                var records = objViewModel.AsQueryable();



                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        records = SortHelper.OrderBy(records, sortBy);
                    }
                    else
                    {
                        records = SortHelper.OrderByDescending(records, sortBy);
                    }
                }

                return records.ToList();
            }
        }

        #endregion
       
    }
}