namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class CronRuleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Rule { get; set; }
        public string Command { get; set; }
        public string CronModel { get; set; }
    }
}
