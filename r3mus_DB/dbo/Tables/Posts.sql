CREATE TABLE [dbo].[Posts] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Title]    NVARCHAR (MAX) NULL,
    [Body]     NVARCHAR (MAX) NULL,
    [PostedAt] SMALLDATETIME  NOT NULL,
    [ThreadId] INT            NOT NULL,
    [UserId]   NVARCHAR (128) NULL,
    CONSTRAINT [PK_dbo.Posts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Posts_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_dbo.Posts_dbo.Threads_ThreadId] FOREIGN KEY ([ThreadId]) REFERENCES [dbo].[Threads] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[Posts]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ThreadId]
    ON [dbo].[Posts]([ThreadId] ASC);

