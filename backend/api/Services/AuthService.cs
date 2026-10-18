using System;
using System.Security.Cryptography;
using System.Text;
using api.Dtos;
using api.Helpers;
using api.IDbService;
using api.IDbServices;
using api.Models;
using Npgsql.Internal;

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

    public async Task<ResponseDto<LoginServiceResponseDto?>> Login(LoginRequestDto dto,CancellationToken ct)
    {
        try
        {
            var user = await _userDbService.GetUserByNameEmailASync(dto.NameEmail,ct);
            if (user == null)
            {
                return ResponseHelper.GenerateResponse<LoginServiceResponseDto?>(null,false,"User not found");
            }

            bool verify = PasswordHasher.PasswordVerify(user.PasswordHash!,dto.Password);
            if (!verify)
            {
                return ResponseHelper.GenerateResponse<LoginServiceResponseDto?>(null,false,"Password do not match");
            }
            var (refreshTokenPlain,refreshTokenHash,expAt) = JWT.GenerateRefreshToken();
            var oldToken = await _refreshTokenDbService.GetLatestTokenByUserIdAsync(user.Id,ct);
            var refreshToken= new RefreshToken()
            {
                ExpAt=expAt,
                Hash=refreshTokenHash,
                UserId=user.Id,
                ParentId = oldToken != null ? oldToken.Id : null
                
            };
            var refreshTokenDbServiceResult = await _refreshTokenDbService.AddRefreshTokenAsync(refreshToken,ct);
            var accessToken = JWT.GenerateAccessToken(user);
            var responseDto = new LoginServiceResponseDto()
            {
              refreshTokenExpAt=expAt,
              refreshTokenPlain=refreshTokenPlain,
              accessToken=accessToken  
            };
            return ResponseHelper.GenerateResponse<LoginServiceResponseDto?>(responseDto,true,"RegisterSuccess");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResponseDto<RegisterServiceResponseDto?>> Register(RegisterUserDto dto,CancellationToken ct)
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
            if (userDbServiceResult == null)
            {
                return ResponseHelper.GenerateResponse<RegisterServiceResponseDto?>(null,false,"RegisterSuccess");
            }
            var (refreshTokenPlain,refreshTokenHash,expAt) = JWT.GenerateRefreshToken();
            var refreshToken= new RefreshToken()
            {
                ExpAt=expAt,
                Hash=refreshTokenHash,
                UserId=User.Id
            };
            var refreshTokenDbServiceResult = await _refreshTokenDbService.AddRefreshTokenAsync(refreshToken,ct);
            var accessToken = JWT.GenerateAccessToken(User);
            var responseDto = new RegisterServiceResponseDto()
            {
              refreshTokenExpAt=expAt,
              refreshTokenPlain=refreshTokenPlain,
              accessToken=accessToken  
            };
            return ResponseHelper.GenerateResponse<RegisterServiceResponseDto?>(responseDto,true,"RegisterSuccess");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ResponseDto<object?>> Logout(string Token,CancellationToken ct)
    {
        try
        {
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(Token)));
            var tokenResult = await _refreshTokenDbService.RevokeTokenByTokenHashAsync(hash,"Logout",ct);
            if (tokenResult!=null)
            {
                return ResponseHelper.GenerateResponse<object?>(null,true,"Logout Success");
            }
            return ResponseHelper.GenerateResponse<object?>(null,false,"Logout Failed");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ResponseDto<UserDto?>> Me(string Id ,CancellationToken ct)
    {
        try
        {
            Guid guidId = Guid.Parse(Id);
            var user = await _userDbService.GetUserByIdAsync(guidId,ct);
            if (user == null)
            {
                return ResponseHelper.GenerateResponse<UserDto?>(null,false,"User not found");
            }
            var userDto = new UserDto()
            {
                Id=user.Id,
                Name=user.Name,
                Email=user.Email
            };
            return ResponseHelper.GenerateResponse<UserDto?>(userDto,true,"Succcess");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ResponseDto<LoginServiceResponseDto?>> Refresh(string tokenPlain,CancellationToken ct)
    {
        try
        {
            var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlain)));
            
            var oldToken = await _refreshTokenDbService.RevokeTokenByTokenHashAsync(hash!,"Refresh",ct);
            if (oldToken == null)
            {
                return ResponseHelper.GenerateResponse<LoginServiceResponseDto?>(null,false,"Token do not exist");
            }
            var (refreshTokenPlain,refreshTokenHash,expAt) = JWT.GenerateRefreshToken();

            var refreshToken= new RefreshToken()
            {
                ExpAt=expAt,
                Hash=refreshTokenHash,
                UserId=oldToken.UserId,
                ParentId = oldToken.Id
                
            };
            
            var user = await _userDbService.GetUserByIdAsync(oldToken.UserId,ct);
            var refreshTokenDbServiceResult = await _refreshTokenDbService.AddRefreshTokenAsync(refreshToken,ct);
            var accessToken = JWT.GenerateAccessToken(user);
            var responseDto = new LoginServiceResponseDto()
            {
              refreshTokenExpAt=expAt,
              refreshTokenPlain=refreshTokenPlain,
              accessToken=accessToken  
            };
            return ResponseHelper.GenerateResponse<LoginServiceResponseDto?>(responseDto,true,"Refresh success");
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
