using api.Dtos;
using api.IServices;

namespace api.Services
{
	public class AiService : IAiService
	{
		public readonly string _url;
		public readonly HttpClient _httpClient;
		public readonly ILogger<AiService> _logger;
		public AiService(HttpClient httpClient,ILogger<AiService> logger)
		{
			_httpClient=httpClient;
			_url = Environment.GetEnvironmentVariable("AI_URL")!;
			_logger=logger;
		}
		public async Task AddFile(string url,string id,string userId)
		{
			try
			{
				var queryParams = new Dictionary<string, string>
				{
					["file_url"] = url,
					["file_id"] = id,
					["col"] = userId
				};
				var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
				var response = await _httpClient.PostAsync($"{_url}/add?{queryString}",null);
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning($"cos poszlo nie tak :/// stautsCode: {response.StatusCode}");
				}
				_logger.LogWarning($"SUPER UDALO SIE DODALISMY TO :D statusCode: {response.StatusCode}");
			}
			catch(Exception ex)
			{
				_logger.LogError($"{ex.Message} \n {ex.Data}");
				throw new Exception(ex.Message);
			}
		}
		public async Task<List<AiSearchResponseDto>> SearchFiles(string query,string userId)
		{
			try
			{
				var queryParams = new Dictionary<string, string>
				{
					["query"] = query,
					["col"] = userId
				};
				var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
				var response = await _httpClient.GetAsync($"{_url}/search?{queryString}");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning($"cos poszlo nie tak :/// stautsCode: {response.StatusCode}");
				}
				_logger.LogWarning($"SUPER UDALO SIE :D statusCode: {response.StatusCode}");
				var data = await response.Content.ReadFromJsonAsync<FastApiResponseWrapper<List<AiSearchResponseDto>>>();
				return data.Data;
			}
			catch(Exception ex)
			{
				_logger.LogError($"{ex.Message} \n {ex.Data}");
				throw new Exception(ex.Message);
			}
		}
	}
}
