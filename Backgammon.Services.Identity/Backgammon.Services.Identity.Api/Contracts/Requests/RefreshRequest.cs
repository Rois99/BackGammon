using System.ComponentModel.DataAnnotations;

namespace Backgammon.Services.Identity.Contracts.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
