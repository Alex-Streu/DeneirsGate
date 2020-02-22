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

        public static SelectList AlignmentDropdown(string alignmentKey)
        {
            var list = new SelectList(PresetSvc.GetAlignments(), "Key", "Value", alignmentKey);
            return list;
        }

        public static string CampaignPortrait(this UrlHelper urlHelper, Guid campaignKey, string image)
        {
            var path = AppLogic.GetCampaignContentDir(campaignKey);

            if (image.IsNullOrEmpty()) { return urlHelper.Content(AppLogic.GetDefaultCampaignImage()); }
            return urlHelper.Content(path + image);
        }

        public static string CharacterPortrait(this UrlHelper urlHelper, Guid campaignKey, Guid characterKey, string image)
        {
            var path = $"{AppLogic.GetCharacterContentDir(campaignKey, characterKey)}{image}";

            if (image.IsNullOrEmpty()) { return urlHelper.Content(AppLogic.GetDefaultPortrait()); }
            return urlHelper.Content(path);
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

        public static MvcHtmlString RenderFancyTextbox(string size, string id, int? limit, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}'>
                            <input type='text' id='{id}' autocomplete='screw-autocomplete' required='' {(limit == null ? "" : $"maxlength='{limit}'")} value='{value}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyNumberbox(int min, int max, string size, string id, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textbox {size ?? ""}'>
                            <input type='number' min='{min}' max='{max}' id='{id}' autocomplete='screw-autocomplete' required='' value='{value}' />     
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderFancyTextarea(string size, string id, int? limit, string value, string label, MvcHtmlString extra = null)
        {
            var str = $@"<div class='fancy-textarea {size ?? ""}'>
                            {(String.IsNullOrEmpty(label) ? "" : $"<label for='{id}'>{label}</label>")}
                            <textarea class='form-control' id='{id}' {(limit == null ? "" : $"maxlength='{limit}'")}>{value}</textarea>
                            {(extra == null ? "" : extra.ToString())}
                        </div>";

            return new MvcHtmlString(str);
        }

        public static MvcHtmlString RenderImageUpload(this UrlHelper urlHelper, string id, string value, Guid campaignKey, Guid contentKey)
        {
            var str = $@"<div class='upload-image' data-campaign='{campaignKey}'>
                            <input id='{id}' type='text' class='image-name hidden' value='{value}' />
                            <input class='uploader hidden' type='file' name='file' accept='image/*' />
                            <img class='img-xs img-responsive' src='{urlHelper.CharacterPortrait(campaignKey, contentKey, value)}' />
                            <div class='overlay'></div>
                        </div>";

            return new MvcHtmlString(str);
        }
    }
}