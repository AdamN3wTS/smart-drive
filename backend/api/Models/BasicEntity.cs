using System;

namespace api.Models;

public class BasicEntity
{
    public Guid Id {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    public DateTime? DeletedAt {get;set;}
    
}
