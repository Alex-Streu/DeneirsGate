using System;
using System.Web.Mvc;

namespace CustomHtmlHelpers
{
    public static class HtmlHelpers
    {
        public static string CampaignPortrait(string image)
        {
            var path = "~\\Content\\img\\campaigns\\";
            var _default = "campaign-default.png";

            if (String.IsNullOrEmpty(image)) { image = _default; }
            return path + image;
        }

        public static MvcHtmlString RenderDateTime(DateTime date)
        {
            var str = date.ToString("M/d/yyyy H:mm tt");
            return new MvcHtmlString(str);
        }
    }
}