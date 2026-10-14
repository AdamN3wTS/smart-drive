using System;
using BC = BCrypt.Net.BCrypt;
namespace api.Helpers;

public static class PasswordHasher
{
    public static string PasswordHash(string password)
    {
        return BC.HashPassword(password,salt: BC.GenerateSalt(),enhancedEntropy:true,hashType:BCrypt.Net.HashType.SHA384);
    }
    public static bool PasswordVerify(string hash,string password)
    {
        return BC.Verify(password,hash,hashType:BCrypt.Net.HashType.SHA384);
    }
}
