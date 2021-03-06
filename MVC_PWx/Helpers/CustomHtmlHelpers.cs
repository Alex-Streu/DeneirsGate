﻿using DeneirsGate.Services;
using System;
using System.Web.Mvc;
using MVC_PWx.Helpers;

namespace CustomHtmlHelpers
{
    public static class HtmlHelpers
    {
        private static PresetService presetSvc;

        public static PresetService PresetSvc
        {
            get
            {
                if (presetSvc == null) { presetSvc = new PresetService(); }
                return presetSvc;
            }
        }

        private static string EscapeText(string value)
        {
            if (value.IsNullOrEmpty()) { return value; }

            return value.Replace("'", "&#39;");
        }

        public static SelectList RaceDropdown(Guid raceKey)
        {
            var list = new SelectList(PresetSvc.GetRaces(), "RaceKey", "Name", raceKey);
            return list;
        }

        public static SelectList ClassDropdown(Guid classKey)
        {
            var list = new SelectList(PresetSvc.GetClasses(), "ClassKey", "Name", classKey);
            return list;
        }

        public static SelectList BackgroundDropdown(Guid backgroundKey)
        {
            var list = new SelectList(PresetSvc.GetBackgrounds(), "BackgroundKey", "Name", backgroundKey);
            return list;
        }

        public static SelectList DamageTypeDropdown(Guid typeKey)
        {
            var list = new SelectList(PresetSvc.GetDamageTypes(), "TypeKey", "Name", typeKey);
            return list;
        }

        public static SelectList AlignmentDropdown(string alignmentKey)
        {
            var list = new SelectList(PresetSvc.GetAlignments(), "Key", "Value", alignmentKey);
            return list;
        }

        public static SelectList SpellcastingAbilityDropdown(string abilityKey)
        {
            var list = new SelectList(PresetSvc.GetSpellcastingAbilities(), "Key", "Value", abilityKey);
            return list;
        }

        public static string CampaignPortrait(this UrlHelper urlHelper, Guid campaignKey, string image, bool useDefault = true)
        {
            var path = AppLogic.GetCampaignContentDir(campaignKey);

            if (image.IsNullOrEmpty()) 
            {
                if (useDefault)
                {
                    return urlHelper.Content(AppLogic.GetDefaultCampaignImage());
                }
                else
                {
                    return "";
                }
            }
            return urlHelper.Content(path + image);
        }

        public static string CharacterPortrait(this UrlHelper urlHelper, Guid campaignKey, Guid characterKey, string image)
        {
            var path = $"{AppLogic.GetCharacterContentDir(campaignKey, characterKey)}{image}";

            if (image.IsNullOrEmpty()) { return urlHelper.Content(AppLogic.GetDefaultPortrait()); }
            return urlHelper.Content(path);
        }

        public static string ArcMap(this UrlHelper urlHelper, Guid campaignKey, Guid arcKey, string image)
        {
            var path = $"{AppLogic.GetArcMapContentDir(campaignKey, arcKey)}{image}";

            if (image.IsNullOrEmpty()) { return ""; }
            return urlHelper.Content(path);
        }

        public static string SettlementMap(this UrlHelper urlHelper, Guid campaignKey, Guid settlementKey, string image)
        {
            var path = $"{AppLogic.GetSettlementMapContentDir(campaignKey, settlementKey)}{image}";

            if (image.IsNullOrEmpty()) { return ""; }
            return urlHelper.Content(path);
        }

        public static string DefaultCharacterPortrait(this UrlHelper urlHelper)
        {
            return urlHelper.Content(AppLogic.GetDefaultPortrait());
        }

        public static string DefaultShallowPortrait(this UrlHelper urlHelper)
        {
            return urlHelper.Content(AppLogic.GetDefaultShallowPortrait());
        }

        public static string LoadFavicon(this UrlHelper urlHelper, string file)
        {
            var path = AppLogic.GetIconDir();
            return urlHelper.Content(path + file);
        }

