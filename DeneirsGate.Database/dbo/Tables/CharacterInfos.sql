CREATE TABLE [dbo].[CharacterInfos] (
    [CharacterKey]  UNIQUEIDENTIFIER NOT NULL,
    [FirstName]     NVARCHAR (50)    NULL,
    [LastName]      NVARCHAR (50)    NULL,
    [PortraitUrl]   NVARCHAR (250)   NULL,
    [RaceKey]       UNIQUEIDENTIFIER NOT NULL,
    [ClassKey]      UNIQUEIDENTIFIER NOT NULL,
    [BackgroundKey] UNIQUEIDENTIFIER NOT NULL,
	[Fears]			NVARCHAR (250)   NULL,
	[Ideals]		NVARCHAR (250)   NULL,
    [Backstory]     NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([CharacterKey] ASC)
);

