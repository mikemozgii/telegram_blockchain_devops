using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models.AbiMdls
{
    public class AbiMdl
    {
        public int ABIversion { get; set; }
        public string[] header { get; set; }
        public AbiFunctionMdl[] functions { get; set; }
        public string[] data { get; set; }
        public AbiEventMdl[] events { get; set; }


    }
}
