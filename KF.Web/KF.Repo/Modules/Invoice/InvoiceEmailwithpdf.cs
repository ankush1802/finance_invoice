using KF.Dto.Modules.Common;
using KF.Entity;
using KF.Utilities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Invoice
{
    public class InvoiceEmailwithpdf
    {
        public static string InvoiceAPiUrl = System.Configuration.ConfigurationSettings.AppSettings["WebsiteBaseUrl"];

        public bool SendEmailToInvoice(int InvoiceId, string pdfpath)
        {
            var Path = pdfpath.Split('\\').Last();
            using (KFentities context = new KFentities())
            {
                string html = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/EmailToCustomer.html"));

                var emailto = context.kl_getCustomerEmail(InvoiceId).FirstOrDefault();

                var InvoiceData = context.kl_Invoicedata_email(InvoiceId).FirstOrDefault();
                var SupplierComapnName = context.Kl_getSupplierEmail(InvoiceId).FirstOrDefault();

                if (emailto != null && InvoiceData != null && SupplierComapnName != null)
                {
                    var InvoiceDate = InvoiceData.InvoiceDate;
                    var logo = InvoiceData.CompanyLogo;
                    var InvoiceNumber = InvoiceData.InvoiceNumber;
                    var DueDate = InvoiceData.DueDate;
                    var Amount = InvoiceData.Total;
                    var SenderName = InvoiceData.FirstName;
                    var CompanyName = InvoiceData.Company_Name;
                    var Phn = InvoiceData.MobileNumber;
                    var Email = InvoiceData.EmailTo;
                    Path = InvoiceAPiUrl + "InvoicePdf/" + Path;

                    //  imagePath = new Uri(HttpContext.Current.Server.MapPath("~/" + data)).AbsoluteUri;
                    var CompanyLogo = InvoiceAPiUrl + logo;
                    html = html.Replace("#CompanyLogo", CompanyLogo);
                    html = html.Replace("#InvoiceDate", InvoiceDate);
                    html = html.Replace("#Customername", SupplierComapnName);
                    html = html.Replace("#InvoiceNumber", InvoiceNumber);
                    html = html.Replace("#DueDate", DueDate);
                    html = html.Replace("#Amount", Amount.ToString());
                    html = html.Replace("#SenderName", SenderName);
                    html = html.Replace("#companyname", CompanyName);
                    html = html.Replace("#Phn", Phn);
                    html = html.Replace("#Email", Email);
                    html = html.Replace("#pdfpath", Path);

                    SendMailModelDto _objModelMail = new SendMailModelDto();
                    _objModelMail.To = emailto;
                    _objModelMail.Subject = "Invitation mail from Kippin-Invoice";
                    _objModelMail.MessageBody = html;
                    var mailSent = Sendmail.SendEmail(_objModelMail);

                    return mailSent;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
