using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Models.Ecosystems
{
    public class EcosystemModuleModel
    {

        public string EcosystemId {
            get;
            set;
        }

        public IEnumerable<string> ModuleTypesIds {
            get;
            set;
        }

    }
}
