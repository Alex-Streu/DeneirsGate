using System;
using System.Collections.Generic;

namespace DeneirsGate.Services
{
    public class UserViewModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Picture { get; set; }
        public bool IsOnline { get; set; }
    }

    public class FriendViewModel : UserViewModel
    {
        public FriendStatus Status { get; set; }
        public string StatusMessage { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class AllFriendsViewModel
    {
        public List<FriendViewModel> Friends { get; set; } = new List<FriendViewModel>();
        public List<FriendViewModel> Requests { get; set; } = new List<FriendViewModel>();
        public List<FriendViewModel> Pending { get; set; } = new List<FriendViewModel>();
        public List<FriendViewModel> Blocked { get; set; } = new List<FriendViewModel>();
    }

    public class SearchUserViewModel : FriendViewModel
    {
        public bool CanAdd { get; set; }
    }

    public class SearchUserPostModel
    {
        public string Search { get; set; }
    }

    public class FriendRequestPostModel
    {
        public Guid RequestUserId { get; set; }
        public string RequestUserName { get; set; }
        public FriendStatus Status { get; set; }
    }

    public class UpdateFriendRequestPostModel
    {
        public Guid SenderId { get; set; }
        public FriendStatus Status { get; set; }
    }
}
