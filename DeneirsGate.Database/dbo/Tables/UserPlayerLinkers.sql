CREATE TABLE [dbo].[UserPlayerLinkers]
(
	[UserKey] UNIQUEIDENTIFIER NOT NULL, 
    [PlayerKey] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_UserPlayerLinkers] PRIMARY KEY NONCLUSTERED ([UserKey], [PlayerKey])
)
