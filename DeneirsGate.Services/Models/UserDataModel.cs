﻿using System;

namespace DeneirsGate.Services
{
    public class UserDataModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public Guid Role { get; set; }
        public int Priviledge { get; set; }
        public string Picture { get; set; }
    }

    public class RoleDataModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Priviledge { get; set; }
    }
}
