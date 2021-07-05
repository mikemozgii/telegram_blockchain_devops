using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.DAL
{
  
    public class Token
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Logined { get; set; }
    }
}
