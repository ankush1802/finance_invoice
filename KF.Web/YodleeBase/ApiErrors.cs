using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace YodleeBase
{

    public class ApiErrors
    {

        public static void LoadErrors(string path)
        {
            string json = "";
            using(var fs=new FileStream(path, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs);
                json = sr.ReadToEnd();
                errors = JsonConvert.DeserializeObject<IDictionary<string, Error>>(json);
            }
        }
        private static IDictionary<string, Error> errors;

        public static IDictionary<string, Error> Errors
        {
            get
            {
                if (errors == null)
                {
                    throw new Exception("You must load the errors first.");
                }

                return errors;
            }
        }
    }

    [DataContract]
    public class Error
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string description { get; set; }
    }
}
