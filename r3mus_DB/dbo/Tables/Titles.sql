CREATE TABLE [dbo].[Titles] (
    [UserId]             NVARCHAR (MAX) NULL,
    [TitleName]          NVARCHAR (MAX) NULL,
    [ApplicationUser_Id] NVARCHAR (128) NULL,
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.Titles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Titles_dbo.AspNetUsers_ApplicationUser_Id] FOREIGN KEY ([ApplicationUser_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationUser_Id]
    ON [dbo].[Titles]([ApplicationUser_Id] ASC);

