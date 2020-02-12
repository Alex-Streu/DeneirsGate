CREATE TABLE [dbo].[Roles] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NCHAR (10)       NOT NULL,
    [Priviledge] INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

