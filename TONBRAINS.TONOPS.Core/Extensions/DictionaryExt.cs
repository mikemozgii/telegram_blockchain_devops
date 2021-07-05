using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Extensions
{
    public static class DictionaryExt
    {
        public static Dictionary<string, bool> ConvertConcurrentToDic(this ConcurrentDictionary<string, bool> concurrentDictionary)
        {
            var r = concurrentDictionary.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);

            return r;
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params Dictionary<TKey, TValue>[] dictionaries)
        {
            foreach (var dict in dictionaries)
            {
                foreach (var x in dict)
                {
                    dictionary.Add(x.Key, x.Value);
                }
            }
            return dictionary;
        }

        public static ConcurrentDictionary<TKey, TValue> Merge<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, params Dictionary<TKey, TValue>[] dictionaries)
        {
            foreach (var dict in dictionaries)
            {
                foreach (var x in dict)
                {
                    dictionary.TryAdd(x.Key, x.Value);
                }
            }
            return dictionary;
        }
     
        public static Dictionary<string, IEnumerable<string>> ConvertConcurrentToDic(this ConcurrentDictionary<string, IEnumerable<string>> concurrentDictionary)
        {
            var r = concurrentDictionary.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);

            return r;
        }

        public static Dictionary<string, byte[]> ConvertConcurrentToDic(this ConcurrentDictionary<string, byte[]> concurrentDictionary)
        {
            var r = concurrentDictionary.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);

            return r;
        }


        public static Dictionary<string, IEnumerable<byte[]>> ConvertConcurrentToDic(this ConcurrentDictionary<string, IEnumerable<byte[]>> concurrentDictionary)
        {
            var r = concurrentDictionary.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);

            return r;
        }

        public static Dictionary<string, string> ConvertConcurrentToDic(this ConcurrentDictionary<string, string> concurrentDictionary)
        {
            var r = concurrentDictionary.ToDictionary(entry => entry.Key,
                                                       entry => entry.Value);

            return r;
        }

        public static Dictionary<string, bool> GetDefaultDic()
        {
            return new Dictionary<string, bool>();
        }

        public static Dictionary<string, byte[]> GetDefaultByteaDic()
        {
            return new Dictionary<string, byte[]>();
        }

        public static Dictionary<string, IEnumerable<string>> GetDefaultResultDic()
        {
            return new Dictionary<string, IEnumerable<string>>();
        }

        public static ConcurrentDictionary<string, bool> GetConcurrentDefaultDic()
        {
            return new ConcurrentDictionary<string, bool>();

        }

        public static ConcurrentDictionary<string, byte[]> GetConcurrentDefaultByteaDic()
        {
            return new ConcurrentDictionary<string, byte[]>();
        }

        public static ConcurrentDictionary<string, IEnumerable<string>> GetConcurrentDefaulResultDic()
        {
            return new ConcurrentDictionary<string, IEnumerable<string>>();
        }


        public static ConcurrentDictionary<string, string> GetConcurrentDefaulStringDic()
        {
            return new ConcurrentDictionary<string, string>();
        }

        public static ConcurrentDictionary<string, IEnumerable<byte[]>> GetConcurrentDefaulResultByteasDic()
        {
            return new ConcurrentDictionary<string, IEnumerable<byte[]>>();
        }

    }
}
