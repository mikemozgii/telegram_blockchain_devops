using TONBRAINS.TONOPS.Core.Handlers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
namespace TONBRAINS.TONOPS.WebApp.Models
{
    public class BashScriptModel
    {
        public string Name { get; set; }

        /// <summary>
        /// Files paths to bash scripts
        /// </summary>
        public string Files { get; set; }

        public string Commands { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Type bash script
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Order execute
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Json array <see cref="Common.Enums.NodeTypes"/>
        /// </summary>
        public string NodeTypes { get; set; }

        public bool IsForNodeType (NodeTypes type)
        {
            if (string.IsNullOrEmpty(NodeTypes))
                return false;
            var types = JsonConvert.DeserializeObject<NodeTypes[]>(NodeTypes);
            //if (types.Contains(Common.Enums.NodeTypes.All))
            //    return true;
            return types.Contains(type);
        }
    }
}
