CREATE TABLE [dbo].[Threads] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [ForumID]   INT            NOT NULL,
    [CreatorId] NVARCHAR (MAX) NULL,
    [Created]   DATETIME       NOT NULL,
    [Title]     NVARCHAR (MAX) NULL,
    [User_Id]   NVARCHAR (128) NULL,
    CONSTRAINT [PK_dbo.Threads] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Threads_dbo.AspNetUsers_User_Id] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.Threads_dbo.Forums_ForumID] FOREIGN KEY ([ForumID]) REFERENCES [dbo].[Forums] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[Threads]([User_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ForumID]
    ON [dbo].[Threads]([ForumID] ASC);

