using DeneirsGate.Services;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace DeneirsGateSite
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<AuthService, AuthService>();
            container.RegisterType<CampaignService, CampaignService>();
            container.RegisterType<CharacterService, CharacterService>();
            container.RegisterType<PresetService, PresetService>();
            container.RegisterType<UserService, UserService>();
            container.RegisterType<RelationshipTreeService, RelationshipTreeService>();
            container.RegisterType<MonsterService, MonsterService>();
            container.RegisterType<MagicItemService, MagicItemService>();
            container.RegisterType<EventService, EventService>();
            container.RegisterType<DungeonService, DungeonService>();
            container.RegisterType<SettlementService, SettlementService>();
            container.RegisterType<SuggestionService, SuggestionService>();
            container.RegisterType<TutorialService, TutorialService>();
            container.RegisterType<PlayerService, PlayerService>();


            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}