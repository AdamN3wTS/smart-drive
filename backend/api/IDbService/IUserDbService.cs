using System;
using api.Models;

namespace api.IDbService;

public interface IUserDbService
{
    // TODO User - > UserDto
    public Task<User?> GetUserByIdAsync(Guid Id,CancellationToken ct);
    public  Task<User> AddUserAsync(User User,CancellationToken ct);

    
}
