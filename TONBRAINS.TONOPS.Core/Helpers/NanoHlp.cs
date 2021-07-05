using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Helpers
{
    public class NanoHlp
    {
        public long ConvertToNanoGram(long val)
        {
            return val * 1000000000;
        }

        public long ConvertFromNanoGram(long val)
        {
            return (long)(val * 0.000000001);
        }

    }
}
