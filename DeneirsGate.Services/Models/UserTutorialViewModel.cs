using DeneirsGate.Data;
using System;

namespace DeneirsGate.Services
{
    public class UserTutorialViewModel
    {
        public Guid TutorialKey { get; set; }
        public bool IsComplete { get; set; }
        public int LastStep { get; set; }

        public UserTutorialViewModel()
        {

        }

        public UserTutorialViewModel(Guid tutorialKey, UserTutorial userTutorial)
        {
            TutorialKey = tutorialKey;
            IsComplete = userTutorial?.IsComplete ?? false;
            LastStep = userTutorial?.LastStep ?? 0;
        }
    }

    public class UserTutorialQueryModel
    {
        public string Route { get; set; }
        public string Name { get; set; }
    }

    public class UserTutorialPostModel : UserTutorialViewModel
    {

    }
}
