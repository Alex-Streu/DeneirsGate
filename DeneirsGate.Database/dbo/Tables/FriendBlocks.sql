CREATE TABLE [dbo].[FriendBlocks]
(
	[UserKey] UNIQUEIDENTIFIER NOT NULL, 
    [BlockedUserKey] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_FriendBlocks] PRIMARY KEY NONCLUSTERED ([UserKey], [BlockedUserKey])
)
