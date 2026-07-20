using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IApplicationSettingRepository : IRepository<ApplicationSetting>
    {
        /// <summary>The single seeded settings row (ApplicationSettingId = 1).</summary>
        Task<ApplicationSetting> GetSingletonAsync();
    }
}
