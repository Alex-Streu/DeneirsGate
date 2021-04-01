
namespace DeneirsGate.Services
{
    public class ErrorViewModel
    {
    }

    public class ErrorPostModel
    {
        public string Header { get; set; }
        public string Message { get; set; }
        public string Html { get; set; }
        public string ReturnUrl { get; set; }
        public string Error { get; set; }
    }
}
