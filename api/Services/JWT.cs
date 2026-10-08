using System;

namespace api.Services;

public static class JWT
{
    public static string GenerateAccessToken()
    {
        throw new NotImplementedException();
    }
    public static (string RefreshToken,DateTime ExpAt) GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }
    public static string HashRefreshToken()
    {
        throw new NotImplementedException();
    }
}
