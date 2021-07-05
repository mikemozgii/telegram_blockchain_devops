namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class EnvironmentModel
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public string Domain { get; set; }

        public string Id { get; set; }

        public string Modules { get; set; }

        public string ConnectionStrings { get; set; }

        public bool InitialEnvironment { get; set; }

    }

}
