CREATE TABLE [dbo].[CharacterInfos] (
    [CharacterKey]  UNIQUEIDENTIFIER NOT NULL,
    [FirstName]     NVARCHAR (50)    NULL,
    [LastName]      NVARCHAR (50)    NULL,
    [PortraitUrl]   NVARCHAR (MAX)   NULL,
    [RaceKey]       UNIQUEIDENTIFIER NOT NULL,
    [ClassKey]      UNIQUEIDENTIFIER NOT NULL,
    [Level]         INT              NOT NULL,
    [BackgroundKey] UNIQUEIDENTIFIER NOT NULL,
    [Bio]           NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([CharacterKey] ASC)
);

