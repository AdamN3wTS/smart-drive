using System;
using api.Dtos;

namespace api.IServices;

public interface IMediaUploadService
{
    public Task<ResponseDto<UploadFileResponseDto?>> GenerateUrl(UploadFileDto dto,Guid UserId,CancellationToken ct);
    public Task<ResponseDto<bool?>> UploadDone(Guid FileId, CancellationToken ct);


}
