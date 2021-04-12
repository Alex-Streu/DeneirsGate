CREATE TABLE [dbo].[UserPasswordResets]
(
    [Code] NVARCHAR(256) NOT NULL,
    [UserKey] UNIQUEIDENTIFIER NOT NULL
    PRIMARY KEY CLUSTERED ([Code] ASC), 
    [DateCreated] DATETIME NOT NULL
)
