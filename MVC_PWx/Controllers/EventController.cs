using DeneirsGate.Services;
using MVC_PWx.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    [Authorize, HasCampaign, HasAccess(Priviledge = AppLogic.Priviledge.DM)]
    public class EventController : DeneirsController
    {
        public ActionResult _Encounter(Guid? id)
        {
            if (id == null) { id = Guid.NewGuid(); }
            var model = new EncounterViewModel();

            try
            {
                model = EventSvc.GetEncounter(id.Value);
                MonsterSvc.GetEncounterMonsters(AppUser.UserId, model);
                MagicItemSvc.GetEncounterItems(AppUser.UserId, model);

                ViewBag.Sizes = new SelectList(MonsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(MonsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(MonsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new SelectList(PresetSvc.GetEnvironments(), "EnvironmentKey", "Name");

                ViewBag.ItemTypes = new SelectList(MagicItemSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.Rarities = new SelectList(MagicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Attunements = new SelectList(MagicItemSvc.GetAttunements(), "Attunement", "Name");
            }
            catch (Exception ex) { }

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

                MonsterSvc.GetEncounterMonsters(AppUser.UserId, model);
                MagicItemSvc.GetEncounterItems(AppUser.UserId, model);

                ViewBag.Sizes = new SelectList(MonsterSvc.GetSizes(), "SizeKey", "Name");
                ViewBag.Types = new SelectList(MonsterSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.ChallengeRatings = new SelectList(MonsterSvc.GetChallengeRatings(), "RatingKey", "Challenge");
                ViewBag.Environments = new SelectList(PresetSvc.GetEnvironments(), "EnvironmentKey", "Name");

                ViewBag.ItemTypes = new SelectList(MagicItemSvc.GetTypes(), "TypeKey", "Name");
                ViewBag.Rarities = new SelectList(MagicItemSvc.GetRarities(), "RarityKey", "Name");
                ViewBag.Attunements = new SelectList(MagicItemSvc.GetAttunements(), "Attunement", "Name");
            }
            catch (Exception ex) { }

            return PartialView("_Encounter", model);
        }

        [HttpPost]
        public JsonResult UpdateEncounter(EncounterPostModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    EventSvc.UpdateEncounter(model);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Updated successfully!" });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost, HasCampaign]
        public JsonResult SuggestMonster(SuggestMonsterPostModel model)
        {
            var monster = new MonsterViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var monsterKey = EventSvc.SuggestMonster(AppUser.UserId, AppUser.ActiveCampaign.Value, model.Difficulty, model.DifficultyChange, model.ExcludeMonsters);
                    if (monsterKey == Guid.Empty) { throw new Exception("No available monsters were found!"); }

                    monster = MonsterSvc.GetMonster(AppUser.UserId, monsterKey);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Retrieved successfully!", data = monster });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost, HasCampaign]
        public JsonResult SearchMonster(SearchMonsterPostModel model)
        {
            var monsters = new List<MonsterViewModel>();
            try
            {
                var keys = EventSvc.SearchMonster(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                foreach (var key in keys)
                {
                    if (key != Guid.Empty) { monsters.Add(MonsterSvc.GetMonster(AppUser.UserId, key)); }
                }
                monsters = monsters.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = monsters });
        }

        [HttpPost, HasCampaign]
        public JsonResult GetCalculators()
        {
            var model = new EncounterCalculatorsViewModel();
            try
            {
                model.Thresholds = EventSvc.GetThresholds(AppUser.UserId, AppUser.ActiveCampaign.Value);
                model.Multipliers = EventSvc.GetMultipliers();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = model });
        }

        [HttpPost, HasCampaign]
        public JsonResult SuggestItem(SuggestItemPostModel model)
        {
            var item = new MagicItemViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    var itemKey = EventSvc.SuggestItem(AppUser.UserId, AppUser.ActiveCampaign.Value, model.Rarity, model.RarityChange, model.ExcludeItems);
                    if (itemKey == Guid.Empty) { throw new Exception("No available items were found!"); }

                    item = MagicItemSvc.GetMagicItem(AppUser.UserId, itemKey);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }

                return Json(new { success = true, message = "Retrieved successfully!", data = item });
            }
            return Json(new { success = false, message = GetValidationError() });
        }

        [HttpPost, HasCampaign]
        public JsonResult SearchItem(SearchItemPostModel model)
        {
            var items = new List<MagicItemViewModel>();
            try
            {
                var keys = EventSvc.SearchItem(AppUser.UserId, AppUser.ActiveCampaign.Value, model);
                foreach (var key in keys)
                {
                    if (key != Guid.Empty) { items.Add(MagicItemSvc.GetMagicItem(AppUser.UserId, key)); }
                }
                items = items.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = items });
        }

        [HttpPost, HasCampaign]
        public JsonResult GenerateTreasure(GenerateTreasurePostModel model)
        {
            var treasure = new TreasureViewModel();
            try
            {
                treasure = EventSvc.GenerateTreasure(model.ChallengeRatings);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = treasure });
        }

        [HttpPost, HasCampaign]
        public JsonResult GenerateTreasureHoard(GenerateTreasureHoardPostModel model)
        {
            var treasure = new TreasureHoardViewModel();
            try
            {
                treasure = EventSvc.GenerateTreasureHoard(model.ChallengeRating);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Retrieved successfully!", data = treasure });
        }
    }
}