using TONBRAINS.TONOPS.WebApp.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public interface IConfigurationService
    {

        Task<IEnumerable<Configuration>> GetModuleConfigurations(string moduleId);

    }
}
