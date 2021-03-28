using System;
using System.Collections.Generic;

namespace DeneirsGate.Services
{
    public class SettlementViewModel
    {
        public Guid SettlementKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Map { get; set; }
        public List<SettlementLocationViewModel> SettlementLocations { get; set; } = new List<SettlementLocationViewModel>();
    }

    public class SettlementLocationViewModel
    {
        public Guid LocationKey { get; set; }
        public Guid SettlementKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
    }
}
