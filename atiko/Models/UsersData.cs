using System.Collections.Generic;

namespace atiko.Models;

public class UsersData
{
    public List<User> Users { get; set; } = [];
    public User? CurrentUser { get; set; }
}