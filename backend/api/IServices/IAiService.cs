using api.Dtos;

namespace api.IServices
{
	public interface IAiService
	{
		public Task AddFile(string url,string id,string userId);
		public Task<List<AiSearchResponseDto>> SearchFiles(string query,string userId);


	}
}
