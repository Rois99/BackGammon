using System.ComponentModel.DataAnnotations;

namespace Backgammon.Services.Identity.Api.Contracts.Requests
{
    public class LogoutRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
