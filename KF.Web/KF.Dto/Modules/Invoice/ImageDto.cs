using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Invoice
{
    [DataContract]
    public class ImageDto
    {
        [DataMember]
        public int Month { get; set; }
        [DataMember]
        public int Year { get; set; }
        [DataMember]
        public string CloudImageName { get; set; }
        [DataMember]
        public bool IsCloud { get; set; }
        /// <summary>
        /// Gets or sets ImagePath.
        /// </summary>
        [DataMember]
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets ImagePath.
        /// </summary>
        [DataMember]
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets Image.
        /// </summary>
        [DataMember]
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets BankId.
        /// </summary>
        [DataMember]
        public int BankId { get; set; }

        /// <summary>
        /// Gets or sets UserId.
        /// </summary>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets ExpenseId.
        /// </summary>
        [DataMember]
        public int ExpenseId { get; set; }


        [DataMember]
        public int StatementId { get; set; }
    }
}
