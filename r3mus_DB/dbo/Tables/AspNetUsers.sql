CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [MemberType]           NVARCHAR (MAX) NULL,
    [EmailAddress]         NVARCHAR (MAX) NULL,
    [MemberSince]          DATETIME       NULL,
    [Avatar]               NVARCHAR (MAX) NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            DEFAULT ((0)) NOT NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            DEFAULT ((0)) NOT NULL,
    [TwoFactorEnabled]     BIT            DEFAULT ((0)) NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            DEFAULT ((0)) NOT NULL,
    [AccessFailedCount]    INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);






GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([UserName] ASC);

