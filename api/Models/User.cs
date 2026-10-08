using System;
using api.Models.Enums;

namespace api.Models;

public class User : BasicEntity
{
    public required string Name {get;set;}
    public required string Email {get;set;} 
    public RoleEnum Role {get;set;} = RoleEnum.User;
}
