using System;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Application.Results;

namespace Backgammon.Services.Identity.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result> LoginAsync(string username,string password);
        Task<Result> ValidateUser(Guid userId, string password);
        Task<Result> LogoutAsync(string refreshToken);
        Task<Result> RefreshAsync(string accessToken, string refreshToken);
    }
}
