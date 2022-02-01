using System;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Application.Interfaces;
using Backgammon.Services.Identity.Application.Results;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Domain.Models;

namespace Backgammon.Services.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManager manager;

        public UserService(IUserManager manager)
        {
            this.manager = manager;
        }

        public async Task<Result> ChangeUserName(Guid userId, string newName)
        {
            if (!await manager.IsExistByIdAsync(userId))
                return new FailedResult()
                {
                    Errors = new[]
                    {
                        "User doesn't exist."
                    }
                };


            var result = await manager.ChangeUsername(userId, newName);

            if(!result.IsSuccess)
                return new FailedResult()
                {
                    Errors = result.Errors
                };


            return new Result(true);
        }

        public async Task<Result> GetUsername(Guid userId)
        {
            if (!await manager.IsExistByIdAsync(userId))
                return new FailedResult()
                {
                    Errors = new[]
                    {
                        "User doesn't exist."
                    }
                };

            var username = await manager.GetUsernameAsync(userId);

            return new NameResult()
            {
                Userame = username
            };
        }

        public async Task<Result> RegisterNewUserAsync(string userName, string password,string confirmPassword)
        {
            if(password != confirmPassword)
                return new FailedResult()
                {
                    Errors = new[]
                    {
                        "Confirm password doesn't match the password."
                    }
                };

            var user = new User()
            {
                UserName = userName
            };
            var result = await manager.CreateAsync(user, password);

            if (!result.IsSuccess)
                return new FailedResult()
                {
                    Errors = result.Errors
                };

            var claimResult = await manager.CreateDefaultClaimsAsync(result.User);

            if (!result.IsSuccess)
                return new FailedResult()
                {
                    Errors = result.Errors
                };

            return new RegisterResult();

        }
    }
}
