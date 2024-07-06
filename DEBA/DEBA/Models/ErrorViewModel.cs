// This part of the code is to check if the error has occured. If there is an error it will make sure that it will not crash the website.
namespace DEBA.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
