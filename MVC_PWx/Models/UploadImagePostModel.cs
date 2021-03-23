using System;

namespace DeneirsGate.Services
{
    public class UploadImagePostModel
    {
        public Guid CampaignKey { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
        public string File { get; set;}
        public string FileType { get; set; }
        public bool IsTemp { get; set; }
    }

    public class SaveTempImagePostModel
    {
        public Guid CampaignKey { get; set; }
        public string Folder { get; set; }
        public string Name { get; set; }
    }

    public class DeleteTempPostModel
    {
        public Guid CampaignKey { get; set; }
    }

    public class UploadFilePostModel
    {
        public string Name { get; set; }
        public string File { get; set; }
    }
}
