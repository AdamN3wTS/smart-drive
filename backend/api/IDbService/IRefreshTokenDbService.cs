using System;
using api.Models;

namespace api.IDbService;

public interface IRefreshTokenDbService
{
    

    public  Task<bool> AddRefreshTokenAsync(RefreshToken Token,CancellationToken ct);

}
