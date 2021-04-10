using System;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class TodoItemViewModel
    {
        public Guid ItemKey { get; set; }
        public Guid ArcKey { get; set; }
        public DateTime DateLogged { get; set; }
        [Required, StringLength(50)]
        public string Text { get; set; }
    }

    public class TodoItemDeleteModel
    {
        public Guid ItemKey { get; set; }
        public Guid ArcKey { get; set; }
    }
}
