CREATE TABLE [dbo].[LoanApplicant] (
    [LoanId]      INT NOT NULL,
    [ApplicantId] INT NOT NULL,
    CONSTRAINT [PK_LoanApplicant] PRIMARY KEY CLUSTERED ([LoanId] ASC, [ApplicantId] ASC)
);

