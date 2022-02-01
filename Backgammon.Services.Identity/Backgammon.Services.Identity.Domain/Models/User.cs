using System;

namespace Backgammon.Services.Identity.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
    }
}
