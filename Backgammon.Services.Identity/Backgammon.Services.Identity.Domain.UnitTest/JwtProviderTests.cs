using Microsoft.VisualStudio.TestTools.UnitTesting;
using TalkBack.Services.Identity.Domain.Interfaces;
using TalkBack.Services.Identity.Domain.TokenProviders;

namespace TalkBack.Services.Identity.Domain.UnitTest
{
    [TestClass]
    public class JwtProviderTests
    {
        [TestMethod]
        public void CreateToken_WithValidClaims_ReturnSecurityToken()
        {
            // Arrange
            IAccessTokenProvider provider = new JwtProvider()
            // Act

            // Assert
        }
    }
}
