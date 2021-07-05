using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.WebApp.Models.Common
{
    public class DeploySmartContractsBundle
    {
        public IEnumerable<string> NetworkIds { get; set; }
        public string FunctionName { get; set; }
        public Dictionary<string, object> Constructor { get; set; }
    }
}
