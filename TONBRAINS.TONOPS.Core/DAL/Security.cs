
namespace TONBRAINS.TONOPS.Core.DAL
{
    public class Security
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public bool IsActive { get; set; }
        public string GroupName { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }
    }
}
