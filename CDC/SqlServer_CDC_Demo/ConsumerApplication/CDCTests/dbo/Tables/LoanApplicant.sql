CREATE TABLE [dbo].[LoanApplicant] (
    [LoanId]      INT NOT NULL,
    [ApplicantId] INT NOT NULL,
    CONSTRAINT [PK_LoanApplicant] PRIMARY KEY CLUSTERED ([LoanId] ASC, [ApplicantId] ASC), 
    CONSTRAINT [FK_LoanApplicant_Loan] FOREIGN KEY ([LoanId]) REFERENCES [Loan](LoanId),
	CONSTRAINT [FK_LoanApplicant_Applicant] FOREIGN KEY ([ApplicantId]) REFERENCES [Applicant]([ApplicantId])
);

