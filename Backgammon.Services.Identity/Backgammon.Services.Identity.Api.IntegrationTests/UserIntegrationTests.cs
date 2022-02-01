using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Backgammon.Services.Identity.Contracts.Requests;
using Backgammon.Services.Identity.Contracts.Responses;

namespace Backgammon.Services.Identity.Api.IntegrationTests
{
    [TestClass]
    public class UserIntegrationTests : IntegrationTest
    {
        [TestMethod]
        public async Task Register_WithValidRequest_ReturnSuccess()
        {
            // Arrange
            var url = "/api/user/register";
            var request = new RegisterRequest()
            {
                Username = "Test123",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(url, request);
            var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
            // Assert

            response.StatusCode
                .Should()
                .Be(System.Net.HttpStatusCode.OK);

            content
                .Should()
                .NotBeNull();

            content.AccessToken
                .Should()
                .NotBeEmpty();

            content.RefreshToken
                .Should()
                .NotBeEmpty();
        }

        [TestMethod]
        [DataRow("Test1232", 1)]
        [DataRow("Test123", 2)]
        [DataRow("Test!23", 1)]
        [DataRow("test123", 3)]
        [DataRow("שגשd32", 2)]
        public async Task Register_WithBadPassword_ReturnFaildResponse(string pass,int numOfErrors)
        {
            // Arrange
            var url = "/api/user/register";
            var request = new RegisterRequest()
            {
                Username = "Test123",
                Password = pass,
                ConfirmPassword = pass
            };

            // Act
            var response = await TestClient.PostAsJsonAsync(url, request);
            var content = await response.Content.ReadFromJsonAsync<FailedResponse>();
            // Assert

            response.StatusCode
                .Should()
                .Be(System.Net.HttpStatusCode.BadRequest);

            content
                .Should()
                .NotBeNull();

            content.IsSuccess
                .Should()
                .BeFalse();

            content.Errors
                .Should()
                .HaveCount(numOfErrors);
        }
    }
}
