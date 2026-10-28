using System;
using api.Models;

namespace api.IDbServices;

public interface IFileDbService
{
    public Task<FileEntity?> UploadFile(FileEntity File,CancellationToken ct);
    public Task<FileEntity?> GetFileByIdAsync(Guid FileId,CancellationToken ct);
    public Task<FileEntity?> UpdateFileAsync(FileEntity File,CancellationToken ct);


}
