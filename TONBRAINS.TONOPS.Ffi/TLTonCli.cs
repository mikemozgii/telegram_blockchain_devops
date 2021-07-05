using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Ffi.Handlers;

namespace TONBRAINS.TONOPS.Ffi
{


    public class TLTonCli
    {
        public const string contractsdir = @"tonos-cli";
        public string Execute(string args)
        {
            string outline = "success";
            //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    WorkingDirectory = path,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    //FileName = @"C:\tonos-cli\tonos-cli\target\release\tonos-cli.exe",
                    FileName = "cmd.exe",
                    //Arguments = "/C netstat -a -n |find \"5816\" |find \"ESTABLISHED\" /c",
                    Arguments = $"/c {contractsdir} {args}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                process.Start();
                // Now read the value, parse to int and add 1 (from the original script)
                outline = process.StandardOutput.ReadToEnd();
                if (string.IsNullOrWhiteSpace(outline))
                {
                    outline = process.StandardError.ReadToEnd();
                }
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                return null;
            }

            return outline;

        }


        public string GenerateAddress(string abiPath, string tvcPath, string keysPath, int wc)
        {
            // --abi no need for this command up front of abi file
            var args = $"genaddr {tvcPath.ToBrackets()} {abiPath.ToBrackets()} {keysPath.ToBrackets().ToSetKeysCommand()} {wc.ToToWCCommand()}";
            var r = Execute(args);
            if (!string.IsNullOrWhiteSpace(r) && IsSuccess(r))
            {
                r = Regex.Matches(r, "[\n\r].*Raw address:\\s*([^\n\r]*)")[0].ToString();
                r = r.Replace("\nRaw address: ", "");
                return r.Replace("\"", "");
            }

            return null;
        }

        public string GenerateAddressSave(string abiPath, string tvcPath, string keysPath, int wc)
        {
            // --abi no need for this command up front of abi file
            var args = $"genaddr {tvcPath.ToBrackets()} {abiPath.ToBrackets()} {keysPath.ToBrackets().ToSetKeysCommand()} {wc.ToToWCCommand()} --save";
            var r = Execute(args);
            if (!string.IsNullOrWhiteSpace(r) && IsSuccess(r))
            {
                r = Regex.Matches(r, "[\n\r].*Raw address:\\s*([^\n\r]*)")[0].ToString();
                r = r.Replace("\nRaw address: ", "");
                return r.Replace("\"", "");
            }

            return null;
        }

        public bool GenerateSave(string abiPath, string tvcPath, string keysPath, int wc)
        {
            // --abi no need for this command up front of abi file
            var args = $"genaddr {tvcPath.ToBrackets()} {abiPath.ToBrackets()} {keysPath.ToBrackets().ToSetKeysCommand()} {wc.ToToWCCommand()} --save";
            var r = Execute(args);
            if (!string.IsNullOrWhiteSpace(r) && IsSuccess(r))
            {
                //r = Regex.Matches(r, "[\n\r].*Raw address:\\s*([^\n\r]*)")[0].ToString();
                //r = r.Replace("\nRaw address: ", "");
                return true;
            }

            return false;
        }

        public bool IsSuccess(string input)
        {
            //Succeded.
            return input.ToLower().Contains("success") || input.ToLower().Contains("succeed") || input.ToLower().Contains("succeded");
        }



        public string GenerateBocMsg(string address, string action, string prams, string abiPath, string keysPath, string outputPath)
        {

            var a = ConvertValsToWindows(prams);
           // var dd = "\"{}\"";
            // --abi no need for this command up front of abi file
            var args = $"message {address.ToBrackets()} {action} {a} {abiPath.ToBrackets().ToAbiCommand()} {keysPath.ToBrackets().ToSignCommand()} --raw --output {outputPath.ToBrackets()} --lifetime 3600";
           // var args = $"message {address.ToBrackets()} {action} {a} {abiPath.ToBrackets().ToAbiCommand()} --raw --output {outputPath.ToBrackets()}";
            var r = Execute(args);
            if (!string.IsNullOrWhiteSpace(r))
            {
                //var rs = r.Split(Environment.NewLine);
                //r = rs[10].Replace("\nMessage saved to file: ", "");
                return "success";
            }

            return null;
        }


        public string GenerateBocConstuctorMsg(string address, string action, string prams, string abiPath, string tvcPath, string keysPath, string outputPath)
        {

            var a = ConvertValsToWindows(prams);
            var dd = "\"{}\"";
            var args = $"message {address.ToBrackets()} {action} {dd} {abiPath.ToBrackets().ToAbiCommand()} {keysPath.ToBrackets().ToSignCommand()} --raw --output {outputPath.ToBrackets()}";
            // var args = $"message {address.ToBrackets()} {action} {a} {abiPath.ToBrackets().ToAbiCommand()} --raw --output {outputPath.ToBrackets()}";
            var r = Execute(args);
            if (!string.IsNullOrWhiteSpace(r))
            {    
                return "success";
            }

            return null;
        }

        public string ConvertValsToWindows(string input)
        {
            input = ParseQuotes(input);
            input = "\"{" + input + "}\"";
            // input = "'{" + input + "}'";
            return input;
        }

        public string ParseQuotes(string input)
        {

            return input.Replace("^^", "\\\"");
        }

    }
}
