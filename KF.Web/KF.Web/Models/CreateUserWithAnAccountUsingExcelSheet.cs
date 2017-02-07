using AutoMapper;
using KF.Dto.Modules.Finance;
using KF.Entity;
using KF.Repo.Modules.Finance;
using KF.Utilities.Common;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KF.Web.Models
{
    public static class CreateUserWithAnAccountUsingExcelSheet
    {
        public static byte[] ReadExcelFile(HttpPostedFileBase _file)
        {
            var successUsersList = new List<UserWithAnAccountantViewModel>();
            var failureUsersList = new List<UserWithAnAccountantViewModel>();
            var userdata = UserData.GetCurrentUserData();
            // AccountRepository _userrepository = new AccountRepository();
            try
            {
                //if (Request != null)
                //{

                    if ((_file != null) && (_file.ContentLength > 0) && !string.IsNullOrEmpty(_file.FileName))
                    {
                        string fileName = _file.FileName;
                        string fileContentType = _file.ContentType;
                        byte[] fileBytes = new byte[_file.ContentLength];
                        var data = _file.InputStream.Read(fileBytes, 0, Convert.ToInt32(_file.ContentLength));
                        var fileDetails = new FileInfo(fileName);

                        using (var package = new ExcelPackage(_file.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (int rowIterator = 3; rowIterator <= noOfRow; rowIterator++)
                            {
                                if (workSheet.Cells[rowIterator, 1].Value == null && workSheet.Cells[rowIterator, 2].Value == null && workSheet.Cells[rowIterator, 3].Value == null
                                    && workSheet.Cells[rowIterator, 4].Value == null && workSheet.Cells[rowIterator, 5].Value == null && workSheet.Cells[rowIterator, 6].Value == null
                                    && workSheet.Cells[rowIterator, 7].Value == null && workSheet.Cells[rowIterator, 8].Value == null && workSheet.Cells[rowIterator, 9].Value == null
                                    && workSheet.Cells[rowIterator, 10].Value == null && workSheet.Cells[rowIterator, 11].Value == null && workSheet.Cells[rowIterator, 12].Value == null
                                    && workSheet.Cells[rowIterator, 13].Value == null && workSheet.Cells[rowIterator, 14].Value == null && workSheet.Cells[rowIterator, 15].Value == null)
                                { }
                                else
                                {
                                    var user = new UserWithAnAccountantViewModel();
                                    if (workSheet.Cells[rowIterator, 1].Value != null)
                                    {
                                        var _userdata = ClsDropDownList.GetUserByEmail(workSheet.Cells[rowIterator, 1].Value.ToString());
                                        if (_userdata == null)
                                        {
                                            user.Email = workSheet.Cells[rowIterator, 1].Value.ToString();
                                        }
                                        else
                                        {
                                            user.Email = "Email already exists !";
                                        }
                                    }
                                    else
                                    {
                                        user.Email = "Email is required !";
                                    }

                                    //user.Email = workSheet.Cells[rowIterator, 1].Value != null ? workSheet.Cells[rowIterator, 1].Value.ToString() : "Email is required !";
                                    user.FirstName = workSheet.Cells[rowIterator, 2].Value != null ? workSheet.Cells[rowIterator, 2].Value.ToString() : "FirstName is required !";
                                    user.LastName = workSheet.Cells[rowIterator, 3].Value != null ? workSheet.Cells[rowIterator, 3].Value.ToString() : "LastName is required !";
                                    user.MobileNumber = workSheet.Cells[rowIterator, 4].Value != null ? workSheet.Cells[rowIterator, 4].Value.ToString() : "MobileNumber is required !";

                                    if (user.MobileNumber != "MobileNumber is required !")
                                    {
                                        if (!IsValidMobileNumber(user.MobileNumber))
                                        {
                                            user.MobileNumber = "Mobile Number is invalid !";
                                        }
                                    }

                                    //if (workSheet.Cells[rowIterator, 5].Value != null)
                                    //{
                                    //    var country = _userrepository.GetCountryByName(workSheet.Cells[rowIterator, 5].Value.ToString());
                                    //    user.CountryId = country != null ? country.Id : 1;
                                    //}
                                    //else
                                    //{
                                    //    user.CountryId = 0;
                                    //}
                                    // user.CountryId = workSheet.Cells[rowIterator, 5].Value != null ? Convert.ToInt32(workSheet.Cells[rowIterator, 5].Value) : 0;
                                    if (workSheet.Cells[rowIterator, 5].Value != null)
                                    {
                                        var province = ClsDropDownList.GetProvinceByName(workSheet.Cells[rowIterator, 5].Value.ToString());
                                        user.ProvinceId = province != null ? province.Id : 0;
                                        user.Province = user.ProvinceId == 0 ? "Province is invalid !" : province.ProvinceName;
                                    }
                                    else
                                    {
                                        user.Province = "Province is invalid !";
                                    }
                                    // user.Province = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "Province is required !";
                                    user.City = workSheet.Cells[rowIterator, 6].Value != null ? workSheet.Cells[rowIterator, 6].Value.ToString() : "City is required !";
                                    user.PostalCode = workSheet.Cells[rowIterator, 7].Value != null ? workSheet.Cells[rowIterator, 7].Value.ToString() : "PostalCode is required !";
                                    if (user.PostalCode.Length != 6 && user.PostalCode != "PostalCode is required !")
                                    {
                                        user.PostalCode = "Postal Code must be 6 characters long !";
                                    }
                                    user.CorporationAddress = workSheet.Cells[rowIterator, 8].Value != null ? workSheet.Cells[rowIterator, 8].Value.ToString() : "CorporationAddress is required !";
                                    user.GSTNumber = workSheet.Cells[rowIterator, 9].Value != null ? workSheet.Cells[rowIterator, 9].Value.ToString() : "GSTNumber is required !";
                                    user.BusinessNumber = workSheet.Cells[rowIterator, 10].Value != null ? workSheet.Cells[rowIterator, 10].Value.ToString() : "BusinessNumber is required !";
                                    user.TaxStartDay = workSheet.Cells[rowIterator, 11].Value != null ? workSheet.Cells[rowIterator, 11].Value.ToString() : "TaxStartDay is required !";
                                    user.TaxStartMonth = workSheet.Cells[rowIterator, 12].Value != null ? workSheet.Cells[rowIterator, 12].Value.ToString() : "TaxStartMonth is required !";
                                    if (user.TaxStartMonth != "TaxStartMonth is required !")
                                    {
                                        if (!IsAllDigits(user.TaxStartMonth))
                                        {
                                            user.TaxStartMonth = "TaxStartMonth is invalid !";
                                        }
                                        else
                                        {
                                            user.TaxEndMonth = user.TaxStartMonth;
                                        }
                                    }

                                    user.TaxStartYear = workSheet.Cells[rowIterator, 13].Value != null ? workSheet.Cells[rowIterator, 13].Value.ToString() : "TaxStartYear is required !";
                                    if (user.TaxStartYear != "TaxStartYear is required !")
                                    {
                                        if (!IsAllDigits(user.TaxStartYear))
                                        {
                                            user.TaxStartYear = "TaxStartYear is invalid !";
                                        }
                                        else
                                        {
                                            user.TaxEndYear = Convert.ToString(Convert.ToInt32(user.TaxStartYear) + 1);
                                        }
                                    }
                                    int ownershipId = 0;
                                    if (workSheet.Cells[rowIterator, 14].Value != null)
                                    {
                                        var ownership = ClsDropDownList.GetOwnershipByName(workSheet.Cells[rowIterator, 14].Value.ToString());
                                        user.OwnershipId = ownership != null ? ownership.Id : 0;
                                        user.OwnershipName = user.OwnershipId == 0 ? "Invalid Ownership Name" : ownership.OwnershipType;
                                        ownershipId = user.OwnershipId;
                                    }
                                    int industryId = 0;
                                    string industryName = string.Empty;
                                    if (workSheet.Cells[rowIterator, 15].Value != null)
                                    {
                                        var industry = ClsDropDownList.GetIndustryByName(workSheet.Cells[rowIterator, 15].Value.ToString());
                                        user.IndustryId = industry != null ? industry.Id : 0;
                                        user.Industryname = user.IndustryId == 0 ? "Invalid Industry Name" : industry.IndustryType;
                                        industryId = user.IndustryId;
                                        industryName = industry.IndustryType;
                                    }
                                    if (industryName == "Other")
                                    {
                                        workSheet.Cells[rowIterator, 16].Value = "Other";
                                    }

                                    if (workSheet.Cells[rowIterator, 16].Value != null)
                                    {
                                        var industry = ClsDropDownList.GetSubIndustryByName(industryId, workSheet.Cells[rowIterator, 16].Value.ToString());
                                        user.SubIndustryId = industry != null ? industry.Id : 0;

                                        user.SubIndustryName = industry == null ? "Invalid Sub-Industry Name" : industry.SubIndustryName;
                                    }


                                    //else
                                    //{
                                    //    user.IndustryId = 0;
                                    //}


                                    if (
                                        user.Email != "Email is required !" && user.Email != "Email already exists !" && user.FirstName != "FirstName is required !" &&
                                        user.LastName != "LastName is required !" && user.MobileNumber != "MobileNumber is required !" && user.MobileNumber != "Mobile Number is invalid !" &&
                                        user.Province != "Province is required !" && user.Province != "Province is invalid !" &&
                                        user.City != "City is required !" && user.PostalCode != "PostalCode is required !" && user.PostalCode != "Postal Code must be 6 characters long !" &&
                                        user.CorporationAddress != "CorporationAddress is required !"
                                        && user.GSTNumber != "GSTNumber is required !" && user.GSTNumber != "GSTNumber is invalid !" &&
                                        user.BusinessNumber != "BusinessNumber is required !" && user.BusinessNumber != "BusinessNumber is invalid !"
                                        && user.TaxStartMonth != "TaxStartMonth is required !" && user.TaxStartMonth != "TaxStartMonth is invalid !"
                                        && user.TaxStartYear != "TaxStartYear is required !" && user.TaxStartYear != "TaxStartYear is invalid !"
                                       && user.OwnershipId != 0 && user.IndustryId != 0 && user.SubIndustryId != 0
                                        )
                                    {
                                        user.TaxStartMonthId = Convert.ToByte(user.TaxStartMonth);
                                        user.TaxEndMonthId = Convert.ToByte(user.TaxEndMonth);
                                        successUsersList.Add(user);
                                    }
                                    else
                                    {
                                        failureUsersList.Add(user);
                                    }
                                }
                            }


                            if (successUsersList.Count() >= 1)
                            {
                                using (var userRepository = new AccountRepository())
                                {
                                    successUsersList.ForEach(y => y.TaxationStartDay = Convert.ToInt32(y.TaxStartDay));
                                    successUsersList.ForEach(y => y.TaxationEndDay = Convert.ToInt32(y.TaxStartDay));
                                    Mapper.CreateMap<UserWithAnAccountantViewModel, UserRegistrationDto>();
                                    var userDetails = Mapper.Map<List<UserRegistrationDto>>(successUsersList);
                                    foreach (var item in userDetails)
                                    {
                                        item.IsUnlink = false;
                                        item.RoleId = 3;
                                        item.SectorId = 2;
                                        item.CountryId = 1;
                                        item.CurrencyId = 1;//By default CAD for canada users
                                        item.AccountantId = userdata.Id;// Accountant id who create user
                                        item.PrivateKey = RandomString.GetUniqueKey();
                                        item.IndustryId = item.IndustryId != 0 ? item.IndustryId : 16;//By default set to OTHERS
                                        item.SubIndustryId = item.SubIndustryId != 0 ? item.SubIndustryId : 109;
                                        userRepository.AddUser(item);
                                    }
                                }
                            }
                            return ExcelFile(failureUsersList, successUsersList);
                        }
                    }
                    else
                        throw new ArgumentNullException();
                //}
                //else
                //    throw new Exception();
            }
            catch (Exception)
            {
                using (var package = new ExcelPackage())
                {
                    var user = new UserWithAnAccountantViewModel();
                    user.Email = "Email is required !";
                    user.FirstName = "FirstName is required !";
                    user.LastName = "LastName is required !";
                    user.MobileNumber = "MobileNumber is required !";
                    user.MobileNumber = "Mobile Number is invalid !";
                    //  user.CountryId = 0;
                    user.Province = "Province is required !";
                    user.City = "City is required !";
                    user.PostalCode = "PostalCode is required !";
                    user.PostalCode = "Postal Code must be 6 characters long !";
                    user.CorporationAddress = "CorporationAddress is required !";
                    user.GSTNumber = "GSTNumber is required !";
                    user.GSTNumber = "GSTNumber is invalid !";
                    user.BusinessNumber = "BusinessNumber is required !";
                    user.BusinessNumber = "BusinessNumber is invalid !";
                    user.TaxStartDay = "TaxStartDay is required !";
                    user.TaxStartMonth = "TaxStartMonth is required !";
                    user.TaxStartYear = "TaxStartYear is required !";
                    user.Industryname = "Industry is required !";
                    failureUsersList.Add(user);
                    return ExcelFile(failureUsersList, successUsersList);
                }
            }
        }


        public static byte[] ExcelFile(List<UserWithAnAccountantViewModel> failedUserlist, List<UserWithAnAccountantViewModel> successUserlist)
        {
            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ResponseExcel - " + DateTime.Now.ToShortDateString());

                worksheet.Cells[1, 1].Value = "Email";
                worksheet.Cells[1, 2].Value = "FirstName";
                worksheet.Cells[1, 3].Value = "LastName";
                worksheet.Cells[1, 4].Value = "MobileNumber";
                worksheet.Cells[1, 5].Value = "Province";
                worksheet.Cells[1, 6].Value = "City";
                worksheet.Cells[1, 7].Value = "PostalCode";
                worksheet.Cells[1, 8].Value = "CorporationAddress";
                worksheet.Cells[1, 9].Value = "GST/HST Number";
                worksheet.Cells[1, 10].Value = "BusinessNumber";
                worksheet.Cells[1, 11].Value = "TaxStartDay";
                worksheet.Cells[1, 12].Value = "TaxStartMonth";
                worksheet.Cells[1, 13].Value = "TaxStartYear";
                worksheet.Cells[1, 14].Value = "Ownership";
                worksheet.Cells[1, 15].Value = "Industry";
                worksheet.Cells[1, 16].Value = "Sub-Industry";
                worksheet.Cells[1, 17].Value = "Response";

                var row = 0;
                List<UserWithAnAccountantViewModel> userlist;
                int rowIterator = 2;
                for (int i = 0; i < 2; i++)
                {
                    var j = 0;
                    row = i == 0 ? failedUserlist.Count() : rowIterator + successUserlist.Count() - 2;
                    userlist = i == 0 ? failedUserlist : successUserlist;
                    //var successRow = successUserlist.Count();
                    for (int k = rowIterator; rowIterator <= row + 1; rowIterator++)
                    {
                        worksheet.Cells[rowIterator, 1].Value = userlist[j].Email;
                        worksheet.Cells[rowIterator, 2].Value = userlist[j].FirstName;
                        worksheet.Cells[rowIterator, 3].Value = userlist[j].LastName;
                        worksheet.Cells[rowIterator, 4].Value = userlist[j].MobileNumber;
                        // worksheet.Cells[rowIterator, 5].Value = userlist[j].CountryId == 0 ? "Country is required !" : userlist[j].CountryId.ToString();
                        worksheet.Cells[rowIterator, 5].Value = userlist[j].Province == "Province is invalid !" ? "Province is invalid !" : userlist[j].Province.ToString();
                        worksheet.Cells[rowIterator, 6].Value = userlist[j].City;
                        worksheet.Cells[rowIterator, 7].Value = userlist[j].PostalCode;
                        worksheet.Cells[rowIterator, 8].Value = userlist[j].CorporationAddress;
                        worksheet.Cells[rowIterator, 9].Value = userlist[j].GSTNumber;
                        worksheet.Cells[rowIterator, 10].Value = userlist[j].BusinessNumber;
                        worksheet.Cells[rowIterator, 11].Value = userlist[j].TaxStartDay;
                        worksheet.Cells[rowIterator, 12].Value = userlist[j].TaxStartMonth;
                        worksheet.Cells[rowIterator, 13].Value = userlist[j].TaxStartYear;
                        worksheet.Cells[rowIterator, 14].Value = userlist[j].OwnershipName == "Invalid Ownership Name" ? "Invalid Ownership Name!" : userlist[j].OwnershipName.ToString();
                        worksheet.Cells[rowIterator, 15].Value = userlist[j].Industryname == "Invalid Industry Name" ? "Invalid Industry Name!" : userlist[j].Industryname.ToString();
                        worksheet.Cells[rowIterator, 16].Value = userlist[j].SubIndustryName == "Invalid Sub-Industry Name" ? "Please select the valid sub industry" : userlist[j].SubIndustryName.ToString();
                        if (i == 0)
                        {
                            worksheet.Cells[rowIterator, 17].Value = "Failed";
                            worksheet.Cells[rowIterator, 17].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            //worksheet.Row(rowIterator).Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        }
                        else
                        {
                            worksheet.Cells[rowIterator, 17].Value = "Success";
                            worksheet.Cells[rowIterator, 17].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        }
                        j++;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
                return package.GetAsByteArray();
            }
        }


        public static bool IsAllDigits(string s)
        {
            return s.All(Char.IsDigit);
        }

        public static bool IsValidMobileNumber(string s)
        {
            if (s.Contains('-'))
            {
                if (s.Length == 11)
                {
                    var a = s.Split('-');
                    if (a[0].All(Char.IsDigit) && a[1].All(Char.IsDigit))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}