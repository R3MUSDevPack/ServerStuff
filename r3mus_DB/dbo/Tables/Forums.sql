CREATE TABLE [dbo].[Forums] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (MAX) NULL,
    [MinimumRole] NVARCHAR (MAX) NULL,
    [CreatorId]   NVARCHAR (MAX) NULL,
    [Created]     DATETIME       NOT NULL,
    [Deleted]     BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Forums] PRIMARY KEY CLUSTERED ([Id] ASC)
);

