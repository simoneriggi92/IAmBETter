using System.Net.Http.Headers;

namespace iambetter.Application.Services.API
{
    public class FastAPIDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FastAPIDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["FastAPI:BaseUrl"]);
        }

        public async Task<string> PostCsvAsync(string csvFilePath, string path)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                //check if file exists
                if (File.Exists(csvFilePath))
                {
                    using var fileStream = File.OpenRead(csvFilePath);
                    using var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"",
                        FileName = $"\"{Path.GetFileName(csvFilePath)}\""
                    };
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    form.Add(fileContent, "file");
                    var response = await _httpClient.PostAsync(path, form);
                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Python API failed: {response.StatusCode} - {error}");
                    }
                    return await response.Content.ReadAsStringAsync();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
