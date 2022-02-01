using System.ComponentModel.DataAnnotations;

namespace Backgammon.Services.Identity.Api.Contracts.Requests
{
    public class RenameRequest
    {
        [Required]
        public string NewUsername { get; set; }
    }
}
