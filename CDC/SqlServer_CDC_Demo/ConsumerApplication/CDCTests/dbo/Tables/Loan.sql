CREATE TABLE [dbo].[Loan] (
    [LoanId]     INT IDENTITY (1, 1) NOT NULL,
    [PropertyId] INT NULL,
    PRIMARY KEY CLUSTERED ([LoanId] ASC)
);

