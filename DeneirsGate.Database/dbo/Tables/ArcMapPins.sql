CREATE TABLE [dbo].[ArcMapPins]
(
    [PinKey]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[ArcKey]        UNIQUEIDENTIFIER NOT NULL,
    [QuestKey]      UNIQUEIDENTIFIER NOT NULL,
    [X]             FLOAT NOT NULL,
    [Y]             FLOAT NOT NULL
    PRIMARY KEY CLUSTERED ([PinKey] ASC)
)
