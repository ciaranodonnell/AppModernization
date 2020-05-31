using CustomerService.Data.Interfaces;
using CustomerService.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace CustomerService.Data
{
	public class SimpleSqlCustomerRepository : ICustomerRepository
	{
		private SqlConnection connection;

		public SimpleSqlCustomerRepository(string sqlConnectionString)
		{
			this.connection = new SqlConnection(sqlConnectionString);
			this.connection.Open();
			LoadIndustryCache();
		}

		public Customer InsertNewCustomer(Customer customer, string userId)
		{
			try
			{
				EnsureConnectionOpen();

				SqlCommand cmd = connection.CreateCommand();
				cmd.CommandText = "spSimpleSaveNewCustomer";
				cmd.CommandType = System.Data.CommandType.StoredProcedure;

				cmd.Parameters.Add("@Id", System.Data.SqlDbType.Int)
					.Direction = System.Data.ParameterDirection.Output;
				cmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 500)
					.Value = customer.Name;
				cmd.Parameters.Add("@Address1", System.Data.SqlDbType.VarChar, 200)
					.Value = customer.RegisteredAddress.Address1;
				cmd.Parameters.Add("@Address2", System.Data.SqlDbType.VarChar, 200)
				.Value = customer.RegisteredAddress.Address2;
				cmd.Parameters.Add("@Address3", System.Data.SqlDbType.VarChar, 200)
					.Value = customer.RegisteredAddress.Address3;
				cmd.Parameters.Add("@TownCity", System.Data.SqlDbType.VarChar, 200)
					.Value = customer.RegisteredAddress.TownCity;
				cmd.Parameters.Add("@County", System.Data.SqlDbType.VarChar, 200)
					.Value = customer.RegisteredAddress.County;
				cmd.Parameters.Add("@CountryCode", System.Data.SqlDbType.VarChar, 2)
					.Value = customer.RegisteredAddress.CountryCode;
				cmd.Parameters.Add("@PostalZipCode", System.Data.SqlDbType.VarChar, 20)
					.Value = customer.RegisteredAddress.PostalCode;
				cmd.Parameters.Add("@RegisteredAddressId", System.Data.SqlDbType.Int)
					.Direction = System.Data.ParameterDirection.Output;
				cmd.Parameters.Add("@IndustryId", System.Data.SqlDbType.Int)
					.Value = customer.PrimaryIndustry.Id;
				cmd.Parameters.Add("@SavingUserId", System.Data.SqlDbType.Int)
					.Value = userId;


				using (var reader = cmd.ExecuteReader())
				{
					Customer customerFromDb = new Customer();
					if (reader.Read())
					{
						//read back the customer record
						customerFromDb.Id = reader.GetInt32FromColumn("Id");
						var industryId = reader.GetInt32FromColumn("IndustryId");
						customerFromDb.PrimaryIndustry = TryGetIndustryFromCache(industryId);
						customerFromDb.Version = reader.GetInt32FromColumn("Version");
						customerFromDb.LastUpdatedByUser = reader.GetStringFromColumn("LastUpdatedByUser");
						customerFromDb.LastUpdatedDate = reader.GetDateTimeFromColumn("LastUpdatedDate");
						customerFromDb.CreatedDate = reader.GetDateTimeFromColumn("CreatedDate");
						customerFromDb.Name = reader.GetStringFromColumn("Name");
					}

					if (reader.NextResult() && reader.Read())
					{
						Address address = new Address();
						address.Address1 = reader.GetStringFromColumn("Address1");
						address.Address2 = reader.GetStringFromColumn("Address2");
						address.Address3 = reader.GetStringFromColumn("Address3");
						address.TownCity = reader.GetStringFromColumn("TownCity");

						address.County = reader.GetStringFromColumn("County");
						address.CountryCode = reader.GetStringFromColumn("CountryCode");
						address.PostalCode = reader.GetStringFromColumn("PostalZipCode");

						address.Version = reader.GetInt32FromColumn("Version");
						address.LastUpdatedByUser = reader.GetStringFromColumn("LastUpdatedByUser");
						address.LastUpdatedDate = reader.GetDateTimeFromColumn("LastUpdatedDate");
						address.CreatedDate = reader.GetDateTimeFromColumn("CreatedDate");

						customerFromDb.RegisteredAddress = address;
						//read back the address
					}
					return customerFromDb;
				}


				throw new Exception("the save didnt work for some reason");

			}
			catch (Exception ex)
			{
				throw;
			}

		}

		private Industry TryGetIndustryFromCache(int industryId)
		{
			if (industryCache.TryGetValue(industryId, out Industry industry))
			{
				return industry;
			}
			else
			{
				//handle the fact we dont have the industry from the database
				return new Industry
				{
					Id = industryId,
					Name = "UNKNOWN INDUSTRY"
				};
			}
		}

		private void EnsureConnectionOpen()
		{
			if (connection.State != System.Data.ConnectionState.Open)
				connection.Open();
		}


		private Dictionary<int, Industry> industryCache = new Dictionary<int, Industry>();

		private void LoadIndustryCache()
		{

			EnsureConnectionOpen();

			SqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM [Industry]";
			cmd.CommandType = System.Data.CommandType.Text;

			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var industry = new Industry
					{
						Id = reader.GetInt32FromColumn("Id"),
						Name = reader.GetStringFromColumn("Description"),
						Version = reader.GetInt32FromColumn("Version"),
						CreatedDate = reader.GetDateTimeFromColumn("CreatedDate"),
						LastUpdatedDate = reader.GetDateTimeFromColumn("LastUpdatedDate"),
						LastUpdatedByUser = reader.GetStringFromColumn("LastUpdatedByUser")
					};

					industryCache.Add(industry.Id, industry);
				}

			}

		}

		public Customer InsertNewCustomerAndEvent(Customer customer, string UserId)
		{
			throw new NotImplementedException();
		}
	}
}
