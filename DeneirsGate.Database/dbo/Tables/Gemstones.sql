CREATE TABLE [dbo].[Gemstones]
(
    [GemstoneKey]   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]          NVARCHAR (50)    NOT NULL,
	[Description]   NVARCHAR (100)   NOT NULL,
	[Value]         INT              NOT NULL
    PRIMARY KEY CLUSTERED ([GemstoneKey] ASC)
)
