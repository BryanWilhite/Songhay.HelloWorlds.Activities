using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.HelloWorlds.Functions.Tests
{
    public class StatelessFunctionsTests
    {
        public StatelessFunctionsTests(ITestOutputHelper helper)
        {
            _testOutputHelper = helper;
        }

        [Theory]
        [InlineData("/ActivityTrigger", "GetHelloWorldOutputActivity Jupiter")]
        public async Task ActivityTrigger_Test(string path, string args)
        {
            // arrange
            var uri = new Uri($"{Local}{path}", UriKind.Absolute);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            //act
            _testOutputHelper.WriteLine($"{nameof(uri)}: {uri.OriginalString}");
            var jO = JObject.FromObject(new {args});
            var response = await message.SendBodyAsync(jO.ToString());

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/HttpTrigger?name=Bill")]
        public async Task HttpTrigger_Test(string path)
        {
            // arrange
            var uri = new Uri($"{Local}{path}", UriKind.Absolute);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            //act
            _testOutputHelper.WriteLine($"{nameof(uri)}: {uri.OriginalString}");
            var response = await message.SendAsync();

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        const string Local = "http://localhost:7071/api";

        readonly ITestOutputHelper _testOutputHelper;
    }
}
