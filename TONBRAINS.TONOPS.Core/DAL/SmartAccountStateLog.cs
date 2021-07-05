using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountStateLog
    {
        public string Id { get; set; }
        public string SmartAccountId { get; set; }
        public string NetworkId { get; set; }
        public string Raw { get; set; }
        public string Status { get; set; }
        public long Balance { get; set; }
        public DateTime Date { get; set; }
    }
}
