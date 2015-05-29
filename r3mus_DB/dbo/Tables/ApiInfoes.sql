CREATE TABLE [dbo].[ApiInfoes] (
    [ApiKey]           INT            NOT NULL,
    [VerificationCode] NVARCHAR (MAX) NULL,
    [User_Id]          NVARCHAR (128) NULL,
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.ApiInfoes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.ApiInfoes_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[ApiInfoes]([User_Id] ASC);

