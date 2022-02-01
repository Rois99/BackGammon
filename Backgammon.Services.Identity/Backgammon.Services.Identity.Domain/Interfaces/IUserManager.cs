using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Domain.Models;
using Backgammon.Services.Identity.Domain.Results;

namespace Backgammon.Services.Identity.Domain.Interfaces
{
    public interface IUserManager
    {
        Task<CreateUserResult> CreateAsync(User user,string password);
        Task<CreateUserResult> CreateDefaultClaimsAsync(User user);

        Task<bool> IsExistByUserNameAsync(string userName);
        Task<bool> CheckPassword(string userName, string password);
        Task<bool> CheckPassword(Guid userId, string password);

        Task<IList<Claim>> GetUserClaimsAsync(string userName);
        Task<string> GetUsernameAsync(Guid userId);

        Task<ChangeNameResult> ChangeUsername(Guid userId, string newName);
        Task<bool> IsExistByIdAsync(Guid userId);
    }
}
