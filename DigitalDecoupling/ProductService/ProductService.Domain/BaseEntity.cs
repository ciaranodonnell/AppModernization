using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain
{
	public abstract class BaseEntity
	{

		public DateTime CreatedDate { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public int Version { get; set; }

	}
}
