using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class CampaignService : DeneirsService
    {

        private enum ContentType
        {
            Arc
        };

        public enum ActivityLogType
        {
            Arc,
            Character,
            Event,
            Dungeon
        }

        private void UserHasArcAccess(Guid userId, Guid contentKey, ContentType type)
        {
            using (DBReset())
            {
                switch (type)
                {
                    case ContentType.Arc:
                        //Check if exists
                        if (DB.Arcs.FirstOrDefault(x => x.ArcKey == contentKey) == null) { return; }

                        //Check campaign owner
                        var campaignKeys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
                        if (DB.Arcs.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.ArcKey == contentKey) != null)
                        {
                            return;
                        }
                        break;
                }
            }

            throw new Exception("You do not have access to this content!");
        }

        public enum QuestStatus
        {
            Incomplete = 0,
            InProgress,
            Abandoned,
            Completed
        }

        public string GetCampaignName(Guid campaignId)
        {
            DBReset();
            return DB.Campaigns.FirstOrDefault(x => x.CampaignKey == campaignId)?.Name;
        }

        public CampaignViewModel CreateCampaign(Guid campaignId)
        {
            DBReset();
            if (DB.Campaigns.FirstOrDefault(x => x.CampaignKey == campaignId) != null)
            {
                return null;
            }

            return new CampaignViewModel
            {
                CampaignKey = campaignId
            };
        }

        public CampaignViewModel GetCampaign(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            DBReset();
            var campaign = CreateCampaign(campaignId);
            if (campaign == null)
            {
                campaign = DB.Campaigns.Where(x => x.CampaignKey == campaignId).Select(x => new CampaignViewModel
                {
                    CampaignKey = x.CampaignKey,
                    Description = x.Description,
                    Name = x.Name,
                    Portrait = x.Portrait,
                    LastUpdated = x.LastUpdated
                }).FirstOrDefault();
            }

            return campaign;
        }

        public void UpdateCampaign(Guid userId, CampaignPostModel model)
        {
            UserHasAccess(userId, model.CampaignKey);

            DBReset();
            var campaign = DB.Campaigns.FirstOrDefault(x => x.CampaignKey == model.CampaignKey);

            var add = false;
            if (campaign == null)
            {
                campaign = new Campaign
                {
                    CampaignKey = model.CampaignKey
                };
                add = true;
            }

            campaign.Name = model.Name;
            campaign.Description = model.Description;
            campaign.Portrait = model.Portrait;
            campaign.LastUpdated = DateTime.Now;

            if (add)
            {
                DB.Campaigns.Add(campaign);
                DB.UserCampaigns.Add(new UserCampaign
                {
                    CampaignKey = model.CampaignKey,
                    UserKey = userId,
                    IsOwner = true
                });
            }

            DB.SaveChanges();
        }

        public void DeleteUserCampaigns(Guid userId)
        {
            foreach (var id in DB.UserCampaigns.Where(x => x.UserKey == userId).Select(x => x.CampaignKey).ToList())
            {
                DeleteCampaign(userId, id);
            }
        }

        public void DeleteCampaign(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            DBReset();

            var arcs = DB.Arcs.Where(x => x.CampaignKey == campaignId).Select(x => x.ArcKey).ToList();
            var characters = DB.CampaignCharacterLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.CharacterKey).ToList();
            var dungeons = DB.CampaignDungeonLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.DungeonKey).ToList();
            var traps = DB.CampaignTrapLinkers.Where(x => x.CampaignKey == campaignId).Select(x => x.TrapKey).ToList();
            var trees = DB.RelationshipTrees.Where(x => x.CampaignKey == campaignId).Select(x => x.TreeKey).ToList();
            var settlements = DB.Settlements.Where(x => x.CampaignKey == campaignId).Select(x => x.SettlementKey).ToList();

            //Arcs
            var quests = DB.Quests.Where(x => arcs.Contains(x.ArcKey)).Select(x => x.QuestKey);
            var questEvents = DB.QuestEvents.Where(x => quests.Contains(x.QuestKey)).Select(x => x.EventKey).ToList();
            var eventEncounters = DB.QuestEventEncounters.Where(x => questEvents.Contains(x.EventKey)).Select(x => x.EncounterKey).ToList();
            DB.ActivityLogs.RemoveRange(x => arcs.Contains(x.ArcKey));
            DB.ArcCharacterLinkers.RemoveRange(x => arcs.Contains(x.ArcKey));
            DB.ArcMapPins.RemoveRange(x => arcs.Contains(x.ArcKey));
            DB.Quests.RemoveRange(x => arcs.Contains(x.ArcKey));
            DB.QuestEvents.RemoveRange(x => quests.Contains(x.QuestKey));
            DB.QuestEventEncounters.RemoveRange(x => eventEncounters.Contains(x.EventKey));
            DB.Encounters.RemoveRange(x => eventEncounters.Contains(x.EncounterKey));
            DB.EncounterItems.RemoveRange(x => eventEncounters.Contains(x.EncounterKey));
            DB.EncounterMonsters.RemoveRange(x => eventEncounters.Contains(x.EncounterKey));
            DB.ArcTodoItems.RemoveRange(x => arcs.Contains(x.ArcKey));

            //Characters
            DB.Characters.RemoveRange(x => characters.Contains(x.CharacterKey));
            DB.CharacterSpells.RemoveRange(x => characters.Contains(x.CharacterKey));
            DB.CharacterWeapons.RemoveRange(x => characters.Contains(x.CharacterKey));

            //Dungeons
            var tiles = DB.DungeonTiles.Where(x => dungeons.Contains(x.DungeonKey)).Select(x => x.TileKey).ToList();
            var tileEncounters = DB.DungeonTileEncounters.Where(x => tiles.Contains(x.TileKey)).Select(x => x.EncounterKey).ToList();
            DB.Dungeons.RemoveRange(x => dungeons.Contains(x.DungeonKey));
            DB.DungeonTiles.RemoveRange(x => dungeons.Contains(x.DungeonKey));
            DB.DungeonTileEncounters.RemoveRange(x => tiles.Contains(x.TileKey));
            DB.DungeonTileTraps.RemoveRange(x => tiles.Contains(x.TileKey));
            DB.Encounters.RemoveRange(x => tileEncounters.Contains(x.EncounterKey));
            DB.EncounterItems.RemoveRange(x => tileEncounters.Contains(x.EncounterKey));
            DB.EncounterMonsters.RemoveRange(x => tileEncounters.Contains(x.EncounterKey));

            //Traps
            DB.Traps.RemoveRange(x => traps.Contains(x.TrapKey));

            //Relationship Trees
            DB.RelationshipTreeCharacters.RemoveRange(x => trees.Contains(x.TreeKey));
            DB.RelationshipTreeTiers.RemoveRange(x => trees.Contains(x.TreeKey));

            //Settlements
            DB.SettlementLocations.RemoveRange(x => settlements.Contains(x.SettlementKey));

            DB.Campaigns.RemoveRange(x => x.CampaignKey == campaignId);
            DB.Arcs.RemoveRange(x => x.CampaignKey == campaignId);
            DB.CampaignCharacterLinkers.RemoveRange(x => x.CampaignKey == campaignId);
            DB.CampaignDungeonLinkers.RemoveRange(x => x.CampaignKey == campaignId);
            //DB.CampaignPlayerLinkers.RemoveRange(x => x.CampaignKey == campaignId);
            DB.CampaignTrapLinkers.RemoveRange(x => x.CampaignKey == campaignId);
            DB.CharacterLogs.RemoveRange(x => x.CampaignKey == campaignId);
            //DB.DungeonLogs.RemoveRange(x => x.CampaignKey == campaignId);
            DB.QuestEventLogs.RemoveRange(x => x.CampaignKey == campaignId);
            DB.RelationshipTrees.RemoveRange(x => x.CampaignKey == campaignId);
            DB.UserCampaigns.RemoveRange(x => x.CampaignKey == campaignId);

            DB.SaveChanges();
        }

        public List<CampaignViewModel> GetCampaigns(Guid userId, bool isOwner)
        {
            var campaigns = new List<CampaignViewModel>();

            using (DBReset())
            {
                var keys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner == isOwner).Select(x => x.CampaignKey).ToList();
                campaigns = DB.Campaigns.Where(x => keys.Contains(x.CampaignKey)).Select(x => new CampaignViewModel
                {
                    CampaignKey = x.CampaignKey,
                    Description = x.Description,
                    Name = x.Name,
                    Portrait = x.Portrait,
                    LastUpdated = x.LastUpdated
                }).OrderByDescending(x => x.LastUpdated).ToList();
            }

            return campaigns;
        }

        public CampaignDashboardViewModel GetCampaignDashboard(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            DBReset();
            if (DB.UserCampaigns.Where(x => x.CampaignKey == campaignId && x.UserKey == userId).FirstOrDefault() == null)
            {
                return null;
            }

            var dashboard = new CampaignDashboardViewModel();
            dashboard.CampaignKey = campaignId;
            dashboard.Arc = GetActiveArc(userId, campaignId);
            dashboard.Players = new List<PlayerShortViewModel>();
            dashboard.NPCs = new List<ArcCharacterViewModel>();

            DBReset();
            dashboard.TodoItems = DB.ArcTodoItems.Where(x => x.ArcKey == dashboard.Arc.ArcKey).Select(x => new TodoItemViewModel
            {
                ArcKey = x.ArcKey,
                DateLogged = x.DateLogged,
                ItemKey = x.ItemKey,
                Text = x.Text
            }).OrderBy(x => x.DateLogged).ToList();

            return dashboard;
        }

        public void UpdateTodoItem(TodoItemViewModel model)
        {
            DBReset();

            var item = DB.ArcTodoItems.FirstOrDefault(x => x.ArcKey == model.ArcKey && x.ItemKey == model.ItemKey);
            if (item == null)
            {
                item = new ArcTodoItem
                {
                    ArcKey = model.ArcKey,
                    DateLogged = DateTime.Now,
                    ItemKey = model.ItemKey
                };
                DB.ArcTodoItems.Add(item);
            }

            item.Text = model.Text;

            DB.SaveChanges();
        }

        public void DeleteTodoItem(TodoItemDeleteModel model)
        {
            DBReset();

            DB.ArcTodoItems.RemoveRange(x => x.ArcKey == model.ArcKey && x.ItemKey == model.ItemKey);

            DB.SaveChanges();
        }

        public List<ActivityLogViewModel> GetActivityLog(Guid arcKey)
        {
            DBReset();
            var logs = DB.ActivityLogs.Where(x => x.ArcKey == arcKey).Select(x => new ActivityLogViewModel
            {
                ArcKey = x.ArcKey,
                LogDate = x.DateLogged,
                LogDescription = x.Log,
                LogKey = x.LogKey,
                Type = (ActivityLogType)x.Type
            }).OrderByDescending(x => x.LogDate).ToList();

            foreach (var log in logs.Where(x => x.Type != ActivityLogType.Arc))
            {
                switch (log.Type)
                {
                    case ActivityLogType.Character:
                        log.ContentKey = DB.CharacterLogs.FirstOrDefault(x => x.LogKey == log.LogKey)?.CharacterKey;
                        if (log.ContentKey != null) { log.ContentName = DB.Characters.Where(x => x.CharacterKey == log.ContentKey).Select(x => new { Name = x.FirstName + " " + x.LastName }).Select(x => x.Name).FirstOrDefault(); }
                        break;
                    case ActivityLogType.Event:
                        log.ContentKey = DB.QuestEventLogs.FirstOrDefault(x => x.LogKey == log.LogKey)?.EventKey;
                        if (log.ContentKey != null) { log.ContentName = DB.QuestEvents.Where(x => x.EventKey == log.ContentKey).Select(x => x.Name).FirstOrDefault(); }
                        break;
                }
            }

            return logs;
        }

        public List<ActivityLogViewModel> GetQuestEventLogs(Guid userId, Guid campaignId, Guid questEventId)
        {
            UserHasAccess(userId, campaignId);

            DBReset();
            var logKeys = DB.QuestEventLogs.Where(x => x.EventKey == questEventId && x.CampaignKey == campaignId).Select(x => x.LogKey).ToList();
            var logs = DB.ActivityLogs.Where(x => logKeys.Contains(x.LogKey)).Select(x => new ActivityLogViewModel
            {
                ArcKey = x.ArcKey,
                LogDate = x.DateLogged,
                LogDescription = x.Log,
                LogKey = x.LogKey,
                Type = (ActivityLogType)x.Type
            }).OrderByDescending(x => x.LogDate).ToList();

            return logs;
        }

        public List<ArcViewModel> GetArcs(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            var arcs = new List<ArcViewModel>();
            using (DBReset())
            {
                arcs = DB.Arcs.Where(x => x.CampaignKey == campaignId).Select(x => new ArcViewModel
                {
                    ArcKey = x.ArcKey,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();
            }

            return arcs;
        }

        public ArcViewModel CreateArc(Guid campaignId, Guid arcId)
        {
            using (DBReset())
            {
                if (DB.Arcs.FirstOrDefault(x => x.CampaignKey == campaignId && x.ArcKey == arcId) != null)
                {
                    return null;
                }
            }

            return new ArcViewModel
            {
                ArcKey = arcId,
                Quests = new List<QuestViewModel>()
            };
        }

        public ArcViewModel GetArc(Guid userId, Guid campaignId, Guid arcId)
        {
            UserHasAccess(userId, campaignId);
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            var arc = CreateArc(campaignId, arcId);
            if (arc == null)
            {
                using (DBReset())
                {
                    arc = DB.Arcs.Where(x => x.CampaignKey == campaignId && x.ArcKey == arcId).Select(x => new ArcViewModel
                    {
                        ArcKey = arcId,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        Map = x.Map
                    }).FirstOrDefault();

                    arc.Quests = DB.Quests.Where(x => x.ArcKey == arcId).Select(x => new QuestViewModel
                    {
                        QuestKey = x.QuestKey,
                        Description = x.Description,
                        Name = x.Name,
                        SortOrder = x.SortOrder,
                        Status = (QuestStatus)x.Status
                    }).OrderBy(x => x.SortOrder).ToList();

                    arc.Pins = DB.ArcMapPins.Where(x => x.ArcKey == arcId).Select(x => new ArcMapPinModel
                    {
                        Index = DB.Quests.FirstOrDefault(y => y.QuestKey == x.QuestKey).SortOrder,
                        QuestKey = x.QuestKey,
                        X = x.X,
                        Y = x.Y
                    }).ToList();

                    foreach (var quest in arc.Quests)
                    {
                        quest.Events = DB.QuestEvents.Where(x => x.QuestKey == quest.QuestKey).Select(x => new QuestEventViewModel
                        {
                            EventKey = x.EventKey,
                            Name = x.Name,
                            Description = x.Description,
                            SortOrder = x.SortOrder,
                            IsComplete = x.IsComplete
                        }).OrderBy(x => x.SortOrder).ToList();

                        foreach (var questEvent in quest.Events)
                        {
                            questEvent.Encounter = DB.QuestEventEncounters.Where(x => x.EventKey == questEvent.EventKey).Select(x => new EncounterViewModel
                            {
                                EncounterKey = x.EncounterKey
                            }).FirstOrDefault();
                        }
                    }
                }
            }

            return arc;
        }

        public QuestViewModel GetArcQuest(Guid questId)
        {
            var quest = DB.Quests.Where(x => x.QuestKey == questId).Select(x => new QuestViewModel
            {
                QuestKey = x.QuestKey,
                Description = x.Description,
                Name = x.Name,
                SortOrder = x.SortOrder,
                Status = (QuestStatus)x.Status
            }).FirstOrDefault();

            quest.Events = DB.QuestEvents.Where(x => x.QuestKey == quest.QuestKey).Select(x => new QuestEventViewModel
            {
                EventKey = x.EventKey,
                Name = x.Name,
                Description = x.Description,
                SortOrder = x.SortOrder,
                IsComplete = x.IsComplete
            }).OrderBy(x => x.SortOrder).ToList();

            return quest;
        }

        public QuestEventViewModel GetArcQuestEvent(Guid eventId)
        {
            var _event = DB.QuestEvents.Where(x => x.EventKey == eventId).Select(x => new QuestEventViewModel
            {
                EventKey = x.EventKey,
                Name = x.Name,
                Description = x.Description,
                SortOrder = x.SortOrder,
                IsComplete = x.IsComplete
            }).FirstOrDefault();

            _event.Encounter = DB.QuestEventEncounters.Where(x => x.EventKey == eventId).Select(x => new EncounterViewModel
            {
                EncounterKey = x.EncounterKey
            }).FirstOrDefault();

            return _event;
        }

        public ArcViewModel GetActiveArc(Guid userId, Guid campaignId)
        {
            var arc = new ArcViewModel();
            using (DBReset())
            {
                var arcKey = DB.Arcs.Where(x => x.IsActive && x.CampaignKey == campaignId).Select(x => x.ArcKey).FirstOrDefault();
                arc = GetArc(userId, campaignId, arcKey);
            }

            return arc;
        }

        public void UpdateArcActive(Guid userId, Guid campaignId, ArcActivePostModel model)
        {
            UserHasAccess(userId, campaignId);
            UserHasArcAccess(userId, model.ArcKey, ContentType.Arc);

            using (DBReset())
            {
                foreach (var arc in DB.Arcs.Where(x => x.CampaignKey == campaignId).ToList())
                {
                    if (arc.ArcKey == model.ArcKey) { arc.IsActive = model.IsActive; }
                    else { arc.IsActive = false; }
                }

                DB.SaveChanges();
            }
        }

        public void UpdateArc(Guid userId, Guid campaignId, ArcPostModel model)
        {
            UserHasAccess(userId, campaignId);
            UserHasArcAccess(userId, model.ArcKey, ContentType.Arc);

            using (DBReset())
            {
                var arc = DB.Arcs.FirstOrDefault(x => x.ArcKey == model.ArcKey && x.CampaignKey == campaignId);

                var add = false;
                if (arc == null)
                {
                    arc = new Arc
                    {
                        ArcKey = model.ArcKey,
                        CampaignKey = campaignId
                    };
                    add = true;
                }

                arc.Name = model.Name;
                arc.Description = model.Description;
                arc.Map = model.Map;

                if (add) { DB.Arcs.Add(arc); }

                //Quests
                var questIds = DB.Quests.Where(x => x.ArcKey == model.ArcKey).Select(x => x.QuestKey).ToList();
                var eventIds = DB.QuestEvents.Where(x => questIds.Contains(x.QuestKey)).Select(x => x.EventKey).ToList();
                DB.Quests.RemoveRange(x => x.ArcKey == model.ArcKey);
                DB.QuestEvents.RemoveRange(x => questIds.Contains(x.QuestKey));
                DB.QuestEventEncounters.RemoveRange(x => eventIds.Contains(x.EventKey));
                foreach (var quest in model.Quests)
                {
                    DB.Quests.Add(new Quest
                    {
                        ArcKey = model.ArcKey,
                        QuestKey = quest.QuestKey,
                        Name = quest.Name,
                        Description = quest.Description,
                        SortOrder = quest.SortOrder,
                        Status = (int)quest.Status
                    });

                    var pin = model.Pins.FirstOrDefault(x => x.Index == quest.SortOrder);
                    if (pin != null) { pin.QuestKey = quest.QuestKey; }

                    //Events
                    foreach (var questEvent in quest.Events)
                    {

                        var newEvent = new QuestEvent
                        {
                            EventKey = questEvent.EventKey,
                            QuestKey = quest.QuestKey,
                            Name = questEvent.Name,
                            Description = questEvent.Description,
                            SortOrder = questEvent.SortOrder,
                            IsComplete = questEvent.IsComplete
                        };
                        DB.QuestEvents.Add(newEvent);

                        if (questEvent.Encounter != null)
                        {
                            DB.QuestEventEncounters.Add(new QuestEventEncounter
                            {
                                EncounterKey = questEvent.Encounter.EncounterKey,
                                EventKey = newEvent.EventKey
                            });
                        }
                    }
                }

                //Map Pins
                DB.ArcMapPins.RemoveRange(x => x.ArcKey == model.ArcKey);
                foreach (var pin in model.Pins)
                {
                    DB.ArcMapPins.Add(new ArcMapPin
                    {
                        ArcKey = model.ArcKey,
                        PinKey = Guid.NewGuid(),
                        QuestKey = pin.QuestKey,
                        X = pin.X,
                        Y = pin.Y
                    });
                }

                DB.SaveChanges();

                UpdateArcActive(userId, campaignId, new ArcActivePostModel { ArcKey = model.ArcKey, IsActive = model.IsActive });
            }
        }

        public List<Guid> DeleteArc(Guid userId, Guid campaignId, Guid arcId)
        {
            UserHasAccess(userId, campaignId);
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            var encounterIds = new List<Guid>();
            using (DBReset())
            {
                var questIds = DB.Quests.Where(x => x.ArcKey == arcId).Select(x => x.QuestKey).ToList();
                var eventIds = DB.QuestEvents.Where(x => questIds.Contains(x.QuestKey)).Select(x => x.EventKey).ToList();
                encounterIds = DB.QuestEventEncounters.Where(x => eventIds.Contains(x.EventKey)).Select(x => x.EncounterKey).ToList();

                DB.Arcs.RemoveRange(x => x.ArcKey == arcId);
                DB.Quests.RemoveRange(x => x.ArcKey == arcId);
                DB.QuestEvents.RemoveRange(x => questIds.Contains(x.QuestKey));
                DB.QuestEventEncounters.RemoveRange(x => eventIds.Contains(x.EventKey));

                DB.SaveChanges();
            }

            return encounterIds;
        }

        public List<Guid> GetArcCharacters(Guid userId, Guid campaignId, Guid arcId)
        {
            UserHasAccess(userId, campaignId);
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            DBReset();

            var characterIds = DB.ArcCharacterLinkers.Where(x => x.ArcKey == arcId).Select(x => x.CharacterKey).ToList();

            return characterIds;
        }

        public void AddArcCharacter(Guid arcId, Guid characterId, bool add)
        {
            DBReset();

            var character = DB.ArcCharacterLinkers.FirstOrDefault(x => x.CharacterKey == characterId && x.ArcKey == arcId);
            if (character == null && add)
            {
                DB.ArcCharacterLinkers.Add(new ArcCharacterLinker
                {
                    ArcKey = arcId,
                    CharacterKey = characterId
                });
            }
            else if (character != null && !add)
            {
                DB.ArcCharacterLinkers.Remove(character);
            }

            DB.SaveChanges();
        }

        public void UpdateActivityLog(Guid userId, Guid campaignId, Guid arcId, Guid logId, string logDescription, ActivityLogType type = ActivityLogType.Arc, Guid? contentId = null)
        {
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            DBReset();
            var add = false;
            var log = DB.ActivityLogs.FirstOrDefault(x => x.LogKey == logId);
            if (log == null)
            {
                log = new ActivityLog
                {
                    LogKey = logId,
                    ArcKey = arcId,
                    DateLogged = DateTime.Now
                };
                add = true;
            }

            log.Log = logDescription;
            log.Type = (int)type;

            DB.CharacterLogs.RemoveRange(x => x.LogKey == logId);
            DB.QuestEventLogs.RemoveRange(x => x.LogKey == logId);
            DB.DungeonLogs.RemoveRange(x => x.LogKey == logId);

            if (contentId.HasValue)
            {
                switch (type)
                {
                    case ActivityLogType.Character:
                        DB.CharacterLogs.Add(new CharacterLog
                        {
                            CampaignKey = campaignId,
                            CharacterKey = contentId.Value,
                            LogKey = logId
                        });
                        break;
                    case ActivityLogType.Dungeon:
                        DB.DungeonLogs.Add(new DungeonLog
                        {
                            CampaignKey = campaignId,
                            DungeonKey = contentId.Value,
                            LogKey = logId
                        });
                        break;
                    case ActivityLogType.Event:
                        DB.QuestEventLogs.Add(new QuestEventLog
                        {
                            CampaignKey = campaignId,
                            EventKey = contentId.Value,
                            LogKey = logId
                        });
                        break;
                }
            }

            if (add)
            {
                DB.ActivityLogs.Add(log);
            }

            DB.SaveChanges();
        }

        public void UpdateQuestStatus(Guid userId, Guid arcId, Guid questId, QuestStatus status)
        {
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            DBReset();
            var quest = DB.Quests.FirstOrDefault(x => x.ArcKey == arcId && x.QuestKey == questId);
            quest.Status = (int)status;

            DB.SaveChanges();
        }

        public void DeleteActivityLog(Guid userId, Guid campaignId, Guid arcId, Guid logId)
        {
            UserHasArcAccess(userId, arcId, ContentType.Arc);

            DBReset();
            DB.ActivityLogs.RemoveRange(x => x.LogKey == logId && x.ArcKey == arcId);
            DB.CharacterLogs.RemoveRange(x => x.LogKey == logId && x.CampaignKey == campaignId);
            //DB.DungeonLogs.RemoveRange(x => x.LogKey == logId && x.CampaignKey == campaignId);
            DB.QuestEventLogs.RemoveRange(x => x.LogKey == logId && x.CampaignKey == campaignId);
            DB.SaveChanges();
        }

        public Dictionary<int, string> GetLogTypes()
        {
            var dictionary = new Dictionary<int, string>();
            dictionary.Add((int)ActivityLogType.Arc, "Arc");
            dictionary.Add((int)ActivityLogType.Character, "Character");
            dictionary.Add((int)ActivityLogType.Event, "Quest Event");
            //dictionary.Add((int)ActivityLogType.Dungeon, "Dungeon");

            return dictionary;
        }
    }
}
