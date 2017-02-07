using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
namespace KF.Web
{
    public class ApiCall
    {
        private static String username = "azullu";
        private static String password = "FmostlyWXgit4589##";

        #region Get Response From Server
        public static HttpWebResponse GetResponseFromApi(string BaseUrl, string HttpVerb, string requestBody)
        {
            try
            {

                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(BaseUrl);


                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = HttpVerb;
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
                httpWebRequest.ContinueTimeout = 12000;

                if ((HttpVerb.ToUpper() == "POST" || HttpVerb.ToUpper() == "PUT" || HttpVerb.ToUpper() == "DELETE") && !string.IsNullOrEmpty(requestBody))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        //requestBody is json searialized string value
                        streamWriter.Write(requestBody);
                        streamWriter.Flush();
                        streamWriter.Close();

                    }

                }
                return (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return null;
            }

        }
        #endregion

        #region Get Image From Server
        //public static Image GetUserImage(string BaseUrl, string HttpVerb)
        //{
        //     Image UserProfilImage;
        //    try
        //    {
        //        String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
        //        WebRequest requestPic = WebRequest.Create(BaseUrl);
        //        requestPic.ContentType = "application/json";
        //        requestPic.Method = HttpVerb;
        //        requestPic.Headers.Add("Authorization", "Basic " + encoded);
        //        WebResponse responseImage = requestPic.GetResponse();
        //         UserProfilImage = Image.FromStream(responseImage.GetResponseStream()); // Error
        //        return UserProfilImage;
        //    }
        //    catch
        //    {
        //        return UserProfilImage; 
        //    }


        //}
        #endregion
    }
}