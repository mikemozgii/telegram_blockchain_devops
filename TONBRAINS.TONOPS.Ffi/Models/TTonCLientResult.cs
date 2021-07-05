using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Ffi.Models
{
    public class TTonCLientResult<T> where T : new()
    {
        public T result { get; set; }
    }
}
