using iambetter.Domain.Entities.API;
using iambetter.Domain.Entities.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests.API
{
    [TestClass]
    public class DeserializationTests
    {
        //// In order to run the below test(s), 
        //// please follow the instructions from https://docs.microsoft.com/en-us/microsoft-edge/webdriver-chromium
        //// to install Microsoft Edge WebDriver.


        [TestMethod]
        public void DeserializeAPIResponse_ShouldReturnTrue()
        {
            try
            {
                //Read the JSON file from project folder
                string json = System.IO.File.ReadAllText($"../../../API/TestFiles/fixture_response.json");
                var result = JsonSerializer.Deserialize<ApiResponse<FixtureResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsTrue(true);
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }

        [TestMethod]
        public void DeserializeAPIStatisticsResponse_ShouldReturnTrue()
        {
            try
            {
                //Read the JSON file from project folder
                string json = System.IO.File.ReadAllText($"../../../API/TestFiles/stats_response.json");
                var result = JsonSerializer.Deserialize<APIStatsResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Assert.IsTrue(true);
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
    }
}
