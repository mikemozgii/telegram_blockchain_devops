using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Models.AccountModels
{
    public class SetAccountSecurityModel
    {

        public string Id { get; set; }

        public IEnumerable<string> Securities { get; set; }

    }

}
