CREATE TABLE [dbo].[RelationshipTreeTiers]
(
    [TierKey]       UNIQUEIDENTIFIER NOT NULL,
    [TreeKey]       UNIQUEIDENTIFIER NOT NULL,
    [SortOrder]     INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([TierKey] ASC)
)
