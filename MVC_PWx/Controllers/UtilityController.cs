using DeneirsGate.Services;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Web;
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

        [HttpPost]
        public JsonResult UploadMonsterList()
        {
            try
            {
                if (Request != null)
                {
                    HttpPostedFileBase monstersfile = Request.Files["Monsters"];
                    if ((monstersfile != null) && (monstersfile.ContentLength > 0) && !string.IsNullOrEmpty(monstersfile.FileName))   
                    {
                        byte[] fileBytes = new byte[monstersfile.ContentLength];
                        //var data = monstersfile.InputStream.Read(fileBytes, 0, Convert.ToInt32(monstersfile.ContentLength));
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(monstersfile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (var i = 1; i <= noOfRow; i++)   
                            {
                                MonsterSvc.UploadMonster(AppUser.UserId, workSheet.Cells[i, 1].Value.ToString(), null, workSheet.Cells[i, 2].Value.ToString(), workSheet.Cells[i, 3].Value.ToString(), workSheet.Cells[i, 4].Value.ToString(), workSheet.Cells[i, 7].Value.ToString(), workSheet.Cells[i, 19].Value.ToString());
                            }
                        }
                    }

                    HttpPostedFileBase environmentsFile = Request.Files["Environments"];
                    if ((environmentsFile != null) && (environmentsFile.ContentLength > 0) && !string.IsNullOrEmpty(environmentsFile.FileName))
                    {
                        byte[] fileBytes = new byte[environmentsFile.ContentLength];
                        //var data = environmentsFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(environmentsFile.ContentLength));
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (var package = new ExcelPackage(environmentsFile.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;
                            for (var i = 1; i <= noOfRow; i++)
                            {
                                var monsters = workSheet.Cells[i, 1].Value.ToString().Split(',');
                                var environment = workSheet.Cells[i, 2].Value.ToString();
                                foreach (var monster in monsters)
                                {
                                    MonsterSvc.UploadMonsterEnvironments(monster.Trim(), environment);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = true, message = "Uploaded successfully!" });
        }
    }
}