using System;
using System.ComponentModel.DataAnnotations;
using static DeneirsGate.Services.SuggestionService;

namespace DeneirsGate.Services
{
    public class SuggestionViewModel
    {
        public Guid SuggestionKey { get; set; }
        public Guid UserKey { get; set; }
        public string UserName { get; set; }
        public string Suggestion { get; set; }
        public SuggestionType Type { get; set; }
        public SuggestionStatus Status { get; set; }
        public DateTime DateSubmitted { get; set; }
    }

    public class SuggestionPostModel
    {
        public Guid SuggestionKey { get; set; } = Guid.NewGuid();
        [Required, StringLength(250)]
        public string Suggestion { get; set; }
        public SuggestionType Type { get; set; }
    }

    public class SuggestionReviewModel
    {
        public Guid SuggestionKey { get; set; }
        public Guid UserKey { get; set; }
        public bool IsApproved { get; set; }
    }
}
