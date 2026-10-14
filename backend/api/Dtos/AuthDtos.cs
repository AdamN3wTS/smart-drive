using System;

namespace api.Dtos;

public class RegisterUserDto
{
    public required string Username {get;set;}
    public required string Password {get;set;}
    public required string VerifyPassword {get;set;}
    public required string Email {get;set;}
}
public class RegisterResponseDto
{
    public string accessToken {get;set;}
    public string refreshTokenPlain {get;set;}
    public DateTime refreshTokenExpAt {get;set;}
    
}
