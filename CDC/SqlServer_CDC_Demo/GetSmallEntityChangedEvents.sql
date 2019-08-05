

/* Smaller Sub Entity Changed Events */


create procedure GetLoanComponentChanges
as
begin
	DECLARE @minlsn binary(10)

	Select top 1 @minlsn = actuallsn from Outbox order by changeid desc


	--These get published as Loan Core Data Created / Updated / Deleted Events
		SELECT * from cdc.fn_cdc_get_all_changes_dbo_Loan(@minlsn)

		

		--These get published as Applicant Created / Updated / Deleted Events
		
		SELECT * from cdc.fn_cdc_get_all_changes_dbo_Applicant()

		-- These become applicatned added / removed from loan events
		SELECT * from cdc.fn_cdc_get_all_changes_dbo_LoanApplicant()

--These get published as Property Created / Updated / Deleted Events
		SELECT * from cdc.fn_cdc_get_all_changes_dbo_Property()

		

end


