using System;
using api.Data;
using api.IDbService;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.DbService;

public class RefreshTokenDbService : IRefreshTokenDbService
{
    private readonly AppDbContext _appDbContext;
    public RefreshTokenDbService(AppDbContext appDbContext)
    {
        _appDbContext=appDbContext;
    }
    public async Task<bool> AddRefreshTokenAsync(RefreshToken Token,CancellationToken ct)
    {
        await _appDbContext.RefreshTokens.AddAsync(Token,ct);
        await _appDbContext.SaveChangesAsync(ct);
        return true;

    }
    public async Task<RefreshToken?> GetLatestTokenByUserIdAsync(Guid UserId,CancellationToken ct)
    {
        var token = await _appDbContext.RefreshTokens.Where(p=>p.UserId==UserId && p.RevokedAt==null && p.ExpAt>DateTime.UtcNow).OrderByDescending(p=>p.CreatedAt).FirstOrDefaultAsync(ct);
        if (token == null)
        {
            return null;
        }
        token.RevokedAt=DateTime.UtcNow;
        token.RevokedReason="pre login";
        await _appDbContext.SaveChangesAsync(ct);
        return token;

    }
    public async Task<RefreshToken?> RevokeTokenByTokenHashAsync(string TokenHash,string Reason,CancellationToken ct)
    {
        var token = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(p=>p.Hash==TokenHash && p.RevokedAt==null && p.ExpAt>DateTime.UtcNow,ct);
        if (token == null)
        {
            return null;
        }
        token.RevokedAt=DateTime.UtcNow;
        token.RevokedReason=Reason;
        await _appDbContext.SaveChangesAsync(ct);
        return token;
    }
}
