CREATE TABLE [dbo].[Applications] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Status]          NVARCHAR (MAX) NULL,
    [Notes]           NVARCHAR (MAX) NULL,
    [DateTimeCreated] DATETIME       NOT NULL,
    [Reviewer_Id]     NVARCHAR (128) NULL,
    [ApplicantId]     INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Applications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Applications_dbo.Applicants_Applicant_Id] FOREIGN KEY ([ApplicantId]) REFERENCES [dbo].[Applicants] ([Id]),
    CONSTRAINT [FK_dbo.Applications_dbo.AspNetUsers_Reviewer_Id] FOREIGN KEY ([Reviewer_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Reviewer_Id]
    ON [dbo].[Applications]([Reviewer_Id] ASC);

