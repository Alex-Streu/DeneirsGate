using DeneirsGate.Services;
using System;
using System.IO;
using System.Web.Mvc;

namespace MVC_PWx.Controllers
{
    public class UtilityController : DeneirsController
    {
        [HttpPost]
        public JsonResult UploadImage(UploadImagePostModel model)
        {
            var name = model.IsTemp ? Guid.NewGuid().ToString() + model.FileType : model.Name;
            var path = $"~/Content/CampaignImages/campaign-{model.CampaignKey.ToString()}/";
            if (model.IsTemp) { path += "temp/"; }
            if (!model.Folder.IsNullOrEmpty()) { path += model.Folder + "/"; }

            try
            {
                var filePath = Server.MapPath(path);
                if (!model.IsTemp && Directory.Exists(filePath)) { Directory.Delete(filePath, true); }
                Directory.CreateDirectory(filePath);

                
                var base64 = model.File.Split(',')[1];
                byte[] bytes = Convert.FromBase64String(base64);
                using (var imageFile = new FileStream(filePath + name, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Uploaded successfully!", image = name, imagePath = path + name });
        }
        
        public JsonResult DeleteTemp(Guid campaignKey)
        {
            var path = $"~/Content/CampaignImages/campaign-{campaignKey.ToString()}/temp";
            try
            {
                var fullPath = Server.MapPath(path);
                var dirInfo = new DirectoryInfo(fullPath);
                dirInfo.Delete(true);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Deleted temp successfully!" });
        }
    }
}