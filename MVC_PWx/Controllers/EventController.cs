using DeneirsGate.Services;
using DeneirsGateSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeneirsGateSite.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class EventController : DeneirsController
    {
        EventService eventSvc;
        MonsterService monsterSvc;
        MagicItemService magicItemSvc;
        PresetService presetSvc;

        public EventController(EventService eventService, MonsterService monsterService, MagicItemService magicItemService, PresetService presetService)
        {
            eventSvc = eventService;
            monsterSvc = monsterService;
            magicItemSvc = magicItemService;
            presetSvc = presetService;
        }

        public ActionResult _Encounter(Guid? id)
        {
            if (id == null) { id = Guid.NewGuid(); }
            var model = new EncounterViewModel();

            try
            {
                model = eventSvc.GetEncounter(id.Value);
                monsterSvc.GetEncounterMonsters(AppUser.UserId, model);
                magicItemSvc.GetEncounterItems(AppUser.UserId, model);

                ViewBag.Sizes = new SelectList(monsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(monsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(monsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new SelectList(presetSvc.GetEnvironments(), "EnvironmentKey", "Name");

                ViewBag.ItemTypes = new SelectList(magicItemSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.Rarities = new SelectList(magicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Attunements = new SelectList(magicItemSvc.GetAttunements(), "Attunement", "Name");
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult _EncounterPost(EncounterPostModel postModel)
        {
            var model = new EncounterViewModel();

            try
            {
                model = new EncounterViewModel
                {
                    Description = postModel.Description,
                    EncounterKey = postModel.EncounterKey,
                    Items = postModel.Items.Select(x => new MagicItemViewModel { ItemKey = x.ItemKey }).ToList(),
                    Monsters = postModel.Monsters.Select(x => new EncounterMonsterViewModel { MonsterKey = x.MonsterKey, Count = x.Count }).ToList(),
                    Name = postModel.Name,
                    RewardSummary = postModel.RewardSummary
                };

                monsterSvc.GetEncounterMonsters(AppUser.UserId, model);
                magicItemSvc.GetEncounterItems(AppUser.UserId, model);

                ViewBag.Sizes = new SelectList(monsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(monsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(monsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new SelectList(presetSvc.GetEnvironments(), "EnvironmentKey", "Name");

                ViewBag.ItemTypes = new SelectList(magicItemSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.Rarities = new SelectList(magicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Attunements = new SelectList(magicItemSvc.GetAttunements(), "Attunement", "Name");
            }
            catch (Exception ex)
            {
                return HandleExceptionRedirectErrorPartial(ex);
            }

            return PartialView("_Encounter", model);
        }

        [HttpPost]
        public JsonResult UpdateEncounter(EncounterPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    eventSvc.UpdateEncounter(model);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Updated successfully!");
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost, HasCampaign]
        public JsonResult SuggestMonster(SuggestMonsterPostModel model)
        {
            var monster = new MonsterViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var monsterKey = eventSvc.SuggestMonster(AppUser.UserId, AppUser.ActiveCampaign.Value, model.Difficulty, model.DifficultyChange, model.ExcludeMonsters);
                    if (monsterKey == Guid.Empty) { throw new Exception("No available monsters were found!"); }

                    monster = monsterSvc.GetMonster(AppUser.UserId, monsterKey);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Retrieved successfully!", monster);
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost, HasCampaign]
        public JsonResult SearchMonster(SearchMonsterPostModel model)
        {
            var monsters = new List<MonsterViewModel>();
            try
            {
                var keys = eventSvc.SearchMonster(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                foreach (var key in keys)
                {
                    if (key != Guid.Empty) { monsters.Add(monsterSvc.GetMonster(AppUser.UserId, key)); }
                }
                monsters = monsters.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully!", monsters);
        }

        [HttpPost, HasCampaign]
        public JsonResult GetCalculators()
        {
            var model = new EncounterCalculatorsViewModel();
            try
            {
                model.Thresholds = eventSvc.GetThresholds(AppUser.UserId, AppUser.ActiveCampaign.Value);
                model.Multipliers = eventSvc.GetMultipliers();
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully!", model);
        }

        [HttpPost, HasCampaign]
        public JsonResult SuggestItem(SuggestItemPostModel model)
        {
            var item = new MagicItemViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var itemKey = eventSvc.SuggestItem(AppUser.UserId, AppUser.ActiveCampaign.Value, model.Rarity, model.RarityChange, model.ExcludeItems);
                    if (itemKey == Guid.Empty) { throw new Exception("No available items were found!"); }

                    item = magicItemSvc.GetMagicItem(AppUser.UserId, itemKey);
                }
                catch (Exception ex)
                {
                    return HandleExceptionJsonErrorResponse(ex);
                }

                return GetJson(true, "Retrieved successfully!", item);
            }
            return HandleValidationJsonErrorResponse();
        }

        [HttpPost, HasCampaign]
        public JsonResult SearchItem(SearchItemPostModel model)
        {
            var items = new List<MagicItemViewModel>();
            try
            {
                var keys = eventSvc.SearchItem(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                foreach (var key in keys)
                {
                    if (key != Guid.Empty) { items.Add(magicItemSvc.GetMagicItem(AppUser.UserId, key)); }
                }
                items = items.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully!", items);
        }

        [HttpPost, HasCampaign]
        public JsonResult GenerateTreasure(GenerateTreasurePostModel model)
        {
            var treasure = new TreasureViewModel();
            try
            {
                treasure = eventSvc.GenerateTreasure(model.ChallengeRatings);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully!", treasure);
        }

        [HttpPost, HasCampaign]
        public JsonResult GenerateTreasureHoard(GenerateTreasureHoardPostModel model)
        {
            var treasure = new TreasureHoardViewModel();
            try
            {
                treasure = eventSvc.GenerateTreasureHoard(model.ChallengeRating);
            }
            catch (Exception ex)
            {
                return HandleExceptionJsonErrorResponse(ex);
            }

            return GetJson(true, "Retrieved successfully!", treasure);
        }
    }
}