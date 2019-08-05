
/* Giant Entity Update Events */

create procedure GetLoanChanges
as
begin
	create table #loadids as (LoanId INT)

	INSERT INTO SELECT LoanId from cdc.fn_cdc_get_net_changes_dbo_Loan()

	INSERT INTO SELECT LoanId 
			from cdc.fn_cdc_get_net_changes_dbo_Applicant() c
			JOIN LoanApplicant la
				la.applicantId = c.applicantid
			





	--DO NORMAL GET LOAN OBJECT SQL

	Select distinct * from loan l join tl on tl.loanid = l.id



end