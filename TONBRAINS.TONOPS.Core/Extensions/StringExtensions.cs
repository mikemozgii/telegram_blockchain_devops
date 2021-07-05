using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        //public static string ToBrackets(this string str)
        //{
        //    return $"\"{str}\"";
        //}

        public static string ToQQ(this string str)
        {
            return $"\"{str}\"";
        }

        public static string ToVV(this string str)
        {
            return $"^^{str}^^";
        }

        public static string ToVVwithKey(this string str, string key)
        {
            return $"{key.ToVV()}:{str.ToVV()}";
        }

        public static string WithKey(this string str, string key)
        {
            return $"{key.ToVV()}:{str}";
        }


        public static string ToTvcFileName(this string str)
        {
            return $"{str}.tvc";
        }

        public static string ToAbiFileName(this string str)
        {
            return $"{str}.abi.json";
        }

        public static string ToBocFileName(this string str)
        {
            return $"{str}.boc";
        }

        public static string ToKeysFileName(this string str)
        {
            return $"{str}.keys.json";
        }
        public static string ReplaceWihtEmpty(this string str, string val)
        {
            return str.Replace(val,"");
        }

        public static string ToConfigVar(this string str)
        {
            return "${"+str+"}";
        }

        public static IEnumerable<string> ToEnumerable(this string str)
        {
           
            return new string[] { str };
        }

        public static int GetWCByAddress(this string str)
        {

            return Convert.ToInt32(str[0]);
        }

        public static string FromBase64ToTonHex(this string str)
        {
            var s = Convert.FromBase64String(str);
            string hex = BitConverter.ToString(s);
            return hex.Replace("-", ""); 
        }
        //public static string ToBase64FromTonHex(this string str)
        //{
        //    var s = Convert.FromBase64String(str);
        //    string hex = BitConverter.ToString(s);
        //    return hex.Replace("-", "");
        //}


        public static string ToBase64FromTonHex(this string input)
        {
            return System.Convert.ToBase64String(input.HexStringToHex());
        }

        public static string FixBase64(this string input)
        {
            return input.Replace("-","+").Replace("_","/");
        }

        public static byte[] HexStringToHex(this string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = System.Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }

        //18B78A4E280D56A4E9632AE0210396FA43C53D3F3C05FF8D125F6946CE268745

        public static string GenerateDbIdIfEmpty(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return IdGenerator.Generate();
            }
            return str;
        }

        public static string Base64Encode(this string  plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string  base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        //public static string FromBase64ToString(this byte[] str)
        //{
        //    string hex = BitConverter.ToString(s);
        //    var s = Convert.FromBase64String(str);

        //    return hex.Replace("-", "");
        //}



        public static string ReplaceByConfigValues(this string str)
        {

            var content = str
                .Replace("LOCALHOST_TEMP_DIR".ToConfigVar(), GlobalVarHandler.LOCALHOST_TEMP_DIR)
                .Replace("SCRIPT_DIR".ToConfigVar(), GlobalVarHandler.SCRIPT_DIR)
                .Replace("ROOT_DIR".ToConfigVar(), GlobalVarHandler.ROOT_DIR)
                .Replace("NET_TON_DEV_SRC_TOP_DIR".ToConfigVar(), GlobalVarHandler.NET_TON_DEV_SRC_TOP_DIR)
                .Replace("TON_GITHUB_REPO".ToConfigVar(), GlobalVarHandler.TON_GITHUB_REPO)
                .Replace("TON_STABLE_GITHUB_COMMIT_ID".ToConfigVar(), GlobalVarHandler.TON_STABLE_GITHUB_COMMIT_ID)
                .Replace("TON_SRC_DIR".ToConfigVar(), GlobalVarHandler.TON_SRC_DIR)
                .Replace("TON_BUILD_DIR".ToConfigVar(), GlobalVarHandler.TON_BUILD_DIR)
                .Replace("TONOS_CLI_SRC_DIR".ToConfigVar(), GlobalVarHandler.TONOS_CLI_SRC_DIR)
                .Replace("TON_WORK_DIR".ToConfigVar(), GlobalVarHandler.TON_WORK_DIR)
                .Replace("UTILS_DIR".ToConfigVar(), GlobalVarHandler.UTILS_DIR)
                .Replace("KEYS_DIR".ToConfigVar(), GlobalVarHandler.KEYS_DIR)
                .Replace("CONFIGS_DIR".ToConfigVar(), GlobalVarHandler.CONFIGS_DIR)
                .Replace("ADNL_PORT".ToConfigVar(), GlobalVarHandler.ADNL_PORT)
                .Replace("PATH".ToConfigVar(), GlobalVarHandler.PATH)
                .Replace("LITESERVER_IP".ToConfigVar(), GlobalVarHandler.LITESERVER_IP)
                .Replace("LITESERVER_PORT".ToConfigVar(), GlobalVarHandler.LITESERVER_PORT)
                .Replace("SETUP_USER".ToConfigVar(), GlobalVarHandler.SETUP_USER)
                .Replace("SETUP_GROUP".ToConfigVar(), GlobalVarHandler.SETUP_GROUP)
                .Replace("TON_WORK_STATIC_DIR".ToConfigVar(), GlobalVarHandler.TON_WORK_STATIC_DIR)
                .Replace("TON_WORK_GLOBAL_CONFIG_PATH".ToConfigVar(), GlobalVarHandler.TON_WORK_GLOBAL_CONFIG_PATH)
                .Replace("TON_WORK_DB".ToConfigVar(), GlobalVarHandler.TON_WORK_DB)
                .Replace("LOCAL_TEMP_DIR".ToConfigVar(), GlobalVarHandler.LOCAL_TEMP_DIR)
                //.Replace("TMP_DIR".ToConfigVar(), GlobalVarHandler.TMP_DIR) getting from actual host
                .Replace("TON_CRYPTO_DIR".ToConfigVar(), GlobalVarHandler.TON_CRYPTO_DIR)
                .Replace("FIFT_LIB_DIR".ToConfigVar(), GlobalVarHandler.FIFT_LIB_DIR)
                .Replace("SMARTCONT_DIR".ToConfigVar(), GlobalVarHandler.SMARTCONT_DIR)
                .Replace("VALIDATOR_ENGINE".ToConfigVar(), GlobalVarHandler.VALIDATOR_ENGINE)
                .Replace("TON_WORK_NODE_LOG".ToConfigVar(), GlobalVarHandler.TON_WORK_NODE_LOG)
                .Replace("LITE_CLIENT".ToConfigVar(), GlobalVarHandler.LITE_CLIENT)
                .Replace("LITE_SERVER_PUB".ToConfigVar(), GlobalVarHandler.LITE_SERVER_PUB)
                .Replace("FIFT".ToConfigVar(), GlobalVarHandler.FIFT)
                .Replace("CREATE_STATE".ToConfigVar(), GlobalVarHandler.CREATE_STATE)
              .Replace("TMP_DIR".ToConfigVar(), GlobalVarHandler.TMP_DIR);
            return content;
        }

    }
}
