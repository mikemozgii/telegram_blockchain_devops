using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Ffi.Models;

namespace TONBRAINS.TONOPS.Ffi
{
    public class TonWorkLib
    {
        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr genphrase();


        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr message_boc(string addr, string abi, string method, string prams, string keys, int lifetime, byte[] tvc, int tvc_len, out int vec_len, out int vec_capacity);

        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr message_boc1(string addr, string abi, string method, string prams, string keys, int lifetime, out int vec_len, out int vec_capacity);

        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void free_vec(IntPtr ptr, int le, int cap);


        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr genaddr(byte[] tvc, int tvc_len, int wc, string pubkey, string init_data, string abi);

        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr verifyseedphrase(string phrase);

        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr converttonsafe(string pubkey);

        [DllImport("tonopslib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr signderivekeys(string phrase);

        public string GenerateMnemonicPhrase()
        {
            var ptr = genphrase();
            var str = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }

        public bool VerifyMnemonicPhrase(string phrase)
        {
            var ptr = verifyseedphrase(phrase);
            var str = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return Convert.ToBoolean(str) ;
        }

        public string ToTonSafeFormat(string phrase)
        {
            var ptr = converttonsafe(phrase);
            var str = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }


        public TonKeyPair GenerateKeyPair(string phrase)
        {
            var ptr = signderivekeys(phrase);
            var str = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            var d = new TonKeyPair();
            var rs = str.Split("___");
            d.Public = rs[0];
            d.secret = rs[1];
            return d;
        }


        public string GenerateAddress(byte[] tvc, int tvc_len, int wc, string pubkey, string init_data, string abi)
        {
            var ptr = genaddr(tvc, tvc_len, wc, pubkey, init_data, abi);
            var str = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }

        public byte[] GenerateMessageBoc(string addr, string abi, string method, string prams, string keys, int lifetime)
        {
            var ptr = message_boc1(addr, abi, method, prams, keys, lifetime, out var vec_len, out var vec_capacity);
            var arr = new byte[vec_len];
            Marshal.Copy(ptr, arr, 0, vec_len);
            free_vec(ptr, vec_len, vec_capacity);
            return arr;
        }

        public byte[] GenerateMessageBoc(string addr, string abi, string method, string prams, string keys, int lifetime, byte[] tvc, int tvc_len)
        {
            var ptr = message_boc(addr, abi, method, prams, keys, lifetime, tvc, tvc_len, out var vec_len, out var vec_capacity);
            var arr = new byte[vec_len];
            Marshal.Copy(ptr, arr, 0, vec_len);
            free_vec(ptr, vec_len, vec_capacity);
            return arr;
        }

        public byte[] GenerateMessageBoc(string addr, string abi, string method, string prams, string keys, byte[] tvc)
        {
            var arr = GenerateMessageBoc(addr, abi, method, prams, keys, 3600, tvc, tvc.Length);
            return arr;
        }

        public byte[] GenerateMessageBoc(string addr, string abi, string method, string prams, string keys)
        {
            var arr = GenerateMessageBoc(addr, abi, method, prams, keys, 3600);
            return arr;
        }
    }
}
