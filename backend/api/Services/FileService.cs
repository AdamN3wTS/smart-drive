using System;
using Amazon.Util.Internal;
using api.Dtos;
using api.Helpers;
using api.IDbServices;
using api.IServices;

namespace api.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IFileDbService _fileDbService;
    public FileService(ILogger<FileService> logger,IFileDbService fileDbService)
    {
        _fileDbService=fileDbService;
        _logger=logger;
    }
    public async Task<ResponseDto<List<FileDto>>> GetFilesForUser(Guid UserId,CancellationToken ct)
    {
        var service = await _fileDbService.GetFilesForUserAsync(UserId,ct);
        var listDto = service.Select(p=>new FileDto{Id=p.Id,ContentType=p.ContentType,Name=p.FileName,Url=p.FileUrl}).ToList();
        var responseDto = ResponseHelper.GenerateResponse<List<FileDto>>(listDto,true,"success");
        return responseDto;
    }

}
