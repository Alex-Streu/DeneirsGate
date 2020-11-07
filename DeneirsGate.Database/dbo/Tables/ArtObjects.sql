CREATE TABLE [dbo].[ArtObjects]
(
    [ArtObjectKey]  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]          NVARCHAR (150)    NOT NULL,
	[Value]         INT              NOT NULL
    PRIMARY KEY CLUSTERED ([ArtObjectKey] ASC)
)
