CREATE TABLE [dbo].[Loan] (
    [LoanId]     INT IDENTITY (1, 1) NOT NULL,
    [PropertyId] INT NOT NULL,
    [AmountInPennies] INT NOT NULL, 
    [RequestedCloseDate] DATETIME NULL, 
    PRIMARY KEY CLUSTERED ([LoanId] ASC), 
    CONSTRAINT [FK_Loan_Property] FOREIGN KEY (PropertyId) REFERENCES [Property]([PropertyId])
);

