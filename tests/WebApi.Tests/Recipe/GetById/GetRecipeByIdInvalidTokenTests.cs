using Shouldly;
using System.Net;

namespace WebApi.Tests.Recipe.GetById
{
    public class GetRecipeByIdInvalidTokenTests : BaseIntegrationTest
    {
        private const string REQUEST_URI = "recipe";
        private readonly string _tokenUserNotExistsDatabase;

        public GetRecipeByIdInvalidTokenTests(MyRecipeBookApplicationFactory factory) : base(factory)
        {
            _tokenUserNotExistsDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATABASE;
        }

        [Fact]
        public async Task Validate_ShouldbeAnErrorResponse_WhenAccessTokenIsInvalid()
        {
            var response = await Get($"{REQUEST_URI}/{Guid.NewGuid()}", token: "invalid_token");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Validate_ShouldbeAnErrorResponse_WhenAccessTokenIsMissing()
        {
            var response = await Get($"{REQUEST_URI}/{Guid.NewGuid()}", token: string.Empty);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Validate_ShouldbeAnErrorResponse_WhenUserFromAcessTokenDoesnotExist()
        {
            var response = await Get($"{REQUEST_URI}/{Guid.NewGuid()}", token: _tokenUserNotExistsDatabase);

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}
