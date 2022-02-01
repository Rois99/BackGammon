using System;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Application.Results;
using Backgammon.Services.Identity.Domain.Models;

namespace Backgammon.Services.Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result> RegisterNewUserAsync(string username, string password, string confirmPassword);
        Task<Result> ChangeUserName(Guid userId, string newName);
        Task<Result> GetUsername(Guid userId);
    }
}
