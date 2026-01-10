using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DATERP.Identity;

public interface IUserStatisticsAppService : IApplicationService
{
    Task<UserStatisticsDto> GetStatisticsAsync();
}
