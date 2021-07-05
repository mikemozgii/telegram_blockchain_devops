using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Models.Modules
{
    public class SaveModuleEnvironmentModel
    {
        public string Id {
            get;
            set;
        }
        
        public IEnumerable<string> Environments
        {
            get;
            set;
        }

    }
}
