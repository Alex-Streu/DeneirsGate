CREATE TABLE [dbo].[Treasures]
(
	[TreasureKey]     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[CP]              NVARCHAR(50) NULL,
	[SP]              NVARCHAR(50) NULL,
	[EP]              NVARCHAR(50) NULL,
	[GP]              NVARCHAR(50) NULL,
	[PP]              NVARCHAR(50) NULL,
	[MinChallenge]    INT NOT NULL,
	[MaxChallenge]    INT NOT NULL,
	[Probability]     INT NOT NULL
    PRIMARY KEY CLUSTERED ([TreasureKey] ASC)
)
