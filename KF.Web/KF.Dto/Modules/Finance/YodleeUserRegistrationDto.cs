using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
    [DataContract]
    public class YodleeUserRegistrationDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string UName { get; set; }
        [DataMember]
        public string UPassword { get; set; }
        [DataMember]
        public string CobrandName { get; set; }
        [DataMember]
        public string CobrandPassword { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DataMember]
        public Nullable<int> KippinUserId { get; set; }
    }
}
