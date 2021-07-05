using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp
{
    public interface IEnvironmentService
    {

        Task CreateEnvironment(string domain, string login = "", string password = "");
        
    }
}
