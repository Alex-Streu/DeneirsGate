CREATE TABLE [dbo].[TreasureHoards]
(
	[TreasureHoardKey]     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[CP]                   NVARCHAR(50) NULL,
	[SP]                   NVARCHAR(50) NULL,
	[EP]                   NVARCHAR(50) NULL,
	[GP]                   NVARCHAR(50) NULL,
	[PP]                   NVARCHAR(50) NULL,
	[Gemstones]            NVARCHAR(50) NULL,
	[ArtObjects]           NVARCHAR(50) NULL,
	[Value]                INT NOT NULL DEFAULT 0,
	[MinChallenge]         INT NOT NULL,
	[MaxChallenge]         INT NOT NULL,
	[Probability]          INT NOT NULL
    PRIMARY KEY CLUSTERED ([TreasureHoardKey] ASC)
)
