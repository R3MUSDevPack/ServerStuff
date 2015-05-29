CREATE TABLE [dbo].[AspNetUsers] (
    [Id]            NVARCHAR (128) NOT NULL,
    [UserName]      NVARCHAR (MAX) NOT NULL,
    [PasswordHash]  NVARCHAR (MAX) NULL,
    [SecurityStamp] NVARCHAR (MAX) NULL,
    [MemberType]    NVARCHAR (MAX) NULL,
    [EmailAddress]  NVARCHAR (MAX) NULL,
    [Discriminator] NVARCHAR (128) NOT NULL,
    [MemberSince]   DATETIME       NULL,
    [Avatar]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);



