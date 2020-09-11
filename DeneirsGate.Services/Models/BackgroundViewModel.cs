using System;

namespace DeneirsGate.Services
{
    public class BackgroundViewModel
    {
        public Guid BackgroundKey { get; set; }
        public string Name { get; set; }
    }

    public class DamageTypeViewModel
    {
        public Guid TypeKey { get; set; }
        public string Name { get; set; }
    }

    public class EnvironmentViewModel
    {
        public Guid EnvironmentKey { get; set; }
        public string Name { get; set; }
    }
}
