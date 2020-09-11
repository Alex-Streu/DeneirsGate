CREATE TABLE [dbo].[MonsterEnvironmentLinkers]
(
	[MonsterKey] UNIQUEIDENTIFIER NOT NULL,
    [EnvironmentKey] UNIQUEIDENTIFIER NOT NULL, 
	CONSTRAINT [PK_MonsterEnvironmentLinkers] PRIMARY KEY NONCLUSTERED ([MonsterKey], [EnvironmentKey])
)
