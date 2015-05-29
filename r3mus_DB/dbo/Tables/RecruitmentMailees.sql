CREATE TABLE [dbo].[RecruitmentMailees] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (MAX) NULL,
    [Submitted]          DATETIME       NOT NULL,
    [Mailed]             DATETIME       NULL,
    [SubmitterId]        NVARCHAR (MAX) NULL,
    [MailerId]           NVARCHAR (MAX) NULL,
    [CorpId_AtLastCheck] BIGINT         DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.RecruitmentMailees] PRIMARY KEY CLUSTERED ([Id] ASC)
);



