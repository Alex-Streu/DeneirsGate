CREATE TABLE [dbo].[CharacterWeapons]
(
	[WeaponKey] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[CharacterKey] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [AttackMod] INT NOT NULL, 
    [DamageDice] NVARCHAR(10) NULL, 
    [DamageMod] INT NOT NULL, 
    [DamageType] UNIQUEIDENTIFIER NOT NULL
)
