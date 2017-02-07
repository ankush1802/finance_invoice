using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KF.Utilities.Common
{
    public static class Sendmail
    {
        public static bool SendEmail(SendMailModelDto _objModelMail)
        {
            bool MailSend = false;
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUserName"]);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.MessageBody;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtpout.secureserver.net");
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpUserName"], ConfigurationManager.AppSettings["smtpPassword"]);
                client.Send(mail);

                #region Second way of sending mail
                //MailMessage mail = new MailMessage();
                //mail.To.Add(_objModelMail.To);
                //mail.From = new MailAddress(ConfigurationManager.AppSettings["smtpUserName"]);
                //mail.Subject = _objModelMail.Subject;
                //string Body = _objModelMail.MessageBody;
                //mail.Body = Body;
                //mail.IsBodyHtml = true;
                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = "smtp.gmail.com";
                //smtp.Port = 587;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUserName"], ConfigurationManager.AppSettings["smtpPassword"]);
                //// (ConfigurationSettings.AppSettings["smtpUserName"], ConfigurationSettings.AppSettings["smtpPassword"]);// Enter seders User name and password
                //smtp.EnableSsl = true;
                //smtp.Send(mail);
                #endregion

                return MailSend = true;
            }
            catch (Exception)
            {
                return MailSend;

            }

        }
        public static string GenerateRandomString(int length)
        {
            //It will generate string with combination of small,capital letters and numbers
            char[] charArr = "023456789ABCDEFGHJKMNOPQRSTUVWXYZabcdefghjkmnopqrstuvwxyz".ToCharArray();
            string randomString = string.Empty;
            Random objRandom = new Random();
            for (int i = 0; i < length; i++)
            {
                //Don't Allow Repetation of Characters
                int x = objRandom.Next(1, charArr.Length);
                if (!randomString.Contains(charArr.GetValue(x).ToString()))
                    randomString += charArr.GetValue(x);
                else
                    i--;
            }
            return randomString;
        }
    }
}
