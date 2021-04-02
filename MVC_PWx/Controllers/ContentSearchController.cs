using DeneirsGate.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class ContentSearchController : DeneirsController
    {
        // GET: ContentSearch
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
                model = CampaignSvc.GetArcs(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
                model = CampaignSvc.GetArc(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
                model = CampaignSvc.GetArcQuest(id);
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
                model = CampaignSvc.GetArcQuestEvent(id);
                if (model.Encounter != null)
                {
                    model.Encounter = EventSvc.GetEncounter(model.Encounter.EncounterKey);
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
                model = CampaignSvc.GetQuestEventLogs(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
                model = CharacterSvc.GetAllCharacters(AppUser.UserId, AppUser.ActiveCampaign.Value);

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
                model = CharacterSvc.GetPlayerShort(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = CharacterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = CharacterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = CharacterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = CharacterSvc.GetPlayer(AppUser.UserId, AppUser.ActiveCampaign.Value, id);

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
                model = RelationshipTreeSvc.GetCharacterTrees(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
                model = CharacterSvc.GetCharacterLogs(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
                model = RelationshipTreeSvc.GetSearchTrees(AppUser.UserId, AppUser.ActiveCampaign.Value);

                ViewBag.SearchBy = new SelectList(RelationshipTreeSvc.GetSearchDropdown(), "Key", "Value");
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
                model = RelationshipTreeSvc.GetRelationshipTree(id, AppUser.ActiveCampaign.Value);
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
                model = MonsterSvc.GetMonsters(AppUser.UserId, AppUser.ActiveCampaign.Value, !User.IsInRole("Admin"));
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
                model = MonsterSvc.GetMonster(AppUser.UserId, id);
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
                model = MagicItemSvc.GetMagicItems(AppUser.UserId, AppUser.ActiveCampaign.Value, false);
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
                model = MagicItemSvc.GetMagicItem(AppUser.UserId, id);
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
                model = DungeonSvc.GetDungeons(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
                model = DungeonSvc.GetDungeon(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
                foreach (var tile in model.Tiles)
                {
                    if (tile.Encounter != null)
                    {
                        tile.Encounter = EventSvc.GetEncounter(tile.Encounter.EncounterKey);
                        MonsterSvc.GetEncounterMonsters(AppUser.UserId, tile.Encounter);
                        MagicItemSvc.GetEncounterItems(AppUser.UserId, tile.Encounter);
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
                model = DungeonSvc.GetTraps(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
                model = DungeonSvc.GetTrap(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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
                model = SettlementSvc.GetSettlements(AppUser.UserId, AppUser.ActiveCampaign.Value);
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
                model = SettlementSvc.GetSettlement(AppUser.UserId, AppUser.ActiveCampaign.Value, id);
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