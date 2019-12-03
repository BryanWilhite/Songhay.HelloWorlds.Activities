using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.HelloWorlds.Functions.Tests
{
    public class DurableFunctionsTests
    {
        public DurableFunctionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Theory]
        [InlineData("/OrchestrationTrigger", "GetHelloWorldOutputActivity Mercury Venus Earth Mars Jupiter")]
        public async Task OrchestrationTrigger_Test(string path, string args)
        {
            // arrange
            var uri = new Uri($"{LOCAL}{path}", UriKind.Absolute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            //act
            this._testOutputHelper.WriteLine($"{nameof(uri)}: {uri.OriginalString}");
            var jO = JObject.FromObject(new { args });
            var response = await message.SendBodyAsync(jO.ToString());

            // assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        const string LOCAL = "http://localhost:7071/api";

        readonly ITestOutputHelper _testOutputHelper;
    }
}
