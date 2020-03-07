CREATE TABLE [dbo].[FriendRequests]
(
	[UserSenderKey] UNIQUEIDENTIFIER NOT NULL, 
    [UserReceiverKey] UNIQUEIDENTIFIER NOT NULL, 
    [Status] INT NOT NULL,
	[LastUpdated] DATETIME NOT NULL, 
    CONSTRAINT [PK_FriendRequests] PRIMARY KEY NONCLUSTERED ([UserSenderKey], [UserReceiverKey])
)
