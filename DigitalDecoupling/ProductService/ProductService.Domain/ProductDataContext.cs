using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductService.Domain
{
	public class ProductDataContext : DbContext
	{
		private string connectionString;

		public ProductDataContext(string sqlConnectionString) {

			this.connectionString = sqlConnectionString;
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(connectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ProductCategoryMapping>().HasKey(pcm => new { pcm.ProductId, pcm.ProductCategoryId });

			modelBuilder.Entity<ProductCategoryMapping>().HasOne<Product>(pcm => pcm.Product)
				.WithMany(p => p.CategoryMappings)
				.HasForeignKey(pcm => pcm.ProductId);


			modelBuilder.Entity<ProductCategoryMapping>().HasOne<ProductCategory>(pcm => pcm.ProductCategory)
				.WithMany(p => p.ProductCategoryMappings)
				.HasForeignKey(pcm => pcm.ProductCategoryId);
		}


		public DbSet<Product> Products { get; set; }
		public DbSet<ProductCategory> ProductCategories { get; set; }
		public DbSet<ProductCategoryMapping> ProductCategoryMappings { get; set; }



	}
}
