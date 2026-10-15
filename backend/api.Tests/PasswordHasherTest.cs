
using api.Helpers;

namespace api.Tests;

public class PasswordHasherTest
{
    

    [Fact]
    public void PasswordVerify_CorrectPassword_ReturnsTrue()
    {
        string password = "Password123!";
        string hash = PasswordHasher.PasswordHash(password);
        bool verify = PasswordHasher.PasswordVerify(hash,password);
        Assert.True(verify);

    }
    [Fact]
    public void PasswordVerify_WrongPassword_ReturnsFalse()
    {
        string password = "Password123!";
        string wrongPassword = "password123!";

        string hash = PasswordHasher.PasswordHash(password);
        bool verify = PasswordHasher.PasswordVerify(hash,wrongPassword);
        Assert.False(verify);

    }
    [Fact]
    public void PasswordHash_ValidPassword_ReturnsHashDifferentFromPassword()
    {
        string password = "Password123!";
        

        string hash = PasswordHasher.PasswordHash(password);
        Assert.NotEqual(password,hash);

    }
    [Fact]
    public void PasswordHash_SamePasswordTwice_ReturnsDifferentHashes()
    {
        string password1 = "Password123!";
        

        string hash1 = PasswordHasher.PasswordHash(password1);
        string hash2 = PasswordHasher.PasswordHash(password1);
        Assert.NotEqual(hash1,hash2);
    }
}
