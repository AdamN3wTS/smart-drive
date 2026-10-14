using System;
using api.Data;
using api.IDbService;
using api.Models;

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
}
