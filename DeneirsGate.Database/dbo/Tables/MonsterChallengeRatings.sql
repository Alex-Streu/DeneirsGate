CREATE TABLE [dbo].[MonsterChallengeRatings]
(
    [RatingKey] UNIQUEIDENTIFIER NOT NULL,
    [Challenge]    NVARCHAR (50)    NOT NULL,
    [Proficiency] INT NOT NULL DEFAULT 2, 
    [XP] INT NOT NULL DEFAULT 0, 
    [Difficulty] INT NOT NULL DEFAULT 1, 
    PRIMARY KEY CLUSTERED ([RatingKey] ASC)
)
