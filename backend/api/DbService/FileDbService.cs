using System;
using api.Data;
using api.Dtos;
using api.IDbServices;
using api.IServices;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.DbService;

public class FileDbService:IFileDbService
{

    private readonly AppDbContext _appDbContext;
    public FileDbService(AppDbContext context)
    {
        _appDbContext=context;
    }
    public async Task<FileEntity?> UploadFile(FileEntity File,CancellationToken ct)
    {
        await _appDbContext.Files.AddAsync(File,ct);
        await _appDbContext.SaveChangesAsync(ct);
        return File;
    }
    public async Task<FileEntity?> GetFileByIdAsync(Guid FileId,CancellationToken ct)
    {
        var file = await _appDbContext.Files.FirstOrDefaultAsync(p=>p.Id==FileId,ct);
        return file;
    }

    public async Task<FileEntity?> UpdateFileAsync(FileEntity File, CancellationToken ct)
    {
        var file = await _appDbContext.Files.FirstOrDefaultAsync(p=>p.Id==File.Id);
        file=File;
        await _appDbContext.SaveChangesAsync();
        return file;
    }
}
