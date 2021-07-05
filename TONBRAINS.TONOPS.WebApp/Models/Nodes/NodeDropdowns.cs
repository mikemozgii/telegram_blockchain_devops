using TONBRAINS.TONOPS.WebApp.Common;
using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Models.Nodes
{
    /// <summary>
    /// View model for dropdowns node
    /// </summary>
    public class NodeDropdowns
    {
        /// <summary>
        /// Types node
        /// </summary>
        public IEnumerable<SelectListItem<int>> NodeTypes { get; set; }

        /// <summary>
        /// Deployment types node
        /// </summary>
        public IEnumerable<SelectListItem<int>> DeplymentTypes { get; set; }

        /// <summary>
        /// Credentials
        /// </summary>
        public IEnumerable<object> Credentials { get; set; }

        /// <summary>
        /// Operating Systems
        /// </summary>
        public IEnumerable<SelectListItem<string>> OperatingSystems { get; set; }

        /// <summary>
        /// Groups
        /// </summary>
        public IEnumerable<SelectListItem<string>> Groups { get; set; }
        public IEnumerable<SelectListItem<string>> Hosts { get; set; }

        public IEnumerable<object> ZabbixServers { get; set; }

        

    }
}
