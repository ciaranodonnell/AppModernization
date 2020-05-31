using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain
{
	public class ProductCategory : BaseEntity
	{

		public int ProductCategoryId { get; set; }

		public string ProductCategoryName { get; set; }
		public IList<ProductCategoryMapping> ProductCategoryMappings { get; set; }
	}
}
