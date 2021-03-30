CREATE TABLE [dbo].[Suggestions]
(
    [SuggestionKey] UNIQUEIDENTIFIER NOT NULL,
    [UserKey]       UNIQUEIDENTIFIER NOT NULL,
    [SuggestionText]    NVARCHAR (250)   NOT NULL,
    [Type]          INT NOT NULL,
    [Status]        INT NOT NULL DEFAULT 0,
    [DateSubmitted] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([SuggestionKey] ASC)
)
