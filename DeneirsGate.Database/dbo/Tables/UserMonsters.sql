CREATE TABLE [dbo].[UserMonsters]
(
    [UserKey] UNIQUEIDENTIFIER NOT NULL,
	[MonsterKey] UNIQUEIDENTIFIER NOT NULL,
    [IsPublic] BIT NOT NULL,
	CONSTRAINT [PK_UserMonsters] PRIMARY KEY NONCLUSTERED ([UserKey], [MonsterKey]) 
)
