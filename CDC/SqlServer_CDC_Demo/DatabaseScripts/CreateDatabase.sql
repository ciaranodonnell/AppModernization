
USE [master]
GO

CREATE DATABASE [CDCTests]
GO


create table Loan
(
LoanId INT Identity(1,1) PRIMARY KEY,
PropertyId INT,
)

GO

create table Applicant
(
ApplicantId INT Identity(1,1) PRIMARY KEY,
[Name] varchar(100),
)

GO

CREATE TABLE [dbo].[LoanApplicant](
	[LoanId] [int] NOT NULL,
	[ApplicantId] [int] NOT NULL,
 CONSTRAINT [PK_LoanApplicant] PRIMARY KEY CLUSTERED 
(
	[LoanId] ASC,
	[ApplicantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

create table Property
(
PropertyId INT Identity(1,1) PRIMARY KEY,
[Address] varchar(500),
[Value] money,
)
