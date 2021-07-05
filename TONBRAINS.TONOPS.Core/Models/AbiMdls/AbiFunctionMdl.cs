using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models.AbiMdls
{
    public class AbiFunctionMdl
    {
        public string name { get; set; }
        public AbiInputMdl[] inputs { get; set; }
    }
}
