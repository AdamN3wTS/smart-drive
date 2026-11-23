using System;
using api.Dtos;

namespace api.IServices;

public interface IFileService
{
    public Task<ResponseDto<List<FileDto>>> GetFilesForUser(Guid UserId,CancellationToken ct);
}
