/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT TOP 1 1 FROM Environments)
BEGIN
	INSERT INTO Environments VALUES (NEWID(), 'Arctic')
	INSERT INTO Environments VALUES (NEWID(), 'Coastal')
	INSERT INTO Environments VALUES (NEWID(), 'Desert')
	INSERT INTO Environments VALUES (NEWID(), 'Forest')
	INSERT INTO Environments VALUES (NEWID(), 'Grassland')
	INSERT INTO Environments VALUES (NEWID(), 'Hill')
	INSERT INTO Environments VALUES (NEWID(), 'Mountain')
	INSERT INTO Environments VALUES (NEWID(), 'Swamp')
	INSERT INTO Environments VALUES (NEWID(), 'Underdark')
	INSERT INTO Environments VALUES (NEWID(), 'Underwater')
	INSERT INTO Environments VALUES (NEWID(), 'Urban')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM MonsterChallengeRatings)
BEGIN
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '0', 2, 0, 1)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '1/8', 2, 25, 2)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '1/4', 2, 50, 3)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '1/2', 2, 100, 4)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '1', 2, 200, 5)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '2', 2, 450, 6)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '3', 2, 700, 7)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '4', 2, 1100, 8)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '5', 3, 1800, 9)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '6', 3, 2300, 10)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '7', 3, 2900, 11)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '8', 3, 3900, 12)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '9', 4, 5000, 13)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '10', 4, 5900, 14)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '11', 4, 7200, 15)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '12', 4, 8400, 16)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '13', 5, 10000, 17)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '14', 5, 11500, 18)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '15', 5, 13000, 19)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '16', 5, 15000, 20)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '17', 6, 18000, 21)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '18', 6, 20000, 22)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '19', 6, 22000, 23)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '20', 6, 25000, 24)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '21', 7, 33000, 25)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '22', 7, 41000, 26)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '23', 7, 50000, 27)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '24', 7, 62000, 28)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '25', 8, 75000, 29)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '26', 8, 90000, 30)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '27', 8, 105000, 31)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '28', 8, 120000, 32)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '29', 9, 135000, 33)
	INSERT INTO MonsterChallengeRatings VALUES (NEWID(), '30', 9, 155000, 34)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM MonsterSizes)
BEGIN
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Tiny', 1)
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Small', 2)
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Medium', 3)
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Large', 4)
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Huge', 5)
	INSERT INTO MonsterSizes VALUES (NEWID(), 'Gargantuan', 6)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM MonsterTypes)
BEGIN
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Abberation')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Beast')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Celestial')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Construct')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Dragon')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Elemental')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Fey')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Fiend')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Giant')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Humanoid')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Monstrosity')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Ooze')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Plant')
	INSERT INTO MonsterTypes VALUES (NEWID(), 'Undead')
END