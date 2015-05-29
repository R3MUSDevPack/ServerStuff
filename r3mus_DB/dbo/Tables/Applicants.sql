CREATE TABLE [dbo].[Applicants] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX) NULL,
    [EmailAddress]     NVARCHAR (MAX) NULL,
    [ApiKey]           INT            NOT NULL,
    [VerificationCode] NVARCHAR (MAX) NULL,
    [Information]      NVARCHAR (MAX) NULL,
    [Age]              NVARCHAR (MAX) NULL,
    [ToonAge]          NVARCHAR (MAX) NULL,
    [Source]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Applicants] PRIMARY KEY CLUSTERED ([Id] ASC)
);

