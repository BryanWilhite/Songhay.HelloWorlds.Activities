using Songhay.Extensions;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.HelloWorlds.Functions.Tests
{
    public class LocalTests
    {
        public LocalTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory]
        [InlineData("/HttpTrigger?name=Bill")]
        public async Task HttpTrigger_Test(string path)
        {
            // arrange
            var uri = new Uri($"{LOCAL}{path}", UriKind.Absolute);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            //act
            this._testOutputHelper.WriteLine($"{nameof(uri)}: {uri.OriginalString}");
            var response = await message.SendAsync();

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        const string LOCAL = "http://localhost:7071/api";

        readonly ITestOutputHelper _testOutputHelper;
    }
}
