using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class SecurityMenuModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool isGroup { get; set; }
        public string GroupId { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }

        public IList<SecurityMenuModel> Links { get; set; }
    }

}
