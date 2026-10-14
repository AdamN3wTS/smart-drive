using System;
using api.Dtos;
using api.Helpers;
using api.IDbService;
using api.IDbServices;
using api.Models;

namespace api.DbServices;

public class AuthService : IAuthService
{
    private readonly IUserDbService _userDbService;
    private readonly IRefreshTokenDbService _refreshTokenDbService;

    public AuthService(IUserDbService userDbService,IRefreshTokenDbService refreshTokenDbService)
    {
        _refreshTokenDbService=refreshTokenDbService;
        _userDbService=userDbService;
    }

    public async Task<ResponseDto<object>> Login()
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDto<RegisterResponseDto>> Register(RegisterUserDto dto,CancellationToken ct)
    {
        try
        {
            var passwordHash = PasswordHasher.PasswordHash(dto.Password);
            var User = new User
            {
                Id=Guid.NewGuid(),
                Name=dto.Username,
                Email=dto.Email,
                PasswordHash=passwordHash
            };
            var userDbServiceResult = await _userDbService.AddUserAsync(User,ct);
            var (refreshTokenPlain,refreshTokenHash,expAt) = JWT.GenerateRefreshToken();
            var refreshToken= new RefreshToken()
            {
                ExpAt=expAt,
                Hash=refreshTokenHash,
                UserId=User.Id
            };
            var refreshTokenDbServiceResult = await _refreshTokenDbService.AddRefreshTokenAsync(refreshToken,ct);
            var accessToken = JWT.GenerateAccessToken(User);
            var responseDto = new RegisterResponseDto()
            {
              refreshTokenExpAt=expAt,
              refreshTokenPlain=refreshTokenPlain,
              accessToken=accessToken  
            };
            return ResponseHelper.GenerateResponse<RegisterResponseDto>(responseDto,true,"RegisterSuccess");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
