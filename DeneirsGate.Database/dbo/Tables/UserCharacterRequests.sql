CREATE TABLE [dbo].[UserCharacterRequests]
(
    [RequestKey] UNIQUEIDENTIFIER NOT NULL,
    [OwnerUserKey] UNIQUEIDENTIFIER NOT NULL, 
    [PlayerUserKey] UNIQUEIDENTIFIER NOT NULL, 
    [CharacterShortKey] NVARCHAR(25) NOT NULL,
    [SentTo] INT NOT NULL DEFAULT 0, 
    PRIMARY KEY CLUSTERED ([RequestKey] ASC)
)
