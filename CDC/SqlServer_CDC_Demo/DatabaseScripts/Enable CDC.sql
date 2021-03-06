USE CDCTests
GO  
EXEC sys.sp_cdc_enable_db  
GO

EXEC sys.sp_cdc_enable_table  
	@source_schema = N'dbo',  
	@source_name   = N'Loan',  
	@role_name     = null,
	@supports_net_changes = 1
GO

EXEC sys.sp_cdc_enable_table  
	@source_schema = N'dbo',  
	@source_name   = N'Applicant',  
	@role_name     = null,
	@supports_net_changes = 1
GO

EXEC sys.sp_cdc_enable_table  
	@source_schema = N'dbo',  
	@source_name   = N'LoanApplicant',  
	@role_name     = null,
	@supports_net_changes = 1
GO

EXEC sys.sp_cdc_enable_table  
	@source_schema = N'dbo',  
	@source_name   = N'Property',  
	@role_name     = null,
	@supports_net_changes = 1
GO
