using System;
using api.Models;

namespace api.IDbService;

public interface IRefreshTokenDbService
{
    

    public  Task<bool> AddRefreshTokenAsync(RefreshToken Token,CancellationToken ct);
    public  Task<RefreshToken?> GetLatestTokenByUserIdAsync(Guid UserId,CancellationToken ct);
    public  Task<RefreshToken?> RevokeTokenByTokenHashAsync(string TokenHash,string Reason,CancellationToken ct);


}
