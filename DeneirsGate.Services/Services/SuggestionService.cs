using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class SuggestionService : DeneirsService
    {
        public enum SuggestionType
        {
            FirstName,
            LastName,
            Fear,
            Ideal,
            Backstory,
            Arc,
            Quest,
            Event,
            Monster,
            MagicItem,
            Dungeon,
            Trap,
            Settlement
        }

        public enum SuggestionStatus
        {
            Pending,
            Approved,
            Rejected
        }

        private void UserHasSuggestionAccess(Guid userId, Guid contentKey, bool isAdmin = false)
        {
            DBReset();

            if (isAdmin || DB.Suggestions.FirstOrDefault(x => x.UserKey == userId && x.SuggestionKey == contentKey) != null)
            {
                return;
            }

            throw new Exception("You do not have access to this content!");
        }

        public List<SuggestionViewModel> GetSuggestions(Guid userId)
        {
            DBReset();

            var suggestions = DB.Suggestions.Where(x => x.UserKey == userId).Select(x => new SuggestionViewModel
            {
                Status = (SuggestionStatus)x.Status,
                Suggestion = x.SuggestionText,
                SuggestionKey = x.SuggestionKey,
                Type = (SuggestionType)x.Type,
                UserKey = x.UserKey,
                DateSubmitted = x.DateSubmitted
            }).OrderBy(x => x.DateSubmitted).ToList();

            return suggestions;
        }

        public List<SuggestionViewModel> GetPendingSuggestions(Guid userId, bool isAdmin = false)
        {
            DBReset();

            var dbSuggestions = new List<Suggestion>();
            if (isAdmin)
            {
                dbSuggestions = DB.Suggestions.Where(x => x.Status == (int)SuggestionStatus.Pending).ToList();
            }
            else
            {
                dbSuggestions = DB.Suggestions.Where(x => x.UserKey == userId && x.Status == (int)SuggestionStatus.Pending).ToList();
            }

            var suggestions =dbSuggestions.Select(x => new SuggestionViewModel
            {
                Status = (SuggestionStatus)x.Status,
                Suggestion = x.SuggestionText,
                SuggestionKey = x.SuggestionKey,
                Type = (SuggestionType)x.Type,
                UserKey = x.UserKey,
                UserName = DB.AspNetUsers.FirstOrDefault(y => y.UserId == x.UserKey)?.UserName,
                DateSubmitted = x.DateSubmitted
            }).OrderBy(x => x.DateSubmitted).ToList();

            return suggestions;
        }

        public void UpdateSuggestion(Guid userId, SuggestionPostModel model, bool isAdmin = false)
        {
            UserHasSuggestionAccess(userId, model.SuggestionKey, isAdmin);

            DBReset();

            var add = false;
            var suggestion = DB.Suggestions.FirstOrDefault(x => x.SuggestionKey == model.SuggestionKey);
            if (suggestion == null)
            {
                suggestion = new Suggestion
                {
                    SuggestionKey = model.SuggestionKey,
                    UserKey = userId,
                    DateSubmitted = DateTime.Now,
                    Status = (int)SuggestionStatus.Pending
                };
                add = true;
            }

            suggestion.SuggestionText = model.Suggestion;
            suggestion.Type = (int)model.Type;

            if (add)
            {
                DB.Suggestions.Add(suggestion);
            }

            DB.SaveChanges();
        }

        public void ReviewSuggestion(SuggestionReviewModel model)
        {
            DBReset();

            var suggestion = DB.Suggestions.FirstOrDefault(x => x.SuggestionKey == model.SuggestionKey && x.UserKey == model.UserKey);
            if (suggestion == null) { return; }

            suggestion.Status = model.IsApproved ? (int)SuggestionStatus.Approved : (int)SuggestionStatus.Rejected;

            DB.SaveChanges();
        }

        public string GenerateSuggestion(SuggestionType type)
        {
            DBReset();

            var rand = new Random();
            var toSkip = rand.Next(0, DB.Suggestions.Where(x => x.Type == (int)type && x.Status == (int)SuggestionStatus.Approved).Count());

            var suggestion = DB.Suggestions.Where(x => x.Type == (int)type && x.Status == (int)SuggestionStatus.Approved).OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault()?.SuggestionText;

            return suggestion;
        }

        public CharacterViewModel GenerateCharacter()
        {
            var character = new CharacterViewModel
            {
                FirstName = GenerateSuggestion(SuggestionType.FirstName),
                LastName = GenerateSuggestion(SuggestionType.LastName),
                Fears = GenerateSuggestion(SuggestionType.Fear),
                Ideals = GenerateSuggestion(SuggestionType.Ideal),
                Backstory = GenerateSuggestion(SuggestionType.Backstory)
            };

            DBReset();


            var rand = new Random();
            var toSkip = rand.Next(0, DB.Races.Count());
            character.RaceKey = DB.Races.OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault().RaceKey;

            toSkip = rand.Next(0, DB.Classes.Count());
            character.ClassKey = DB.Classes.OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault().ClassKey;

            toSkip = rand.Next(0, DB.Backgrounds.Count());
            character.BackgroundKey = DB.Backgrounds.OrderBy(x => Guid.NewGuid()).Skip(toSkip).Take(1).FirstOrDefault().BackgroundKey;

            var randStats = new List<int>();
            randStats.Add(rand.Next(16, 20));
            randStats.Add(rand.Next(6, 9));
            randStats.Add(rand.Next(10, 15));
            randStats.Add(rand.Next(10, 15));
            randStats.Add(rand.Next(10, 15));
            randStats.Add(rand.Next(10, 15));

            var stats = new List<int>();
            do
            {
                toSkip = rand.Next(0, randStats.Count);
                var addStat = randStats[toSkip];
                stats.Add(addStat);
                randStats.Remove(addStat);
            } while (randStats.Count > 0);

            character.Strength = stats[0];
            character.Dexterity = stats[1];
            character.Constitution = stats[2];
            character.Wisdom = stats[3];
            character.Intelligence = stats[4];
            character.Charisma = stats[5];

            return character;
        }

        public void DeleteSuggestion(Guid userId, Guid suggestionId)
        {
            DBReset();

            DB.Suggestions.RemoveRange(x => x.UserKey == userId && x.SuggestionKey == suggestionId);

            DB.SaveChanges();
        }

        public Dictionary<int, string> GetSuggestionTypeList()
        {
            var items = new Dictionary<int, string>();
            items.Add((int)SuggestionType.Arc, GetSuggestionTypeDisplay(SuggestionType.Arc));
            items.Add((int)SuggestionType.Backstory, GetSuggestionTypeDisplay(SuggestionType.Backstory));
            items.Add((int)SuggestionType.Dungeon, GetSuggestionTypeDisplay(SuggestionType.Dungeon));
            items.Add((int)SuggestionType.Event, GetSuggestionTypeDisplay(SuggestionType.Event));
            items.Add((int)SuggestionType.Fear, GetSuggestionTypeDisplay(SuggestionType.Fear));
            items.Add((int)SuggestionType.FirstName, GetSuggestionTypeDisplay(SuggestionType.FirstName));
            items.Add((int)SuggestionType.Ideal, GetSuggestionTypeDisplay(SuggestionType.Ideal));
            items.Add((int)SuggestionType.LastName, GetSuggestionTypeDisplay(SuggestionType.LastName));
            items.Add((int)SuggestionType.MagicItem, GetSuggestionTypeDisplay(SuggestionType.MagicItem));
            items.Add((int)SuggestionType.Monster, GetSuggestionTypeDisplay(SuggestionType.Monster));
            items.Add((int)SuggestionType.Quest, GetSuggestionTypeDisplay(SuggestionType.Quest));
            items.Add((int)SuggestionType.Settlement, GetSuggestionTypeDisplay(SuggestionType.Settlement));
            items.Add((int)SuggestionType.Trap, GetSuggestionTypeDisplay(SuggestionType.Trap));

            return items;
        }

        public static string GetSuggestionTypeDisplay(SuggestionType type)
        {
            switch (type)
            {
                case SuggestionType.Arc:
                    return "Arc";
                case SuggestionType.Backstory:
                    return "Backstory";
                case SuggestionType.Dungeon:
                    return "Dungeon";
                case SuggestionType.Event:
                    return "Event";
                case SuggestionType.Fear:
                    return "Fear";
                case SuggestionType.FirstName:
                    return "First Name";
                case SuggestionType.Ideal:
                    return "Ideal";
                case SuggestionType.LastName:
                    return "Last Name";
                case SuggestionType.MagicItem:
                    return "Magic Item";
                case SuggestionType.Monster:
                    return "Monster";
                case SuggestionType.Quest:
                    return "Quest";
                case SuggestionType.Settlement:
                    return "Settlement";
                case SuggestionType.Trap:
                    return "Trap";
            }

            return null;
        }
    }
}
