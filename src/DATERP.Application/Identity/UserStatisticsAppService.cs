using System.Threading.Tasks;
using System.Linq;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Domain.Repositories;

using DATERP;

namespace DATERP.Identity;

public class UserStatisticsAppService : DATERPAppService, IUserStatisticsAppService
{
    protected IIdentityUserRepository UserRepository { get; }

    public UserStatisticsAppService(IIdentityUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    public virtual async Task<UserStatisticsDto> GetStatisticsAsync()
    {
        var totalUsers = await UserRepository.GetCountAsync();
        var activeUsers = 0; // await AsyncExecuter.CountAsync((await UserRepository.GetQueryableAsync()).Where(u => u.IsActive));
        var inactiveUsers = 0; // await AsyncExecuter.CountAsync((await UserRepository.GetQueryableAsync()).Where(u => !u.IsActive));
        var pendingUsers = 0; // await AsyncExecuter.CountAsync((await UserRepository.GetQueryableAsync()).Where(u => !u.EmailConfirmed));

        return new UserStatisticsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = inactiveUsers,
            PendingUsers = pendingUsers
        };
    }
}
