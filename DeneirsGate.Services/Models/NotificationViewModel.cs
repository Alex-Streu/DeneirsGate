using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeneirsGate.Services
{
    public class NotificationViewModel
    {
        public Guid NotificationKey { get; set; }
        public Guid UserKey { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public DateTime DateAdded { get; set; }
        public string Age { get; set; }
        public bool IsRead { get; set; }
    }
}
