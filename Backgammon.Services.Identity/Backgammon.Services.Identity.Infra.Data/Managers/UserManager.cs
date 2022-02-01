using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Domain.Models;
using Backgammon.Services.Identity.Infra.Data.Dtos;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Backgammon.Services.Identity.Domain.Results;

namespace Backgammon.Services.Identity.Infra.Data.Managers
{
    public class UserManager : UserManager<UserDto> , IUserManager
    {
        public UserManager(IUserStore<UserDto> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<UserDto> passwordHasher, IEnumerable<IUserValidator<UserDto>> userValidators, IEnumerable<IPasswordValidator<UserDto>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<UserDto>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public async Task<ChangeNameResult> ChangeUsername(Guid userId, string newName)
        {
            if (string.IsNullOrEmpty(userId.ToString()))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(newName))
                throw new ArgumentNullException();

            var userDto = await FindByIdAsync(userId.ToString());

            if (userDto == null)
                throw new ArgumentException();

            var oldName = userDto.UserName;

            var result = await SetUserNameAsync(userDto, newName);

            if (!result.Succeeded)
                return new ChangeNameResult(false, result.Errors.Select(e => e.Description));

            var oldSubClaim = (await GetClaimsAsync(userDto)).FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var newClaim = new Claim(JwtRegisteredClaimNames.Sub, newName);

            var claimResult = await ReplaceClaimAsync(userDto, oldSubClaim, newClaim);
            if(!claimResult.Succeeded)
            {
                await SetUserNameAsync(userDto, oldName);
                return new ChangeNameResult(false, claimResult.Errors.Select(e => e.Description));
            }

            return new ChangeNameResult(true, null);
        }

        public async Task<bool> CheckPassword(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            var user = await FindByNameAsync(userName);

            if (user is null)
                throw new ArgumentException();

            return await CheckPasswordAsync(user, password);
        }

        public async Task<CreateUserResult> CreateAsync(User user, string password)
        {
            if (user is null)
                throw new ArgumentNullException();

            if(string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            var dto = new UserDto()
            {
                UserName = user.UserName
            };

            var result = await CreateAsync(dto, password);

            if(!result.Succeeded)
                return new CreateUserResult(false,null, result.Errors.Select(e => e.Description));

            user.Id = dto.Id;

            return new CreateUserResult(true, user, null);       
        }

        public async Task<CreateUserResult> CreateDefaultClaimsAsync(User user)
        {
            if (user is null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException();

            if (user.Id == Guid.Empty)
                throw new ArgumentException();

            var userClaims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name,user.UserName)
            };

            var userDto = await FindByNameAsync(user.UserName);
            if (userDto is null)
                throw new ArgumentException("User doesn't exist.");


            var result = await AddClaimsAsync(userDto, userClaims);
            if(!result.Succeeded)
                return new CreateUserResult(false, null, result.Errors.Select(e => e.Description));

            return new CreateUserResult(true, null, null);
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException();

            var user = await FindByNameAsync(userName);
            if (user is null)
                throw new ArgumentException();

            return await GetClaimsAsync(user);
        }

        public async Task<bool> IsExistByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException();

            var user = await FindByNameAsync(userName);
            if (user is null)
                return false;
            return true;
        }

        public async Task<bool> IsExistByIdAsync(Guid userId)
        {
            if (string.IsNullOrEmpty(userId.ToString()))
                throw new ArgumentNullException();

            var user = await FindByIdAsync(userId.ToString());
            if (user is null)
                return false;
            return true;
        }

        public async Task<bool> CheckPassword(Guid userId, string password)
        {
            if (Guid.Empty == userId)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException();

            var user = await FindByIdAsync(userId.ToString());

            if (user is null)
                throw new ArgumentException();

            return await CheckPasswordAsync(user, password);
        }

        public async Task<string> GetUsernameAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user is null)
                throw new ArgumentException();
            return user.UserName;
        }
    }
}
