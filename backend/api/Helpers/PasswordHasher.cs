using System;
using BC = BCrypt.Net.BCrypt;
namespace api.Helpers;

public static class PasswordHasher
{
    public static string PasswordHash(string password)
    {
        return BC.HashPassword(password);
    }
    public static bool PasswordVerify(string hash,string password)
    {
        return BC.Verify(password,hash);
    }
}
