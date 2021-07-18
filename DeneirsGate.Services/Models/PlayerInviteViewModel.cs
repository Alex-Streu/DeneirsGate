using CSharpVitamins;
using System;

namespace DeneirsGate.Services
{
    public class PlayerInviteViewModel
    {
        public Guid RequestKey { get; set; }
        public string UserName { get; set; }
        public string CharacterName { get; set; }
    }

    public class PlayerInvitePostModel
    {
        public Guid UserKey { get; set; }
        public PlayerInviteTo SendTo { get; set; }
        public Guid CharacterKey { get; set; }
        public ShortGuid CharacterShortKey { get; set; }
    }

    public class PlayerInviteResponseModel
    {
        public Guid RequestKey { get; set; }
        public bool IsAccepted { get; set; }
    }
}
