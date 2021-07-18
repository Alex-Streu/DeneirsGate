using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class ContentSearchController : DeneirsController
    {
        CampaignService campaignSvc;
        EventService eventSvc;
        CharacterService characterSvc;
        MonsterService monsterSvc;
        MagicItemService magicItemSvc;
        RelationshipTreeService relationshipTreeSvc;
        DungeonService dungeonSvc;
        SettlementService settlementSvc;

        public ContentSearchController(CampaignService campaignService, EventService eventService, CharacterService characterService, MonsterService monsterService,
            MagicItemService magicItemService, RelationshipTreeService relationshipTreeService, DungeonService dungeonService, SettlementService settlementService)
        {
            campaignSvc = campaignService;
            eventSvc = eventService;
            characterSvc = characterService;
            monsterSvc = monsterService;
            magicItemSvc = magicItemService;
            relationshipTreeSvc = relationshipTreeService;
            dungeonSvc = dungeonService;
            settlementSvc = settlementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Arcs

        public ActionResult _Arcs()
        {
            var model = new List<ArcViewModel>();
            try
            {
                model = campaignSvc.GetArcs(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Arc(Guid id)
        {
            var model = new ArcViewModel();
            try
            {
                model = campaignSvc.GetArc(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _ArcLogs(Guid id)
        {
            var model = new List<ActivityLogViewModel>();
            try
            {
                model = campaignSvc.GetActivityLog(id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _ArcQuest(Guid id)
        {
            var model = new QuestViewModel();
            try
            {
                model = campaignSvc.GetArcQuest(id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _ArcQuestEvent(Guid id)
        {
            var model = new QuestEventViewModel();
            try
            {
                model = campaignSvc.GetArcQuestEvent(id);
                if (model.Encounter != null)
                {
                    model.Encounter = eventSvc.GetEncounter(model.Encounter.EncounterKey);
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _QuestEventLogs(Guid id)
        {
            var model = new List<ActivityLogViewModel>();

            try
            {
                model = campaignSvc.GetQuestEventLogs(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Characters

        public ActionResult _Characters()
        {
            var model = new List<CharacterShortViewModel>();
            try
            {
                model = characterSvc.GetAllCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Character(Guid id, bool hideOptions = false)
        {
            var model = new CharacterShortViewModel();
            try
            {
                model = characterSvc.GetPlayerShort(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
                ViewBag.HideOptions = hideOptions;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterInfo(Guid id)
        {
            var model = new CharacterViewModel();
            try
            {
                model = characterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterWeapons(Guid id)
        {
            var model = new CharacterViewModel();
            try
            {
                model = characterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterInventory(Guid id)
        {
            var model = new CharacterViewModel();
            try
            {
                model = characterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterBackstory(Guid id)
        {
            var model = new CharacterViewModel();
            try
            {
                model = characterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterTrees(Guid id)
        {
            var model = new List<RelationshipTreeSearchModel>();

            try
            {
                model = relationshipTreeSvc.GetCharacterTrees(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _CharacterLogs(Guid id)
        {
            var model = new List<ActivityLogViewModel>();

            try
            {
                model = characterSvc.GetCharacterLogs(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Relationship Trees

        public ActionResult _RelationshipTrees()
        {
            var model = new List<RelationshipTreeSearchModel>();

            try
            {
                model = relationshipTreeSvc.GetSearchTrees(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.SearchBy = new SelectList(relationshipTreeSvc.GetSearchDropdown(), "Key", "Value");
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _RelationshipTree(Guid id)
        {

            var model = new RelationshipTreeViewModel();
            try
            {
                model = relationshipTreeSvc.GetRelationshipTree(id, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _ShallowCharacterBackstory(string name, string backstory)
        {
            var model = new CharacterViewModel();
            try
            {
                model = new CharacterViewModel
                {
                    FirstName = name,
                    Backstory = backstory
                };

                ViewBag.CampaignKey = AppUser.ActiveCampaign.Value;
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Monsters

        public ActionResult _Monsters()
        {
            var model = new List<MonsterViewModel>();
            try
            {
                model = monsterSvc.GetMonsters(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Monster(Guid id)
        {
            var model = new MonsterViewModel();
            try
            {
                model = monsterSvc.GetMonster(AppUser.UserId, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Magic Items

        public ActionResult _MagicItems()
        {
            var model = new List<MagicItemViewModel>();
            try
            {
                model = magicItemSvc.GetMagicItems(AppUser.UserId, AppUser.ActiveCampaign.Value, false);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _MagicItem(Guid id)
        {
            var model = new MagicItemViewModel();
            try
            {
                model = magicItemSvc.GetMagicItem(AppUser.UserId, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Dungeons

        public ActionResult _Dungeons()
        {
            var model = new List<DungeonListViewModel>();
            try
            {
                model = dungeonSvc.GetDungeons(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Dungeon(Guid id)
        {
            var model = new DungeonViewModel();
            try
            {
                model = dungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = eventSvc.GetEncounter(tile.Encounter.EncounterKey);
                        monsterSvc.GetEncounterMonsters(AppUser.UserId, tile.Encounter);
                        magicItemSvc.GetEncounterItems(AppUser.UserId, tile.Encounter);
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Traps

        public ActionResult _Traps()
        {
            var model = new List<TrapViewModel>();
            try
            {
                model = dungeonSvc.GetTraps(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Trap(Guid id)
        {
            var model = new TrapViewModel();
            try
            {
                model = dungeonSvc.GetTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion

        #region Settlements

        public ActionResult _Settlements()
        {
            var model = new List<SettlementViewModel>();
            try
            {
                model = settlementSvc.GetSettlements(AppUser.UserId, AppUser.ActiveCampaign.Value);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        public ActionResult _Settlement(Guid id)
        {
            var model = new SettlementViewModel();
            try
            {
                model = settlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        #endregion
    }
}