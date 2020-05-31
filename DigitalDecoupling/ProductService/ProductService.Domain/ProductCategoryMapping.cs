using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain
{
	public class ProductCategoryMapping
	{
		public int ProductId { get; set; }
		public Product Product { get; set; }


		public int ProductCategoryId { get; set; }
		public ProductCategory ProductCategory { get; set; }
	}
}
