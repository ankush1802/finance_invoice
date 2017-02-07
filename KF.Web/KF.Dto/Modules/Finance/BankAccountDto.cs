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
    public class BankAccountDto : ApiResponseDto
    {
        /// <summary>
        /// Gets or sets AccountId.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets AccountNumber.
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string Accountname { get; set; }

        /// <summary>
        /// Gets or sets Password.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets BankId.
        /// </summary>
        [DataMember]
        public int? BankId { get; set; }

        /// <summary>
        /// Gets or sets CardId.
        /// </summary>
        [DataMember]
        public int? CardId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is deleted or not.
        /// </summary>
        [DataMember]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        [DataMember]
        public System.DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets ModifiedDate.
        /// </summary>
        [DataMember]
        public System.DateTime? ModifiedDate { get; set; }
        [DataMember]
        public int Month { get; set; }
        [DataMember]
        public int Year { get; set; }
    }
}
