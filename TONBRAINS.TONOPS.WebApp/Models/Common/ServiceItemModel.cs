using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class ServiceItemModel
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string WebUrl { get; set; }

        public IEnumerable<ConfigationServiceModel> Items { get; set; }

        public IEnumerable<string> Environments { get; set; }
        public LoggingModel Logging { get; set; }
    }
}
