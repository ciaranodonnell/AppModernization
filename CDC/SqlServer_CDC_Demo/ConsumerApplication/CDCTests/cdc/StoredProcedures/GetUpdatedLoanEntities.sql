CREATE PROCEDURE [dbo].[GetUpdatedLoanEntities]
AS

	DECLARE @EventBatchDate DATETIMEOFFSET
	SET @EventBatchDate = GETUTCDATE()

	DECLARE @MinLSN binary(10)
	DECLARE @MaxLSN binary(10)

	SELECT TOP 1 @MinLSN = [ActualLSN] FROM 
	cdc.OutboxPostmarks 
	WHERE EventSentUTC IS NOT NULL
	ORDER BY ChangeId DESC

	-- need to check if the get all changes is inclusive of the min lsn


	SET @MaxLSN = sys.fn_cdc_get_max_lsn()

	
	SELECT * 
	INTO #LoanCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Loan(
	ISNULL(@MinLSN, sys.fn_cdc_get_min_lsn('dbo_Loan'))
	, @MaxLSN, N'all');  

	
	SELECT * 
	INTO #ApplicantCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Applicant(
	ISNULL(@MinLSN, sys.fn_cdc_get_min_lsn('dbo_Applicant'))
	, @MaxLSN, N'all');  


	
	SELECT * 
	INTO #LoanApplicantCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_LoanApplicant(
	ISNULL(@MinLSN, sys.fn_cdc_get_min_lsn('dbo_LoanApplicant'))
	, @MaxLSN, N'all');  


	
	SELECT * 
	INTO #PropertyCDC  
	FROM cdc.fn_cdc_get_all_changes_dbo_Property(
	ISNULL(@MinLSN, sys.fn_cdc_get_min_lsn('dbo_Property'))
	, @MaxLSN, N'all');  

	/*if minlsn is inclusive delete it from our temp tables

	delete from #propertyCDC where [__$start_lsn]= @MinLSN
	delete from #loanapplicantCDC where [__$start_lsn]= @MinLSN
	delete from #applicantCDC where [__$start_lsn]= @MinLSN
	delete from #loanCDC where [__$start_lsn]= @MinLSN

	*/
	
	INSERT INTO cdc.OutboxPostmarks 
	(ActualLSN,  EventBatchDate) 
	SELECT [__$start_lsn], @EventBatchDate FROM (
	SELECT [__$start_lsn] FROM #LoanCDC
	UNION 
	SELECT [__$start_lsn]  FROM #ApplicantCDC
	UNION 
	SELECT [__$start_lsn]  FROM #LoanApplicantCDC
	UNION 
	SELECT [__$start_lsn]  FROM #PropertyCDC
	) lsns ORDER BY [__$start_lsn] ASC
	
	SELECT c.*, ChangeId FROM #LoanCDC c JOIN cdc.OutboxPostmarks o on c.[__$start_lsn] = o.ActualLSN WHERE EventBatchDate = @EventBatchDate
	SELECT c.*, ChangeId FROM #ApplicantCDC c JOIN cdc.OutboxPostmarks o on c.[__$start_lsn] = o.ActualLSN WHERE EventBatchDate = @EventBatchDate
	SELECT c.*, ChangeId FROM #LoanApplicantCDC c JOIN cdc.OutboxPostmarks o on c.[__$start_lsn] = o.ActualLSN WHERE EventBatchDate = @EventBatchDate
	SELECT c.*, ChangeId FROM #PropertyCDC c JOIN cdc.OutboxPostmarks o on c.[__$start_lsn] = o.ActualLSN WHERE EventBatchDate = @EventBatchDate
	
	Select ActualLSN from 
	cdc.OutboxPostmarks 
	WHERE EventBatchDate = @EventBatchDate

	DROP TABLE #LoanCDC
	DROP TABLE #ApplicantCDC
	DROP TABLE #LoanApplicantCDC
	DROP TABLE #PropertyCDC

	

RETURN 0
