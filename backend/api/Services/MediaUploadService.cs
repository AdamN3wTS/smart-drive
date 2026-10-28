using System;
using Amazon.Util.Internal;
using api.Dtos;
using api.Helpers;
using api.IDbServices;
using api.IServices;
using api.Models;
using Cloudflare.NET.R2;
using Cloudflare.NET.R2.Models;
namespace api.Services;

public class MediaUploadService : IMediaUploadService
{
    private readonly IFileDbService _fileDbService;
    private readonly IR2Client _r2Client;
    private const string Bucket ="smart-drive";
    private string[] AllowedMedia = new[] { "image/jpeg", "image/png", "image/webp", "video/mp4" };
    public MediaUploadService(IR2Client r2Client,IFileDbService fileDbService)
    {
        _fileDbService=fileDbService;
        _r2Client=r2Client;
    }
    public async Task<ResponseDto<UploadFileResponseDto?>> GenerateUrl(UploadFileDto dto,Guid UserId,CancellationToken ct)
    {
        var extenstion = dto.ContentType.Split('/')[1];
        var fileId = Guid.NewGuid();
        var key = $"users/{UserId}/{fileId}.{extenstion}";
        var file =  new FileEntity()
        {
            Id=fileId,
            ContentLength=dto.ContentLength,
            ContentType=dto.ContentType,
            UserId=UserId,
            Key=key,
            FileName=dto.Name
            
        };
        var url = _r2Client.CreatePresignedPutUrl(Bucket,new PresignedPutRequest(
            Key:key,
            ExpiresAfter:TimeSpan.FromMinutes(10),
            ContentType:dto.ContentType,
            ContentLength:dto.ContentLength
        ));
        var fileService = await _fileDbService.UploadFile(file,ct);
        return ResponseHelper.GenerateResponse<UploadFileResponseDto?>(new UploadFileResponseDto(){Url=url,FileId=fileId},true,"success");
    }

    public async Task<ResponseDto<bool?>> UploadDone(Guid FileId, CancellationToken ct)
    {
        var file = await _fileDbService.GetFileByIdAsync(FileId,ct);
        if (file == null)
        {
            throw new Exception();
        }
        var result = await _r2Client.ListObjectsAsync(Bucket, prefix: file.Key);
        var obj = result.Data.FirstOrDefault(o => o.Key == file.Key);

        if (obj is null)
            throw new Exception();

        if (obj.Size != file.ContentLength)
        {
            await _r2Client.DeleteObjectAsync(Bucket, file.Key); 
            throw new Exception();
        }
        file.Status=Models.Enums.FileStatus.added;
        return ResponseHelper.GenerateResponse<bool?>(true,true,"success");
    }
}
