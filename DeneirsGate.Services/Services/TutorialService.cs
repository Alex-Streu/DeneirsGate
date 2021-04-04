using DeneirsGate.Data;
using System;
using System.Linq;

namespace DeneirsGate.Services
{
    public class TutorialService : DeneirsService
    {
        public Guid GetTutorial(string route, string name)
        {
            if (route.IsNullOrEmpty() || name.IsNullOrEmpty())
            {
                throw new Exception("Empty route or tutorial name!");
            }

            DBReset();

            var tutorial = DB.Tutorials.FirstOrDefault(x => x.Route == route && x.Name == name);
            if (tutorial == null)
            {
                tutorial = new Tutorial
                {
                    TutorialKey = Guid.NewGuid(),
                    Name = name,
                    Route = route
                };
                DB.Tutorials.Add(tutorial);
                DB.SaveChanges();
            }

            return tutorial.TutorialKey;
        }

        public UserTutorialViewModel GetUserTutorial(Guid userId, string route, string name)
        {
            var tutorial = new UserTutorialViewModel();
            try
            {
                var tutorialKey = GetTutorial(route, name);
                var userTutorial = DB.UserTutorials.FirstOrDefault(x => x.UserKey == userId && x.TutorialKey == tutorialKey);

                tutorial = new UserTutorialViewModel(tutorialKey, userTutorial);
            }
            catch (Exception ex) { throw ex; }

            return tutorial;
        }

        public void UpdateUserTutorial(Guid userId, Guid tutorialId, bool isComplete, int lastStep)
        {
            DBReset();

            var userTutorial = DB.UserTutorials.FirstOrDefault(x => x.UserKey == userId && x.TutorialKey == tutorialId);
            if (userTutorial == null)
            {
                userTutorial = new UserTutorial
                {
                    TutorialKey = tutorialId,
                    UserKey = userId
                };
                DB.UserTutorials.Add(userTutorial);
            }

            userTutorial.IsComplete = isComplete;
            userTutorial.LastStep = lastStep;

            DB.SaveChanges();
        }

        public void DeleteTutorial(Guid tutorialId)
        {
            DBReset();

            DB.Tutorials.RemoveRange(x => x.TutorialKey == tutorialId);
            DB.UserTutorials.RemoveRange(x => x.TutorialKey == tutorialId);

            DB.SaveChanges();
        }
    }
}
