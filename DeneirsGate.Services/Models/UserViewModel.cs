using System;

namespace DeneirsGate.Services
{
    public class UserViewModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Picture { get; set; }
        public bool IsOnline { get; set; }
    }
}
