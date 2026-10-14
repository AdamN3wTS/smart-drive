using System;
using api.Dtos;

namespace api.IDbServices;

public interface IAuthService
{
    public Task<ResponseDto<object>> Login();
    public Task<ResponseDto<RegisterResponseDto>> Register(RegisterUserDto dto,CancellationToken ct);

}
