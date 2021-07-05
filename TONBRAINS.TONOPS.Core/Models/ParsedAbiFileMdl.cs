using Newtonsoft.Json;
using System.Collections.Generic;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class ParsedAbiFileMdl
    {
        [JsonProperty(PropertyName = "ABI version")]
        public long AbiVersion { get; set; }
        public List<string> Header { get; set; }
        public List<ParsedFunctionMdl> Functions { get; set; }
    }

    public class ParsedFunctionMdl
    {
        public string Name { get; set; }
        public List<ParsedInputMdl> Inputs { get; set; }
        public List<object> Outputs { get; set; }
    }

    public class ParsedInputMdl
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
