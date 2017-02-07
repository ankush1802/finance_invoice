using AutoMapper;
using KF.Dto.Modules.Finance;
using KF.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KF.Web.Models
{
    public static class ClsDropDownList
    {
        #region Finance

        public static List<AccountantCustomers> GetAccountantCustomerList(Boolean IsUpdate, int? subAccountantId)
        {
            try
            {
                using (var db = new KFentities())
                {
                    var getCurrentUserData = UserData.GetCurrentUserData();
                    List<AccountantCustomers> AccountantCustomerList = new List<AccountantCustomers>();
                    var CustomerList = db.UserRegistrations.Where(cond => cond.AccountantId == getCurrentUserData.Id && cond.IsDeleted == false && cond.RoleId == 3)
                        .Select(c => new
                        {
                            c.AccountantId,
                            c.Id,
                            c.FirstName,
                            c.LastName,
                            c.CompanyName
                        }).ToList();

                    AccountantCustomerList = CustomerList.Select(x => new AccountantCustomers()
                    {
                        CustomerId = x.Id,
                        CustomerName = x.FirstName + " " + x.LastName + " -" + x.CompanyName,
                        IsChecked = !IsUpdate ? false : db.SubAccountantUserPermissions.Where(d => d.UserWithAnAccountantId == x.Id &&
                            d.AccountantId == getCurrentUserData.Id &&
                            d.SubAccountantId == subAccountantId && d.IsDeleted == false).Any()
                    }).ToList();
                    return AccountantCustomerList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Create Classification


        public static List<SelectListItem> SPopulateCategory()
        {
            try
            {
                using (var context = new KFentities())
                {
                    var Objbank = context.Classifications.Where(d => d.Type == "S").Select(s => new { SName = s.ClassificationType + "-" + s.ChartAccountDisplayNumber, SValues = s.ChartAccountDisplayNumber.Substring(0, 4) }).ToList();
                    var Banks = new List<SelectListItem>();
                    Banks = Objbank.Select(x => new SelectListItem { Text = x.SName, Value = Convert.ToString(x.SValues) }).ToList();
                    return Banks;
                }
            }
            catch
            {
                return null;
            }

        }

        public static List<SelectListItem> PopulateClassificationType()
        {
            try
            {
                using (var context = new KFentities())
                {

                    var Objbank = context.ClassificationTypes.ToList();
                    var Banks = new List<SelectListItem>();
                    Banks = Objbank.Select(x => new SelectListItem { Text = x.ClassificationType1, Value = Convert.ToString(x.Id) }).ToList();
                    return Banks;
                }
            }
            catch
            {
                return null;

            }

        }

        public static List<SelectListItem> T1PopulateCategory(int SelectedActiveUser)
        {
            try
            {
                using (KFentities dB = new KFentities())
                {
                    var Objbank = dB.sp_TType(SelectedActiveUser).ToList();
                    var Banks = new List<SelectListItem>();
                    Banks = Objbank.Select(x => new SelectListItem { Text = x.T1Name, Value = Convert.ToString(x.T1Values) }).ToList();
                    return Banks;
                }
            }
            catch
            {
                return null;
            }
        }
        public static List<SelectListItem> T2PopulateCategory(int SelectedActiveUser)
        {
            try
            {
                using (var context = new KFentities())
                {
                    var Objbank = context.sp_GType(SelectedActiveUser).ToList();
                    var Banks = new List<SelectListItem>();
                    Banks = Objbank.Select(x => new SelectListItem { Text = x.T2Name, Value = Convert.ToString(x.T2Values) }).ToList();
                    return Banks;
                }
            }
            catch
            {
                return null;

            }
        }
        public static List<SelectListItem> ZPopulateCategory()
        {

            try
            {
                using (var context = new KFentities())
                {

                    var Objbank = context.Classifications.Where(d => d.Type == "Z").Select(s => new { ZName = s.ClassificationType + "-" + s.ChartAccountDisplayNumber, ZValues = s.ChartAccountDisplayNumber.Substring(0, 4) }).ToList();
                    var Banks = new List<SelectListItem>();
                    Banks = Objbank.Select(x => new SelectListItem { Text = x.ZName, Value = Convert.ToString(x.ZValues) }).ToList();
                    return Banks;
                }
            }
            catch
            {
                return null;

            }

        }
        #endregion

        public static UserRegistrationDto GetUserDetailsByStatementId(int statementID)
        {
            try
            {
                using (var db = new KFentities())
                {
                    Int32 SelectedUserId = 0;
                    var getCurrentUserData = UserData.GetCurrentUserData();
                    if (getCurrentUserData.RoleId != 2)
                    {
                        if (HttpContext.Current.Request.Cookies["SelectedActiveUser"] != null)
                        {
                            if (HttpContext.Current.Request.Cookies["SelectedActiveUser"].Value != null)
                            {
                                int.TryParse(Convert.ToString(HttpContext.Current.Request.Cookies["SelectedActiveUser"].Value), out SelectedUserId);
                            }

                        }
                    }
                    else
                    {
                        SelectedUserId = getCurrentUserData.Id;
                    }
                    var data = db.BankExpenses.Where(s => s.Id == statementID).Select(d => d.UserRegistration).FirstOrDefault();

                    if (SelectedUserId != data.Id)
                    {
                        return null;
                    }
                    else
                    {
                        Mapper.CreateMap<UserRegistration, UserRegistrationDto>();
                        return Mapper.Map<UserRegistrationDto>(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static CatNClassViewModel GetCategoryById(int id)
        {
            try
            {
                using (var dbContext = new KFentities())
                {

                    var data = dbContext.Classifications.Where(i => i.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        var categoriesDetails = dbContext.Categories.Where(u => u.Id == data.CategoryId).FirstOrDefault();
                        if (data != null)
                        {
                            Mapper.CreateMap<Category, CatNClassViewModel>();
                            var dataDto = Mapper.Map<CatNClassViewModel>(categoriesDetails);
                            dataDto.ClassificationDesc = data.Desc;
                            dataDto.ClassificationChartNumber = data.ChartAccountDisplayNumber;
                            return dataDto;
                        }
                        else
                            throw new Exception();
                    }
                    else
                        throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Default Classification ID for MJV when page loads
        /// </summary>
        /// <returns></returns>
        public static Int32 GetDefaultClassificationForMJVentries()
        {
            using (var context = new KFentities())
            {
                Int32 ClassificationID = 1;
                ClassificationID = context.Classifications.Where(s => s.ClassificationType == "Cash Withdrawn").Select(f => f.Id).FirstOrDefault();
                return ClassificationID;
            }
        }
        /// <summary>
        /// For mannual entry screen
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IEnumerable<GroupedSelectListItem> PopulateCustomClassificationForManualEntry(int userId)
        {
            using (var context = new KFentities())
            {
                List<CustomClassificationDto> ClassificationList = new List<CustomClassificationDto>();
                var userData = context.UserRegistrations.Where(s => s.Id == userId).FirstOrDefault();
                List<Classification> objList = new List<Classification>();
                switch (userData.OwnershipId)
                {
                    case 1:
                        objList = context.Classifications.Where(i => i.IsSole == true && i.IndustryId == null).ToList();
                        break;
                    case 2:
                        objList = context.Classifications.Where(i => i.IsIncorporated == true && i.IndustryId == null).ToList();
                        break;
                    case 3:
                        objList = context.Classifications.Where(i => i.IsPartnerShip == true && i.IndustryId == null).ToList();
                        break;
                    default:
                        objList = context.Classifications.Where(i => i.IsPartnerShip == true && i.IsDeleted == false && i.IndustryId == null).ToList();
                        break;
                }
                var industryClassification = context.Classifications.Where(i => i.IndustryId == userData.IndustryId).ToList();
                if (userData.SubIndustryId != null)
                {
                    objList.AddRange(industryClassification.Where(s => s.SubIndustryId == userData.SubIndustryId).ToList());
                }
                objList.AddRange(context.Classifications.Where(i => i.UserId == userId).ToList());
                objList = objList.Where(d => d.Type.Trim() != "H" && d.Type.Trim() != "S" && d.Type.Trim() != "T" && d.Type.Trim() != "Z").ToList();
                objList = objList.Where(s => s.IsDeleted == false).ToList();

                var reorderedClassificationList = new List<Classification>();
                reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 1).ToList()); //Asset
                reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 4).ToList()); //Liability
                reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 5).ToList()); //Equity
                reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 3).ToList()); //Revenue
                reorderedClassificationList.AddRange(objList.Where(d => d.CategoryId == 2).ToList()); //Expense

                foreach (var data in reorderedClassificationList)
                {
                    var classification = new CustomClassificationDto();
                    classification.Id = data.Id;
                    classification.ClassificationType = data.ClassificationType;
                    classification.Category = context.Categories.Where(s => s.Id == data.CategoryId).Select(c => c.CategoryType).FirstOrDefault();
                    classification.CategoryId = data.CategoryId;
                    ClassificationList.Add(classification);
                }

                var CustomClassificationList = new List<GroupedSelectListItem>();
                CustomClassificationList = ClassificationList.Select(x => new GroupedSelectListItem() { Value = x.Id.ToString(), Text = x.ClassificationType = x.ClassificationType, GroupName = x.Category, GroupKey = x.CategoryId.ToString() }).ToList();
                return CustomClassificationList;
            }
        }

        /// <summary>
        /// User list by accountant id for bank section 
        /// </summary>
        /// <param name="AccountantId"></param>
        /// <returns></returns>
        public static List<SelectListItem> PopulateUser(int AccountantId)
        {
            using (var context = new KFentities())
            {
                var ObjYear = context.UserRegistrations.Where(i => i.AccountantId == AccountantId && (i.IsUnlink == false || i.IsUnlink == null)
                        && i.IsDeleted == false && i.RoleId != 4 && i.IsPaid == true).Select(d => new { Username = d.Username, Id = d.Id }).ToList();
                var Year = new List<SelectListItem>();
                Year = ObjYear.Select(x => new SelectListItem { Text = x.Username, Value = Convert.ToString(x.Id) }).ToList();
                return Year;
            }
        }

        //Get bank list user specified for statement section
        public static List<SelectListItem> PopulateUserBank(int userId)
        {
            using (var context = new KFentities())
            {
                var Objbank = context.BankExpenses.Where(s => s.UserId == userId).Select(d => d.Bank).Distinct().ToList();
                var Banks = new List<SelectListItem>();
                Banks = Objbank.Select(x => new SelectListItem { Text = x.BankName, Value = Convert.ToString(x.Id) }).ToList();
                return Banks;
            }
        }
        /// <summary>
        /// Get Status list for statement section
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateStatus()
        {
            using (var context = new KFentities())
            {
                var Objbank = context.Status.ToList();
                var Banks = new List<SelectListItem>();
                Banks = Objbank.Select(x => new SelectListItem { Text = x.StatusType, Value = Convert.ToString(x.Id) }).ToList();
                return Banks;
            }
        }
        /// <summary>
        /// Get Category List for statement list section
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateCategory()
        {
            using (var context = new KFentities())
            {
                var ObjList = context.Categories.Where(a => a.Id != 6).ToList();
                var Categories = new List<SelectListItem>();
                Categories = ObjList.Select(x => new SelectListItem { Text = x.CategoryType, Value = Convert.ToString(x.Id) }).ToList();
                return Categories;
            }
        }
        public static List<SelectListItem> PopulateUser(int AccountantId, bool isAdmin)
        {
            using (var context = new KFentities())
            {
                if (isAdmin)
                {
                    var userList = context.UserRegistrations.Where(i => i.AccountantId == AccountantId && (i.IsUnlink == false || i.IsUnlink == null) && i.IsDeleted == false && i.RoleId != 4).ToList();
                    List<UserRegistration> objList = new List<UserRegistration>();
                    foreach (var item in userList)
                    {
                        if (item.IsPaid == null || item.IsPaid == false)
                        {
                            DateTime UserModifiedDate = Convert.ToDateTime(item.ModifiedDate);
                            DateTime CurrentDate = DateTime.Now;
                            var days = (CurrentDate - UserModifiedDate).TotalDays;
                            if (days > 1)
                            {
                                // userList.Remove(item);
                            }
                            else
                            {
                                objList.Add(item);
                            }
                        }
                        else
                        {
                            objList.Add(item);
                        }
                    }
                    var UserList = new List<SelectListItem>();
                    UserList = objList.Select(x => new SelectListItem { Text = x.Username, Value = Convert.ToString(x.Id) }).ToList();
                    return UserList;
                }
                else
                {
                    var userList = context.UserRegistrations.Where(i => i.Id == AccountantId && (i.IsUnlink == false || i.IsUnlink == null) && i.IsDeleted == false).ToList();
                    List<UserRegistration> objList = new List<UserRegistration>();
                    foreach (var item in userList)
                    {
                        if (item.IsPaid == null || item.IsPaid == false)
                        {
                            DateTime UserModifiedDate = Convert.ToDateTime(item.ModifiedDate);
                            DateTime CurrentDate = DateTime.Now;
                            var days = (CurrentDate - UserModifiedDate).TotalDays;
                            if (days > 1)
                            {
                                // userList.Remove(item);
                            }
                            else
                            {
                                objList.Add(item);
                            }
                        }
                        else
                        {
                            objList.Add(item);
                        }
                    }
                    var UserList = new List<SelectListItem>();
                    UserList = objList.Select(x => new SelectListItem { Text = x.Username, Value = Convert.ToString(x.Id) }).ToList();
                    return UserList;
                }

            }

        }

        public static List<SelectListItem> PopulateUserBank(int userId, bool isAdmin)
        {
            using (var context = new KFentities())
            {
                List<Bank> objBankList = new List<Bank>();
                if (!isAdmin)
                {
                    objBankList = (from i in context.Banks
                                   join
                                       j in context.BankExpenses on i.Id equals j.BankId
                                   where j.UserId == userId
                                   select i).OrderBy(s => s.Id).Distinct().ToList();
                }
                else
                {
                    objBankList = context.Banks.Take(5).ToList();
                }
                var Banks = new List<SelectListItem>();
                Banks = objBankList.Select(x => new SelectListItem { Text = x.BankName, Value = Convert.ToString(x.Id) }).ToList();
                return Banks;
            }
        }


        public static UserRegistrationDto GetUserByEmail(string email)
        {
            try
            {
                using (var db = new KFentities())
                {
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


        /// <summary>
        /// Get province List
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateProvince()
        {
            using (var context = new KFentities())
            {
                var ObjProvince = context.Provinces.ToList();
                var Province = new List<SelectListItem>();
                Province = ObjProvince.Select(x => new SelectListItem { Text = x.ProvinceName, Value = Convert.ToString(x.Id) }).ToList();
                return Province;
            }

        }

        /// <summary>
        /// Get month list
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateMonth()
        {
            using (var context = new KFentities())
            {
                var ObjMonth = context.tblMonths.ToList();
                var Month = new List<SelectListItem>();
                Month = ObjMonth.Select(x => new SelectListItem { Text = x.Month, Value = Convert.ToString(x.Id) }).ToList();
                return Month;
            }

        }

        /// <summary>
        /// Get Year List
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateYear()
        {
            using (var context = new KFentities())
            {
                var ObjYear = context.tblYears.ToList();
                var Year = new List<SelectListItem>();
                Year = ObjYear.Select(x => new SelectListItem { Text = Convert.ToString(x.Year), Value = Convert.ToString(x.Id) }).ToList();
                return Year;
            }

        }

        /// <summary>
        /// Get List of Industries
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> PopulateIndustry()
        {
            using (var context = new KFentities())
            {
                var ObjIndustry = context.Industries.ToList();
                var Industry = new List<SelectListItem>();
                Industry = ObjIndustry.Select(x => new SelectListItem { Text = x.IndustryType, Value = Convert.ToString(x.Id) }).ToList();
                return Industry;
            }
        }

        public static List<SelectListItem> PopulateOwnership()
        {
            using (var context = new KFentities())
            {
                var ObjOwnership = context.Ownerships.ToList();
                var Ownership = new List<SelectListItem>();
                Ownership = ObjOwnership.Select(x => new SelectListItem { Text = x.OwnershipType, Value = Convert.ToString(x.Id) }).ToList();
                return Ownership;
            }

        }

        /// <summary>
        /// Get Sub industry list based on industry id
        /// </summary>
        /// <param name="industryId"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetSubIndustryListById(int industryId)
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

        /// <summary>
        /// Get Province details by name
        /// </summary>
        /// <param name="provincename"></param>
        /// <returns></returns>
        public static Province GetProvinceByName(string provincename)
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

        /// <summary>
        /// Get Ownership details by name
        /// </summary>
        /// <param name="ownershipName"></param>
        /// <returns></returns>
        public static Ownership GetOwnershipByName(string ownershipName)
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

        /// <summary>
        /// Get industry detail by name
        /// </summary>
        /// <param name="Industryname"></param>
        /// <returns></returns>
        public static Industry GetIndustryByName(string Industryname)
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

        /// <summary>
        /// Get Sub industry details on name
        /// </summary>
        /// <param name="IndustryId"></param>
        /// <param name="SubIndustryName"></param>
        /// <returns></returns>
        public static SubIndustry GetSubIndustryByName(int IndustryId, string SubIndustryName)
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
        #endregion


        #region Invoice
        public static List<SelectListItem> PopulateTypeOfInvoices()
        {
            var Objbank = GetAllTypeOfInvoice();
            var Banks = new List<SelectListItem>();
            Banks = Objbank.Select(x => new SelectListItem { Text = x.InvoiceType, Value = Convert.ToString(x.InvoiceType) }).ToList();
            return Banks;
        }
        public static List<tblTypeOfInvoice> GetAllTypeOfInvoice()
        {
            try
            {
                using (var context = new KFentities())
                {
                    return context.tblTypeOfInvoices.ToList();
                }
            }
            catch
            {
                return null;

            }

        }

        public static List<SelectListItem> PopulateFlowStatus()
        {
            var Objbank = GetAllFlowsStatus();
            var Banks = new List<SelectListItem>();
            Banks = Objbank.Select(x => new SelectListItem { Text = x.FlowStatus, Value = Convert.ToString(x.FlowStatus) }).ToList();
            return Banks;
        }
        public static List<tblFlowStatu> GetAllFlowsStatus()
        {
            try
            {
                using (var context = new KFentities())
                {
                    return context.tblFlowStatus.ToList();
                }
            }
            catch
            {
                return null;

            }

        }

        public static List<SelectListItem> PopulateServiceProductType(int UserId, int num)
        {
            var ObjYear = GetServiceProductType(UserId, num);
            var Year = new List<SelectListItem>();
            Year = ObjYear.Select(x => new SelectListItem { Text = x.Name, Value = Convert.ToString(x.Id) }).ToList();
            return Year;
        }
        public static List<Classification> GetServiceProductType(int UserId, int num)
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

        }

        public static List<SelectListItem> PopulateCurrency_Guri()
        {
            var Objbank = GetPopulateCurrency().Select(d => new { SymbolCode = d.SymbolCode }).Distinct();
            var Banks = new List<SelectListItem>();
            Banks = Objbank.Select(x => new SelectListItem { Text = x.SymbolCode, Value = Convert.ToString(x.SymbolCode) }).ToList();
            return Banks;
        }

        public static List<tblCurrency> GetPopulateCurrency()
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




    }
}