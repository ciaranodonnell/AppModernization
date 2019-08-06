CREATE PROCEDURE [dbo].[GetUpdatedLoanEntities]
AS

	

	DECLARE @MinLSN binary(10)
	DECLARE @MaxLSN binary(10)

	SELECT TOP 1 @MinLSN = [ActualLSN] FROM cdc.OutboxPostmarks ORDER BY ChangeId DESC

	SET @MaxLSN = sys.fn_cdc_get_max_lsn()

	
	SELECT * 
	INTO #LoanCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Loan(@MinLSN, @MaxLSN, N'all');  

	
	SELECT * 
	INTO #ApplicantCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Applicant(@MinLSN, @MaxLSN, N'all');  


	
	SELECT * 
	INTO #LoanApplicantCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_LoanApplicant(@MinLSN, @MaxLSN, N'all');  


	
	SELECT * 
	INTO #PropertyCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Property(@MinLSN, @MaxLSN, N'all');  


	SELECT * FROM #LoanCDC
	SELECT * FROM #ApplicantCDC
	SELECT * FROM #LoanApplicantCDC
	SELECT * FROM #PropertyCDC

	SELECT [__$start_lsn] FROM (
	SELECT [__$start_lsn] FROM #LoanCDC
	UNION 
	SELECT [__$start_lsn]  FROM #ApplicantCDC
	UNION 
	SELECT [__$start_lsn]  FROM #LoanApplicantCDC
	UNION 
	SELECT [__$start_lsn]  FROM #PropertyCDC
	) lsns ORDER BY [__$start_lsn] ASC
	

	

RETURN 0
