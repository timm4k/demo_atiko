using System;

namespace atiko.Models;

public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public double AverageNotifications { get; set; }
    public DateTime NewestUserDate { get; set; }
    public DateTime OldestUserDate { get; set; }
}
