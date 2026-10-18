using System;
using api.IServices;
using Cloudflare.NET.R2;
namespace api.Services;

public class MediaUploadService : IMediaUploadService
{
    private readonly R2Client _r2Client;
    private const string Bucket ="smart-drive";
    private string[] AllowedMedia = new[] { "image/jpeg", "image/png", "image/webp", "video/mp4" };
    public MediaUploadService(R2Client r2Client)
    {
        _r2Client=r2Client;
    }
}
