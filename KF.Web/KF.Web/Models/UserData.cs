using KF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace KF.Web.Models
{
    public static class UserData
    {
        public static Boolean CheckUserFiscalYearClosed(int userId,string Date)
        {
            UserRegistration user = new UserRegistration();
            try
            {
                using (KFentities entities = new KFentities())
                {
                    var seletedUserData = GetUserData(userId);
                    var fiscalYearData = entities.FiscalYearPostings.Where(s => s.UserId == userId).OrderByDescending(s => s.Id).FirstOrDefault();

                    if(fiscalYearData != null)
                    {
                        string FiscalYearstring = seletedUserData.TaxStartMonthId + "/" + seletedUserData.TaxationStartDay + "/" + seletedUserData.TaxStartYear;
                        DateTime FiscalYearstart = Convert.ToDateTime(fiscalYearData.TaxStartYear);
                        DateTime FiscalYearEnd = FiscalYearstart.AddYears(1);
                        DateTime UploadDataDate = Convert.ToDateTime(Date);
                        if (UploadDataDate >= FiscalYearstart && UploadDataDate <= FiscalYearEnd)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        public static UserRegistration GetCurrentUserData()
        {
            UserRegistration user = new UserRegistration();
            try
            {
                string username = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                string roles = string.Empty;

                using (KFentities entities = new KFentities())
                {
                    user = entities.UserRegistrations.SingleOrDefault(u => u.Username == username);
                    var userRole = entities.Roles.Where(i => i.Id == user.RoleId).FirstOrDefault();
                    roles = userRole.RoleType;
                    return user;
                }
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                return user;
            }


        }

        public static UserRegistration GetUserDataByEmail(string email)
        {
            UserRegistration user = new UserRegistration();
            try
            {
                using (KFentities entities = new KFentities())
                {
                    return user = entities.UserRegistrations.SingleOrDefault(u => u.Email == email);
                }
            }
            catch (Exception)
            {
                return user;
            }


        }


        public static UserRegistration GetUserData(int userId)
        {
            UserRegistration user = new UserRegistration();
            try
            {
                using (KFentities entities = new KFentities())
                {
                    return user = entities.UserRegistrations.SingleOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception)
            {
                return user;
            }


        }
        public static UserRegistration GetUnpaidUserData(int userId)
        {
            UserRegistration user = new UserRegistration();
            try
            {
                using (KFentities entities = new KFentities())
                {
                    return user = entities.UserRegistrations.SingleOrDefault(u => u.Id == userId);
                }
            }
            catch (Exception)
            {
                return user;
            }


        }

        public static string GetCategory(int categoryId)
        {
            using (KFentities entities = new KFentities())
            {
                var data = entities.Categories.Where(i => i.Id == categoryId).Select(u => u.CategoryType).FirstOrDefault();
                return Convert.ToString(data);
            }
        }
        public static string GetClassificationType(int typeId)
        {
            using (KFentities entities = new KFentities())
            {
                var data = entities.ClassificationTypes.Where(i => i.Id == typeId).Select(u => u.ClassificationType1).FirstOrDefault();
                return Convert.ToString(data);
            }
        }
        public static string GetYear(int yearId)
        {
            using (KFentities entities = new KFentities())
            {
                var data = entities.tblYears.Where(i => i.Id == yearId).Select(u => u.Year).FirstOrDefault();
                return Convert.ToString(data);
            }
        }

        public static string GetClassificationById(int classificationId)
        {
            using (KFentities entities = new KFentities())
            {
                //var data = entities.Classifications.Where(i => i.Id == classificationId).Select(u => u.ClassificationType).FirstOrDefault();
                var data = entities.Classifications.Where(i => i.Id == classificationId).Select(u => u.ClassificationType).FirstOrDefault();
                return Convert.ToString(data);
            }
        }
        public static string GetCategoryId(int categoryId)
        {
            using (KFentities entities = new KFentities())
            {
                var data = entities.Categories.Where(i => i.Id == categoryId).Select(u => u.CategoryType).FirstOrDefault();
                return Convert.ToString(data);
            }
        }

        #region Invoice
        //public static InvoiceUserRegistration GetCurrentInvoiceUserData()
        //{
        //    InvoiceUserRegistration user = new InvoiceUserRegistration();
        //    try
        //    {
        //        string username = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
        //        string roles = string.Empty;

        //        using (KFentities entities = new KFentities())
        //        {
        //            user = entities.InvoiceUserRegistrations.SingleOrDefault(u => u.Username == username);
        //            //var userRole = entities.Roles.Where(i => i.Id == user.RoleId).FirstOrDefault();
        //            //roles = userRole.RoleType;
        //            return user;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        FormsAuthentication.SignOut();
        //        return user;
        //    }


        //}
        public static InvoiceUserRegistration GetCurrentInvoiceUserData()
        {
            InvoiceUserRegistration user = new InvoiceUserRegistration();
            try
            {
                //Session["panel_login_info"] = null;
                String username = "";
                if (HttpContext.Current.Session["invoiceuaer"]!=null)
                {
                    username = HttpContext.Current.Session["invoiceuaer"].ToString();
                }
                else
                {
                    username = null;
                }


                
                if (username != null)
                {
                    using (KFentities entities = new KFentities())
                    {
                        user = entities.InvoiceUserRegistrations.SingleOrDefault(u => u.Username == username);


                    }
                }
                else
                {

                    if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                    {
                        string username1 = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        using (KFentities entities = new KFentities())
                        {
                            user = entities.InvoiceUserRegistrations.SingleOrDefault(u => u.Username == username1);

                        }

                    }
                }
                return user;
            }
            catch (Exception)
            {
                FormsAuthentication.SignOut();
                return user;
            }


        }
        #endregion

    }
}