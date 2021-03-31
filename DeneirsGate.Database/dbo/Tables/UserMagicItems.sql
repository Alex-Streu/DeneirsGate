CREATE TABLE [dbo].[UserMagicItems]
(
    [UserKey] UNIQUEIDENTIFIER NOT NULL,
	[MagicItemKey] UNIQUEIDENTIFIER NOT NULL,
    [IsPublic] BIT NOT NULL,
	CONSTRAINT [PK_UserMagicItems] PRIMARY KEY NONCLUSTERED ([UserKey], [MagicItemKey]) 
)