        public static MvcHtmlString RenderDateTime(DateTime date)
        {
            var str = date.ToString("M/d/yyyy H:mm tt");
            return new MvcHtmlString(str);
        }
        public static MvcHtmlString RenderDate(DateTime date)
        {
            var str = date.ToString("M/d/yyyy");
            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyPassword(string size, string id, int? limit, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}'>
                            <input type='password' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} autocomplete='screw-autocomplete' required='' {(limit == null ? "" : $"maxlength='{limit}'")} value='{EscapeText(value)}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyTextbox(string size, string id, int? limit, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}'>
                            <input type='text' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} autocomplete='screw-autocomplete' required='' {(limit == null ? "" : $"maxlength='{limit}'")} value='{EscapeText(value)}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyTextbox(string size, string id, int? limit, string value, string label, SuggestionService.SuggestionType type, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}' style='position:relative'>
                            <div style='display:none' class='btn btn-sm btn-default btn-suggestion' onclick='suggest(`{id}`, `{(int)type}`)'><i class='fa fa-magic'></i></div>
                            <input type='text' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} autocomplete='screw-autocomplete' required='' {(limit == null ? "" : $"maxlength='{limit}'")} value='{EscapeText(value)}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyNumberbox(int min, int max, string size, string id, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}'>
                            <input type='number' min='{min}' max='{max}' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} autocomplete='screw-autocomplete' required='' value='{value}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyTextarea(string size, string id, int? limit, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textarea {size ?? ""}'>
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            <textarea class='form-control' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} {(limit == null ? "" : $"maxlength='{limit}'")}>{value}</textarea>
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyTextarea(string size, string id, int? limit, string value, string label, SuggestionService.SuggestionType type, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textarea {size ?? ""}' style='position:relative'>
                            <div style='display:none' class='btn btn-sm btn-default btn-suggestion btn-suggestion-textarea' onclick='suggest(`{id}`, `{(int)type}`)'><i class='fa fa-magic'></i></div>
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            <textarea class='form-control' {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} {(limit == null ? "" : $"maxlength='{limit}'")}>{value}</textarea>
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderPortraitUpload(this UrlHelper urlHelper, string id, string value, Guid campaignKey, Guid contentKey)
        {
            var str = $@"<div class='upload-image' data-campaign='{campaignKey}'>
                            <input {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} type='text' class='image-name hidden' value='{value}' />
                            <input class='uploader hidden user-image' type='file' name='file' accept='image/*' />
                            <img class='img-xs img-responsive' src='{urlHelper.CharacterPortrait(campaignKey, contentKey, value)}' />
                            <div class='overlay'></div>
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderArcMapUpload(this UrlHelper urlHelper, string id, string value, Guid campaignKey, Guid contentKey)
        {
            var str = $@"<div data-campaign='{campaignKey}'>
                            <div class='btn btn-warning image-upload'><i class='fa fa-upload'></i>&nbsp;Upload Image</div>
                            <input {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} type='text' class='image-name hidden' value='{value}' />
                            <input class='uploader hidden user-image' type='file' name='file' accept='image/*' />
                            <img class='hidden' src='{urlHelper.ArcMap(campaignKey, contentKey, value)}'/>
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderCampaignPortraitUpload(this UrlHelper urlHelper, string id, string value, Guid campaignKey)
        {
            var str = $@"<div data-campaign='{campaignKey}'>
                            <div class='btn btn-warning image-upload'><i class='fa fa-upload'></i>&nbsp;Upload Image</div>
                            <input {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} type='text' class='image-name hidden' value='{value}' />
                            <input class='uploader hidden user-image' type='file' name='file' accept='image/*' />
                            <div class='mt'><img class='img-responsive' src='{urlHelper.CampaignPortrait(campaignKey, value, false)}'/></div>
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderSettlementMapUpload(this UrlHelper urlHelper, string id, string value, Guid campaignKey, Guid contentKey)
        {
            var str = $@"<div data-campaign='{campaignKey}'>
                            <div class='btn btn-warning image-upload'><i class='fa fa-upload'></i>&nbsp;Upload Image</div>
                            <input {(!id.IsNullOrEmpty() ? $"id='{id}' name='{id}'" : "")} type='text' class='image-name hidden' value='{value}' />
                            <input class='uploader hidden user-image' type='file' name='file' accept='image/*' />
                            <img class='hidden' src='{urlHelper.SettlementMap(campaignKey, contentKey, value)}'/>
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderSuggestionButton(string fieldId, SuggestionService.SuggestionType type)
        {
            var str = $@"<div style='display:none' class='btn btn-sm btn-default btn-suggestion' onclick='suggest(`{fieldId}`, `{(int)type}`)'><i class='fa fa-magic'></i></div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderSuggestionButtonTextArea(string fieldId, SuggestionService.SuggestionType type)
        {
            var str = $@"<div style='display:none' class='btn btn-sm btn-default btn-suggestion btn-suggestion-textarea' onclick='suggest(`{fieldId}`, `{(int)type}`)'><i class='fa fa-magic'></i></div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderSuggestionButtonEditor(SuggestionService.SuggestionType type, string editor)
        {
            var str = $@"<div style='display:none' class='btn btn-sm btn-default btn-suggestion btn-suggestion-editor' onclick='suggest(null, `{(int)type}`, {editor})'><i class='fa fa-magic'></i></div>";

            return new MvcHtmlString(str);
        }

        public static string LimitString(string value, int limit = 100)
        {
            if (value.IsNullOrEmpty() || value.Length <= limit) { return value; }

            return String.Concat(value.Substring(0, Math.Min(value.Length, limit)), "...");
        }
    }
}