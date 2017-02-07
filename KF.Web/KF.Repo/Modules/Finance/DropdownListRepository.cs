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
    public class DropdownListRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Month List
        public List<MonthModelDto> GetMonthList()
        {
            try
            {
                using (var db = new KFentities())
                {
                    Mapper.CreateMap<tblMonth, MonthModelDto>();
                    return Mapper.Map<List<MonthModelDto>>(db.tblMonths.ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Year List
        public List<YearModelDto> GetYearList()
        {
            try
            {
                using (var db = new KFentities())
                {
                    Mapper.CreateMap<tblYear, YearModelDto>();
                    return Mapper.Map<List<YearModelDto>>(db.tblYears.ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Province List
        public List<ProvinceDto> GetProvinceList()
        {
            try
            {
                using (var db = new KFentities())
                {
                    Mapper.CreateMap<Province, ProvinceDto>();
                    return Mapper.Map<List<ProvinceDto>>(db.Provinces.Take(13).ToList());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Bank List
        public List<BankListDto> GetBankList(int userid, int bankType)
        {
            try
            {
                //banktype 1 for bank,2 for credit
                using (var db = new KFentities())
                {
                    var bankList = new List<Bank>();
                    if (bankType == 1)
                    {
                        bankList = db.Banks.Where(s => s.UserId == userid && s.BankType.Contains("bank")).ToList();
                    }
                    else
                    {
                        bankList = db.Banks.Where(s => s.UserId == userid && s.BankType.Contains("credits")).ToList();
                    }

                    if (bankList.Count > 0)
                    {

                    }
                    else
                    {
                        bankList = db.Banks.OrderBy(s => s.Id).Take(5).ToList();
                    }

                    Mapper.CreateMap<Bank, BankListDto>();
                    return Mapper.Map<List<BankListDto>>(bankList);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Classification
        public bool CheckClassificationAccountNumberExistCheck(string ChartAccountNumber)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ChartAccountDisplayNumber == ChartAccountNumber).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        public bool CheckClassificationNameExistCheck(string Name)
        {
            try
            {
                using (var db = new KFentities())
                {
                    if (db.Classifications.Where(a => a.ClassificationType == Name).Any())
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }
        public bool AddNewClassification(AddNewClassificationDto obj)
        {
            bool IsSave = false;
            using (var db = new KFentities())
            {
                try
                {
                    if (db.Classifications.Where(a => a.ClassificationType == obj.ctext && a.UserId == obj.UserID).Any())
                    {

                    }
                    else
                    {
                        var classificationType = db.ClassificationTypes.Where(a => a.Id == obj.ClassificationTypeId).Select(s => s.ClassificationType1).FirstOrDefault().Trim();
                        var raw = classificationType.Split('-');
                        Classification dbInsert = new Classification();
                        dbInsert.CategoryId = Convert.ToInt32(obj.CategoryId);
                        dbInsert.ClassificationType = obj.ctext.Trim();
                        dbInsert.Desc = obj.cDesc.Trim();
                        dbInsert.UserId = obj.UserID;
                        dbInsert.CreatedDate = DateTime.Now;
                        dbInsert.IsDeleted = false;
                        dbInsert.ChartAccountDisplayNumber = obj.ChartAccountNumber;
                        dbInsert.ChartAccountNumber = Convert.ToInt32(obj.ChartAccountNumber.Replace("-",string.Empty));
                        dbInsert.Type = raw[0];
                        var userDetail = db.UserRegistrations.Where(i => i.Id == obj.UserID).FirstOrDefault();
                        if (userDetail != null && userDetail.RoleId == 1)
                        {
                            //dbInsert.IsSole = true;
                            //dbInsert.IsIncorporated = true;
                            //dbInsert.IsPartnerShip = true;
                        }
                        else
                        {
                            //if (userDetail != null && userDetail.OwnershipId != null)
                            //{
                            //    switch (userDetail.OwnershipId)
                            //    {
                            //        case 1:
                            //            dbInsert.IsSole = true;
                            //            break;
                            //        case 2:
                            //            dbInsert.IsIncorporated = true;
                            //            break;
                            //        case 3:
                            //            dbInsert.IsPartnerShip = true;
                            //            break;

                            //    }
                            //}
                        }

                        db.Classifications.Add(dbInsert);
                        db.SaveChanges();
                        IsSave = true;
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return IsSave;
        }
        
        #endregion

        #region Classification type List
        public List<ClassificationTypeDto> GetClassificationType()
        {
            try
            {
                using (var db = new KFentities())
                {
                    //Mapper.CreateMap<ClassificationType, ClassificationTypeDto>();
                    //return Mapper.Map<List<ClassificationTypeDto>>(db.ClassificationTypes.ToList());

                    List<ClassificationTypeDto> objList = new List<ClassificationTypeDto>();
                    return objList = db.ClassificationTypes.Select(d => new ClassificationTypeDto {Id=d.Id,ClassificationType=d.ClassificationType1 }).ToList();
                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Classification List
        public List<ClassificationDto> GetClassificationListByUserId(int userId)
        {
            using (var db = new KFentities())
            {
                try
                {
                    List<Classification> objList = new List<Classification>();

                    var userData = db.UserRegistrations.Where(u => u.Id == userId).FirstOrDefault();
                    switch (userData.OwnershipId)
                    {
                        case 1:
                            objList = db.Classifications.Where(i => i.IsSole == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        case 2:
                            objList = db.Classifications.Where(i => i.IsIncorporated == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        case 3:
                            objList = db.Classifications.Where(i => i.IsPartnerShip == true && i.IndustryId == null && i.IsDeleted == false).ToList();
                            break;
                        default:
                            objList = db.Classifications.Where(i => i.IsPartnerShip == true && i.IsDeleted == false && i.IndustryId == null).ToList();
                            break;

                    }
                    var industryClassification = db.Classifications.Where(i => i.IndustryId == userData.IndustryId && i.IsDeleted == false).ToList();
                    if (userData.SubIndustryId != null)
                    {
                        objList.AddRange(industryClassification.Where(s => s.SubIndustryId == userData.SubIndustryId && s.IsDeleted == false).ToList());
                    }
                    objList.AddRange(db.Classifications.Where(i => i.UserId == userId && i.IsDeleted == false).ToList());
                    objList = objList.Where(d => d.Type.Trim() != "H" && d.Type.Trim() != "S" && d.Type.Trim() != "T").ToList();
                    objList = objList.Where(s => s.IsDeleted == false).ToList();
                    // objList.Add(db.Classifications.Where(i => i.Id == 95).FirstOrDefault());

                    Mapper.CreateMap<Classification, ClassificationDto>();
                    var classificationList = Mapper.Map<List<ClassificationDto>>(objList);
                    return classificationList;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Ownership List
        public List<OwnershipDto> GetOwnershipList()
        {
            using (var db = new KFentities())
            {
                try
                {
                    var ownershipList = db.Ownerships.ToList();
                    Mapper.CreateMap<Ownership, OwnershipDto>();
                    return Mapper.Map<List<OwnershipDto>>(ownershipList);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Get Category List
        public Category GetCategoryById(int id)
        {
            using (var db = new KFentities())
            {
                try
                {
                    return db.Categories.Where(i => i.Id == id).FirstOrDefault();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<CategoryDto> GetCategoryList()
        {
            using (var db = new KFentities())
            {
                try
                {
                    List<CategoryDto> objLst = new List<CategoryDto>();
                    objLst = db.Categories.Where(a => a.Id != 6).Select(i => new CategoryDto()
                    {
                        CategoryType = i.CategoryType,
                        Id = i.Id
                    }).ToList();
                    return objLst;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Currency List
        public List<CurrencyDto> GetCurrencyList()
        {
            using (var db = new KFentities())
            {
                try
                {
                    var currencyList = db.Currencies.ToList();
                    Mapper.CreateMap<Currency, CurrencyDto>();
                    return Mapper.Map<List<CurrencyDto>>(currencyList);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion

        #region Industry List
        public List<IndustryListDto> GetIndustryWithSubIndustryList()
        {
            // IndustryWithSubIndustryList objMainList=new IndustryWithSubIndustryList();
            using (var db = new KFentities())
            {
                try
                {
                    var industryList = db.Industries.ToList();
                    List<IndustryListDto> objIndustryList = new List<IndustryListDto>();

                    foreach (var data in industryList)
                    {
                        IndustryListDto objIndustry = new IndustryListDto();
                        objIndustry.IndustryId = data.Id;
                        objIndustry.IndustryName = data.IndustryType;
                        var subIndustryList = db.SubIndustries.Where(s => s.IndustryId == data.Id).ToList();
                        List<SubIndustryList> ObjSubIndustryList = new List<SubIndustryList>();
                        foreach (var item in subIndustryList)
                        {
                            SubIndustryList objSubIndustry = new SubIndustryList();
                            objSubIndustry.SubIndustryId = item.Id;
                            objSubIndustry.SubIndustryName = item.SubIndustryName;
                            ObjSubIndustryList.Add(objSubIndustry);
                        }
                        objIndustry.ObjSubIndustryList = ObjSubIndustryList;

                        objIndustryList.Add(objIndustry);
                    }
                    return objIndustryList;
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
