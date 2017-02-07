
using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Invoice
{
    [DataContract]
    public class ImageListDto : ApiResponseDto
    {
        [DataMember]
        public string ImagePath { get; set; }

        [DataMember]
        public string ImageName { get; set; }

        [DataMember]
        public bool IsAssociated{ get; set; }

        [DataMember]
        public string UserId { get; set; }
    }

    [DataContract]
    public class FolderListDto : ApiResponseDto
    {

        [DataMember]
        public string FolderName { get; set; }
        [DataMember]
        public bool IsAssociated { get; set; }
    }
}
