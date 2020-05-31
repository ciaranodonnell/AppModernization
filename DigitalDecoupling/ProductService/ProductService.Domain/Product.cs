using System;
using System.Collections.Generic;

namespace ProductService.Domain
{
    public class Product : BaseEntity
    {
        
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int PriceLevelId { get; set; }
		public IList<ProductCategoryMapping> CategoryMappings { get; set; }
	}
}
