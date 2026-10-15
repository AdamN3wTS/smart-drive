using System;
using api.Data;
using api.IDbService;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.DbService;

public class UserDbService : IUserDbService
{
    private readonly AppDbContext _appDbContext;
    public UserDbService(AppDbContext appDbContext)
    {
        _appDbContext=appDbContext;
    }
    public async Task<User?> GetUserByIdAsync(Guid Id,CancellationToken ct)
    {
        var user = await _appDbContext.Users.FirstOrDefaultAsync(p=>p.Id==Id,ct);
        return user;
    }
    public async Task<User> AddUserAsync(User User,CancellationToken ct)
    {
        await _appDbContext.Users.AddAsync(User,ct);
        await _appDbContext.SaveChangesAsync(ct);
        return User;
        
    }
    public async Task<User?> GetUserByNameEmailASync(string NameEmail,CancellationToken ct)
    {
        var user = await _appDbContext.Users.FirstOrDefaultAsync(p=>p.Email==NameEmail || p.Name==NameEmail,ct);
        return user;
    }
}
