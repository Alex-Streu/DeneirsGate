CREATE TABLE [dbo].[Characters] (
    [CharacterKey]  UNIQUEIDENTIFIER NOT NULL,
    [FirstName]     NVARCHAR (50)    NULL,
    [LastName]      NVARCHAR (50)    NULL,
    [Portrait]   NVARCHAR (250)   NULL,
    [RaceKey]       UNIQUEIDENTIFIER NOT NULL,
    [ClassKey]      UNIQUEIDENTIFIER NOT NULL,
    [BackgroundKey] UNIQUEIDENTIFIER NOT NULL,
	[Fears]			NVARCHAR (250)   NULL,
	[Ideals]		NVARCHAR (250)   NULL,
    [Backstory]     NVARCHAR (MAX)   NULL,
    [Languages] NVARCHAR(250) NULL, 
    [Alignment] NVARCHAR(2) NOT NULL DEFAULT 'TN', 
    PRIMARY KEY CLUSTERED ([CharacterKey] ASC)
);

