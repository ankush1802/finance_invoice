using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
    [DataContract]
    public class CameraImageUploadDto : ApiResponseDto
    {
        [DataMember]
        public string Base64Image { get; set; }

        [DataMember]
        public string ImageName { get; set; }


        [DataMember]
        public int UserId { get; set; }


    }
}
