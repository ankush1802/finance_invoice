using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
	[DataContract]
	public class ItemSummary
	{
		[DataMember]
		public int itemId { get; set; }

		[DataMember]
		public ContentServiceInfo contentServiceInfo { get; set; }

		[DataMember]
		public int contentServiceId { get; set; }

		[DataMember]
		public string itemDisplayName { get; set; }

		[DataMember]
		public ItemData itemData { get; set; }
	}

	[DataContract]
	public class ItemData
	{
		[DataMember]
        public ICollection<Account> accounts { get; set; }
	}
}
