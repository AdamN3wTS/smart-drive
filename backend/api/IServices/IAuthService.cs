using System;
using api.Dtos;

namespace api.IDbServices;

public interface IAuthService
{
    public Task<ResponseDto<LoginServiceResponseDto?>> Login(LoginRequestDto dto,CancellationToken ct);
    public Task<ResponseDto<RegisterServiceResponseDto?>> Register(RegisterUserDto dto,CancellationToken ct);
    public Task<ResponseDto<object?>> Logout(string Token,CancellationToken ct);
    public Task<ResponseDto<UserDto?>> Me(string Id ,CancellationToken ct);
    public  Task<ResponseDto<LoginServiceResponseDto?>> Refresh(string tokenPlain,CancellationToken ct);


    

}
