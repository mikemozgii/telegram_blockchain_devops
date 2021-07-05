using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using TONBRAINS.TONOPS.WebApp;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public class EnvironmentService : IEnvironmentService
    {

        private static int CountMigrations = 0;

        public const string PasswordSalt = "lotussaltforTONBRAINS.TONOPS.WebAppaccount";

        private async Task ExecuteMigration(int number, string connectionString)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string environmentSql = "";
            using (Stream stream = assembly.GetManifestResourceStream($"EnvironmentMigrations/{number}/up.script"))
            using (StreamReader reader = new StreamReader(stream))
            {
                environmentSql = reader.ReadToEnd();
            }

            await ExecuteSql(environmentSql, connectionString: connectionString);
        }

        public async Task CreateEnvironment(string domain, string login = "", string password = "")
        {
            var connectionString = "Host=172.17.1.81;Database=postgres;Port=5432;Username=postgres;Password=postgres";
            var domainName = $"\"{domain}\"";
            await ExecuteSql($"CREATE DATABASE {domainName} WITH OWNER = \"postgres\";", connectionString: connectionString);

            var baseConnectionString = $"Host=172.17.1.81;Database={domain};Port=5432;Username=postgres;Password=postgres";

            await ExecuteSql($"CREATE EXTENSION IF NOT EXISTS hstore;", connectionString: baseConnectionString);

            var assembly = Assembly.GetExecutingAssembly();
            string environmentSql = "";
            using (Stream stream = assembly.GetManifestResourceStream("TONBRAINS.TONOPS.WebApp.environment.sql"))
            using (StreamReader reader = new StreamReader(stream))
            {
                environmentSql = reader.ReadToEnd();
            }

            await ExecuteSql(environmentSql, connectionString: baseConnectionString);

            //administrator / temp3232
            var administratorLogin = string.IsNullOrEmpty(login) ? "administrator" : login;
            var administratorPassword = string.IsNullOrEmpty(password) ? "$2b$10$kbYc3nBX1c.3GpBhJjN0wOIX2T31t06OA1GHd2cbhrVcb9tLllZ6G" : new PasswordHashService().Hash(password, PasswordSalt);
            await ExecuteSql($"INSERT INTO \"public\".\"accounts\"(\"id\", \"name\", \"password\", \"super_user\") VALUES('-MCBSEfn--5-6ghqrBBm', '{administratorLogin}', '{administratorPassword}', true)", connectionString: baseConnectionString);

            await ExecuteSql($"INSERT INTO global_settings(environment_name, environment_type, billing_type, modules, owners) VALUES ('{domain}', 'multiorganizationandlocations', 'billeachorgindividually', '[]', '[]')", connectionString: baseConnectionString);

            for (var i = 0; i < CountMigrations; i++) await ExecuteMigration(i + 1, baseConnectionString);
        }

    }

}
