using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CoreLibrariesTests.API
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializeAPIResponse_ShouldReturnTrue()
        {
            //Load the json file from the test project
            var json = System.IO.File.ReadAllText("../../TestFiles/APIResponse.json");
            //deserialize the json file
            var result = JsonSerializer.Deserialize<ApiResponse<FixtureResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
