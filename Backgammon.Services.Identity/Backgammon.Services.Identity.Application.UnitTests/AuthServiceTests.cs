using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Backgammon.Services.Identity.Domain.Interfaces;
using Backgammon.Services.Identity.Application.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Backgammon.Services.Identity.Infra.Data.Dtos;
using Backgammon.Services.Identity.Application.Results;

namespace Backgammon.Services.Identity.Application.UnitTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IUserManager> userManager;
        private Mock<IJsonWebTokenProvider> accessTokenProvider;
        private Mock<IRefreshTokenProvider> refreshTokenProvider;
        private Mock<IRefreshToken> refreshToken;
        private Mock<IJsonWebToken> jsonWebToken;
        [TestInitialize]
        public void Init()
        {
            userManager = new();
            accessTokenProvider = new();
            refreshTokenProvider = new();
            refreshToken = new();
            jsonWebToken = new();
        }

        [TestMethod]
        public async Task LoginAsync_WithValidInput_ReturnAuthResult()
        {
            // Arrange
            var username = "Test123";
            var password = "Test123!";
            var userId = Guid.NewGuid();
            var jwtId = Guid.NewGuid();
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                new Claim("uid",userId.ToString())
            };

            var expect = new AuthResult()
            {
                AccessToken = "jsonWebToken",
                RefreshToken = "refreshToken",
                UserId = userId
            };
            userManager.Setup(um => um.IsExistByUserNameAsync(It.IsAny<string>()))
                .ReturnsAsync(true);
            userManager.Setup(um => um.CheckPassword(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            userManager.Setup(um => um.GetUserClaimsAsync(It.IsAny<string>()))
                .ReturnsAsync(claims);

            jsonWebToken.Setup(t => t.UserId).Returns(userId);
            jsonWebToken.Setup(t => t.Id).Returns(jwtId);

            accessTokenProvider.Setup(p => p.CreateToken(It.IsAny<IEnumerable<Claim>>()))
                .Returns(jsonWebToken.Object);
            accessTokenProvider.Setup(p => p.WriteToken(It.IsAny<IJsonWebToken>()))
                .Returns("jsonWebToken");

            refreshTokenProvider.Setup(p => p.CreateToken(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(refreshToken.Object);
            refreshTokenProvider.Setup(p => p.WriteToken(It.IsAny<IRefreshToken>()))
                .Returns("refreshToken");

            var service = new AuthService(userManager.Object,accessTokenProvider.Object,refreshTokenProvider.Object);
            // Act

            var result = await service.LoginAsync(username, password);

            // Assert
            userManager.Verify(um => um.IsExistByUserNameAsync(username), Times.Once());
            userManager.Verify(um => um.CheckPassword(username, password), Times.Once());
            userManager.Verify(um => um.GetUserClaimsAsync(username), Times.Once());

            accessTokenProvider.Verify(p => p.CreateToken(claims), Times.Once());
            accessTokenProvider.Verify(p => p.WriteToken(jsonWebToken.Object), Times.Once());

            refreshTokenProvider.Verify(p => p.CreateToken(userId, jwtId));
            refreshTokenProvider.Verify(p => p.WriteToken(refreshToken.Object));

            result.Should().NotBeNull();

            result.IsSuccess.Should().BeTrue();

            result.Should()
                .BeOfType<AuthResult>()
                .Which
                .Should()
                .BeEquivalentTo(expect);
        }
    }
}
