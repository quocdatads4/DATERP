using System;

namespace DATERP.Identity;

public class UserStatisticsDto
{
    public long TotalUsers { get; set; }
    public long ActiveUsers { get; set; }
    public long InactiveUsers { get; set; }
    public long PendingUsers { get; set; }
}
