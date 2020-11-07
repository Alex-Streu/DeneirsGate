CREATE TABLE [dbo].[Environments]
(
    [EnvironmentKey] UNIQUEIDENTIFIER NOT NULL,
    [Name]    NVARCHAR (50)    NOT NULL,
    PRIMARY KEY CLUSTERED ([EnvironmentKey] ASC)
)
