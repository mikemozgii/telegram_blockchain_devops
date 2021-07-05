using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Ffi.Handlers
{
    public static class TLTonCliCommandExtensions
    {

        public static string ToBrackets(this string str)
        {
            return $"\"{str}\"";
        }

        public static string ToAbiCommand(this string str)
        {
            return $"--abi {str}";
        }
        public static string ToSignCommand(this string str)
        {
            return $"--sign {str}";
        }
        public static string ToSetKeysCommand(this string str)
        {
            return $"--setkey {str}";
        }

        public static string ToToWCCommand(this int str)
        {
            return $"--wc {str}";
        }
    }
}
