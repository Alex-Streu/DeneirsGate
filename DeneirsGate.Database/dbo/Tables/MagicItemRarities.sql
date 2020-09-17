CREATE TABLE [dbo].[MagicItemRarities]
(
    [RarityKey] UNIQUEIDENTIFIER NOT NULL,
    [Name]      NVARCHAR (50)    NOT NULL,
    [Rarity]    INT NOT NULL DEFAULT 1, 
    PRIMARY KEY CLUSTERED ([RarityKey] ASC)
)
