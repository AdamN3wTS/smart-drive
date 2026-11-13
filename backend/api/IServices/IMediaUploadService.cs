using System;
using api.Dtos;
using api.Models;

namespace api.IServices;

public interface IMediaUploadService
{
    public Task<ResponseDto<UploadFileResponseDto?>> GenerateUrl(UploadFileDto dto,Guid UserId,CancellationToken ct);
    public Task<ResponseDto<FileEntity?>> UploadDone(Guid FileId, CancellationToken ct);


}
