using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Ffi.Models;

namespace TONBRAINS.TONOPS.Ffi
{
    public class TLTonClientx
    {
//        [DllImport("ton_client.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr request_cshart(string method_name, string params_json);

//        public string Request(string method_name, object args)
//        {
//            var options = new JsonSerializerOptions();
//            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//            var params_json = JsonSerializer.Serialize(args, options);
//            var ptr = request_cshart(method_name, params_json);
//            var str = Marshal.PtrToStringAnsi(ptr);
//            Marshal.FreeHGlobal(ptr);
//            return str;
//        }


//        public string dd = @"{
//""type"": ""Abi"",
//""value"": {
//	""ABI version"": 1,
//	""functions"": [
//		{
//			""name"": ""constructor"",
//			""inputs"": [

//            ],
//			""outputs"": [

//            ]
//    },
//		{
//			""name"": ""sendGrams"",
//			""inputs"": [
//				{""name"":""dest"",""type"":""address""},
//				{ ""name"":""amount"",""type"":""uint64""}
//			],
//			""outputs"": [

//            ]
//		}
//	],
//	""events"": [

//    ],
//	""data"": [

//    ]
//}}";


//        public TTonClientMnemonicPhraseResult GenerateAccountAddress()
//        {
//            var toobject = JsonSerializer.Deserialize<object>(dd);
//            var r = Request("abi.encode_message",
//                new
//                {
//                    abi = "{}",
//                    signer = new
//                    {
//                        type = "Keys",
//                        value = 0,
//                        keys = new
//                        {
//                            Public = "4c7c408ff1ddebb8d6405ee979c716a14fdd6cc08124107a61d3c25597099499",
//                            secret = "cc8929d635719612a9478b9cd17675a39cfad52d8959e8a177389b8c0b9122a7"
//                        }
//                    }
//                }
//                );
//            var fr = JsonSerializer.Deserialize<TTonCLientResult<TTonClientMnemonicPhraseResult>>(r);
//            return fr.result;
//        }


//        public TTonClientMnemonicPhraseResult GenerateMnemonicPhrase()
//        {
//            var r = Request("crypto.mnemonic_from_random", new { dictionary = 1, wordCount = 12 });
//            var fr = JsonSerializer.Deserialize<TTonCLientResult<TTonClientMnemonicPhraseResult>>(r);
//            return fr.result;
//        }

//        public TTonClientValidResult VerifyMnemonicPhrase(string phrase)
//        {
//            var r = Request("crypto.mnemonic_verify", new { phrase = phrase });
//            var fr = JsonSerializer.Deserialize<TTonCLientResult<TTonClientValidResult>>(r);
//            return fr.result;
//        }

//        public TTonClientKeyPairResult GenerateKeyPair(string phrase)
//        {
//            var r = Request("crypto.mnemonic_derive_sign_keys", new { phrase = phrase });
//            var options = new JsonSerializerOptions();
//            options.PropertyNameCaseInsensitive = true;
//            var fr = JsonSerializer.Deserialize<TTonCLientResult<TTonClientKeyPairResult>>(r, options);
//            return fr.result;
//        }

//        public TTonClientTonSafePubKeyResult ToTonSafeFormat(string public_key)
//        {
//            var r = Request("crypto.convert_public_key_to_ton_safe_format", new { public_key = public_key });
//            var fr = JsonSerializer.Deserialize<TTonCLientResult<TTonClientTonSafePubKeyResult>>(r);
//            return fr.result;
//        }
    }
}
