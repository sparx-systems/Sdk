using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sparx.Sdk;
using Xunit;

namespace Sdk.UnitTests
{
    public class GetAuthenticatedClientTests
    {
        private const string Domain = "https://test.sparx.com.ua";

        [Fact]
        public async Task ShouldHaveInvalidClient()
        {
            var api = new Api(Domain, "test_client", "secret");

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await api.MakeRequestAsync("/v1/articles", null, HttpMethod.Get);
            });

            Assert.Equal("invalid_client", exception.Message);
        }

        [Fact]
        public async Task ShouldHaveUnauthorizedException()
        {
            var api = new Api(Domain, "test", "secret");

            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await api.MakeRequestAsync("/v1/orders", null, HttpMethod.Get);
            });

            Assert.Equal("Response status code does not indicate success: 403 (Forbidden).", exception.Message);
        }

        [Fact]
        public async Task ShouldHaveSuccessResult()
        {
            var api = new Api(Domain, "test", "secret");
            var result = await api.MakeRequestAsync("/v1/articles", null, HttpMethod.Get);

            Assert.NotEmpty(result);
        }
    }
}