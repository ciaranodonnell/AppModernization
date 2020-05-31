CREATE PROCEDURE [dbo].[spSaveNewCustomerWithOutbox]
	(
	@Id INT OUTPUT,
	@Name VARCHAR(500) NOT NULL,
	@Address1 VARCHAR(500),
	@Address2 VARCHAR(500),
	@Address3 VARCHAR(500),
	@TownCity VARCHAR(200),
	@County VARCHAR(200),
	@CountryCode VARCHAR(2),
	@PostalZipCode VARCHAR(20),
	@RegisteredAddressId INT OUTPUT,
	@IndustryId INT NULL,
	@SavingUserId INT NOT NULL
	)
AS

	BEGIN TRAN INSERTCUSTOMER

		INSERT INTO Address
			(Address1,Address2, Address3, TownCity, County, CountryCode, PostalZipCode)
		VALUES 
			(@Address1, @Address2, @Address3, @TownCity, @County, @CountryCode, @PostalZipCode)
	
		SET @RegisteredAddressId = @@IDENTITY


		INSERT INTO Customer
			([Name], RegisteredAddressId, IndustryId, Version, CreatedDate, LastUpdatedDate, LastUpdatedByUser)
		VALUES 
			(@Name, @RegisteredAddressId, @IndustryId, 1, GETUTCDATE(),GETUTCDATE(), @SavingUserId)

		SET @Id = @@IDENTITY


		
		INSERT INTO [CommonOutbox]
			(EntityType, GeneratedDate) 
		VALUES
			('Customer', GETUTCDATE())



	COMMIT TRAN INSERTCUSTOMER

	SELECT * FROM [Customer] WHERE Id = @Id
	SELECT * FROM [Address] WHERE Id = @RegisteredAddressId



RETURN 0
